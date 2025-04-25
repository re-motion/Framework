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
GO

CREATE DATABASE DBPrefix_SchemaGenerationTestDomain2
GO

CREATE DATABASE DBPrefix_SchemaGenerationTestDomain3
GO

ALTER DATABASE DBPrefix_SchemaGenerationTestDomain1 SET RECOVERY SIMPLE
ALTER DATABASE DBPrefix_SchemaGenerationTestDomain2 SET RECOVERY SIMPLE