USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'DBPrefix_RemotionDataDomainObjectsWebIntegrationTestDomain')
BEGIN
  ALTER DATABASE DBPrefix_RemotionDataDomainObjectsWebIntegrationTestDomain SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE DBPrefix_RemotionDataDomainObjectsWebIntegrationTestDomain
END
GO

CREATE DATABASE DBPrefix_RemotionDataDomainObjectsWebIntegrationTestDomain
ON PRIMARY (
	Name = 'DBPrefix_RemotionDataDomainObjectsWebIntegrationTestDomain_Data',
	Filename = 'C:\Databases\DBPrefix_RemotionDataDomainObjectsWebIntegrationTestDomain.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'DBPrefix_RemotionDataDomainObjectsWebIntegrationTestDomain_Log',
	Filename = 'C:\Databases\DBPrefix_RemotionDataDomainObjectsWebIntegrationTestDomain.ldf',
	Size = 10MB
)
GO

ALTER DATABASE DBPrefix_RemotionDataDomainObjectsWebIntegrationTestDomain SET RECOVERY SIMPLE
GO
