USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'PerformanceTestDomain')
  DROP DATABASE PerformanceTestDomain
GO  
  
CREATE DATABASE PerformanceTestDomain
GO

ALTER DATABASE PerformanceTestDomain SET RECOVERY SIMPLE
GO