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
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BusinessObjectListAdapterTest
  {
    private Mock<IList> _listMock;
    private BusinessObjectListAdapter<IBusinessObject> _adapter;

    [SetUp]
    public void SetUp ()
    {
      _listMock = new Mock<IList>();
      _adapter = new BusinessObjectListAdapter<IBusinessObject>(_listMock.Object);
    }

    [Test]
    public void Count ()
    {
      _listMock.Setup(_ => _.Count).Returns(17);

      Assert.That(_adapter.Count, Is.EqualTo(17));
    }

    [Test]
    public void GetIndexer ()
    {
      var expected = new Mock<IBusinessObject>();
      _listMock.Setup(_ => _[17]).Returns(expected.Object);

      Assert.That(_adapter[17], Is.EqualTo(expected.Object));
    }

    [Test]
    public void GetEnumerator ()
    {
      var expected = new[] { new Mock<IBusinessObject>().Object, new Mock<IBusinessObject>().Object };
      _listMock.Setup(_ => _.GetEnumerator()).Returns(expected.GetEnumerator());

      Assert.That(_adapter, Is.EqualTo(expected));
    }

    [Test]
    public void GetEnumeratorForIList ()
    {
      var expected = new Mock<IEnumerator>();
      _listMock.Setup(_ => _.GetEnumerator()).Returns(expected.Object);

      Assert.That(((IList) _adapter).GetEnumerator(), Is.EqualTo(expected.Object));
    }

    [Test]
    public void CopyTo ()
    {
      Array array = new object[3];

      ((IList) _adapter).CopyTo(array, 13);

      _listMock.Verify(_ => _.CopyTo(array, 13));
    }

    [Test]
    public void IsSynchronized ()
    {
      var expected = BooleanObjectMother.GetRandomBoolean();
      _listMock.Setup(_ => _.IsSynchronized).Returns(expected);

      Assert.That(((IList) _adapter).IsSynchronized, Is.EqualTo(expected));
    }

    [Test]
    public void SyncRoot ()
    {
      var expected = new object();
      _listMock.Setup(_ => _.SyncRoot).Returns(expected);

      Assert.That(((IList) _adapter).SyncRoot, Is.EqualTo(expected));
    }

    [Test]
    public void Add ()
    {
      var expected = new object();
      _listMock.Setup(_ => _.Add(expected)).Returns(23);

      Assert.That(((IList) _adapter).Add(expected), Is.EqualTo(23));
    }

    [Test]
    public void Clear ()
    {
      ((IList) _adapter).Clear();

      _listMock.Verify(_ => _.Clear());
    }

    [Test]
    public void Contains ()
    {
      var expectedParameter = new object();
      var expectedResult = BooleanObjectMother.GetRandomBoolean();
      _listMock.Setup(_ => _.Contains(expectedParameter)).Returns(expectedResult);

      Assert.That(((IList) _adapter).Contains(expectedParameter), Is.EqualTo(expectedResult));
    }

    [Test]
    public void IndexOf ()
    {
      var expected = new object();
      _listMock.Setup(_ => _.IndexOf(expected)).Returns(7);

      Assert.That(((IList) _adapter).IndexOf(expected), Is.EqualTo(7));
    }

    [Test]
    public void Insert ()
    {
      var expected = new object();

      ((IList) _adapter).Insert(3, expected);

      _listMock.Verify(_ => _.Insert(3, expected));
    }

    [Test]
    public void Remove ()
    {
      var expected = new object();

      ((IList) _adapter).Remove(expected);

      _listMock.Verify(_ => _.Remove(expected));
    }

    [Test]
    public void RemoveAt ()
    {
      ((IList) _adapter).Remove(17);

      _listMock.Verify(_ => _.Remove(17));
    }

    [Test]
    public void IsFixedSize ()
    {
      var expected = BooleanObjectMother.GetRandomBoolean();
      _listMock.Setup(_ => _.IsFixedSize).Returns(expected);

      Assert.That(((IList) _adapter).IsFixedSize, Is.EqualTo(expected));
    }

    [Test]
    public void IsReadOnly ()
    {
      var expected = BooleanObjectMother.GetRandomBoolean();
      _listMock.Setup(_ => _.IsReadOnly).Returns(expected);

      Assert.That(((IList) _adapter).IsReadOnly, Is.EqualTo(expected));
    }

    [Test]
    public void GetIndexerForIList ()
    {
      var expected = BooleanObjectMother.GetRandomBoolean();
      _listMock.Setup(_ => _[17]).Returns(expected);

      Assert.That(((IList) _adapter)[17], Is.EqualTo(expected));
    }

    [Test]
    public void SetIndexer ()
    {
      var expected = BooleanObjectMother.GetRandomBoolean();

      ((IList) _adapter)[17] = expected;

      _listMock.VerifySet(_ => _[17] = expected);
    }
  }
}