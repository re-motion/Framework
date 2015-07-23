USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'RemotionDataDomainObjectsObjectBindingIntegrationTestDomain')
BEGIN
  ALTER DATABASE RemotionDataDomainObjectsObjectBindingIntegrationTestDomain SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE RemotionDataDomainObjectsObjectBindingIntegrationTestDomain
END
GO

CREATE DATABASE RemotionDataDomainObjectsObjectBindingIntegrationTestDomain
ON PRIMARY (
	Name = 'RemotionDataDomainObjectsObjectBindingIntegrationTestDomain_Data',
	Filename = 'C:\Databases\RemotionDataDomainObjectsObjectBindingIntegrationTestDomain.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'RemotionDataDomainObjectsObjectBindingIntegrationTestDomain_Log',
	Filename = 'C:\Databases\RemotionDataDomainObjectsObjectBindingIntegrationTestDomain.ldf',
	Size = 10MB
)
GO

ALTER DATABASE RemotionDataDomainObjectsObjectBindingIntegrationTestDomain SET RECOVERY SIMPLE
GO
