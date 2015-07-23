use TestDomain

delete from [TableInheritance_DerivedClassWithEntityWithHierarchy]
delete from [TableInheritance_Order]
delete from [TableInheritance_Address]
delete from [TableInheritance_HistoryEntry]
delete from [TableInheritance_Person]
delete from [TableInheritance_Region]
delete from [TableInheritance_OrganizationalUnit]
delete from [TableInheritance_Client]
delete from [TableInheritance_TableWithUnidirectionalRelation]
delete from [TableInheritance_BaseClassWithInvalidRelationClassIDColumns]
delete from [TableInheritance_File]
delete from [TableInheritance_Folder]


-- Client
insert into [TableInheritance_Client] (ID, ClassID, [Name]) values ('{F7AD91EF-AC75-4fe3-A427-E40312B12917}', 'TI_Client', 'remotion')

-- Note: This client has an OrganizationalUnit assigned with an invalid ClassID:
insert into [TableInheritance_Client] (ID, ClassID, [Name]) values ('{58535280-84EC-41d9-9F8F-BCAC64BB3709}', 'TI_Client', 'ClientWithOrganizationalUnitWithInvalidClassID')

-- ClassWithUnidirectionalRelation
insert into [TableInheritance_TableWithUnidirectionalRelation] (ID, ClassID, [DomainBaseID], [DomainBaseIDClassID])
    values ('{7E7E4002-19BF-4e8b-9525-4634A8D0FCB5}', 'TI_ClassWithUnidirectionalRelation', '{084010C4-82E5-4b0d-AE9F-A953303C03A4}', 'TI_Person')

-- OrganizationalUnit
insert into [TableInheritance_OrganizationalUnit] (ID, ClassID, [ClientID], [CreatedBy], [CreatedAt], [Name]) 
    values ('{C6F4E04D-0465-4a9e-A944-C9FD26E33C44}', 'TI_OrganizationalUnit', '{F7AD91EF-AC75-4fe3-A427-E40312B12917}', 'UnitTests', '2006/1/1', 'Entwicklung')

insert into [TableInheritance_OrganizationalUnit] (ID, ClassID, [ClientID], [CreatedBy], [CreatedAt], [Name]) 
    values ('{1B5BA13A-F6AD-4390-87BB-D85A1C098D1C}', 'InvalidClassID', '{58535280-84EC-41d9-9F8F-BCAC64BB3709}', 'UnitTests', '2006/1/1', 'OrganizationalUnitWithInvalidClassID')

-- Note: This OrganizationalUnit has the same ID as PersonWithSameIDAsOrganizationalUnit.
--       A SqlProvider test checks that RdbmsProvider.LoadDataContainerByRelatedID uses the classID column.
--       This OrganizationalUnit must have an associated HistoryEntry.
insert into [TableInheritance_OrganizationalUnit] (ID, ClassID, [ClientID], [CreatedBy], [CreatedAt], [Name]) 
    values ('{B969AFCB-2CDA-45ff-8490-EB52A86D5464}', 'TI_OrganizationalUnit', null, 'UnitTests', '2006/1/2', 'OrganizationalUnitWithSameIDAsPerson')


-- Region
insert into [TableInheritance_Region] (ID, ClassID, [Name]) values ('{7905CF32-FBC2-47fe-AC40-3E398BEEA5AB}', 'TI_Region', 'NÖ')


-- Person 
insert into [TableInheritance_Person] (ID, ClassID, [ClientID], [CreatedBy], [CreatedAt], [FirstName], [LastName], [DateOfBirth], [Photo])
    values ('{21E9BEA1-3026-430a-A01E-E9B6A39928A8}', 'TI_Person', '{F7AD91EF-AC75-4fe3-A427-E40312B12917}', 'UnitTests', '2006/1/3', 'Max', 'Mustermann', '1980/6/9', null)

-- Note: This person has an OrganizationalUnit with the same ID. It needs at least one HistoryEntry.
insert into [TableInheritance_Person] (ID, ClassID, [ClientID], [CreatedBy], [CreatedAt], [FirstName], [LastName], [DateOfBirth], [Photo])
    values ('{B969AFCB-2CDA-45ff-8490-EB52A86D5464}', 'TI_Person', null, 'UnitTests','2006/1/4', '', 'PersonWithSameIDAsOrganizationalUnit', '1980/6/9', null)
    
insert into [TableInheritance_Person] (ID, ClassID, [ClientID], [CreatedBy], [CreatedAt], [FirstName], [LastName], [DateOfBirth], [Photo])
    values ('{084010C4-82E5-4b0d-AE9F-A953303C03A4}', 'TI_Person', '{F7AD91EF-AC75-4fe3-A427-E40312B12917}', 'UnitTests', '2006/1/4', 'Max',
    'PersonForUnidirectionalRelationTest', '1980/6/9', null)


-- Customer
insert into [TableInheritance_Person] (ID, ClassID, [ClientID], [RegionID], [CreatedBy], [CreatedAt], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince])
    values ('{623016F9-B525-4CAE-A2BD-D4A6155B2F33}', 'TI_Customer', '{F7AD91EF-AC75-4fe3-A427-E40312B12917}', '{7905CF32-FBC2-47fe-AC40-3E398BEEA5AB}',
    'UnitTests', '2006/1/5', 'Zaphod', 'Beeblebrox', '1950/1/1', null, 1, '1992/12/24')

-- Note: The customer's order contains an invalid CustomerIDClassID value.
insert into [TableInheritance_Person] (ID, ClassID, [ClientID], [RegionID], [CreatedBy], [CreatedAt], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince])
    values ('{3C8854E7-16C6-4783-93B2-8C303A881761}', 'TI_Customer', null, null, 'UnitTests', '2006/1/15', '', 'CustomerWithInvalidOrder', '1951/1/1', null, 1, '1992/12/26')


-- HistoryEntry
insert into [TableInheritance_HistoryEntry] (ID, ClassID, [OwnerID], [OwnerIDClassID], [HistoryDate], [Text])
    values ('{0A2A6302-9CCB-4ab2-B006-2F1D89526435}', 'TI_HistoryEntry', '{623016F9-B525-4CAE-A2BD-D4A6155B2F33}', 'TI_Customer', '2006/5/24', 'Kunde angelegt')

insert into [TableInheritance_HistoryEntry] (ID, ClassID, [OwnerID], [OwnerIDClassID], [HistoryDate], [Text])
    values ('{02D662F0-ED50-49b4-8A26-BB6025EDCA8C}', 'TI_HistoryEntry', '{623016F9-B525-4CAE-A2BD-D4A6155B2F33}', 'TI_Customer', '2006/5/25', 'Name geändert')

insert into [TableInheritance_HistoryEntry] (ID, ClassID, [OwnerID], [OwnerIDClassID], [HistoryDate], [Text])
    values ('{9CCCB590-B765-4bc3-B481-AAC67AEEAD7E}', 'TI_HistoryEntry', '{21E9BEA1-3026-430a-A01E-E9B6A39928A8}', 'TI_Person', '2006/5/26', 'Person angelegt')

insert into [TableInheritance_HistoryEntry] (ID, ClassID, [OwnerID], [OwnerIDClassID], [HistoryDate], [Text])
    values ('{840EC009-03FB-4221-85B9-72E480A47373}', 'TI_HistoryEntry', '{B969AFCB-2CDA-45ff-8490-EB52A86D5464}', 'TI_Person', '2006/5/26', 'Person angelegt')

insert into [TableInheritance_HistoryEntry] (ID, ClassID, [OwnerID], [OwnerIDClassID], [HistoryDate], [Text])
    values ('{2C7FB7B3-EB16-43f9-BDDE-B8B3F23A93D2}', 'TI_HistoryEntry', '{B969AFCB-2CDA-45ff-8490-EB52A86D5464}', 'TI_OrganizationalUnit', '2006/5/27', 'OU angelegt')


-- Address
insert into [TableInheritance_Address] (ID, ClassID, [PersonID], [PersonIDClassID], [Street], [Zip], [City], [Country]) 
    values ('{5D5AA233-7371-44bc-807F-7849E8B08302}', 'TI_Address', '{21E9BEA1-3026-430a-A01E-E9B6A39928A8}', 'TI_Person', 'Werdertorgasse 14', '1010', 'Wien', 'Österreich')


-- Order
insert into [TableInheritance_Order] (ID, ClassID, [CustomerID], [CustomerIDClassID], [Number], [OrderDate]) 
    values ('{6B88B60C-1C91-4005-8C60-72053DB48D5D}', 'TI_Order', '{623016F9-B525-4CAE-A2BD-D4A6155B2F33}', 'TI_Customer', 1, '2006/05/24')

-- Note: This Order is invalid, because column CustomerIDClassID refers to abstract class DomainBase.
insert into [TableInheritance_Order] (ID, ClassID, [CustomerID], [CustomerIDClassID], [Number], [OrderDate]) 
    values ('{F404FD2C-B92F-46d8-BEAC-F92C0599BFD3}', 'TI_Order', '{3C8854E7-16C6-4783-93B2-8C303A881761}', 'TI_DomainBase', 1, '2006/01/21')


-- Folder
insert into [TableInheritance_Folder] (ID, ClassID, [Name], [FolderCreatedAt], [ParentFolderID], [ParentFolderIDClassID])
    values ('{1A45A89B-746E-4a9e-AC2C-E960E90C0DAD}', 'TI_Folder', 'Root', '2006/02/01', null, null)

insert into [TableInheritance_Folder] (ID, ClassID, [Name], [FolderCreatedAt], [ParentFolderID], [ParentFolderIDClassID])
    values ('{6B8A65C1-1D49-4dab-97D7-F466F3EAB91E}', 'TI_Folder', 'Ordner 1', '2006/02/02', '{1A45A89B-746E-4a9e-AC2C-E960E90C0DAD}', 'TI_Folder')

-- File
insert into [TableInheritance_File] (ID, ClassID, [Name], [ParentFolderID], [ParentFolderIDClassID], [Size], [FileCreatedAt])
    values ('{023392E2-AB99-434f-A71F-8A9865D10C8C}', 'TI_File', 'Datei im Root', '{1A45A89B-746E-4a9e-AC2C-E960E90C0DAD}', 'TI_Folder', 42, '2006/02/03')

insert into [TableInheritance_File] (ID, ClassID, [Name], [ParentFolderID], [ParentFolderIDClassID], [Size], [FileCreatedAt])
    values ('{6108E150-6D3C-4e38-9865-895BD143D180}', 'TI_File', 'Datei im Ordner 1', '{6B8A65C1-1D49-4dab-97D7-F466F3EAB91E}', 'TI_Folder', 512, '2006/02/04')


-- DerivedClassWithEntityWithHierarchy
insert into [TableInheritance_DerivedClassWithEntityWithHierarchy] 
    (ID, ClassID, [Name], 
    [ParentAbstractBaseClassWithHierarchyID], [ParentAbstractBaseClassWithHierarchyIDClassID], 
    [ParentDerivedClassWithEntityWithHierarchyID], [ParentDerivedClassWithEntityWithHierarchyIDClassID],
    [ClientFromAbstractBaseClassID], [ClientFromDerivedClassWithEntityID],
    [FileSystemItemFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassIDClassID],
    [FileSystemItemFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityIDClassID])
values
    ('{137DA04C-2B53-463e-A893-D8B246D6BFA9}', 'TI_DerivedClassWithEntityWithHierarchy', 'TI_DerivedClassWithEntityWithHierarchy 1',
    null, null,
    null, null,
    '{F7AD91EF-AC75-4fe3-A427-E40312B12917}', '{58535280-84EC-41d9-9F8F-BCAC64BB3709}',
    '{1A45A89B-746E-4a9e-AC2C-E960E90C0DAD}', 'TI_Folder',
    '{023392E2-AB99-434f-A71F-8A9865D10C8C}', 'TI_File')

insert into [TableInheritance_DerivedClassWithEntityWithHierarchy] 
    (ID, ClassID, [Name], 
    [ParentAbstractBaseClassWithHierarchyID], [ParentAbstractBaseClassWithHierarchyIDClassID], 
    [ParentDerivedClassWithEntityWithHierarchyID], [ParentDerivedClassWithEntityWithHierarchyIDClassID],
    [ClientFromAbstractBaseClassID], [ClientFromDerivedClassWithEntityID],
    [FileSystemItemFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassIDClassID],
    [FileSystemItemFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityIDClassID])
values
    ('{6389C3AB-9E65-4bfb-9321-EC9F50B6A479}', 'TI_DerivedClassWithEntityWithHierarchy', 'TI_DerivedClassWithEntityWithHierarchy 2',
    '{137DA04C-2B53-463e-A893-D8B246D6BFA9}', 'TI_DerivedClassWithEntityWithHierarchy',
    '{137DA04C-2B53-463e-A893-D8B246D6BFA9}', 'TI_DerivedClassWithEntityWithHierarchy',
    null, null,
    null, null,
    null, null)

insert into [TableInheritance_DerivedClassWithEntityWithHierarchy] 
    (ID, ClassID, [Name], 
    [ParentAbstractBaseClassWithHierarchyID], [ParentAbstractBaseClassWithHierarchyIDClassID], 
    [ParentDerivedClassWithEntityWithHierarchyID], [ParentDerivedClassWithEntityWithHierarchyIDClassID],
    [ClientFromAbstractBaseClassID], [ClientFromDerivedClassWithEntityID],
    [FileSystemItemFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassIDClassID],
    [FileSystemItemFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityIDClassID])
values
    ('{15526A7A-57EC-42c3-95A7-B40E46784846}', 'TI_DerivedClassWithEntityWithHierarchy', 'TI_DerivedClassWithEntityWithHierarchy 3',
    '{137DA04C-2B53-463e-A893-D8B246D6BFA9}', 'TI_DerivedClassWithEntityWithHierarchy',
    '{137DA04C-2B53-463e-A893-D8B246D6BFA9}', 'TI_DerivedClassWithEntityWithHierarchy',
    null, null,
    null, null,
    null, null)

-- DerivedClassWithEntityFromBaseClassWithHierarchy
insert into [TableInheritance_DerivedClassWithEntityWithHierarchy] 
    (ID, ClassID, [Name], 
    [ParentAbstractBaseClassWithHierarchyID], [ParentAbstractBaseClassWithHierarchyIDClassID], 
    [ParentDerivedClassWithEntityWithHierarchyID], [ParentDerivedClassWithEntityWithHierarchyIDClassID],
    [ParentDerivedClassWithEntityFromBaseClassWithHierarchyID],[ParentDerivedClassWithEntityFromBaseClassWithHierarchyIDClassID],
    [ClientFromAbstractBaseClassID], [ClientFromDerivedClassWithEntityID], [ClientFromDerivedClassWithEntityFromBaseClassID],
    [FileSystemItemFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassIDClassID],
    [FileSystemItemFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityIDClassID],
    [FileSystemItemFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassIDClassID])
values
    ('{24F27B35-68F8-4035-A454-33CFC1AF6339}', 'TI_DerivedClassWithEntityFromBaseClassWithHierarchy', 'TI_DerivedClassWithEntityFromBaseClassWithHierarchy 1',
    '{137DA04C-2B53-463e-A893-D8B246D6BFA9}', 'TI_DerivedClassWithEntityWithHierarchy',
    '{137DA04C-2B53-463e-A893-D8B246D6BFA9}', 'TI_DerivedClassWithEntityWithHierarchy',
    null, null,
    null, null, '{F7AD91EF-AC75-4fe3-A427-E40312B12917}',
    null, null,
    null, null,
    '{6108E150-6D3C-4e38-9865-895BD143D180}', 'TI_File')

insert into [TableInheritance_DerivedClassWithEntityWithHierarchy] 
    (ID, ClassID, [Name], 
    [ParentAbstractBaseClassWithHierarchyID], [ParentAbstractBaseClassWithHierarchyIDClassID], 
    [ParentDerivedClassWithEntityWithHierarchyID], [ParentDerivedClassWithEntityWithHierarchyIDClassID],
    [ParentDerivedClassWithEntityFromBaseClassWithHierarchyID],[ParentDerivedClassWithEntityFromBaseClassWithHierarchyIDClassID],
    [ClientFromAbstractBaseClassID], [ClientFromDerivedClassWithEntityID], [ClientFromDerivedClassWithEntityFromBaseClassID],
    [FileSystemItemFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassIDClassID],
    [FileSystemItemFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityIDClassID],
    [FileSystemItemFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassIDClassID])
values
    ('{9C730A8A-8F83-4b26-AF40-FB0C3D4DD387}', 'TI_DerivedClassWithEntityFromBaseClassWithHierarchy', 'TI_DerivedClassWithEntityFromBaseClassWithHierarchy 2',
    '{6389C3AB-9E65-4bfb-9321-EC9F50B6A479}', 'TI_DerivedClassWithEntityWithHierarchy',
    '{6389C3AB-9E65-4bfb-9321-EC9F50B6A479}', 'TI_DerivedClassWithEntityWithHierarchy',
    '{24F27B35-68F8-4035-A454-33CFC1AF6339}', 'TI_DerivedClassWithEntityFromBaseClassWithHierarchy',
    null, null, null,
    null, null,
    null, null,
    null, null)

insert into [TableInheritance_DerivedClassWithEntityWithHierarchy] 
    (ID, ClassID, [Name], 
    [ParentAbstractBaseClassWithHierarchyID], [ParentAbstractBaseClassWithHierarchyIDClassID], 
    [ParentDerivedClassWithEntityWithHierarchyID], [ParentDerivedClassWithEntityWithHierarchyIDClassID],
    [ParentDerivedClassWithEntityFromBaseClassWithHierarchyID],[ParentDerivedClassWithEntityFromBaseClassWithHierarchyIDClassID],
    [ClientFromAbstractBaseClassID], [ClientFromDerivedClassWithEntityID], [ClientFromDerivedClassWithEntityFromBaseClassID],
    [FileSystemItemFromAbstractBaseClassID], [FileSystemItemFromAbstractBaseClassIDClassID],
    [FileSystemItemFromDerivedClassWithEntityID], [FileSystemItemFromDerivedClassWithEntityIDClassID],
    [FileSystemItemFromDerivedClassWithEntityFromBaseClassID], [FileSystemItemFromDerivedClassWithEntityFromBaseClassIDClassID])
values
    ('{953B2E51-C324-4f86-8FA0-3AFA2A2E4E72}', 'TI_DerivedClassWithEntityFromBaseClassWithHierarchy', 'TI_DerivedClassWithEntityFromBaseClassWithHierarchy 3',
    '{15526A7A-57EC-42c3-95A7-B40E46784846}', 'TI_DerivedClassWithEntityWithHierarchy',
    '{15526A7A-57EC-42c3-95A7-B40E46784846}', 'TI_DerivedClassWithEntityWithHierarchy',
    '{24F27B35-68F8-4035-A454-33CFC1AF6339}', 'TI_DerivedClassWithEntityFromBaseClassWithHierarchy',
    null, null, null,
    null, null,
    null, null,
    null, null)


-- Tables with invalid database structure for exception testing only ---------------------------------------------------------

-- DerivedClassWithInvalidRelationClassIDColumns

-- Note: Foreign key values are irrelevant, because table structure is invalid
insert into [TableInheritance_BaseClassWithInvalidRelationClassIDColumns] 
    (ID, ClassID, [ClientID], [ClientIDClassID], [DomainBaseID], 
    [DomainBaseWithInvalidClassIDValueID], [DomainBaseWithInvalidClassIDValueIDClassID], 
    [DomainBaseWithInvalidClassIDNullValueID], [DomainBaseWithInvalidClassIDNullValueIDClassID]) 
values (
    '{BEBF584B-31A6-4d5e-8628-7EACE9034588}', 'DerivedClassWithInvalidRelationClassIDColumns', NEWID(), 'TI_Client', NEWID(), 
    null, 'TI_Person', NEWID(), null)