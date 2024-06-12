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
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class DomainObjectCollectionEndPointCollectionManagerTest : StandardMappingTest
  {
    private Mock<IDomainObjectCollectionEndPointCollectionProvider> _domainObjectCollectionProviderMock;
    private Mock<IAssociatedDomainObjectCollectionDataStrategyFactory> _associatedDomainObjectCollectionDataStrategyFactoryMock;
    private RelationEndPointID _endPointID;

    private DomainObjectCollectionEndPointCollectionManager _manager;

    private Mock<IDomainObjectCollectionData> _associatedDataStrategyStub;

    public override void SetUp ()
    {
      base.SetUp();

      _domainObjectCollectionProviderMock = new Mock<IDomainObjectCollectionEndPointCollectionProvider>(MockBehavior.Strict);
      _associatedDomainObjectCollectionDataStrategyFactoryMock = new Mock<IAssociatedDomainObjectCollectionDataStrategyFactory>(MockBehavior.Strict);
      _endPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, typeof(Customer), "Orders");

      _manager = new DomainObjectCollectionEndPointCollectionManager(
          _endPointID,
          _domainObjectCollectionProviderMock.Object,
          _associatedDomainObjectCollectionDataStrategyFactoryMock.Object);

      _associatedDataStrategyStub = new Mock<IDomainObjectCollectionData>();
      _associatedDataStrategyStub.Setup(stub => stub.RequiredItemType).Returns(typeof(Order));
      _associatedDataStrategyStub.Setup(stub => stub.AssociatedEndPointID).Returns(_endPointID);
    }

    [Test]
    public void GetOriginalCollectionReference ()
    {
      var collection = new DomainObjectCollection(_associatedDataStrategyStub.Object);
      _domainObjectCollectionProviderMock
          .Setup(mock => mock.GetCollection(_endPointID))
          .Returns(collection)
          .Verifiable();

      var result = _manager.GetOriginalCollectionReference();

      _domainObjectCollectionProviderMock.Verify();
      Assert.That(result, Is.SameAs(collection));
    }

    [Test]
    public void GetOriginalCollectionReference_Twice_ForSameID ()
    {
      var collection = new DomainObjectCollection(_associatedDataStrategyStub.Object);
      _domainObjectCollectionProviderMock
          .Setup(mock => mock.GetCollection(_endPointID))
          .Returns(collection)
          .Verifiable();

      var result1 = _manager.GetOriginalCollectionReference();
      var result2 = _manager.GetOriginalCollectionReference();

      _associatedDomainObjectCollectionDataStrategyFactoryMock.Verify();

      Assert.That(result2, Is.SameAs(result1));
    }

    [Test]
    public void GetCurrentCollectionReference_UsesOriginalReference ()
    {
      var collection = new DomainObjectCollection(_associatedDataStrategyStub.Object);
      _domainObjectCollectionProviderMock
          .Setup(mock => mock.GetCollection(_endPointID))
          .Returns(collection)
          .Verifiable();

      var originalResult = _manager.GetOriginalCollectionReference();
      var currentResult = _manager.GetCurrentCollectionReference();

      _associatedDomainObjectCollectionDataStrategyFactoryMock.Verify();
      Assert.That(currentResult, Is.SameAs(originalResult));
    }

    [Test]
    public void GetCurrentCollectionReference_CreatesOriginalReference_IfNoneAvailable ()
    {
      var collection = new DomainObjectCollection(_associatedDataStrategyStub.Object);
      _domainObjectCollectionProviderMock
          .Setup(mock => mock.GetCollection(_endPointID))
          .Returns(collection)
          .Verifiable();

      var result = _manager.GetCurrentCollectionReference();

      _associatedDomainObjectCollectionDataStrategyFactoryMock.Verify();
      Assert.That(result, Is.SameAs(collection));
      Assert.That(result, Is.SameAs(_manager.GetOriginalCollectionReference()));
    }

    [Test]
    public void AssociateCollectionWithEndPoint ()
    {
      var oldDataStrategyOfOldCollection = new DomainObjectCollectionData();
      var oldCollectionMock = new Mock<DomainObjectCollection>(MockBehavior.Strict);
      oldCollectionMock
          .As<IAssociatableDomainObjectCollection>()
          .Setup(mock => mock.AssociatedEndPointID)
          .Returns(_endPointID);
      oldCollectionMock
          .As<IAssociatableDomainObjectCollection>()
          .Setup(mock => mock.TransformToStandAlone())
          .Returns(oldDataStrategyOfOldCollection)
          .Verifiable();

      var oldDataStrategyOfNewCollection = new DomainObjectCollectionData();
      var newCollectionMock = new Mock<DomainObjectCollection>(MockBehavior.Strict);
      newCollectionMock
          .As<IAssociatableDomainObjectCollection>()
          .Setup(mock => mock.TransformToAssociated(_endPointID, _associatedDomainObjectCollectionDataStrategyFactoryMock.Object))
          .Returns(oldDataStrategyOfNewCollection)
          .Verifiable();

      RegisterOriginalCollection(oldCollectionMock.Object);

      var result = _manager.AssociateCollectionWithEndPoint(newCollectionMock.Object);

      oldCollectionMock.Verify();
      newCollectionMock.Verify();
      Assert.That(result, Is.SameAs(oldDataStrategyOfNewCollection));
    }

    [Test]
    public void AssociateCollectionWithEndPoint_RemembersTheNewCollectionAsCurrent ()
    {
      var oldCollection = RegisterAssociatedOriginalCollection();
      var newCollection = new DomainObjectCollection();

      _associatedDomainObjectCollectionDataStrategyFactoryMock.Setup(stub => stub.CreateDataStrategyForEndPoint(_endPointID)).Returns(_associatedDataStrategyStub.Object);

      _manager.AssociateCollectionWithEndPoint(newCollection);

      Assert.That(_manager.GetCurrentCollectionReference(), Is.SameAs(newCollection));
      Assert.That(_manager.GetOriginalCollectionReference(), Is.SameAs(oldCollection));
    }

    [Test]
    public void HasCollectionReferenceChanged_False_NoCollectionsYet ()
    {
      var result = _manager.HasCollectionReferenceChanged();

      Assert.That(result, Is.False);
    }

    [Test]
    public void HasCollectionReferenceChanged_False_NoCurrentCollectionYet ()
    {
      RegisterAssociatedOriginalCollection();

      var result = _manager.HasCollectionReferenceChanged();

      Assert.That(result, Is.False);
    }

    [Test]
    public void HasCollectionReferenceChanged_False_CurrentCollectionSameAsOriginal ()
    {
      RegisterAssociatedOriginalCollection();

      _manager.GetOriginalCollectionReference();
      _manager.GetCurrentCollectionReference();

      var result = _manager.HasCollectionReferenceChanged();

      Assert.That(result, Is.False);
    }

    [Test]
    public void HasCollectionReferenceChanged_True_CurrentCollectionChanged ()
    {
      RegisterAssociatedOriginalCollection();

      _associatedDomainObjectCollectionDataStrategyFactoryMock.Setup(stub => stub.CreateDataStrategyForEndPoint(_endPointID)).Returns(_associatedDataStrategyStub.Object);
      _manager.AssociateCollectionWithEndPoint(new OrderCollection());

      Assert.That(_manager.GetOriginalCollectionReference(), Is.Not.SameAs(_manager.GetCurrentCollectionReference()));

      var result = _manager.HasCollectionReferenceChanged();

      Assert.That(result, Is.True);
    }

    [Test]
    public void CommitCollectionReference_NoOriginalCollection ()
    {
      _manager.CommitCollectionReference();

      _domainObjectCollectionProviderMock.Setup(stub => stub.GetCollection(_endPointID)).Returns(new DomainObjectCollection(_associatedDataStrategyStub.Object));
      Assert.That(_manager.GetCurrentCollectionReference(), Is.SameAs(_manager.GetOriginalCollectionReference()));
    }

    [Test]
    public void CommitCollectionReference_NoCurrentCollection ()
    {
      RegisterAssociatedOriginalCollection();

      var originalBefore = _manager.GetOriginalCollectionReference();

      _manager.CommitCollectionReference();

      Assert.That(_manager.GetCurrentCollectionReference(), Is.SameAs(originalBefore));
      Assert.That(_manager.GetOriginalCollectionReference(), Is.SameAs(originalBefore));
    }

    [Test]
    public void CommitCollectionReference_NoChanges ()
    {
      RegisterAssociatedOriginalCollection();

      var originalBefore = _manager.GetOriginalCollectionReference();
      Assert.That(_manager.GetCurrentCollectionReference(), Is.SameAs(originalBefore));

      _manager.CommitCollectionReference();

      Assert.That(_manager.GetCurrentCollectionReference(), Is.SameAs(originalBefore));
      Assert.That(_manager.GetOriginalCollectionReference(), Is.SameAs(originalBefore));
    }

    [Test]
    public void CommitCollectionReference_Changes ()
    {
      RegisterAssociatedOriginalCollection();

      var newCollection = new OrderCollection();
      _associatedDomainObjectCollectionDataStrategyFactoryMock.Setup(stub => stub.CreateDataStrategyForEndPoint(_endPointID)).Returns(_associatedDataStrategyStub.Object);
      _manager.AssociateCollectionWithEndPoint(newCollection);

      _domainObjectCollectionProviderMock.Setup(mock => mock.RegisterCollection(_endPointID, newCollection)).Verifiable();

      _manager.CommitCollectionReference();

      _domainObjectCollectionProviderMock.Verify();
      Assert.That(_manager.GetCurrentCollectionReference(), Is.SameAs(newCollection));
      Assert.That(_manager.GetOriginalCollectionReference(), Is.SameAs(newCollection));
    }

    [Test]
    public void RollbackCollectionReference_NoOriginalCollection ()
    {
      _manager.RollbackCollectionReference();

      _domainObjectCollectionProviderMock.Setup(stub => stub.GetCollection(_endPointID)).Returns(new DomainObjectCollection(_associatedDataStrategyStub.Object));
      Assert.That(_manager.GetCurrentCollectionReference(), Is.SameAs(_manager.GetOriginalCollectionReference()));
    }

    [Test]
    public void RollbackCollectionReference_NoCurrentCollection ()
    {
      RegisterAssociatedOriginalCollection();

      var originalBefore = _manager.GetOriginalCollectionReference();

      _manager.RollbackCollectionReference();

      Assert.That(_manager.GetCurrentCollectionReference(), Is.SameAs(originalBefore));
      Assert.That(_manager.GetOriginalCollectionReference(), Is.SameAs(originalBefore));
    }

    [Test]
    public void RollbackCollectionReference_NoChanges ()
    {
      RegisterAssociatedOriginalCollection();

      var originalBefore = _manager.GetOriginalCollectionReference();
      Assert.That(_manager.GetCurrentCollectionReference(), Is.SameAs(originalBefore));

      _manager.RollbackCollectionReference();

      Assert.That(_manager.GetCurrentCollectionReference(), Is.SameAs(originalBefore));
      Assert.That(_manager.GetOriginalCollectionReference(), Is.SameAs(originalBefore));
    }

    [Test]
    public void RollbackCollectionReference_UndoesAssociation ()
    {
      var originalCollection = RegisterAssociatedOriginalCollection();

      var newCollection = new OrderCollection();
      _associatedDomainObjectCollectionDataStrategyFactoryMock.Setup(stub => stub.CreateDataStrategyForEndPoint(_endPointID)).Returns(_associatedDataStrategyStub.Object);
      _manager.AssociateCollectionWithEndPoint(newCollection);

      Assert.That(DomainObjectCollectionDataTestHelper.GetDataStrategy(newCollection), Is.SameAs(_associatedDataStrategyStub.Object));
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy(originalCollection, typeof(Order));

      // The Rollback operation must now transform the new collection to a standalone collection and reassociate the original collection with the end-
      // point being rolled back. (In addition to making the original collection the current collection again.)

      _manager.RollbackCollectionReference();

      Assert.That(_manager.GetCurrentCollectionReference(), Is.SameAs(originalCollection));
      Assert.That(_manager.GetOriginalCollectionReference(), Is.SameAs(originalCollection));

      Assert.That(DomainObjectCollectionDataTestHelper.GetDataStrategy(originalCollection), Is.SameAs(_associatedDataStrategyStub.Object));
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy(newCollection, typeof(Order));
    }

    [Test]
    public void RollbackCollectionReference_LeavesNewCollectionAloneIfAlreadyReassociatedWithOther ()
    {
      var originalCollection = RegisterAssociatedOriginalCollection();

      var newCollection = new OrderCollection();
      _associatedDomainObjectCollectionDataStrategyFactoryMock.Setup(stub => stub.CreateDataStrategyForEndPoint(_endPointID)).Returns(_associatedDataStrategyStub.Object);
      _manager.AssociateCollectionWithEndPoint(newCollection);

      Assert.That(DomainObjectCollectionDataTestHelper.GetDataStrategy(newCollection), Is.SameAs(_associatedDataStrategyStub.Object));
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy(originalCollection, typeof(Order));

      // Simulate that newCollection has already been re-associated by another rollback operation.
      // The Rollback operation must leave this other strategy alone.
      var otherStrategy = new DomainObjectCollectionData();
      DomainObjectCollectionDataTestHelper.SetDataStrategy(newCollection, otherStrategy);

      _manager.RollbackCollectionReference();

      Assert.That(DomainObjectCollectionDataTestHelper.GetDataStrategy(originalCollection), Is.SameAs(_associatedDataStrategyStub.Object));
      Assert.That(DomainObjectCollectionDataTestHelper.GetDataStrategy(newCollection), Is.SameAs(otherStrategy));
    }

    private DomainObjectCollection RegisterAssociatedOriginalCollection ()
    {
      var oldCollection = new DomainObjectCollection(_associatedDataStrategyStub.Object);
      StubEmptyDataStrategy(_associatedDataStrategyStub);
      RegisterOriginalCollection(oldCollection);
      return oldCollection;
    }

    private void RegisterOriginalCollection (DomainObjectCollection domainObjectCollection)
    {
      PrivateInvoke.SetNonPublicField(_manager, "_originalCollectionReference", domainObjectCollection);
    }

    private void StubEmptyDataStrategy (Mock<IDomainObjectCollectionData> dataStrategyStub)
    {
      dataStrategyStub.Setup(stub => stub.GetEnumerator()).Returns(Enumerable.Empty<DomainObject>().GetEnumerator());
    }
  }
}
