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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.CollectionData
{
  [TestFixture]
  public class ReadOnlyCollectionDataDecoratorTest : ClientTransactionBaseTest
  {
    private ReadOnlyCollectionDataDecorator _readOnlyDecorator;
    private IDomainObjectCollectionData _wrappedDataStub;

    private Order _order1;
    private Order _order3;
    private Order _order4;
    private Order _order5;

    public override void SetUp ()
    {
      base.SetUp();
      _wrappedDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData>();
      _readOnlyDecorator = new ReadOnlyCollectionDataDecorator (_wrappedDataStub);

      _order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      _order3 = DomainObjectIDs.Order3.GetObject<Order> ();
      _order4 = DomainObjectIDs.Order4.GetObject<Order> ();
      _order5 = DomainObjectIDs.Order5.GetObject<Order> ();
    }

    [Test]
    public void Enumeration ()
    {
      StubInnerData (_order1, _order3, _order4);
      Assert.That (_readOnlyDecorator.ToArray(), Is.EqualTo (new[] { _order1, _order3, _order4 }));
    }

    [Test]
    public void Count ()
    {
      StubInnerData (_order1, _order3, _order4);
      Assert.That (_readOnlyDecorator.Count, Is.EqualTo (3));
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_readOnlyDecorator.IsReadOnly, Is.True);
    }

    [Test]
    public void AssociatedEndPointID ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders");
      _wrappedDataStub.Stub (stub => stub.AssociatedEndPointID).Return (endPointID);

      Assert.That (_readOnlyDecorator.AssociatedEndPointID, Is.SameAs (endPointID));
    }

    [Test]
    public void IsDataComplete ()
    {
      _wrappedDataStub.Stub (stub => stub.IsDataComplete).Return (true);
      Assert.That (_readOnlyDecorator.IsDataComplete, Is.True);

      _wrappedDataStub.BackToRecord();
      _wrappedDataStub.Stub (stub => stub.IsDataComplete).Return (false);
      Assert.That (_readOnlyDecorator.IsDataComplete, Is.False);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _readOnlyDecorator.EnsureDataComplete();
      _wrappedDataStub.AssertWasCalled (stub => stub.EnsureDataComplete());
    }

    [Test]
    public void ContainsObjectID ()
    {
      StubInnerData (_order1, _order3, _order4);

      Assert.That (_readOnlyDecorator.ContainsObjectID (_order1.ID), Is.True);
      Assert.That (_readOnlyDecorator.ContainsObjectID (_order5.ID), Is.False);
    }

    [Test]
    public void GetObject_ByIndex ()
    {
      StubInnerData (_order1, _order3, _order4);

      Assert.That (_readOnlyDecorator.GetObject (0), Is.SameAs (_order1));
    }

    [Test]
    public void GetObject_ByID ()
    {
      StubInnerData (_order1, _order3, _order4);

      Assert.That (_readOnlyDecorator.GetObject (_order3.ID), Is.SameAs (_order3));
    }

    [Test]
    public void IndexOf ()
    {
      StubInnerData (_order1, _order3, _order4);

      Assert.That (_readOnlyDecorator.IndexOf (_order3.ID), Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot clear a read-only collection.")]
    public void Clear_Throws ()
    {
      _readOnlyDecorator.Clear();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot insert an item into a read-only collection.")]
    public void Insert_Throws ()
    {
      _readOnlyDecorator.Insert (0, _order5);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot remove an item from a read-only collection.")]
    public void Remove_Throws ()
    {
      _readOnlyDecorator.Remove (_order1);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot remove an item from a read-only collection.")]
    public void Remove_ID_Throws ()
    {
      _readOnlyDecorator.Remove (_order1.ID);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot replace an item in a read-only collection.")]
    public void Replace_Throws ()
    {
      _readOnlyDecorator.Replace (1, _order1);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot sort a read-only collection.")]
    public void Sort_Throws ()
    {
      _readOnlyDecorator.Sort ((one, two) => 0);
    }

    [Test]
    public void Serializable ()
    {
      var decorator = new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData (new[] { _order1, _order3, _order4 }));
      var result = Serializer.SerializeAndDeserialize (decorator);
      Assert.That (result.Count, Is.EqualTo (3));
    }

    private void StubInnerData (params DomainObject[] contents)
    {
      _wrappedDataStub.Stub (stub => stub.Count).Return (contents.Length);
      _wrappedDataStub.Stub (stub => stub.GetEnumerator()).Return (((IEnumerable<DomainObject>) contents).GetEnumerator());

      for (int i = 0; i < contents.Length; i++)
      {
        int currentIndex = i; // required because Stub creates a closure
        _wrappedDataStub.Stub (stub => stub.ContainsObjectID (contents[currentIndex].ID)).Return (true);
        _wrappedDataStub.Stub (stub => stub.GetObject (contents[currentIndex].ID)).Return (contents[currentIndex]);
        _wrappedDataStub.Stub (stub => stub.GetObject (currentIndex)).Return (contents[currentIndex]);
        _wrappedDataStub.Stub (stub => stub.IndexOf (contents[currentIndex].ID)).Return (currentIndex);
      }
    }
  }
}