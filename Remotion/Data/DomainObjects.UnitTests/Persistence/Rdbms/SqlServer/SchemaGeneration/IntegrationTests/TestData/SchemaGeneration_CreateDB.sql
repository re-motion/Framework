USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'SchemaGenerationTestDomain1')
BEGIN
  ALTER DATABASE SchemaGenerationTestDomain1 SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE SchemaGenerationTestDomain1
END
GO
IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'SchemaGenerationTestDomain2')
BEGIN
  ALTER DATABASE SchemaGenerationTestDomain2 SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE SchemaGenerationTestDomain2
END
GO
IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'SchemaGenerationTestDomain3')
BEGIN
  ALTER DATABASE SchemaGenerationTestDomain3 SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE SchemaGenerationTestDomain3
END
GO
  
CREATE DATABASE SchemaGenerationTestDomain1
ON PRIMARY (
	Name = 'SchemaGenerationTestDomain1_Data',
	Filename = 'C:\Databases\SchemaGenerationTestDomain1.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'SchemaGenerationTestDomain1_Log',
	Filename = 'C:\Databases\SchemaGenerationTestDomain1.ldf',
	Size = 10MB	
)
GO

CREATE DATABASE SchemaGenerationTestDomain2
ON PRIMARY (
	Name = 'SchemaGenerationTestDomain2_Data',
	Filename = 'C:\Databases\SchemaGenerationTestDomain2.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'SchemaGenerationTestDomain2_Log',
	Filename = 'C:\Databases\SchemaGenerationTestDomain2.ldf',
	Size = 10MB	
)
GO

CREATE DATABASE SchemaGenerationTestDomain3
ON PRIMARY (
	Name = 'SchemaGenerationTestDomain3_Data',
	Filename = 'C:\Databases\SchemaGenerationTestDomain3.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'SchemaGenerationTestDomain3_Log',
	Filename = 'C:\Databases\SchemaGenerationTestDomain3.ldf',
	Size = 10MB	
)
GO

ALTER DATABASE SchemaGenerationTestDomain1 SET RECOVERY SIMPLE
ALTER DATABASE SchemaGenerationTestDomain2 SET RECOVERY SIMPLE