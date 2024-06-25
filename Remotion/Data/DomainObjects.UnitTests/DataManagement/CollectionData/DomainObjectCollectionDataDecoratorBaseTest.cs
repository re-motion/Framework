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
  public class DomainObjectCollectionDataDecoratorBaseTest : StandardMappingTest
  {
    private Mock<IDomainObjectCollectionData> _wrappedDataMock;
    private TestDomainObjectCollectionDecorator _decorator;


    private Order _order1;
    private Order _order3;

    public override void SetUp ()
    {
      base.SetUp();

      _wrappedDataMock = new Mock<IDomainObjectCollectionData>(MockBehavior.Strict);

      _decorator = new TestDomainObjectCollectionDecorator(_wrappedDataMock.Object);

      _order1 = DomainObjectMother.CreateFakeObject<Order>();
      _order3 = DomainObjectMother.CreateFakeObject<Order>();
    }

    [Test]
    public void WrappedData ()
    {
      Assert.That(_decorator.WrappedData, Is.SameAs(_wrappedDataMock.Object));
      var newData = new DomainObjectCollectionData(new[] { _order1 });

      _decorator.WrappedData = newData;
      Assert.That(_decorator.WrappedData, Is.SameAs(newData));

      Assert.That(_decorator.ToArray(), Is.EqualTo(newData.ToArray()));
    }

    [Test]
    public void GetEnumerator ()
    {
      using (IEnumerator<DomainObject> fakeEnumerator = new List<DomainObject>().GetEnumerator())
      {
        _wrappedDataMock.Setup(mock => mock.GetEnumerator()).Returns(fakeEnumerator).Verifiable();

        var result = _decorator.GetEnumerator();

        _wrappedDataMock.Verify();
        Assert.That(result, Is.SameAs(fakeEnumerator));
      }
    }

    [Test]
    public void Count ()
    {
      _wrappedDataMock.Setup(mock => mock.Count).Returns(7).Verifiable();

      var result = _decorator.Count;

      _wrappedDataMock.Verify();
      Assert.That(result, Is.EqualTo(7));
    }

    [Test]
    public void IsReadOnly ()
    {
      _wrappedDataMock.Setup(mock => mock.IsReadOnly).Returns(true).Verifiable();

      var result = _decorator.IsReadOnly;

      _wrappedDataMock.Verify();
      Assert.That(result, Is.EqualTo(true));
    }

    [Test]
    public void AssociatedEndPointID ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, typeof(Customer), "Orders");

      _wrappedDataMock.Setup(mock => mock.AssociatedEndPointID).Returns(endPointID).Verifiable();

      var result = _decorator.AssociatedEndPointID;

      _wrappedDataMock.Verify();
      Assert.That(result, Is.SameAs(endPointID));
    }

    [Test]
    public void IsDataComplete ()
    {
      _wrappedDataMock.Setup(mock => mock.IsDataComplete).Returns(true).Verifiable();

      var result = _decorator.IsDataComplete;

      _wrappedDataMock.Verify();
      Assert.That(result, Is.EqualTo(true));
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _wrappedDataMock.Setup(mock => mock.EnsureDataComplete()).Verifiable();

      _decorator.EnsureDataComplete();

      _wrappedDataMock.Verify();
    }

    [Test]
    public void ContainsObjectID ()
    {
      _wrappedDataMock.Setup(mock => mock.ContainsObjectID(_order1.ID)).Returns(true).Verifiable();

      var result = _decorator.ContainsObjectID(_order1.ID);

      _wrappedDataMock.Verify();
      Assert.That(result, Is.EqualTo(true));
    }

    [Test]
    public void GetObject_ByIndex ()
    {
      _wrappedDataMock.Setup(mock => mock.GetObject(1)).Returns(_order1).Verifiable();

      var result = _decorator.GetObject(1);

      _wrappedDataMock.Verify();
      Assert.That(result, Is.SameAs(_order1));
    }

    [Test]
    public void GetObject_ByID ()
    {
      _wrappedDataMock.Setup(mock => mock.GetObject(_order3.ID)).Returns(_order3).Verifiable();

      var result = _decorator.GetObject(_order3.ID);

      _wrappedDataMock.Verify();
      Assert.That(result, Is.SameAs(_order3));
    }

    [Test]
    public void IndexOf ()
    {
      _wrappedDataMock.Setup(mock => mock.IndexOf(_order3.ID)).Returns(47).Verifiable();

      var result = _decorator.IndexOf(_order3.ID);

      _wrappedDataMock.Verify();
      Assert.That(result, Is.EqualTo(47));
    }

    [Test]
    public void Clear ()
    {
      _wrappedDataMock.Setup(mock => mock.Clear()).Verifiable();

      _decorator.Clear();

      _wrappedDataMock.Verify();
    }

    [Test]
    public void Insert ()
    {
      _wrappedDataMock.Setup(mock => mock.Insert(13, _order1)).Verifiable();

      _decorator.Insert(13, _order1);

      _wrappedDataMock.Verify();
    }

    [Test]
    public void Remove ()
    {
      _wrappedDataMock.Setup(mock => mock.Remove(_order3)).Returns(false).Verifiable();

      var result = _decorator.Remove(_order3);

      _wrappedDataMock.Verify();
      Assert.That(result, Is.EqualTo(false));
    }

    [Test]
    public void Remove_ID ()
    {
      _wrappedDataMock.Setup(mock => mock.Remove(_order3.ID)).Returns(false).Verifiable();

      var result = _decorator.Remove(_order3.ID);

      _wrappedDataMock.Verify();
      Assert.That(result, Is.EqualTo(false));
    }

    [Test]
    public void Replace ()
    {
      _wrappedDataMock.Setup(mock => mock.Replace(10, _order3)).Verifiable();

      _decorator.Replace(10, _order3);

      _wrappedDataMock.Verify();
    }

    [Test]
    public void Sort ()
    {
      Comparison<DomainObject> comparison = (one, two) => 0;
      _wrappedDataMock.Setup(mock => mock.Sort(comparison)).Verifiable();

      _decorator.Sort(comparison);

      _wrappedDataMock.Verify();
    }
  }
}
