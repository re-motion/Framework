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
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class DomainObjectCollectionExtensionsTest : ClientTransactionBaseTest
  {
    private DomainObjectCollection _collection;
    private Customer _customer1;
    private Customer _customer2;
    private Customer _customer3NotInCollection;
    private DomainObject _customer1FromOtherTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _customer1 = DomainObjectIDs.Customer1.GetObject<Customer> ();
      _customer2 = DomainObjectIDs.Customer2.GetObject<Customer> ();
      _customer3NotInCollection = DomainObjectIDs.Customer3.GetObject<Customer> ();
      _customer1FromOtherTransaction = DomainObjectMother.GetObjectInOtherTransaction<Customer> (_customer1.ID);

      _collection = new DomainObjectCollection (typeof (Customer)) { _customer1, _customer2 };
    }

    [Test]
    public void CheckNotReadOnly_NotReadOnly ()
    {
      _collection.CheckNotReadOnly ("Test");
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Test")]
    public void CheckNotReadOnly_ReadOnly ()
    {
      var readOnlyCollection = _collection.Clone (true);
      readOnlyCollection.CheckNotReadOnly ("Test");
    }

    [Test]
    public void UnionWith ()
    {
      var secondCollection = _collection.Clone();
      secondCollection.Add (DomainObjectIDs.Customer3.GetObject<Customer> ());

      _collection.UnionWith (secondCollection);

      Assert.That (_collection.Count, Is.EqualTo (3));
      Assert.That (_collection.ContainsObject (_customer1), Is.True);
      Assert.That (_collection.ContainsObject (_customer2), Is.True);
      Assert.That (_collection.Contains (DomainObjectIDs.Customer3), Is.True);
      Assert.That (_collection.IsReadOnly, Is.False);
    }

    [Test]
    public void UnionWith_WithIdenticalID_AndDifferentReference ()
    {
      var secondCollection = new DomainObjectCollection ();
      secondCollection.Add (_customer1FromOtherTransaction);

      secondCollection.UnionWith (_collection);

      Assert.That (secondCollection, Is.EqualTo (new[] { _customer1FromOtherTransaction, _customer2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Item 0 of parameter 'domainObjects' has the type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' "
        + "instead of 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer'."
        + "\r\nParameter name: domainObjects")]
    public void UnionWith_ChecksItems ()
    {
      var secondCollection = new DomainObjectCollection();
      secondCollection.Add (DomainObjectIDs.Order1.GetObject<Order>());

      _collection.UnionWith (secondCollection);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "A read-only collection cannot be combined with another collection.")]
    public void UnionWith_ChecksNotReadOnly ()
    {
      var readOnlyCollection = _collection.Clone (true);
      readOnlyCollection.UnionWith (_collection);
    }

    [Test]
    public void GetItemsExcept ()
    {
      var exceptedDomainObjects = new HashSet<DomainObject> { _customer1 };

      var itemsNotInCollection = _collection.GetItemsExcept (exceptedDomainObjects);

      Assert.That (itemsNotInCollection.ToArray(), Is.EqualTo (new[] { _customer2 }));
    }

    [Test]
    public void SequenceEqual ()
    {
      Assert.That (_collection.SequenceEqual (new[] { _customer1, _customer2 }), Is.True);
      Assert.That (_collection.SequenceEqual (new[] { _customer2, _customer1 }), Is.False);
      Assert.That (_collection.SequenceEqual (new[] { _customer1, _customer2, _customer3NotInCollection }), Is.False);
    }

    [Test]
    public void SequenceEqual_UsesReferenceComparison ()
    {
      Assert.That (_collection.SequenceEqual (new[] { _customer1FromOtherTransaction, _customer2 }), Is.False);
    }

    [Test]
    public void SetEquals ()
    {
      Assert.That (_collection.SetEquals (new[] { _customer1, _customer2 }), Is.True);
      Assert.That (_collection.SetEquals (new[] { _customer2, _customer1 }), Is.True);
      Assert.That (_collection.SetEquals (new[] { _customer1, _customer2, _customer3NotInCollection }), Is.False);
    }

    [Test]
    public void SetEquals_UsesReferenceComparison ()
    {
      Assert.That (_collection.SetEquals (new[] { _customer1FromOtherTransaction, _customer2 }), Is.False);
    }

    [Test]
    public void SetEquals_HandlesDuplicates ()
    {
      Assert.That (_collection.SetEquals (new[] { _customer1, _customer2, _customer2, _customer1 }), Is.True);
    }

    [Test]
    public void AsList ()
    {
      var list = _collection.AsList<Customer> ();

      Assert.That (list, Is.InstanceOf (typeof (DomainObjectCollectionWrapper<Customer>)));
      Assert.That (((DomainObjectCollectionWrapper<Customer>) list).WrappedCollection, Is.SameAs (_collection));
    }

    [Test]
    public void AsReadOnlyCollection ()
    {
      var readOnlyCollection = _collection.AsReadOnlyCollection ();

      Assert.That (readOnlyCollection, Is.InstanceOf (typeof (ReadOnlyCollection<DomainObject>)));
      Assert.That (readOnlyCollection, Is.EqualTo (new[] { _customer1, _customer2 }));
    }

    [Test]
    public void AsReadOnlyCollection_ResultReflectsChangesToOriginalCollection ()
    {
      var readOnlyCollection = _collection.AsReadOnlyCollection ();

      Assert.That (readOnlyCollection, Is.EqualTo (new[] { _customer1, _customer2 }));

      _collection.Add (_customer3NotInCollection);
      Assert.That (readOnlyCollection, Is.EqualTo (new[] { _customer1, _customer2, _customer3NotInCollection }));
    }

    [Test]
    public void AsReadOnlyCollection_ObjectList ()
    {
      var objectList = new ObjectList<Customer> { _customer1, _customer2 };
      var readOnlyCollection = objectList.AsReadOnlyCollection ();

      Assert.That (readOnlyCollection, Is.InstanceOf (typeof (ReadOnlyCollection<Customer>)));
      Assert.That (readOnlyCollection, Is.EqualTo (new[] { _customer1, _customer2 }));
    }
  }
}