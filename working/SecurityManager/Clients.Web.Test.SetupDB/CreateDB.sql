USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = '<Database>')
  DROP DATABASE <Database>
GO  

CREATE DATABASE <Database>
ON PRIMARY (
	Name = '<Database>_Data',
	Filename = '<DatabaseFilesPath>\<Database>.mdf',
	Size = 10MB
)
LOG ON (
	Name = '<Database>_Log',
	Filename = '<DatabaseFilesPath>\<Database>.ldf',
	Size = 10MB
)
GO

ALTER DATABASE <Database> SET RECOVERY SIMPLE
GO