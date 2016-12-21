@echo off
setlocal
set buildconf=Debug
if not "%1"=="" set buildconf=%1
echo Running .\..\..\Remotion\Data\DomainObjects.RdbmsTools\bin\%buildconf%\dbschema.exe from %CD%...

.\..\..\Remotion\Data\DomainObjects.RdbmsTools\bin\%buildconf%\dbschema.exe ^
    "/baseDirectory:bin\%buildconf%" ^
    "/config:..\Clients.Web\web.config" ^
    "/schema" ^
    "/schemaDirectory:Database"

if not %ERRORLEVEL%==0 goto dbschema_error
echo.

del Database\SecurityManagerSetupDB.sql 2> NUL
ren Database\SetupDB.sql SecurityManagerSetupDB.sql
del Database\SecurityManagerTearDownDB.sql 2> NUL
ren Database\TearDownDB.sql SecurityManagerTearDownDB.sql

if not %ERRORLEVEL%==0 goto ren_error

goto end

:dbschema_error
echo.
echo There was an error running dbschema.exe.
exit /b 1

:ren_error
echo.
echo There was an error renaming SetupDB.sql to SecurityManagerSetupDB.sql or renaming TearDownDB.sql to SecurityManagerTearDownDB.sql
exit /b 2

:end

exit /b 0