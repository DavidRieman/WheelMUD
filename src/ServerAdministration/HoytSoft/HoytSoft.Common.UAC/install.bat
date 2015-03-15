@echo off
set OLDDIR=%CD%
set MYDIR=%~dp0

cd /d %MYDIR%

IF "%PROCESSOR_ARCHITECTURE%"=="x86" (
	goto x86
) ELSE (
	goto x64
)

:x86
regsvr32 "%MYDIR%\bin\Debug\HoytSoft.Common.UAC.dll"
goto END

:x64
echo Do 64-bit version here
goto END

:END
cd /d %OLDDIR% &rem restore current directory
@echo on