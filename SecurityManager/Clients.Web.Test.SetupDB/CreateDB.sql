USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = '<Database>')
  DROP DATABASE <Database>
GO  

CREATE DATABASE <Database>
GO

ALTER DATABASE <Database> SET RECOVERY SIMPLE
GO