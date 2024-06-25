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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.CollectionData
{
  [TestFixture]
  public class ReadOnlyDomainObjectCollectionDataDecoratorTest : ClientTransactionBaseTest
  {
    private ReadOnlyDomainObjectCollectionDataDecorator _readOnlyDecorator;
    private Mock<IDomainObjectCollectionData> _wrappedDataStub;

    private Order _order1;
    private Order _order3;
    private Order _order4;
    private Order _order5;

    public override void SetUp ()
    {
      base.SetUp();
      _wrappedDataStub = new Mock<IDomainObjectCollectionData>();
      _readOnlyDecorator = new ReadOnlyDomainObjectCollectionDataDecorator(_wrappedDataStub.Object);

      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _order3 = DomainObjectIDs.Order3.GetObject<Order>();
      _order4 = DomainObjectIDs.Order4.GetObject<Order>();
      _order5 = DomainObjectIDs.Order5.GetObject<Order>();
    }

    [Test]
    public void Enumeration ()
    {
      StubInnerData(_order1, _order3, _order4);
      Assert.That(_readOnlyDecorator.ToArray(), Is.EqualTo(new[] { _order1, _order3, _order4 }));
    }

    [Test]
    public void Count ()
    {
      StubInnerData(_order1, _order3, _order4);
      Assert.That(_readOnlyDecorator.Count, Is.EqualTo(3));
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That(_readOnlyDecorator.IsReadOnly, Is.True);
    }

    [Test]
    public void AssociatedEndPointID ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, typeof(Customer), "Orders");
      _wrappedDataStub.Setup(stub => stub.AssociatedEndPointID).Returns(endPointID);

      Assert.That(_readOnlyDecorator.AssociatedEndPointID, Is.SameAs(endPointID));
    }

    [Test]
    public void IsDataComplete ()
    {
      _wrappedDataStub.Setup(stub => stub.IsDataComplete).Returns(true);
      Assert.That(_readOnlyDecorator.IsDataComplete, Is.True);

      _wrappedDataStub.Reset();
      _wrappedDataStub.Setup(stub => stub.IsDataComplete).Returns(false);
      Assert.That(_readOnlyDecorator.IsDataComplete, Is.False);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _readOnlyDecorator.EnsureDataComplete();
      _wrappedDataStub.Verify(stub => stub.EnsureDataComplete(), Times.AtLeastOnce());
    }

    [Test]
    public void ContainsObjectID ()
    {
      StubInnerData(_order1, _order3, _order4);

      Assert.That(_readOnlyDecorator.ContainsObjectID(_order1.ID), Is.True);
      Assert.That(_readOnlyDecorator.ContainsObjectID(_order5.ID), Is.False);
    }

    [Test]
    public void GetObject_ByIndex ()
    {
      StubInnerData(_order1, _order3, _order4);

      Assert.That(_readOnlyDecorator.GetObject(0), Is.SameAs(_order1));
    }

    [Test]
    public void GetObject_ByID ()
    {
      StubInnerData(_order1, _order3, _order4);

      Assert.That(_readOnlyDecorator.GetObject(_order3.ID), Is.SameAs(_order3));
    }

    [Test]
    public void IndexOf ()
    {
      StubInnerData(_order1, _order3, _order4);

      Assert.That(_readOnlyDecorator.IndexOf(_order3.ID), Is.EqualTo(1));
    }

    [Test]
    public void Clear_Throws ()
    {
      Assert.That(
          () => _readOnlyDecorator.Clear(),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Cannot clear a read-only collection."));
    }

    [Test]
    public void Insert_Throws ()
    {
      Assert.That(
          () => _readOnlyDecorator.Insert(0, _order5),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot insert an item into a read-only collection."));
    }

    [Test]
    public void Remove_Throws ()
    {
      Assert.That(
          () => _readOnlyDecorator.Remove(_order1),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot remove an item from a read-only collection."));
    }

    [Test]
    public void Remove_ID_Throws ()
    {
      Assert.That(
          () => _readOnlyDecorator.Remove(_order1.ID),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot remove an item from a read-only collection."));
    }

    [Test]
    public void Replace_Throws ()
    {
      Assert.That(
          () => _readOnlyDecorator.Replace(1, _order1),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot replace an item in a read-only collection."));
    }

    [Test]
    public void Sort_Throws ()
    {
      Assert.That(
          () => _readOnlyDecorator.Sort((one, two) => 0),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Cannot sort a read-only collection."));
    }

    private void StubInnerData (params DomainObject[] contents)
    {
      _wrappedDataStub.Setup(stub => stub.Count).Returns(contents.Length);
      _wrappedDataStub.Setup(stub => stub.GetEnumerator()).Returns(((IEnumerable<DomainObject>)contents).GetEnumerator());

      for (int i = 0; i < contents.Length; i++)
      {
        int currentIndex = i; // required because Stub creates a closure
        _wrappedDataStub.Setup(stub => stub.ContainsObjectID(contents[currentIndex].ID)).Returns(true);
        _wrappedDataStub.Setup(stub => stub.GetObject(contents[currentIndex].ID)).Returns(contents[currentIndex]);
        _wrappedDataStub.Setup(stub => stub.GetObject(currentIndex)).Returns(contents[currentIndex]);
        _wrappedDataStub.Setup(stub => stub.IndexOf(contents[currentIndex].ID)).Returns(currentIndex);
      }
    }
  }
}
