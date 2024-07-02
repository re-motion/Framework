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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class DomainObjectCollectionEndPointDataManagerTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private Mock<IDomainObjectCollectionEndPointChangeDetectionStrategy> _changeDetectionStrategyMock;

    private DomainObject _domainObject1;
    private DomainObject _domainObject2;
    private DomainObject _domainObject3;
    private DomainObject _domainObject4;

    private Mock<IRealObjectEndPoint> _domainObjectEndPoint1;
    private Mock<IRealObjectEndPoint> _domainObjectEndPoint2;
    private Mock<IRealObjectEndPoint> _domainObjectEndPoint3;

    private DomainObjectCollectionEndPointDataManager _dataManager;

    private Comparison<DomainObject> _comparison123;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Customer1, "Orders");
      _changeDetectionStrategyMock = new Mock<IDomainObjectCollectionEndPointChangeDetectionStrategy>(MockBehavior.Strict);

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order1);
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order3);
      _domainObject3 = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order4);
      _domainObject4 = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order5);

      _domainObjectEndPoint1 = new Mock<IRealObjectEndPoint>();
      _domainObjectEndPoint1.Setup(stub => stub.GetDomainObjectReference()).Returns(_domainObject1);
      _domainObjectEndPoint1.Setup(stub => stub.ObjectID).Returns(_domainObject1.ID);

      _domainObjectEndPoint2 = new Mock<IRealObjectEndPoint>();
      _domainObjectEndPoint2.Setup(stub => stub.GetDomainObjectReference()).Returns(_domainObject2);
      _domainObjectEndPoint2.Setup(stub => stub.ObjectID).Returns(_domainObject2.ID);

      _domainObjectEndPoint3 = new Mock<IRealObjectEndPoint>();
      _domainObjectEndPoint3.Setup(stub => stub.GetDomainObjectReference()).Returns(_domainObject3);
      _domainObjectEndPoint3.Setup(stub => stub.ObjectID).Returns(_domainObject3.ID);

      _dataManager = new DomainObjectCollectionEndPointDataManager(_endPointID, _changeDetectionStrategyMock.Object);

      _comparison123 = Compare123;
    }

    [Test]
    public void Initialization ()
    {
      var dataManager = new DomainObjectCollectionEndPointDataManager(_endPointID, _changeDetectionStrategyMock.Object);

      Assert.That(dataManager.CollectionData, Is.TypeOf(typeof(ChangeCachingDomainObjectCollectionDataDecorator)));
      Assert.That(dataManager.CollectionData.ToArray(), Is.Empty);
      Assert.That(dataManager.OriginalOppositeEndPoints, Is.Empty);
   }

    [Test]
    public void CollectionData ()
    {
      _dataManager.CollectionData.Insert(0, _domainObject1);

      Assert.That(_dataManager.CollectionData.ToArray(), Is.EqualTo(new[] { _domainObject1 }));
    }

    [Test]
    public void ContainsOriginalObjectID ()
    {
      Assert.That(_dataManager.ContainsOriginalObjectID(_domainObject1.ID), Is.False);

      _dataManager.RegisterOriginalItemWithoutEndPoint(_domainObject1);

      Assert.That(_dataManager.ContainsOriginalObjectID(_domainObject1.ID), Is.True);
    }

    [Test]
    public void ContainsOriginalOppositeEndPoint ()
    {
      var oppositeEndPoint = DomainObjectCollectionEndPointTestHelper.GetFakeOppositeEndPoint(_domainObject1);

      Assert.That(_dataManager.ContainsOriginalOppositeEndPoint(oppositeEndPoint), Is.False);

      _dataManager.RegisterOriginalOppositeEndPoint(oppositeEndPoint);

      Assert.That(_dataManager.ContainsOriginalOppositeEndPoint(oppositeEndPoint), Is.True);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      Assert.That(_dataManager.OriginalOppositeEndPoints.ToArray(), Is.Empty);

      var endPointStub = new Mock<IRealObjectEndPoint>();
      endPointStub.Setup(stub => stub.GetDomainObjectReference()).Returns(_domainObject2);
      endPointStub.Setup(stub => stub.ObjectID).Returns(_domainObject2.ID);

      Assert.That(_dataManager.CollectionData.ToArray(), Has.No.Member(_domainObject2));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Has.No.Member(_domainObject2));
      Assert.That(_dataManager.OriginalItemsWithoutEndPoints, Has.No.Member(_domainObject2));

      _dataManager.RegisterOriginalOppositeEndPoint(endPointStub.Object);

      Assert.That(_dataManager.HasDataChanged(), Is.False);
      Assert.That(_dataManager.CollectionData.ToArray(), Has.Member(_domainObject2));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Has.Member(_domainObject2));
      Assert.That(_dataManager.OriginalOppositeEndPoints.ToArray(), Is.EqualTo(new[] { endPointStub.Object }));
      Assert.That(_dataManager.CurrentOppositeEndPoints.ToArray(), Is.EqualTo(new[] { endPointStub.Object }));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_AlreadyRegistered ()
    {
      var oppositeEndPoint = DomainObjectCollectionEndPointTestHelper.GetFakeOppositeEndPoint(_domainObject1);
      _dataManager.RegisterOriginalOppositeEndPoint(oppositeEndPoint);
      Assert.That(
          () => _dataManager.RegisterOriginalOppositeEndPoint(oppositeEndPoint),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The opposite end-point has already been registered."));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_AlreadyRegisteredWithoutEndPoint ()
    {
      Assert.That(_dataManager.OriginalOppositeEndPoints.ToArray(), Is.Empty);

      var endPointStub = new Mock<IRealObjectEndPoint>();
      endPointStub.Setup(stub => stub.GetDomainObjectReference()).Returns(_domainObject2);
      endPointStub.Setup(stub => stub.ObjectID).Returns(_domainObject2.ID);

      _dataManager.RegisterOriginalItemWithoutEndPoint(_domainObject2);

      Assert.That(_dataManager.CollectionData.ToArray(), Has.Member(_domainObject2));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Has.Member(_domainObject2));
      Assert.That(_dataManager.OriginalOppositeEndPoints, Is.Empty);
      Assert.That(_dataManager.CurrentOppositeEndPoints, Is.Empty);
      Assert.That(_dataManager.OriginalItemsWithoutEndPoints, Has.Member(_domainObject2));

      _dataManager.RegisterOriginalOppositeEndPoint(endPointStub.Object);

      Assert.That(_dataManager.HasDataChanged(), Is.False);
      Assert.That(_dataManager.CollectionData.ToArray(), Has.Member(_domainObject2));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Has.Member(_domainObject2));
      Assert.That(_dataManager.OriginalOppositeEndPoints.ToArray(), Is.EqualTo(new[] { endPointStub.Object }));
      Assert.That(_dataManager.OriginalItemsWithoutEndPoints, Has.No.Member(_domainObject2));
      Assert.That(_dataManager.CurrentOppositeEndPoints.ToArray(), Is.EqualTo(new[]{endPointStub.Object}));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_NullOppositeEndPoint ()
    {
      var endPointStub = new Mock<IRealObjectEndPoint>();
      endPointStub.Setup(stub => stub.IsNull).Returns(true);

      Assert.That(
          () => _dataManager.RegisterOriginalOppositeEndPoint(endPointStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("End point must not be a null object.", "oppositeEndPoint"));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      var endPointStub = new Mock<IRealObjectEndPoint>();
      endPointStub.Setup(stub => stub.GetDomainObjectReference()).Returns(_domainObject2);
      endPointStub.Setup(stub => stub.ObjectID).Returns(_domainObject2.ID);
      endPointStub.Setup(stub => stub.MarkSynchronized());

      _dataManager.RegisterOriginalOppositeEndPoint(endPointStub.Object);

      Assert.That(_dataManager.OriginalOppositeEndPoints.Length, Is.EqualTo(1));
      Assert.That(_dataManager.CollectionData.ToArray(), Has.Member(_domainObject2));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Has.Member(_domainObject2));
      Assert.That(_dataManager.CurrentOppositeEndPoints.ToArray(), Is.EqualTo(new[] { endPointStub.Object }));

      _dataManager.UnregisterOriginalOppositeEndPoint(endPointStub.Object);

      Assert.That(_dataManager.HasDataChanged(), Is.False);
      Assert.That(_dataManager.CollectionData.ToArray(), Has.No.Member(_domainObject2));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Has.No.Member(_domainObject2));
      Assert.That(_dataManager.OriginalOppositeEndPoints.ToArray(), Is.Empty);
      Assert.That(_dataManager.CurrentOppositeEndPoints, Is.Empty);
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_NotRegistered ()
    {
      var oppositeEndPoint = DomainObjectCollectionEndPointTestHelper.GetFakeOppositeEndPoint(_domainObject1);
      Assert.That(
          () => _dataManager.UnregisterOriginalOppositeEndPoint(oppositeEndPoint),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The opposite end-point has not been registered."));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_NullOppositeEndPoint ()
    {
      var endPointStub = new Mock<IRealObjectEndPoint>();
      endPointStub.Setup(stub => stub.IsNull).Returns(true);

      Assert.That(
          () => _dataManager.UnregisterOriginalOppositeEndPoint(endPointStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("End point must not be a null object.", "oppositeEndPoint"));
    }

    [Test]
    public void ContainsCurrentOppositeEndPoint ()
    {
      Assert.That(_dataManager.ContainsCurrentOppositeEndPoint(_domainObjectEndPoint1.Object), Is.False);

      _dataManager.RegisterCurrentOppositeEndPoint(_domainObjectEndPoint1.Object);

      Assert.That(_dataManager.ContainsCurrentOppositeEndPoint(_domainObjectEndPoint1.Object), Is.True);
    }

    [Test]
    public void ContainsCurrentOppositeEndPoint_NullOppositeEndPoint ()
    {
      var endPointStub = new Mock<IRealObjectEndPoint>();
      endPointStub.Setup(stub => stub.IsNull).Returns(true);

      Assert.That(
          () => _dataManager.ContainsCurrentOppositeEndPoint(endPointStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("End point must not be a null object.", "oppositeEndPoint"));
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      Assert.That(_dataManager.CurrentOppositeEndPoints.ToArray(), Is.Empty);

      _dataManager.RegisterCurrentOppositeEndPoint(_domainObjectEndPoint1.Object);

      Assert.That(_dataManager.CurrentOppositeEndPoints.ToArray(), Is.EqualTo(new[] { _domainObjectEndPoint1.Object }));
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint_AlreadyRegistered ()
    {
      _dataManager.RegisterCurrentOppositeEndPoint(_domainObjectEndPoint1.Object);
      Assert.That(
          () => _dataManager.RegisterCurrentOppositeEndPoint(_domainObjectEndPoint1.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The opposite end-point has already been registered."));
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint_NullOppositeEndPoint ()
    {
      var endPointStub = new Mock<IRealObjectEndPoint>();
      endPointStub.Setup(stub => stub.IsNull).Returns(true);

      Assert.That(
          () => _dataManager.RegisterCurrentOppositeEndPoint(endPointStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("End point must not be a null object.", "oppositeEndPoint"));
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _dataManager.RegisterCurrentOppositeEndPoint(_domainObjectEndPoint1.Object);

      Assert.That(_dataManager.CurrentOppositeEndPoints.ToArray(), Has.Member(_domainObjectEndPoint1.Object));

      _dataManager.UnregisterCurrentOppositeEndPoint(_domainObjectEndPoint1.Object);

      Assert.That(_dataManager.CurrentOppositeEndPoints.ToArray(), Has.No.Member(_domainObjectEndPoint1.Object));
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint_NotRegistered ()
    {
      Assert.That(
          () => _dataManager.UnregisterCurrentOppositeEndPoint(_domainObjectEndPoint1.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The opposite end-point has not been registered."));
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint_NullOppositeEndPoint ()
    {
      var endPointStub = new Mock<IRealObjectEndPoint>();
      endPointStub.Setup(stub => stub.IsNull).Returns(true);

      Assert.That(
          () => _dataManager.UnregisterCurrentOppositeEndPoint(endPointStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("End point must not be a null object.", "oppositeEndPoint"));
    }

    [Test]
    public void ContainsOriginalItemWithoutEndPoint_True ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint(_domainObject2);
      Assert.That(_dataManager.ContainsOriginalItemWithoutEndPoint(_domainObject2), Is.True);
    }

    [Test]
    public void ContainsOriginalItemWithoutEndPoint_False ()
    {
      Assert.That(_dataManager.ContainsOriginalItemWithoutEndPoint(_domainObject2), Is.False);
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint ()
    {
      Assert.That(_dataManager.CollectionData.ToArray(), Has.No.Member(_domainObject2));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Has.No.Member(_domainObject2));
      Assert.That(_dataManager.OriginalItemsWithoutEndPoints.ToArray(), Has.No.Member(_domainObject2));

      _dataManager.RegisterOriginalItemWithoutEndPoint(_domainObject2);

      Assert.That(_dataManager.OriginalItemsWithoutEndPoints, Is.EqualTo(new[] { _domainObject2 }));
      Assert.That(_dataManager.HasDataChanged(), Is.False);
      Assert.That(_dataManager.CollectionData.ToArray(), Has.Member(_domainObject2));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Has.Member(_domainObject2));
      Assert.That(_dataManager.OriginalOppositeEndPoints.ToArray(), Is.Empty);
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint_AlreadyRegisteredWithoutEndPoint ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint(_domainObject2);
      Assert.That(
          () => _dataManager.RegisterOriginalItemWithoutEndPoint(_domainObject2),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The original collection already contains a domain object with ID 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid'."));
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint_AlreadyRegisteredWithEndPoint ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(_domainObjectEndPoint2.Object);

      Assert.That(
          () => _dataManager.RegisterOriginalItemWithoutEndPoint(_domainObject2),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The original collection already contains a domain object with ID 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid'."));

      Assert.That(_dataManager.OriginalItemsWithoutEndPoints, Has.No.Member(_domainObject2));
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint(_domainObject2);
      Assert.That(_dataManager.OriginalItemsWithoutEndPoints, Is.EqualTo(new[] { _domainObject2 }));
      Assert.That(_dataManager.CollectionData.ToArray(), Has.Member(_domainObject2));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Has.Member(_domainObject2));

      _dataManager.UnregisterOriginalItemWithoutEndPoint(_domainObject2);

      Assert.That(_dataManager.CollectionData.ToArray(), Has.No.Member(_domainObject2));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Has.No.Member(_domainObject2));
      Assert.That(_dataManager.OriginalItemsWithoutEndPoints.ToArray(), Has.No.Member(_domainObject2));
      Assert.That(_dataManager.HasDataChanged(), Is.False);
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint_ItemNotRegisteredWithoutEndPoint ()
    {
      Assert.That(
          () => _dataManager.UnregisterOriginalItemWithoutEndPoint(_domainObject2),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' has not been registered as an item without end-point."));
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint_RegisteredWithEndPoint ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(_domainObjectEndPoint2.Object);

      Assert.That(
          () => _dataManager.UnregisterOriginalItemWithoutEndPoint(_domainObject2),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object with ID 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' has not been registered as an item without end-point."));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Has.Member(_domainObject2));
    }

    [Test]
    public void HasDataChanged ()
    {
      _changeDetectionStrategyMock
          .Setup(mock => mock.HasDataChanged(_dataManager.CollectionData, _dataManager.OriginalCollectionData))
          .Returns(true)
          .Verifiable();

      // require use of strategy
      _dataManager.CollectionData.Add(_domainObject2);
      _dataManager.CollectionData.Remove(_domainObject2);

      var result = _dataManager.HasDataChanged();

      _changeDetectionStrategyMock.Verify();
      Assert.That(result, Is.EqualTo(true));
    }

    [Test]
    public void HasDataChanged_Cached ()
    {
      _changeDetectionStrategyMock
          .Setup(mock => mock.HasDataChanged(_dataManager.CollectionData, _dataManager.OriginalCollectionData))
          .Returns(true)
          .Verifiable();

      // require use of strategy
      _dataManager.CollectionData.Add(_domainObject2);
      _dataManager.CollectionData.Remove(_domainObject2);

      var result1 = _dataManager.HasDataChanged();
      var result2 = _dataManager.HasDataChanged();

      _changeDetectionStrategyMock.Verify(mock => mock.HasDataChanged(_dataManager.CollectionData, _dataManager.OriginalCollectionData), Times.Once());

      Assert.That(result1, Is.EqualTo(true));
      Assert.That(result2, Is.EqualTo(true));
    }

    [Test]
    public void HasDataChanged_Cache_InvalidatedOnModifyingOperations ()
    {
      _changeDetectionStrategyMock
            .SetupSequence(mock => mock.HasDataChanged(_dataManager.CollectionData, _dataManager.OriginalCollectionData))
            .Returns(true)
            .Returns(false);

      // require use of strategy
      _dataManager.CollectionData.Add(_domainObject2);
      _dataManager.CollectionData.Remove(_domainObject2);

      var result1 = _dataManager.HasDataChanged();

      _dataManager.CollectionData.Add(_domainObject2);
      _dataManager.CollectionData.Remove(_domainObject2);

      var result2 = _dataManager.HasDataChanged();

      _changeDetectionStrategyMock.Verify(mock => mock.HasDataChanged(_dataManager.CollectionData, _dataManager.OriginalCollectionData), Times.Exactly(2));

      Assert.That(result1, Is.EqualTo(true));
      Assert.That(result2, Is.EqualTo(false));
    }

    [Test]
    public void HasDataChangedFast_CacheNotUpToDate ()
    {
      _dataManager.CollectionData.Add(_domainObject2); // invalidate cache

      var result = _dataManager.HasDataChangedFast();

      _changeDetectionStrategyMock.Verify(mock => mock.HasDataChanged(It.IsAny<IDomainObjectCollectionData>(), It.IsAny<IDomainObjectCollectionData>()), Times.Never());
      Assert.That(result, Is.Null);
    }

    [Test]
    public void HasDataChangedFast_CacheUpToDate ()
    {
      _changeDetectionStrategyMock
            .Setup(mock => mock.HasDataChanged(It.IsAny<IDomainObjectCollectionData>(), It.IsAny<IDomainObjectCollectionData>()))
            .Returns(true);

      Dev.Null = _dataManager.HasDataChanged();
      _changeDetectionStrategyMock.Reset();

      var result = _dataManager.HasDataChangedFast();

      _changeDetectionStrategyMock.Verify(mock => mock.HasDataChanged(It.IsAny<IDomainObjectCollectionData>(), It.IsAny<IDomainObjectCollectionData>()), Times.Never());
      Assert.That(result, Is.False);
    }

    [Test]
    public void SortCurrentData ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(DomainObjectCollectionEndPointTestHelper.GetFakeOppositeEndPoint(_domainObject3));
      _dataManager.RegisterOriginalOppositeEndPoint(DomainObjectCollectionEndPointTestHelper.GetFakeOppositeEndPoint(_domainObject1));
      _dataManager.RegisterOriginalOppositeEndPoint(DomainObjectCollectionEndPointTestHelper.GetFakeOppositeEndPoint(_domainObject2));

      _dataManager.SortCurrentData(_comparison123);

      Assert.That(_dataManager.CollectionData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2, _domainObject3 }));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Is.EqualTo(new[] { _domainObject3, _domainObject1, _domainObject2 }));
    }

    [Test]
    public void SortCurrentAndOriginalData ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(DomainObjectCollectionEndPointTestHelper.GetFakeOppositeEndPoint(_domainObject3));
      _dataManager.RegisterOriginalOppositeEndPoint(DomainObjectCollectionEndPointTestHelper.GetFakeOppositeEndPoint(_domainObject1));
      _dataManager.RegisterOriginalOppositeEndPoint(DomainObjectCollectionEndPointTestHelper.GetFakeOppositeEndPoint(_domainObject2));

      _dataManager.SortCurrentAndOriginalData(_comparison123);

      Assert.That(_dataManager.CollectionData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2, _domainObject3 }));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2, _domainObject3 }));
    }

    [Test]
    public void OriginalCollectionData ()
    {
      var originalData = _dataManager.OriginalCollectionData;

      Assert.That(originalData.ToArray(), Is.Empty);
      Assert.That(originalData.IsReadOnly, Is.True);
    }

    [Test]
    public void Commit_UpdatesOriginalContentsAndEndPoints ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(_domainObjectEndPoint1.Object);
      _dataManager.RegisterOriginalItemWithoutEndPoint(_domainObject2);

      _dataManager.CollectionData.Insert(0, _domainObject3);
      _dataManager.RegisterCurrentOppositeEndPoint(_domainObjectEndPoint3.Object);

      _dataManager.CollectionData.Insert(0, _domainObject4);

      Assert.That(_dataManager.CollectionData.ToArray(), Is.EqualTo(new[] { _domainObject4, _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That(_dataManager.CurrentOppositeEndPoints, Is.EquivalentTo(new[] { _domainObjectEndPoint1.Object, _domainObjectEndPoint3.Object }));

      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2 }));
      Assert.That(_dataManager.OriginalOppositeEndPoints, Is.EquivalentTo(new[] { _domainObjectEndPoint1.Object }));
      Assert.That(_dataManager.OriginalItemsWithoutEndPoints, Is.EquivalentTo(new[] { _domainObject2 }));

      _dataManager.Commit();

      Assert.That(_dataManager.CollectionData.ToArray(), Is.EqualTo(new[] { _domainObject4, _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That(_dataManager.CurrentOppositeEndPoints, Is.EquivalentTo(new[] { _domainObjectEndPoint1.Object, _domainObjectEndPoint3.Object }));

      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Is.EqualTo(new[] { _domainObject4, _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That(_dataManager.OriginalOppositeEndPoints, Is.EquivalentTo(new[] { _domainObjectEndPoint1.Object, _domainObjectEndPoint3.Object }));
      Assert.That(_dataManager.OriginalItemsWithoutEndPoints, Is.EquivalentTo(new[] { _domainObject2, _domainObject4 }));
    }

    [Test]
    public void Commit_InvalidatesHasChangedCache ()
    {
      _changeDetectionStrategyMock
          .Setup(mock => mock.HasDataChanged(_dataManager.CollectionData, _dataManager.OriginalCollectionData))
          .Returns(true)
          .Verifiable();

      // require use of strategy
      _dataManager.CollectionData.Add(_domainObject2);
      _dataManager.CollectionData.Remove(_domainObject2);

      Assert.That(_dataManager.HasDataChanged(), Is.True);

      _dataManager.Commit();

      Assert.That(_dataManager.HasDataChanged(), Is.False);
    }

    [Test]
    public void Rollback_UpdatesCurrentContentsAndEndPoints ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint(_domainObjectEndPoint1.Object);
      _dataManager.RegisterOriginalItemWithoutEndPoint(_domainObject2);

      _dataManager.CollectionData.Insert(0, _domainObject3);
      _dataManager.RegisterCurrentOppositeEndPoint(_domainObjectEndPoint3.Object);

      Assert.That(_dataManager.CollectionData.ToArray(), Is.EqualTo(new[] { _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That(_dataManager.CurrentOppositeEndPoints.ToArray(), Is.EquivalentTo(new[] { _domainObjectEndPoint1.Object, _domainObjectEndPoint3.Object }));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2 }));
      Assert.That(_dataManager.OriginalOppositeEndPoints.ToArray(), Is.EquivalentTo(new[] { _domainObjectEndPoint1.Object }));
      Assert.That(_dataManager.OriginalItemsWithoutEndPoints.ToArray(), Is.EquivalentTo(new[] { _domainObject2 }));

      _dataManager.Rollback();

      Assert.That(_dataManager.CollectionData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2 }));
      Assert.That(_dataManager.CurrentOppositeEndPoints.ToArray(), Is.EquivalentTo(new[] { _domainObjectEndPoint1.Object }));
      Assert.That(_dataManager.OriginalCollectionData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2 }));
      Assert.That(_dataManager.OriginalOppositeEndPoints.ToArray(), Is.EquivalentTo(new[] { _domainObjectEndPoint1.Object }));
      Assert.That(_dataManager.OriginalItemsWithoutEndPoints.ToArray(), Is.EquivalentTo(new[] { _domainObject2 }));
    }

    [Test]
    public void Rollback_InvalidatesHasChangedCache ()
    {
      _changeDetectionStrategyMock
            .Setup(mock => mock.HasDataChanged(_dataManager.CollectionData, _dataManager.OriginalCollectionData))
            .Returns(true)
            .Verifiable();

      // require use of strategy
      _dataManager.CollectionData.Add(_domainObject2);
      _dataManager.CollectionData.Remove(_domainObject2);

      Assert.That(_dataManager.HasDataChanged(), Is.True);

      _dataManager.Rollback();

      Assert.That(_dataManager.HasDataChanged(), Is.False);
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      var sourceOppositeEndPointStub = new Mock<IRealObjectEndPoint>();
      sourceOppositeEndPointStub.Setup(stub => stub.ID).Returns(_domainObjectEndPoint2.Object.ID);
      sourceOppositeEndPointStub.Setup(stub => stub.ObjectID).Returns(_domainObjectEndPoint2.Object.ObjectID);

      _dataManager.CollectionData.Add(_domainObject1);
      _dataManager.RegisterCurrentOppositeEndPoint(_domainObjectEndPoint1.Object);

      var sourceDataManager = new DomainObjectCollectionEndPointDataManager(_endPointID, new Mock<IDomainObjectCollectionEndPointChangeDetectionStrategy>().Object);
      sourceDataManager.CollectionData.Add(_domainObject2);
      sourceDataManager.RegisterCurrentOppositeEndPoint(sourceOppositeEndPointStub.Object);

      var endPointProviderStub = new Mock<IRelationEndPointProvider>();
      endPointProviderStub
          .Setup(stub => stub.GetRelationEndPointWithoutLoading(sourceOppositeEndPointStub.Object.ID))
          .Returns(_domainObjectEndPoint2.Object);

      _dataManager.SetDataFromSubTransaction(sourceDataManager, endPointProviderStub.Object);

      Assert.That(_dataManager.CollectionData.ToArray(), Is.EqualTo(new[] { _domainObject2 }));
      Assert.That(_dataManager.CurrentOppositeEndPoints, Is.EquivalentTo(new[] { _domainObjectEndPoint2.Object }));
    }

    private int Compare123 (DomainObject x, DomainObject y)
    {
      if (x == y)
        return 0;

      if (x == _domainObject1)
        return -1;

      if (x == _domainObject3)
        return 1;

      if (y == _domainObject1)
        return 1;

      return -1;
    }
  }
}
