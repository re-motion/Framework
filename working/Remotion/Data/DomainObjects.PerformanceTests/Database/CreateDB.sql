USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'PerformanceTestDomain')
  DROP DATABASE PerformanceTestDomain
GO  
  
CREATE DATABASE PerformanceTestDomain
ON PRIMARY (
	Name = 'PerformanceTestDomain_Data',
	Filename = 'C:\Databases\PerformanceTestDomain.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'PerformanceTestDomain_Log',
	Filename = 'C:\Databases\PerformanceTestDomain.ldf',
	Size = 10MB	
)
GO

ALTER DATABASE PerformanceTestDomain SET RECOVERY SIMPLE
GO