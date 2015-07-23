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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class CollectionEndPointDataManagerTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private ICollectionEndPointChangeDetectionStrategy _changeDetectionStrategyMock;
    
    private DomainObject _domainObject1;
    private DomainObject _domainObject2;
    private DomainObject _domainObject3;
    private DomainObject _domainObject4;

    private IRealObjectEndPoint _domainObjectEndPoint1;
    private IRealObjectEndPoint _domainObjectEndPoint2;
    private IRealObjectEndPoint _domainObjectEndPoint3;

    private CollectionEndPointDataManager _dataManager;

    private Comparison<DomainObject> _comparison123;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _changeDetectionStrategyMock = MockRepository.GenerateStrictMock<ICollectionEndPointChangeDetectionStrategy> ();

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order3);
      _domainObject3 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order4);
      _domainObject4 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order5);

      _domainObjectEndPoint1 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _domainObjectEndPoint1.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject1);
      _domainObjectEndPoint1.Stub (stub => stub.ObjectID).Return (_domainObject1.ID);

      _domainObjectEndPoint2 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _domainObjectEndPoint2.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);
      _domainObjectEndPoint2.Stub (stub => stub.ObjectID).Return (_domainObject2.ID);

      _domainObjectEndPoint3 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _domainObjectEndPoint3.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject3);
      _domainObjectEndPoint3.Stub (stub => stub.ObjectID).Return (_domainObject3.ID);

      _dataManager = new CollectionEndPointDataManager (_endPointID, _changeDetectionStrategyMock);

      _comparison123 = Compare123;
    }

    [Test]
    public void Initialization ()
    {
      var dataManager = new CollectionEndPointDataManager (_endPointID, _changeDetectionStrategyMock);

      Assert.That (dataManager.CollectionData, Is.TypeOf (typeof (ChangeCachingCollectionDataDecorator)));
      Assert.That (dataManager.CollectionData.ToArray (), Is.Empty);
      Assert.That (dataManager.OriginalOppositeEndPoints, Is.Empty);
   }

    [Test]
    public void CollectionData ()
    {
      _dataManager.CollectionData.Insert (0, _domainObject1);

      Assert.That (_dataManager.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));
    }

    [Test]
    public void ContainsOriginalObjectID ()
    {
      Assert.That (_dataManager.ContainsOriginalObjectID (_domainObject1.ID), Is.False);

      _dataManager.RegisterOriginalItemWithoutEndPoint (_domainObject1);

      Assert.That (_dataManager.ContainsOriginalObjectID (_domainObject1.ID), Is.True);
    }

    [Test]
    public void ContainsOriginalOppositeEndPoint ()
    {
      var oppositeEndPoint = CollectionEndPointTestHelper.GetFakeOppositeEndPoint(_domainObject1);

      Assert.That (_dataManager.ContainsOriginalOppositeEndPoint (oppositeEndPoint), Is.False);

      _dataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint);

      Assert.That (_dataManager.ContainsOriginalOppositeEndPoint (oppositeEndPoint), Is.True);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      Assert.That (_dataManager.OriginalOppositeEndPoints.ToArray(), Is.Empty);

      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);
      endPointStub.Stub (stub => stub.ObjectID).Return (_domainObject2.ID);
      
      Assert.That (_dataManager.CollectionData.ToArray (), Has.No.Member (_domainObject2));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Has.No.Member(_domainObject2));
      Assert.That (_dataManager.OriginalItemsWithoutEndPoints, Has.No.Member(_domainObject2));

      _dataManager.RegisterOriginalOppositeEndPoint (endPointStub);

      Assert.That (_dataManager.HasDataChanged (), Is.False);
      Assert.That (_dataManager.CollectionData.ToArray (), Has.Member(_domainObject2));
      Assert.That (_dataManager.OriginalCollectionData.ToArray(), Has.Member(_domainObject2));
      Assert.That (_dataManager.OriginalOppositeEndPoints.ToArray(), Is.EqualTo (new[] { endPointStub }));
      Assert.That (_dataManager.CurrentOppositeEndPoints.ToArray (), Is.EqualTo (new[] { endPointStub }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has already been registered.")]
    public void RegisterOriginalOppositeEndPoint_AlreadyRegistered ()
    {
      var oppositeEndPoint = CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1);
      _dataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      _dataManager.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_AlreadyRegisteredWithoutEndPoint ()
    {
      Assert.That (_dataManager.OriginalOppositeEndPoints.ToArray (), Is.Empty);

      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);
      endPointStub.Stub (stub => stub.ObjectID).Return (_domainObject2.ID);

      _dataManager.RegisterOriginalItemWithoutEndPoint (_domainObject2);

      Assert.That (_dataManager.CollectionData.ToArray (), Has.Member(_domainObject2));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Has.Member(_domainObject2));
      Assert.That (_dataManager.OriginalOppositeEndPoints, Is.Empty);
      Assert.That (_dataManager.CurrentOppositeEndPoints, Is.Empty);
      Assert.That (_dataManager.OriginalItemsWithoutEndPoints, Has.Member(_domainObject2));

      _dataManager.RegisterOriginalOppositeEndPoint (endPointStub);

      Assert.That (_dataManager.HasDataChanged (), Is.False);
      Assert.That (_dataManager.CollectionData.ToArray (), Has.Member(_domainObject2));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Has.Member(_domainObject2));
      Assert.That (_dataManager.OriginalOppositeEndPoints.ToArray (), Is.EqualTo (new[] { endPointStub }));
      Assert.That (_dataManager.OriginalItemsWithoutEndPoints, Has.No.Member(_domainObject2));
      Assert.That (_dataManager.CurrentOppositeEndPoints.ToArray(), Is.EqualTo(new[]{endPointStub}));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);
      endPointStub.Stub (stub => stub.ObjectID).Return (_domainObject2.ID);
      endPointStub.Stub (stub => stub.MarkSynchronized());

      _dataManager.RegisterOriginalOppositeEndPoint (endPointStub);

      Assert.That (_dataManager.OriginalOppositeEndPoints.Length, Is.EqualTo (1));
      Assert.That (_dataManager.CollectionData.ToArray (), Has.Member(_domainObject2));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Has.Member(_domainObject2));
      Assert.That (_dataManager.CurrentOppositeEndPoints.ToArray (), Is.EqualTo (new[] { endPointStub }));

      _dataManager.UnregisterOriginalOppositeEndPoint (endPointStub);

      Assert.That (_dataManager.HasDataChanged (), Is.False);
      Assert.That (_dataManager.CollectionData.ToArray (), Has.No.Member(_domainObject2));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Has.No.Member(_domainObject2));
      Assert.That (_dataManager.OriginalOppositeEndPoints.ToArray(), Is.Empty);
      Assert.That (_dataManager.CurrentOppositeEndPoints, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has not been registered.")]
    public void UnregisterOriginalOppositeEndPoint_NotRegistered ()
    {
      var oppositeEndPoint = CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1);
      _dataManager.UnregisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    [Test]
    public void ContainsCurrentOppositeEndPoint ()
    {
      Assert.That (_dataManager.ContainsCurrentOppositeEndPoint (_domainObjectEndPoint1), Is.False);

      _dataManager.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint1);

      Assert.That (_dataManager.ContainsCurrentOppositeEndPoint (_domainObjectEndPoint1), Is.True);
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      Assert.That (_dataManager.CurrentOppositeEndPoints.ToArray (), Is.Empty);

      _dataManager.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint1);

      Assert.That (_dataManager.CurrentOppositeEndPoints.ToArray (), Is.EqualTo (new[] { _domainObjectEndPoint1 }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has already been registered.")]
    public void RegisterCurrentOppositeEndPoint_AlreadyRegistered ()
    {
      _dataManager.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint1);
      _dataManager.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint1);
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _dataManager.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint1);

      Assert.That (_dataManager.CurrentOppositeEndPoints.ToArray (), Has.Member(_domainObjectEndPoint1));

      _dataManager.UnregisterCurrentOppositeEndPoint (_domainObjectEndPoint1);

      Assert.That (_dataManager.CurrentOppositeEndPoints.ToArray(), Has.No.Member(_domainObjectEndPoint1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has not been registered.")]
    public void UnregisterCurrentOppositeEndPoint_NotRegistered ()
    {
      _dataManager.UnregisterCurrentOppositeEndPoint (_domainObjectEndPoint1);
    }
    
    [Test]
    public void ContainsOriginalItemWithoutEndPoint_True ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint (_domainObject2);
      Assert.That (_dataManager.ContainsOriginalItemWithoutEndPoint (_domainObject2), Is.True);
    }

    [Test]
    public void ContainsOriginalItemWithoutEndPoint_False ()
    {
      Assert.That (_dataManager.ContainsOriginalItemWithoutEndPoint (_domainObject2), Is.False);
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint ()
    {
      Assert.That (_dataManager.CollectionData.ToArray (), Has.No.Member(_domainObject2));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Has.No.Member(_domainObject2));
      Assert.That (_dataManager.OriginalItemsWithoutEndPoints.ToArray (), Has.No.Member (_domainObject2));

      _dataManager.RegisterOriginalItemWithoutEndPoint (_domainObject2);

      Assert.That (_dataManager.OriginalItemsWithoutEndPoints, Is.EqualTo (new[] { _domainObject2 }));
      Assert.That (_dataManager.HasDataChanged (), Is.False);
      Assert.That (_dataManager.CollectionData.ToArray (), Has.Member(_domainObject2));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Has.Member(_domainObject2));
      Assert.That (_dataManager.OriginalOppositeEndPoints.ToArray (), Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The original collection already contains a domain object with ID 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid'.")]
    public void RegisterOriginalItemWithoutEndPoint_AlreadyRegisteredWithoutEndPoint ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint (_domainObject2);
      _dataManager.RegisterOriginalItemWithoutEndPoint (_domainObject2);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The original collection already contains a domain object with ID 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid'.")]
    public void RegisterOriginalItemWithoutEndPoint_AlreadyRegisteredWithEndPoint ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (_domainObjectEndPoint2);
      try
      {
        _dataManager.RegisterOriginalItemWithoutEndPoint (_domainObject2);
      }
      catch
      {
        Assert.That (_dataManager.OriginalItemsWithoutEndPoints, Has.No.Member(_domainObject2));
        throw;
      }
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint ()
    {
      _dataManager.RegisterOriginalItemWithoutEndPoint (_domainObject2);
      Assert.That (_dataManager.OriginalItemsWithoutEndPoints, Is.EqualTo (new[] { _domainObject2 }));
      Assert.That (_dataManager.CollectionData.ToArray (), Has.Member(_domainObject2));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Has.Member(_domainObject2));

      _dataManager.UnregisterOriginalItemWithoutEndPoint (_domainObject2);

      Assert.That (_dataManager.CollectionData.ToArray (), Has.No.Member(_domainObject2));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Has.No.Member(_domainObject2));
      Assert.That (_dataManager.OriginalItemsWithoutEndPoints.ToArray (), Has.No.Member(_domainObject2));
      Assert.That (_dataManager.HasDataChanged (), Is.False);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' has not been registered as an item without end-point.")]
    public void UnregisterOriginalItemWithoutEndPoint_ItemNotRegisteredWithoutEndPoint ()
    {
      _dataManager.UnregisterOriginalItemWithoutEndPoint (_domainObject2);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' has not been registered as an item without end-point.")]
    public void UnregisterOriginalItemWithoutEndPoint_RegisteredWithEndPoint ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (_domainObjectEndPoint2);

      try
      {
        _dataManager.UnregisterOriginalItemWithoutEndPoint (_domainObject2);
      }
      catch
      {
        Assert.That (_dataManager.OriginalCollectionData.ToArray (), Has.Member(_domainObject2));
        throw;
      }
    }

    [Test]
    public void HasDataChanged ()
    {
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (
              Arg.Is (_dataManager.CollectionData), 
              Arg<IDomainObjectCollectionData>.List.Equal (_dataManager.OriginalCollectionData)))
          .Return (true);
      _changeDetectionStrategyMock.Replay ();

      // require use of strategy
      _dataManager.CollectionData.Add (_domainObject2);
      _dataManager.CollectionData.Remove (_domainObject2);

      var result = _dataManager.HasDataChanged ();

      _changeDetectionStrategyMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (true));
    }

    [Test]
    public void HasDataChanged_Cached ()
    {
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (
              Arg.Is (_dataManager.CollectionData), 
              Arg<IDomainObjectCollectionData>.List.Equal (_dataManager.OriginalCollectionData)))
          .Return (true)
          .Repeat.Once();
      _changeDetectionStrategyMock.Replay ();

      // require use of strategy
      _dataManager.CollectionData.Add (_domainObject2);
      _dataManager.CollectionData.Remove (_domainObject2);
 
      var result1 = _dataManager.HasDataChanged ();
      var result2 = _dataManager.HasDataChanged ();

      _changeDetectionStrategyMock.VerifyAllExpectations ();

      Assert.That (result1, Is.EqualTo (true));
      Assert.That (result2, Is.EqualTo (true));
    }

    [Test]
    public void HasDataChanged_Cache_InvalidatedOnModifyingOperations ()
    {
      using (_changeDetectionStrategyMock.GetMockRepository ().Ordered ())
      {
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (
                Arg.Is (_dataManager.CollectionData), 
                Arg<IDomainObjectCollectionData>.List.Equal (_dataManager.OriginalCollectionData)))
            .Return (true);
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (
                Arg.Is (_dataManager.CollectionData), 
                Arg<IDomainObjectCollectionData>.List.Equal (_dataManager.OriginalCollectionData)))
            .Return (false);
      }
      _changeDetectionStrategyMock.Replay ();

      // require use of strategy
      _dataManager.CollectionData.Add (_domainObject2);
      _dataManager.CollectionData.Remove (_domainObject2);

      var result1 = _dataManager.HasDataChanged ();

      _dataManager.CollectionData.Add (_domainObject2);
      _dataManager.CollectionData.Remove (_domainObject2);

      var result2 = _dataManager.HasDataChanged ();

      _changeDetectionStrategyMock.VerifyAllExpectations ();
      Assert.That (result1, Is.EqualTo (true));
      Assert.That (result2, Is.EqualTo (false));
    }

    [Test]
    public void HasDataChangedFast_CacheNotUpToDate ()
    {
      _changeDetectionStrategyMock.Replay ();

      _dataManager.CollectionData.Add (_domainObject2); // invalidate cache

      var result = _dataManager.HasDataChangedFast ();

      _changeDetectionStrategyMock.AssertWasNotCalled (
          mock => mock.HasDataChanged (Arg<IDomainObjectCollectionData>.Is.Anything, Arg<IDomainObjectCollectionData>.Is.Anything));
      Assert.That (result, Is.Null);
    }

    [Test]
    public void HasDataChangedFast_CacheUpToDate ()
    {
      _changeDetectionStrategyMock
            .Stub (mock => mock.HasDataChanged (Arg<IDomainObjectCollectionData>.Is.Anything, Arg<IDomainObjectCollectionData>.Is.Anything))
            .Return (true);
      _changeDetectionStrategyMock.Replay ();

      Dev.Null = _dataManager.HasDataChanged();
      _changeDetectionStrategyMock.BackToRecord();
      _changeDetectionStrategyMock.Replay();

      var result = _dataManager.HasDataChangedFast ();

      _changeDetectionStrategyMock.AssertWasNotCalled (
          mock => mock.HasDataChanged (Arg<IDomainObjectCollectionData>.Is.Anything, Arg<IDomainObjectCollectionData>.Is.Anything));
      Assert.That (result, Is.False);
    }

    [Test]
    public void SortCurrentData ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject3));
      _dataManager.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1));
      _dataManager.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject2));

      _dataManager.SortCurrentData (_comparison123);

      Assert.That (_dataManager.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject3, _domainObject1, _domainObject2 }));
    }

    [Test]
    public void SortCurrentAndOriginalData ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject3));
      _dataManager.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1));
      _dataManager.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject2));
      
      _dataManager.SortCurrentAndOriginalData(_comparison123);

      Assert.That (_dataManager.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
    }

    [Test]
    public void OriginalCollectionData ()
    {
      var originalData = _dataManager.OriginalCollectionData;

      Assert.That (originalData.ToArray (), Is.Empty);
      Assert.That (originalData.IsReadOnly, Is.True);
    }

    [Test]
    public void Commit_UpdatesOriginalContentsAndEndPoints ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (_domainObjectEndPoint1);
      _dataManager.RegisterOriginalItemWithoutEndPoint (_domainObject2);

      _dataManager.CollectionData.Insert (0, _domainObject3);
      _dataManager.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint3);

      _dataManager.CollectionData.Insert (0, _domainObject4);

      Assert.That (_dataManager.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject4, _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That (_dataManager.CurrentOppositeEndPoints, Is.EquivalentTo(new[] { _domainObjectEndPoint1, _domainObjectEndPoint3 }));

      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_dataManager.OriginalOppositeEndPoints, Is.EquivalentTo (new[] { _domainObjectEndPoint1 }));
      Assert.That (_dataManager.OriginalItemsWithoutEndPoints, Is.EquivalentTo (new[] { _domainObject2 }));

      _dataManager.Commit();

      Assert.That (_dataManager.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject4, _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That (_dataManager.CurrentOppositeEndPoints, Is.EquivalentTo (new[] { _domainObjectEndPoint1, _domainObjectEndPoint3 }));

      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject4, _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That (_dataManager.OriginalOppositeEndPoints, Is.EquivalentTo (new[] { _domainObjectEndPoint1, _domainObjectEndPoint3 }));
      Assert.That (_dataManager.OriginalItemsWithoutEndPoints, Is.EquivalentTo (new[] { _domainObject2, _domainObject4 }));
    }

    [Test]
    public void Commit_InvalidatesHasChangedCache ()
    {
      using (_changeDetectionStrategyMock.GetMockRepository ().Ordered ())
      {
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (
                Arg<IDomainObjectCollectionData>.List.Equal (_dataManager.CollectionData),
                Arg<IDomainObjectCollectionData>.List.Equal (_dataManager.OriginalCollectionData)))
            .Return (true);
      }
      _changeDetectionStrategyMock.Replay ();

      // require use of strategy
      _dataManager.CollectionData.Add (_domainObject2);
      _dataManager.CollectionData.Remove (_domainObject2);

      Assert.That (_dataManager.HasDataChanged (), Is.True);

      _dataManager.Commit ();

      Assert.That (_dataManager.HasDataChanged (), Is.False);
    }

    [Test]
    public void Rollback_UpdatesCurrentContentsAndEndPoints ()
    {
      _dataManager.RegisterOriginalOppositeEndPoint (_domainObjectEndPoint1);
      _dataManager.RegisterOriginalItemWithoutEndPoint (_domainObject2);

      _dataManager.CollectionData.Insert (0, _domainObject3);
      _dataManager.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint3);

      Assert.That (_dataManager.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That (_dataManager.CurrentOppositeEndPoints.ToArray (), Is.EquivalentTo (new[] { _domainObjectEndPoint1, _domainObjectEndPoint3 }));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_dataManager.OriginalOppositeEndPoints.ToArray (), Is.EquivalentTo (new[] { _domainObjectEndPoint1 }));
      Assert.That (_dataManager.OriginalItemsWithoutEndPoints.ToArray (), Is.EquivalentTo (new[] { _domainObject2 }));

      _dataManager.Rollback();

      Assert.That (_dataManager.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_dataManager.CurrentOppositeEndPoints.ToArray (), Is.EquivalentTo (new[] { _domainObjectEndPoint1 }));
      Assert.That (_dataManager.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_dataManager.OriginalOppositeEndPoints.ToArray (), Is.EquivalentTo (new[] { _domainObjectEndPoint1 }));
      Assert.That (_dataManager.OriginalItemsWithoutEndPoints.ToArray (), Is.EquivalentTo (new[] { _domainObject2 }));
    }

    [Test]
    public void Rollback_InvalidatesHasChangedCache ()
    {
      using (_changeDetectionStrategyMock.GetMockRepository ().Ordered ())
      {
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (
                Arg<IDomainObjectCollectionData>.List.Equal (_dataManager.CollectionData),
                Arg<IDomainObjectCollectionData>.List.Equal (_dataManager.OriginalCollectionData)))
            .Return (true);
      }
      _changeDetectionStrategyMock.Replay ();

      // require use of strategy
      _dataManager.CollectionData.Add (_domainObject2);
      _dataManager.CollectionData.Remove (_domainObject2);

      Assert.That (_dataManager.HasDataChanged (), Is.True);

      _dataManager.Rollback();

      Assert.That (_dataManager.HasDataChanged (), Is.False);
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      var sourceOppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      sourceOppositeEndPointStub.Stub (stub => stub.ID).Return (_domainObjectEndPoint2.ID);
      sourceOppositeEndPointStub.Stub (stub => stub.ObjectID).Return (_domainObjectEndPoint2.ObjectID);

      _dataManager.CollectionData.Add (_domainObject1);
      _dataManager.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint1);

      var sourceDataManager = new CollectionEndPointDataManager (_endPointID, MockRepository.GenerateStub<ICollectionEndPointChangeDetectionStrategy>());
      sourceDataManager.CollectionData.Add (_domainObject2);
      sourceDataManager.RegisterCurrentOppositeEndPoint (sourceOppositeEndPointStub);

      var endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (sourceOppositeEndPointStub.ID))
          .Return (_domainObjectEndPoint2);

      _dataManager.SetDataFromSubTransaction (sourceDataManager, endPointProviderStub);

      Assert.That (_dataManager.CollectionData.ToArray(), Is.EqualTo (new[] { _domainObject2 }));
      Assert.That (_dataManager.CurrentOppositeEndPoints, Is.EquivalentTo (new[] { _domainObjectEndPoint2 }));
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var changeDetectionStrategy = new SerializableCollectionEndPointChangeDetectionStrategyFake();
      var data = new CollectionEndPointDataManager (_endPointID, changeDetectionStrategy);

      var endPointFake = new SerializableRealObjectEndPointFake (null, _domainObject1);
      data.RegisterOriginalOppositeEndPoint (endPointFake);
      data.RegisterOriginalItemWithoutEndPoint (_domainObject2);

      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (data);

      Assert.That (deserializedInstance.EndPointID, Is.Not.Null);
      Assert.That (deserializedInstance.ChangeDetectionStrategy, Is.Not.Null);
      Assert.That (deserializedInstance.CollectionData.Count, Is.EqualTo (2));
      Assert.That (deserializedInstance.OriginalCollectionData.Count, Is.EqualTo (2));
      Assert.That (deserializedInstance.OriginalOppositeEndPoints.Length, Is.EqualTo (1));
      Assert.That (deserializedInstance.OriginalItemsWithoutEndPoints.Length, Is.EqualTo (1));
      Assert.That (deserializedInstance.CurrentOppositeEndPoints.Length, Is.EqualTo (1));
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