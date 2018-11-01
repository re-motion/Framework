USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'DBPrefix_RemotionDataDomainObjectsUberProfIntegrationTestDomain')
BEGIN
  ALTER DATABASE DBPrefix_RemotionDataDomainObjectsUberProfIntegrationTestDomain SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE DBPrefix_RemotionDataDomainObjectsUberProfIntegrationTestDomain
END
GO

CREATE DATABASE DBPrefix_RemotionDataDomainObjectsUberProfIntegrationTestDomain
ON PRIMARY (
	Name = 'DBPrefix_RemotionDataDomainObjectsUberProfIntegrationTestDomain_Data',
	Filename = 'C:\Databases\DBPrefix_RemotionDataDomainObjectsUberProfIntegrationTestDomain.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'DBPrefix_RemotionDataDomainObjectsUberProfIntegrationTestDomain_Log',
	Filename = 'C:\Databases\DBPrefix_RemotionDataDomainObjectsUberProfIntegrationTestDomain.ldf',
	Size = 10MB
)
GO

ALTER DATABASE DBPrefix_RemotionDataDomainObjectsUberProfIntegrationTestDomain SET RECOVERY SIMPLE
GO
