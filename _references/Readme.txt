This directory contains third party assemblies that may be used for WheelMUD.

Some information should be added here for any references added to this folder.
In order to take updates to new references, it can be a pain to track down all 
the locations that utilize the reference for testing them all appropriately, 
etc.  So please consolidate some dependency info here, including for example:
* Which modules use it (or if it is believed unused, call attention to this).
* What it does and/or why we have a dependency on it.
* Where one would look 
for new versions/support/etc (website or whatnot).
* Source folder location (if we manage a modified branch of it).
* The name of the dependency's licensing structure.
* What version(s) we are using here.
* Any other short comments that might be relevant.

Antlr3.Runtime.dll
* Used by FluentNHibernate.dll
* Current version is 3.1.3.42154

Castle.Core.dll
* Used by FluentNhibernate.dll
* Current version is 1.1.0

Castle.DynamicProxy2.dll
* Used by FluentNhibernate.dll
* Current version is 2.1.0.0

CSScriptLibrary.dll
* Used by WheelMUD.GameEngine
* Provides a scripting environment to run *.cs files without having to
  compile them first.
* Project website is at http://www.csscript.net
* Current version is 2.7.0.0

FluentNHibernate.dll
* Used by WheelMUD.Data
* Current version is 1.0.0.593

ICSharpCode.SharpZipLib.dll:
* Used by WheelMUD.Server
* Provides Deflate and Gzip facilities to the Mud Compression Protocol (MCP).

Iesi.Collections.dll
* Used by FluentNhibernate.dll
* File version is 3.0.0.1001
* Product version is 1.0

log4net.dll:
* Provides logging functions to code.
* Project is hosted at http://logging.apache.org/log4net
* Current version is 1.2.10.0

MySql.Data.dll:
* Used by NHibernate
* Provides the Data namespace for MySQL databases.

NHibernate.dll
* Used by FluentNhibernate.dll
* Current version is 3.0.0.1001

NHibernate.ByteCode.Castle.dll
* Used by NHibernate.dll
* Current version is 2.1.4000

NHibernate.Linq
* Used by NHibernate.dll
* Current version is 1.0.0.0

Nini.dll:
* Used in the WheelMUD.configuration project.
* Used for reading and writing ini and xml config files.
* Source is currently at http://sourceforge.net/projects/nini
* Current version is 1.1
* This does not cause file locks like the code in System.Configuration namespace.

nunit.framework.dll:
* Used by WheelMUD.Tests
* Provides unit testing features.
* Binaries being hosted at http://www.nunit.org
* Current version is 2.2.9.0

NVelocity.dll:
* Used by WheelMUD.Core
* Facilitates the text format and layout for rooms descriptions.
* This is a .NET port of the Java Jakarta Velocity templeting engine.
* Project and code is housed http://nvelocity.codeplex.com/
* Sample code usage can be seen http://www.codeproject.com/KB/aspnet/nvelocityaspnet.aspx

Rhino.Mocks.dll:
* Used by WheelMUD.Tests
* Provides mocking facilities for the unit tests.
* Current version is 3.4.0.1210

Remotion.Data.Linq.dll
* Used by NHibernate.Linq.dll
* Current version is 1.13.41.2

System.Data.SQLite.DLL:
* Used by NHibernate
* Provides the Data namespace for SQLite databases.
* Current version is 1.0.66.0

System.Data.SqlServerCe.dll
* Used by NHibernate
* Provides the Data namespace for SQL Server Compact Edition databases.
* Current version is 3.5.5365.0
