// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public sealed class DomainObjectIDs
  {
    private readonly IMappingConfiguration _mappingConfiguration;

    public DomainObjectIDs (IMappingConfiguration mappingConfiguration)
    {
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);

      _mappingConfiguration = mappingConfiguration;
    }

    #region Employee

    // Supervisor: -
    // Subordinates: Employee4, Employee5
    // Computer: -
    public ObjectID Employee1
    {
      get { return CreateObjectID ("Employee", new Guid ("{51ECE39B-F040-45b0-8B72-AD8B45353990}")); }
    }

    // Supervisor: -
    // Subordinates: Employee3
    // Computer: -
    public ObjectID Employee2
    {
      get { return CreateObjectID ("Employee", new Guid ("{C3B2BBC3-E083-4974-BAC7-9CEE1FB85A5E}")); }
    }

    // Supervisor: Employee2
    // Subordinates: -
    // Computer: Computer1
    public ObjectID Employee3
    {
      get { return CreateObjectID ("Employee", new Guid ("{3C4F3FC8-0DB2-4c1f-AA00-ADE72E9EDB32}")); }
    }

    // Supervisor: Employee1
    // Subordinates: -
    // Computer: Computer2
    public ObjectID Employee4
    {
      get { return CreateObjectID ("Employee", new Guid ("{890BF138-7559-40d6-9C7F-436BC1AD4F59}")); }
    }

    // Supervisor: Employee1
    // Subordinates: -
    // Computer: Computer3
    public ObjectID Employee5
    {
      get { return CreateObjectID ("Employee", new Guid ("{43329F84-D8BB-4988-BFD2-96D4F48EE5DE}")); }
    }

    // Supervisor: -
    // Subordinates: Employee7
    // Computer: -
    public ObjectID Employee6
    {
      get { return CreateObjectID ("Employee", new Guid ("{3A24D098-EAAD-4dd7-ADA2-932D9B6935F1}")); }
    }


    // Supervisor: Employee6
    // Subordinates: -
    // Computer: -
    public ObjectID Employee7
    {
      get { return CreateObjectID ("Employee", new Guid ("{DBD9EA74-8C97-4411-AC02-9205D1D6D031}")); }
    }

    #endregion

    #region Computer

    // Employee: Employee3
    public ObjectID Computer1
    {
      get { return CreateObjectID ("Computer", new Guid ("{C7C26BF5-871D-48c7-822A-E9B05AAC4E5A}")); }
    }

    // Employee: Employee4
    public ObjectID Computer2
    {
      get { return CreateObjectID ("Computer", new Guid ("{176A0FF6-296D-4934-BD1A-23CF52C22411}")); }
    }

    // Employee: Employee5
    public ObjectID Computer3
    {
      get { return CreateObjectID ("Computer", new Guid ("{704CE38C-4A08-4ef2-A6FE-9ED849BA31E5}")); }
    }

    // Employee: -
    public ObjectID Computer4
    {
      get { return CreateObjectID ("Computer", new Guid ("{D6F50E77-2041-46b8-A840-AAA4D2E1BF5A}")); }
    }

    // Employee: -
    public ObjectID Computer5
    {
      get { return CreateObjectID ("Computer", new Guid ("{AEAC0C5D-44E0-45cc-B716-103B0A4981A4}")); }
    }

    #endregion

    #region Person

    public ObjectID Person1
    {
      get { return CreateObjectID ("Person", new Guid ("{2001BF42-2AA4-4c81-AD8E-73E9145411E9}")); }
    }

    public ObjectID Person2
    {
      get { return CreateObjectID ("Person", new Guid ("{DC50A962-EC95-4cf6-A4E7-A6608EAA23C8}")); }
    }

    public ObjectID Person3
    {
      get { return CreateObjectID ("Person", new Guid ("{10F36130-E97B-4078-A535-B79E07F16AB2}")); }
    }

    public ObjectID Person4
    {
      get { return CreateObjectID ("Person", new Guid ("{45C6730A-DE0B-40d2-9D35-C1E56B8A89D6}")); }
    }

    public ObjectID Person5
    {
      get { return CreateObjectID ("Person", new Guid ("{70C91528-4DB4-4e6a-B3F8-70C53A728DCC}")); }
    }

    public ObjectID Person6
    {
      get { return CreateObjectID ("Person", new Guid ("{19C04A28-094F-4d1f-9705-E2FC7107A68F}")); }
    }

    public ObjectID Person7
    {
      get { return CreateObjectID ("Person", new Guid ("{E4F6F59F-80F7-4e41-A004-1A5BA0F68F78}")); }
    }

    public ObjectID ContactPersonInTwoOrganizations
    {
      get { return CreateObjectID ("Person", new Guid ("{911957D1-483C-4a8b-AA53-FF07464C58F9}")); }
    }

    #endregion

    #region IndustrialSector

    // Companies: Customer1, Partner1, PartnerWithoutCeo, Supplier1, Distributor2
    public ObjectID IndustrialSector1
    {
      get { return CreateObjectID ("IndustrialSector", new Guid ("{3BB7BEE9-2AF9-4a85-998E-618BEBBE5A6B}")); }
    }

    // Companies: Company1, Company2, Customer2, Customer3, Partner2, Supplier2, Distributor1
    public ObjectID IndustrialSector2
    {
      get { return CreateObjectID ("IndustrialSector", new Guid ("{8565A077-EA01-4b5d-BEAA-293DC484BDDC}")); }
    }

    // Companies: DistributorWithoutContactPerson
    public ObjectID IndustrialSector3
    {
      get { return CreateObjectID ("IndustrialSector", new Guid ("{53B322BF-25D8-4fe1-96C8-508E055143E7}")); }
    }

    #endregion

    #region Company

    // IndustrialSector: IndustrialSector2
    // Ceo: Ceo1
    public ObjectID Company1
    {
      get { return CreateObjectID ("Company", new Guid ("{C4954DA8-8870-45c1-B7A3-C7E5E6AD641A}")); }
    }

    // IndustrialSector: IndustrialSector2
    // Ceo: Ceo2
    public ObjectID Company2
    {
      get { return CreateObjectID ("Company", new Guid ("{A21A9EC2-17D6-44de-9F1A-2AB6FC3742DF}")); }
    }

    #endregion

    #region Customer

    // IndustrialSector: IndustrialSector1
    // Ceo: Ceo3
    // Orders: Order1, Order2
    // CustomerType: Standard
    // Name: Kunde 1
    public ObjectID Customer1
    {
      get { return CreateObjectID ("Customer", new Guid ("{55B52E75-514B-4e82-A91B-8F0BB59B80AD}")); }
    }

    // IndustrialSector: IndustrialSector2
    // Ceo: Ceo4
    // Orders: -
    // CustomerType: Premium
    public ObjectID Customer2
    {
      get { return CreateObjectID ("Customer", new Guid ("{F577F879-2DB4-4a3c-A18A-AFB4E57CE098}")); }
    }

    // IndustrialSector: IndustrialSector2
    // Ceo: Ceo5
    // Orders: Order3
    // CustomerType: Gold
    public ObjectID Customer3
    {
      get { return CreateObjectID ("Customer", new Guid ("{DD3E3D55-C16F-497f-A3E1-384D08DE0D66}")); }
    }


    // IndustrialSector: -
    // Ceo: Ceo12
    // Orders: Order4, Order5
    // CustomerType: Gold
    public ObjectID Customer4
    {
      get { return CreateObjectID ("Customer", new Guid ("{B3F0A333-EC2A-4ddd-9035-9ADA34052450}")); }
    }

    public ObjectID Customer5
    {
      get { return CreateObjectID ("Customer", new Guid ("{DA658F26-8107-44CE-9DD0-1804503ECCAF}")); }
    }

    #endregion

    #region Partner

    // IndustrialSector: IndustrialSector1
    // ContactPerson: Person1
    // Ceo: Ceo6
    public ObjectID Partner1
    {
      get { return CreateObjectID ("Partner", new Guid ("{5587A9C0-BE53-477d-8C0A-4803C7FAE1A9}")); }
    }

    // IndustrialSector: IndustrialSector2
    // ContactPerson: Person2
    // Ceo: Ceo7
    public ObjectID Partner2
    {
      get { return CreateObjectID ("Partner", new Guid ("{B403E58E-9FA5-47ed-883C-73420D64DEB3}")); }
    }

    // IndustrialSector: IndustrialSector1
    // ContactPerson: Person7
    // Ceo: -
    public ObjectID PartnerWithoutCeo
    {
      get { return CreateObjectID ("Partner", new Guid ("{A65B123A-6E17-498e-A28E-946217C0AE30}")); }
    }

    #endregion

    #region Supplier

    // IndustrialSector: IndustrialSector1
    // ContactPerson: Person3
    // Ceo: Ceo8
    public ObjectID Supplier1
    {
      get { return CreateObjectID ("Supplier", new Guid ("{FD392135-1FDD-42a3-8E2F-232BAB9893A2}")); }
    }

    // IndustrialSector: IndustrialSector2
    // ContactPerson: Person4
    // Ceo: Ceo9
    public ObjectID Supplier2
    {
      get { return CreateObjectID ("Supplier", new Guid ("{92A8BB6A-412A-4fe3-9B09-3E1B6136E425}")); }
    }

    #endregion

    #region Distributor

    // IndustrialSector: IndustrialSector2
    // ContactPerson: Person5
    // Ceo: Ceo10
    public ObjectID Distributor1
    {
      get { return CreateObjectID ("Distributor", new Guid ("{E4087155-D60A-4d31-95B3-9A401A3E4E78}")); }
    }

    // IndustrialSector: IndustrialSector1
    // ContactPerson: Person6
    // Ceo: Ceo11
    public ObjectID Distributor2
    {
      get { return CreateObjectID ("Distributor", new Guid ("{247206C3-7B48-4e17-91DD-3363B568D7E4}")); }
    }

    // IndustrialSector: IndustrialSector3
    // ContactPerson: -
    // Ceo: -
    public ObjectID DistributorWithoutContactPersonAndCeo
    {
      get { return CreateObjectID ("Distributor", new Guid ("{1514D668-A0A5-40e9-AC22-F24900E0EB39}")); }
    }

    #endregion

    #region Order

    // OrderTicket: OrderTicket1
    // OrderItems: OrderItem1, OrderItem2
    // Customer: Customer1
    // Official: Official1
    // OrderNumber: 1
    public ObjectID Order1
    {
      get { return CreateObjectID ("Order", new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}")); }
    }
    
    // OrderTicket: OrderTicket2
    // OrderItems: OrderItem6
    // Customer: Customer1
    // Official: Official1
    // OrderNumber: 2
    public ObjectID Order2
    {
      get { return CreateObjectID ("Order", new Guid ("{F4016F41-F4E4-429e-B8D1-659C8C480A67}")); }
    }

    // OrderTicket: OrderTicket3
    // OrderItems: OrderItem3
    // Customer: Customer3
    // Official: Official1
    // OrderNumber: 3
    public ObjectID Order3
    {
      get { return CreateObjectID ("Order", new Guid ("{83445473-844A-4d3f-A8C3-C27F8D98E8BA}")); }
    }

    // OrderTicket: OrderTicket4
    // OrderItems: OrderItem4
    // Customer: Customer4
    // Official: Official1
    // OrderNumber: 4
    public ObjectID Order4
    {
      get { return CreateObjectID ("Order", new Guid ("{3C0FB6ED-DE1C-4e70-8D80-218E0BF58DF3}")); }
    }

    // OrderTicket: OrderTicket5
    // OrderItems: OrderItem5
    // Customer: Customer4
    // Official: Official1
    // OrderNumber: 5
    public ObjectID Order5
    {
      get { return CreateObjectID ("Order", new Guid ("{90E26C86-611F-4735-8D1B-E1D0918515C2}")); }
    }

    // OrderTicket: OrderTicket6
    // OrderItems: OrderItem6
    // Customer: Customer5
    // Official: Official2
    // OrderNumber: 99
    public ObjectID OrderWithoutOrderItems
    {
      get { return CreateObjectID ("Order", new Guid ("{F7607CBC-AB34-465C-B282-0531D51F3B04}")); }
    }

    // OrderTicket: -
    // OrderItems: -
    // Customer: invalid
    // Official: does not exist
    // OrderNumber: 6
    public ObjectID InvalidOrder
    {
      get { return CreateObjectID ("Order", new Guid ("{DA658F26-8107-44ce-9DD0-1804503ECCAF}")); }
    }

    #endregion

    #region OrderItem

    // Order: Order1
    // Product: Mainboard
    public ObjectID OrderItem1
    {
      get { return CreateObjectID ("OrderItem", new Guid ("{2F4D42C7-7FFA-490d-BFCD-A9101BBF4E1A}")); }
    }

    // Order: Order1
    // Product: CPU Fan
    public ObjectID OrderItem2
    {
      get { return CreateObjectID ("OrderItem", new Guid ("{AD620A11-4BC4-4791-BCF4-A0770A08C5B0}")); }
    }

    // Order: Order3
    // Product: Harddisk
    public ObjectID OrderItem3
    {
      get { return CreateObjectID ("OrderItem", new Guid ("{0D7196A5-8161-4048-820D-B1BBDABE3293}")); }
    }

    // Order: Order4
    // Product: Hitchhiker's guide
    public ObjectID OrderItem4
    {
      get { return CreateObjectID ("OrderItem", new Guid ("{DC20E0EB-4B55-4f23-89CF-6D6478F96D3B}")); }
    }

    // Order: Order5
    // Product: Blumentopf
    public ObjectID OrderItem5
    {
      get { return CreateObjectID ("OrderItem", new Guid ("{EA505094-770A-4505-82C1-5A4F94F56FE2}")); }
    }

    // Order: Order2
    // Product: Solar panel
    public ObjectID OrderItem6
    {
      get { return CreateObjectID ("OrderItem", new Guid ("{5A33809B-06F5-4F62-B103-C8E1869D36EF}")); }
    }

    // Order: -
    // Product: Rubik's Cube
    public ObjectID OrderItemWithoutOrder
    {
      get { return CreateObjectID ("OrderItem", new Guid ("{386D99F9-B0BA-4C55-8F22-BF194A3D745A}")); }
    }

    #endregion

    #region OrderTicket

    // Order: Order1
    public ObjectID OrderTicket1
    {
      get { return CreateObjectID ("OrderTicket", new Guid ("{058EF259-F9CD-4cb1-85E5-5C05119AB596}")); }
    }

    // Order: Order2
    public ObjectID OrderTicket2
    {
      get { return CreateObjectID ("OrderTicket", new Guid ("{0005BDF4-4CCC-4a41-B9B5-BAAB3EB95237}")); }
    }

    // Order: Order3
    public ObjectID OrderTicket3
    {
      get { return CreateObjectID ("OrderTicket", new Guid ("{BCF6C5F6-323F-4471-9CA5-7DF0A48C7A59}")); }
    }

    // Order: Order4
    public ObjectID OrderTicket4
    {
      get { return CreateObjectID ("OrderTicket", new Guid ("{6768DB2B-9C66-4e2f-BBA2-89C56718FF2B}")); }
    }

    // Order: Order5
    public ObjectID OrderTicket5
    {
      get { return CreateObjectID ("OrderTicket", new Guid ("{DC20E0EB-4B55-4f23-89CF-6D6478F96D3B}")); }
    }

    // Order: OrderWithoutOrderItems
    public ObjectID OrderTicket99
    {
      get { return CreateObjectID ("OrderTicket", new Guid ("{87E9C075-B208-4475-923D-7BF3B50AB18E}")); }
    }

    #endregion

    #region Ceo

    // Company: Company1
    public ObjectID Ceo1
    {
      get { return CreateObjectID ("Ceo", new Guid ("{A1691AF1-F96D-42e1-B021-B5099840D572}")); }
    }

    // Company: Company2
    public ObjectID Ceo2
    {
      get { return CreateObjectID ("Ceo", new Guid ("{A6A848CE-505F-4cd3-A337-1F5EEA1D2260}")); }
    }

    // Company: Customer1
    public ObjectID Ceo3
    {
      get { return CreateObjectID ("Ceo", new Guid ("{481C7840-9D8A-4872-BBCD-B41A9BD85528}")); }
    }

    // Company: Customer2
    public ObjectID Ceo4
    {
      get { return CreateObjectID ("Ceo", new Guid ("{BE7F24E2-600C-4cd8-A7C3-8669AFD54154}")); }
    }

    // Company: Customer3
    public ObjectID Ceo5
    {
      get { return CreateObjectID ("Ceo", new Guid ("{7236BA88-48C6-415f-A0BA-A328A1A22DFE}")); }
    }

    // Company: Partner1
    public ObjectID Ceo6
    {
      get { return CreateObjectID ("Ceo", new Guid ("{C7837D11-C1D6-458f-A3F7-7D5C96C1F726}")); }
    }

    // Company: Partner2
    public ObjectID Ceo7
    {
      get { return CreateObjectID ("Ceo", new Guid ("{9F0AC953-E78E-4939-8AFE-0EFF9B3B3ED9}")); }
    }

    // Company: Supplier1
    public ObjectID Ceo8
    {
      get { return CreateObjectID ("Ceo", new Guid ("{394C69B2-BD40-48d1-A2AE-A73FB63C0B66}")); }
    }

    // Company: Supplier2
    public ObjectID Ceo9
    {
      get { return CreateObjectID ("Ceo", new Guid ("{421D04B4-BC77-4682-B0FE-58B96802C524}")); }
    }

    // Company: Distributor1
    public ObjectID Ceo10
    {
      get { return CreateObjectID ("Ceo", new Guid ("{6B801331-2163-4837-B20C-973BD9B8768E}")); }
    }

    // Company: Distributor2
    public ObjectID Ceo11
    {
      get { return CreateObjectID ("Ceo", new Guid ("{2E8AE776-DC3A-45a5-9B0C-35900CC78FDC}")); }
    }

    // Company: Customer4
    public ObjectID Ceo12
    {
      get { return CreateObjectID ("Ceo", new Guid ("{FD1B587C-3E26-43f8-9866-8B770194D70F}")); }
    }

    #endregion

    #region Official

    // Orders: Order1, Order3, Order2, Order4, Order5
    public ObjectID Official1
    {
      get { return CreateObjectID ("Official", 1); }
    }

    // Orders: -
    public ObjectID Official2
    {
      get { return CreateObjectID ("Official", 2); }
    }

    #endregion

    #region Client

    // ChildClients: Client2, Client3
    // ParentClient: -
    // Location: Location1, Location2
    public ObjectID Client1
    {
      get { return CreateObjectID ("Client", new Guid ("{1627ADE8-125F-4819-8E33-CE567C42B00C}")); }
    }

    // ChildClients: -
    // ParentClient: Client1
    // Location: Location3
    public ObjectID Client2
    {
      get { return CreateObjectID ("Client", new Guid ("{090D54F2-738C-48ac-9C78-F40365A72305}")); }
    }

    // ChildClients: -
    // ParentClient: Client1
    // Location: -
    public ObjectID Client3
    {
      get { return CreateObjectID ("Client", new Guid ("{01349595-88A3-4583-A7BA-CB08795C97F6}")); }
    }

    // ChildClients: -
    // ParentClient: -
    // Location: -
    public ObjectID Client4
    {
      get { return CreateObjectID ("Client", new Guid ("{015E25B1-ACFA-4364-87F5-D28A45384D11}")); }
    }

    #endregion

    #region Location

    // Client: Client1
    public ObjectID Location1
    {
      get { return CreateObjectID ("Location", new Guid ("{D527B630-B0AC-4572-A614-EAC9F486148D}")); }
    }

    // Client: Client1
    public ObjectID Location2
    {
      get { return CreateObjectID ("Location", new Guid ("{20380C9D-B70F-4d9a-880E-EAE5D6E3919C}")); }
    }

    // Client: Client2
    public ObjectID Location3
    {
      get { return CreateObjectID ("Location", new Guid ("{903E7EE5-CBB8-44c0-BEB6-ACAFFA5ADA7F}")); }
    }

    #endregion

    #region ClassWithAllDataTypes

    public ObjectID ClassWithAllDataTypes1
    {
      get { return CreateObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}")); }
    }

    public ObjectID ClassWithAllDataTypes2
    {
      get { return CreateObjectID ("ClassWithAllDataTypes", new Guid ("{583EC716-8443-4b55-92BF-09F7C8768529}")); }
    }

    #endregion

    #region StorageGroupClass

    public ObjectID StorageGroupClass1
    {
      get { return CreateObjectID ("StorageGroupClass", new Guid ("{09755471-E551-496d-941B-84D90D0C9ECA}")); }
    }

    public ObjectID StorageGroupClass2
    {
      get { return CreateObjectID ("StorageGroupClass", new Guid ("{F394AE2E-CB4E-4e38-8E08-9C847EE1F376}")); }
    }

    #endregion

    #region TargetClassForPersistentMixins

    // PersistentProperty: 99
    // ExtraPersistentProperty: 100
    // RelationProperty: -
    // VirtualRelationProperty: -
    // CollectionProperty1Side: -
    // CollectionPropertyNSide: -
    public ObjectID TargetClassForPersistentMixins1
    {
      get { return CreateObjectID ("TargetClassForPersistentMixin", new Guid ("{784EBDDD-EE94-456D-A5F4-F6CB1B41B6CA}")); }
    }

    // PersistentProperty: 13
    // ExtraPersistentProperty: 1333
    // RelationProperty: RelationTargetForPersistentMixin1
    // VirtualRelationProperty: RelationTargetForPersistentMixin2
    // CollectionProperty1Side: RelationTargetForPersistentMixin3
    // CollectionPropertyNSide: RelationTargetForPersistentMixin4
    // UnidirectionalRelationProperty: RelationTargetForPersistentMixin5
    public ObjectID TargetClassForPersistentMixins2
    {
      get { return CreateObjectID ("TargetClassForPersistentMixin", new Guid ("{FF79502F-FF40-45E0-929A-230006EA3E83}")); }
    }

    // PersistentProperty: 199
    // ExtraPersistentProperty: 100
    // RelationProperty: -
    // VirtualRelationProperty: -
    // CollectionProperty1Side: -
    // CollectionPropertyNSide: -
    public ObjectID DerivedTargetClassForPersistentMixin1
    {
      get { return CreateObjectID ("DerivedTargetClassForPersistentMixin", new Guid ("{4ED563B8-B337-4C8E-9A77-5FA907919377}")); }
    }

    // PersistentProperty: 299
    // ExtraPersistentProperty: 100
    // RelationProperty: -
    // VirtualRelationProperty: -
    // CollectionProperty1Side: -
    // CollectionPropertyNSide: -
    public ObjectID DerivedDerivedTargetClassForPersistentMixin1
    {
      get { return CreateObjectID ("DerivedDerivedTargetClassForPersistentMixin", new Guid ("{B551C440-8C80-4930-A2A1-7FBB4F6B69D8}")); }
    }

    // PersistentProperty: 199
    // ExtraPersistentProperty: 100
    // RelationProperty: -
    // VirtualRelationProperty: -
    // CollectionProperty1Side: -
    // CollectionPropertyNSide: -
    public ObjectID DerivedTargetClassWithDerivedMixinWithInterface1
    {
      get { return CreateObjectID ("DerivedTargetClassWithDerivedMixinWithInterface", new Guid ("{5FFD52D9-2A38-4DEC-9AA1-FA76C30B91A4}")); }
    }

    #endregion

    #region RelationTargetForPersistentMixin

    public ObjectID RelationTargetForPersistentMixin1
    {
      get { return CreateObjectID ("RelationTargetForPersistentMixin", new Guid ("{DC42158C-7DA6-4D5C-B522-C0879E404DEC}")); }
    }

    public ObjectID RelationTargetForPersistentMixin2
    {
      get { return CreateObjectID ("RelationTargetForPersistentMixin", new Guid ("{332F9971-E6FD-411C-862A-23416E0019BC}")); }
    }

    public ObjectID RelationTargetForPersistentMixin3
    {
      get { return CreateObjectID ("RelationTargetForPersistentMixin", new Guid ("{58458546-9C4A-4E36-9D62-C6CF171748A6}")); }
    }

    public ObjectID RelationTargetForPersistentMixin4
    {
      get { return CreateObjectID ("RelationTargetForPersistentMixin", new Guid ("{A5AC369E-9742-412C-8275-4B31B48CEFF3}")); }
    }

    public ObjectID RelationTargetForPersistentMixin5
    {
      get { return CreateObjectID ("RelationTargetForPersistentMixin", new Guid ("{C007F590-7953-4429-A34E-778309F2FC1D}")); }
    }

    #endregion

    #region EagerFetching

    public ObjectID EagerFetching_BaseClass
    {
      get { return CreateObjectID ("EagerFetching_BaseClass", new Guid ("{DA3B6AA5-9DEF-4048-9479-6ADB940EB88C}")); }
    }

    public ObjectID EagerFetching_DerivedClass1_WithCollectionVirtualEndPoint
    {
      get { return CreateObjectID ("EagerFetching_DerivedClass1", new Guid ("{BD4403A5-E96D-426E-A7B0-4A5DB2363D3A}")); }
    }

    public ObjectID EagerFetching_DerivedClass1_WithScalarVirtualEndPoint
    {
      get { return CreateObjectID ("EagerFetching_DerivedClass1", new Guid ("{80B729A2-1FC6-46D0-A69B-39B881346588}")); }
    }

    public ObjectID EagerFetching_DerivedClass2_WithScalarRealEndPoint
    {
      get { return CreateObjectID ("EagerFetching_DerivedClass2", new Guid ("{10742FC3-AB58-49BC-AF04-CBDE1B160CBF}")); }
    }

    public ObjectID EagerFetching_DerivedClass2_WithUnidirectionalEndPoint
    {
      get { return CreateObjectID ("EagerFetching_DerivedClass2", new Guid ("{F6090162-7B12-4689-951B-B52EFD72C34F}")); }
    }
    
    public ObjectID EagerFetching_RelationTarget_WithCollectionRealEndPoint1
    {
      get { return CreateObjectID ("EagerFetching_RelationTarget", new Guid ("{3DD744D2-91C7-463B-A5AF-01AD064B3821}")); }
    }
    
    public ObjectID EagerFetching_RelationTarget_WithCollectionRealEndPoint2
    {
      get { return CreateObjectID ("EagerFetching_RelationTarget", new Guid ("{FF5709EB-2523-44B6-B0EA-92423AE705F5}")); }
    }
    
    public ObjectID EagerFetching_RelationTarget_WithScalarRealEndPoint
    {
      get { return CreateObjectID ("EagerFetching_RelationTarget", new Guid ("{36381FD9-D454-4FCD-B860-3E30166BE092}")); }
    }
    
    public ObjectID EagerFetching_RelationTarget_WithScalarVirtualEndPoint
    {
      get { return CreateObjectID ("EagerFetching_RelationTarget", new Guid ("{5DD64E95-5449-4345-8DE8-1D766BB23B17}")); }
    }
    
    public ObjectID EagerFetching_RelationTarget_WithAnonymousEndPoint
    {
      get { return CreateObjectID ("EagerFetching_RelationTarget", new Guid ("{03CDC3BF-DA09-442D-A2C0-96567B48BC5D}")); }
    }
    
    #endregion

    private ObjectID CreateObjectID (string classID, object value)
    {
      return new ObjectID(_mappingConfiguration.GetClassDefinition (classID), value);
    }
  }
}