USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'RemotionSecurityManager')
BEGIN
  ALTER DATABASE RemotionSecurityManager SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE RemotionSecurityManager
END
GO

CREATE DATABASE RemotionSecurityManager
ON PRIMARY (
	Name = 'RemotionSecurityManager_Data',
	Filename = 'C:\Databases\RemotionSecurityManager.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'RemotionSecurityManager_Log',
	Filename = 'C:\Databases\RemotionSecurityManager.ldf',
	Size = 10MB
)
GO

ALTER DATABASE RemotionSecurityManager SET RECOVERY SIMPLE
GO
