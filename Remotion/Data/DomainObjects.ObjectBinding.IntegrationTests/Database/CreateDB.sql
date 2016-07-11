USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'DBPrefix_RemotionDataDomainObjectsObjectBindingIntegrationTestDomain')
BEGIN
  ALTER DATABASE DBPrefix_RemotionDataDomainObjectsObjectBindingIntegrationTestDomain SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE DBPrefix_RemotionDataDomainObjectsObjectBindingIntegrationTestDomain
END
GO

CREATE DATABASE DBPrefix_RemotionDataDomainObjectsObjectBindingIntegrationTestDomain
ON PRIMARY (
	Name = 'DBPrefix_RemotionDataDomainObjectsObjectBindingIntegrationTestDomain_Data',
	Filename = 'C:\Databases\DBPrefix_RemotionDataDomainObjectsObjectBindingIntegrationTestDomain.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'DBPrefix_RemotionDataDomainObjectsObjectBindingIntegrationTestDomain_Log',
	Filename = 'C:\Databases\DBPrefix_RemotionDataDomainObjectsObjectBindingIntegrationTestDomain.ldf',
	Size = 10MB
)
GO

ALTER DATABASE DBPrefix_RemotionDataDomainObjectsObjectBindingIntegrationTestDomain SET RECOVERY SIMPLE
GO
