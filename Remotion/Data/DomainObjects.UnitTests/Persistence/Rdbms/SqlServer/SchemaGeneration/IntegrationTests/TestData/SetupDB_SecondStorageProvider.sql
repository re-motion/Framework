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
CREATE TYPE [dbo].[TVP_String] AS TABLE
(
  [Value] nvarchar (max) NULL
)
GO
CREATE TYPE [dbo].[TVP_Binary] AS TABLE
(
  [Value] varbinary (max) NULL
)
GO
CREATE TYPE [dbo].[TVP_AnsiString] AS TABLE
(
  [Value] varchar (max) NULL
)
GO
CREATE TYPE [dbo].[TVP_Boolean] AS TABLE
(
  [Value] bit NULL
)
GO
CREATE TYPE [dbo].[TVP_Boolean_Distinct] AS TABLE
(
  [Value] bit NULL
  UNIQUE CLUSTERED ([Value])
)
GO
CREATE TYPE [dbo].[TVP_Byte] AS TABLE
(
  [Value] tinyint NULL
)
GO
CREATE TYPE [dbo].[TVP_Byte_Distinct] AS TABLE
(
  [Value] tinyint NULL
  UNIQUE CLUSTERED ([Value])
)
GO
CREATE TYPE [dbo].[TVP_Int16] AS TABLE
(
  [Value] smallint NULL
)
GO
CREATE TYPE [dbo].[TVP_Int16_Distinct] AS TABLE
(
  [Value] smallint NULL
  UNIQUE CLUSTERED ([Value])
)
GO
CREATE TYPE [dbo].[TVP_Int32] AS TABLE
(
  [Value] int NULL
)
GO
CREATE TYPE [dbo].[TVP_Int32_Distinct] AS TABLE
(
  [Value] int NULL
  UNIQUE CLUSTERED ([Value])
)
GO
CREATE TYPE [dbo].[TVP_Int64] AS TABLE
(
  [Value] bigint NULL
)
GO
CREATE TYPE [dbo].[TVP_Int64_Distinct] AS TABLE
(
  [Value] bigint NULL
  UNIQUE CLUSTERED ([Value])
)
GO
CREATE TYPE [dbo].[TVP_Decimal] AS TABLE
(
  [Value] decimal (38, 3) NULL
)
GO
CREATE TYPE [dbo].[TVP_Decimal_Distinct] AS TABLE
(
  [Value] decimal (38, 3) NULL
  UNIQUE CLUSTERED ([Value])
)
GO
CREATE TYPE [dbo].[TVP_Single] AS TABLE
(
  [Value] real NULL
)
GO
CREATE TYPE [dbo].[TVP_Single_Distinct] AS TABLE
(
  [Value] real NULL
  UNIQUE CLUSTERED ([Value])
)
GO
CREATE TYPE [dbo].[TVP_Double] AS TABLE
(
  [Value] float NULL
)
GO
CREATE TYPE [dbo].[TVP_Double_Distinct] AS TABLE
(
  [Value] float NULL
  UNIQUE CLUSTERED ([Value])
)
GO
CREATE TYPE [dbo].[TVP_DateTime] AS TABLE
(
  [Value] datetime NULL
)
GO
CREATE TYPE [dbo].[TVP_DateTime_Distinct] AS TABLE
(
  [Value] datetime NULL
  UNIQUE CLUSTERED ([Value])
)
GO
CREATE TYPE [dbo].[TVP_Guid] AS TABLE
(
  [Value] uniqueidentifier NULL
)
GO
CREATE TYPE [dbo].[TVP_Guid_Distinct] AS TABLE
(
  [Value] uniqueidentifier NULL
  UNIQUE CLUSTERED ([Value])
)
GO
