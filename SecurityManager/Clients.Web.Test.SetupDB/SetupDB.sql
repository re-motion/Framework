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
GO
