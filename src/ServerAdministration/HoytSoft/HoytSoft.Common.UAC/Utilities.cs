/*********************************************************
 * Copyright © 2008 HoytSoft. All rights reserved. 
 *	Please see included license for more details.
 *********************************************************/

namespace HoytSoft.Common.UAC
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using Microsoft.Win32;

    ///<summary>
    /// Portions courtesy ICSharpCode.UAC
    ///		http://chrison.net/UACElevationInManagedCodeGuidanceForImplementingCOMElevation.aspx
    ///</summary>
    public class Utilities
    {
        public const int ERROR_CANCELLED = 1223;
        public const string UAC_REMOTING_CHANNEL_NAME = "HoytSoft.Common.UAC.Channel";
        public const string UAC_REMOTING_CHANNEL_URI = "HoytSoft.Common.UAC.AdministrativeTask";
        public const string UAC_RUN_APPLICATION_TYPE_NAME = "HoytSoft.Common.UAC.Run.Program, HoytSoft.Common.UAC.Run";
        public static readonly Type UAC_RUN_APPLICATION_TYPE = Type.GetType(UAC_RUN_APPLICATION_TYPE_NAME);
        public static readonly string UAC_RUN_APPLICATION = UAC_RUN_APPLICATION_TYPE.Assembly.Location;

        static Utilities()
        {
            Console.WriteLine(UAC_RUN_APPLICATION);
        }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator));
        }

        public static bool IsVista()
        {
            return (Environment.OSVersion.Version.Major >= 6);
        }

        public static bool IsReallyVista()
        {
            IntPtr hmodule = Native.LoadLibrary(Native.LIB_KERNEL);

            if (hmodule != IntPtr.Zero)
            {
                //just any old API function that happens only to exist on Vista and higher
                IntPtr hProc = Native.GetProcAddress(hmodule, Native.VISTA_ONLY_API_METHOD);
                if (hProc != IntPtr.Zero)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///
        ///The possible values are:

        ///TRUE - the current process is elevated.
        ///	This value indicates that either UAC is enabled, and the process was elevated by 
        ///	the administrator, or that UAC is disabled and the process was started by a user 
        ///	who is a member of the Administrators group.

        ///FALSE - the current process is not elevated (limited).
        ///	This value indicates that either UAC is enabled, and the process was started normally, 
        ///	without the elevation, or that UAC is disabled and the process was started by a standard user. 

        /// </summary>
        /// <returns>Bool indicating whether the current process is elevated</returns>
        public static bool IsElevated()
        {
            if (!IsReallyVista())
                throw new UACException("Function requires Vista or higher");

            IntPtr hToken = IntPtr.Zero;
            IntPtr hProcess = Native.GetCurrentProcess();

            if (hProcess == IntPtr.Zero)
                throw new UACException("Error getting current process handle");

            bool bRetVal = Native.OpenProcessToken(hProcess, Native.TOKEN_QUERY, out hToken);

            if (!bRetVal)
                throw new UACException("Error opening process token");

            try
            {
                Native.TOKEN_ELEVATION te;
                te.TokenIsElevated = 0;

                int teSize = Marshal.SizeOf(te);
                IntPtr tePtr = Marshal.AllocHGlobal(teSize);
                try
                {
                    Marshal.StructureToPtr(te, tePtr, true);

                    UInt32 dwReturnLength;
                    bRetVal = Native.GetTokenInformation(hToken, Native.TOKEN_INFORMATION_CLASS.TokenElevation, tePtr, (uint)teSize, out dwReturnLength);

                    if ((!bRetVal) | (teSize != dwReturnLength))
                        throw new UACException("Error getting token information");

                    te = (Native.TOKEN_ELEVATION)Marshal.PtrToStructure(tePtr, typeof(Native.TOKEN_ELEVATION));

                }
                finally
                {
                    Marshal.FreeHGlobal(tePtr);
                }

                return (te.TokenIsElevated != 0);

            }
            finally
            {
                Native.CloseHandle(hToken);
            }

        }

        ///<summary>
        ///	TokenElevationTypeDefault - User is not using a "split" token. 
        ///	This value indicates that either UAC is disabled, or the process is started
        ///	by a standard user (not a member of the Administrators group).

        ///	The following two values can be returned only if both the UAC is enabled and
        ///	the user is a member of the Administrator's group (that is, the user has a "split" token):

        ///	TokenElevationTypeFull - the process is running elevated. 

        ///	TokenElevationTypeLimited - the process is not running elevated.
        ///</summary>
        ///<returns>TokenElevationType</returns>
        public static ElevationType GetElevationType()
        {
            if (!IsReallyVista())
                throw new UACException("Function requires Vista or higher");

            IntPtr hToken = IntPtr.Zero;
            IntPtr hProcess = Native.GetCurrentProcess();

            if (hProcess == IntPtr.Zero)
                throw new UACException("Error getting current process handle");

            bool bRetVal = Native.OpenProcessToken(hProcess, Native.TOKEN_QUERY, out hToken);


            if (!bRetVal)
                throw new UACException("Error opening process token");

            try
            {

                ElevationType tet = ElevationType.TokenElevationTypeDefault;

                uint tetSize = (uint)Marshal.SizeOf((int)tet);
                IntPtr tetPtr = Marshal.AllocHGlobal((int)tetSize);
                try
                {
                    uint dwReturnLength = 0;
                    bRetVal = Native.GetTokenInformation(hToken, Native.TOKEN_INFORMATION_CLASS.TokenElevationType, tetPtr, tetSize, out dwReturnLength);

                    if ((!bRetVal) | (tetSize != dwReturnLength))
                        throw new UACException("Error getting token information");

                    tet = (ElevationType)Marshal.ReadInt32(tetPtr);
                }
                finally
                {
                    Marshal.FreeHGlobal(tetPtr);
                }

                return tet;

            }
            finally
            {
                Native.CloseHandle(hToken);
            }

        }

        public static bool IsWOW64()
        {
            IntPtr processHandle = Native.GetCurrentProcess();
            if (processHandle == IntPtr.Zero)
            {
                throw new UACException("Error getting current process handle");
            }

            IntPtr moduleHandle = Native.GetModuleHandle(Native.LIB_KERNEL);
            if (moduleHandle == IntPtr.Zero)
            {
                throw new UACException("Error getting module handle");
            }

            IntPtr funcIsWow64 = Native.GetProcAddress(moduleHandle, Native.IS_WOW_64_PROCESS);
            var func = (Native.IsWow64ProcessDelegate)Marshal.GetDelegateForFunctionPointer(funcIsWow64, typeof(Native.IsWow64ProcessDelegate));

            bool bIsWOW64;
            if (!func(processHandle, out bIsWOW64))
            {
                throw new UACException("Error discovering WOW64 status");
            }

            return bIsWOW64;
        }

        public static Platform GetPlatformType()
        {
            try
            {
                var sysInfo = new Native.SYSTEM_INFO();
                Native.GetNativeSystemInfo(ref sysInfo);

                switch (sysInfo.wProcessorArchitecture)
                {
                    case Native.PROCESSOR_ARCHITECTURE_AMD64:
                    case Native.PROCESSOR_ARCHITECTURE_IA64:
                        return Platform.X64;
                    case Native.PROCESSOR_ARCHITECTURE_INTEL:
                        return Platform.X86;
                    default:
                        return Platform.Unknown;
                }
            }
            catch
            {
                return Platform.Unknown;
            }
        }

        public static bool Is64Bit()
        {
            return (GetPlatformType() == Platform.X64);
        }

        public static bool Is32Bit()
        {
            return (GetPlatformType() != Platform.X64);
        }

        public static Process CreateDefaultProcess(string fileName)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                                {
                                    FileName = fileName,
                                    CreateNoWindow = true,
                                    UseShellExecute = true
                                },
                EnableRaisingEvents = true
            };

            // Must enable Exited event for both sync and async scenarios

            return process;
        }

        public static bool RunElevatedAsync(string fileName)
        {
            return RunElevatedAsync(fileName, string.Empty, null);
        }

        public static bool RunElevatedAsync(string fileName, string arguments)
        {
            return RunElevatedAsync(fileName, arguments, null);
        }

        public static bool RunElevatedAsync(string fileName, string arguments, string workingDirectory)
        {
            using (Process p = CreateDefaultProcess(fileName))
            {
                p.StartInfo.Verb = "runas";
                p.StartInfo.Arguments = arguments;
                if (workingDirectory != null)
                {
                    p.StartInfo.WorkingDirectory = workingDirectory;
                }

                try
                {
                    return p.Start();
                }
                catch (Win32Exception)
                {
                    return false;
                }
            }
        }

        public static bool RunElevated(string fileName)
        {
            return RunElevated(fileName, string.Empty, null);
        }

        public static bool RunElevated(string fileName, string arguments)
        {
            return RunElevated(fileName, string.Empty, null);
        }

        public static bool RunElevated(string fileName, string arguments, string workingDirectory)
        {
            using (Process p = CreateDefaultProcess(fileName))
            {
                p.StartInfo.Verb = "runas";
                p.StartInfo.Arguments = arguments;
                if (workingDirectory != null)
                {
                    p.StartInfo.WorkingDirectory = workingDirectory;
                }

                try
                {
                    if (!p.Start())
                    {
                        return false;
                    }

                    p.WaitForExit();
                    return (p.ExitCode == 0);
                }
                catch (Win32Exception)
                {
                    return false;
                }
            }
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public static object LaunchElevatedCOMObject(Guid Clsid, Guid InterfaceID)
        {
            string CLSID = Clsid.ToString("B"); // B formatting directive: returns {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx} 
            string monikerName = "Elevation:Administrator!new:" + CLSID;

            var bo = new Native.BIND_OPTS3();
            bo.cbStruct = (uint)Marshal.SizeOf(bo);
            bo.hwnd = IntPtr.Zero;
            bo.dwClassContext = (int)Native.CLSCTX.CLSCTX_LOCAL_SERVER;

            object retVal = Native.CoGetObject(monikerName, ref bo, InterfaceID);

            return (retVal);
        }

        public static void RegisterForElevation(string assemblyLocation, string classToElevate, string appId, int localizedStringId)
        {
            if (!IsReallyVista())
            {
                return;
            }

            // [HKEY_CLASSES_ROOT\CLSID\{71E050A7-AF7F-42dd-BE00-BF955DDD13D4}]
            // "AppID"="{75AB90B0-8B9C-45c9-AC55-C53A9D718E1A}"
            // "LocalizedString"="@E:\\Daten\\Firma\\Konferenzen und Talks\\VSone 2007\\UAC\\Samples\\ConsumeMyElevatedCOM\\ManagedElevator\\bin\\Debug\\ManagedElevator.dll,-100"
            RegistryKey classKey = Registry.ClassesRoot.OpenSubKey(@"CLSID\{" + classToElevate + "}", true);
            classKey.SetValue("AppId", "{" + appId + "}", RegistryValueKind.String);
            classKey.SetValue("LocalizedString", "@" + assemblyLocation + ",-" + localizedStringId.ToString(), RegistryValueKind.String);

            // [HKEY_CLASSES_ROOT\CLSID\{71E050A7-AF7F-42dd-BE00-BF955DDD13D4}\Elevation]
            // "Enabled"=dword:00000001
            RegistryKey elevationKey = classKey.CreateSubKey("Elevation");
            elevationKey.SetValue("Enabled", 1, RegistryValueKind.DWord);
            elevationKey.Close();

            classKey.Close();

            // [HKEY_CLASSES_ROOT\AppID\{75AB90B0-8B9C-45c9-AC55-C53A9D718E1A}]
            // @="ManagedElevator"
            // "DllSurrogate"=""
            RegistryKey hkcrappId = Registry.ClassesRoot.OpenSubKey("AppID", true);
            RegistryKey appIdKey = hkcrappId.CreateSubKey("{" + appId + "}");
            appIdKey.SetValue(null, Path.GetFileNameWithoutExtension(assemblyLocation));
            appIdKey.SetValue("DllSurrogate", "", RegistryValueKind.String);
            appIdKey.Close();

            // [HKEY_CLASSES_ROOT\AppID\ManagedElevator.dll]
            // "AppID"="{75AB90B0-8B9C-45c9-AC55-C53A9D718E1A}"
            RegistryKey asmKey = hkcrappId.CreateSubKey(Path.GetFileName(assemblyLocation));
            asmKey.SetValue("AppID", "{" + appId + "}", RegistryValueKind.String);
            asmKey.Close();

            hkcrappId.Close();
        }

        public static void UnRegisterFromElevation(string assemblyLocation, string appId)
        {
            if (!IsReallyVista())
            {
                return;
            }

            RegistryKey hkcrappId = Registry.ClassesRoot.OpenSubKey("AppID", true);
            hkcrappId.DeleteSubKey("{" + appId + "}");
            hkcrappId.DeleteSubKey(Path.GetFileName(assemblyLocation));
            hkcrappId.Close();
        }

        private const int FACILITY_WIN32 = 7;

        // From macro in winerror.h:
        // #define __HRESULT_FROM_WIN32(x) 
        //     ((HRESULT)(x) <= 0 ? ((HRESULT)(x)) : ((HRESULT) (((x) & 0x0000FFFF) | (FACILITY_WIN32 << 16) | 0x80000000)))
        public static int HResultFromWin32(int win32ErrorCode)
        {
            if (win32ErrorCode > 0)
            {
                win32ErrorCode = (int)((win32ErrorCode & (uint)0x0000FFFF) | (FACILITY_WIN32 << 16) | 0x80000000);
            }
            
            return win32ErrorCode;
        }

        public static bool IsCancelled(int Win32ErrorCode)
        {
            return HResultFromWin32(Win32ErrorCode) == ERROR_CANCELLED;
        }

        public static bool IsCancelled(COMException e)
        {
            return IsCancelled(e.ErrorCode);
        }
    }
}
