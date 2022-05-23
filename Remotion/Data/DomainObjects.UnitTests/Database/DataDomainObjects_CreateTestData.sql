use DBPrefix_TestDomain

delete from [MixedDomains_TargetWithTwoUnidirectionalMixins]
delete from [MixedDomains_TargetWithUnidirectionalMixin1]
delete from [MixedDomains_TargetWithUnidirectionalMixin2]

-- update required to avoid foreign key violations
update [MixedDomains_Target] set [UnidirectionalRelationPropertyID]=NULL,[RelationPropertyID]=NULL,[PrivateBaseRelationPropertyID]=NULL,[CollectionPropertyNSideID]=NULL
update [MixedDomains_RelationTarget] set [RelationProperty2ID]=NULL,[RelationProperty2IDClassID]=NULL,[RelationProperty3ID]=NULL,[RelationProperty3IDClassID]=NULL
update [EagerFetching_BaseClass] set [ScalarProperty2RealSideID]=NULL, [UnidirectionalPropertyID]=NULL
update [EagerFetching_RelationTarget] set [CollectionPropertyOneSideID]=NULL, [CollectionPropertyOneSideIDClassID]=NULL
delete from [MixedDomains_Target]
delete from [MixedDomains_RelationTarget]
delete from [EagerFetching_BaseClass]
delete from [EagerFetching_RelationTarget]

delete from [FileSystemItem]
delete from [Location]
delete from [Client]
delete from [Computer]
delete from [Employee]
delete from [TableWithoutRelatedClassIDColumnAndDerivation]
delete from [TableWithOptionalOneToOneRelationAndOppositeDerivedClass]
delete from [TableWithoutRelatedClassIDColumn]
delete from [Ceo]
delete from [OrderTicket]
delete from [OrderItem]
delete from [Order]
delete from [Company]
delete from [IndustrialSector]
delete from [ProductReview]
delete from [Product]
delete from [Person]
delete from [TableWithAllDataTypes]
delete from [TableWithValidRelations]
delete from [TableWithInvalidRelation]
delete from [TableWithGuidKey]
delete from [TableWithKeyOfInvalidType]
delete from [TableWithoutIDColumn]
delete from [TableWithoutClassIDColumn]
delete from [TableWithoutTimestampColumn]
delete from [StorageGroupClass]
delete from [SingleInheritanceBaseClass]
delete from [ConcreteInheritanceFirstDerivedClass]
delete from [ConcreteInheritanceSecondDerivedClass]
delete from [SingleInheritanceObjectWithRelations]
delete from [ConcreteInheritanceObjectWithRelations]

-- FileSystemItem
-- An invalid file points to this folder:
insert into [FileSystemItem] (ID, ClassID, [ParentFolderID], [ParentFolderIDClassID])
    values ('{976A6864-3344-4b3c-8F67-6348CF361D22}', 'Folder', null, null)

-- This file is invalid, because ParentFolderID is null
insert into [FileSystemItem] (ID, ClassID, [ParentFolderID], [ParentFolderIDClassID])
    values ('{DCBE9554-2724-49a6-AECA-B811E20E4110}', 'File', null, 'Folder')

-- This file is invalid, because ParentFolderIDClassID is null
insert into [FileSystemItem] (ID, ClassID, [ParentFolderID], [ParentFolderIDClassID])
    values ('{A26B6A4E-D497-4b32-821B-74AFAD7EAD0A}', 'File', '{976A6864-3344-4b3c-8F67-6348CF361D22}', null)


-- Employee
insert into [Employee] (ID, ClassID, [Name], [SupervisorID])
    values ('{51ECE39B-F040-45b0-8B72-AD8B45353990}', 'Employee', 'Zaphod Beeblebrox', null)
    
insert into [Employee] (ID, ClassID, [Name], [SupervisorID])
    values ('{C3B2BBC3-E083-4974-BAC7-9CEE1FB85A5E}', 'Employee', 'Ford Prefect', null)
    
insert into [Employee] (ID, ClassID, [Name], [SupervisorID])
    values ('{3C4F3FC8-0DB2-4c1f-AA00-ADE72E9EDB32}', 'Employee', 'Arthur Dent', '{C3B2BBC3-E083-4974-BAC7-9CEE1FB85A5E}')
    
insert into [Employee] (ID, ClassID, [Name], [SupervisorID])
    values ('{890BF138-7559-40d6-9C7F-436BC1AD4F59}', 'Employee', 'Trillian', '{51ECE39B-F040-45b0-8B72-AD8B45353990}')
    
insert into [Employee] (ID, ClassID, [Name], [SupervisorID])
    values ('{43329F84-D8BB-4988-BFD2-96D4F48EE5DE}', 'Employee', 'Marvin', '{51ECE39B-F040-45b0-8B72-AD8B45353990}')
    
insert into [Employee] (ID, ClassID, [Name], [SupervisorID])
    values ('{3A24D098-EAAD-4dd7-ADA2-932D9B6935F1}', 'Employee', 'Employee 6', null)
    
insert into [Employee] (ID, ClassID, [Name], [SupervisorID])
    values ('{DBD9EA74-8C97-4411-AC02-9205D1D6D031}', 'Employee', 'Employee 7', '{3A24D098-EAAD-4dd7-ADA2-932D9B6935F1}')


-- Computer
insert into [Computer] (ID, ClassID, [SerialNumber], [EmployeeID])
    values ('{C7C26BF5-871D-48c7-822A-E9B05AAC4E5A}', 'Computer', '12345-xzy-56', '{3C4F3FC8-0DB2-4c1f-AA00-ADE72E9EDB32}');

insert into [Computer] (ID, ClassID, [SerialNumber], [EmployeeID])
    values ('{176A0FF6-296D-4934-BD1A-23CF52C22411}', 'Computer', '98678-abc-43', '{890BF138-7559-40d6-9C7F-436BC1AD4F59}');

insert into [Computer] (ID, ClassID, [SerialNumber], [EmployeeID])
    values ('{704CE38C-4A08-4ef2-A6FE-9ED849BA31E5}', 'Computer', '34554-def-87', '{43329F84-D8BB-4988-BFD2-96D4F48EE5DE}');

insert into [Computer] (ID, ClassID, [SerialNumber], [EmployeeID])
    values ('{D6F50E77-2041-46b8-A840-AAA4D2E1BF5A}', 'Computer', '63457-kol-34', null);

insert into [Computer] (ID, ClassID, [SerialNumber], [EmployeeID])
    values ('{AEAC0C5D-44E0-45cc-B716-103B0A4981A4}', 'Computer', '93756-ndf-23', null);


-- Person
insert into [Person] (ID, ClassID, [Name], [AssociatedCustomerCompanyID], [AssociatedCustomerCompanyIDClassID]) 
    values ('{2001BF42-2AA4-4c81-AD8E-73E9145411E9}', 'Person', 'Franz Huber', null, null)

insert into [Person] (ID, ClassID, [Name], [AssociatedCustomerCompanyID], [AssociatedCustomerCompanyIDClassID]) 
    values ('{DC50A962-EC95-4cf6-A4E7-A6608EAA23C8}', 'Person', 'Gisela Maier', null, null)

insert into [Person] (ID, ClassID, [Name], [AssociatedCustomerCompanyID], [AssociatedCustomerCompanyIDClassID]) 
    values ('{10F36130-E97B-4078-A535-B79E07F16AB2}', 'Person', 'Margarethe Gans', null, null)

insert into [Person] (ID, ClassID, [Name], [AssociatedCustomerCompanyID], [AssociatedCustomerCompanyIDClassID]) 
    values ('{45C6730A-DE0B-40d2-9D35-C1E56B8A89D6}', 'Person', 'Meister Eder', null, null)

insert into [Person] (ID, ClassID, [Name], [AssociatedCustomerCompanyID], [AssociatedCustomerCompanyIDClassID]) 
    values ('{70C91528-4DB4-4e6a-B3F8-70C53A728DCC}', 'Person', 'Ulrike Giftzahn', null, null)

insert into [Person] (ID, ClassID, [Name], [AssociatedCustomerCompanyID], [AssociatedCustomerCompanyIDClassID]) 
    values ('{19C04A28-094F-4d1f-9705-E2FC7107A68F}', 'Person', 'Pippi Langstrumpf', null, null)

insert into [Person] (ID, ClassID, [Name], [AssociatedCustomerCompanyID], [AssociatedCustomerCompanyIDClassID]) 
    values ('{E4F6F59F-80F7-4e41-A004-1A5BA0F68F78}', 'Person', 'Hans Dampf', null, null)

-- Note: This does not conform to mapping, because it is a contact person in two organizations.
insert into [Person] (ID, ClassID, [Name], [AssociatedCustomerCompanyID], [AssociatedCustomerCompanyIDClassID]) 
    values ('{911957D1-483C-4a8b-AA53-FF07464C58F9}', 'Person', 'Contact person in two organizations', null, null)


-- Product
insert into [Product] (ID, ClassID, [Name], [Price]) 
    values ('{BA684594-CF77-4009-A010-B70B30396A01}', 'Product', 'Pen', 1.0)

insert into [Product] (ID, ClassID, [Name], [Price]) 
    values ('{8DD65EF7-BDDA-433E-B081-725B4D53317D}', 'Product', 'Paper', 0.01)


-- ProductReview
insert into [ProductReview] (ID, ClassID, [ProductID], [ReviewerID], [CreatedAt], [Comment]) 
    values ('{877540A7-FBCF-4BF3-9007-355EA43E796F}', 'ProductReview', '{BA684594-CF77-4009-A010-B70B30396A01}', '{2001BF42-2AA4-4c81-AD8E-73E9145411E9}', '2005-03-28 10:15:19', 'Writes blue')

insert into [ProductReview] (ID, ClassID, [ProductID], [ReviewerID], [CreatedAt], [Comment]) 
    values ('{C3E4587D-626E-40D2-9D79-67CB148BE842}', 'ProductReview', '{BA684594-CF77-4009-A010-B70B30396A01}', '{DC50A962-EC95-4cf6-A4E7-A6608EAA23C8}', '2008-01-04 14:21:09', 'Uses ink')

insert into [ProductReview] (ID, ClassID, [ProductID], [ReviewerID], [CreatedAt], [Comment]) 
    values ('{793157CD-10FF-468F-994B-AEA83ADE183B}', 'ProductReview', '{BA684594-CF77-4009-A010-B70B30396A01}', '{10F36130-E97B-4078-A535-B79E07F16AB2}', '2010-09-17 11:48:23', 'You have to press down at the top to get it work')

insert into [ProductReview] (ID, ClassID, [ProductID], [ReviewerID], [CreatedAt], [Comment]) 
    values ('{B4F0052E-5F8B-4D1F-BEF9-5AA8595E78A7}', 'ProductReview', '{8DD65EF7-BDDA-433E-B081-725B4D53317D}', '{2001BF42-2AA4-4c81-AD8E-73E9145411E9}', '2006-05-21 09:24:42', 'Needs a pen preserve information. Pen is not included.')


-- IndustrialSector
insert into [IndustrialSector] (ID, ClassID, [Name]) 
    values ('{3BB7BEE9-2AF9-4a85-998E-618BEBBE5A6B}', 'IndustrialSector', 'Raumschiffproduktion')

insert into [IndustrialSector] (ID, ClassID, [Name]) 
    values ('{8565A077-EA01-4b5d-BEAA-293DC484BDDC}', 'IndustrialSector', 'Tellerwäscherei')

insert into [IndustrialSector] (ID, ClassID, [Name]) 
    values ('{53B322BF-25D8-4fe1-96C8-508E055143E7}', 'IndustrialSector', 'Putzereibetrieb')


-- Company
insert into [Company] (ID, ClassID, [Name], [IndustrialSectorID]) 
    values ('{C4954DA8-8870-45c1-B7A3-C7E5E6AD641A}', 'Company', 'Firma 1', '{8565A077-EA01-4b5d-BEAA-293DC484BDDC}')

insert into [Company] (ID, ClassID, [Name], [IndustrialSectorID]) 
    values ('{A21A9EC2-17D6-44de-9F1A-2AB6FC3742DF}', 'Company', 'Firma 2', '{8565A077-EA01-4b5d-BEAA-293DC484BDDC}')

-- The CompanyIDClassID of the CEO pointing to this company is invalid:
insert into [Company] (ID, ClassID, [Name], [IndustrialSectorID]) 
    values ('{C3DB20D6-138E-4ced-8576-E81BB4B7961F}', 'Company', 'Company with invalid CEO', null)


-- Customer
insert into [Company] (ID, ClassID, [Name], CustomerSince, CustomerType, [IndustrialSectorID]) 
    values ('{55B52E75-514B-4e82-A91B-8F0BB59B80AD}', 'Customer', 'Kunde 1', '2000/01/01', 0, '{3BB7BEE9-2AF9-4a85-998E-618BEBBE5A6B}')

insert into [Company] (ID, ClassID, [Name], CustomerSince, CustomerType, [IndustrialSectorID]) 
    values ('{F577F879-2DB4-4a3c-A18A-AFB4E57CE098}', 'Customer', 'Kunde 2', '2000/02/01', 1, '{8565A077-EA01-4b5d-BEAA-293DC484BDDC}')

insert into [Company] (ID, ClassID, [Name], CustomerSince, CustomerType, [IndustrialSectorID]) 
    values ('{DD3E3D55-C16F-497f-A3E1-384D08DE0D66}', 'Customer', 'Kunde 3', '2000/03/01', 2, '{8565A077-EA01-4b5d-BEAA-293DC484BDDC}')

insert into [Company] (ID, ClassID, [Name], CustomerSince, CustomerType, [IndustrialSectorID]) 
    values ('{B3F0A333-EC2A-4ddd-9035-9ADA34052450}', 'Customer', 'Kunde 4', '1999/03/01', 2, null)

-- The Order pointing to this customer is invalid
insert into [Company] (ID, ClassID, [Name], CustomerSince, CustomerType, [IndustrialSectorID]) 
    values ('{DA658F26-8107-44ce-9DD0-1804503ECCAF}', 'Customer', 'Customer with invalid Order', '1999/03/01', 2, null)


-- Partner
insert into [Company] (ID, ClassID, [Name], ContactPersonID, [IndustrialSectorID]) 
    values ('{5587A9C0-BE53-477d-8C0A-4803C7FAE1A9}', 'Partner', 'Partner 1', '{2001BF42-2AA4-4c81-AD8E-73E9145411E9}', '{3BB7BEE9-2AF9-4a85-998E-618BEBBE5A6B}')

insert into [Company] (ID, ClassID, [Name], ContactPersonID, [IndustrialSectorID]) 
    values ('{B403E58E-9FA5-47ed-883C-73420D64DEB3}', 'Partner', 'Partner 2', '{DC50A962-EC95-4cf6-A4E7-A6608EAA23C8}', '{8565A077-EA01-4b5d-BEAA-293DC484BDDC}')

-- This row does not conform to mapping, because no CEO row points to this:
insert into [Company] (ID, ClassID, [Name], ContactPersonID, [IndustrialSectorID]) 
    values ('{A65B123A-6E17-498e-A28E-946217C0AE30}', 'Partner', 'Partner 3', '{E4F6F59F-80F7-4e41-A004-1A5BA0F68F78}', '{3BB7BEE9-2AF9-4a85-998E-618BEBBE5A6B}')

-- This row does not conform to mapping, because this and the row below refer to the same person:
insert into [Company] (ID, ClassID, [Name], ContactPersonID, [IndustrialSectorID]) 
    values ('{A3B45592-8756-48b9-B68F-6BDEA1393CA6}', 'Partner', 'Invalid Company 1', '{911957D1-483C-4a8b-AA53-FF07464C58F9}', null)

-- This row does not conform to mapping, because this and the row above refer to the same person:
insert into [Company] (ID, ClassID, [Name], ContactPersonID, [IndustrialSectorID]) 
    values ('{38E7DB32-C14F-4b61-863D-6F5188562D61}', 'Partner', 'Invalid Company 2', '{911957D1-483C-4a8b-AA53-FF07464C58F9}', null)


-- Supplier
insert into [Company] (ID, ClassID, [Name], ContactPersonID, SupplierQuality, [IndustrialSectorID]) 
    values ('{FD392135-1FDD-42a3-8E2F-232BAB9893A2}', 'Supplier', 'Lieferant 1', '{10F36130-E97B-4078-A535-B79E07F16AB2}', 1, '{3BB7BEE9-2AF9-4a85-998E-618BEBBE5A6B}')

insert into [Company] (ID, ClassID, [Name], ContactPersonID, SupplierQuality, [IndustrialSectorID]) 
    values ('{92A8BB6A-412A-4fe3-9B09-3E1B6136E425}', 'Supplier', 'Lieferant 2', '{45C6730A-DE0B-40d2-9D35-C1E56B8A89D6}', 50, '{8565A077-EA01-4b5d-BEAA-293DC484BDDC}')


-- Distributor
insert into [Company] (ID, ClassID, [Name], ContactPersonID, NumberOfShops, [IndustrialSectorID]) 
    values ('{E4087155-D60A-4d31-95B3-9A401A3E4E78}', 'Distributor', 'Händler 1', '{70C91528-4DB4-4e6a-B3F8-70C53A728DCC}', 1, '{8565A077-EA01-4b5d-BEAA-293DC484BDDC}')

insert into [Company] (ID, ClassID, [Name], ContactPersonID, NumberOfShops, [IndustrialSectorID]) 
    values ('{247206C3-7B48-4e17-91DD-3363B568D7E4}', 'Distributor', 'Händler 2', '{19C04A28-094F-4d1f-9705-E2FC7107A68F}', 10, '{3BB7BEE9-2AF9-4a85-998E-618BEBBE5A6B}')

-- This row does not conform to mapping, because it lacks a pointer to a contact person:
insert into [Company] (ID, ClassID, [Name], ContactPersonID, NumberOfShops, [IndustrialSectorID]) 
    values ('{1514D668-A0A5-40e9-AC22-F24900E0EB39}', 'Distributor', 'Händler 3', null, 5, '{53B322BF-25D8-4fe1-96C8-508E055143E7}')

-- Order
insert into [Order] (ID, ClassID, OrderNo, DeliveryDate, CustomerID, CustomerIDClassID, OfficialID) 
    values ('{5682F032-2F0B-494b-A31C-C97F02B89C36}', 'Order', 1, '2005/01/01', 
    '{55B52E75-514B-4e82-A91B-8F0BB59B80AD}', 'Customer', 'Official|1|System.Int32')
    
insert into [Order] (ID, ClassID, OrderNo, DeliveryDate, CustomerID, CustomerIDClassID, OfficialID) 
    values ('{83445473-844A-4d3f-A8C3-C27F8D98E8BA}', 'Order', 3, '2005/03/01', 
    '{DD3E3D55-C16F-497f-A3E1-384D08DE0D66}', 'Customer', 'Official|1|System.Int32')

insert into [Order] (ID, ClassID, OrderNo, DeliveryDate, CustomerID, CustomerIDClassID, OfficialID) 
    values ('{3C0FB6ED-DE1C-4e70-8D80-218E0BF58DF3}', 'Order', 4, '2006/02/01', 
    '{B3F0A333-EC2A-4ddd-9035-9ADA34052450}', 'Customer', 'Official|1|System.Int32')

insert into [Order] (ID, ClassID, OrderNo, DeliveryDate, CustomerID, CustomerIDClassID, OfficialID) 
    values ('{90E26C86-611F-4735-8D1B-E1D0918515C2}', 'Order', 5, '2006/03/01', 
    '{B3F0A333-EC2A-4ddd-9035-9ADA34052450}', 'Customer', 'Official|1|System.Int32')

insert into [Order] (ID, ClassID, OrderNo, DeliveryDate, CustomerID, CustomerIDClassID, OfficialID) 
    values ('{F4016F41-F4E4-429e-B8D1-659C8C480A67}', 'Order', 2, '2005/02/01', 
    '{55B52E75-514B-4e82-A91B-8F0BB59B80AD}', 'Customer', 'Official|1|System.Int32')

-- No OrderItem points to this Order => This leads to a PersistenceException when navigating over relation
insert into [Order] (ID, ClassID, OrderNo, DeliveryDate, CustomerID, CustomerIDClassID, OfficialID) 
    values ('{F7607CBC-AB34-465C-B282-0531D51F3B04}', 'Order', 99, '2013/03/07', 
    '{DA658F26-8107-44CE-9DD0-1804503ECCAF}', 'Customer', 'Official|2|System.Int32')

-- This order does not conform to mapping: CustomerIDClassID is invalid, no OrderTicket points to this Order and
-- this Order has no OrderItems and the Official does not exist
insert into [Order] (ID, ClassID, OrderNo, DeliveryDate, CustomerID, CustomerIDClassID, OfficialID) 
    values ('{DA658F26-8107-44ce-9DD0-1804503ECCAF}', 'Order', 6, '2006/03/01', 
    '{DA658F26-8107-44ce-9DD0-1804503ECCAF}', 'Company', 'Official|-1|System.Int32')


-- OrderItem
insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) 
    values ('{2F4D42C7-7FFA-490d-BFCD-A9101BBF4E1A}', 'OrderItem', 
    '{5682F032-2F0B-494b-A31C-C97F02B89C36}', 1, 'Mainboard')

insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) 
    values ('{AD620A11-4BC4-4791-BCF4-A0770A08C5B0}', 'OrderItem', 
    '{5682F032-2F0B-494b-A31C-C97F02B89C36}', 2, 'CPU Fan')

insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) 
    values ('{0D7196A5-8161-4048-820D-B1BBDABE3293}', 'OrderItem', 
    '{83445473-844A-4d3f-A8C3-C27F8D98E8BA}', 1, 'Harddisk')

insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) 
    values ('{DC20E0EB-4B55-4f23-89CF-6D6478F96D3B}', 'OrderItem', 
    '{3C0FB6ED-DE1C-4e70-8D80-218E0BF58DF3}', 1, 'Hitchhiker''s guide')

insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) 
    values ('{EA505094-770A-4505-82C1-5A4F94F56FE2}', 'OrderItem', 
    '{90E26C86-611F-4735-8D1B-E1D0918515C2}', 1, 'Blumentopf')

insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) 
    values ('{5A33809B-06F5-4F62-B103-C8E1869D36EF}', 'OrderItem', 
    '{F4016F41-F4E4-429e-B8D1-659C8C480A67}', 1, 'Solar Panel')

-- This OrderItem does not have an Order (which does not conform to the mapping)
insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) 
    values ('{386D99F9-B0BA-4C55-8F22-BF194A3D745A}', 'OrderItem', 
    NULL, 1, 'Rubik''s Cube')

-- OrderTicket
insert into [OrderTicket] (ID, ClassID, FileName, OrderID) 
    values ('{058EF259-F9CD-4cb1-85E5-5C05119AB596}', 'OrderTicket', 'C:\order1.png', '{5682F032-2F0B-494b-A31C-C97F02B89C36}')

insert into [OrderTicket] (ID, ClassID, FileName, OrderID) 
    values ('{0005BDF4-4CCC-4a41-B9B5-BAAB3EB95237}', 'OrderTicket', 'C:\order2.png', '{F4016F41-F4E4-429e-B8D1-659C8C480A67}')

insert into [OrderTicket] (ID, ClassID, FileName, OrderID) 
    values ('{BCF6C5F6-323F-4471-9CA5-7DF0A48C7A59}', 'OrderTicket', 'C:\order3.png', '{83445473-844A-4d3f-A8C3-C27F8D98E8BA}')

insert into [OrderTicket] (ID, ClassID, FileName, OrderID) 
    values ('{6768DB2B-9C66-4e2f-BBA2-89C56718FF2B}', 'OrderTicket', 'C:\order4.png', '{3C0FB6ED-DE1C-4e70-8D80-218E0BF58DF3}')

insert into [OrderTicket] (ID, ClassID, FileName, OrderID) 
    values ('{DC20E0EB-4B55-4f23-89CF-6D6478F96D3B}', 'OrderTicket', 'C:\order5.png', '{90E26C86-611F-4735-8D1B-E1D0918515C2}')

insert into [OrderTicket] (ID, ClassID, FileName, OrderID) 
    values ('{87E9C075-B208-4475-923D-7BF3B50AB18E}', 'OrderTicket', 'C:\order99.png', '{F7607CBC-AB34-465C-B282-0531D51F3B04}')


-- Ceo
insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{A1691AF1-F96D-42e1-B021-B5099840D572}', 'Ceo', 'Hermann Boss', 
    '{C4954DA8-8870-45c1-B7A3-C7E5E6AD641A}', 'Company')

insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{A6A848CE-505F-4cd3-A337-1F5EEA1D2260}', 'Ceo', 'Sepp Boss', 
    '{A21A9EC2-17D6-44de-9F1A-2AB6FC3742DF}', 'Company')

insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{481C7840-9D8A-4872-BBCD-B41A9BD85528}', 'Ceo', 'Hermann Chef', 
    '{55B52E75-514B-4e82-A91B-8F0BB59B80AD}', 'Customer')

insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{BE7F24E2-600C-4cd8-A7C3-8669AFD54154}', 'Ceo', 'Sepp Fischer', 
    '{F577F879-2DB4-4a3c-A18A-AFB4E57CE098}', 'Customer')

insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{7236BA88-48C6-415f-A0BA-A328A1A22DFE}', 'Ceo', 'Hugo Boss', 
    '{DD3E3D55-C16F-497f-A3E1-384D08DE0D66}', 'Customer')

insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{C7837D11-C1D6-458f-A3F7-7D5C96C1F726}', 'Ceo', 'Thomas Friedrich', 
    '{5587A9C0-BE53-477d-8C0A-4803C7FAE1A9}', 'Partner')

insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{9F0AC953-E78E-4939-8AFE-0EFF9B3B3ED9}', 'Ceo', 'Sabine Karl', 
    '{B403E58E-9FA5-47ed-883C-73420D64DEB3}', 'Partner')

insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{394C69B2-BD40-48d1-A2AE-A73FB63C0B66}', 'Ceo', 'Zenzi Tischler', 
    '{FD392135-1FDD-42a3-8E2F-232BAB9893A2}', 'Supplier')

insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{421D04B4-BC77-4682-B0FE-58B96802C524}', 'Ceo', 'Brunhilde von Island', 
    '{92A8BB6A-412A-4fe3-9B09-3E1B6136E425}', 'Supplier')

insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{6B801331-2163-4837-B20C-973BD9B8768E}', 'Ceo', 'Siegfried Drachentöter', 
    '{E4087155-D60A-4d31-95B3-9A401A3E4E78}', 'Distributor')

insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{2E8AE776-DC3A-45a5-9B0C-35900CC78FDC}', 'Ceo', 'Gitti Linzer', 
    '{247206C3-7B48-4e17-91DD-3363B568D7E4}', 'Distributor')

insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{FD1B587C-3E26-43f8-9866-8B770194D70F}', 'Ceo', 'Harry Fleischer', 
    '{B3F0A333-EC2A-4ddd-9035-9ADA34052450}', 'Customer')

-- The CompanyIDClassID of this CEO is invalid:
insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{C3DB20D6-138E-4ced-8576-E81BB4B7961F}', 'Ceo', 'Ceo with invalid CompanyIDClassID', 
    '{C3DB20D6-138E-4ced-8576-E81BB4B7961F}', 'Customer')

-- This CEO does not conform to mapping, because CompanyID and CompanyIDClassID are null.
insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{2927059E-AE59-49a7-8B59-B959E579C629}', 'Ceo', 'Ceo with NULL CompanyID and ClassID', null, null)

-- This CEO is invalid, because CompanyID is null.
insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{523B490A-5B18-4f22-AF5B-BD9A4DA3F629}', 'Ceo', 'Ceo with NULL CompanyID', null, 'Distributor')

-- This CEO is invalid, because CompanyIDClassID is null.
insert into [Ceo] (ID, ClassID, [Name], CompanyID, CompanyIDClassID) 
    values ('{04341C7D-7B7C-49fc-82E6-8E481CDACA30}', 'Ceo', 'Ceo with NULL CompanyIDClassID', '{1514D668-A0A5-40e9-AC22-F24900E0EB39}', null)


-- Client
insert into [Client] (ID, ClassID, [ParentClientID]) values ('{1627ADE8-125F-4819-8E33-CE567C42B00C}', 'Client', null)
insert into [Client] (ID, ClassID, [ParentClientID]) values ('{090D54F2-738C-48ac-9C78-F40365A72305}', 'Client', '{1627ADE8-125F-4819-8E33-CE567C42B00C}')
insert into [Client] (ID, ClassID, [ParentClientID]) values ('{01349595-88A3-4583-A7BA-CB08795C97F6}', 'Client', '{1627ADE8-125F-4819-8E33-CE567C42B00C}')
insert into [Client] (ID, ClassID, [ParentClientID]) values ('{015E25B1-ACFA-4364-87F5-D28A45384D11}', 'Client', null)


-- Location
insert into [Location] (ID, ClassID, [ClientID]) values ('{D527B630-B0AC-4572-A614-EAC9F486148D}', 'Location', '{1627ADE8-125F-4819-8E33-CE567C42B00C}')
insert into [Location] (ID, ClassID, [ClientID]) values ('{20380C9D-B70F-4d9a-880E-EAE5D6E3919C}', 'Location', '{1627ADE8-125F-4819-8E33-CE567C42B00C}')
insert into [Location] (ID, ClassID, [ClientID]) values ('{903E7EE5-CBB8-44c0-BEB6-ACAFFA5ADA7F}', 'Location', '{090D54F2-738C-48ac-9C78-F40365A72305}')


-- ClassWithoutRelatedClassIDColumn
insert into [TableWithoutRelatedClassIDColumn] (ID, ClassID, DistributorID) 
    values ('{CD3BE83E-FBB7-4251-AAE4-B216485C5638}', 'ClassWithoutRelatedClassIDColumn', 
    '{E4087155-D60A-4d31-95B3-9A401A3E4E78}')

-- TableWithoutRelatedClassIDColumnAndDerivation
insert into [TableWithoutRelatedClassIDColumnAndDerivation] (ID, ClassID, CompanyID) 
    values ('{4821D7F7-B586-4435-B572-8A96A44B113E}', 'ClassWithoutRelatedClassIDColumnAndDerivation', 
    '{C4954DA8-8870-45c1-B7A3-C7E5E6AD641A}')

-- TableWithOptionalOneToOneRelationAndOppositeDerivedClass
-- Note: This row does not conform to mapping, because column 'CompanyIDClassID' must be null.
insert into [TableWithOptionalOneToOneRelationAndOppositeDerivedClass] (ID, ClassID, CompanyID, CompanyIDClassID) 
    values ('{5115A733-5CD1-46C5-81EE-0B50EF0A5858}', 'ClassWithOptionalOneToOneRelationAndOppositeDerivedClass', 
    null, 'Company')

-- TableWithAllDataTypes

-- Note: Actual test values for column 'Binary' are updated by TestDataLoader. Therefore '' is used here.
insert into [TableWithAllDataTypes] (ID, ClassID, [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [Flags], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [Binary],
    [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaFlags], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], 
    [StringWithNullValue], [ExtensibleEnumWithNullValue], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaFlagsWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary]) 
    values 
    ('{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}', 'ClassWithAllDataTypes', 
    0, 85, '2005/01/01', '2005/01/01 17:00', 123456.789, 987654.321, 1, 2, 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ColorExtensions.Red', '{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}', 32767, 2147483647, 9223372036854775807, 6789.321, 'abcdeföäü', '12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890', 0x0,
    1, 78, '2005/02/01', '2005/02/01 05:00', 765.098, 654321.789, 2, 3, '{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}', 12000, -2147483647, 3147483647, 12.456,
    null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)
    
insert into [TableWithAllDataTypes] (ID, ClassID, [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [Flags], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [Binary],
    [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaFlags], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], 
    [StringWithNullValue], [ExtensibleEnumWithNullValue], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaFlagsWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary]) 
    values 
    ('{583EC716-8443-4b55-92BF-09F7C8768529}', 'ClassWithAllDataTypes', 
    1, 86, '2005/01/02', '2005/01/02 01:00', 654321.987, 456789.123, 0, 3, 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ColorExtensions.Blue', '{D2146236-FBD4-4b93-A835-26563FE3F043}', -32767, -2147483647, -9223372036854775807, -6789.321, 'üäöfedcba', '09876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321', 0x0,
    1, 79, '2005/02/02', '2005/02/02 15:00', 123.111, -654321.789, 2, 1, '{8C21745D-1269-4cb2-BC81-E1B112A0C146}', 390, 2147483647, 2147483648, 321.321,
    null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)


-- TableWithGuidKey
insert into [TableWithGuidKey] (ID, ClassID) 
    values ('{7D1F5F2E-D111-433b-A675-300B55DC4756}', 'ClassWithGuidKey')

insert into [TableWithGuidKey] (ID, ClassID) 
    values ('{D0A1BDDE-B13F-47c1-98BD-EBAE21189B01}', 'ClassWithGuidKey')

-- This row contains an invalid ClassID => Thus an error occurs when loaded:
insert into [TableWithGuidKey] (ID, ClassID) 
    values ('{C9F16F93-CF42-4357-B87B-7493882AAEAF}', 'NonExistingClassID')
    
-- This row contains a ClassID from another class => Thus an error occurs when loaded:
insert into [TableWithGuidKey] (ID, ClassID) 
    values ('{895853EB-06CD-4291-B467-160560AE8EC1}', 'Order')

-- No other row should refer to this row:
insert into [TableWithGuidKey] (ID, ClassID) 
    values ('{672C8754-C617-4b7a-890C-BFEF8AC86564}', 'ClassWithGuidKey')

-- TableWithKeyOfInvalidType
insert into [TableWithKeyOfInvalidType] (ID, ClassID) 
    values ('2005/01/01', 'ClassWithKeyOfInvalidType')

-- TableWithoutIDColumn
insert into [TableWithoutIDColumn] (ClassID) 
    values ('ClassWithoutIDColumn')
    
-- TableWithoutClassIDColumn
insert into [TableWithoutClassIDColumn] (ID) 
    values ('{DDD02092-355B-4820-90B6-7F1540C0547E}')

-- TableWithoutTimestampColumn
insert into [TableWithoutTimestampColumn] (ID, ClassID) 
    values ('{027DCBD7-ED68-461d-AE80-B8E145A7B816}', 'ClassWithoutTimestampColumn')
 
-- TableWithValidRelations
insert into [TableWithValidRelations] (ID, ClassID, TableWithGuidKeyOptionalID, TableWithGuidKeyNonOptionalID)  
    values ('{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}', 'ClassWithValidRelations', 
    null, '{D0A1BDDE-B13F-47c1-98BD-EBAE21189B01}')

-- This row does not conform to mapping, because TableWithGuidKeyNonOptionalID is mandatory, but null:
insert into [TableWithValidRelations] (ID, ClassID, TableWithGuidKeyOptionalID, TableWithGuidKeyNonOptionalID)  
    values ('{3E5AED0E-C6F9-4dca-A901-4DA50F5A97AB}', 'ClassWithValidRelations', 
    '{D0A1BDDE-B13F-47c1-98BD-EBAE21189B01}', null)

-- TableWithInvalidRelation
-- This row points to non-existing row in TableWithGuidKey
insert into [TableWithInvalidRelation] (ID, ClassID, TableWithGuidKeyID) 
    values ('{AFA9CF46-8E77-4da8-9793-53CAA86A277C}', 'ClassWithInvalidRelation', 
    '{A53F679D-0E91-4504-AEE8-59250DE249B3}')


-- StorageGroupClass
insert into [StorageGroupClass] (ID, ClassID, AboveInheritanceIdentifier, StorageGroupClassIdentifier)
    values ('{09755471-E551-496d-941B-84D90D0C9ECA}', 'StorageGroupClass', 'AboveInheritanceName1', 'StorageGroupName1')
    
insert into [StorageGroupClass] (ID, ClassID, AboveInheritanceIdentifier, StorageGroupClassIdentifier)
    values ('{F394AE2E-CB4E-4e38-8E08-9C847EE1F376}', 'StorageGroupClass', 'AboveInheritanceName2', 'StorageGroupName2')

--MixedDomains_Target
insert into [MixedDomains_Target] (ID, ClassID, PersistentProperty, ExtraPersistentProperty)
    values ('{784EBDDD-EE94-456D-A5F4-F6CB1B41B6CA}', 'TargetClassForPersistentMixin', 99, 100)
insert into [MixedDomains_Target] (ID, ClassID, PersistentProperty, ExtraPersistentProperty)
    values ('{4ED563B8-B337-4C8E-9A77-5FA907919377}', 'DerivedTargetClassForPersistentMixin', 199, 100)
insert into [MixedDomains_Target] (ID, ClassID, PersistentProperty, ExtraPersistentProperty)
    values ('{B551C440-8C80-4930-A2A1-7FBB4F6B69D8}', 'DerivedDerivedTargetClassForPersistentMixin', 299, 100)
insert into [MixedDomains_Target] (ID, ClassID, PersistentProperty, ExtraPersistentProperty)
    values ('{5FFD52D9-2A38-4DEC-9AA1-FA76C30B91A4}', 'DerivedTargetClassWithDerivedMixinWithInterface', 199, 100)


insert into [MixedDomains_Target] ([ID], [ClassID], [PersistentProperty], [ExtraPersistentProperty]) 
    values ('{FF79502F-FF40-45E0-929A-230006EA3E83}', 'TargetClassForPersistentMixin', 13, 1333)

insert into [MixedDomains_RelationTarget] ([ID], [ClassID]) 
    values ('{DC42158C-7DA6-4D5C-B522-C0879E404DEC}', 'RelationTargetForPersistentMixin')
insert into [MixedDomains_RelationTarget] ([ID], [ClassID]) 
    values ('{332F9971-E6FD-411C-862A-23416E0019BC}', 'RelationTargetForPersistentMixin')
insert into [MixedDomains_RelationTarget] ([ID], [ClassID]) 
    values ('{58458546-9C4A-4E36-9D62-C6CF171748A6}', 'RelationTargetForPersistentMixin')
insert into [MixedDomains_RelationTarget] ([ID], [ClassID]) 
    values ('{A5AC369E-9742-412C-8275-4B31B48CEFF3}', 'RelationTargetForPersistentMixin')
insert into [MixedDomains_RelationTarget] ([ID], [ClassID]) 
    values ('{C007F590-7953-4429-A34E-778309F2FC1D}', 'RelationTargetForPersistentMixin')

update [MixedDomains_Target] 
    set [UnidirectionalRelationPropertyID] = 'C007F590-7953-4429-A34E-778309F2FC1D', 
        [RelationPropertyID] = 'DC42158C-7DA6-4D5C-B522-C0879E404DEC', 
        [CollectionPropertyNSideID] = 'A5AC369E-9742-412C-8275-4B31B48CEFF3', 
        [PrivateBaseRelationPropertyID] = null 
    where [ID] = 'FF79502F-FF40-45E0-929A-230006EA3E83'

update [MixedDomains_RelationTarget] 
    set [RelationProperty2ID] = NULL, 
        [RelationProperty2IDClassID] = NULL, 
        [RelationProperty3ID] = NULL, 
        [RelationProperty3IDClassID] = NULL 
    where [ID] = 'DC42158C-7DA6-4D5C-B522-C0879E404DEC'

update [MixedDomains_RelationTarget] 
    set [RelationProperty2ID] = 'FF79502F-FF40-45E0-929A-230006EA3E83', 
        [RelationProperty2IDClassID] = 'TargetClassForPersistentMixin', 
        [RelationProperty3ID] = NULL, 
        [RelationProperty3IDClassID] = NULL 
    where [ID] = '332F9971-E6FD-411C-862A-23416E0019BC'

update [MixedDomains_RelationTarget] 
    set [RelationProperty2ID] = NULL, 
        [RelationProperty2IDClassID] = NULL, 
        [RelationProperty3ID] = 'FF79502F-FF40-45E0-929A-230006EA3E83', 
        [RelationProperty3IDClassID] = 'TargetClassForPersistentMixin' 
    where [ID] = '58458546-9C4A-4E36-9D62-C6CF171748A6'

update [MixedDomains_RelationTarget] 
    set [RelationProperty2ID] = NULL, 
        [RelationProperty2IDClassID] = NULL, 
        [RelationProperty3ID] = NULL, 
        [RelationProperty3IDClassID] = NULL 
    where [ID] = 'A5AC369E-9742-412C-8275-4B31B48CEFF3'

update [MixedDomains_RelationTarget] 
    set [RelationProperty2ID] = NULL, 
        [RelationProperty2IDClassID] = NULL, 
        [RelationProperty3ID] = NULL, 
        [RelationProperty3IDClassID] = NULL 
    where [ID] = 'C007F590-7953-4429-A34E-778309F2FC1D'

insert into [EagerFetching_BaseClass] ([ID], [ClassID]) 
    values ('{DA3B6AA5-9DEF-4048-9479-6ADB940EB88C}', 'EagerFetching_BaseClass')

insert into [EagerFetching_BaseClass] ([ID], [ClassID]) 
    values ('{BD4403A5-E96D-426E-A7B0-4A5DB2363D3A}', 'EagerFetching_DerivedClass1')

insert into [EagerFetching_BaseClass] ([ID], [ClassID]) 
    values ('{80B729A2-1FC6-46D0-A69B-39B881346588}', 'EagerFetching_DerivedClass1')

insert into [EagerFetching_BaseClass] ([ID], [ClassID], [ScalarProperty2RealSideID], [UnidirectionalPropertyID]) 
    values ('{10742FC3-AB58-49BC-AF04-CBDE1B160CBF}', 'EagerFetching_DerivedClass2', NULL, NULL)

insert into [EagerFetching_BaseClass] ([ID], [ClassID], [ScalarProperty2RealSideID], [UnidirectionalPropertyID]) 
    values ('{F6090162-7B12-4689-951B-B52EFD72C34F}', 'EagerFetching_DerivedClass2', NULL, NULL)

insert into [EagerFetching_RelationTarget] ([ID], [ClassID], [CollectionPropertyOneSideID], [CollectionPropertyOneSideIDClassID], [ScalarProperty1RealSideID], [ScalarProperty1RealSideIDClassID]) 
    values ('{3DD744D2-91C7-463B-A5AF-01AD064B3821}', 'EagerFetching_RelationTarget', '{BD4403A5-E96D-426E-A7B0-4A5DB2363D3A}', 'EagerFetching_DerivedClass1', NULL, NULL)

insert into [EagerFetching_RelationTarget] ([ID], [ClassID], [CollectionPropertyOneSideID], [CollectionPropertyOneSideIDClassID], [ScalarProperty1RealSideID], [ScalarProperty1RealSideIDClassID]) 
    values ('{FF5709EB-2523-44B6-B0EA-92423AE705F5}', 'EagerFetching_RelationTarget', '{BD4403A5-E96D-426E-A7B0-4A5DB2363D3A}', 'EagerFetching_DerivedClass1', NULL, NULL)

insert into [EagerFetching_RelationTarget] ([ID], [ClassID], [CollectionPropertyOneSideID], [CollectionPropertyOneSideIDClassID], [ScalarProperty1RealSideID], [ScalarProperty1RealSideIDClassID]) 
    values ('{36381FD9-D454-4FCD-B860-3E30166BE092}', 'EagerFetching_RelationTarget', NULL, NULL, '{80B729A2-1FC6-46D0-A69B-39B881346588}', 'EagerFetching_DerivedClass1')

insert into [EagerFetching_RelationTarget] ([ID], [ClassID], [CollectionPropertyOneSideID], [CollectionPropertyOneSideIDClassID], [ScalarProperty1RealSideID], [ScalarProperty1RealSideIDClassID]) 
    values ('{5DD64E95-5449-4345-8DE8-1D766BB23B17}', 'EagerFetching_RelationTarget', NULL, NULL, NULL, NULL)

insert into [EagerFetching_RelationTarget] ([ID], [ClassID], [CollectionPropertyOneSideID], [CollectionPropertyOneSideIDClassID], [ScalarProperty1RealSideID], [ScalarProperty1RealSideIDClassID]) 
    values ('{03CDC3BF-DA09-442D-A2C0-96567B48BC5D}', 'EagerFetching_RelationTarget', NULL, NULL, NULL, NULL)

update [EagerFetching_BaseClass] 
    set [ScalarProperty2RealSideID] = '{5DD64E95-5449-4345-8DE8-1D766BB23B17}'
    where [ID] = '10742FC3-AB58-49BC-AF04-CBDE1B160CBF'

update [EagerFetching_BaseClass] 
    set [UnidirectionalPropertyID] = '{03CDC3BF-DA09-442D-A2C0-96567B48BC5D}'
    where [ID] = 'F6090162-7B12-4689-951B-B52EFD72C34F'

--
insert into [OnlyImplementor] ([ID], [ClassID], [InterfaceProperty], [OnlyImplementorProperty]) 
    values ('{D20AD209-61CC-4E01-B570-EE4554038DB0}', 'OnlyImplementor', 3, 1337);

insert into [OnlyImplementor] ([ID], [ClassID], [InterfaceProperty], [OnlyImplementorProperty]) 
    values ('{CDA944EF-4ABC-48DA-AFCB-CEFD1F826597}', 'OnlyImplementor', 5, 1338);

insert into [OrderGroup] ([ID], [ClassID], [GroupName]) 
    values ('{38A0D154-662B-4F94-9F89-E59E5BAE30E4}', 'OrderGroup', 'Order Group 1');

insert into [SimpleOrder] ([ID], [ClassID], [OrderNumber], [GroupID], [GroupIDClassID], [SimpleOrderName]) 
    values ('{DFFF4E9C-1EE1-4D36-977C-037909173CE0}', 'SimpleOrder', 1, NULL, NULL, 'Coffee');

insert into [SimpleOrder] ([ID], [ClassID], [OrderNumber], [GroupID], [GroupIDClassID], [SimpleOrderName]) 
    values ('{6B71C813-86A6-44D0-B0E7-846C37F41D47}', 'SimpleOrder', 2, '{38A0D154-662B-4F94-9F89-E59E5BAE30E4}', 'OrderGroup', 'Coffee and Milk');

insert into [SimpleOrderItem] ([ID], [ClassID], [Position], [Product],
        [OrderID], [OrderIDClassID], [SimpleOrderItemName]) 
    values ('{1D39C517-0B47-4232-8683-87109B277AFC}', 'SimpleOrderItem', 1, 'Coffee',
        '{DFFF4E9C-1EE1-4D36-977C-037909173CE0}', 'SimpleOrder', 'black arabian coffee');

insert into [SimpleOrderItem] ([ID], [ClassID], [Position], [Product],
        [OrderID], [OrderIDClassID], [SimpleOrderItemName]) 
    values ('{FD01262F-FAD2-48BC-B91E-65B7A74F3AA3}', 'SimpleOrderItem', 1, 'Coffee (Milk)',
        '{6B71C813-86A6-44D0-B0E7-846C37F41D47}', 'SimpleOrder', 'black arabian coffee with milk');

insert into [SimpleOrderItem] ([ID], [ClassID], [Position], [Product],
        [OrderID], [OrderIDClassID], [SimpleOrderItemName]) 
    values ('{003A1474-66E7-4998-A1EF-F29690B41955}', 'SimpleOrderItem', 2, 'Glass of water',
        '{6B71C813-86A6-44D0-B0E7-846C37F41D47}', 'SimpleOrder', 'tap water');

insert into [ComplexOrder] ([ID], [ClassID], [OrderNumber], [GroupID], [GroupIDClassID], [ComplexOrderName]) 
    values ('{E4B34F09-466B-437C-922F-045588CA4CA3}', 'ComplexOrder', 1, '{38A0D154-662B-4F94-9F89-E59E5BAE30E4}', 'OrderGroup', 'Special coffe');
