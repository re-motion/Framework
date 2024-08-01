USE RemotionSecurityManagerWebClientTest
GO

-- Create all tables
CREATE TABLE [dbo].[File]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- File columns
  [Name] nvarchar (100) NOT NULL,
  [Confidentiality] int NOT NULL,
  [TenantID] uniqueidentifier NULL,
  [CreatorUserID] uniqueidentifier NULL,
  [ClerkUserID] uniqueidentifier NULL,

  CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[FileItem]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- FileItem columns
  [Name] nvarchar (100) NOT NULL,
  [TenantID] uniqueidentifier NULL,
  [FileID] uniqueidentifier NULL,

  CONSTRAINT [PK_FileItem] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
ALTER TABLE [dbo].[File] ADD
  CONSTRAINT [FK_TenantToFile] FOREIGN KEY ([TenantID]) REFERENCES [dbo].[Tenant] ([ID]),
  CONSTRAINT [FK_OwnerUserToFile] FOREIGN KEY ([CreatorUserID]) REFERENCES [dbo].[User] ([ID]),
  CONSTRAINT [FK_ClerkUserToFile] FOREIGN KEY ([ClerkUserID]) REFERENCES [dbo].[User] ([ID])

ALTER TABLE [dbo].[FileItem] ADD
  CONSTRAINT [FK_FileItemToFile] FOREIGN KEY ([FileID]) REFERENCES [dbo].[File] ([ID]),
  CONSTRAINT [FK_TenantToFileItem] FOREIGN KEY ([TenantID]) REFERENCES [dbo].[Tenant] ([ID])
GO

-- Create a view for every class
CREATE VIEW [dbo].[FileView] ([ID], [ClassID], [Timestamp], [Name], [Confidentiality], [TenantID], [CreatorUserID], [ClerkUserID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [Confidentiality], [TenantID], [CreatorUserID], [ClerkUserID]
    FROM [dbo].[File]
    WHERE [ClassID] IN ('File')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[FileItemView] ([ID], [ClassID], [Timestamp], [Name], [TenantID], [FileID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [TenantID], [FileID]
    FROM [dbo].[FileItem]
    WHERE [ClassID] IN ('FileItem')
  WITH CHECK OPTION
GO

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
  [Value] bit NOT NULL
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
CREATE TYPE [dbo].[TVP_DateTime2] AS TABLE
(
  [Value] datetime2 NULL
)
GO
CREATE TYPE [dbo].[TVP_DateTime2_Distinct] AS TABLE
(
  [Value] datetime2 NULL
  UNIQUE CLUSTERED ([Value])
)
GO
CREATE TYPE [dbo].[TVP_Date] AS TABLE
(
  [Value] date NULL
)
GO
CREATE TYPE [dbo].[TVP_Date_Distinct] AS TABLE
(
  [Value] date NULL
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
