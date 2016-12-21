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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class DomainObjectCollectionWrapperTest : ClientTransactionBaseTest
  {
    private Customer _customer1;
    private Customer _customer2;
    private Customer _customer3NotInCollection;

    private DomainObjectCollection _wrappedCollection;
    private DomainObjectCollectionWrapper<Customer> _wrapper;

    public override void SetUp ()
    {
      base.SetUp ();

      _customer1 = DomainObjectIDs.Customer1.GetObject<Customer> ();
      _customer2 = DomainObjectIDs.Customer2.GetObject<Customer> ();
      _customer3NotInCollection = DomainObjectIDs.Customer3.GetObject<Customer> ();

      _wrappedCollection = new DomainObjectCollection (new[] { _customer1, _customer2 }, typeof (Customer));
      _wrapper = new DomainObjectCollectionWrapper<Customer> (_wrappedCollection);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_wrapper.WrappedCollection, Is.SameAs (_wrappedCollection));
    }

    [Test]
    public void Initialization_TypeCanBeMoreGeneral ()
    {
      var wrapper = new DomainObjectCollectionWrapper<DomainObject> (_wrappedCollection);
      Assert.That (wrapper.WrappedCollection, Is.SameAs (_wrappedCollection));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Cannot implement 'IList<Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer>' for a DomainObjectCollection with required item type "
        + "'Remotion.Data.DomainObjects.DomainObject'. The IList<T>'s item type must be assignable from the required item type.\r\n"
        + "Parameter name: wrappedCollection")]
    public void Initialization_TypeCannotBeMoreRestricted ()
    {
      var wrappedCollection = new DomainObjectCollection (typeof (DomainObject));
      new DomainObjectCollectionWrapper<Customer> (wrappedCollection);
    }

    [Test]
    public void Initialization_NoRequiredItemType_DomainObjectWorks ()
    {
      var wrappedCollection = new DomainObjectCollection ();
      var wrapper = new DomainObjectCollectionWrapper<DomainObject> (wrappedCollection);
      Assert.That (wrapper.WrappedCollection, Is.SameAs (wrappedCollection));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot implement 'IList<Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer>' for a DomainObjectCollection with required item type "
        + "'Remotion.Data.DomainObjects.DomainObject'. The IList<T>'s item type must be assignable from the required item type.\r\n"
        + "Parameter name: wrappedCollection")]
    public void Initialization_NoRequiredItemType_DerivedTypeThrows ()
    {
      var wrappedCollection = new DomainObjectCollection ();
      new DomainObjectCollectionWrapper<Customer> (wrappedCollection);
    }

    [Test]
    public void Count ()
    {
      Assert.That (_wrapper.Count, Is.EqualTo (2));
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_wrapper.IsReadOnly, Is.False);

      var readOnlyCollection = new DomainObjectCollection ().Clone (true);
      var readOnlyWrapper = new DomainObjectCollectionWrapper<DomainObject> (readOnlyCollection);
      Assert.That (readOnlyWrapper.IsReadOnly, Is.True);
    }

    [Test]
    public void Item_Get ()
    {
      Assert.That (_wrapper[0], Is.SameAs (_customer1));
    }

    [Test]
    public void Item_Set ()
    {
      _wrapper[1] = _customer3NotInCollection;
      Assert.That (_wrappedCollection[1], Is.SameAs (_customer3NotInCollection));
    }

    [Test]
    public void GetEnumerator ()
    {
      var enumerator = _wrapper.GetEnumerator ();

      Assert.That (enumerator.MoveNext (), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (_customer1));

      Assert.That (enumerator.MoveNext (), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (_customer2));

      Assert.That (enumerator.MoveNext (), Is.False);
    }

    [Test]
    public void Contains ()
    {
      Assert.That (_wrapper.Contains (_customer1), Is.True);
      Assert.That (_wrapper.Contains (_customer3NotInCollection), Is.False);
    }

    [Test]
    public void IndexOf ()
    {
      Assert.That (_wrapper.IndexOf (_customer2), Is.EqualTo (1));
    }

    [Test]
    public void Insert ()
    {
      _wrapper.Insert (1, _customer3NotInCollection);

      Assert.That (_wrappedCollection, Is.EqualTo (new[] { _customer1, _customer3NotInCollection, _customer2 }));
    }

    [Test]
    public void RemoveAt ()
    {
      _wrapper.RemoveAt (1);

      Assert.That (_wrappedCollection, Is.EqualTo (new[] { _customer1 }));
    }

    [Test]
    public void Add ()
    {
      _wrapper.Add (_customer3NotInCollection);

      Assert.That (_wrappedCollection, Is.EqualTo (new[] { _customer1, _customer2, _customer3NotInCollection }));
    }

    [Test]
    public void Clear ()
    {
      _wrapper.Clear ();

      Assert.That (_wrappedCollection, Is.Empty);
    }

    [Test]
    public void Remove ()
    {
      var result = _wrapper.Remove (_customer2);

      Assert.That (_wrappedCollection, Is.EqualTo (new[] { _customer1 }));
      Assert.That (result, Is.True);

      result = _wrapper.Remove (_customer2);

      Assert.That (result, Is.False);
    }

    [Test]
    public void CopyTo ()
    {
      var array = new Customer[5];
      _wrapper.CopyTo (array, 1);

      Assert.That (array, Is.EqualTo (new[] { null, _customer1, _customer2, null, null }));
    }

  }
}