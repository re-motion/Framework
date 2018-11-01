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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class VirtualObjectEndPointDataManagerTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    
    private VirtualObjectEndPointDataManager _dataManager;

    private OrderTicket _oppositeObject;
    private IRealObjectEndPoint _oppositeEndPointStub;

    private OrderTicket _oppositeObject2;
    private IRealObjectEndPoint _oppositeEndPointStub2;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      _dataManager = new VirtualObjectEndPointDataManager (_endPointID);

      _oppositeObject = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      _oppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _oppositeEndPointStub.Stub (stub => stub.GetDomainObjectReference()).Return (_oppositeObject);

      _oppositeObject2 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket2);
      _oppositeEndPointStub2 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _oppositeEndPointStub2.Stub (stub => stub.GetDomainObjectReference()).Return (_oppositeObject2);
    }

    [Test]
    public void CurrentOppositeObject_Set ()
    {
      Assert.That (_dataManager.CurrentOppositeObject, Is.Null);

      _dataManager.CurrentOppositeObject = _oppositeObject;

      Assert.That (_dataManager.CurrentOppositeObject, Is.SameAs (_oppositeObject));
    }

    [Test]
    public void ContainsOriginalObjectID_False ()
    {
      Assert.That (_dataManager.ContainsOriginalObjectID (_oppositeObject.ID), Is.False);
    }

    [Test]
    public void ContainsOriginalObjectID_True ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataManager.ContainsOriginalObjectID (_oppositeObject.ID), Is.True);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataManager.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.OriginalOppositeObject, Is.SameAs (_oppositeObject));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_CurrentValueAlreadySet ()
    {
      _dataManager.CurrentOppositeObject = _oppositeObject2;
      _dataManager.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub2);

      _dataManager.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataManager.OriginalOppositeObject, Is.EqualTo (_oppositeObject));

      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub2));
      Assert.That (_dataManager.CurrentOppositeObject, Is.SameAs (_oppositeObject2));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_PreviouslyItemWithoutEndPoint ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
      
      _dataManager.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataManager.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.OriginalItemWithoutEndPoint, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "A different original opposite item has already been registered.")]
    public void RegisterOriginalOppositeEndPoint_PreviouslyOtherItemWithoutEndPoint ()
    {
      var oppositeObject2 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket2);
      _dataManager.RegisterOriginalItemWithoutEndPoint (oppositeObject2);

      _dataManager.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The original opposite end-point has already been registered.")]
    public void RegisterOriginalOppositeEndPoint_AlreadyRegistered ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
      _dataManager.RegisterOriginalOppositeEndPoint (MockRepository.GenerateStub<IRealObjectEndPoint>());
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.Not.Null);

      _dataManager.UnregisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.Null);
      Assert.That (_dataManager.CurrentOppositeObject, Is.Null);
      Assert.That (_dataManager.OriginalOppositeObject, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The original opposite end-point has not been registered.")]
    public void UnregisterOriginalOppositeEndPoint_NotRegistered ()
    {
      _dataManager.UnregisterOriginalOppositeEndPoint (_oppositeEndPointStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The original opposite end-point has not been registered.")]
    public void UnregisterOriginalOppositeEndPoint_DifferentRegistered ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
      _dataManager.UnregisterOriginalOppositeEndPoint (MockRepository.GenerateStub<IRealObjectEndPoint>());
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint ()
    {
      Assert.That (_dataManager.OriginalOppositeObject, Is.Not.SameAs (_oppositeObject));
      Assert.That (_dataManager.CurrentOppositeObject, Is.Not.SameAs (_oppositeObject));
      Assert.That (_dataManager.OriginalItemWithoutEndPoint, Is.Null);
      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.Null);

      _dataManager.RegisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataManager.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.OriginalItemWithoutEndPoint, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.Null);
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint_CurrentValueAlreadySet ()
    {
      _dataManager.CurrentOppositeObject = _oppositeObject2;
      _dataManager.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub2);

      _dataManager.RegisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataManager.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataManager.OriginalItemWithoutEndPoint, Is.SameAs (_oppositeObject));

      Assert.That (_dataManager.CurrentOppositeObject, Is.SameAs (_oppositeObject2));
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub2));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An original opposite item has already been registered.")]
    public void RegisterOriginalItemWithoutEndPoint_WithOriginalOppositeObjectID ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
      _dataManager.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An original opposite item has already been registered.")]
    public void RegisterOriginalItemWithoutEndPoint_WithOriginalOppositeEndPoint ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      _dataManager.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
      Assert.That (_dataManager.OriginalOppositeObject, Is.Not.Null);
      Assert.That (_dataManager.OriginalItemWithoutEndPoint, Is.Not.Null);
      Assert.That (_dataManager.CurrentOppositeObject, Is.Not.Null);

      _dataManager.UnregisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataManager.OriginalOppositeObject, Is.Null);
      Assert.That (_dataManager.OriginalItemWithoutEndPoint, Is.Null);
      Assert.That (_dataManager.CurrentOppositeObject, Is.Null);
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint_CurrentValueAlreadySet ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
      _dataManager.CurrentOppositeObject = _oppositeObject2;

      _dataManager.UnregisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataManager.OriginalOppositeObject, Is.Null);
      Assert.That (_dataManager.OriginalItemWithoutEndPoint, Is.Null);
      Assert.That (_dataManager.CurrentOppositeObject, Is.SameAs (_oppositeObject2));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot unregister original item, it has not been registered.")]
    public void UnregisterOriginalItemWithoutEndPoint_InvalidID ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint (_oppositeObject);

      _dataManager.UnregisterOriginalItemWithoutEndPoint (DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket2));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot unregister original item, an end-point has been registered for it.")]
    public void UnregisterOriginalItemWithoutEndPoint_EndPointExists ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      _dataManager.UnregisterOriginalItemWithoutEndPoint (_oppositeObject);
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _dataManager.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An opposite end-point has already been registered.")]
    public void RegisterCurrentOppositeEndPoint_AlreadyRegistered ()
    {
      _dataManager.RegisterCurrentOppositeEndPoint (MockRepository.GenerateStub<IRealObjectEndPoint>());
      _dataManager.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _dataManager.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.Not.Null);

      _dataManager.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has not been registered.")]
    public void UnregisterCurrentOppositeEndPoint_NotRegistered ()
    {
      _dataManager.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has not been registered.")]
    public void UnregisterCurrentOppositeEndPoint_DifferentRegistered ()
    {
      _dataManager.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);
      _dataManager.UnregisterCurrentOppositeEndPoint (MockRepository.GenerateStub<IRealObjectEndPoint>());
    }

    [Test]
    public void HasDataChanged ()
    {
      Assert.That (_dataManager.HasDataChanged(), Is.False);

      _dataManager.CurrentOppositeObject = _oppositeObject;

      Assert.That (_dataManager.HasDataChanged(), Is.True);

      _dataManager.CurrentOppositeObject = null;

      Assert.That (_dataManager.HasDataChanged(), Is.False);
    }

    [Test]
    public void Commit ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
      Assert.That (_dataManager.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));

      _dataManager.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);
      _dataManager.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub2);
      _dataManager.CurrentOppositeObject = _oppositeObject2;

      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.Not.SameAs (_oppositeEndPointStub2));
      Assert.That (_dataManager.OriginalOppositeObject, Is.Not.SameAs (_oppositeObject2));

      _dataManager.Commit();

      Assert.That (_dataManager.CurrentOppositeObject, Is.EqualTo (_oppositeObject2));
      Assert.That (_dataManager.OriginalOppositeObject, Is.EqualTo (_oppositeObject2));
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub2));
      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub2));
    }

    [Test]
    public void Commit_ClearsItemWithoutEndPoint ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint (DomainObjectMother.CreateFakeObject<OrderTicket>());
      _dataManager.CurrentOppositeObject = _oppositeObject;
      _dataManager.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      _dataManager.Commit ();

      Assert.That (_dataManager.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataManager.OriginalItemWithoutEndPoint, Is.Null);
    }

    [Test]
    public void Commit_SetsItemWithoutEndPoint ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint (DomainObjectMother.CreateFakeObject<OrderTicket> ());
      _dataManager.CurrentOppositeObject = _oppositeObject;

      _dataManager.Commit ();

      Assert.That (_dataManager.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.Null);
      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataManager.OriginalItemWithoutEndPoint, Is.SameAs (_oppositeObject));
    }

    [Test]
    public void Rollback ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
      Assert.That (_dataManager.CurrentOppositeObject, Is.EqualTo (_oppositeObject));
      Assert.That (_dataManager.OriginalOppositeObject, Is.EqualTo (_oppositeObject));
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));

      _dataManager.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);
      _dataManager.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub2);
      _dataManager.CurrentOppositeObject = _oppositeObject2;
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.Not.SameAs (_oppositeEndPointStub));
      Assert.That (_dataManager.CurrentOppositeObject, Is.Not.SameAs (_oppositeObject));

      _dataManager.Rollback();

      Assert.That (_dataManager.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataManager.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      var sourceOppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
      sourceOppositeEndPointStub.Stub (stub => stub.ID).Return (_oppositeEndPointStub.ID);

      var sourceDataManager = new VirtualObjectEndPointDataManager (_endPointID);
      sourceDataManager.CurrentOppositeObject = _oppositeObject;
      sourceDataManager.RegisterCurrentOppositeEndPoint (sourceOppositeEndPointStub);

      var endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();
      endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (sourceOppositeEndPointStub.ID))
          .Return (_oppositeEndPointStub);

      _dataManager.SetDataFromSubTransaction (sourceDataManager, endPointProviderStub);

      Assert.That (_dataManager.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
    }

    [Test]
    public void SetDataFromSubTransaction_Null ()
    {
      _dataManager.CurrentOppositeObject = _oppositeObject;
      _dataManager.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      var sourceDataManager = new VirtualObjectEndPointDataManager (_endPointID);
      Assert.That (sourceDataManager.CurrentOppositeObject, Is.Null);
      Assert.That (sourceDataManager.CurrentOppositeEndPoint, Is.Null);
      var endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();

      _dataManager.SetDataFromSubTransaction (sourceDataManager, endPointProviderStub);

      Assert.That (_dataManager.CurrentOppositeObject, Is.Null);
      Assert.That (_dataManager.CurrentOppositeEndPoint, Is.Null);
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var data = new VirtualObjectEndPointDataManager (_endPointID);

      var endPointFake = new SerializableRealObjectEndPointFake (null, DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1));
      data.RegisterOriginalOppositeEndPoint (endPointFake);

      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (data);

      Assert.That (deserializedInstance.EndPointID, Is.Not.Null);
      Assert.That (deserializedInstance.OriginalOppositeEndPoint, Is.Not.Null);
      Assert.That (deserializedInstance.OriginalOppositeObject, Is.Not.Null);
      Assert.That (deserializedInstance.CurrentOppositeEndPoint, Is.Not.Null);
      Assert.That (deserializedInstance.CurrentOppositeObject, Is.Not.Null);
    }
  }
}