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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DataContainerCollectionTest : ClientTransactionBaseTest
  {
    private DataContainer _dataContainer;
    private DataContainerCollection _collection;

    public override void SetUp ()
    {
      base.SetUp();

      _dataContainer = TestDataContainerObjectMother.CreateOrder1DataContainer();
      _collection = new DataContainerCollection();
    }

    [Test]
    public void Add ()
    {
      _collection.Add(_dataContainer);
      Assert.That(_collection.Count, Is.EqualTo(1));
    }

    [Test]
    public void Add_Generic ()
    {
      ((IList<DataContainer>)_collection).Add(_dataContainer);
      Assert.That(_collection.Count, Is.EqualTo(1));
    }

    [Test]
    public void GetEnumerator_Generic ()
    {
      _collection.Add(_dataContainer);

      using (var enumerator = _collection.GetEnumerator())
      {
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.SameAs(_dataContainer));
        Assert.That(enumerator.MoveNext(), Is.False);
      }
    }

    [Test]
    public void GetEnumerator_Generic_ChecksVersion ()
    {
      _collection.Add(_dataContainer);

      Assert.That(
          () =>
          {
            foreach (var dataContainer in _collection)
              _collection.Remove(dataContainer.ID);
          },
          Throws.InvalidOperationException
              .With.Message.EqualTo("Collection was modified during enumeration."));
    }

    [Test]
    public void Item_Get_ObjectID ()
    {
      _collection.Add(_dataContainer);
      Assert.That(_collection[_dataContainer.ID], Is.SameAs(_dataContainer));
    }

    [Test]
    public void Item_Get_Index ()
    {
      _collection.Add(_dataContainer);
      Assert.That(_collection[0], Is.SameAs(_dataContainer));
      Assert.That(((IList<DataContainer>)_collection)[0], Is.SameAs(_dataContainer));
    }

    [Test]
    public void Item_Set_Index ()
    {
      _collection.Add(_dataContainer);
      Assert.That(
          () => ((IList<DataContainer>)_collection)[0] = _dataContainer,
          Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void ContainsObjectIDTrue ()
    {
      _collection.Add(_dataContainer);
      Assert.That(_collection.Contains(_dataContainer.ID), Is.True);
    }

    [Test]
    public void ContainsObjectIDFalse ()
    {
      Assert.That(_collection.Contains(_dataContainer.ID), Is.False);
    }

    [Test]
    public void CopyConstructor ()
    {
      _collection.Add(_dataContainer);

      DataContainerCollection copiedCollection = new DataContainerCollection(_collection, false);

      Assert.That(copiedCollection.Count, Is.EqualTo(1));
      Assert.That(copiedCollection[0], Is.SameAs(_dataContainer));
    }

    [Test]
    public void GetEmptyDifference ()
    {
      DataContainerCollection difference = _collection.GetDifference(new DataContainerCollection());
      Assert.That(difference.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetDifferenceFromEmptySet ()
    {
      _collection.Add(_dataContainer);
      DataContainerCollection difference = _collection.GetDifference(new DataContainerCollection());
      Assert.That(difference.Count, Is.EqualTo(1));
      Assert.That(difference[0], Is.SameAs(_dataContainer));
    }

    [Test]
    public void GetDifference ()
    {
      DataContainer differentDataContainer = TestDataContainerObjectMother.CreateOrder2DataContainer();

      _collection.Add(_dataContainer);
      _collection.Add(differentDataContainer);

      DataContainerCollection secondCollection = new DataContainerCollection();

      secondCollection.Add(_dataContainer);

      DataContainerCollection difference = _collection.GetDifference(secondCollection);

      Assert.That(difference.Count, Is.EqualTo(1));
      Assert.That(difference[0], Is.SameAs(differentDataContainer));
    }

    [Test]
    public void EmptyMerge ()
    {
      DataContainerCollection mergedCollection = _collection.Merge(new DataContainerCollection());
      Assert.That(mergedCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void MergeCollectionAndEmptyCollection ()
    {
      _collection.Add(_dataContainer);
      DataContainerCollection mergedCollection = _collection.Merge(new DataContainerCollection());

      Assert.That(mergedCollection.Count, Is.EqualTo(1));
      Assert.That(mergedCollection[0], Is.SameAs(_dataContainer));
    }

    [Test]
    public void MergeEmptyCollectionAndCollection ()
    {
      DataContainerCollection secondCollection = new DataContainerCollection();
      secondCollection.Add(_dataContainer);

      DataContainerCollection mergedCollection = _collection.Merge(secondCollection);

      Assert.That(mergedCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void MergeTwoCollectionsWithEqualDataContainer ()
    {
      _collection.Add(_dataContainer);

      DataContainerCollection secondCollection = new DataContainerCollection();
      DataContainer container = TestDataContainerObjectMother.CreateOrder1DataContainer();
      secondCollection.Add(container);

      DataContainerCollection mergedCollection = _collection.Merge(secondCollection);

      Assert.That(mergedCollection.Count, Is.EqualTo(1));
      Assert.That(mergedCollection[0], Is.SameAs(container));
    }

    [Test]
    public void MergeTwoCollections ()
    {
      _collection.Add(_dataContainer);
      DataContainer order3 = TestDataContainerObjectMother.CreateOrder2DataContainer();
      _collection.Add(order3);

      DataContainerCollection secondCollection = new DataContainerCollection();
      DataContainer order1 = TestDataContainerObjectMother.CreateOrder1DataContainer();
      secondCollection.Add(order1);

      DataContainerCollection mergedCollection = _collection.Merge(secondCollection);

      Assert.That(mergedCollection.Count, Is.EqualTo(2));
      Assert.That(mergedCollection[_dataContainer.ID], Is.SameAs(order1));
      Assert.That(mergedCollection[order3.ID], Is.SameAs(order3));
    }

    [Test]
    public void RemoveByID ()
    {
      _collection.Add(_dataContainer);
      Assert.That(_collection.Count, Is.EqualTo(1));

      _collection.Remove(_dataContainer.ID);
      Assert.That(_collection.Count, Is.EqualTo(0));

      _collection.Remove(_dataContainer.ID);
      Assert.That(_collection.Count, Is.EqualTo(0));
    }

    [Test]
    public void RemoveByDataContainer ()
    {
      _collection.Add(_dataContainer);
      Assert.That(_collection.Count, Is.EqualTo(1));

      var result = _collection.Remove(_dataContainer);
      Assert.That(result, Is.True);
      Assert.That(_collection.Count, Is.EqualTo(0));

      result = _collection.Remove(_dataContainer);
      Assert.That(result, Is.False);
      Assert.That(_collection.Count, Is.EqualTo(0));
    }

    [Test]
    public void RemoveAt ()
    {
      _collection.Add(_dataContainer);
      Assert.That(_collection.Count, Is.EqualTo(1));

      _collection.RemoveAt(0);
      Assert.That(_collection.Count, Is.EqualTo(0));
    }

    [Test]
    public void RemoveNullDataContainer ()
    {
      Assert.That(
          () => _collection.Remove((DataContainer)null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void RemoveNullObjectID ()
    {
      Assert.That(
          () => _collection.Remove((ObjectID)null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void ContainsDataContainerTrue ()
    {
      _collection.Add(_dataContainer);

      Assert.That(_collection.Contains(_dataContainer), Is.True);
    }

    [Test]
    public void ContainsDataContainerFalse ()
    {
      _collection.Add(_dataContainer);

      DataContainer copy = DataContainer.CreateNew(_dataContainer.ID);

      Assert.That(_collection.Contains(copy), Is.False);
    }

    [Test]
    public void ContainsDataContainerNull ()
    {
      Assert.That(
          () => _collection.Contains((DataContainer)null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Clear ()
    {
      _collection.Add(_dataContainer);
      Assert.That(_collection.Count, Is.EqualTo(1));

      _collection.Clear();
      Assert.That(_collection.Count, Is.EqualTo(0));
    }

    [Test]
    public void Join ()
    {
      DataContainer firstDataContainer = TestDataContainerObjectMother.CreateOrder1DataContainer();
      DataContainerCollection firstCollection = new DataContainerCollection();
      firstCollection.Add(firstDataContainer);

      DataContainer secondDataContainer = TestDataContainerObjectMother.CreateOrder2DataContainer();
      DataContainerCollection secondCollection = new DataContainerCollection();
      secondCollection.Add(secondDataContainer);

      DataContainerCollection joinedCollection = DataContainerCollection.Join(firstCollection, secondCollection);
      Assert.That(joinedCollection.Count, Is.EqualTo(2));
      Assert.That(joinedCollection[0].ID, Is.EqualTo(firstDataContainer.ID));
      Assert.That(joinedCollection[1].ID, Is.EqualTo(secondDataContainer.ID));
    }

    [Test]
    public void JoinWithSameDataContainer ()
    {
      DataContainer dataContainer = TestDataContainerObjectMother.CreateOrder1DataContainer();
      DataContainerCollection firstCollection = new DataContainerCollection();
      firstCollection.Add(dataContainer);

      DataContainerCollection secondCollection = new DataContainerCollection();
      secondCollection.Add(dataContainer);

      DataContainerCollection joinedCollection = DataContainerCollection.Join(firstCollection, secondCollection);
      Assert.That(joinedCollection.Count, Is.EqualTo(1));
      Assert.That(joinedCollection[0].ID, Is.EqualTo(dataContainer.ID));
    }

    [Test]
    public void JoinWithDataContainersOfSameID ()
    {
      DataContainer dataContainer = TestDataContainerObjectMother.CreateOrder1DataContainer();
      DataContainerCollection firstCollection = new DataContainerCollection();
      firstCollection.Add(dataContainer);

      DataContainerCollection secondCollection = new DataContainerCollection();
      secondCollection.Add(TestDataContainerObjectMother.CreateOrder1DataContainer());

      DataContainerCollection joinedCollection = DataContainerCollection.Join(firstCollection, secondCollection);
      Assert.That(joinedCollection.Count, Is.EqualTo(1));
      Assert.That(joinedCollection[0], Is.SameAs(dataContainer));
    }

    [Test]
    public void CopyTo_Generic ()
    {
      var destArray = new DataContainer[] { null, null, null };
      _collection.Add(_dataContainer);

      ((IList<DataContainer>)_collection).CopyTo(destArray, 1);

      Assert.That(destArray, Is.EqualTo(new[] { null, _dataContainer, null }));
    }

    [Test]
    public void IndexOf_Generic ()
    {
      Assert.That(_collection.IndexOf(_dataContainer), Is.EqualTo(-1));

      _collection.Add(_dataContainer);
      Assert.That(_collection.IndexOf(_dataContainer), Is.EqualTo(0));
    }

    [Test]
    public void Insert ()
    {
      _collection.Insert(0, _dataContainer);

      Assert.That(_collection, Is.EqualTo(new[] { _dataContainer }));
    }
  }
}
