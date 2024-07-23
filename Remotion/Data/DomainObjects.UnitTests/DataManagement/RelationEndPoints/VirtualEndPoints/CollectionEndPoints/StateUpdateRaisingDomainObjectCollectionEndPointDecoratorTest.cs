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
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.UnitTesting;
using Remotion.Development.Moq.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class StateUpdateRaisingDomainObjectCollectionEndPointDecoratorTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private Mock<IVirtualEndPointStateUpdateListener> _listenerMock;
    private Mock<IDomainObjectCollectionEndPoint> _innerEndPointMock;

    private StateUpdateRaisingDomainObjectCollectionEndPointDecorator _decorator;
    private DecoratorTestHelper<IDomainObjectCollectionEndPoint> _decoratorTestHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      _listenerMock = new Mock<IVirtualEndPointStateUpdateListener>(MockBehavior.Strict);
      _innerEndPointMock = new Mock<IDomainObjectCollectionEndPoint>(MockBehavior.Strict);
      _innerEndPointMock.Setup(stub => stub.HasChangedFast).Returns(false);
      _innerEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);

      _decorator = new StateUpdateRaisingDomainObjectCollectionEndPointDecorator(_innerEndPointMock.Object, _listenerMock.Object);
      _decoratorTestHelper = new DecoratorTestHelper<IDomainObjectCollectionEndPoint>(_decorator, _innerEndPointMock);
    }

    [Test]
    public void SetDataFromSubTransaction_UnwrapsSourceEndPoint ()
    {
      var sourceInnerEndPoint = new Mock<IDomainObjectCollectionEndPoint>();
      var sourceEndPoint = new StateUpdateRaisingDomainObjectCollectionEndPointDecorator(sourceInnerEndPoint.Object, _listenerMock.Object);

      _listenerMock.Setup(mock => mock.VirtualEndPointStateUpdated(_endPointID, null)).Verifiable();

      _innerEndPointMock.Reset();
      _innerEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
      _innerEndPointMock.Setup(stub => stub.HasChangedFast).Returns((bool?)null);
      _innerEndPointMock
          .Setup(ep => ep.SetDataFromSubTransaction(sourceInnerEndPoint.Object))
          .Callback(
              (IRelationEndPoint _) => _listenerMock.Verify(
                  mock => mock.VirtualEndPointStateUpdated(It.IsAny<RelationEndPointID>(), It.IsAny<bool?>()),
                  Times.Never()))
          .Verifiable();

      _decorator.SetDataFromSubTransaction(sourceEndPoint);

      _innerEndPointMock.Verify();
      _listenerMock.Verify(mock => mock.VirtualEndPointStateUpdated(_endPointID, null), Times.AtLeastOnce());
    }

    [Test]
    public void SetDataFromSubTransaction_WithException ()
    {
      var sourceInnerEndPoint = new Mock<IDomainObjectCollectionEndPoint>();
      var sourceEndPoint = new StateUpdateRaisingDomainObjectCollectionEndPointDecorator(sourceInnerEndPoint.Object, _listenerMock.Object);

      _listenerMock.Setup(mock => mock.VirtualEndPointStateUpdated(_endPointID, null)).Verifiable();

      var exception = new Exception();
      _innerEndPointMock.Reset();
      _innerEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
      _innerEndPointMock.Setup(stub => stub.HasChangedFast).Returns((bool?)null);
      _innerEndPointMock
          .Setup(ep => ep.SetDataFromSubTransaction(sourceInnerEndPoint.Object))
          .Throws(exception)
          .Verifiable();

      Assert.That(() => _decorator.SetDataFromSubTransaction(sourceEndPoint), Throws.Exception.SameAs(exception));

      _innerEndPointMock.Verify();
      _listenerMock.Verify();
    }

    [Test]
    public void Synchronize ()
    {
      CheckDelegationWithStateUpdate(ep => ep.Synchronize());
    }

    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      var endPoint = new Mock<IRealObjectEndPoint>();
      CheckDelegationWithStateUpdate(ep => ep.SynchronizeOppositeEndPoint(endPoint.Object));
    }

    [Test]
    public void Commit ()
    {
      CheckDelegationWithStateUpdate(ep => ep.Commit());
    }

    [Test]
    public void Rollback ()
    {
      CheckDelegationWithStateUpdate(ep => ep.Rollback());
    }

    [Test]
    public void SortCurrentData ()
    {
      Comparison<DomainObject> expectedComparison = (one, two) => 0;
      CheckDelegationWithStateUpdate(ep => ep.SortCurrentData(expectedComparison));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var orderItem = DomainObjectMother.CreateFakeObject<OrderItem>();
      CheckCreateStateUpdateRaisingCommand(ep => ep.CreateRemoveCommand(orderItem));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      CheckCreateStateUpdateRaisingCommand(ep => ep.CreateDeleteCommand());
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      var collection = new DomainObjectCollection();
      CheckCreateStateUpdateRaisingCommand(ep => ep.CreateSetCollectionCommand(collection));
    }

    [Test]
    public void CreateInsertCommand ()
    {
      var orderItem = DomainObjectMother.CreateFakeObject<OrderItem>();
      CheckCreateStateUpdateRaisingCommand(ep => ep.CreateInsertCommand(orderItem, 0));
    }

    [Test]
    public void CreateAddCommand ()
    {
      var orderItem = DomainObjectMother.CreateFakeObject<OrderItem>();
      CheckCreateStateUpdateRaisingCommand(ep => ep.CreateAddCommand(orderItem));
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      var orderItem = DomainObjectMother.CreateFakeObject<OrderItem>();
      CheckCreateStateUpdateRaisingCommand(ep => ep.CreateReplaceCommand(0, orderItem));
    }

    [Test]
    public void ID ()
    {
      Assert.That(_decorator.ID, Is.EqualTo(_endPointID));
    }

    [Test]
    public void DelegatedMembers ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      var endPoint = new Mock<IRealObjectEndPoint>();
      var readOnlyCollectionDataDecorator = new ReadOnlyDomainObjectCollectionDataDecorator(new DomainObjectCollectionData());
      var domainObjectCollection = new DomainObjectCollection();
      var eventRaiser = new Mock<IDomainObjectCollectionEventRaiser>();
      var orderItem = DomainObjectMother.CreateFakeObject<OrderItem>();

      _decoratorTestHelper.CheckDelegation(ep => ep.IsNull, false);
      _decoratorTestHelper.CheckDelegation(ep => ep.IsNull, true);
      _decoratorTestHelper.CheckDelegation(ep => ep.ClientTransaction, ClientTransaction.CreateRootTransaction());
      _decoratorTestHelper.CheckDelegation(ep => ep.ObjectID, DomainObjectIDs.Order1);
      _decoratorTestHelper.CheckDelegation(ep => ep.Definition, GetEndPointDefinition(typeof(Order), "OrderItems"));
      _decoratorTestHelper.CheckDelegation(ep => ep.RelationDefinition, GetRelationDefinition(typeof(Order), "OrderItems"));
      _decoratorTestHelper.CheckDelegation(ep => ep.HasChanged, false);
      _decoratorTestHelper.CheckDelegation(ep => ep.HasChanged, true);
      _decoratorTestHelper.CheckDelegation(ep => ep.HasBeenTouched, false);
      _decoratorTestHelper.CheckDelegation(ep => ep.HasBeenTouched, true);
      _decoratorTestHelper.CheckDelegation(ep => ep.GetDomainObject(), orderItem);
      _decoratorTestHelper.CheckDelegation(ep => ep.GetDomainObjectReference(), orderItem);
      _decoratorTestHelper.CheckDelegation(ep => ep.IsDataComplete, false);
      _decoratorTestHelper.CheckDelegation(ep => ep.IsDataComplete, true);
      _decoratorTestHelper.CheckDelegation(ep => ep.EnsureDataComplete());
      _decoratorTestHelper.CheckDelegation(ep => ep.IsSynchronized, false);
      _decoratorTestHelper.CheckDelegation(ep => ep.IsSynchronized, true);
      _decoratorTestHelper.CheckDelegation(ep => ep.IsSynchronized, null);
      _decoratorTestHelper.CheckDelegation(ep => ep.Touch());
      _decoratorTestHelper.CheckDelegation(ep => ep.ValidateMandatory());
      _decoratorTestHelper.CheckDelegation(ep => ep.GetOppositeRelationEndPointIDs(), new[] { endPointID });
      _decoratorTestHelper.CheckDelegation(ep => ep.CanBeCollected, false);
      _decoratorTestHelper.CheckDelegation(ep => ep.CanBeCollected, true);
      _decoratorTestHelper.CheckDelegation(ep => ep.CanBeMarkedIncomplete, false);
      _decoratorTestHelper.CheckDelegation(ep => ep.CanBeMarkedIncomplete, true);
      _decoratorTestHelper.CheckDelegation(ep => ep.MarkDataIncomplete());
      _decoratorTestHelper.CheckDelegation(ep => ep.RegisterOriginalOppositeEndPoint(endPoint.Object));
      _decoratorTestHelper.CheckDelegation(ep => ep.UnregisterOriginalOppositeEndPoint(endPoint.Object));
      _decoratorTestHelper.CheckDelegation(ep => ep.RegisterCurrentOppositeEndPoint(endPoint.Object));
      _decoratorTestHelper.CheckDelegation(ep => ep.UnregisterCurrentOppositeEndPoint(endPoint.Object));
      _decoratorTestHelper.CheckDelegation(ep => ep.GetData(), readOnlyCollectionDataDecorator);
      _decoratorTestHelper.CheckDelegation(ep => ep.GetOriginalData(), readOnlyCollectionDataDecorator);
      _decoratorTestHelper.CheckDelegation(ep => ep.Collection, domainObjectCollection);
      _decoratorTestHelper.CheckDelegation(ep => ep.OriginalCollection, domainObjectCollection);
      _decoratorTestHelper.CheckDelegation(ep => ep.GetCollectionEventRaiser(), eventRaiser.Object);
      _decoratorTestHelper.CheckDelegation(ep => ep.GetCollectionWithOriginalData(), domainObjectCollection);
      _decoratorTestHelper.CheckDelegation(ep => ep.MarkDataComplete(new[] { orderItem }));

      _innerEndPointMock.Reset();

      _decoratorTestHelper.CheckDelegation(ep => ep.HasChangedFast, true);
      _decoratorTestHelper.CheckDelegation(ep => ep.HasChangedFast, false);
      _decoratorTestHelper.CheckDelegation(ep => ep.HasChangedFast, null);
    }

    [Test]
    public void TestToString ()
    {
      Assert.That(_decorator.ToString(), Is.EqualTo("StateUpdateRaisingDomainObjectCollectionEndPointDecorator { " + _innerEndPointMock.Object + " }"));
    }

    private void CheckDelegationWithStateUpdate (Expression<Action<IDomainObjectCollectionEndPoint>> action)
    {
      // Check with HasChangedFast returning the same value before and after the operation - no state update should be raised then

      _innerEndPointMock.Reset();
      _innerEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
      _innerEndPointMock.Setup(stub => stub.HasChangedFast).Returns(true);

      _listenerMock.Reset();

      _decoratorTestHelper.CheckDelegation(action);

      _listenerMock.Verify(mock => mock.VirtualEndPointStateUpdated(It.IsAny<RelationEndPointID>(), It.IsAny<bool?>()), Times.Never());

      // Check with HasChangedFast returning null, also check that listener is called _after_ delegation
      _innerEndPointMock.Reset();
      _innerEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
      _innerEndPointMock.Setup(stub => stub.HasChangedFast).Returns((bool?)null);

      _listenerMock.Setup(mock => mock.VirtualEndPointStateUpdated(_endPointID, null)).Verifiable();

      _decoratorTestHelper.CheckDelegationWithContinuation(
          action,
          _ => _listenerMock.Verify(mock => mock.VirtualEndPointStateUpdated(It.IsAny<RelationEndPointID>(), It.IsAny<bool?>()), Times.Never()));

      _listenerMock.Verify(mock => mock.VirtualEndPointStateUpdated(_endPointID, null), Times.AtLeastOnce());

      // Check with HasChangedFast returning true, then false

      _innerEndPointMock.Reset();
      _innerEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
      _innerEndPointMock
          .SetupSequence(stub => stub.HasChangedFast)
          .Returns(true)
          .Returns(false);

      _listenerMock.Reset();
      _listenerMock.Setup(mock => mock.VirtualEndPointStateUpdated(_endPointID, false)).Verifiable();

      _decoratorTestHelper.CheckDelegation(action);

      _listenerMock.Verify();

      // Check with exception

      _innerEndPointMock.Reset();
      _innerEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
      _innerEndPointMock
          .SetupSequence(stub => stub.HasChangedFast)
          .Returns(true)
          .Returns(false);

      _listenerMock.Reset();
      _listenerMock.Setup(mock => mock.VirtualEndPointStateUpdated(_endPointID, false)).Verifiable();

      var exception = new Exception();
      Assert.That(
          () => _decoratorTestHelper.CheckDelegationWithContinuation(action, _ => { throw exception; }),
          Throws.Exception.SameAs(exception));

      _listenerMock.Verify();
    }

    private void CheckCreateStateUpdateRaisingCommand (Expression<Func<IDomainObjectCollectionEndPoint, IDataManagementCommand>> action)
    {
      var fakeCommand = new Mock<IDataManagementCommand>();
      _decoratorTestHelper.CheckDelegation(
          action,
          fakeCommand.Object,
          result =>
          {
            Assert.That(
                result,
                Is.TypeOf<VirtualEndPointStateUpdatedRaisingCommandDecorator>()
                    .With.Property<VirtualEndPointStateUpdatedRaisingCommandDecorator>(d => d.DecoratedCommand).SameAs(fakeCommand.Object)
                    .With.Property<VirtualEndPointStateUpdatedRaisingCommandDecorator>(d => d.ModifiedEndPointID).EqualTo(_endPointID)
                    .And.Property<VirtualEndPointStateUpdatedRaisingCommandDecorator>(d => d.Listener).SameAs(_listenerMock.Object));
            var changeStateProvider = ((VirtualEndPointStateUpdatedRaisingCommandDecorator)result).ChangeStateProvider;

            _innerEndPointMock.Reset();
            _innerEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
            _innerEndPointMock.Setup(stub => stub.HasChangedFast).Returns(true);
            Assert.That(changeStateProvider(), Is.True);

            _innerEndPointMock.Reset();
            _innerEndPointMock.Setup(stub => stub.ID).Returns(_endPointID);
            _innerEndPointMock.Setup(stub => stub.HasChangedFast).Returns((bool?)null);
            Assert.That(changeStateProvider(), Is.Null);
          });
    }
  }
}
