--
-- This file is auto-generated. Do not edit manually.
-- See TestDomainDBScriptGenerationTest for the relevant tests.
--

USE DBPrefix_TestDomain
-- Create all tables
CREATE TABLE [dbo].[ClassWithPropertiesHavingStorageClassAttribute]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Persistent] int NOT NULL,
  CONSTRAINT [PK_ClassWithPropertiesHavingStorageClassAttribute] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[EagerFetching_BaseClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [ScalarProperty2RealSideID] uniqueidentifier NULL,
  [UnidirectionalPropertyID] uniqueidentifier NULL,
  CONSTRAINT [PK_EagerFetching_BaseClass] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[EagerFetching_RelationTarget]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [CollectionPropertyOneSideID] uniqueidentifier NULL,
  [CollectionPropertyOneSideIDClassID] varchar (100) NULL,
  [ScalarProperty1RealSideID] uniqueidentifier NULL,
  [ScalarProperty1RealSideIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_EagerFetching_RelationTarget] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[NestedDomainObject]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_NestedDomainObject] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ConcreteInheritanceFirstDerivedClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [BaseProperty] nvarchar (max) NULL,
  [VectorOpposingPropertyID] uniqueidentifier NULL,
  [FirstDerivedProperty] nvarchar (max) NULL,
  [PersistentProperty] nvarchar (max) NULL,
  CONSTRAINT [PK_ConcreteInheritanceFirstDerivedClass] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ConcreteInheritanceObjectWithRelations]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [ScalarPropertyID] uniqueidentifier NULL,
  [ScalarPropertyIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_ConcreteInheritanceObjectWithRelations] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ConcreteInheritanceSecondDerivedClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [BaseProperty] nvarchar (max) NULL,
  [VectorOpposingPropertyID] uniqueidentifier NULL,
  [SecondDerivedProperty] nvarchar (max) NULL,
  [PersistentProperty] nvarchar (max) NULL,
  CONSTRAINT [PK_ConcreteInheritanceSecondDerivedClass] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[HookedTargetClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Property] int NOT NULL,
  [TargetID] uniqueidentifier NULL,
  CONSTRAINT [PK_HookedTargetClass] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[InheritanceRootInheritingPersistentMixin]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [PersistentProperty] int NOT NULL,
  [PersistentRelationPropertyID] varchar (255) NULL,
  CONSTRAINT [PK_InheritanceRootInheritingPersistentMixin] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[MixedDomains_RelationTarget]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [RelationProperty2ID] uniqueidentifier NULL,
  [RelationProperty2IDClassID] varchar (100) NULL,
  [RelationProperty3ID] uniqueidentifier NULL,
  [RelationProperty3IDClassID] varchar (100) NULL,
  CONSTRAINT [PK_MixedDomains_RelationTarget] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[SingleInheritanceBaseClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [BaseProperty] nvarchar (max) NULL,
  [VectorOpposingPropertyID] uniqueidentifier NULL,
  [FirstDerivedProperty] nvarchar (max) NULL,
  [PersistentProperty] nvarchar (max) NULL,
  [SecondDerivedProperty] nvarchar (max) NULL,
  CONSTRAINT [PK_SingleInheritanceBaseClass] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[SingleInheritanceObjectWithRelations]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [ScalarPropertyID] uniqueidentifier NULL,
  [ScalarPropertyIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_SingleInheritanceObjectWithRelations] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TargetClassForBehavioralMixin]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Property] nvarchar (max) NULL,
  CONSTRAINT [PK_TargetClassForBehavioralMixin] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TargetClassForMixinWithState]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_TargetClassForMixinWithState] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[MixedDomains_Target]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [PersistentProperty] int NOT NULL,
  [ExtraPersistentProperty] int NOT NULL,
  [UnidirectionalRelationPropertyID] uniqueidentifier NULL,
  [RelationPropertyID] uniqueidentifier NULL,
  [CollectionPropertyNSideID] uniqueidentifier NULL,
  [PrivateBaseRelationPropertyID] uniqueidentifier NULL,
  CONSTRAINT [PK_MixedDomains_Target] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[MixedDomains_TargetClassWithSameInterfaceAsPersistentMixin]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [PersistentPropertyRedirectedToMixin] int NOT NULL,
  CONSTRAINT [PK_MixedDomains_TargetClassWithSameInterfaceAsPersistentMixin] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[MixedDomains_TargetWithTwoUnidirectionalMixins]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [ComputerID] uniqueidentifier NULL,
  [Computer2ID] uniqueidentifier NULL,
  CONSTRAINT [PK_MixedDomains_TargetWithTwoUnidirectionalMixins] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[MixedDomains_TargetWithUnidirectionalMixin1]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [ComputerID] uniqueidentifier NULL,
  CONSTRAINT [PK_MixedDomains_TargetWithUnidirectionalMixin1] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[MixedDomains_TargetWithUnidirectionalMixin2]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [ComputerID] uniqueidentifier NULL,
  CONSTRAINT [PK_MixedDomains_TargetWithUnidirectionalMixin2] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[Ceo]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [CompanyID] uniqueidentifier NULL,
  [CompanyIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_Ceo] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassNotInMapping]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_ClassNotInMapping] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableWithAllDataTypes]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Boolean] bit NOT NULL,
  [Byte] tinyint NOT NULL,
  [Date] datetime NOT NULL,
  [DateTime] datetime NOT NULL,
  [Decimal] decimal (38, 3) NOT NULL,
  [Double] float NOT NULL,
  [Enum] int NOT NULL,
  [Flags] int NOT NULL,
  [ExtensibleEnum] varchar (70) NOT NULL,
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
  [NaFlags] int NULL,
  [NaGuid] uniqueidentifier NULL,
  [NaInt16] smallint NULL,
  [NaInt32] int NULL,
  [NaInt64] bigint NULL,
  [NaSingle] real NULL,
  [StringWithNullValue] nvarchar (100) NULL,
  [ExtensibleEnumWithNullValue] varchar (70) NULL,
  [NaBooleanWithNullValue] bit NULL,
  [NaByteWithNullValue] tinyint NULL,
  [NaDateWithNullValue] datetime NULL,
  [NaDateTimeWithNullValue] datetime NULL,
  [NaDecimalWithNullValue] decimal (38, 3) NULL,
  [NaDoubleWithNullValue] float NULL,
  [NaEnumWithNullValue] int NULL,
  [NaFlagsWithNullValue] int NULL,
  [NaGuidWithNullValue] uniqueidentifier NULL,
  [NaInt16WithNullValue] smallint NULL,
  [NaInt32WithNullValue] int NULL,
  [NaInt64WithNullValue] bigint NULL,
  [NaSingleWithNullValue] real NULL,
  [NullableBinary] varbinary (1000) NULL,
  CONSTRAINT [PK_TableWithAllDataTypes] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassWithEnumNotDefiningZero]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [EnumValue] int NOT NULL,
  CONSTRAINT [PK_ClassWithEnumNotDefiningZero] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableWithGuidKey]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_TableWithGuidKey] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassWithNonPublicProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [PublicGetSet] int NOT NULL,
  [PublicGetProtectedSet] int NOT NULL,
  [ProtectedGetSet] int NOT NULL,
  [PrivateGetSet] int NOT NULL,
  CONSTRAINT [PK_ClassWithNonPublicProperties] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableWithOptionalOneToOneRelationAndOppositeDerivedClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [CompanyID] uniqueidentifier NULL,
  [CompanyIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_TableWithOptionalOneToOneRelationAndOppositeDerivedClass] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassWithoutProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_ClassWithoutProperties] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassWithPropertyTypeImplementingIStructuralEquatable]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_ClassWithPropertyTypeImplementingIStructuralEquatable] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableWithRelatedClassIDColumnAndNoInheritance]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [TableWithGuidKeyID] uniqueidentifier NULL,
  CONSTRAINT [PK_TableWithRelatedClassIDColumnAndNoInheritance] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[TableWithValidRelations]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [TableWithGuidKeyOptionalID] uniqueidentifier NULL,
  [TableWithGuidKeyNonOptionalID] uniqueidentifier NULL,
  CONSTRAINT [PK_TableWithValidRelations] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[Client]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [ParentClientID] uniqueidentifier NULL,
  CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[Company]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [IndustrialSectorID] uniqueidentifier NULL,
  [CustomerSince] datetime NULL,
  [CustomerType] int NULL,
  [ContactPersonID] uniqueidentifier NULL,
  [NumberOfShops] int NULL,
  [SupplierQuality] int NULL,
  CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[Computer]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [SerialNumber] nvarchar (20) NOT NULL,
  [EmployeeID] uniqueidentifier NULL,
  CONSTRAINT [PK_Computer] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[Employee]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [SupervisorID] uniqueidentifier NULL,
  CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[FileSystemItem]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [ParentFolderID] uniqueidentifier NULL,
  [ParentFolderIDClassID] varchar (100) NULL,
  [ParentFolderRelation] uniqueidentifier NULL,
  [ParentFolderRelationClassID] varchar (100) NULL,
  CONSTRAINT [PK_FileSystemItem] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[IndustrialSector]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  CONSTRAINT [PK_IndustrialSector] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[StorageGroupClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [AboveInheritanceIdentifier] nvarchar (100) NOT NULL,
  [StorageGroupClassIdentifier] nvarchar (100) NOT NULL,
  CONSTRAINT [PK_StorageGroupClass] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ComplexOrder]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [OrderNumber] int NULL,
  [ComplexOrderName] nvarchar (max) NULL,
  CONSTRAINT [PK_ComplexOrder] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[Implementor1]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [InterfaceProperty] int NOT NULL,
  [OnlyImplementor1Property] int NOT NULL,
  CONSTRAINT [PK_Implementor1] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[Implementor2]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [InterfaceProperty] int NOT NULL,
  [OnlyImplementor2Property] int NOT NULL,
  CONSTRAINT [PK_Implementor2] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[OnlyImplementor]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [InterfaceProperty] int NULL,
  [OnlyImplementorProperty] int NOT NULL,
  CONSTRAINT [PK_OnlyImplementor] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[SimpleOrder]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [OrderNumber] int NULL,
  [SimpleOrderName] nvarchar (max) NULL,
  CONSTRAINT [PK_SimpleOrder] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[SimpleOrderItem]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Position] int NULL,
  [Product] nvarchar (100) NULL,
  [OrderID] uniqueidentifier NULL,
  [OrderIDClassID] varchar (100) NULL,
  [SimpleOrderItemName] nvarchar (max) NULL,
  CONSTRAINT [PK_SimpleOrderItem] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[Location]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [ClientID] uniqueidentifier NULL,
  CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[Order]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [OrderNo] int NOT NULL,
  [DeliveryDate] datetime NOT NULL,
  [OfficialID] varchar (255) NULL,
  [CustomerID] uniqueidentifier NULL,
  [CustomerIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[OrderItem]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Position] int NOT NULL,
  [Product] nvarchar (100) NOT NULL,
  [OrderID] uniqueidentifier NULL,
  CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[OrderTicket]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [FileName] nvarchar (255) NOT NULL,
  [OrderID] uniqueidentifier NULL,
  CONSTRAINT [PK_OrderTicket] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[Person]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [AssociatedCustomerCompanyID] uniqueidentifier NULL,
  [AssociatedCustomerCompanyIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[Product]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [Price] decimal (38, 3) NOT NULL,
  CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ProductReview]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [ProductID] uniqueidentifier NULL,
  [ReviewerID] uniqueidentifier NULL,
  [CreatedAt] datetime NOT NULL,
  [Comment] nvarchar (1000) NOT NULL,
  CONSTRAINT [PK_ProductReview] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassIDForClassHavingClassIDAttribute]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_ClassIDForClassHavingClassIDAttribute] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassHavingStorageSpecificIdentifierAttributeTable]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [NoAttribute] int NOT NULL,
  [CustomName] int NOT NULL,
  CONSTRAINT [PK_ClassHavingStorageSpecificIdentifierAttributeTable] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassWithBinaryProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [NoAttribute] varbinary (max) NULL,
  [NullableFromAttribute] varbinary (max) NULL,
  [NotNullable] varbinary (max) NOT NULL,
  [MaximumLength] varbinary (100) NULL,
  [NotNullableAndMaximumLength] varbinary (100) NOT NULL,
  CONSTRAINT [PK_ClassWithBinaryProperties] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassWithBothEndPointsOnSameClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [ParentID] uniqueidentifier NULL,
  CONSTRAINT [PK_ClassWithBothEndPointsOnSameClass] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassWithDifferentProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [BaseString] nvarchar (max) NULL,
  [BaseUnidirectionalOneToOneID] uniqueidentifier NULL,
  [BasePrivateUnidirectionalOneToOneID] uniqueidentifier NULL,
  [Int32] int NOT NULL,
  [String] nvarchar (max) NULL,
  [UnidirectionalOneToOneID] uniqueidentifier NULL,
  [PrivateString] nvarchar (max) NULL,
  [OtherString] nvarchar (max) NULL,
  [NewString] nvarchar (max) NULL,
  [DerivedPrivateString] nvarchar (max) NULL,
  CONSTRAINT [PK_ClassWithDifferentProperties] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassWithExtensibleEnumProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [NoAttribute] varchar (113) NULL,
  [NullableFromAttribute] varchar (113) NULL,
  [NotNullable] varchar (113) NOT NULL,
  CONSTRAINT [PK_ClassWithExtensibleEnumProperties] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassWithManySideRelationProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [BaseUnidirectionalID] uniqueidentifier NULL,
  [BaseBidirectionalOneToOneID] uniqueidentifier NULL,
  [BaseBidirectionalOneToManyID] uniqueidentifier NULL,
  [BasePrivateUnidirectionalID] uniqueidentifier NULL,
  [BasePrivateBidirectionalOneToOneID] uniqueidentifier NULL,
  [BasePrivateBidirectionalOneToManyID] uniqueidentifier NULL,
  [NoAttributeID] uniqueidentifier NULL,
  [NotNullableID] uniqueidentifier NULL,
  [UnidirectionalID] uniqueidentifier NULL,
  [BidirectionalOneToOneID] uniqueidentifier NULL,
  [BidirectionalOneToManyID] uniqueidentifier NULL,
  CONSTRAINT [PK_ClassWithManySideRelationProperties] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClassWithOneSideRelationProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_ClassWithOneSideRelationProperties] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClosedGenericClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_ClosedGenericClass] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClosedGenericClassWithManySideRelationProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [BaseUnidirectionalID] uniqueidentifier NULL,
  [BaseBidirectionalOneToOneID] uniqueidentifier NULL,
  [BaseBidirectionalOneToManyID] uniqueidentifier NULL,
  CONSTRAINT [PK_ClosedGenericClassWithManySideRelationProperties] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ClosedGenericClassWithOneSideRelationProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_ClosedGenericClassWithOneSideRelationProperties] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[DerivedClassWithStorageSpecificIdentifierAttribute]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_DerivedClassWithStorageSpecificIdentifierAttribute] PRIMARY KEY CLUSTERED ([ID])
)
-- Create foreign key constraints for tables that were created above
ALTER TABLE [dbo].[EagerFetching_BaseClass] ADD
  CONSTRAINT [FK_EagerFetching_BaseClass_ScalarProperty2RealSideID] FOREIGN KEY ([ScalarProperty2RealSideID]) REFERENCES [dbo].[EagerFetching_RelationTarget] ([ID])
ALTER TABLE [dbo].[EagerFetching_BaseClass] ADD
  CONSTRAINT [FK_EagerFetching_BaseClass_UnidirectionalPropertyID] FOREIGN KEY ([UnidirectionalPropertyID]) REFERENCES [dbo].[EagerFetching_RelationTarget] ([ID])
ALTER TABLE [dbo].[EagerFetching_RelationTarget] ADD
  CONSTRAINT [FK_EagerFetching_RelationTarget_CollectionPropertyOneSideID] FOREIGN KEY ([CollectionPropertyOneSideID]) REFERENCES [dbo].[EagerFetching_BaseClass] ([ID])
ALTER TABLE [dbo].[EagerFetching_RelationTarget] ADD
  CONSTRAINT [FK_EagerFetching_RelationTarget_ScalarProperty1RealSideID] FOREIGN KEY ([ScalarProperty1RealSideID]) REFERENCES [dbo].[EagerFetching_BaseClass] ([ID])
ALTER TABLE [dbo].[ConcreteInheritanceFirstDerivedClass] ADD
  CONSTRAINT [FK_ConcreteInheritanceFirstDerivedClass_VectorOpposingPropertyID] FOREIGN KEY ([VectorOpposingPropertyID]) REFERENCES [dbo].[ConcreteInheritanceObjectWithRelations] ([ID])
ALTER TABLE [dbo].[ConcreteInheritanceSecondDerivedClass] ADD
  CONSTRAINT [FK_ConcreteInheritanceSecondDerivedClass_VectorOpposingPropertyID] FOREIGN KEY ([VectorOpposingPropertyID]) REFERENCES [dbo].[ConcreteInheritanceObjectWithRelations] ([ID])
ALTER TABLE [dbo].[HookedTargetClass] ADD
  CONSTRAINT [FK_HookedTargetClass_TargetID] FOREIGN KEY ([TargetID]) REFERENCES [dbo].[HookedTargetClass] ([ID])
ALTER TABLE [dbo].[MixedDomains_RelationTarget] ADD
  CONSTRAINT [FK_MixedDomains_RelationTarget_RelationProperty2ID] FOREIGN KEY ([RelationProperty2ID]) REFERENCES [dbo].[MixedDomains_Target] ([ID])
ALTER TABLE [dbo].[MixedDomains_RelationTarget] ADD
  CONSTRAINT [FK_MixedDomains_RelationTarget_RelationProperty3ID] FOREIGN KEY ([RelationProperty3ID]) REFERENCES [dbo].[MixedDomains_Target] ([ID])
ALTER TABLE [dbo].[SingleInheritanceBaseClass] ADD
  CONSTRAINT [FK_SingleInheritanceBaseClass_VectorOpposingPropertyID] FOREIGN KEY ([VectorOpposingPropertyID]) REFERENCES [dbo].[SingleInheritanceObjectWithRelations] ([ID])
ALTER TABLE [dbo].[SingleInheritanceObjectWithRelations] ADD
  CONSTRAINT [FK_SingleInheritanceObjectWithRelations_ScalarPropertyID] FOREIGN KEY ([ScalarPropertyID]) REFERENCES [dbo].[SingleInheritanceBaseClass] ([ID])
ALTER TABLE [dbo].[MixedDomains_Target] ADD
  CONSTRAINT [FK_MixedDomains_Target_UnidirectionalRelationPropertyID] FOREIGN KEY ([UnidirectionalRelationPropertyID]) REFERENCES [dbo].[MixedDomains_RelationTarget] ([ID])
ALTER TABLE [dbo].[MixedDomains_Target] ADD
  CONSTRAINT [FK_MixedDomains_Target_RelationPropertyID] FOREIGN KEY ([RelationPropertyID]) REFERENCES [dbo].[MixedDomains_RelationTarget] ([ID])
ALTER TABLE [dbo].[MixedDomains_Target] ADD
  CONSTRAINT [FK_MixedDomains_Target_CollectionPropertyNSideID] FOREIGN KEY ([CollectionPropertyNSideID]) REFERENCES [dbo].[MixedDomains_RelationTarget] ([ID])
ALTER TABLE [dbo].[MixedDomains_Target] ADD
  CONSTRAINT [FK_MixedDomains_Target_PrivateBaseRelationPropertyID] FOREIGN KEY ([PrivateBaseRelationPropertyID]) REFERENCES [dbo].[MixedDomains_RelationTarget] ([ID])
ALTER TABLE [dbo].[MixedDomains_TargetWithTwoUnidirectionalMixins] ADD
  CONSTRAINT [FK_MixedDomains_TargetWithTwoUnidirectionalMixins_ComputerID] FOREIGN KEY ([ComputerID]) REFERENCES [dbo].[Computer] ([ID])
ALTER TABLE [dbo].[MixedDomains_TargetWithTwoUnidirectionalMixins] ADD
  CONSTRAINT [FK_MixedDomains_TargetWithTwoUnidirectionalMixins_Computer2ID] FOREIGN KEY ([Computer2ID]) REFERENCES [dbo].[Computer] ([ID])
ALTER TABLE [dbo].[MixedDomains_TargetWithUnidirectionalMixin1] ADD
  CONSTRAINT [FK_MixedDomains_TargetWithUnidirectionalMixin1_ComputerID] FOREIGN KEY ([ComputerID]) REFERENCES [dbo].[Computer] ([ID])
ALTER TABLE [dbo].[MixedDomains_TargetWithUnidirectionalMixin2] ADD
  CONSTRAINT [FK_MixedDomains_TargetWithUnidirectionalMixin2_ComputerID] FOREIGN KEY ([ComputerID]) REFERENCES [dbo].[Computer] ([ID])
ALTER TABLE [dbo].[Ceo] ADD
  CONSTRAINT [FK_Ceo_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [dbo].[Company] ([ID])
ALTER TABLE [dbo].[TableWithOptionalOneToOneRelationAndOppositeDerivedClass] ADD
  CONSTRAINT [FK_TableWithOptionalOneToOneRelationAndOppositeDerivedClass_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [dbo].[Company] ([ID])
ALTER TABLE [dbo].[TableWithRelatedClassIDColumnAndNoInheritance] ADD
  CONSTRAINT [FK_TableWithRelatedClassIDColumnAndNoInheritance_TableWithGuidKeyID] FOREIGN KEY ([TableWithGuidKeyID]) REFERENCES [dbo].[TableWithGuidKey] ([ID])
ALTER TABLE [dbo].[TableWithValidRelations] ADD
  CONSTRAINT [FK_TableWithValidRelations_TableWithGuidKeyOptionalID] FOREIGN KEY ([TableWithGuidKeyOptionalID]) REFERENCES [dbo].[TableWithGuidKey] ([ID])
ALTER TABLE [dbo].[TableWithValidRelations] ADD
  CONSTRAINT [FK_TableWithValidRelations_TableWithGuidKeyNonOptionalID] FOREIGN KEY ([TableWithGuidKeyNonOptionalID]) REFERENCES [dbo].[TableWithGuidKey] ([ID])
ALTER TABLE [dbo].[Client] ADD
  CONSTRAINT [FK_Client_ParentClientID] FOREIGN KEY ([ParentClientID]) REFERENCES [dbo].[Client] ([ID])
ALTER TABLE [dbo].[Company] ADD
  CONSTRAINT [FK_Company_IndustrialSectorID] FOREIGN KEY ([IndustrialSectorID]) REFERENCES [dbo].[IndustrialSector] ([ID])
ALTER TABLE [dbo].[Company] ADD
  CONSTRAINT [FK_Company_ContactPersonID] FOREIGN KEY ([ContactPersonID]) REFERENCES [dbo].[Person] ([ID])
ALTER TABLE [dbo].[Computer] ADD
  CONSTRAINT [FK_Computer_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([ID])
ALTER TABLE [dbo].[Employee] ADD
  CONSTRAINT [FK_Employee_SupervisorID] FOREIGN KEY ([SupervisorID]) REFERENCES [dbo].[Employee] ([ID])
ALTER TABLE [dbo].[FileSystemItem] ADD
  CONSTRAINT [FK_FileSystemItem_ParentFolderID] FOREIGN KEY ([ParentFolderID]) REFERENCES [dbo].[FileSystemItem] ([ID])
ALTER TABLE [dbo].[FileSystemItem] ADD
  CONSTRAINT [FK_FileSystemItem_ParentFolderRelation] FOREIGN KEY ([ParentFolderRelation]) REFERENCES [dbo].[FileSystemItem] ([ID])
ALTER TABLE [dbo].[Location] ADD
  CONSTRAINT [FK_Location_ClientID] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Client] ([ID])
ALTER TABLE [dbo].[Order] ADD
  CONSTRAINT [FK_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Company] ([ID])
ALTER TABLE [dbo].[OrderItem] ADD
  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])
ALTER TABLE [dbo].[OrderTicket] ADD
  CONSTRAINT [FK_OrderTicket_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])
ALTER TABLE [dbo].[Person] ADD
  CONSTRAINT [FK_Person_AssociatedCustomerCompanyID] FOREIGN KEY ([AssociatedCustomerCompanyID]) REFERENCES [dbo].[Company] ([ID])
ALTER TABLE [dbo].[ProductReview] ADD
  CONSTRAINT [FK_ProductReview_ProductID] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ID])
ALTER TABLE [dbo].[ProductReview] ADD
  CONSTRAINT [FK_ProductReview_ReviewerID] FOREIGN KEY ([ReviewerID]) REFERENCES [dbo].[Person] ([ID])
ALTER TABLE [dbo].[ClassWithBothEndPointsOnSameClass] ADD
  CONSTRAINT [FK_ClassWithBothEndPointsOnSameClass_ParentID] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[ClassWithBothEndPointsOnSameClass] ([ID])
ALTER TABLE [dbo].[ClassWithDifferentProperties] ADD
  CONSTRAINT [FK_ClassWithDifferentProperties_BaseUnidirectionalOneToOneID] FOREIGN KEY ([BaseUnidirectionalOneToOneID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithDifferentProperties] ADD
  CONSTRAINT [FK_ClassWithDifferentProperties_BasePrivateUnidirectionalOneToOneID] FOREIGN KEY ([BasePrivateUnidirectionalOneToOneID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithDifferentProperties] ADD
  CONSTRAINT [FK_ClassWithDifferentProperties_UnidirectionalOneToOneID] FOREIGN KEY ([UnidirectionalOneToOneID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClassWithManySideRelationProperties_BaseUnidirectionalID] FOREIGN KEY ([BaseUnidirectionalID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClassWithManySideRelationProperties_BaseBidirectionalOneToOneID] FOREIGN KEY ([BaseBidirectionalOneToOneID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClassWithManySideRelationProperties_BaseBidirectionalOneToManyID] FOREIGN KEY ([BaseBidirectionalOneToManyID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClassWithManySideRelationProperties_BasePrivateUnidirectionalID] FOREIGN KEY ([BasePrivateUnidirectionalID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClassWithManySideRelationProperties_BasePrivateBidirectionalOneToOneID] FOREIGN KEY ([BasePrivateBidirectionalOneToOneID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClassWithManySideRelationProperties_BasePrivateBidirectionalOneToManyID] FOREIGN KEY ([BasePrivateBidirectionalOneToManyID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClassWithManySideRelationProperties_NoAttributeID] FOREIGN KEY ([NoAttributeID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClassWithManySideRelationProperties_NotNullableID] FOREIGN KEY ([NotNullableID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClassWithManySideRelationProperties_UnidirectionalID] FOREIGN KEY ([UnidirectionalID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClassWithManySideRelationProperties_BidirectionalOneToOneID] FOREIGN KEY ([BidirectionalOneToOneID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClassWithManySideRelationProperties_BidirectionalOneToManyID] FOREIGN KEY ([BidirectionalOneToManyID]) REFERENCES [dbo].[ClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClosedGenericClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClosedGenericClassWithManySideRelationProperties_BaseUnidirectionalID] FOREIGN KEY ([BaseUnidirectionalID]) REFERENCES [dbo].[ClosedGenericClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClosedGenericClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClosedGenericClassWithManySideRelationProperties_BaseBidirectionalOneToOneID] FOREIGN KEY ([BaseBidirectionalOneToOneID]) REFERENCES [dbo].[ClosedGenericClassWithOneSideRelationProperties] ([ID])
ALTER TABLE [dbo].[ClosedGenericClassWithManySideRelationProperties] ADD
  CONSTRAINT [FK_ClosedGenericClassWithManySideRelationProperties_BaseBidirectionalOneToManyID] FOREIGN KEY ([BaseBidirectionalOneToManyID]) REFERENCES [dbo].[ClosedGenericClassWithOneSideRelationProperties] ([ID])
-- Create a view for every class
GO
CREATE VIEW [dbo].[ClassWithPropertiesHavingStorageClassAttributeView] ([ID], [ClassID], [Timestamp], [Persistent])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Persistent]
    FROM [dbo].[ClassWithPropertiesHavingStorageClassAttribute]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[EagerFetching_BaseClassView] ([ID], [ClassID], [Timestamp], [ScalarProperty2RealSideID], [UnidirectionalPropertyID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ScalarProperty2RealSideID], [UnidirectionalPropertyID]
    FROM [dbo].[EagerFetching_BaseClass]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[EagerFetching_DerivedClass1View] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[EagerFetching_BaseClass]
    WHERE [ClassID] IN ('EagerFetching_DerivedClass1')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[EagerFetching_DerivedClass2View] ([ID], [ClassID], [Timestamp], [ScalarProperty2RealSideID], [UnidirectionalPropertyID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ScalarProperty2RealSideID], [UnidirectionalPropertyID]
    FROM [dbo].[EagerFetching_BaseClass]
    WHERE [ClassID] IN ('EagerFetching_DerivedClass2')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[EagerFetching_RelationTargetView] ([ID], [ClassID], [Timestamp], [CollectionPropertyOneSideID], [CollectionPropertyOneSideIDClassID], [ScalarProperty1RealSideID], [ScalarProperty1RealSideIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CollectionPropertyOneSideID], [CollectionPropertyOneSideIDClassID], [ScalarProperty1RealSideID], [ScalarProperty1RealSideIDClassID]
    FROM [dbo].[EagerFetching_RelationTarget]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[NestedDomainObjectView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[NestedDomainObject]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ConcreteInheritanceBaseClassView] ([ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], [FirstDerivedProperty], [PersistentProperty], [SecondDerivedProperty])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], [FirstDerivedProperty], [PersistentProperty], NULL
    FROM [dbo].[ConcreteInheritanceFirstDerivedClass]
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], NULL, [PersistentProperty], [SecondDerivedProperty]
    FROM [dbo].[ConcreteInheritanceSecondDerivedClass]
GO
CREATE VIEW [dbo].[ConcreteInheritanceFirstDerivedClassView] ([ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], [FirstDerivedProperty], [PersistentProperty])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], [FirstDerivedProperty], [PersistentProperty]
    FROM [dbo].[ConcreteInheritanceFirstDerivedClass]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ConcreteInheritanceObjectWithRelationsView] ([ID], [ClassID], [Timestamp], [ScalarPropertyID], [ScalarPropertyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ScalarPropertyID], [ScalarPropertyIDClassID]
    FROM [dbo].[ConcreteInheritanceObjectWithRelations]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ConcreteInheritanceSecondDerivedClassView] ([ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], [SecondDerivedProperty], [PersistentProperty])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], [SecondDerivedProperty], [PersistentProperty]
    FROM [dbo].[ConcreteInheritanceSecondDerivedClass]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[DerivedDerivedTargetClassForPersistentMixinView] ([ID], [ClassID], [Timestamp], [PersistentProperty], [ExtraPersistentProperty], [UnidirectionalRelationPropertyID], [RelationPropertyID], [CollectionPropertyNSideID], [PrivateBaseRelationPropertyID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PersistentProperty], [ExtraPersistentProperty], [UnidirectionalRelationPropertyID], [RelationPropertyID], [CollectionPropertyNSideID], [PrivateBaseRelationPropertyID]
    FROM [dbo].[MixedDomains_Target]
    WHERE [ClassID] IN ('DerivedDerivedTargetClassForPersistentMixin')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[DerivedTargetClassForPersistentMixinView] ([ID], [ClassID], [Timestamp], [PersistentProperty], [ExtraPersistentProperty], [UnidirectionalRelationPropertyID], [RelationPropertyID], [CollectionPropertyNSideID], [PrivateBaseRelationPropertyID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PersistentProperty], [ExtraPersistentProperty], [UnidirectionalRelationPropertyID], [RelationPropertyID], [CollectionPropertyNSideID], [PrivateBaseRelationPropertyID]
    FROM [dbo].[MixedDomains_Target]
    WHERE [ClassID] IN ('DerivedTargetClassForPersistentMixin', 'DerivedDerivedTargetClassForPersistentMixin')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[DerivedTargetClassWithDerivedMixinWithInterfaceView] ([ID], [ClassID], [Timestamp], [PersistentProperty], [ExtraPersistentProperty], [UnidirectionalRelationPropertyID], [RelationPropertyID], [CollectionPropertyNSideID], [PrivateBaseRelationPropertyID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PersistentProperty], [ExtraPersistentProperty], [UnidirectionalRelationPropertyID], [RelationPropertyID], [CollectionPropertyNSideID], [PrivateBaseRelationPropertyID]
    FROM [dbo].[MixedDomains_Target]
    WHERE [ClassID] IN ('DerivedTargetClassWithDerivedMixinWithInterface')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[HookedTargetClassView] ([ID], [ClassID], [Timestamp], [Property], [TargetID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Property], [TargetID]
    FROM [dbo].[HookedTargetClass]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[InheritanceRootInheritingPersistentMixinView] ([ID], [ClassID], [Timestamp], [PersistentProperty], [PersistentRelationPropertyID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PersistentProperty], [PersistentRelationPropertyID]
    FROM [dbo].[InheritanceRootInheritingPersistentMixin]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[RelationTargetForPersistentMixinView] ([ID], [ClassID], [Timestamp], [RelationProperty2ID], [RelationProperty2IDClassID], [RelationProperty3ID], [RelationProperty3IDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [RelationProperty2ID], [RelationProperty2IDClassID], [RelationProperty3ID], [RelationProperty3IDClassID]
    FROM [dbo].[MixedDomains_RelationTarget]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[SingleInheritanceBaseClassView] ([ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], [FirstDerivedProperty], [PersistentProperty], [SecondDerivedProperty])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], [FirstDerivedProperty], [PersistentProperty], [SecondDerivedProperty]
    FROM [dbo].[SingleInheritanceBaseClass]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[SingleInheritanceFirstDerivedClassView] ([ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], [FirstDerivedProperty], [PersistentProperty])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], [FirstDerivedProperty], [PersistentProperty]
    FROM [dbo].[SingleInheritanceBaseClass]
    WHERE [ClassID] IN ('SingleInheritanceFirstDerivedClass')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[SingleInheritanceObjectWithRelationsView] ([ID], [ClassID], [Timestamp], [ScalarPropertyID], [ScalarPropertyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ScalarPropertyID], [ScalarPropertyIDClassID]
    FROM [dbo].[SingleInheritanceObjectWithRelations]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[SingleInheritanceSecondDerivedClassView] ([ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], [SecondDerivedProperty], [PersistentProperty])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [BaseProperty], [VectorOpposingPropertyID], [SecondDerivedProperty], [PersistentProperty]
    FROM [dbo].[SingleInheritanceBaseClass]
    WHERE [ClassID] IN ('SingleInheritanceSecondDerivedClass')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TargetClassForBehavioralMixinView] ([ID], [ClassID], [Timestamp], [Property])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Property]
    FROM [dbo].[TargetClassForBehavioralMixin]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TargetClassForMixinWithStateView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[TargetClassForMixinWithState]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TargetClassForPersistentMixinView] ([ID], [ClassID], [Timestamp], [PersistentProperty], [ExtraPersistentProperty], [UnidirectionalRelationPropertyID], [RelationPropertyID], [CollectionPropertyNSideID], [PrivateBaseRelationPropertyID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PersistentProperty], [ExtraPersistentProperty], [UnidirectionalRelationPropertyID], [RelationPropertyID], [CollectionPropertyNSideID], [PrivateBaseRelationPropertyID]
    FROM [dbo].[MixedDomains_Target]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TargetClassWithSameInterfaceAsPersistentMixinView] ([ID], [ClassID], [Timestamp], [PersistentPropertyRedirectedToMixin])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PersistentPropertyRedirectedToMixin]
    FROM [dbo].[MixedDomains_TargetClassWithSameInterfaceAsPersistentMixin]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TargetClassWithTwoUnidirectionalMixinsView] ([ID], [ClassID], [Timestamp], [ComputerID], [Computer2ID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ComputerID], [Computer2ID]
    FROM [dbo].[MixedDomains_TargetWithTwoUnidirectionalMixins]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TargetClassWithUnidirectionalMixin1View] ([ID], [ClassID], [Timestamp], [ComputerID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ComputerID]
    FROM [dbo].[MixedDomains_TargetWithUnidirectionalMixin1]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[TargetClassWithUnidirectionalMixin2View] ([ID], [ClassID], [Timestamp], [ComputerID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ComputerID]
    FROM [dbo].[MixedDomains_TargetWithUnidirectionalMixin2]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[CeoView] ([ID], [ClassID], [Timestamp], [Name], [CompanyID], [CompanyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [CompanyID], [CompanyIDClassID]
    FROM [dbo].[Ceo]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassNotInMappingView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[ClassNotInMapping]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithAllDataTypesView] ([ID], [ClassID], [Timestamp], [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [Flags], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [Binary], [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaFlags], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], [StringWithNullValue], [ExtensibleEnumWithNullValue], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaFlagsWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [Flags], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [Binary], [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaFlags], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], [StringWithNullValue], [ExtensibleEnumWithNullValue], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaFlagsWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary]
    FROM [dbo].[TableWithAllDataTypes]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithEnumNotDefiningZeroView] ([ID], [ClassID], [Timestamp], [EnumValue])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [EnumValue]
    FROM [dbo].[ClassWithEnumNotDefiningZero]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithGuidKeyView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[TableWithGuidKey]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithNonPublicPropertiesView] ([ID], [ClassID], [Timestamp], [PublicGetSet], [PublicGetProtectedSet], [ProtectedGetSet], [PrivateGetSet])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PublicGetSet], [PublicGetProtectedSet], [ProtectedGetSet], [PrivateGetSet]
    FROM [dbo].[ClassWithNonPublicProperties]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithOptionalOneToOneRelationAndOppositeDerivedClassView] ([ID], [ClassID], [Timestamp], [CompanyID], [CompanyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CompanyID], [CompanyIDClassID]
    FROM [dbo].[TableWithOptionalOneToOneRelationAndOppositeDerivedClass]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithoutPropertiesView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[ClassWithoutProperties]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithPropertyTypeImplementingIStructuralEquatableView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[ClassWithPropertyTypeImplementingIStructuralEquatable]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithRelatedClassIDColumnAndNoInheritanceView] ([ID], [ClassID], [Timestamp], [TableWithGuidKeyID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [TableWithGuidKeyID]
    FROM [dbo].[TableWithRelatedClassIDColumnAndNoInheritance]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithValidRelationsView] ([ID], [ClassID], [Timestamp], [TableWithGuidKeyOptionalID], [TableWithGuidKeyNonOptionalID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [TableWithGuidKeyOptionalID], [TableWithGuidKeyNonOptionalID]
    FROM [dbo].[TableWithValidRelations]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClientView] ([ID], [ClassID], [Timestamp], [ParentClientID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ParentClientID]
    FROM [dbo].[Client]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[CompanyView] ([ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [CustomerSince], [CustomerType], [ContactPersonID], [NumberOfShops], [SupplierQuality])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [CustomerSince], [CustomerType], [ContactPersonID], [NumberOfShops], [SupplierQuality]
    FROM [dbo].[Company]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ComputerView] ([ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID]
    FROM [dbo].[Computer]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[CustomerView] ([ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [CustomerSince], [CustomerType])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [CustomerSince], [CustomerType]
    FROM [dbo].[Company]
    WHERE [ClassID] IN ('Customer', 'SpecialCustomer')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[DistributorView] ([ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [NumberOfShops])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [NumberOfShops]
    FROM [dbo].[Company]
    WHERE [ClassID] IN ('Distributor')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[EmployeeView] ([ID], [ClassID], [Timestamp], [Name], [SupervisorID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [SupervisorID]
    FROM [dbo].[Employee]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[FileView] ([ID], [ClassID], [Timestamp], [ParentFolderID], [ParentFolderIDClassID], [ParentFolderRelation], [ParentFolderRelationClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ParentFolderID], [ParentFolderIDClassID], [ParentFolderRelation], [ParentFolderRelationClassID]
    FROM [dbo].[FileSystemItem]
    WHERE [ClassID] IN ('File')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[FileSystemItemView] ([ID], [ClassID], [Timestamp], [ParentFolderID], [ParentFolderIDClassID], [ParentFolderRelation], [ParentFolderRelationClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ParentFolderID], [ParentFolderIDClassID], [ParentFolderRelation], [ParentFolderRelationClassID]
    FROM [dbo].[FileSystemItem]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[FolderView] ([ID], [ClassID], [Timestamp], [ParentFolderID], [ParentFolderIDClassID], [ParentFolderRelation], [ParentFolderRelationClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ParentFolderID], [ParentFolderIDClassID], [ParentFolderRelation], [ParentFolderRelationClassID]
    FROM [dbo].[FileSystemItem]
    WHERE [ClassID] IN ('Folder')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[IndustrialSectorView] ([ID], [ClassID], [Timestamp], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name]
    FROM [dbo].[IndustrialSector]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[StorageGroupClassView] ([ID], [ClassID], [Timestamp], [AboveInheritanceIdentifier], [StorageGroupClassIdentifier])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [AboveInheritanceIdentifier], [StorageGroupClassIdentifier]
    FROM [dbo].[StorageGroupClass]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ComplexOrderView] ([ID], [ClassID], [Timestamp], [OrderNumber], [ComplexOrderName])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [OrderNumber], [ComplexOrderName]
    FROM [dbo].[ComplexOrder]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[IInterfaceWithOneImplementorView] ([ID], [ClassID], [Timestamp], [InterfaceProperty], [OnlyImplementorProperty])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [InterfaceProperty], [OnlyImplementorProperty]
    FROM [dbo].[OnlyImplementor]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[IInterfaceWithoutImplementorsView] ([ID], [ClassID], [Timestamp], [InterfaceProperty])
  AS
  SELECT CONVERT(uniqueidentifier,NULL) AS [ID], CONVERT(varchar (100),NULL) AS [ClassID], CONVERT(rowversion,NULL) AS [Timestamp], CONVERT(int,NULL) AS [InterfaceProperty]
    WHERE 1 = 0
GO
CREATE VIEW [dbo].[Implementor1View] ([ID], [ClassID], [Timestamp], [InterfaceProperty], [OnlyImplementor1Property])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [InterfaceProperty], [OnlyImplementor1Property]
    FROM [dbo].[Implementor1]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[Implementor2View] ([ID], [ClassID], [Timestamp], [InterfaceProperty], [OnlyImplementor2Property])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [InterfaceProperty], [OnlyImplementor2Property]
    FROM [dbo].[Implementor2]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[IOrderView] ([ID], [ClassID], [Timestamp], [OrderNumber], [ComplexOrderName], [SimpleOrderName])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [OrderNumber], [ComplexOrderName], NULL
    FROM [dbo].[ComplexOrder]
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [OrderNumber], NULL, [SimpleOrderName]
    FROM [dbo].[SimpleOrder]
GO
CREATE VIEW [dbo].[IOrderItemView] ([ID], [ClassID], [Timestamp], [Position], [Product], [OrderID], [OrderIDClassID], [SimpleOrderItemName])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Position], [Product], [OrderID], [OrderIDClassID], [SimpleOrderItemName]
    FROM [dbo].[SimpleOrderItem]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[OnlyImplementorView] ([ID], [ClassID], [Timestamp], [InterfaceProperty], [OnlyImplementorProperty])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [InterfaceProperty], [OnlyImplementorProperty]
    FROM [dbo].[OnlyImplementor]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[SimpleOrderView] ([ID], [ClassID], [Timestamp], [OrderNumber], [SimpleOrderName])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [OrderNumber], [SimpleOrderName]
    FROM [dbo].[SimpleOrder]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[SimpleOrderItemView] ([ID], [ClassID], [Timestamp], [Position], [Product], [OrderID], [OrderIDClassID], [SimpleOrderItemName])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Position], [Product], [OrderID], [OrderIDClassID], [SimpleOrderItemName]
    FROM [dbo].[SimpleOrderItem]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[LocationView] ([ID], [ClassID], [Timestamp], [ClientID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ClientID]
    FROM [dbo].[Location]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [OrderNo], [DeliveryDate], [OfficialID], [CustomerID], [CustomerIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [OrderNo], [DeliveryDate], [OfficialID], [CustomerID], [CustomerIDClassID]
    FROM [dbo].[Order]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[OrderItemView] ([ID], [ClassID], [Timestamp], [Position], [Product], [OrderID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Position], [Product], [OrderID]
    FROM [dbo].[OrderItem]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[OrderTicketView] ([ID], [ClassID], [Timestamp], [FileName], [OrderID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [FileName], [OrderID]
    FROM [dbo].[OrderTicket]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[PartnerView] ([ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [NumberOfShops], [SupplierQuality])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [NumberOfShops], [SupplierQuality]
    FROM [dbo].[Company]
    WHERE [ClassID] IN ('Partner', 'Distributor', 'Supplier')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[PersonView] ([ID], [ClassID], [Timestamp], [Name], [AssociatedCustomerCompanyID], [AssociatedCustomerCompanyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [AssociatedCustomerCompanyID], [AssociatedCustomerCompanyIDClassID]
    FROM [dbo].[Person]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ProductView] ([ID], [ClassID], [Timestamp], [Name], [Price])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [Price]
    FROM [dbo].[Product]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ProductReviewView] ([ID], [ClassID], [Timestamp], [ProductID], [ReviewerID], [CreatedAt], [Comment])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ProductID], [ReviewerID], [CreatedAt], [Comment]
    FROM [dbo].[ProductReview]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[BaseClassWithoutStorageSpecificIdentifierAttributeView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[DerivedClassWithStorageSpecificIdentifierAttribute]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassIDForClassHavingClassIDAttributeView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[ClassIDForClassHavingClassIDAttribute]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassHavingStorageSpecificIdentifierAttributeView] ([ID], [ClassID], [Timestamp], [NoAttribute], [CustomName])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [NoAttribute], [CustomName]
    FROM [dbo].[ClassHavingStorageSpecificIdentifierAttributeTable]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithBinaryPropertiesView] ([ID], [ClassID], [Timestamp], [NoAttribute], [NullableFromAttribute], [NotNullable], [MaximumLength], [NotNullableAndMaximumLength])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [NoAttribute], [NullableFromAttribute], [NotNullable], [MaximumLength], [NotNullableAndMaximumLength]
    FROM [dbo].[ClassWithBinaryProperties]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithBothEndPointsOnSameClassView] ([ID], [ClassID], [Timestamp], [ParentID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ParentID]
    FROM [dbo].[ClassWithBothEndPointsOnSameClass]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithDifferentPropertiesView] ([ID], [ClassID], [Timestamp], [BaseString], [BaseUnidirectionalOneToOneID], [BasePrivateUnidirectionalOneToOneID], [Int32], [String], [UnidirectionalOneToOneID], [PrivateString], [OtherString], [NewString], [DerivedPrivateString])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [BaseString], [BaseUnidirectionalOneToOneID], [BasePrivateUnidirectionalOneToOneID], [Int32], [String], [UnidirectionalOneToOneID], [PrivateString], [OtherString], [NewString], [DerivedPrivateString]
    FROM [dbo].[ClassWithDifferentProperties]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithExtensibleEnumPropertiesView] ([ID], [ClassID], [Timestamp], [NoAttribute], [NullableFromAttribute], [NotNullable])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [NoAttribute], [NullableFromAttribute], [NotNullable]
    FROM [dbo].[ClassWithExtensibleEnumProperties]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithManySideRelationPropertiesView] ([ID], [ClassID], [Timestamp], [BaseUnidirectionalID], [BaseBidirectionalOneToOneID], [BaseBidirectionalOneToManyID], [BasePrivateUnidirectionalID], [BasePrivateBidirectionalOneToOneID], [BasePrivateBidirectionalOneToManyID], [NoAttributeID], [NotNullableID], [UnidirectionalID], [BidirectionalOneToOneID], [BidirectionalOneToManyID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [BaseUnidirectionalID], [BaseBidirectionalOneToOneID], [BaseBidirectionalOneToManyID], [BasePrivateUnidirectionalID], [BasePrivateBidirectionalOneToOneID], [BasePrivateBidirectionalOneToManyID], [NoAttributeID], [NotNullableID], [UnidirectionalID], [BidirectionalOneToOneID], [BidirectionalOneToManyID]
    FROM [dbo].[ClassWithManySideRelationProperties]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClassWithOneSideRelationPropertiesView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[ClassWithOneSideRelationProperties]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClosedGenericClassView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[ClosedGenericClass]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClosedGenericClassWithManySideRelationPropertiesView] ([ID], [ClassID], [Timestamp], [BaseUnidirectionalID], [BaseBidirectionalOneToOneID], [BaseBidirectionalOneToManyID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [BaseUnidirectionalID], [BaseBidirectionalOneToOneID], [BaseBidirectionalOneToManyID]
    FROM [dbo].[ClosedGenericClassWithManySideRelationProperties]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[ClosedGenericClassWithOneSideRelationPropertiesView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[ClosedGenericClassWithOneSideRelationProperties]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[DerivedClassWithDifferentPropertiesView] ([ID], [ClassID], [Timestamp], [BaseString], [BaseUnidirectionalOneToOneID], [BasePrivateUnidirectionalOneToOneID], [Int32], [String], [UnidirectionalOneToOneID], [PrivateString], [OtherString], [NewString], [DerivedPrivateString])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [BaseString], [BaseUnidirectionalOneToOneID], [BasePrivateUnidirectionalOneToOneID], [Int32], [String], [UnidirectionalOneToOneID], [PrivateString], [OtherString], [NewString], [DerivedPrivateString]
    FROM [dbo].[ClassWithDifferentProperties]
    WHERE [ClassID] IN ('DerivedClassWithDifferentProperties')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[DerivedClassWithStorageSpecificIdentifierAttributeView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[DerivedClassWithStorageSpecificIdentifierAttribute]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[SpecialCustomerView] ([ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [CustomerSince], [CustomerType])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [CustomerSince], [CustomerType]
    FROM [dbo].[Company]
    WHERE [ClassID] IN ('SpecialCustomer')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[SupplierView] ([ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [SupplierQuality])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [SupplierQuality]
    FROM [dbo].[Company]
    WHERE [ClassID] IN ('Supplier')
  WITH CHECK OPTION
GO
-- Create indexes for tables that were created above
-- Create synonyms for tables that were created above
