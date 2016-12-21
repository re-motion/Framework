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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RelationEndPointManagerTest : ClientTransactionBaseTest
  {
    private RelationEndPointManager _relationEndPointManager;

    public override void SetUp ()
    {
      base.SetUp();

      _relationEndPointManager = (RelationEndPointManager) DataManagerTestHelper.GetRelationEndPointManager (TestableClientTransaction.DataManager);
    }

    [Test]
    public void CreateNullEndPoint_RealObjectEndPoint ()
    {
      var orderTicketDefinition =
          Configuration.GetTypeDefinition (typeof (OrderTicket)).GetRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");

      var nullObjectEndPoint = RelationEndPointManager.CreateNullEndPoint (TestableClientTransaction, orderTicketDefinition);

      Assert.That (nullObjectEndPoint, Is.TypeOf (typeof (NullRealObjectEndPoint)));
      var objectEndPointID = RelationEndPointID.Create (null, orderTicketDefinition);
      Assert.That (nullObjectEndPoint.ID, Is.EqualTo (objectEndPointID));
    }

    [Test]
    public void CreateNullEndPoint_VirtualObjectEndPoint ()
    {
      var orderTicketDefinition =
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");

      var nullObjectEndPoint = RelationEndPointManager.CreateNullEndPoint (TestableClientTransaction, orderTicketDefinition);

      Assert.That (nullObjectEndPoint, Is.TypeOf (typeof (NullVirtualObjectEndPoint)));
      var objectEndPointID = RelationEndPointID.Create (null, orderTicketDefinition);
      Assert.That (nullObjectEndPoint.ID, Is.EqualTo (objectEndPointID));
    }

    [Test]
    public void CreateNullEndPoint_CollectionEndPoint ()
    {
      var orderItemsDefinition = 
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

      var nullObjectEndPoint = RelationEndPointManager.CreateNullEndPoint (TestableClientTransaction, orderItemsDefinition);

      Assert.That (nullObjectEndPoint, Is.TypeOf (typeof (NullCollectionEndPoint)));
      var collectionEndPointID = RelationEndPointID.Create (null, orderItemsDefinition);
      Assert.That (nullObjectEndPoint.ID, Is.EqualTo (collectionEndPointID));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersRealObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, DomainObjectIDs.Order3);
      var foreignKeyProperty = GetPropertyDefinition (typeof (OrderTicket), "Order");

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      var endPoint = (RealObjectEndPoint) _relationEndPointManager.RelationEndPoints[endPointID];
      Assert.That (endPoint, Is.Not.Null);
      Assert.That (endPoint.PropertyDefinition, Is.EqualTo (foreignKeyProperty));
      Assert.That (endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order3));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersOppositeVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, DomainObjectIDs.Order3);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      var oppositeID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order3, "OrderTicket");
      var oppositeEndPoint = (IVirtualObjectEndPoint) _relationEndPointManager.RelationEndPoints[oppositeID];

      Assert.That (oppositeEndPoint, Is.Not.Null);
      Assert.That (oppositeEndPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersNoOppositeNullObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, null);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      var oppositeEndPointDefinition = endPointID.Definition.GetOppositeEndPointDefinition ();
      var expectedID = RelationEndPointID.Create (null, oppositeEndPointDefinition);

      Assert.That (_relationEndPointManager.RelationEndPoints[expectedID], Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersNoCollectionEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var dataContainer = RelationEndPointTestHelper.CreateExistingDataContainer (endPointID);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersNoVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var dataContainer = RelationEndPointTestHelper.CreateExistingDataContainer (endPointID);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_New_RegistersVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (endPointID);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      var objectEndPoint = (IVirtualObjectEndPoint) _relationEndPointManager.RelationEndPoints[endPointID];
      Assert.That (objectEndPoint, Is.Not.Null);
      Assert.That (objectEndPoint.IsDataComplete, Is.True);
      Assert.That (objectEndPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_New_RegistersRealObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (endPointID);
      var foreignKeyProperty = GetPropertyDefinition (typeof (OrderTicket), "Order");

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      var objectEndPoint = (RealObjectEndPoint) _relationEndPointManager.RelationEndPoints[endPointID];
      Assert.That (objectEndPoint.PropertyDefinition, Is.EqualTo (foreignKeyProperty));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_New_RegistersCollectionEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (endPointID);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      var collectionEndPoint = (ICollectionEndPoint) _relationEndPointManager.RelationEndPoints[endPointID];
      Assert.That (collectionEndPoint, Is.Not.Null);
      Assert.That (collectionEndPoint.IsDataComplete, Is.True);
      Assert.That (collectionEndPoint.Collection, Is.Empty);
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_Existing_IncludesRealObjectEndPoints_IgnoresVirtualEndPoints ()
    {
      var realEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (realEndPointID, DomainObjectIDs.Order3);
      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);
      var realEndPoint = _relationEndPointManager.RelationEndPoints[realEndPointID];

      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var virtualObjectEndPointStub = MockRepository.GenerateStub<IVirtualObjectEndPoint> ();
      virtualObjectEndPointStub.Stub (stub => stub.ID).Return (virtualObjectEndPointID);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, virtualObjectEndPointStub);

      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var collectionEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      collectionEndPointStub.Stub (stub => stub.ID).Return (collectionEndPointID);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, collectionEndPointStub);

      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<UnregisterEndPointsCommand> ());
      Assert.That (((UnregisterEndPointsCommand) command).RegistrationAgent, Is.SameAs (_relationEndPointManager.RegistrationAgent));
      Assert.That (((UnregisterEndPointsCommand) command).Map, Is.SameAs (_relationEndPointManager.RelationEndPoints));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.Member (realEndPoint));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.No.Member (virtualObjectEndPointStub));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.No.Member (collectionEndPointStub));
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_New_IncludesRealObjectEndPoints_IncludesVirtualEndPoints ()
    {
      var realEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (realEndPointID);
      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);
      var realEndPoint = _relationEndPointManager.RelationEndPoints[realEndPointID];

      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var virtualObjectEndPoint = _relationEndPointManager.RelationEndPoints[virtualObjectEndPointID];
      Assert.That (virtualObjectEndPoint, Is.Not.Null);

      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var collectionEndPoint = _relationEndPointManager.RelationEndPoints[collectionEndPointID];
      Assert.That (collectionEndPoint, Is.Not.Null);

      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<UnregisterEndPointsCommand> ());
      Assert.That (((UnregisterEndPointsCommand) command).RegistrationAgent, Is.SameAs (_relationEndPointManager.RegistrationAgent));
      Assert.That (((UnregisterEndPointsCommand) command).Map, Is.SameAs (_relationEndPointManager.RelationEndPoints));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.Member (realEndPoint));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.Member (virtualObjectEndPoint));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.Member (collectionEndPoint));
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_IgnoresNonRegisteredEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<UnregisterEndPointsCommand> ());
      Assert.That (((UnregisterEndPointsCommand) command).RegistrationAgent, Is.SameAs (_relationEndPointManager.RegistrationAgent));
      Assert.That (((UnregisterEndPointsCommand) command).Map, Is.SameAs (_relationEndPointManager.RelationEndPoints));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Is.Empty);
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_WithUnidirectionalEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (endPointID);
      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);
      var unidirectionalEndPoint = (RealObjectEndPoint) _relationEndPointManager.RelationEndPoints[endPointID];
      Assert.That (unidirectionalEndPoint, Is.Not.Null);

      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<UnregisterEndPointsCommand> ());
      Assert.That (((UnregisterEndPointsCommand) command).RegistrationAgent, Is.SameAs (_relationEndPointManager.RegistrationAgent));
      Assert.That (((UnregisterEndPointsCommand) command).Map, Is.SameAs (_relationEndPointManager.RelationEndPoints));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.Member (unidirectionalEndPoint));
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_WithUnregisterableEndPoint ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      var endPoint = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPoint.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "Customer"));
      endPoint.Stub (stub => stub.Definition).Return (endPoint.ID.Definition);
      endPoint.Stub (stub => stub.HasChanged).Return (true);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPoint);

      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<ExceptionCommand> ());
      Assert.That (((ExceptionCommand) command).Exception, Is.TypeOf<InvalidOperationException> ());
      Assert.That (((ExceptionCommand) command).Exception.Message, Is.EqualTo (
          "The relations of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded.\r\n"
          + "Relation end-point "
          + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' has changed. "
          + "Only unchanged relation end-points can be unregistered."));
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_WithUnregisterableEndPoint_DueToChangedOpposite ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      var endPoint = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPoint.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "Customer"));
      endPoint.Stub (stub => stub.Definition).Return (endPoint.ID.Definition);
      endPoint.Stub (stub => stub.HasChanged).Return (false);
      endPoint.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Customer1);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPoint);

      var oppositeEndPoint = MockRepository.GenerateStub<IVirtualEndPoint> ();
      oppositeEndPoint.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders"));
      oppositeEndPoint.Stub (stub => stub.Definition).Return (oppositeEndPoint.ID.Definition);
      oppositeEndPoint.Stub (stub => stub.HasChanged).Return (true);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, oppositeEndPoint);

      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<ExceptionCommand> ());
      Assert.That (((ExceptionCommand) command).Exception, Is.TypeOf<InvalidOperationException> ());
      Assert.That (((ExceptionCommand) command).Exception.Message, Is.EqualTo (
          "The relations of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded.\r\n"
          + "The opposite relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of relation end-point "
          + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' has changed. "
          + "Non-virtual end-points that are part of changed relations cannot be unloaded."));
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_WithMultipleUnregisterableEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var endPoint1 = MockRepository.GenerateStub<IVirtualObjectEndPoint> ();
      endPoint1.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "OrderTicket"));
      endPoint1.Stub (stub => stub.Definition).Return (endPoint1.ID.Definition);
      endPoint1.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.OrderTicket1);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPoint1);

      var endPoint2 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPoint2.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "Customer"));
      endPoint2.Stub (stub => stub.Definition).Return (endPoint2.ID.Definition);
      endPoint2.Stub (stub => stub.OriginalOppositeObjectID).Return (DomainObjectIDs.Customer1);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPoint2);

      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<ExceptionCommand> ());
      Assert.That (
          ((ExceptionCommand) command).Exception.Message,
          Is.EqualTo (
              "The relations of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded.\r\n"
              + "Relation end-point "
              + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' "
              + "would leave a dangling reference.\r\n"
              + "Relation end-point "
              + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' "
              + "would leave a dangling reference."));

    }

    [Test]
    public void CreateUnloadVirtualEndPointsCommand ()
    {
      var endPointID1 = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointID2 = RelationEndPointID.Create (DomainObjectIDs.Order3, typeof (Order), "OrderItems");

      var endPointStub1 = MockRepository.GenerateStub<IVirtualEndPoint> ();
      endPointStub1.Stub (stub => stub.ID).Return (endPointID1);
      endPointStub1.Stub (stub => stub.CanBeMarkedIncomplete).Return (true);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub1);

      var endPointStub2 = MockRepository.GenerateStub<IVirtualEndPoint> ();
      endPointStub2.Stub (stub => stub.ID).Return (endPointID2);
      endPointStub2.Stub (stub => stub.CanBeMarkedIncomplete).Return (true);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub2);

      var result = _relationEndPointManager.CreateUnloadVirtualEndPointsCommand (new[] { endPointID1, endPointID2 });

      Assert.That (
          result,
          Is.TypeOf<UnloadVirtualEndPointsCommand>()
              .With.Property ("VirtualEndPoints").EqualTo (new[] { endPointStub1, endPointStub2 })
              .And.Property ("RelationEndPointMap").SameAs (_relationEndPointManager.RelationEndPoints)
              .And.Property ("RegistrationAgent").SameAs (_relationEndPointManager.RegistrationAgent));
    }

    [Test]
    public void CreateUnloadVirtualEndPointsCommand_NonLoadedEndPoints ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var result = _relationEndPointManager.CreateUnloadVirtualEndPointsCommand (new[] { endPointID });

      Assert.That (result, Is.TypeOf<NopCommand>());
    }

    [Test]
    public void CreateUnloadVirtualEndPointsCommand_SomeNonLoadedEndPoints ()
    {
      var endPointID1 = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointID2 = RelationEndPointID.Create (DomainObjectIDs.Order3, typeof (Order), "OrderItems");

      var endPointStub2 = MockRepository.GenerateStub<IVirtualEndPoint> ();
      endPointStub2.Stub (stub => stub.ID).Return (endPointID2);
      endPointStub2.Stub (stub => stub.CanBeMarkedIncomplete).Return (true);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub2);

      var result = _relationEndPointManager.CreateUnloadVirtualEndPointsCommand (new[] { endPointID1, endPointID2 });

      Assert.That (result, Is.TypeOf<UnloadVirtualEndPointsCommand> ().With.Property ("VirtualEndPoints").EqualTo (new[] { endPointStub2 }));
    }

    [Test]
    public void CreateUnloadVirtualEndPointsCommand_NonUnloadableEndPoints ()
    {
      var endPointID1 = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointID2 = RelationEndPointID.Create (DomainObjectIDs.Order3, typeof (Order), "OrderItems");
      var endPointID3 = RelationEndPointID.Create (DomainObjectIDs.Order4, typeof (Order), "OrderItems");

      var endPointStub1 = MockRepository.GenerateStub<IVirtualEndPoint> ();
      endPointStub1.Stub (stub => stub.ID).Return (endPointID1);
      endPointStub1.Stub (stub => stub.CanBeMarkedIncomplete).Return (false);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub1);

      var endPointStub2 = MockRepository.GenerateStub<IVirtualEndPoint> ();
      endPointStub2.Stub (stub => stub.ID).Return (endPointID2);
      endPointStub2.Stub (stub => stub.CanBeMarkedIncomplete).Return (false);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub2);

      var endPointStub3 = MockRepository.GenerateStub<IVirtualEndPoint> ();
      endPointStub3.Stub (stub => stub.ID).Return (endPointID3);
      endPointStub3.Stub (stub => stub.CanBeMarkedIncomplete).Return (true);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub3);

      var result = _relationEndPointManager.CreateUnloadVirtualEndPointsCommand (new[] { endPointID1, endPointID2, endPointID3 });

      Assert.That (result, Is.TypeOf<CompositeCommand> ());
      var exceptionCommands = ((CompositeCommand) result).GetNestedCommands();
      Assert.That (exceptionCommands[0], Is.TypeOf<ExceptionCommand>());
      Assert.That (((ExceptionCommand) exceptionCommands[0]).Exception, Is.TypeOf<InvalidOperationException>().With.Message.EqualTo (
          "The end point with ID "
          + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' "
          + "has been changed. Changed end points cannot be unloaded."));
      Assert.That (exceptionCommands[1], Is.TypeOf<ExceptionCommand> ());
      Assert.That (((ExceptionCommand) exceptionCommands[1]).Exception, Is.TypeOf<InvalidOperationException> ().With.Message.EqualTo (
          "The end point with ID "
          + "'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' "
          + "has been changed. Changed end points cannot be unloaded."));
    }

    [Test]
    public void CreateUnloadVirtualEndPointsCommand_NonVirtualIDs ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, typeof (OrderItem), "Order");

      Assert.That (
          () => _relationEndPointManager.CreateUnloadVirtualEndPointsCommand (new[] { endPointID }),
          Throws.ArgumentException.With.Message.EqualTo (
              "The given end point ID "
              + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' "
              + "does not denote a virtual end-point.\r\nParameter name: endPointIDs"));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_NullRealObjectEndPoint ()
    {
      var endPointDefinition =
          Configuration.GetTypeDefinition (typeof (OrderTicket)).GetRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      var relationEndPointID = RelationEndPointID.Create (null, endPointDefinition);

      var result = _relationEndPointManager.GetRelationEndPointWithoutLoading (relationEndPointID);

      Assert.That (result, Is.TypeOf (typeof (NullRealObjectEndPoint)));
      Assert.That (result.Definition, Is.EqualTo (endPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_NullVirtualObjectEndPoint ()
    {
      var endPointDefinition =
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      var relationEndPointID = RelationEndPointID.Create (null, endPointDefinition);

      var result = _relationEndPointManager.GetRelationEndPointWithoutLoading (relationEndPointID);

      Assert.That (result, Is.TypeOf (typeof (NullVirtualObjectEndPoint)));
      Assert.That (result.Definition, Is.EqualTo (endPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_NullCollectionEndPoint ()
    {
      var endPointDefinition =
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      var relationEndPointID = RelationEndPointID.Create (null, endPointDefinition);

      var result = _relationEndPointManager.GetRelationEndPointWithoutLoading (relationEndPointID);

      Assert.That (result, Is.TypeOf (typeof (NullCollectionEndPoint)));
      Assert.That (result.Definition, Is.EqualTo (endPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_EndPointRegistered ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);

      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub);

      var result = _relationEndPointManager.GetRelationEndPointWithoutLoading (endPointID);

      Assert.That (result, Is.SameAs (endPointStub));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_EndPointNotRegistered ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);

      var result = _relationEndPointManager.GetRelationEndPointWithoutLoading (endPointID);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_NullObjectEndPoint ()
    {
      var endPointDefinition =
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      var relationEndPointID = RelationEndPointID.Create (null, endPointDefinition);

      var result = _relationEndPointManager.GetRelationEndPointWithLazyLoad (relationEndPointID);

      Assert.That (result, Is.TypeOf (typeof (NullVirtualObjectEndPoint)));
      Assert.That (result.Definition, Is.EqualTo (endPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_NullCollectionEndPoint ()
    {
      var endPointDefinition =
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      var relationEndPointID = RelationEndPointID.Create (null, endPointDefinition);

      var result = _relationEndPointManager.GetRelationEndPointWithLazyLoad (relationEndPointID);

      Assert.That (result, Is.TypeOf (typeof (NullCollectionEndPoint)));
      Assert.That (result.Definition, Is.EqualTo (endPointDefinition));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "GetRelationEndPointWithLazyLoad cannot be called for anonymous end points.\r\nParameter name: endPointID")]
    public void GetRelationEndPointWithLazyLoad_DoesNotSupportAnonymousEndPoints ()
    {
      var client = DomainObjectIDs.Client2.GetObject<Client> ();
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client) + ".ParentClient");
      IRelationEndPoint unidirectionalEndPoint =
          _relationEndPointManager.GetRelationEndPointWithLazyLoad (RelationEndPointID.Create (client.ID, parentClientEndPointDefinition));

      Client parentClient = client.ParentClient;
      Assert.That (parentClient, Is.Not.Null);

      var anonymousEndPointDefinition = unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition ();
      _relationEndPointManager.GetRelationEndPointWithLazyLoad (RelationEndPointID.Create (parentClient.ID, anonymousEndPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_EndPointAlreadyAvailable ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var endPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Expect (mock => mock.EnsureDataComplete ());
      endPointMock.Replay ();

      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointMock);

      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.SameAs (endPointMock));

      var result = _relationEndPointManager.GetRelationEndPointWithLazyLoad (endPointID);

      Assert.That (result, Is.SameAs (endPointMock));
      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.SameAs (endPointMock));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_RegistersCollectionEndPoint_ButDoesNotLoadItsContents ()
    {
      _relationEndPointManager.ClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1); // preload Order1 before lazily loading its virtual end point

      var orderItemsEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (_relationEndPointManager.RelationEndPoints[orderItemsEndPointID], Is.Null);

      var endPoint = _relationEndPointManager.GetRelationEndPointWithLazyLoad (orderItemsEndPointID);

      Assert.That (endPoint, Is.Not.Null);
      Assert.That (endPoint.IsDataComplete, Is.False);
      Assert.That (_relationEndPointManager.RelationEndPoints[orderItemsEndPointID], Is.SameAs (endPoint));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_RegistersVirtualObjectEndPoint_ButDoesNotLoadItsContents ()
    {
      _relationEndPointManager.ClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1); // preload Order1 before lazily loading its virtual end point

      var orderTicketEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (_relationEndPointManager.RelationEndPoints[orderTicketEndPointID], Is.Null);

      var endPoint = _relationEndPointManager.GetRelationEndPointWithLazyLoad (orderTicketEndPointID);

      Assert.That (endPoint, Is.Not.Null);
      Assert.That (endPoint.IsDataComplete, Is.False);
      Assert.That (_relationEndPointManager.RelationEndPoints[orderTicketEndPointID], Is.SameAs (endPoint));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_RegistersVirtualObjectEndPointWithNull ()
    {
      _relationEndPointManager.ClientTransaction.EnsureDataAvailable (DomainObjectIDs.Employee1); // preload Employee before lazily loading its virtual end point

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Employee1, "Computer");
      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.Null);

      var endPoint = _relationEndPointManager.GetRelationEndPointWithLazyLoad (endPointID);

      Assert.That (endPoint, Is.Not.Null);
      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.SameAs (endPoint));
      Assert.That (((IVirtualObjectEndPoint) endPoint).OppositeObjectID, Is.Null);
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_LoadsData_OfObjectsWithRealEndPointNotYetRegistered ()
    {
      var locationEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      Assert.That (locationEndPointID.Definition.IsVirtual, Is.False);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Location1], Is.Null);

      var result = _relationEndPointManager.GetRelationEndPointWithLazyLoad (locationEndPointID);
      Assert.That (result, Is.Not.Null);

      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Location1], Is.Not.Null);
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_DoesNotLoadData_OfObjectsWithVirtualEndPointNotYetRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (endPointID.Definition.IsVirtual, Is.True);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);

      var result = _relationEndPointManager.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (result, Is.Not.Null);

      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void GetOrCreateVirtualEndPoint_AlreadyAvailable ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var endPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Expect (mock => mock.EnsureDataComplete ());
      endPointMock.Replay ();

      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointMock);

      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.SameAs (endPointMock));

      var result = _relationEndPointManager.GetOrCreateVirtualEndPoint (endPointID);

      Assert.That (result, Is.SameAs (endPointMock));
      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.SameAs (endPointMock));
    }

    [Test]
    public void GetOrCreateVirtualEndPoint_NewlyCreated_ObjectEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.Null);

      var result = _relationEndPointManager.GetOrCreateVirtualEndPoint (endPointID);

      Assert.That (result, Is.Not.Null.And.AssignableTo<IVirtualObjectEndPoint>());
      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.SameAs (result));
      Assert.That (result.IsDataComplete, Is.False);
    }

    [Test]
    public void GetOrCreateVirtualEndPoint_NewlyCreated_CollectionEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.Null);

      var result = _relationEndPointManager.GetOrCreateVirtualEndPoint (endPointID);

      Assert.That (result, Is.Not.Null.And.AssignableTo<ICollectionEndPoint>());
      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.SameAs (result));
      Assert.That (result.IsDataComplete, Is.False);
    }

    [Test]
    public void GetOrCreateVirtualEndPoint_Null_ObjectEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (null, GetEndPointDefinition (typeof (Order), "OrderTicket"));
      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.Null);

      var result = _relationEndPointManager.GetOrCreateVirtualEndPoint (endPointID);

      Assert.That (result, Is.TypeOf<NullVirtualObjectEndPoint> ());
    }

    [Test]
    public void GetOrCreateVirtualEndPoint_Null_CollectionEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (null, GetEndPointDefinition (typeof (Order), "OrderItems"));
      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.Null);

      var result = _relationEndPointManager.GetOrCreateVirtualEndPoint (endPointID);

      Assert.That (result, Is.TypeOf<NullCollectionEndPoint> ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "GetOrCreateVirtualEndPoint cannot be called for anonymous end points.\r\nParameter name: endPointID")]
    public void GetOrCreateVirtualEndPoint_DoesNotSupportAnonymousEndPoints ()
    {
      var client = DomainObjectIDs.Client2.GetObject<Client> ();
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client) + ".ParentClient");
      IRelationEndPoint unidirectionalEndPoint =
          _relationEndPointManager.GetRelationEndPointWithLazyLoad (RelationEndPointID.Create (client.ID, parentClientEndPointDefinition));

      Client parentClient = client.ParentClient;
      Assert.That (parentClient, Is.Not.Null);

      var anonymousEndPointDefinition = unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition ();
      _relationEndPointManager.GetOrCreateVirtualEndPoint (RelationEndPointID.Create (parentClient.ID, anonymousEndPointDefinition));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "GetOrCreateVirtualEndPoint cannot be called for non-virtual end points.\r\nParameter name: endPointID")]
    public void GetOrCreateVirtualEndPoint_NonVirtualEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, typeof (OrderItem), "Order");
      _relationEndPointManager.GetOrCreateVirtualEndPoint (endPointID);
    }

    [Test]
    public void CommitAllEndPoints_CommitsEndPoints ()
    {
      RelationEndPointID endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Expect (mock => mock.Commit ());
      endPointMock.Replay ();

      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointMock);

      _relationEndPointManager.CommitAllEndPoints ();

      endPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void RollbackAllEndPoints_RollsbackEndPoints ()
    {
      RelationEndPointID endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Expect (mock => mock.Rollback ());
      endPointMock.Replay ();

      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointMock);

      _relationEndPointManager.RollbackAllEndPoints ();

      endPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void Reset_RemovesEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Expect (mock => mock.Rollback ());
      endPointMock.Replay ();
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointMock);
      Assert.That (_relationEndPointManager.RelationEndPoints, Is.Not.Empty.And.Member (endPointMock));

      _relationEndPointManager.Reset ();

      endPointMock.VerifyAllExpectations ();
      Assert.That (_relationEndPointManager.RelationEndPoints, Is.Empty);
    }

    [Test]
    public void Reset_RaisesUnregisteringEvents ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub);

      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock (_relationEndPointManager.ClientTransaction);

      _relationEndPointManager.Reset ();

      listenerMock.AssertWasCalled (mock => mock.RelationEndPointMapUnregistering (_relationEndPointManager.ClientTransaction, endPointID));
    }
  }
}