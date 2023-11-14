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
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UnitTests.Domain;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocListManageRowsInUnboundListTest : BocTest
  {
    private BocListMock _bocList;

    private IBusinessObject[] _values;
    private IBusinessObject[] _newValues;

    private BindableObjectClass _typeWithStringClass;

    private IBusinessObjectPropertyPath _typeWithStringFirstValuePath;
    private IBusinessObjectPropertyPath _typeWithStringSecondValuePath;

    private BocSimpleColumnDefinition _typeWithStringFirstValueSimpleColumn;
    private BocSimpleColumnDefinition _typeWithStringSecondValueSimpleColumn;
    private Mock<IReadOnlyList<IBusinessObject>> _valuesAsIReadOnlyListStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _values = new IBusinessObject[5];
      _values[0] = (IBusinessObject)TypeWithString.Create("0", "A");
      _values[1] = (IBusinessObject)TypeWithString.Create("1", "A");
      _values[2] = (IBusinessObject)TypeWithString.Create("2", "B");
      _values[3] = (IBusinessObject)TypeWithString.Create("3", "B");
      _values[4] = (IBusinessObject)TypeWithString.Create("4", "C");

      _newValues = new IBusinessObject[2];
      _newValues[0] = (IBusinessObject)TypeWithString.Create("5", "C");
      _newValues[1] = (IBusinessObject)TypeWithString.Create("6", "D");

      _valuesAsIReadOnlyListStub = new Mock<IReadOnlyList<IBusinessObject>>();
      _valuesAsIReadOnlyListStub.Setup(_ => _.GetEnumerator()).Returns(((IEnumerable<IBusinessObject>)_values).GetEnumerator());
      _valuesAsIReadOnlyListStub.Setup(_ => _.Count).Returns(_values.Length);
      _valuesAsIReadOnlyListStub.Setup(_ => _[It.IsAny<int>()]).Returns((int i) => _values[i]);

      _typeWithStringClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(TypeWithString));

      _typeWithStringFirstValuePath = BusinessObjectPropertyPath.CreateStatic(_typeWithStringClass, "FirstValue");
      _typeWithStringSecondValuePath = BusinessObjectPropertyPath.CreateStatic(_typeWithStringClass, "SecondValue");

      _typeWithStringFirstValueSimpleColumn = new BocSimpleColumnDefinition();
      _typeWithStringFirstValueSimpleColumn.SetPropertyPath(_typeWithStringFirstValuePath);

      _typeWithStringSecondValueSimpleColumn = new BocSimpleColumnDefinition();
      _typeWithStringSecondValueSimpleColumn.SetPropertyPath(_typeWithStringSecondValuePath);

      _bocList = new BocListMock();
      _bocList.ID = "BocList";
      NamingContainer.Controls.Add(_bocList);
    }

    [Test]
    public void AddRow ()
    {
      _bocList.LoadUnboundValue(_values, false);

      int index = _bocList.AddRow(_newValues[0]);

      Assert.That(ReferenceEquals(_values, _bocList.Value), Is.False);
      Assert.That(_bocList.Value.Count, Is.EqualTo(6));
      Assert.That(_bocList.Value[0], Is.SameAs(_values[0]));
      Assert.That(_bocList.Value[1], Is.SameAs(_values[1]));
      Assert.That(_bocList.Value[2], Is.SameAs(_values[2]));
      Assert.That(_bocList.Value[3], Is.SameAs(_values[3]));
      Assert.That(_bocList.Value[4], Is.SameAs(_values[4]));

      Assert.That(index, Is.EqualTo(5));
      Assert.That(_bocList.Value[5], Is.SameAs(_newValues[0]));
    }

    [Test]
    public void AddRowWithoutValue ()
    {
      _bocList.LoadUnboundValue(_values, false);

      _bocList.LoadUnboundValue(null, false);
      Assert.That(
          () => _bocList.AddRow(_newValues[0]),
          Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void AddRowWithValueAsIReadOnlyList ()
    {
      _bocList.LoadUnboundValue(_valuesAsIReadOnlyListStub.Object, false);

      Assert.That(
          () => _bocList.AddRow(_newValues[0]),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The BocList is bound to a collection that does not implement the IList interface. "
                  + "Add and remove rows is not supported for collections that do not implement the IList interface."));
    }

    [Test]
    public void AddRows ()
    {
      _bocList.LoadUnboundValue(_values, false);

      _bocList.AddRows(_newValues);

      Assert.That(ReferenceEquals(_values, _bocList.Value), Is.False);
      Assert.That(_bocList.Value.Count, Is.EqualTo(7));
      Assert.That(_bocList.Value[0], Is.SameAs(_values[0]));
      Assert.That(_bocList.Value[1], Is.SameAs(_values[1]));
      Assert.That(_bocList.Value[2], Is.SameAs(_values[2]));
      Assert.That(_bocList.Value[3], Is.SameAs(_values[3]));
      Assert.That(_bocList.Value[4], Is.SameAs(_values[4]));

      Assert.That(_bocList.Value[5], Is.SameAs(_newValues[0]));
      Assert.That(_bocList.Value[6], Is.SameAs(_newValues[1]));
    }

    [Test]
    public void AddRowsWithoutValue ()
    {
      _bocList.LoadUnboundValue(_values, false);

      _bocList.LoadUnboundValue(null, false);
      Assert.That(
          () => _bocList.AddRows(_newValues),
          Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void AddRowsWithValueAsIReadOnlyList ()
    {
      _bocList.LoadUnboundValue(_valuesAsIReadOnlyListStub.Object, false);

      Assert.That(
          () => _bocList.AddRows(_newValues),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The BocList is bound to a collection that does not implement the IList interface. "
                  + "Add and remove rows is not supported for collections that do not implement the IList interface."));
    }

    [Test]
    public void RemoveRowWithIndex ()
    {
      _bocList.LoadUnboundValue(_values, false);

      _bocList.RemoveRow(2);

      Assert.That(ReferenceEquals(_values, _bocList.Value), Is.False);
      Assert.That(_bocList.Value.Count, Is.EqualTo(4));
      Assert.That(_bocList.Value[0], Is.SameAs(_values[0]));
      Assert.That(_bocList.Value[1], Is.SameAs(_values[1]));
      Assert.That(_bocList.Value[2], Is.SameAs(_values[3]));
      Assert.That(_bocList.Value[3], Is.SameAs(_values[4]));
    }

    [Test]
    public void RowRowWithIndexAndWithValueAsIReadOnlyList ()
    {
      _bocList.LoadUnboundValue(_valuesAsIReadOnlyListStub.Object, false);

      Assert.That(
          () => _bocList.RemoveRow(2),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The BocList is bound to a collection that does not implement the IList interface. "
                  + "Add and remove rows is not supported for collections that do not implement the IList interface."));
    }

    [Test]
    public void RemoveRowWithBusinessObject ()
    {
      _bocList.LoadUnboundValue(_values, false);

      _bocList.RemoveRow(_values[2]);

      Assert.That(ReferenceEquals(_values, _bocList.Value), Is.False);
      Assert.That(_bocList.Value.Count, Is.EqualTo(4));
      Assert.That(_bocList.Value[0], Is.SameAs(_values[0]));
      Assert.That(_bocList.Value[1], Is.SameAs(_values[1]));
      Assert.That(_bocList.Value[2], Is.SameAs(_values[3]));
      Assert.That(_bocList.Value[3], Is.SameAs(_values[4]));
    }

    [Test]
    public void RowRowWithValueAsIReadOnlyList ()
    {
      _bocList.LoadUnboundValue(_valuesAsIReadOnlyListStub.Object, false);

      Assert.That(
          () => _bocList.RemoveRow(_values[2]),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The BocList is bound to a collection that does not implement the IList interface. "
                  + "Add and remove rows is not supported for collections that do not implement the IList interface."));
    }

    [Test]
    public void RemoveRowsWithNoRows ()
    {
      _bocList.LoadUnboundValue(_values, false);

      _bocList.RemoveRows(new IBusinessObject[0]);

      Assert.That(ReferenceEquals(_values, _bocList.Value), Is.False);
      Assert.That(_bocList.Value.Count, Is.EqualTo(5));
      Assert.That(_bocList.Value[0], Is.SameAs(_values[0]));
      Assert.That(_bocList.Value[1], Is.SameAs(_values[1]));
      Assert.That(_bocList.Value[2], Is.SameAs(_values[2]));
      Assert.That(_bocList.Value[3], Is.SameAs(_values[3]));
      Assert.That(_bocList.Value[4], Is.SameAs(_values[4]));
    }

    [Test]
    public void RemoveRowsWithSingleRow ()
    {
      _bocList.LoadUnboundValue(_values, false);

      _bocList.RemoveRows(new IBusinessObject[] {_values[2]});

      Assert.That(ReferenceEquals(_values, _bocList.Value), Is.False);
      Assert.That(_bocList.Value.Count, Is.EqualTo(4));
      Assert.That(_bocList.Value[0], Is.SameAs(_values[0]));
      Assert.That(_bocList.Value[1], Is.SameAs(_values[1]));
      Assert.That(_bocList.Value[2], Is.SameAs(_values[3]));
      Assert.That(_bocList.Value[3], Is.SameAs(_values[4]));
    }

    [Test]
    public void RemoveRowsWithMultipleRows ()
    {
      _bocList.LoadUnboundValue(_values, false);

      _bocList.RemoveRows(new IBusinessObject[] {_values[1], _values[3]});

      Assert.That(ReferenceEquals(_values, _bocList.Value), Is.False);
      Assert.That(_bocList.Value.Count, Is.EqualTo(3));
      Assert.That(_bocList.Value[0], Is.SameAs(_values[0]));
      Assert.That(_bocList.Value[1], Is.SameAs(_values[2]));
      Assert.That(_bocList.Value[2], Is.SameAs(_values[4]));
    }

    [Test]
    public void RowRowsWithValueAsIReadOnlyList ()
    {
      _bocList.LoadUnboundValue(_valuesAsIReadOnlyListStub.Object, false);

      Assert.That(
          () => _bocList.RemoveRows(new IBusinessObject[] {_values[1], _values[3]}),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The BocList is bound to a collection that does not implement the IList interface. "
                  + "Add and remove rows is not supported for collections that do not implement the IList interface."));
    }
  }
}
