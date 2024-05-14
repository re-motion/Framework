USE DBPrefix_SchemaGenerationTestDomain2
-- Create all tables
CREATE TABLE [dbo].[Official]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [ResponsibleForOrderPriority] int NOT NULL,
  [ResponsibleForCustomerType] int NOT NULL,
  [Speciality] nvarchar (255) NULL,
  CONSTRAINT [PK_Official] PRIMARY KEY CLUSTERED ([ID])
)
-- Create foreign key constraints for tables that were created above
-- Create a view for every class
GO
CREATE VIEW [dbo].[OfficialView] ([ID], [ClassID], [Timestamp], [Name], [ResponsibleForOrderPriority], [ResponsibleForCustomerType], [Speciality])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ResponsibleForOrderPriority], [ResponsibleForCustomerType], [Speciality]
    FROM [dbo].[Official]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[SpecialOfficialView] ([ID], [ClassID], [Timestamp], [Name], [ResponsibleForOrderPriority], [ResponsibleForCustomerType], [Speciality])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ResponsibleForOrderPriority], [ResponsibleForCustomerType], [Speciality]
    FROM [dbo].[Official]
    WHERE [ClassID] IN ('SpecialOfficial')
  WITH CHECK OPTION
GO
-- Create indexes for tables that were created above
-- Create synonyms for tables that were created above
-- Create all structured types
IF TYPE_ID('[dbo].[TVP_String]') IS NULL CREATE TYPE [dbo].[TVP_String] AS TABLE
(
  [Value] nvarchar (max) NULL
)
IF TYPE_ID('[dbo].[TVP_Binary]') IS NULL CREATE TYPE [dbo].[TVP_Binary] AS TABLE
(
  [Value] varbinary (max) NULL
)
IF TYPE_ID('[dbo].[TVP_Boolean]') IS NULL CREATE TYPE [dbo].[TVP_Boolean] AS TABLE
(
  [Value] bit NULL
)
IF TYPE_ID('[dbo].[TVP_Byte]') IS NULL CREATE TYPE [dbo].[TVP_Byte] AS TABLE
(
  [Value] tinyint NULL
)
IF TYPE_ID('[dbo].[TVP_DateTime]') IS NULL CREATE TYPE [dbo].[TVP_DateTime] AS TABLE
(
  [Value] datetime NULL
)
IF TYPE_ID('[dbo].[TVP_Decimal]') IS NULL CREATE TYPE [dbo].[TVP_Decimal] AS TABLE
(
  [Value] decimal (38, 3) NULL
)
IF TYPE_ID('[dbo].[TVP_Double]') IS NULL CREATE TYPE [dbo].[TVP_Double] AS TABLE
(
  [Value] float NULL
)
IF TYPE_ID('[dbo].[TVP_Guid]') IS NULL CREATE TYPE [dbo].[TVP_Guid] AS TABLE
(
  [Value] uniqueidentifier NULL
)
IF TYPE_ID('[dbo].[TVP_Int16]') IS NULL CREATE TYPE [dbo].[TVP_Int16] AS TABLE
(
  [Value] smallint NULL
)
IF TYPE_ID('[dbo].[TVP_Int32]') IS NULL CREATE TYPE [dbo].[TVP_Int32] AS TABLE
(
  [Value] int NULL
)
IF TYPE_ID('[dbo].[TVP_Int64]') IS NULL CREATE TYPE [dbo].[TVP_Int64] AS TABLE
(
  [Value] bigint NULL
)
IF TYPE_ID('[dbo].[TVP_Single]') IS NULL CREATE TYPE [dbo].[TVP_Single] AS TABLE
(
  [Value] real NULL
)
