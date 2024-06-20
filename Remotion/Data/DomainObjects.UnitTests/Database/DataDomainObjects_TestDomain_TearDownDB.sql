--
-- This file is auto-generated. Do not edit manually.
-- See TestDomainDBScriptGenerationTest for the relevant tests.
--

USE DBPrefix_TestDomain
-- Drop all structured types
DROP TYPE IF EXISTS [dbo].[TVP_String]
DROP TYPE IF EXISTS [dbo].[TVP_Binary]
DROP TYPE IF EXISTS [dbo].[TVP_AnsiString]
DROP TYPE IF EXISTS [dbo].[TVP_Boolean]
DROP TYPE IF EXISTS [dbo].[TVP_Boolean_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Byte]
DROP TYPE IF EXISTS [dbo].[TVP_Byte_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Int16]
DROP TYPE IF EXISTS [dbo].[TVP_Int16_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Int32]
DROP TYPE IF EXISTS [dbo].[TVP_Int32_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Int64]
DROP TYPE IF EXISTS [dbo].[TVP_Int64_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Decimal]
DROP TYPE IF EXISTS [dbo].[TVP_Decimal_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Single]
DROP TYPE IF EXISTS [dbo].[TVP_Single_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Double]
DROP TYPE IF EXISTS [dbo].[TVP_Double_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_DateTime]
DROP TYPE IF EXISTS [dbo].[TVP_DateTime_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Guid]
DROP TYPE IF EXISTS [dbo].[TVP_Guid_Distinct]
-- Drop all synonyms
-- Drop all indexes
-- Drop all views
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithPropertiesHavingStorageClassAttributeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithPropertiesHavingStorageClassAttributeView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EagerFetching_BaseClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[EagerFetching_BaseClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EagerFetching_DerivedClass1View' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[EagerFetching_DerivedClass1View]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EagerFetching_DerivedClass2View' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[EagerFetching_DerivedClass2View]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EagerFetching_RelationTargetView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[EagerFetching_RelationTargetView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'NestedDomainObjectView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[NestedDomainObjectView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ConcreteInheritanceBaseClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ConcreteInheritanceBaseClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ConcreteInheritanceFirstDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ConcreteInheritanceFirstDerivedClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ConcreteInheritanceObjectWithRelationsView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ConcreteInheritanceObjectWithRelationsView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ConcreteInheritanceSecondDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ConcreteInheritanceSecondDerivedClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TargetClassForPersistentMixinView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TargetClassForPersistentMixinView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedTargetClassForPersistentMixinView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedTargetClassForPersistentMixinView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedDerivedTargetClassForPersistentMixinView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedDerivedTargetClassForPersistentMixinView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedTargetClassWithDerivedMixinWithInterfaceView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedTargetClassWithDerivedMixinWithInterfaceView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'HookedTargetClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[HookedTargetClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'InheritanceRootInheritingPersistentMixinView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[InheritanceRootInheritingPersistentMixinView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'RelationTargetForPersistentMixinView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[RelationTargetForPersistentMixinView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SingleInheritanceBaseClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SingleInheritanceBaseClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SingleInheritanceFirstDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SingleInheritanceFirstDerivedClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SingleInheritanceObjectWithRelationsView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SingleInheritanceObjectWithRelationsView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SingleInheritanceSecondDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SingleInheritanceSecondDerivedClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TargetClassForBehavioralMixinView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TargetClassForBehavioralMixinView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TargetClassForMixinWithStateView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TargetClassForMixinWithStateView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TargetClassWithSameInterfaceAsPersistentMixinView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TargetClassWithSameInterfaceAsPersistentMixinView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TargetClassWithTwoUnidirectionalMixinsView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TargetClassWithTwoUnidirectionalMixinsView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TargetClassWithUnidirectionalMixin1View' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TargetClassWithUnidirectionalMixin1View]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TargetClassWithUnidirectionalMixin2View' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TargetClassWithUnidirectionalMixin2View]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CeoView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CeoView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassNotInMappingView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassNotInMappingView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithAllDataTypesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithAllDataTypesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithEnumNotDefiningZeroView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithEnumNotDefiningZeroView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithGuidKeyView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithGuidKeyView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithNonPublicPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithNonPublicPropertiesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithOptionalOneToOneRelationAndOppositeDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithOptionalOneToOneRelationAndOppositeDerivedClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithoutPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithoutPropertiesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithPropertyTypeImplementingIStructuralEquatableView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithPropertyTypeImplementingIStructuralEquatableView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithRelatedClassIDColumnAndNoInheritanceView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithRelatedClassIDColumnAndNoInheritanceView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithValidRelationsView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithValidRelationsView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClientView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClientView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ComputerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ComputerView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CompanyView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PartnerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PartnerView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DistributorView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DistributorView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EmployeeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[EmployeeView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FileSystemItemView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[FileSystemItemView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FileView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[FileView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FolderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[FolderView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'IndustrialSectorView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[IndustrialSectorView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StorageGroupClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StorageGroupClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'LocationView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[LocationView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderItemView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderItemView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderTicketView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderTicketView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PersonView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PersonView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ProductView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ProductView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ProductReviewView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ProductReviewView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassIDForClassHavingClassIDAttributeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassIDForClassHavingClassIDAttributeView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassHavingStorageSpecificIdentifierAttributeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassHavingStorageSpecificIdentifierAttributeView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithBinaryPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithBinaryPropertiesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithBothEndPointsOnSameClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithBothEndPointsOnSameClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithExtensibleEnumPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithExtensibleEnumPropertiesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithManySideRelationPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithManySideRelationPropertiesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithOneSideRelationPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithOneSideRelationPropertiesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClosedGenericClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClosedGenericClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClosedGenericClassWithManySideRelationPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClosedGenericClassWithManySideRelationPropertiesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClosedGenericClassWithOneSideRelationPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClosedGenericClassWithOneSideRelationPropertiesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithDifferentPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithDifferentPropertiesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedClassWithDifferentPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedClassWithDifferentPropertiesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'BaseClassWithoutStorageSpecificIdentifierAttributeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[BaseClassWithoutStorageSpecificIdentifierAttributeView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedClassWithStorageSpecificIdentifierAttributeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedClassWithStorageSpecificIdentifierAttributeView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CustomerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CustomerView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SpecialCustomerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SpecialCustomerView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SupplierView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SupplierView]
-- Drop foreign keys of all tables
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_EagerFetching_BaseClass_ScalarProperty2RealSideID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'EagerFetching_BaseClass')
  ALTER TABLE [dbo].[EagerFetching_BaseClass] DROP CONSTRAINT FK_EagerFetching_BaseClass_ScalarProperty2RealSideID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_EagerFetching_BaseClass_UnidirectionalPropertyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'EagerFetching_BaseClass')
  ALTER TABLE [dbo].[EagerFetching_BaseClass] DROP CONSTRAINT FK_EagerFetching_BaseClass_UnidirectionalPropertyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_EagerFetching_RelationTarget_CollectionPropertyOneSideID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'EagerFetching_RelationTarget')
  ALTER TABLE [dbo].[EagerFetching_RelationTarget] DROP CONSTRAINT FK_EagerFetching_RelationTarget_CollectionPropertyOneSideID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_EagerFetching_RelationTarget_ScalarProperty1RealSideID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'EagerFetching_RelationTarget')
  ALTER TABLE [dbo].[EagerFetching_RelationTarget] DROP CONSTRAINT FK_EagerFetching_RelationTarget_ScalarProperty1RealSideID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ConcreteInheritanceFirstDerivedClass_VectorOpposingPropertyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ConcreteInheritanceFirstDerivedClass')
  ALTER TABLE [dbo].[ConcreteInheritanceFirstDerivedClass] DROP CONSTRAINT FK_ConcreteInheritanceFirstDerivedClass_VectorOpposingPropertyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ConcreteInheritanceSecondDerivedClass_VectorOpposingPropertyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ConcreteInheritanceSecondDerivedClass')
  ALTER TABLE [dbo].[ConcreteInheritanceSecondDerivedClass] DROP CONSTRAINT FK_ConcreteInheritanceSecondDerivedClass_VectorOpposingPropertyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_MixedDomains_Target_UnidirectionalRelationPropertyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'MixedDomains_Target')
  ALTER TABLE [dbo].[MixedDomains_Target] DROP CONSTRAINT FK_MixedDomains_Target_UnidirectionalRelationPropertyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_MixedDomains_Target_RelationPropertyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'MixedDomains_Target')
  ALTER TABLE [dbo].[MixedDomains_Target] DROP CONSTRAINT FK_MixedDomains_Target_RelationPropertyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_MixedDomains_Target_CollectionPropertyNSideID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'MixedDomains_Target')
  ALTER TABLE [dbo].[MixedDomains_Target] DROP CONSTRAINT FK_MixedDomains_Target_CollectionPropertyNSideID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_MixedDomains_Target_PrivateBaseRelationPropertyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'MixedDomains_Target')
  ALTER TABLE [dbo].[MixedDomains_Target] DROP CONSTRAINT FK_MixedDomains_Target_PrivateBaseRelationPropertyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_HookedTargetClass_TargetID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'HookedTargetClass')
  ALTER TABLE [dbo].[HookedTargetClass] DROP CONSTRAINT FK_HookedTargetClass_TargetID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_MixedDomains_RelationTarget_RelationProperty2ID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'MixedDomains_RelationTarget')
  ALTER TABLE [dbo].[MixedDomains_RelationTarget] DROP CONSTRAINT FK_MixedDomains_RelationTarget_RelationProperty2ID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_MixedDomains_RelationTarget_RelationProperty3ID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'MixedDomains_RelationTarget')
  ALTER TABLE [dbo].[MixedDomains_RelationTarget] DROP CONSTRAINT FK_MixedDomains_RelationTarget_RelationProperty3ID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_SingleInheritanceBaseClass_VectorOpposingPropertyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'SingleInheritanceBaseClass')
  ALTER TABLE [dbo].[SingleInheritanceBaseClass] DROP CONSTRAINT FK_SingleInheritanceBaseClass_VectorOpposingPropertyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_SingleInheritanceObjectWithRelations_ScalarPropertyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'SingleInheritanceObjectWithRelations')
  ALTER TABLE [dbo].[SingleInheritanceObjectWithRelations] DROP CONSTRAINT FK_SingleInheritanceObjectWithRelations_ScalarPropertyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_MixedDomains_TargetWithTwoUnidirectionalMixins_ComputerID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'MixedDomains_TargetWithTwoUnidirectionalMixins')
  ALTER TABLE [dbo].[MixedDomains_TargetWithTwoUnidirectionalMixins] DROP CONSTRAINT FK_MixedDomains_TargetWithTwoUnidirectionalMixins_ComputerID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_MixedDomains_TargetWithTwoUnidirectionalMixins_Computer2ID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'MixedDomains_TargetWithTwoUnidirectionalMixins')
  ALTER TABLE [dbo].[MixedDomains_TargetWithTwoUnidirectionalMixins] DROP CONSTRAINT FK_MixedDomains_TargetWithTwoUnidirectionalMixins_Computer2ID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_MixedDomains_TargetWithUnidirectionalMixin1_ComputerID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'MixedDomains_TargetWithUnidirectionalMixin1')
  ALTER TABLE [dbo].[MixedDomains_TargetWithUnidirectionalMixin1] DROP CONSTRAINT FK_MixedDomains_TargetWithUnidirectionalMixin1_ComputerID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_MixedDomains_TargetWithUnidirectionalMixin2_ComputerID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'MixedDomains_TargetWithUnidirectionalMixin2')
  ALTER TABLE [dbo].[MixedDomains_TargetWithUnidirectionalMixin2] DROP CONSTRAINT FK_MixedDomains_TargetWithUnidirectionalMixin2_ComputerID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Ceo_CompanyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Ceo')
  ALTER TABLE [dbo].[Ceo] DROP CONSTRAINT FK_Ceo_CompanyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableWithOptionalOneToOneRelationAndOppositeDerivedClass_CompanyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableWithOptionalOneToOneRelationAndOppositeDerivedClass')
  ALTER TABLE [dbo].[TableWithOptionalOneToOneRelationAndOppositeDerivedClass] DROP CONSTRAINT FK_TableWithOptionalOneToOneRelationAndOppositeDerivedClass_CompanyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableWithRelatedClassIDColumnAndNoInheritance_TableWithGuidKeyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableWithRelatedClassIDColumnAndNoInheritance')
  ALTER TABLE [dbo].[TableWithRelatedClassIDColumnAndNoInheritance] DROP CONSTRAINT FK_TableWithRelatedClassIDColumnAndNoInheritance_TableWithGuidKeyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableWithValidRelations_TableWithGuidKeyOptionalID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableWithValidRelations')
  ALTER TABLE [dbo].[TableWithValidRelations] DROP CONSTRAINT FK_TableWithValidRelations_TableWithGuidKeyOptionalID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableWithValidRelations_TableWithGuidKeyNonOptionalID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableWithValidRelations')
  ALTER TABLE [dbo].[TableWithValidRelations] DROP CONSTRAINT FK_TableWithValidRelations_TableWithGuidKeyNonOptionalID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Client_ParentClientID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Client')
  ALTER TABLE [dbo].[Client] DROP CONSTRAINT FK_Client_ParentClientID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Computer_EmployeeID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Computer')
  ALTER TABLE [dbo].[Computer] DROP CONSTRAINT FK_Computer_EmployeeID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Company_IndustrialSectorID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Company')
  ALTER TABLE [dbo].[Company] DROP CONSTRAINT FK_Company_IndustrialSectorID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Company_ContactPersonID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Company')
  ALTER TABLE [dbo].[Company] DROP CONSTRAINT FK_Company_ContactPersonID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Employee_SupervisorID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Employee')
  ALTER TABLE [dbo].[Employee] DROP CONSTRAINT FK_Employee_SupervisorID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_FileSystemItem_ParentFolderID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'FileSystemItem')
  ALTER TABLE [dbo].[FileSystemItem] DROP CONSTRAINT FK_FileSystemItem_ParentFolderID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_FileSystemItem_ParentFolderRelation' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'FileSystemItem')
  ALTER TABLE [dbo].[FileSystemItem] DROP CONSTRAINT FK_FileSystemItem_ParentFolderRelation
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Location_ClientID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Location')
  ALTER TABLE [dbo].[Location] DROP CONSTRAINT FK_Location_ClientID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Order_CustomerID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Order')
  ALTER TABLE [dbo].[Order] DROP CONSTRAINT FK_Order_CustomerID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_OrderItem_OrderID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'OrderItem')
  ALTER TABLE [dbo].[OrderItem] DROP CONSTRAINT FK_OrderItem_OrderID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_OrderTicket_OrderID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'OrderTicket')
  ALTER TABLE [dbo].[OrderTicket] DROP CONSTRAINT FK_OrderTicket_OrderID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Person_AssociatedCustomerCompanyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Person')
  ALTER TABLE [dbo].[Person] DROP CONSTRAINT FK_Person_AssociatedCustomerCompanyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ProductReview_ProductID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ProductReview')
  ALTER TABLE [dbo].[ProductReview] DROP CONSTRAINT FK_ProductReview_ProductID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ProductReview_ReviewerID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ProductReview')
  ALTER TABLE [dbo].[ProductReview] DROP CONSTRAINT FK_ProductReview_ReviewerID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithBothEndPointsOnSameClass_ParentID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithBothEndPointsOnSameClass')
  ALTER TABLE [dbo].[ClassWithBothEndPointsOnSameClass] DROP CONSTRAINT FK_ClassWithBothEndPointsOnSameClass_ParentID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithManySideRelationProperties_BaseUnidirectionalID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClassWithManySideRelationProperties_BaseUnidirectionalID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithManySideRelationProperties_BaseBidirectionalOneToOneID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClassWithManySideRelationProperties_BaseBidirectionalOneToOneID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithManySideRelationProperties_BaseBidirectionalOneToManyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClassWithManySideRelationProperties_BaseBidirectionalOneToManyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithManySideRelationProperties_BasePrivateUnidirectionalID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClassWithManySideRelationProperties_BasePrivateUnidirectionalID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithManySideRelationProperties_BasePrivateBidirectionalOneToOneID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClassWithManySideRelationProperties_BasePrivateBidirectionalOneToOneID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithManySideRelationProperties_BasePrivateBidirectionalOneToManyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClassWithManySideRelationProperties_BasePrivateBidirectionalOneToManyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithManySideRelationProperties_NoAttributeID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClassWithManySideRelationProperties_NoAttributeID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithManySideRelationProperties_NotNullableID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClassWithManySideRelationProperties_NotNullableID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithManySideRelationProperties_UnidirectionalID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClassWithManySideRelationProperties_UnidirectionalID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithManySideRelationProperties_BidirectionalOneToOneID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClassWithManySideRelationProperties_BidirectionalOneToOneID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithManySideRelationProperties_BidirectionalOneToManyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClassWithManySideRelationProperties_BidirectionalOneToManyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClosedGenericClassWithManySideRelationProperties_BaseUnidirectionalID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClosedGenericClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClosedGenericClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClosedGenericClassWithManySideRelationProperties_BaseUnidirectionalID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClosedGenericClassWithManySideRelationProperties_BaseBidirectionalOneToOneID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClosedGenericClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClosedGenericClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClosedGenericClassWithManySideRelationProperties_BaseBidirectionalOneToOneID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClosedGenericClassWithManySideRelationProperties_BaseBidirectionalOneToManyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClosedGenericClassWithManySideRelationProperties')
  ALTER TABLE [dbo].[ClosedGenericClassWithManySideRelationProperties] DROP CONSTRAINT FK_ClosedGenericClassWithManySideRelationProperties_BaseBidirectionalOneToManyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithDifferentProperties_BaseUnidirectionalOneToOneID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithDifferentProperties')
  ALTER TABLE [dbo].[ClassWithDifferentProperties] DROP CONSTRAINT FK_ClassWithDifferentProperties_BaseUnidirectionalOneToOneID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithDifferentProperties_BasePrivateUnidirectionalOneToOneID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithDifferentProperties')
  ALTER TABLE [dbo].[ClassWithDifferentProperties] DROP CONSTRAINT FK_ClassWithDifferentProperties_BasePrivateUnidirectionalOneToOneID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ClassWithDifferentProperties_UnidirectionalOneToOneID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ClassWithDifferentProperties')
  ALTER TABLE [dbo].[ClassWithDifferentProperties] DROP CONSTRAINT FK_ClassWithDifferentProperties_UnidirectionalOneToOneID
-- Drop all tables
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithPropertiesHavingStorageClassAttribute' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithPropertiesHavingStorageClassAttribute]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EagerFetching_BaseClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[EagerFetching_BaseClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EagerFetching_RelationTarget' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[EagerFetching_RelationTarget]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'NestedDomainObject' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[NestedDomainObject]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ConcreteInheritanceFirstDerivedClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ConcreteInheritanceFirstDerivedClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ConcreteInheritanceObjectWithRelations' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ConcreteInheritanceObjectWithRelations]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ConcreteInheritanceSecondDerivedClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ConcreteInheritanceSecondDerivedClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_Target' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[MixedDomains_Target]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'HookedTargetClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[HookedTargetClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'InheritanceRootInheritingPersistentMixin' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[InheritanceRootInheritingPersistentMixin]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_RelationTarget' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[MixedDomains_RelationTarget]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SingleInheritanceBaseClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SingleInheritanceBaseClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SingleInheritanceObjectWithRelations' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SingleInheritanceObjectWithRelations]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TargetClassForBehavioralMixin' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TargetClassForBehavioralMixin]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TargetClassForMixinWithState' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TargetClassForMixinWithState]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_TargetClassWithSameInterfaceAsPersistentMixin' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[MixedDomains_TargetClassWithSameInterfaceAsPersistentMixin]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_TargetWithTwoUnidirectionalMixins' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[MixedDomains_TargetWithTwoUnidirectionalMixins]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_TargetWithUnidirectionalMixin1' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[MixedDomains_TargetWithUnidirectionalMixin1]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_TargetWithUnidirectionalMixin2' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[MixedDomains_TargetWithUnidirectionalMixin2]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Ceo' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Ceo]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassNotInMapping' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassNotInMapping]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithAllDataTypes]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithEnumNotDefiningZero' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithEnumNotDefiningZero]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithGuidKey' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithGuidKey]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithNonPublicProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithNonPublicProperties]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithOptionalOneToOneRelationAndOppositeDerivedClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithOptionalOneToOneRelationAndOppositeDerivedClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithoutProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithoutProperties]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithPropertyTypeImplementingIStructuralEquatable' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithPropertyTypeImplementingIStructuralEquatable]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithRelatedClassIDColumnAndNoInheritance' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithRelatedClassIDColumnAndNoInheritance]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithValidRelations' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithValidRelations]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Client' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Client]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Computer' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Computer]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Company' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Company]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Employee' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Employee]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'FileSystemItem' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[FileSystemItem]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'IndustrialSector' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[IndustrialSector]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StorageGroupClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[StorageGroupClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Location' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Location]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Order]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderItem' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[OrderItem]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderTicket' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[OrderTicket]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Person' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Person]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Product' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Product]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ProductReview' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ProductReview]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassIDForClassHavingClassIDAttribute' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassIDForClassHavingClassIDAttribute]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassHavingStorageSpecificIdentifierAttributeTable' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassHavingStorageSpecificIdentifierAttributeTable]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithBinaryProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithBinaryProperties]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithBothEndPointsOnSameClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithBothEndPointsOnSameClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithExtensibleEnumProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithExtensibleEnumProperties]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithManySideRelationProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithManySideRelationProperties]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithOneSideRelationProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithOneSideRelationProperties]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClosedGenericClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClosedGenericClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClosedGenericClassWithManySideRelationProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClosedGenericClassWithManySideRelationProperties]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClosedGenericClassWithOneSideRelationProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClosedGenericClassWithOneSideRelationProperties]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithDifferentProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithDifferentProperties]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DerivedClassWithStorageSpecificIdentifierAttribute' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[DerivedClassWithStorageSpecificIdentifierAttribute]
