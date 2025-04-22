USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'RemotionSecurityManager')
BEGIN
  ALTER DATABASE RemotionSecurityManager SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE RemotionSecurityManager
END
GO

CREATE DATABASE RemotionSecurityManager
GO

ALTER DATABASE RemotionSecurityManager SET RECOVERY SIMPLE
GO
