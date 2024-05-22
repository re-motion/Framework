USE DBPrefix_SchemaGenerationTestDomain1
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
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CompanyView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AbstractWithoutConcreteClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AbstractWithoutConcreteClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AddressView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AddressView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CeoView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CeoView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithAllDataTypesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithAllDataTypesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithoutPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithoutPropertiesView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithRelationsBaseView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithRelationsBaseView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithRelationsView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithRelationsView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CustomerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CustomerView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AbstractClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AbstractClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedAbstractClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedAbstractClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedDerivedConcreteClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedDerivedConcreteClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ConcreteClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ConcreteClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedOfDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedOfDerivedClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PartnerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PartnerView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DevelopmentPartnerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DevelopmentPartnerView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EmployeeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[EmployeeView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FirstClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[FirstClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'MixinAddedTwiceWithDifferentNullability_LayerSupertypeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[MixinAddedTwiceWithDifferentNullability_LayerSupertypeView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'MixinAddedTwiceWithDifferentNullability_BaseClassWithDBTableView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[MixinAddedTwiceWithDifferentNullability_BaseClassWithDBTableView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'MixinAddedTwiceWithDifferentNullability_TargetClassBelowDBTableView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[MixinAddedTwiceWithDifferentNullability_TargetClassBelowDBTableView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'MixinAddedTwiceWithDifferentNullability_TargetClassWithDBTableView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[MixinAddedTwiceWithDifferentNullability_TargetClassWithDBTableView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderItemView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderItemView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SecondClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SecondClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SecondDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SecondDerivedClassView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SiblingOfClassWithRelationsView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SiblingOfClassWithRelationsView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ThirdClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ThirdClassView]
-- Drop foreign keys of all tables
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_TableWithRelations_DerivedClassID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'TableWithRelations')
  ALTER TABLE [dbo].[TableWithRelations] DROP CONSTRAINT FK_TableWithRelations_DerivedClassID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Customer_AddressID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Customer')
  ALTER TABLE [dbo].[Customer] DROP CONSTRAINT FK_Customer_AddressID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ConcreteClass_ClassWithRelationsInDerivedOfDerivedClassID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ConcreteClass')
  ALTER TABLE [dbo].[ConcreteClass] DROP CONSTRAINT FK_ConcreteClass_ClassWithRelationsInDerivedOfDerivedClassID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_ConcreteClass_ClassWithRelationsInSecondDerivedClassID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'ConcreteClass')
  ALTER TABLE [dbo].[ConcreteClass] DROP CONSTRAINT FK_ConcreteClass_ClassWithRelationsInSecondDerivedClassID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_DevelopmentPartner_AddressID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'DevelopmentPartner')
  ALTER TABLE [dbo].[DevelopmentPartner] DROP CONSTRAINT FK_DevelopmentPartner_AddressID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Employee_SupervisorID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Employee')
  ALTER TABLE [dbo].[Employee] DROP CONSTRAINT FK_Employee_SupervisorID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_FirstClass_SecondClassID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'FirstClass')
  ALTER TABLE [dbo].[FirstClass] DROP CONSTRAINT FK_FirstClass_SecondClassID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_FirstClass_ThirdClassID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'FirstClass')
  ALTER TABLE [dbo].[FirstClass] DROP CONSTRAINT FK_FirstClass_ThirdClassID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Order_CustomerID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Order')
  ALTER TABLE [dbo].[Order] DROP CONSTRAINT FK_Order_CustomerID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_OrderItem_OrderID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'OrderItem')
  ALTER TABLE [dbo].[OrderItem] DROP CONSTRAINT FK_OrderItem_OrderID
-- Drop all tables
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Address' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Address]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Ceo' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Ceo]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithAllDataTypes]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithoutProperties]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithRelations' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithRelations]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Customer]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AbstractClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[AbstractClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ConcreteClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ConcreteClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DevelopmentPartner' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[DevelopmentPartner]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Employee' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Employee]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'FirstClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[FirstClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixinAddedTwiceWithDifferentNullability_BaseClassWithDBTable' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[MixinAddedTwiceWithDifferentNullability_BaseClassWithDBTable]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixinAddedTwiceWithDifferentNullability_TargetClassWithDBTable' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[MixinAddedTwiceWithDifferentNullability_TargetClassWithDBTable]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Order]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderItem' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[OrderItem]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SecondClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SecondClass]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SiblingOfTableWithRelations' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SiblingOfTableWithRelations]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ThirdClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ThirdClass]
