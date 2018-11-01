USE PerformanceTestDomain
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('Client', 'File', 'Person', 'Company')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithFewValuePropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithFewValuePropertiesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithRelationPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithRelationPropertiesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithValuePropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithValuePropertiesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClientView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClientView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClientBoundBaseClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClientBoundBaseClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CompanyView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FileView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[FileView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OppositeClassWithAnonymousRelationPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OppositeClassWithAnonymousRelationPropertiesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OppositeClassWithCollectionRelationPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OppositeClassWithCollectionRelationPropertiesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OppositeClassWithRealRelationPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OppositeClassWithRealRelationPropertiesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OppositeClassWithVirtualRelationPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OppositeClassWithVirtualRelationPropertiesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PersonView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PersonView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('ClassWithFewValueProperties', 'ClassWithRelationProperties', 'ClassWithValueProperties', 'Client', 'Company', 'File', 'OppositeClassWithAnonymousRelationProperties', 'OppositeClassWithCollectionRelationProperties', 'OppositeClassWithRealRelationProperties', 'OppositeClassWithVirtualRelationProperties', 'Person')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithFewValueProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithFewValueProperties]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithRelationProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithRelationProperties]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithValueProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithValueProperties]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Client' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Client]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Company' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Company]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'File' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[File]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OppositeClassWithAnonymousRelationProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[OppositeClassWithAnonymousRelationProperties]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OppositeClassWithCollectionRelationProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[OppositeClassWithCollectionRelationProperties]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OppositeClassWithRealRelationProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[OppositeClassWithRealRelationProperties]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OppositeClassWithVirtualRelationProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[OppositeClassWithVirtualRelationProperties]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Person' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Person]
GO

-- Create all tables
CREATE TABLE [dbo].[ClassWithFewValueProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClassWithFewValueProperties columns
  [IntProperty1] int NOT NULL,
  [IntProperty2] int NOT NULL,
  [IntProperty3] int NOT NULL,
  [IntProperty4] int NOT NULL,
  [IntProperty5] int NOT NULL,
  [DateTimeProperty1] datetime NOT NULL,
  [DateTimeProperty2] datetime NOT NULL,
  [DateTimeProperty3] datetime NOT NULL,
  [DateTimeProperty4] datetime NOT NULL,
  [DateTimeProperty5] datetime NOT NULL,
  [StringProperty1] nvarchar (max) NULL,
  [StringProperty2] nvarchar (max) NULL,
  [StringProperty3] nvarchar (max) NULL,
  [StringProperty4] nvarchar (max) NULL,
  [StringProperty5] nvarchar (max) NULL,
  [BoolProperty1] bit NOT NULL,
  [BoolProperty2] bit NOT NULL,
  [BoolProperty3] bit NOT NULL,
  [BoolProperty4] bit NOT NULL,
  [BoolProperty5] bit NOT NULL,

  CONSTRAINT [PK_ClassWithFewValueProperties] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[ClassWithRelationProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClassWithRelationProperties columns
  [Unary1ID] uniqueidentifier NULL,
  [Unary2ID] uniqueidentifier NULL,
  [Unary3ID] uniqueidentifier NULL,
  [Unary4ID] uniqueidentifier NULL,
  [Unary5ID] uniqueidentifier NULL,
  [Unary6ID] uniqueidentifier NULL,
  [Unary7ID] uniqueidentifier NULL,
  [Unary8ID] uniqueidentifier NULL,
  [Unary9ID] uniqueidentifier NULL,
  [Unary10ID] uniqueidentifier NULL,
  [Real1ID] uniqueidentifier NULL,
  [Real2ID] uniqueidentifier NULL,
  [Real3ID] uniqueidentifier NULL,
  [Real4ID] uniqueidentifier NULL,
  [Real5ID] uniqueidentifier NULL,
  [Real6ID] uniqueidentifier NULL,
  [Real7ID] uniqueidentifier NULL,
  [Real8ID] uniqueidentifier NULL,
  [Real9ID] uniqueidentifier NULL,
  [Real10ID] uniqueidentifier NULL,

  CONSTRAINT [PK_ClassWithRelationProperties] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[ClassWithValueProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClassWithValueProperties columns
  [IntProperty1] int NOT NULL,
  [IntProperty2] int NOT NULL,
  [IntProperty3] int NOT NULL,
  [IntProperty4] int NOT NULL,
  [IntProperty5] int NOT NULL,
  [IntProperty6] int NOT NULL,
  [IntProperty7] int NOT NULL,
  [IntProperty8] int NOT NULL,
  [IntProperty9] int NOT NULL,
  [IntProperty10] int NOT NULL,
  [DateTimeProperty1] datetime NOT NULL,
  [DateTimeProperty2] datetime NOT NULL,
  [DateTimeProperty3] datetime NOT NULL,
  [DateTimeProperty4] datetime NOT NULL,
  [DateTimeProperty5] datetime NOT NULL,
  [DateTimeProperty6] datetime NOT NULL,
  [DateTimeProperty7] datetime NOT NULL,
  [DateTimeProperty8] datetime NOT NULL,
  [DateTimeProperty9] datetime NOT NULL,
  [DateTimeProperty10] datetime NOT NULL,
  [StringProperty1] nvarchar (max) NULL,
  [StringProperty2] nvarchar (max) NULL,
  [StringProperty3] nvarchar (max) NULL,
  [StringProperty4] nvarchar (max) NULL,
  [StringProperty5] nvarchar (max) NULL,
  [StringProperty6] nvarchar (max) NULL,
  [StringProperty7] nvarchar (max) NULL,
  [StringProperty8] nvarchar (max) NULL,
  [StringProperty9] nvarchar (max) NULL,
  [StringProperty10] nvarchar (max) NULL,
  [BoolProperty1] bit NOT NULL,
  [BoolProperty2] bit NOT NULL,
  [BoolProperty3] bit NOT NULL,
  [BoolProperty4] bit NOT NULL,
  [BoolProperty5] bit NOT NULL,
  [BoolProperty6] bit NOT NULL,
  [BoolProperty7] bit NOT NULL,
  [BoolProperty8] bit NOT NULL,
  [BoolProperty9] bit NOT NULL,
  [BoolProperty10] bit NOT NULL,

  CONSTRAINT [PK_ClassWithValueProperties] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Client]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Client columns
  [Name] nvarchar (100) NOT NULL,

  CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Company]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClientBoundBaseClass columns
  [ClientID] uniqueidentifier NULL,

  -- Company columns
  [Name] nvarchar (100) NOT NULL,

  CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[File]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- File columns
  [Number] nvarchar (100) NOT NULL,
  [ClientID] uniqueidentifier NULL,

  CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[OppositeClassWithAnonymousRelationProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- OppositeClassWithAnonymousRelationProperties columns

  CONSTRAINT [PK_OppositeClassWithAnonymousRelationProperties] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[OppositeClassWithCollectionRelationProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- OppositeClassWithCollectionRelationProperties columns
  [EndOfCollectionID] uniqueidentifier NULL,

  CONSTRAINT [PK_OppositeClassWithCollectionRelationProperties] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[OppositeClassWithRealRelationProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- OppositeClassWithRealRelationProperties columns
  [Real1ID] uniqueidentifier NULL,
  [Real2ID] uniqueidentifier NULL,
  [Real3ID] uniqueidentifier NULL,
  [Real4ID] uniqueidentifier NULL,
  [Real5ID] uniqueidentifier NULL,
  [Real6ID] uniqueidentifier NULL,
  [Real7ID] uniqueidentifier NULL,
  [Real8ID] uniqueidentifier NULL,
  [Real9ID] uniqueidentifier NULL,
  [Real10ID] uniqueidentifier NULL,

  CONSTRAINT [PK_OppositeClassWithRealRelationProperties] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[OppositeClassWithVirtualRelationProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- OppositeClassWithVirtualRelationProperties columns

  CONSTRAINT [PK_OppositeClassWithVirtualRelationProperties] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Person]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClientBoundBaseClass columns
  [ClientID] uniqueidentifier NULL,

  -- Person columns
  [FirstName] nvarchar (100) NOT NULL,
  [LastName] nvarchar (100) NOT NULL,

  CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
ALTER TABLE [dbo].[ClassWithRelationProperties] ADD
  CONSTRAINT [FK_ClassWithRelationProperties_Unary1ID] FOREIGN KEY ([Unary1ID]) REFERENCES [dbo].[OppositeClassWithAnonymousRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Unary2ID] FOREIGN KEY ([Unary2ID]) REFERENCES [dbo].[OppositeClassWithAnonymousRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Unary3ID] FOREIGN KEY ([Unary3ID]) REFERENCES [dbo].[OppositeClassWithAnonymousRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Unary4ID] FOREIGN KEY ([Unary4ID]) REFERENCES [dbo].[OppositeClassWithAnonymousRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Unary5ID] FOREIGN KEY ([Unary5ID]) REFERENCES [dbo].[OppositeClassWithAnonymousRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Unary6ID] FOREIGN KEY ([Unary6ID]) REFERENCES [dbo].[OppositeClassWithAnonymousRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Unary7ID] FOREIGN KEY ([Unary7ID]) REFERENCES [dbo].[OppositeClassWithAnonymousRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Unary8ID] FOREIGN KEY ([Unary8ID]) REFERENCES [dbo].[OppositeClassWithAnonymousRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Unary9ID] FOREIGN KEY ([Unary9ID]) REFERENCES [dbo].[OppositeClassWithAnonymousRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Unary10ID] FOREIGN KEY ([Unary10ID]) REFERENCES [dbo].[OppositeClassWithAnonymousRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Real1ID] FOREIGN KEY ([Real1ID]) REFERENCES [dbo].[OppositeClassWithVirtualRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Real2ID] FOREIGN KEY ([Real2ID]) REFERENCES [dbo].[OppositeClassWithVirtualRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Real3ID] FOREIGN KEY ([Real3ID]) REFERENCES [dbo].[OppositeClassWithVirtualRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Real4ID] FOREIGN KEY ([Real4ID]) REFERENCES [dbo].[OppositeClassWithVirtualRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Real5ID] FOREIGN KEY ([Real5ID]) REFERENCES [dbo].[OppositeClassWithVirtualRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Real6ID] FOREIGN KEY ([Real6ID]) REFERENCES [dbo].[OppositeClassWithVirtualRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Real7ID] FOREIGN KEY ([Real7ID]) REFERENCES [dbo].[OppositeClassWithVirtualRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Real8ID] FOREIGN KEY ([Real8ID]) REFERENCES [dbo].[OppositeClassWithVirtualRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Real9ID] FOREIGN KEY ([Real9ID]) REFERENCES [dbo].[OppositeClassWithVirtualRelationProperties] ([ID]),
  CONSTRAINT [FK_ClassWithRelationProperties_Real10ID] FOREIGN KEY ([Real10ID]) REFERENCES [dbo].[OppositeClassWithVirtualRelationProperties] ([ID])

ALTER TABLE [dbo].[Company] ADD
  CONSTRAINT [FK_Company_ClientID] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Client] ([ID])

ALTER TABLE [dbo].[File] ADD
  CONSTRAINT [FK_File_ClientID] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Client] ([ID])

ALTER TABLE [dbo].[OppositeClassWithCollectionRelationProperties] ADD
  CONSTRAINT [FK_OppositeClassWithCollectionRelationProperties_EndOfCollectionID] FOREIGN KEY ([EndOfCollectionID]) REFERENCES [dbo].[ClassWithRelationProperties] ([ID])

ALTER TABLE [dbo].[OppositeClassWithRealRelationProperties] ADD
  CONSTRAINT [FK_OppositeClassWithRealRelationProperties_Real1ID] FOREIGN KEY ([Real1ID]) REFERENCES [dbo].[ClassWithRelationProperties] ([ID]),
  CONSTRAINT [FK_OppositeClassWithRealRelationProperties_Real2ID] FOREIGN KEY ([Real2ID]) REFERENCES [dbo].[ClassWithRelationProperties] ([ID]),
  CONSTRAINT [FK_OppositeClassWithRealRelationProperties_Real3ID] FOREIGN KEY ([Real3ID]) REFERENCES [dbo].[ClassWithRelationProperties] ([ID]),
  CONSTRAINT [FK_OppositeClassWithRealRelationProperties_Real4ID] FOREIGN KEY ([Real4ID]) REFERENCES [dbo].[ClassWithRelationProperties] ([ID]),
  CONSTRAINT [FK_OppositeClassWithRealRelationProperties_Real5ID] FOREIGN KEY ([Real5ID]) REFERENCES [dbo].[ClassWithRelationProperties] ([ID]),
  CONSTRAINT [FK_OppositeClassWithRealRelationProperties_Real6ID] FOREIGN KEY ([Real6ID]) REFERENCES [dbo].[ClassWithRelationProperties] ([ID]),
  CONSTRAINT [FK_OppositeClassWithRealRelationProperties_Real7ID] FOREIGN KEY ([Real7ID]) REFERENCES [dbo].[ClassWithRelationProperties] ([ID]),
  CONSTRAINT [FK_OppositeClassWithRealRelationProperties_Real8ID] FOREIGN KEY ([Real8ID]) REFERENCES [dbo].[ClassWithRelationProperties] ([ID]),
  CONSTRAINT [FK_OppositeClassWithRealRelationProperties_Real9ID] FOREIGN KEY ([Real9ID]) REFERENCES [dbo].[ClassWithRelationProperties] ([ID]),
  CONSTRAINT [FK_OppositeClassWithRealRelationProperties_Real10ID] FOREIGN KEY ([Real10ID]) REFERENCES [dbo].[ClassWithRelationProperties] ([ID])

ALTER TABLE [dbo].[Person] ADD
  CONSTRAINT [FK_Person_ClientID] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Client] ([ID])
GO

-- Create a view for every class
CREATE VIEW [dbo].[ClassWithFewValuePropertiesView] ([ID], [ClassID], [Timestamp], [IntProperty1], [IntProperty2], [IntProperty3], [IntProperty4], [IntProperty5], [DateTimeProperty1], [DateTimeProperty2], [DateTimeProperty3], [DateTimeProperty4], [DateTimeProperty5], [StringProperty1], [StringProperty2], [StringProperty3], [StringProperty4], [StringProperty5], [BoolProperty1], [BoolProperty2], [BoolProperty3], [BoolProperty4], [BoolProperty5])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [IntProperty1], [IntProperty2], [IntProperty3], [IntProperty4], [IntProperty5], [DateTimeProperty1], [DateTimeProperty2], [DateTimeProperty3], [DateTimeProperty4], [DateTimeProperty5], [StringProperty1], [StringProperty2], [StringProperty3], [StringProperty4], [StringProperty5], [BoolProperty1], [BoolProperty2], [BoolProperty3], [BoolProperty4], [BoolProperty5]
    FROM [dbo].[ClassWithFewValueProperties]
    WHERE [ClassID] IN ('ClassWithFewValueProperties')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClassWithRelationPropertiesView] ([ID], [ClassID], [Timestamp], [Unary1ID], [Unary2ID], [Unary3ID], [Unary4ID], [Unary5ID], [Unary6ID], [Unary7ID], [Unary8ID], [Unary9ID], [Unary10ID], [Real1ID], [Real2ID], [Real3ID], [Real4ID], [Real5ID], [Real6ID], [Real7ID], [Real8ID], [Real9ID], [Real10ID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Unary1ID], [Unary2ID], [Unary3ID], [Unary4ID], [Unary5ID], [Unary6ID], [Unary7ID], [Unary8ID], [Unary9ID], [Unary10ID], [Real1ID], [Real2ID], [Real3ID], [Real4ID], [Real5ID], [Real6ID], [Real7ID], [Real8ID], [Real9ID], [Real10ID]
    FROM [dbo].[ClassWithRelationProperties]
    WHERE [ClassID] IN ('ClassWithRelationProperties')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClassWithValuePropertiesView] ([ID], [ClassID], [Timestamp], [IntProperty1], [IntProperty2], [IntProperty3], [IntProperty4], [IntProperty5], [IntProperty6], [IntProperty7], [IntProperty8], [IntProperty9], [IntProperty10], [DateTimeProperty1], [DateTimeProperty2], [DateTimeProperty3], [DateTimeProperty4], [DateTimeProperty5], [DateTimeProperty6], [DateTimeProperty7], [DateTimeProperty8], [DateTimeProperty9], [DateTimeProperty10], [StringProperty1], [StringProperty2], [StringProperty3], [StringProperty4], [StringProperty5], [StringProperty6], [StringProperty7], [StringProperty8], [StringProperty9], [StringProperty10], [BoolProperty1], [BoolProperty2], [BoolProperty3], [BoolProperty4], [BoolProperty5], [BoolProperty6], [BoolProperty7], [BoolProperty8], [BoolProperty9], [BoolProperty10])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [IntProperty1], [IntProperty2], [IntProperty3], [IntProperty4], [IntProperty5], [IntProperty6], [IntProperty7], [IntProperty8], [IntProperty9], [IntProperty10], [DateTimeProperty1], [DateTimeProperty2], [DateTimeProperty3], [DateTimeProperty4], [DateTimeProperty5], [DateTimeProperty6], [DateTimeProperty7], [DateTimeProperty8], [DateTimeProperty9], [DateTimeProperty10], [StringProperty1], [StringProperty2], [StringProperty3], [StringProperty4], [StringProperty5], [StringProperty6], [StringProperty7], [StringProperty8], [StringProperty9], [StringProperty10], [BoolProperty1], [BoolProperty2], [BoolProperty3], [BoolProperty4], [BoolProperty5], [BoolProperty6], [BoolProperty7], [BoolProperty8], [BoolProperty9], [BoolProperty10]
    FROM [dbo].[ClassWithValueProperties]
    WHERE [ClassID] IN ('ClassWithValueProperties')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClientView] ([ID], [ClassID], [Timestamp], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name]
    FROM [dbo].[Client]
    WHERE [ClassID] IN ('Client')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClientBoundBaseClassView] ([ID], [ClassID], [Timestamp], [ClientID], [Name], [FirstName], [LastName])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ClientID], [Name], null, null
    FROM [dbo].[Company]
    WHERE [ClassID] IN ('Company', 'Person')
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [ClientID], null, [FirstName], [LastName]
    FROM [dbo].[Person]
    WHERE [ClassID] IN ('Company', 'Person')
GO

CREATE VIEW [dbo].[CompanyView] ([ID], [ClassID], [Timestamp], [ClientID], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ClientID], [Name]
    FROM [dbo].[Company]
    WHERE [ClassID] IN ('Company')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[FileView] ([ID], [ClassID], [Timestamp], [Number], [ClientID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Number], [ClientID]
    FROM [dbo].[File]
    WHERE [ClassID] IN ('File')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[OppositeClassWithAnonymousRelationPropertiesView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[OppositeClassWithAnonymousRelationProperties]
    WHERE [ClassID] IN ('OppositeClassWithAnonymousRelationProperties')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[OppositeClassWithCollectionRelationPropertiesView] ([ID], [ClassID], [Timestamp], [EndOfCollectionID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [EndOfCollectionID]
    FROM [dbo].[OppositeClassWithCollectionRelationProperties]
    WHERE [ClassID] IN ('OppositeClassWithCollectionRelationProperties')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[OppositeClassWithRealRelationPropertiesView] ([ID], [ClassID], [Timestamp], [Real1ID], [Real2ID], [Real3ID], [Real4ID], [Real5ID], [Real6ID], [Real7ID], [Real8ID], [Real9ID], [Real10ID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Real1ID], [Real2ID], [Real3ID], [Real4ID], [Real5ID], [Real6ID], [Real7ID], [Real8ID], [Real9ID], [Real10ID]
    FROM [dbo].[OppositeClassWithRealRelationProperties]
    WHERE [ClassID] IN ('OppositeClassWithRealRelationProperties')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[OppositeClassWithVirtualRelationPropertiesView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[OppositeClassWithVirtualRelationProperties]
    WHERE [ClassID] IN ('OppositeClassWithVirtualRelationProperties')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[PersonView] ([ID], [ClassID], [Timestamp], [ClientID], [FirstName], [LastName])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ClientID], [FirstName], [LastName]
    FROM [dbo].[Person]
    WHERE [ClassID] IN ('Person')
  WITH CHECK OPTION
GO
