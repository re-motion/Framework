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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class VirtualObjectEndPointDataManagerTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;

    private VirtualObjectEndPointDataManager _dataManager;

    private OrderTicket _oppositeObject;
    private Mock<IRealObjectEndPoint> _oppositeEndPointStub;

    private OrderTicket _oppositeObject2;
    private Mock<IRealObjectEndPoint> _oppositeEndPointStub2;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderTicket");

      _dataManager = new VirtualObjectEndPointDataManager(_endPointID);

      _oppositeObject = DomainObjectMother.CreateFakeObject<OrderTicket>(DomainObjectIDs.OrderTicket1);
      _oppositeEndPointStub = new Mock<IRealObjectEndPoint>();
      _oppositeEndPointStub.Setup(stub => stub.GetDomainObjectReference()).Returns(_oppositeObject);

      _oppositeObject2 = DomainObjectMother.CreateFakeObject<OrderTicket>(DomainObjectIDs.OrderTicket2);
      _oppositeEndPointStub2 = new Mock<IRealObjectEndPoint>();
      _oppositeEndPointStub2.Setup(stub => stub.GetDomainObjectReference()).Returns(_oppositeObject2);
    }

    [Test]
    public void CurrentOppositeObject_Set ()
    {
      Assert.That(_dataManager.CurrentOppositeObject, Is.Null);

      _dataManager.CurrentOppositeObject = _oppositeObject;

      Assert.That(_dataManager.CurrentOppositeObject, Is.SameAs(_oppositeObject));
    }

    [Test]
    public void ContainsOriginalObjectID_False ()
    {
      Assert.That(_dataManager.ContainsOriginalObjectID(_oppositeObject.ID), Is.False);
    }

    [Test]
    public void ContainsOriginalObjectID_True ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint(_oppositeObject);

      Assert.That(_dataManager.ContainsOriginalObjectID(_oppositeObject.ID), Is.True);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);

      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
      Assert.That(_dataManager.CurrentOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.OriginalOppositeObject, Is.SameAs(_oppositeObject));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_CurrentValueAlreadySet ()
    {
      _dataManager.CurrentOppositeObject = _oppositeObject2;
      _dataManager.RegisterCurrentOppositeEndPoint(_oppositeEndPointStub2.Object);

      _dataManager.RegisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);

      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
      Assert.That(_dataManager.OriginalOppositeObject, Is.EqualTo(_oppositeObject));

      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.SameAs(_oppositeEndPointStub2.Object));
      Assert.That(_dataManager.CurrentOppositeObject, Is.SameAs(_oppositeObject2));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_PreviouslyItemWithoutEndPoint ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint(_oppositeObject);

      _dataManager.RegisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);

      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
      Assert.That(_dataManager.CurrentOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.OriginalOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.OriginalItemWithoutEndPoint, Is.Null);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_PreviouslyOtherItemWithoutEndPoint ()
    {
      var oppositeObject2 = DomainObjectMother.CreateFakeObject<OrderTicket>(DomainObjectIDs.OrderTicket2);
      _dataManager.RegisterOriginalItemWithoutEndPoint(oppositeObject2);
      Assert.That(
          () => _dataManager.RegisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo("A different original opposite item has already been registered."));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_AlreadyRegistered ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);
      Assert.That(
          () => _dataManager.RegisterOriginalOppositeEndPoint(new Mock<IRealObjectEndPoint>().Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The original opposite end-point has already been registered."));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);
      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.Not.Null);

      _dataManager.UnregisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);

      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.Null);
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.Null);
      Assert.That(_dataManager.CurrentOppositeObject, Is.Null);
      Assert.That(_dataManager.OriginalOppositeObject, Is.Null);
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_NotRegistered ()
    {
      Assert.That(
          () => _dataManager.UnregisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The original opposite end-point has not been registered."));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_DifferentRegistered ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);
      Assert.That(
          () => _dataManager.UnregisterOriginalOppositeEndPoint(new Mock<IRealObjectEndPoint>().Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The original opposite end-point has not been registered."));
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint ()
    {
      Assert.That(_dataManager.OriginalOppositeObject, Is.Not.SameAs(_oppositeObject));
      Assert.That(_dataManager.CurrentOppositeObject, Is.Not.SameAs(_oppositeObject));
      Assert.That(_dataManager.OriginalItemWithoutEndPoint, Is.Null);
      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.Null);
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.Null);

      _dataManager.RegisterOriginalItemWithoutEndPoint(_oppositeObject);

      Assert.That(_dataManager.OriginalOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.OriginalItemWithoutEndPoint, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.CurrentOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.Null);
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.Null);
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint_CurrentValueAlreadySet ()
    {
      _dataManager.CurrentOppositeObject = _oppositeObject2;
      _dataManager.RegisterCurrentOppositeEndPoint(_oppositeEndPointStub2.Object);

      _dataManager.RegisterOriginalItemWithoutEndPoint(_oppositeObject);

      Assert.That(_dataManager.OriginalOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.Null);
      Assert.That(_dataManager.OriginalItemWithoutEndPoint, Is.SameAs(_oppositeObject));

      Assert.That(_dataManager.CurrentOppositeObject, Is.SameAs(_oppositeObject2));
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.SameAs(_oppositeEndPointStub2.Object));
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint_WithOriginalOppositeObjectID ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint(_oppositeObject);
      Assert.That(
          () => _dataManager.RegisterOriginalItemWithoutEndPoint(_oppositeObject),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "An original opposite item has already been registered."));
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint_WithOriginalOppositeEndPoint ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);
      Assert.That(
          () => _dataManager.RegisterOriginalItemWithoutEndPoint(_oppositeObject),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "An original opposite item has already been registered."));
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint(_oppositeObject);
      Assert.That(_dataManager.OriginalOppositeObject, Is.Not.Null);
      Assert.That(_dataManager.OriginalItemWithoutEndPoint, Is.Not.Null);
      Assert.That(_dataManager.CurrentOppositeObject, Is.Not.Null);

      _dataManager.UnregisterOriginalItemWithoutEndPoint(_oppositeObject);

      Assert.That(_dataManager.OriginalOppositeObject, Is.Null);
      Assert.That(_dataManager.OriginalItemWithoutEndPoint, Is.Null);
      Assert.That(_dataManager.CurrentOppositeObject, Is.Null);
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint_CurrentValueAlreadySet ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint(_oppositeObject);
      _dataManager.CurrentOppositeObject = _oppositeObject2;

      _dataManager.UnregisterOriginalItemWithoutEndPoint(_oppositeObject);

      Assert.That(_dataManager.OriginalOppositeObject, Is.Null);
      Assert.That(_dataManager.OriginalItemWithoutEndPoint, Is.Null);
      Assert.That(_dataManager.CurrentOppositeObject, Is.SameAs(_oppositeObject2));
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint_InvalidID ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint(_oppositeObject);
      Assert.That(
          () => _dataManager.UnregisterOriginalItemWithoutEndPoint(DomainObjectMother.CreateFakeObject<OrderTicket>(DomainObjectIDs.OrderTicket2)),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot unregister original item, it has not been registered."));
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint_EndPointExists ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);
      Assert.That(
          () => _dataManager.UnregisterOriginalItemWithoutEndPoint(_oppositeObject),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot unregister original item, an end-point has been registered for it."));
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _dataManager.RegisterCurrentOppositeEndPoint(_oppositeEndPointStub.Object);

      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint_AlreadyRegistered ()
    {
      _dataManager.RegisterCurrentOppositeEndPoint(new Mock<IRealObjectEndPoint>().Object);
      Assert.That(
          () => _dataManager.RegisterCurrentOppositeEndPoint(_oppositeEndPointStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "An opposite end-point has already been registered."));
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _dataManager.RegisterCurrentOppositeEndPoint(_oppositeEndPointStub.Object);
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.Not.Null);

      _dataManager.UnregisterCurrentOppositeEndPoint(_oppositeEndPointStub.Object);

      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.Null);
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint_NotRegistered ()
    {
      Assert.That(
          () => _dataManager.UnregisterCurrentOppositeEndPoint(_oppositeEndPointStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The opposite end-point has not been registered."));
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint_DifferentRegistered ()
    {
      _dataManager.RegisterCurrentOppositeEndPoint(_oppositeEndPointStub.Object);
      Assert.That(
          () => _dataManager.UnregisterCurrentOppositeEndPoint(new Mock<IRealObjectEndPoint>().Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The opposite end-point has not been registered."));
    }

    [Test]
    public void HasDataChanged ()
    {
      Assert.That(_dataManager.HasDataChanged(), Is.False);

      _dataManager.CurrentOppositeObject = _oppositeObject;

      Assert.That(_dataManager.HasDataChanged(), Is.True);

      _dataManager.CurrentOppositeObject = null;

      Assert.That(_dataManager.HasDataChanged(), Is.False);
    }

    [Test]
    public void Commit ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);
      Assert.That(_dataManager.CurrentOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.OriginalOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));

      _dataManager.UnregisterCurrentOppositeEndPoint(_oppositeEndPointStub.Object);
      _dataManager.RegisterCurrentOppositeEndPoint(_oppositeEndPointStub2.Object);
      _dataManager.CurrentOppositeObject = _oppositeObject2;

      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.Not.SameAs(_oppositeEndPointStub2.Object));
      Assert.That(_dataManager.OriginalOppositeObject, Is.Not.SameAs(_oppositeObject2));

      _dataManager.Commit();

      Assert.That(_dataManager.CurrentOppositeObject, Is.EqualTo(_oppositeObject2));
      Assert.That(_dataManager.OriginalOppositeObject, Is.EqualTo(_oppositeObject2));
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.SameAs(_oppositeEndPointStub2.Object));
      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.SameAs(_oppositeEndPointStub2.Object));
    }

    [Test]
    public void Commit_ClearsItemWithoutEndPoint ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint(DomainObjectMother.CreateFakeObject<OrderTicket>());
      _dataManager.CurrentOppositeObject = _oppositeObject;
      _dataManager.RegisterCurrentOppositeEndPoint(_oppositeEndPointStub.Object);

      _dataManager.Commit();

      Assert.That(_dataManager.CurrentOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.OriginalOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
      Assert.That(_dataManager.OriginalItemWithoutEndPoint, Is.Null);
    }

    [Test]
    public void Commit_SetsItemWithoutEndPoint ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint(DomainObjectMother.CreateFakeObject<OrderTicket>());
      _dataManager.CurrentOppositeObject = _oppositeObject;

      _dataManager.Commit();

      Assert.That(_dataManager.CurrentOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.OriginalOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.Null);
      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.Null);
      Assert.That(_dataManager.OriginalItemWithoutEndPoint, Is.SameAs(_oppositeObject));
    }

    [Test]
    public void Rollback ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(_oppositeEndPointStub.Object);
      Assert.That(_dataManager.CurrentOppositeObject, Is.EqualTo(_oppositeObject));
      Assert.That(_dataManager.OriginalOppositeObject, Is.EqualTo(_oppositeObject));
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));

      _dataManager.UnregisterCurrentOppositeEndPoint(_oppositeEndPointStub.Object);
      _dataManager.RegisterCurrentOppositeEndPoint(_oppositeEndPointStub2.Object);
      _dataManager.CurrentOppositeObject = _oppositeObject2;
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.Not.SameAs(_oppositeEndPointStub.Object));
      Assert.That(_dataManager.CurrentOppositeObject, Is.Not.SameAs(_oppositeObject));

      _dataManager.Rollback();

      Assert.That(_dataManager.CurrentOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.OriginalOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
      Assert.That(_dataManager.OriginalOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      var sourceOppositeEndPointStub = new Mock<IRealObjectEndPoint>();
      sourceOppositeEndPointStub.Setup(stub => stub.ID).Returns(_oppositeEndPointStub.Object.ID);

      var sourceDataManager = new VirtualObjectEndPointDataManager(_endPointID);
      sourceDataManager.CurrentOppositeObject = _oppositeObject;
      sourceDataManager.RegisterCurrentOppositeEndPoint(sourceOppositeEndPointStub.Object);

      var endPointProviderStub = new Mock<IRelationEndPointProvider>();
      endPointProviderStub
          .Setup(stub => stub.GetRelationEndPointWithoutLoading(sourceOppositeEndPointStub.Object.ID))
          .Returns(_oppositeEndPointStub.Object);

      _dataManager.SetDataFromSubTransaction(sourceDataManager, endPointProviderStub.Object);

      Assert.That(_dataManager.CurrentOppositeObject, Is.SameAs(_oppositeObject));
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.SameAs(_oppositeEndPointStub.Object));
    }

    [Test]
    public void SetDataFromSubTransaction_Null ()
    {
      _dataManager.CurrentOppositeObject = _oppositeObject;
      _dataManager.RegisterCurrentOppositeEndPoint(_oppositeEndPointStub.Object);

      var sourceDataManager = new VirtualObjectEndPointDataManager(_endPointID);
      Assert.That(sourceDataManager.CurrentOppositeObject, Is.Null);
      Assert.That(sourceDataManager.CurrentOppositeEndPoint, Is.Null);
      var endPointProviderStub = new Mock<IRelationEndPointProvider>();

      _dataManager.SetDataFromSubTransaction(sourceDataManager, endPointProviderStub.Object);

      Assert.That(_dataManager.CurrentOppositeObject, Is.Null);
      Assert.That(_dataManager.CurrentOppositeEndPoint, Is.Null);
    }
  }
}
