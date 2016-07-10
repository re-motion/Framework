USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'DBPrefix_TestDomain')
BEGIN
  ALTER DATABASE DBPrefix_TestDomain SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE DBPrefix_TestDomain
END
GO
  
CREATE DATABASE DBPrefix_TestDomain
ON PRIMARY (
	Name = 'DBPrefix_TestDomain_Data',
	Filename = 'C:\Databases\DBPrefix_TestDomain.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'DBPrefix_TestDomain_Log',
	Filename = 'C:\Databases\DBPrefix_TestDomain.ldf',
	Size = 10MB	
)
GO

ALTER DATABASE DBPrefix_TestDomain SET RECOVERY SIMPLE
