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
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public partial class DomainObjectCollectionTest : ClientTransactionBaseTest
  {
    private Customer _customer1;
    private Customer _customer2;
    private Customer _customer3NotInCollection;

    private DomainObjectCollection _collection;
    private DomainObjectCollection _readOnlyCollection;

    private Mock<IDomainObjectCollectionData> _dataStrategyMock;
    private DomainObjectCollection _collectionWithDataStrategyMock;

    public override void SetUp ()
    {
      base.SetUp();

      _customer1 = DomainObjectIDs.Customer1.GetObject<Customer>();
      _customer2 = DomainObjectIDs.Customer2.GetObject<Customer>();
      _customer3NotInCollection = DomainObjectIDs.Customer3.GetObject<Customer>();

      _collection = CreateCustomerCollection();
      _readOnlyCollection = DomainObjectCollectionFactory.Instance.CreateReadOnlyCollection(
          typeof(DomainObjectCollection),
          new[] { _customer1, _customer2 });

      _dataStrategyMock = new Mock<IDomainObjectCollectionData>();
      _collectionWithDataStrategyMock = new DomainObjectCollection(_dataStrategyMock.Object);
    }

    [Test]
    public void CreateDataStrategyForStandAloneCollection ()
    {
      var dataStoreStub = new Mock<IDomainObjectCollectionData>();
      var eventRaiserStub = new Mock<IDomainObjectCollectionEventRaiser>();

      var modificationCheckingDecorator =
          DomainObjectCollection.CreateDataStrategyForStandAloneCollection(dataStoreStub.Object, typeof(Order), eventRaiserStub.Object);
      Assert.That(modificationCheckingDecorator, Is.InstanceOf(typeof(ModificationCheckingDomainObjectCollectionDataDecorator)));
      Assert.That(modificationCheckingDecorator.RequiredItemType, Is.SameAs(typeof(Order)));

      var eventRaisingDecorator = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<EventRaisingDomainObjectCollectionDataDecorator>(
          (ModificationCheckingDomainObjectCollectionDataDecorator)modificationCheckingDecorator);
      Assert.That(eventRaisingDecorator.EventRaiser, Is.SameAs(eventRaiserStub.Object));

      var dataStore = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<IDomainObjectCollectionData>(eventRaisingDecorator);
      Assert.That(dataStore, Is.SameAs(dataStoreStub.Object));
    }

    [Test]
    public void Initialization_Default ()
    {
      var collection = new DomainObjectCollection();

      Assert.That(collection.IsReadOnly, Is.False);
      Assert.That(collection.AssociatedEndPointID, Is.Null);
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy(collection, null);
    }

    [Test]
    public void Initialization_WithItemType ()
    {
      var collection = new DomainObjectCollection(typeof(Order));

      Assert.That(collection.IsReadOnly, Is.False);
      Assert.That(collection.AssociatedEndPointID, Is.Null);
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy(collection, typeof(Order));
    }

    [Test]
    public void Initialization_WithData ()
    {
      var givenData = new DomainObjectCollectionData();
      var collection = new DomainObjectCollection(givenData);

      Assert.That(collection.IsReadOnly, Is.False);
      Assert.That(collection.AssociatedEndPointID, Is.Null);

      var actualData = DomainObjectCollectionDataTestHelper.GetDataStrategy(collection);
      Assert.That(actualData, Is.SameAs(givenData));
    }

    [Test]
    public void Initialization_WithEnumerable ()
    {
      var collection = new DomainObjectCollection(new[] { _customer1, _customer2 }, typeof(Customer));

      Assert.That(collection, Is.EqualTo(new[] { _customer1, _customer2 }));
      Assert.That(collection.RequiredItemType, Is.SameAs(typeof(Customer)));
      Assert.That(collection.IsReadOnly, Is.False);
      Assert.That(collection.AssociatedEndPointID, Is.Null);

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy(collection, typeof(Customer));
    }

    [Test]
    public void Initialization_WithEnumerable_ChecksItems ()
    {
      Assert.That(
          () => new DomainObjectCollection(new[] { _customer1 }, typeof(Order)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Item 0 of parameter 'domainObjects' has the type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer' "
                  + "instead of 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order'.", "domainObjects"));
    }

    [Test]
    public void IsDataComplete ()
    {
      _dataStrategyMock.Setup(mock => mock.IsDataComplete).Returns(true);
      Assert.That(_collectionWithDataStrategyMock.IsDataComplete, Is.True);

      _dataStrategyMock.Reset();
      _dataStrategyMock.Setup(mock => mock.IsDataComplete).Returns(false);
      Assert.That(_collectionWithDataStrategyMock.IsDataComplete, Is.False);
    }

    [Test]
    public void EnsureDataComplete_DelegatesToStrategy ()
    {
      _collectionWithDataStrategyMock.EnsureDataComplete();
      _dataStrategyMock.Verify(mock => mock.EnsureDataComplete(), Times.AtLeastOnce());
    }

    [Test]
    public void Count ()
    {
      Assert.That(_collection.Count, Is.EqualTo(2));
    }

    [Test]
    public void AssociatedEndPointID_AssociatedCollection ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, typeof(Customer), "Orders");
      _dataStrategyMock.Setup(stub => stub.AssociatedEndPointID).Returns(endPointID);

      Assert.That(_collectionWithDataStrategyMock.AssociatedEndPointID, Is.EqualTo(endPointID));
    }

    [Test]
    public void AssociatedEndPointID_StandAloneCollection ()
    {
      Assert.That(_collection.AssociatedEndPointID, Is.Null);
    }

    [Test]
    public void GetEnumerator ()
    {
      using (var enumerator = _collection.GetEnumerator())
      {
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.SameAs(_customer1));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.SameAs(_customer2));

        Assert.That(enumerator.MoveNext(), Is.False);
      }
    }

    [Test]
    public void ContainsObject_True ()
    {
      Assert.That(_collection.ContainsObject(_customer1), Is.True);
    }

    [Test]
    public void ContainsObject_False_NoID ()
    {
      Assert.That(_collection.ContainsObject(_customer3NotInCollection), Is.False);
    }

    [Test]
    public void ContainsObject_False_SameID_DifferentReference ()
    {
      var customer1InOtherTransaction = DomainObjectMother.GetObjectInOtherTransaction<Customer>(_collection[0].ID);
      Assert.That(_collection.ContainsObject(customer1InOtherTransaction), Is.False);
    }

    [Test]
    public void ContainsDomainObject_WithNull ()
    {
      Assert.That(
          () => _collection.ContainsObject(null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Contains_True ()
    {
      Assert.That(_collection.Contains(_customer1.ID), Is.True);
    }

    [Test]
    public void Contains_False ()
    {
      Assert.That(_collection.Contains(_customer3NotInCollection.ID), Is.False);
    }

    [Test]
    public void Contains_Null ()
    {
      Assert.That(
          () => _collection.Contains(null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void IndexOf_Object ()
    {
      Assert.That(_collection.IndexOf(_customer1), Is.EqualTo(0));
    }

    [Test]
    public void IndexOf_Object_Null ()
    {
      Assert.That(_collection.IndexOf((DomainObject)null), Is.EqualTo(-1));
    }

    [Test]
    public void IndexOf_Object_OtherTransaction ()
    {
      var customer1InOtherTransaction = DomainObjectMother.GetObjectInOtherTransaction<Customer>(_collection[0].ID);
      Assert.That(_collection.IndexOf(customer1InOtherTransaction), Is.EqualTo(-1));
    }

    [Test]
    public void IndexOf_Object_IDNotContained ()
    {
      Assert.That(_collection.IndexOf(_customer3NotInCollection), Is.EqualTo(-1));
    }

    [Test]
    public void IndexOf_ID ()
    {
      Assert.That(_collection.IndexOf(_customer1.ID), Is.EqualTo(0));
    }

    [Test]
    public void IndexOf_ID_Null ()
    {
      Assert.That(_collection.IndexOf((ObjectID)null), Is.EqualTo(-1));
    }

    [Test]
    public void Item_Get_ByIndex ()
    {
      _dataStrategyMock.Setup(stub => stub.GetObject(12)).Returns(_customer1);

      Assert.That(_collectionWithDataStrategyMock[12], Is.SameAs(_customer1));
    }

    [Test]
    public void Item_Get_ByID ()
    {
      _dataStrategyMock.Setup(stub => stub.GetObject(_customer1.ID)).Returns(_customer1);

      Assert.That(_collectionWithDataStrategyMock[_customer1.ID], Is.SameAs(_customer1));
    }

    [Test]
    public void Item_Set ()
    {
      _collectionWithDataStrategyMock[12] = _customer1;
      _dataStrategyMock.Verify(mock => mock.Replace(12, _customer1), Times.AtLeastOnce());
    }

    [Test]
    public void Item_Set_Null ()
    {
      _dataStrategyMock.Setup(stub => stub.GetObject(12)).Returns(_customer2);
      _collectionWithDataStrategyMock[12] = null;

      _dataStrategyMock.Verify(mock => mock.Remove(_customer2), Times.AtLeastOnce());
    }

    [Test]
    public void Item_Set_ReadOnly_Throws ()
    {
      Assert.That(
          () => _readOnlyCollection[0] = _customer3NotInCollection,
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Cannot modify a read-only collection."));
    }

    [Test]
    public void Add ()
    {
      var result = _collection.Add(_customer3NotInCollection);
      Assert.That(result, Is.EqualTo(2));

      Assert.That(_collection, Is.EqualTo(new[] { _customer1, _customer2, _customer3NotInCollection }));
    }

    [Test]
    public void Add_ReadOnly_Throws ()
    {
      Assert.That(
          () => _readOnlyCollection.Add(_customer3NotInCollection),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot add an item to a read-only collection."));
    }

    [Test]
    public void AddRange ()
    {
      var collection = new DomainObjectCollection();
      collection.AddRange(new[] { _customer1, _customer2 });

      Assert.That(collection, Is.EqualTo(new[] { _customer1, _customer2 }));
    }

    [Test]
    public void AddRange_ChecksItems ()
    {
      var collection = new DomainObjectCollection();
      Assert.That(
          () => collection.AddRange(new[] { _customer1, null }),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo("Item 1 of parameter 'domainObjects' is null.", "domainObjects"));
    }

    [Test]
    public void AddRange_ReadOnly_Throws ()
    {
      Assert.That(
          () => _readOnlyCollection.AddRange(new[] { _customer3NotInCollection }),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Cannot add items to a read-only collection."));
    }

    [Test]
    public void RemoveAt ()
    {
      _dataStrategyMock.Setup(stub => stub.GetObject(12)).Returns(_customer2);
      _collectionWithDataStrategyMock.RemoveAt(12);

      _dataStrategyMock.Verify(mock => mock.Remove(_customer2), Times.AtLeastOnce());
    }

    [Test]
    public void RemoveAt_ReadOnly_Throws ()
    {
      Assert.That(
          () => _readOnlyCollection.RemoveAt(0),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot remove an item from a read-only collection."));
    }

    [Test]
    public void Remove_ID ()
    {
      _collectionWithDataStrategyMock.Remove(_customer1.ID);

      _dataStrategyMock.Verify(mock => mock.Remove(_customer1.ID), Times.AtLeastOnce());
    }

    [Test]
    public void Remove_ID_ReadOnly_Throws ()
    {
      Assert.That(
          () => _readOnlyCollection.Remove(_customer1.ID),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot remove an item from a read-only collection."));
    }

    [Test]
    public void Remove_Object ()
    {
      _collectionWithDataStrategyMock.Remove(_customer1);

      _dataStrategyMock.Verify(mock => mock.Remove(_customer1), Times.AtLeastOnce());
    }

    [Test]
    public void Remove_Object_ReadOnly_Throws ()
    {
      Assert.That(
          () => _readOnlyCollection.Remove(_customer1),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot remove an item from a read-only collection."));
    }

    [Test]
    public void Clear ()
    {
      _collectionWithDataStrategyMock.Clear();

      _dataStrategyMock.Verify(mock => mock.Clear(), Times.AtLeastOnce());
    }

    [Test]
    public void Clear_ReadOnly_Throws ()
    {
      Assert.That(
          () => _readOnlyCollection.Clear(),
          Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void Insert_Object ()
    {
      _collectionWithDataStrategyMock.Insert(12, _customer1);

      _dataStrategyMock.Verify(mock => mock.Insert(12, _customer1), Times.AtLeastOnce());
    }

    [Test]
    public void Insert_Object_ReadOnly_Throws ()
    {
      Assert.That(
          () => _readOnlyCollection.Insert(0, _customer3NotInCollection),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot insert an item into a read-only collection."));
    }

    [Test]
    public void CopyTo ()
    {
      var array = new DomainObject[4];
      _collection.CopyTo(array, 1);

      Assert.That(array, Is.EqualTo(new[] { null, _customer1, _customer2, null }));
    }

    [Test]
    public void CopyTo_ArraySmallerThanCollection ()
    {
      var array = new DomainObject[_collection.Count - 1];
      Assert.That(
          () => _collection.CopyTo(array, 0),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Destination array was not long enough. Check the destination index, length, and the array's lower bounds.", "destinationArray")
          );
    }

    [Test]
    public void CopyTo_EmptyArray_WithEmptyCollection ()
    {
      var emptyCollection = new DomainObjectCollection();
      var array = new DomainObject[0];

      emptyCollection.CopyTo(array, 0);

      // expectation: no exception
    }

    [Test]
    public void Sort ()
    {
      Assert.That(_collection, Is.EqualTo(new[] { _customer1, _customer2 }));

      var weights = new Dictionary<DomainObject, int> { { _customer1, 2 }, { _customer2, 1 } };
      Comparison<DomainObject> comparison = ((one, two) => weights[one].CompareTo(weights[two]));
      PrivateInvoke.InvokeNonPublicMethod(_collection, "Sort", comparison);

      Assert.That(_collection, Is.EqualTo(new[] { _customer2, _customer1 }));
    }

    [Test]
    public void Clone ()
    {
      var clonedCollection = _collection.Clone();

      Assert.That(clonedCollection, Is.EqualTo(new[] { _customer1, _customer2 }));
      Assert.That(clonedCollection.IsReadOnly, Is.False);
      Assert.That(clonedCollection.RequiredItemType, Is.EqualTo(_collection.RequiredItemType));

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy(clonedCollection, typeof(Customer));
    }

    [Test]
    public void Clone_DecouplesFromOriginalDataStore ()
    {
      var clonedCollection = _collection.Clone();

      _collection.Remove(_customer1);
      clonedCollection.Add(_customer3NotInCollection);

      Assert.That(_collection, Is.EqualTo(new[] { _customer2 }));
      Assert.That(clonedCollection, Is.EqualTo(new[] { _customer1, _customer2, _customer3NotInCollection }));
    }

    [Test]
    public void Clone_BecomesStandAlone ()
    {
      OrderCollection associatedCollection = CreateAssociatedCollectionWithEndPointStub();
      var clonedCollection = (DomainObjectCollection)associatedCollection.Clone();

      // clone is always stand-alone, even when source is associated with end point
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy(clonedCollection, associatedCollection.RequiredItemType);
    }

    [Test]
    public void Clone_IsOfSameType_AsOriginal ()
    {
      var orderCollection = new OrderCollection();

      var clone = (OrderCollection)orderCollection.Clone();

      Assert.That(clone.GetType(), Is.EqualTo(typeof(OrderCollection)));
      Assert.That(clone.RequiredItemType, Is.EqualTo(orderCollection.RequiredItemType));
    }

    [Test]
    public void Clone_ReadOnly ()
    {
      var clonedCollection = _collection.Clone(true);

      Assert.That(clonedCollection, Is.EqualTo(new[] { _customer1, _customer2 }));
      Assert.That(clonedCollection.IsReadOnly, Is.True);
    }

    [Test]
    public void Clone_ReadOnly_DecouplesFromOriginalDataStore ()
    {
      var clonedCollection = _collection.Clone(true);

      _collection.Remove(_customer1);

      Assert.That(_collection, Is.EqualTo(new[] { _customer2 }));
      Assert.That(clonedCollection, Is.EqualTo(new[] { _customer1, _customer2 }));
    }

    [Test]
    public void Clone_ReadOnly_DataStrategy ()
    {
      OrderCollection associatedCollection = CreateAssociatedCollectionWithEndPointStub();
      var clonedCollection = associatedCollection.Clone(true);

      // clone is always stand-alone, even when source is associated with end point
      DomainObjectCollectionDataTestHelper.CheckReadOnlyCollectionStrategy(clonedCollection);
    }

    [Test]
    public void Clone_ReadOnly_IsOfSameType_AsOriginal ()
    {
      var orderCollection = new OrderCollection();

      var clone = (OrderCollection)orderCollection.Clone(true);

      Assert.That(clone.GetType(), Is.EqualTo(typeof(OrderCollection)));
      Assert.That(clone.RequiredItemType, Is.Null);
    }

    [Test]
    public void TransformToAssociated ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, typeof(Customer), "Orders");
      var originalCollectionDataStrategy = DomainObjectCollectionDataTestHelper.GetDataStrategy(_collection);
      var originalCollectionContents = _collection.Cast<DomainObject>().ToArray();
      var originalEndPointContents =
          ((IDomainObjectCollectionEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad(endPointID)).GetData().ToArray();
      var associatedCollectionDataStrategyFactory = new AssociatedDomainObjectCollectionDataStrategyFactory(TestableClientTransaction.DataManager);

      var result = ((IAssociatableDomainObjectCollection)_collection).TransformToAssociated(endPointID, associatedCollectionDataStrategyFactory);

      DomainObjectCollectionDataTestHelper.CheckAssociatedCollectionStrategy(_collection, typeof(Order), endPointID);
      Assert.That(result, Is.SameAs(originalCollectionDataStrategy));
      Assert.That(result, Is.EqualTo(originalCollectionContents));
      Assert.That(_collection, Is.EqualTo(originalEndPointContents));
    }

    [Test]
    public void TransformToStandAlone ()
    {
      var endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint_Customer1_Orders();
      var collection = endPoint.Collection;
      var originalCollectionDataStrategy = DomainObjectCollectionDataTestHelper.GetDataStrategy(collection);
      var originalCollectionContents = collection.Cast<DomainObject>().ToArray();

      var result = ((IAssociatableDomainObjectCollection)collection).TransformToStandAlone();

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy(collection, typeof(Order));
      Assert.That(collection, Is.EqualTo(originalCollectionContents));
      Assert.That(result, Is.SameAs(originalCollectionDataStrategy));
    }

    [Test]
    public void CopyEventHandlersFrom ()
    {
      var source = new DomainObjectCollection();
      var destination = new DomainObjectCollection();

      source.Added += delegate { };
      source.Added += delegate { };
      source.Adding += delegate { };
      source.Adding += delegate { };
      source.Removed += delegate { };
      source.Removed += delegate { };
      source.Removing += delegate { };
      source.Removing += delegate { };
      source.Deleted += delegate { };
      source.Deleted += delegate { };
      source.Deleting += delegate { };
      source.Deleting += delegate { };

      CallCopyEventHandlersFrom(source, destination);

      CheckSameEventHandlers(source, destination, "Adding");
      CheckSameEventHandlers(source, destination, "Added");
      CheckSameEventHandlers(source, destination, "Removing");
      CheckSameEventHandlers(source, destination, "Removed");
      CheckSameEventHandlers(source, destination, "Deleting");
      CheckSameEventHandlers(source, destination, "Deleted");
    }

    [Test]
    public void OnDeleting ()
    {
      object eventSender = null;
      EventArgs eventArgs = null;
      _collection.Deleting += (sender, args) => { eventSender = sender; eventArgs = args; };

      PrivateInvoke.InvokeNonPublicMethod(_collection, "OnDeleting");

      Assert.That(eventSender, Is.SameAs(_collection));
      Assert.That(eventArgs, Is.EqualTo(EventArgs.Empty));
    }

    [Test]
    public void OnDeleted ()
    {
      object eventSender = null;
      EventArgs eventArgs = null;
      _collection.Deleted += (sender, args) => { eventSender = sender; eventArgs = args; };

      PrivateInvoke.InvokeNonPublicMethod(_collection, "OnDeleted");

      Assert.That(eventSender, Is.SameAs(_collection));
      Assert.That(eventArgs, Is.EqualTo(EventArgs.Empty));
    }

    private void CallCopyEventHandlersFrom (DomainObjectCollection source, DomainObjectCollection destination)
    {
      PrivateInvoke.InvokeNonPublicMethod(destination, "CopyEventHandlersFrom", source);
    }

    private void CheckSameEventHandlers (DomainObjectCollection source, DomainObjectCollection destination, string eventName)
    {
      var sourceEvent = ((Delegate)PrivateInvoke.GetNonPublicField(source, eventName));
      Delegate[] sourceInvocationList = sourceEvent.GetInvocationList();

      var destinationEvent = ((Delegate)PrivateInvoke.GetNonPublicField(destination, eventName));
      Assert.That(destinationEvent, Is.Not.Null, eventName + " event handlers not copied");
      Delegate[] destinationInvocationList = destinationEvent.GetInvocationList();

      Assert.That(sourceInvocationList, Is.EqualTo(destinationInvocationList), eventName + " event handlers not copied");
    }

    private DomainObjectCollection CreateCustomerCollection ()
    {
      var collection = new DomainObjectCollection(typeof(Customer));
      collection.Add(_customer1);
      collection.Add(_customer2);

      return collection;
    }

    private OrderCollection CreateAssociatedCollectionWithEndPointStub ()
    {
      var collectionEndPointStub = new Mock<IDomainObjectCollectionEndPoint>();
      var endPointDataStub = new ReadOnlyDomainObjectCollectionDataDecorator(new DomainObjectCollectionData());

      collectionEndPointStub.Setup(stub => stub.GetData()).Returns(endPointDataStub);

      var virtualEndPointProviderStub = new Mock<IVirtualEndPointProvider>();
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, typeof(Customer), "Orders");
      virtualEndPointProviderStub.Setup(stub => stub.GetOrCreateVirtualEndPoint(endPointID)).Returns(collectionEndPointStub.Object);

      var delegatingStrategy = new EndPointDelegatingDomainObjectCollectionData(endPointID, virtualEndPointProviderStub.Object);
      var associatedCollection = new OrderCollection(new ModificationCheckingDomainObjectCollectionDataDecorator(typeof(Order), delegatingStrategy));
      Assert.That(DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(associatedCollection), Is.SameAs(collectionEndPointStub.Object));
      return associatedCollection;
    }
  }
}
