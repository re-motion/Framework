USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'RemotionDataDomainObjectsUberProfIntegrationTestDomain')
BEGIN
  ALTER DATABASE RemotionDataDomainObjectsUberProfIntegrationTestDomain SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE RemotionDataDomainObjectsUberProfIntegrationTestDomain
END
GO

CREATE DATABASE RemotionDataDomainObjectsUberProfIntegrationTestDomain
ON PRIMARY (
	Name = 'RemotionDataDomainObjectsUberProfIntegrationTestDomain_Data',
	Filename = 'C:\Databases\RemotionDataDomainObjectsUberProfIntegrationTestDomain.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'RemotionDataDomainObjectsUberProfIntegrationTestDomain_Log',
	Filename = 'C:\Databases\RemotionDataDomainObjectsUberProfIntegrationTestDomain.ldf',
	Size = 10MB
)
GO

ALTER DATABASE RemotionDataDomainObjectsUberProfIntegrationTestDomain SET RECOVERY SIMPLE
GO
