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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ReadOnlyDomainObjectCollectionAdapterTest : ClientTransactionBaseTest
  {
    private ReadOnlyDomainObjectCollectionAdapter<DomainObject> _readOnlyAdapter;
    private IList<DomainObject> _readOnlyAdapterAsIList;
    private DomainObjectCollection _wrappedData;

    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;

    public override void SetUp ()
    {
      base.SetUp();
      _wrappedData = new DomainObjectCollection(typeof(Order));
      _readOnlyAdapter = new ReadOnlyDomainObjectCollectionAdapter<DomainObject>(_wrappedData);
      _readOnlyAdapterAsIList = _readOnlyAdapter;

      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _order2 = DomainObjectIDs.Order2.GetObject<Order>();
      _order3 = DomainObjectIDs.Order3.GetObject<Order>();
      _order4 = DomainObjectIDs.Order4.GetObject<Order>();
    }

    [Test]
    public void Enumeration ()
    {
      StubInnerData(_order1, _order2, _order3);
      Assert.That(_readOnlyAdapter.ToArray(), Is.EqualTo(new[] { _order1, _order2, _order3 }));

      var nonGenericEnumerableAdapter = (IEnumerable)_readOnlyAdapter;
      var enumerator = nonGenericEnumerableAdapter.GetEnumerator();
      Assert.That(enumerator.MoveNext(), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(_order1));
      Assert.That(enumerator.MoveNext(), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(_order2));
      Assert.That(enumerator.MoveNext(), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(_order3));
      Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void RequiredItemType ()
    {
      Assert.That(_readOnlyAdapter.RequiredItemType, Is.SameAs(typeof(Order)));
    }

    [Test]
    public void AssociatedEndPointID ()
    {
      var associatedCollection = DomainObjectIDs.Order1.GetObject<Order>().OrderItems;
      var readOnlyAdapter = new ReadOnlyDomainObjectCollectionAdapter<DomainObject>(associatedCollection);

      Assert.That(readOnlyAdapter.AssociatedEndPointID, Is.EqualTo(associatedCollection.AssociatedEndPointID));
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That(_readOnlyAdapter.IsDataComplete, Is.True);

      var associatedCollection = DomainObjectIDs.Order1.GetObject<Order>().OrderItems;
      UnloadService.UnloadVirtualEndPoint(
          TestableClientTransaction,
          associatedCollection.AssociatedEndPointID);

      var readOnlyAdapter = new ReadOnlyDomainObjectCollectionAdapter<DomainObject>(associatedCollection);
      Assert.That(readOnlyAdapter.IsDataComplete, Is.False);
    }

    [Test]
    public void Count ()
    {
      StubInnerData(_order1, _order2, _order3);
      Assert.That(_readOnlyAdapter.Count, Is.EqualTo(3));
    }

    [Test]
    public void Contains_ID ()
    {
      StubInnerData(_order1, _order2, _order3);

      Assert.That(_readOnlyAdapter.Contains(_order1.ID), Is.True);
      Assert.That(_readOnlyAdapter.Contains(_order4.ID), Is.False);
    }

    [Test]
    public void ContainsObject ()
    {
      StubInnerData(_order1, _order2, _order3);

      Assert.That(_readOnlyAdapter.ContainsObject(_order1), Is.True);
      Assert.That(_readOnlyAdapter.ContainsObject(_order4), Is.False);
    }

    [Test]
    public void Item_ByIndex ()
    {
      StubInnerData(_order1, _order2, _order3);

      Assert.That(_readOnlyAdapter[0], Is.SameAs(_order1));
    }

    [Test]
    public void Item_ByID ()
    {
      StubInnerData(_order1, _order2, _order3);

      Assert.That(_readOnlyAdapter[_order2.ID], Is.SameAs(_order2));
    }

    [Test]
    public void IndexOf_ID ()
    {
      StubInnerData(_order1, _order2, _order3);

      Assert.That(_readOnlyAdapter.IndexOf(_order2.ID), Is.EqualTo(1));
    }

    [Test]
    public void IndexOf_Object ()
    {
      StubInnerData(_order1, _order2, _order3);

      Assert.That(_readOnlyAdapter.IndexOf(_order2), Is.EqualTo(1));
    }

    [Test]
    public void GetObject ()
    {
      StubInnerData(_order1, _order2, _order3);

      Assert.That(_readOnlyAdapter.GetObject(_order2.ID), Is.SameAs(_order2));
    }

    [Test]
    public void IList_Contains ()
    {
      StubInnerData(_order1, _order2);

      Assert.That(_readOnlyAdapterAsIList.Contains(_order2), Is.True);
      Assert.That(_readOnlyAdapterAsIList.Contains(_order3), Is.False);
    }

    [Test]
    public void IList_Item_Get ()
    {
      StubInnerData(_order1, _order2);
      var result = _readOnlyAdapterAsIList[0];

      Assert.That(result, Is.SameAs(_order1));
    }

    private void StubInnerData (params DomainObject[] contents)
    {
      _wrappedData.AddRange(contents);
    }
  }
}
