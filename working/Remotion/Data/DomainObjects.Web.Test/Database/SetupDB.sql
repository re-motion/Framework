USE RpaTest
GO

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassForRelationTestView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassForRelationTestView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithAllDataTypesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithAllDataTypesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithoutPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithoutPropertiesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithUndefinedEnumView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithUndefinedEnumView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('TableForRelationTest', 'TableWithAllDataTypes', 'TableWithoutColumns', 'TableWithUndefinedEnum')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableForRelationTest' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableForRelationTest]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithAllDataTypes]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutColumns' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithoutColumns]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithUndefinedEnum' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithUndefinedEnum]
GO

-- Create all tables
CREATE TABLE [dbo].[TableForRelationTest]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClassForRelationTest columns
  [Name] nvarchar (100) NOT NULL,
  [TableWithAllDataTypesMandatory] uniqueidentifier NULL,
  [TableWithAllDataTypesOptional] uniqueidentifier NULL,

  CONSTRAINT [PK_TableForRelationTest] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[TableWithAllDataTypes]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClassWithAllDataTypes columns
  [DelimitedStringArray] nvarchar (1000) NOT NULL,
  [DelimitedNullStringArray] nvarchar (1000) NULL,
  [Boolean] bit NOT NULL,
  [Byte] tinyint NOT NULL,
  [Date] datetime NOT NULL,
  [DateTime] datetime NOT NULL,
  [Decimal] decimal (38, 3) NOT NULL,
  [Double] float NOT NULL,
  [Enum] int NOT NULL,
  [ExtensibleEnum] varchar (81) NOT NULL,
  [Guid] uniqueidentifier NOT NULL,
  [Int16] smallint NOT NULL,
  [Int32] int NOT NULL,
  [Int64] bigint NOT NULL,
  [Single] real NOT NULL,
  [String] nvarchar (100) NOT NULL,
  [StringWithoutMaxLength] nvarchar (max) NOT NULL,
  [Binary] varbinary (max) NOT NULL,
  [NaBoolean] bit NULL,
  [NaByte] tinyint NULL,
  [NaDate] datetime NULL,
  [NaDateTime] datetime NULL,
  [NaDecimal] decimal (38, 3) NULL,
  [NaDouble] float NULL,
  [NaEnum] int NULL,
  [NaGuid] uniqueidentifier NULL,
  [NaInt16] smallint NULL,
  [NaInt32] int NULL,
  [NaInt64] bigint NULL,
  [NaSingle] real NULL,
  [StringWithNullValue] nvarchar (100) NULL,
  [ExtensibleEnumWithNullValue] varchar (81) NULL,
  [NaBooleanWithNullValue] bit NULL,
  [NaByteWithNullValue] tinyint NULL,
  [NaDateWithNullValue] datetime NULL,
  [NaDateTimeWithNullValue] datetime NULL,
  [NaDecimalWithNullValue] decimal (38, 3) NULL,
  [NaDoubleWithNullValue] float NULL,
  [NaEnumWithNullValue] int NULL,
  [NaGuidWithNullValue] uniqueidentifier NULL,
  [NaInt16WithNullValue] smallint NULL,
  [NaInt32WithNullValue] int NULL,
  [NaInt64WithNullValue] bigint NULL,
  [NaSingleWithNullValue] real NULL,
  [NullableBinary] varbinary (1000) NULL,
  [TableForRelationTestMandatory] uniqueidentifier NULL,
  [TableForRelationTestOptional] uniqueidentifier NULL,

  CONSTRAINT [PK_TableWithAllDataTypes] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[TableWithoutColumns]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClassWithoutProperties columns

  CONSTRAINT [PK_TableWithoutColumns] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[TableWithUndefinedEnum]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClassWithUndefinedEnum columns
  [UndefinedEnum] int NOT NULL,

  CONSTRAINT [PK_TableWithUndefinedEnum] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
ALTER TABLE [dbo].[TableForRelationTest] ADD
  CONSTRAINT [FK_TableForRelationTest_TableWithAllDataTypesMandatory] FOREIGN KEY ([TableWithAllDataTypesMandatory]) REFERENCES [dbo].[TableWithAllDataTypes] ([ID]),
  CONSTRAINT [FK_TableForRelationTest_TableWithAllDataTypesOptional] FOREIGN KEY ([TableWithAllDataTypesOptional]) REFERENCES [dbo].[TableWithAllDataTypes] ([ID])

ALTER TABLE [dbo].[TableWithAllDataTypes] ADD
  CONSTRAINT [FK_TableWithAllDataTypes_TableForRelationTestMandatory] FOREIGN KEY ([TableForRelationTestMandatory]) REFERENCES [dbo].[TableForRelationTest] ([ID]),
  CONSTRAINT [FK_TableWithAllDataTypes_TableForRelationTestOptional] FOREIGN KEY ([TableForRelationTestOptional]) REFERENCES [dbo].[TableForRelationTest] ([ID])
GO

-- Create a view for every class
CREATE VIEW [dbo].[ClassForRelationTestView] ([ID], [ClassID], [Timestamp], [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional]
    FROM [dbo].[TableForRelationTest]
    WHERE [ClassID] IN ('ClassForRelationTest')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClassWithAllDataTypesView] ([ID], [ClassID], [Timestamp], [DelimitedStringArray], [DelimitedNullStringArray], [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [Binary], [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], [StringWithNullValue], [ExtensibleEnumWithNullValue], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary], [TableForRelationTestMandatory], [TableForRelationTestOptional])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [DelimitedStringArray], [DelimitedNullStringArray], [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [Binary], [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], [StringWithNullValue], [ExtensibleEnumWithNullValue], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary], [TableForRelationTestMandatory], [TableForRelationTestOptional]
    FROM [dbo].[TableWithAllDataTypes]
    WHERE [ClassID] IN ('ClassWithAllDataTypes')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClassWithoutPropertiesView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[TableWithoutColumns]
    WHERE [ClassID] IN ('ClassWithoutProperties')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClassWithUndefinedEnumView] ([ID], [ClassID], [Timestamp], [UndefinedEnum])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [UndefinedEnum]
    FROM [dbo].[TableWithUndefinedEnum]
    WHERE [ClassID] IN ('ClassWithUndefinedEnum')
  WITH CHECK OPTION
GO
