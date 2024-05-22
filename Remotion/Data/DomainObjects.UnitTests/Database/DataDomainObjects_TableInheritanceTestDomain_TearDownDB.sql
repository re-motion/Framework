--
-- This file is auto-generated. Do not edit manually.
-- See TestDomainDBScriptGenerationTest for the relevant tests.
--

USE DBPrefix_TestDomain
-- Drop all structured types
DROP TYPE IF EXISTS [dbo].[TVP_String]
DROP TYPE IF EXISTS [dbo].[TVP_Binary]
DROP TYPE IF EXISTS [dbo].[TVP_Boolean]
DROP TYPE IF EXISTS [dbo].[TVP_Boolean_distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Byte]
DROP TYPE IF EXISTS [dbo].[TVP_Byte_distinct]
DROP TYPE IF EXISTS [dbo].[TVP_DateTime]
DROP TYPE IF EXISTS [dbo].[TVP_DateTime_distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Decimal]
DROP TYPE IF EXISTS [dbo].[TVP_Decimal_distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Double]
DROP TYPE IF EXISTS [dbo].[TVP_Double_distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Guid]
DROP TYPE IF EXISTS [dbo].[TVP_Guid_distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Int16]
DROP TYPE IF EXISTS [dbo].[TVP_Int16_distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Int32]
DROP TYPE IF EXISTS [dbo].[TVP_Int32_distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Int64]
DROP TYPE IF EXISTS [dbo].[TVP_Int64_distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Single]
DROP TYPE IF EXISTS [dbo].[TVP_Single_distinct]
-- Drop all synonyms
-- Drop all indexes
-- Drop all views
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_AbstractClassWithoutDerivationsView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_AbstractClassWithoutDerivationsView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_AbstractBaseClassWithHierarchyView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_AbstractBaseClassWithHierarchyView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_DerivedClassWithEntityWithHierarchyView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_DerivedClassWithEntityWithHierarchyView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_DerivedClassWithEntityFromBaseClassWithHierarchyView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_DerivedClassWithEntityFromBaseClassWithHierarchyView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_AddressView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_AddressView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_ClassWithUnidirectionalRelationView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_ClassWithUnidirectionalRelationView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_ClientView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_ClientView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_DomainBaseView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_DomainBaseView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_PersonView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_PersonView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_CustomerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_CustomerView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_FileSystemItemView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_FileSystemItemView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_FileView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_FileView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_HistoryEntryView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_HistoryEntryView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_OrderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_OrderView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_OrganizationalUnitView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_OrganizationalUnitView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_RegionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_RegionView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_FolderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_FolderView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_SpecificFolderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_SpecificFolderView]
-- Drop foreign keys of all tables
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ParentDerivedClassWithEntityWithHierarchyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableInheritance_DerivedClassWithEntityWithHierarchy')
  ALTER TABLE [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy] DROP CONSTRAINT FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ParentDerivedClassWithEntityWithHierarchyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ClientFromDerivedClassWithEntityID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableInheritance_DerivedClassWithEntityWithHierarchy')
  ALTER TABLE [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy] DROP CONSTRAINT FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ClientFromDerivedClassWithEntityID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ClientFromAbstractBaseClassID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableInheritance_DerivedClassWithEntityWithHierarchy')
  ALTER TABLE [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy] DROP CONSTRAINT FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ClientFromAbstractBaseClassID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ParentDerivedClassWithEntityFromBaseClassWithHierarchyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableInheritance_DerivedClassWithEntityWithHierarchy')
  ALTER TABLE [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy] DROP CONSTRAINT FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ParentDerivedClassWithEntityFromBaseClassWithHierarchyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ClientFromDerivedClassWithEntityFromBaseClassID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableInheritance_DerivedClassWithEntityWithHierarchy')
  ALTER TABLE [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy] DROP CONSTRAINT FK_TableInheritance_DerivedClassWithEntityWithHierarchy_ClientFromDerivedClassWithEntityFromBaseClassID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableInheritance_Address_PersonID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableInheritance_Address')
  ALTER TABLE [dbo].[TableInheritance_Address] DROP CONSTRAINT FK_TableInheritance_Address_PersonID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableInheritance_Person_ClientID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableInheritance_Person')
  ALTER TABLE [dbo].[TableInheritance_Person] DROP CONSTRAINT FK_TableInheritance_Person_ClientID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableInheritance_Person_RegionID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableInheritance_Person')
  ALTER TABLE [dbo].[TableInheritance_Person] DROP CONSTRAINT FK_TableInheritance_Person_RegionID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableInheritance_File_ParentFolderID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableInheritance_File')
  ALTER TABLE [dbo].[TableInheritance_File] DROP CONSTRAINT FK_TableInheritance_File_ParentFolderID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableInheritance_Order_CustomerID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableInheritance_Order')
  ALTER TABLE [dbo].[TableInheritance_Order] DROP CONSTRAINT FK_TableInheritance_Order_CustomerID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableInheritance_OrganizationalUnit_ClientID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableInheritance_OrganizationalUnit')
  ALTER TABLE [dbo].[TableInheritance_OrganizationalUnit] DROP CONSTRAINT FK_TableInheritance_OrganizationalUnit_ClientID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableInheritance_Folder_ParentFolderID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableInheritance_Folder')
  ALTER TABLE [dbo].[TableInheritance_Folder] DROP CONSTRAINT FK_TableInheritance_Folder_ParentFolderID
-- Drop all tables
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_DerivedClassWithEntityWithHierarchy' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableInheritance_DerivedClassWithEntityWithHierarchy]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Address' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableInheritance_Address]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_TableWithUnidirectionalRelation' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableInheritance_TableWithUnidirectionalRelation]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Client' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableInheritance_Client]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Person' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableInheritance_Person]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_File' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableInheritance_File]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_HistoryEntry' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableInheritance_HistoryEntry]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Order' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableInheritance_Order]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_OrganizationalUnit' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableInheritance_OrganizationalUnit]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Region' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableInheritance_Region]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Folder' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableInheritance_Folder]
