--
-- This file is auto-generated. Do not edit manually.
-- See TestDomainDBScriptGenerationTest for the relevant tests.
--

USE DBPrefix_TestDomain
-- Create all tables
CREATE TABLE [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [ParentAbstractBaseClassWithHierarchyID] uniqueidentifier NULL,
  [ParentAbstractBaseClassWithHierarchyIDClassID] varchar (100) NULL,
  [ClientFromAbstractBaseClassID] uniqueidentifier NULL,
  [FileSystemItemFromAbstractBaseClassID] uniqueidentifier NULL,
  [FileSystemItemFromAbstractBaseClassIDClassID] varchar (100) NULL,
  [ParentDerivedClassWithEntityWithHierarchyID] uniqueidentifier NULL,
  [ParentDerivedClassWithEntityWithHierarchyIDClassID] varchar (100) NULL,
  [ClientFromDerivedClassWithEntityID] uniqueidentifier NULL,
  [FileSystemItemFromDerivedClassWithEntityID] uniqueidentifier NULL,
  [FileSystemItemFromDerivedClassWithEntityIDClassID] varchar (100) NULL,
  [ParentDerivedClassWithEntityFromBaseClassWithHierarchyID] uniqueidentifier NULL,
  [ParentDerivedClassWithEntityFromBaseClassWithHierarchyIDClassID] varchar (100) NULL,
  [ClientFromDerivedClassWithEntityFromBaseClassID] uniqueidentifier NULL,
  [FileSystemItemFromDerivedClassWithEntityFromBaseClassID] uniqueidentifier NULL,
  [FileSystemItemFromDerivedClassWithEntityFromBaseClassIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_TableInheritance_DerivedClassWithEntityWithHierarchy] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableInheritance_Address]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Street] nvarchar (100) NOT NULL,
  [Zip] nvarchar (10) NOT NULL,
  [City] nvarchar (100) NOT NULL,
  [Country] nvarchar (100) NOT NULL,
  [PersonID] uniqueidentifier NULL,
  [PersonIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_TableInheritance_Address] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableInheritance_TableWithUnidirectionalRelation]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [DomainBaseID] uniqueidentifier NULL,
  [DomainBaseIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_TableInheritance_TableWithUnidirectionalRelation] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableInheritance_Client]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  CONSTRAINT [PK_TableInheritance_Client] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableInheritance_Person]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [CreatedBy] nvarchar (100) NOT NULL,
  [CreatedAt] datetime2 NOT NULL,
  [ClientID] uniqueidentifier NULL,
  [FirstName] nvarchar (100) NOT NULL,
  [LastName] nvarchar (100) NOT NULL,
  [DateOfBirth] datetime2 NOT NULL,
  [Photo] varbinary (max) NULL,
  [CustomerType] int NULL,
  [CustomerSince] datetime2 NULL,
  [RegionID] uniqueidentifier NULL,
  CONSTRAINT [PK_TableInheritance_Person] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableInheritance_File]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [ParentFolderID] uniqueidentifier NULL,
  [ParentFolderIDClassID] varchar (100) NULL,
  [Size] int NOT NULL,
  [FileCreatedAt] datetime2 NOT NULL,
  CONSTRAINT [PK_TableInheritance_File] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableInheritance_HistoryEntry]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [HistoryDate] datetime2 NOT NULL,
  [Text] nvarchar (250) NOT NULL,
  [OwnerID] uniqueidentifier NULL,
  [OwnerIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_TableInheritance_HistoryEntry] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableInheritance_Order]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Number] int NOT NULL,
  [OrderDate] datetime2 NOT NULL,
  [CustomerID] uniqueidentifier NULL,
  [CustomerIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_TableInheritance_Order] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableInheritance_OrganizationalUnit]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [CreatedBy] nvarchar (100) NOT NULL,
  [CreatedAt] datetime2 NOT NULL,
  [ClientID] uniqueidentifier NULL,
  [Name] nvarchar (100) NOT NULL,
  CONSTRAINT [PK_TableInheritance_OrganizationalUnit] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableInheritance_Region]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  CONSTRAINT [PK_TableInheritance_Region] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableInheritance_Folder]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [ParentFolderID] uniqueidentifier NULL,
  [ParentFolderIDClassID] varchar (100) NULL,
  [FolderCreatedAt] datetime2 NOT NULL,
  CONSTRAINT [PK_TableInheritance_Folder] PRIMARY KEY CLUSTERED ([ID])
)
-- Create foreign key constraints for tables that were created above
ALTER TABLE [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy] ADD
  CONSTRAINT [FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ParentDerivedClassWithEntityWithHierarchyID] FOREIGN KEY ([ParentDerivedClassWithEntityWithHierarchyID]) REFERENCES [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy] ([ID])
ALTER TABLE [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy] ADD
  CONSTRAINT [FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ClientFromDerivedClassWithEntityID] FOREIGN KEY ([ClientFromDerivedClassWithEntityID]) REFERENCES [dbo].[TableInheritance_Client] ([ID])
ALTER TABLE [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy] ADD
  CONSTRAINT [FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ClientFromAbstractBaseClassID] FOREIGN KEY ([ClientFromAbstractBaseClassID]) REFERENCES [dbo].[TableInheritance_Client] ([ID])
ALTER TABLE [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy] ADD
  CONSTRAINT [FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ParentDerivedClassWithEntityFromBaseClassWithHierarchyID] FOREIGN KEY ([ParentDerivedClassWithEntityFromBaseClassWithHierarchyID]) REFERENCES [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy] ([ID])
ALTER TABLE [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy] ADD
  CONSTRAINT [FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ClientFromDerivedClassWithEntityFromBaseClassID] FOREIGN KEY ([ClientFromDerivedClassWithEntityFromBaseClassID]) REFERENCES [dbo].[TableInheritance_Client] ([ID])
ALTER TABLE [dbo].[TableInheritance_Address] ADD
  CONSTRAINT [FK_TableInheritance_Address_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[TableInheritance_Person] ([ID])
ALTER TABLE [dbo].[TableInheritance_Person] ADD
  CONSTRAINT [FK_TableInheritance_Person_ClientID] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[TableInheritance_Client] ([ID])
ALTER TABLE [dbo].[TableInheritance_Person] ADD
  CONSTRAINT [FK_TableInheritance_Person_RegionID] FOREIGN KEY ([RegionID]) REFERENCES [dbo].[TableInheritance_Region] ([ID])
ALTER TABLE [dbo].[TableInheritance_File] ADD
  CONSTRAINT [FK_TableInheritance_File_ParentFolderID] FOREIGN KEY ([ParentFolderID]) REFERENCES [dbo].[TableInheritance_Folder] ([ID])
ALTER TABLE [dbo].[TableInheritance_Order] ADD
  CONSTRAINT [FK_TableInheritance_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[TableInheritance_Person] ([ID])
ALTER TABLE [dbo].[TableInheritance_OrganizationalUnit] ADD
  CONSTRAINT [FK_TableInheritance_OrganizationalUnit_ClientID] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[TableInheritance_Client] ([ID])
ALTER TABLE [dbo].[TableInheritance_Folder] ADD
  CONSTRAINT [FK_TableInheritance_Folder_ParentFolderID] FOREIGN KEY ([ParentFolderID]) REFERENCES [dbo].[TableInheritance_Folder] ([ID])
-- Create a view for every class
GO
CREATE VIEW [dbo].[TI_AbstractClassWithoutDerivationsView] ([ID], [ClassID], [Timestamp], [DomainBaseID], [DomainBaseIDClassID])
  AS
  SELECT CONVERT(uniqueidentifier,NULL) AS [ID], CONVERT(varchar (100),NULL) AS [ClassID], CONVERT(rowversion,NULL) AS [Timestamp], CONVERT(uniqueidentifier,NULL) AS [DomainBaseID], CONVERT(varchar (100),NULL) AS [DomainBaseIDClassID]
    WHERE 1 = 0
GO
CREATE VIEW [dbo].[TI_AbstractBaseClassWithHierarchyView] ([ID], [ClassID], [Timestamp], [Name], [ParentAbstractBaseClassWithHierarchyID], [ParentAbstractBaseClassWithHierarchyIDClassID], [ClientFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassIDClassID], [ParentDerivedClassWithEntityWithHierarchyID], [ParentDerivedClassWithEntityWithHierarchyIDClassID], [ClientFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityIDClassID], [ParentDerivedClassWithEntityFromBaseClassWithHierarchyID], [ParentDerivedClassWithEntityFromBaseClassWithHierarchyIDClassID], [ClientFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentAbstractBaseClassWithHierarchyID], [ParentAbstractBaseClassWithHierarchyIDClassID], [ClientFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassIDClassID], [ParentDerivedClassWithEntityWithHierarchyID], [ParentDerivedClassWithEntityWithHierarchyIDClassID], [ClientFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityIDClassID], [ParentDerivedClassWithEntityFromBaseClassWithHierarchyID], [ParentDerivedClassWithEntityFromBaseClassWithHierarchyIDClassID], [ClientFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassIDClassID]
    FROM [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_DerivedClassWithEntityWithHierarchyView] ([ID], [ClassID], [Timestamp], [Name], [ParentAbstractBaseClassWithHierarchyID], [ParentAbstractBaseClassWithHierarchyIDClassID], [ClientFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassIDClassID], [ParentDerivedClassWithEntityWithHierarchyID], [ParentDerivedClassWithEntityWithHierarchyIDClassID], [ClientFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityIDClassID], [ParentDerivedClassWithEntityFromBaseClassWithHierarchyID], [ParentDerivedClassWithEntityFromBaseClassWithHierarchyIDClassID], [ClientFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentAbstractBaseClassWithHierarchyID], [ParentAbstractBaseClassWithHierarchyIDClassID], [ClientFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassIDClassID], [ParentDerivedClassWithEntityWithHierarchyID], [ParentDerivedClassWithEntityWithHierarchyIDClassID], [ClientFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityIDClassID], [ParentDerivedClassWithEntityFromBaseClassWithHierarchyID], [ParentDerivedClassWithEntityFromBaseClassWithHierarchyIDClassID], [ClientFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassIDClassID]
    FROM [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_DerivedClassWithEntityFromBaseClassWithHierarchyView] ([ID], [ClassID], [Timestamp], [Name], [ParentAbstractBaseClassWithHierarchyID], [ParentAbstractBaseClassWithHierarchyIDClassID], [ClientFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassIDClassID], [ParentDerivedClassWithEntityWithHierarchyID], [ParentDerivedClassWithEntityWithHierarchyIDClassID], [ClientFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityIDClassID], [ParentDerivedClassWithEntityFromBaseClassWithHierarchyID], [ParentDerivedClassWithEntityFromBaseClassWithHierarchyIDClassID], [ClientFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentAbstractBaseClassWithHierarchyID], [ParentAbstractBaseClassWithHierarchyIDClassID], [ClientFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassIDClassID], [ParentDerivedClassWithEntityWithHierarchyID], [ParentDerivedClassWithEntityWithHierarchyIDClassID], [ClientFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityIDClassID], [ParentDerivedClassWithEntityFromBaseClassWithHierarchyID], [ParentDerivedClassWithEntityFromBaseClassWithHierarchyIDClassID], [ClientFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassIDClassID]
    FROM [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy]
    WHERE [ClassID] IN ('TI_DerivedClassWithEntityFromBaseClassWithHierarchy')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_AddressView] ([ID], [ClassID], [Timestamp], [Street], [Zip], [City], [Country], [PersonID], [PersonIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Street], [Zip], [City], [Country], [PersonID], [PersonIDClassID]
    FROM [dbo].[TableInheritance_Address]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_ClassWithUnidirectionalRelationView] ([ID], [ClassID], [Timestamp], [DomainBaseID], [DomainBaseIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [DomainBaseID], [DomainBaseIDClassID]
    FROM [dbo].[TableInheritance_TableWithUnidirectionalRelation]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_ClientView] ([ID], [ClassID], [Timestamp], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name]
    FROM [dbo].[TableInheritance_Client]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_DomainBaseView] ([ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince], [RegionID], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince], [RegionID], NULL
    FROM [dbo].[TableInheritance_Person]
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], NULL, NULL, NULL, NULL, NULL, NULL, NULL, [Name]
    FROM [dbo].[TableInheritance_OrganizationalUnit]
GO
CREATE VIEW [dbo].[TI_PersonView] ([ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince], [RegionID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince], [RegionID]
    FROM [dbo].[TableInheritance_Person]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_CustomerView] ([ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince], [RegionID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince], [RegionID]
    FROM [dbo].[TableInheritance_Person]
    WHERE [ClassID] IN ('TI_Customer')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_FileSystemItemView] ([ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [Size], [FileCreatedAt], [FolderCreatedAt])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [Size], [FileCreatedAt], NULL
    FROM [dbo].[TableInheritance_File]
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], NULL, NULL, [FolderCreatedAt]
    FROM [dbo].[TableInheritance_Folder]
GO
CREATE VIEW [dbo].[TI_FileView] ([ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [Size], [FileCreatedAt])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [Size], [FileCreatedAt]
    FROM [dbo].[TableInheritance_File]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_HistoryEntryView] ([ID], [ClassID], [Timestamp], [HistoryDate], [Text], [OwnerID], [OwnerIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [HistoryDate], [Text], [OwnerID], [OwnerIDClassID]
    FROM [dbo].[TableInheritance_HistoryEntry]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_OrderView] ([ID], [ClassID], [Timestamp], [Number], [OrderDate], [CustomerID], [CustomerIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Number], [OrderDate], [CustomerID], [CustomerIDClassID]
    FROM [dbo].[TableInheritance_Order]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_OrganizationalUnitView] ([ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [Name]
    FROM [dbo].[TableInheritance_OrganizationalUnit]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_RegionView] ([ID], [ClassID], [Timestamp], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name]
    FROM [dbo].[TableInheritance_Region]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_FolderView] ([ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [FolderCreatedAt])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [FolderCreatedAt]
    FROM [dbo].[TableInheritance_Folder]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TI_SpecificFolderView] ([ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [FolderCreatedAt])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [FolderCreatedAt]
    FROM [dbo].[TableInheritance_Folder]
    WHERE [ClassID] IN ('TI_SpecificFolder')
  WITH CHECK OPTION
GO
-- Create indexes for tables that were created above
-- Create synonyms for tables that were created above
-- Create all structured types
