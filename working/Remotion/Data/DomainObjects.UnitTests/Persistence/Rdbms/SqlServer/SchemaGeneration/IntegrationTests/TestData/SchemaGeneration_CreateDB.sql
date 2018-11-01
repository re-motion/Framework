USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'DBPrefix_SchemaGenerationTestDomain1')
BEGIN
  ALTER DATABASE DBPrefix_SchemaGenerationTestDomain1 SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE DBPrefix_SchemaGenerationTestDomain1
END
GO
IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'DBPrefix_SchemaGenerationTestDomain2')
BEGIN
  ALTER DATABASE DBPrefix_SchemaGenerationTestDomain2 SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE DBPrefix_SchemaGenerationTestDomain2
END
GO
IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'DBPrefix_SchemaGenerationTestDomain3')
BEGIN
  ALTER DATABASE DBPrefix_SchemaGenerationTestDomain3 SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE DBPrefix_SchemaGenerationTestDomain3
END
GO
  
CREATE DATABASE DBPrefix_SchemaGenerationTestDomain1
ON PRIMARY (
	Name = 'DBPrefix_SchemaGenerationTestDomain1_Data',
	Filename = 'C:\Databases\DBPrefix_SchemaGenerationTestDomain1.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'DBPrefix_SchemaGenerationTestDomain1_Log',
	Filename = 'C:\Databases\DBPrefix_SchemaGenerationTestDomain1.ldf',
	Size = 10MB	
)
GO

CREATE DATABASE DBPrefix_SchemaGenerationTestDomain2
ON PRIMARY (
	Name = 'DBPrefix_SchemaGenerationTestDomain2_Data',
	Filename = 'C:\Databases\DBPrefix_SchemaGenerationTestDomain2.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'DBPrefix_SchemaGenerationTestDomain2_Log',
	Filename = 'C:\Databases\DBPrefix_SchemaGenerationTestDomain2.ldf',
	Size = 10MB	
)
GO

CREATE DATABASE DBPrefix_SchemaGenerationTestDomain3
ON PRIMARY (
	Name = 'DBPrefix_SchemaGenerationTestDomain3_Data',
	Filename = 'C:\Databases\DBPrefix_SchemaGenerationTestDomain3.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'DBPrefix_SchemaGenerationTestDomain3_Log',
	Filename = 'C:\Databases\DBPrefix_SchemaGenerationTestDomain3.ldf',
	Size = 10MB	
)
GO

ALTER DATABASE DBPrefix_SchemaGenerationTestDomain1 SET RECOVERY SIMPLE
ALTER DATABASE DBPrefix_SchemaGenerationTestDomain2 SET RECOVERY SIMPLE