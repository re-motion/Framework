--
-- This file contains manual additions/changes to the test domain database.
-- This file is executed after all the other scripts.
--  1. Misc additions
--  2. Procedures
--

-- ==============================================
--  1. MISC ADDITIONS
-- ==============================================

-- Create TableWithInvalidRelation
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithInvalidRelation') 
  DROP TABLE [TableWithInvalidRelation]
GO

CREATE TABLE [TableWithInvalidRelation] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [TableWithGuidKeyID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_TableWithInvalidRelation] PRIMARY KEY CLUSTERED ([ID])
  -- Note the lack of a foreign key referring to TableWithGuidKey
) 
GO

-- Create TableWithKeyOfInvalidType
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithKeyOfInvalidType') 
  DROP TABLE [TableWithKeyOfInvalidType]
GO

CREATE TABLE [TableWithKeyOfInvalidType] (
  [ID] datetime NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  CONSTRAINT [PK_TableWithKeyOfInvalidType] PRIMARY KEY CLUSTERED ([ID])
) 
GO

-- Create TableWithoutClassIDColumn
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutClassIDColumn') 
  DROP TABLE [TableWithoutClassIDColumn]
GO

CREATE TABLE [TableWithoutClassIDColumn] (
  [ID] uniqueidentifier NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  CONSTRAINT [PK_TableWithoutClassIDColumn] PRIMARY KEY CLUSTERED ([ID])
) 
GO

-- Create TableWithoutIDColumn
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutIDColumn') 
  DROP TABLE [TableWithoutIDColumn]
GO

CREATE TABLE [TableWithoutIDColumn] (
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL
) 
GO

-- Create TableWithoutRelatedClassIDColumn
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutRelatedClassIDColumn') 
  DROP TABLE [TableWithoutRelatedClassIDColumn]
GO

CREATE TABLE [TableWithoutRelatedClassIDColumn] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [DistributorID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_TableWithoutRelatedClassIDColumn] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableWithoutRelatedClassIDColumn_Distributor] FOREIGN KEY ([DistributorID]) REFERENCES [Company] ([ID])
) 
GO

-- Create TableWithoutRelatedClassIDColumnAndDerivation
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutRelatedClassIDColumnAndDerivation') 
  DROP TABLE [TableWithoutRelatedClassIDColumnAndDerivation]
GO

CREATE TABLE [TableWithoutRelatedClassIDColumnAndDerivation] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [CompanyID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_TableWithoutRelatedClassIDColumnAndDerivation] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableWithoutRelatedClassIDColumnAndDerivation_Company] FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([ID])
) 
GO

-- Create TableWithoutTimestampColumn
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutTimestampColumn') 
  DROP TABLE [TableWithoutTimestampColumn]
GO

CREATE TABLE [TableWithoutTimestampColumn] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  
  CONSTRAINT [PK_TableWithoutTimestampColumn] PRIMARY KEY CLUSTERED ([ID])
) 
GO

-- Create TableInheritance_BaseClassWithInvalidRelationClassIDColumns
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_BaseClassWithInvalidRelationClassIDColumns') 
  DROP TABLE [TableInheritance_BaseClassWithInvalidRelationClassIDColumns]
GO

CREATE TABLE [TableInheritance_BaseClassWithInvalidRelationClassIDColumns] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [ClientID] uniqueidentifier NULL, -- Note: To conform to mapping column ClientIDClassID must not be defined.
  [ClientIDClassID] varchar (100) NULL, 
  [DomainBaseID] uniqueidentifier NULL, -- Note: To conform to mapping column DomainBaseIDClassID must be defined.
  
  [DomainBaseWithInvalidClassIDValueID] uniqueidentifier NULL, 
  [DomainBaseWithInvalidClassIDValueIDClassID] varchar (100) NULL, 
  [DomainBaseWithInvalidClassIDNullValueID] uniqueidentifier NULL, 
  [DomainBaseWithInvalidClassIDNullValueIDClassID] varchar (100) NULL, 
  
  -- Note: This table does not need to have foreign keys, because rows cannot be read because of invalid ClassID column structure
  CONSTRAINT [PK_TableInheritance_BaseClassWithInvalidRelationClassIDColumns] PRIMARY KEY CLUSTERED ([ID])
) 
GO

-- CustomDataType_ClassWithCustomDataType
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CustomDataType_ClassWithCustomDataTypeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CustomDataType_ClassWithCustomDataTypeView]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'CustomDataType_ClassWithCustomDataType')
  DROP TABLE [dbo].[CustomDataType_ClassWithCustomDataType]
GO

CREATE TABLE [dbo].[CustomDataType_ClassWithCustomDataType]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [CompoundDataTypeValueStringValue] nvarchar (100) NULL,
  [CompoundDataTypeValueInt32Value] int NULL,
  [SimpleDataTypeValue] nvarchar (max) NULL,
  CONSTRAINT [PK_ClassWithCustomDataType] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE VIEW [dbo].[CustomDataType_ClassWithCustomDataTypeView] ([ID], [ClassID], [Timestamp], [CompoundDataTypeValueStringValue], [CompoundDataTypeValueInt32Value], [SimpleDataTypeValue])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CompoundDataTypeValueStringValue], [CompoundDataTypeValueInt32Value], [SimpleDataTypeValue]
    FROM [dbo].[CustomDataType_ClassWithCustomDataType]
  WITH CHECK OPTION
GO

-- ==============================================
--  2. PROCEDURES
-- ==============================================

-- rpf_testSPQuery
IF OBJECT_ID ('rpf_testSPQuery', 'P') IS NOT NULL 
  DROP PROCEDURE rpf_testSPQuery;
GO

CREATE PROCEDURE rpf_testSPQuery
AS
  SELECT * FROM [Order] WHERE [OrderNo] = 1 OR [OrderNo] = 3 ORDER BY [OrderNo] ASC
GO

-- rpf_testSPQueryWithParameter
IF OBJECT_ID ('rpf_testSPQueryWithParameter', 'P') IS NOT NULL 
  DROP PROCEDURE rpf_testSPQueryWithParameter;
GO

CREATE PROCEDURE rpf_testSPQueryWithParameter
  @customerID uniqueidentifier
AS
  SELECT * FROM [Order] WHERE [CustomerID] = @customerID ORDER BY [OrderNo] ASC
GO
