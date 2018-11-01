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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class CollectionEndPointCollectionManagerTest : StandardMappingTest
  {
    private ICollectionEndPointCollectionProvider _collectionProviderMock;
    private IAssociatedCollectionDataStrategyFactory _associatedCollectionDataStrategyFactoryMock;
    private RelationEndPointID _endPointID;

    private CollectionEndPointCollectionManager _manager;

    private IDomainObjectCollectionData _associatedDataStrategyStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _collectionProviderMock = MockRepository.GenerateStrictMock<ICollectionEndPointCollectionProvider>();
      _associatedCollectionDataStrategyFactoryMock = MockRepository.GenerateStrictMock<IAssociatedCollectionDataStrategyFactory>();
      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders");

      _manager = new CollectionEndPointCollectionManager (_endPointID, _collectionProviderMock, _associatedCollectionDataStrategyFactoryMock);
      
      _associatedDataStrategyStub = MockRepository.GenerateStub<IDomainObjectCollectionData> ();
      _associatedDataStrategyStub.Stub (stub => stub.RequiredItemType).Return (typeof (Order));
      _associatedDataStrategyStub.Stub (stub => stub.AssociatedEndPointID).Return (_endPointID);
    }

    [Test]
    public void GetOriginalCollectionReference ()
    {
      var collection = new DomainObjectCollection (_associatedDataStrategyStub);
      _collectionProviderMock
          .Expect (mock => mock.GetCollection (_endPointID))
          .Return (collection);
      _collectionProviderMock.Replay ();

      var result = _manager.GetOriginalCollectionReference ();

      _collectionProviderMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (collection));
    }

    [Test]
    public void GetOriginalCollectionReference_Twice_ForSameID ()
    {
      var collection = new DomainObjectCollection (_associatedDataStrategyStub);
      _collectionProviderMock
          .Expect (mock => mock.GetCollection (_endPointID))
          .Return (collection)
          .Repeat.Once();
      _collectionProviderMock.Replay ();

      var result1 = _manager.GetOriginalCollectionReference ();
      var result2 = _manager.GetOriginalCollectionReference ();

      _associatedCollectionDataStrategyFactoryMock.VerifyAllExpectations ();

      Assert.That (result2, Is.SameAs (result1));
    }

    [Test]
    public void GetCurrentCollectionReference_UsesOriginalReference ()
    {
      var collection = new DomainObjectCollection (_associatedDataStrategyStub);
      _collectionProviderMock
          .Expect (mock => mock.GetCollection (_endPointID))
          .Return (collection)
          .Repeat.Once();
      _collectionProviderMock.Replay ();

      var originalResult = _manager.GetOriginalCollectionReference ();
      var currentResult = _manager.GetCurrentCollectionReference ();

      _associatedCollectionDataStrategyFactoryMock.VerifyAllExpectations ();
      Assert.That (currentResult, Is.SameAs (originalResult));
    }

    [Test]
    public void GetCurrentCollectionReference_CreatesOriginalReference_IfNoneAvailable ()
    {
      var collection = new DomainObjectCollection (_associatedDataStrategyStub);
      _collectionProviderMock
          .Expect (mock => mock.GetCollection (_endPointID))
          .Return (collection)
          .Repeat.Once ();
      _collectionProviderMock.Replay ();

      var result = _manager.GetCurrentCollectionReference ();

      _associatedCollectionDataStrategyFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (collection));
      Assert.That (result, Is.SameAs (_manager.GetOriginalCollectionReference ()));
    }

    [Test]
    public void AssociateCollectionWithEndPoint ()
    {
      var oldDataStrategyOfOldCollection = new DomainObjectCollectionData ();
      var oldCollectionMock = MockRepository.GenerateStrictMock<DomainObjectCollection, IAssociatableDomainObjectCollection> ();
      oldCollectionMock.Stub (mock => ((IAssociatableDomainObjectCollection) mock).AssociatedEndPointID).Return (_endPointID);
      oldCollectionMock.Expect (mock => ((IAssociatableDomainObjectCollection) mock).TransformToStandAlone ()).Return (oldDataStrategyOfOldCollection);
      oldCollectionMock.Replay();

      var oldDataStrategyOfNewCollection = new DomainObjectCollectionData ();
      var newCollectionMock = MockRepository.GenerateStrictMock<DomainObjectCollection, IAssociatableDomainObjectCollection> ();
      newCollectionMock
          .Expect (mock => ((IAssociatableDomainObjectCollection) mock).TransformToAssociated (_endPointID, _associatedCollectionDataStrategyFactoryMock))
          .Return (oldDataStrategyOfNewCollection);
      newCollectionMock.Replay();

      RegisterOriginalCollection (oldCollectionMock);

      var result = _manager.AssociateCollectionWithEndPoint (newCollectionMock);

      oldCollectionMock.VerifyAllExpectations ();
      newCollectionMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (oldDataStrategyOfNewCollection));
    }

    [Test]
    public void AssociateCollectionWithEndPoint_RemembersTheNewCollectionAsCurrent ()
    {
      var oldCollection = RegisterAssociatedOriginalCollection();
      var newCollection = new DomainObjectCollection ();

      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointID)).Return (_associatedDataStrategyStub);
      
      _manager.AssociateCollectionWithEndPoint (newCollection);

      Assert.That (_manager.GetCurrentCollectionReference (), Is.SameAs (newCollection));
      Assert.That (_manager.GetOriginalCollectionReference (), Is.SameAs (oldCollection));
    }

    [Test]
    public void HasCollectionReferenceChanged_False_NoCollectionsYet ()
    {
      _collectionProviderMock.Replay();

      var result = _manager.HasCollectionReferenceChanged ();

      Assert.That (result, Is.False);
    }

    [Test]
    public void HasCollectionReferenceChanged_False_NoCurrentCollectionYet ()
    {
      RegisterAssociatedOriginalCollection();
      
      var result = _manager.HasCollectionReferenceChanged ();

      Assert.That (result, Is.False);
    }

    [Test]
    public void HasCollectionReferenceChanged_False_CurrentCollectionSameAsOriginal ()
    {
      RegisterAssociatedOriginalCollection ();

      _manager.GetOriginalCollectionReference ();
      _manager.GetCurrentCollectionReference ();

      var result = _manager.HasCollectionReferenceChanged ();

      Assert.That (result, Is.False);
    }

    [Test]
    public void HasCollectionReferenceChanged_True_CurrentCollectionChanged ()
    {
      RegisterAssociatedOriginalCollection ();

      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointID)).Return (_associatedDataStrategyStub);
      _manager.AssociateCollectionWithEndPoint (new OrderCollection ());

      Assert.That (_manager.GetOriginalCollectionReference (), Is.Not.SameAs (_manager.GetCurrentCollectionReference ()));

      var result = _manager.HasCollectionReferenceChanged ();

      Assert.That (result, Is.True);
    }

    [Test]
    public void CommitCollectionReference_NoOriginalCollection ()
    {
      _manager.CommitCollectionReference ();

      _collectionProviderMock.Stub (stub => stub.GetCollection (_endPointID)).Return (new DomainObjectCollection (_associatedDataStrategyStub));
      Assert.That (_manager.GetCurrentCollectionReference (), Is.SameAs (_manager.GetOriginalCollectionReference ()));
    }

    [Test]
    public void CommitCollectionReference_NoCurrentCollection ()
    {
      RegisterAssociatedOriginalCollection ();

      var originalBefore = _manager.GetOriginalCollectionReference ();

      _manager.CommitCollectionReference ();

      Assert.That (_manager.GetCurrentCollectionReference (), Is.SameAs (originalBefore));
      Assert.That (_manager.GetOriginalCollectionReference (), Is.SameAs (originalBefore));
    }

    [Test]
    public void CommitCollectionReference_NoChanges ()
    {
      RegisterAssociatedOriginalCollection ();

      var originalBefore = _manager.GetOriginalCollectionReference ();
      Assert.That (_manager.GetCurrentCollectionReference (), Is.SameAs (originalBefore));

      _manager.CommitCollectionReference ();

      Assert.That (_manager.GetCurrentCollectionReference (), Is.SameAs (originalBefore));
      Assert.That (_manager.GetOriginalCollectionReference (), Is.SameAs (originalBefore));
    }

    [Test]
    public void CommitCollectionReference_Changes ()
    {
      RegisterAssociatedOriginalCollection ();

      var newCollection = new OrderCollection();
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointID)).Return (_associatedDataStrategyStub);
      _manager.AssociateCollectionWithEndPoint (newCollection);

      _collectionProviderMock.Expect (mock => mock.RegisterCollection (_endPointID, newCollection));
      _collectionProviderMock.Replay();

      _manager.CommitCollectionReference ();

      _collectionProviderMock.VerifyAllExpectations();
      Assert.That (_manager.GetCurrentCollectionReference (), Is.SameAs (newCollection));
      Assert.That (_manager.GetOriginalCollectionReference (), Is.SameAs (newCollection));
    }

    [Test]
    public void RollbackCollectionReference_NoOriginalCollection ()
    {
      _manager.RollbackCollectionReference ();

      _collectionProviderMock.Stub (stub => stub.GetCollection (_endPointID)).Return (new DomainObjectCollection (_associatedDataStrategyStub));
      Assert.That (_manager.GetCurrentCollectionReference (), Is.SameAs (_manager.GetOriginalCollectionReference ()));
    }

    [Test]
    public void RollbackCollectionReference_NoCurrentCollection ()
    {
      RegisterAssociatedOriginalCollection ();

      var originalBefore = _manager.GetOriginalCollectionReference ();

      _manager.RollbackCollectionReference ();

      Assert.That (_manager.GetCurrentCollectionReference (), Is.SameAs (originalBefore));
      Assert.That (_manager.GetOriginalCollectionReference (), Is.SameAs (originalBefore));
    }

    [Test]
    public void RollbackCollectionReference_NoChanges ()
    {
      RegisterAssociatedOriginalCollection ();

      var originalBefore = _manager.GetOriginalCollectionReference ();
      Assert.That (_manager.GetCurrentCollectionReference (), Is.SameAs (originalBefore));

      _manager.RollbackCollectionReference ();

      Assert.That (_manager.GetCurrentCollectionReference (), Is.SameAs (originalBefore));
      Assert.That (_manager.GetOriginalCollectionReference (), Is.SameAs (originalBefore));
    }

    [Test]
    public void RollbackCollectionReference_UndoesAssociation ()
    {
      var originalCollection = RegisterAssociatedOriginalCollection ();

      var newCollection = new OrderCollection ();
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointID)).Return (_associatedDataStrategyStub);
      _manager.AssociateCollectionWithEndPoint (newCollection);

      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (newCollection), Is.SameAs (_associatedDataStrategyStub));
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (originalCollection, typeof (Order));

      // The Rollback operation must now transform the new collection to a standalone collection and reassociate the original collection with the end-
      // point being rolled back. (In addition to making the original collection the current collection again.)
      
      _manager.RollbackCollectionReference ();

      Assert.That (_manager.GetCurrentCollectionReference (), Is.SameAs (originalCollection));
      Assert.That (_manager.GetOriginalCollectionReference (), Is.SameAs (originalCollection));

      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (originalCollection), Is.SameAs (_associatedDataStrategyStub));
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (newCollection, typeof (Order));
    }

    [Test]
    public void RollbackCollectionReference_LeavesNewCollectionAloneIfAlreadyReassociatedWithOther ()
    {
      var originalCollection = RegisterAssociatedOriginalCollection ();

      var newCollection = new OrderCollection ();
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointID)).Return (_associatedDataStrategyStub);
      _manager.AssociateCollectionWithEndPoint (newCollection);

      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (newCollection), Is.SameAs (_associatedDataStrategyStub));
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (originalCollection, typeof (Order));
      
      // Simulate that newCollection has already been re-associated by another rollback operation.
      // The Rollback operation must leave this other strategy alone.
      var otherStrategy = new DomainObjectCollectionData();
      DomainObjectCollectionDataTestHelper.SetDataStrategy (newCollection, otherStrategy);

      _manager.RollbackCollectionReference ();

      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (originalCollection), Is.SameAs (_associatedDataStrategyStub));
      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (newCollection), Is.SameAs (otherStrategy));
    }

    [Test]
    public void Serialization ()
    {
      var instance = new CollectionEndPointCollectionManager (
          _endPointID,
          new SerializableCollectionEndPointCollectionProviderFake(),
          new SerializableAssociatedCollectionDataStrategyFactoryFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.EndPointID, Is.Not.Null);
      Assert.That (deserializedInstance.CollectionProvider, Is.Not.Null);
      Assert.That (deserializedInstance.DataStrategyFactory, Is.Not.Null);
    }

    private DomainObjectCollection RegisterAssociatedOriginalCollection ()
    {
      var oldCollection = new DomainObjectCollection (_associatedDataStrategyStub);
      StubEmptyDataStrategy (_associatedDataStrategyStub);
      RegisterOriginalCollection (oldCollection);
      return oldCollection;
    }

    private void RegisterOriginalCollection (DomainObjectCollection domainObjectCollection)
    {
      PrivateInvoke.SetNonPublicField (_manager, "_originalCollectionReference", domainObjectCollection);
    }

    private void StubEmptyDataStrategy (IDomainObjectCollectionData dataStrategyStub)
    {
      dataStrategyStub.Stub (stub => stub.GetEnumerator ()).Return (Enumerable.Empty<DomainObject> ().GetEnumerator ());
    }
  }
}