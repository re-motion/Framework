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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UnitTests.Domain;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocListManageRowsInListEditModeTest : BocTest
  {
    private BocListMock _bocList;

    private IBusinessObject[] _values;
    private IBusinessObject[] _newValues;

    private BindableObjectClass _typeWithStringClass;

    private IBusinessObjectPropertyPath _typeWithStringFirstValuePath;
    private IBusinessObjectPropertyPath _typeWithStringSecondValuePath;

    private BocSimpleColumnDefinition _typeWithStringFirstValueSimpleColumn;
    private BocSimpleColumnDefinition _typeWithStringSecondValueSimpleColumn;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _values = new IBusinessObject[5];
      _values[0] = (IBusinessObject) TypeWithString.Create ("0", "A");
      _values[1] = (IBusinessObject) TypeWithString.Create ("1", "A");
      _values[2] = (IBusinessObject) TypeWithString.Create ("2", "B");
      _values[3] = (IBusinessObject) TypeWithString.Create ("3", "B");
      _values[4] = (IBusinessObject) TypeWithString.Create ("4", "C");

      _newValues = new IBusinessObject[2];
      _newValues[0] = (IBusinessObject) TypeWithString.Create ("5", "C");
      _newValues[1] = (IBusinessObject) TypeWithString.Create ("6", "D");

      _typeWithStringClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof (TypeWithString));

      _typeWithStringFirstValuePath = BusinessObjectPropertyPath.CreateStatic (_typeWithStringClass, "FirstValue");
      _typeWithStringSecondValuePath = BusinessObjectPropertyPath.CreateStatic (_typeWithStringClass, "SecondValue");

      _typeWithStringFirstValueSimpleColumn = new BocSimpleColumnDefinition();
      _typeWithStringFirstValueSimpleColumn.SetPropertyPath (_typeWithStringFirstValuePath);

      _typeWithStringSecondValueSimpleColumn = new BocSimpleColumnDefinition();
      _typeWithStringSecondValueSimpleColumn.SetPropertyPath (_typeWithStringSecondValuePath);

      _bocList = new BocListMock();
      _bocList.ID = "BocList";
      NamingContainer.Controls.Add (_bocList);

      _bocList.LoadUnboundValue (_values, false);
      _bocList.SwitchListIntoEditMode();

      Assert.That (_bocList.IsListEditModeActive, Is.True);
    }

    [Test]
    public void AddRow ()
    {
      int index = _bocList.AddRow (_newValues[0]);

      Assert.That (ReferenceEquals (_values, _bocList.Value), Is.False);
      Assert.That (_bocList.Value.Count, Is.EqualTo (6));
      Assert.That (_bocList.Value[0], Is.SameAs (_values[0]));
      Assert.That (_bocList.Value[1], Is.SameAs (_values[1]));
      Assert.That (_bocList.Value[2], Is.SameAs (_values[2]));
      Assert.That (_bocList.Value[3], Is.SameAs (_values[3]));
      Assert.That (_bocList.Value[4], Is.SameAs (_values[4]));

      Assert.That (index, Is.EqualTo (5));
      Assert.That (_bocList.Value[5], Is.SameAs (_newValues[0]));

      Assert.That (_bocList.IsListEditModeActive, Is.True);
    }

    [Test]
    public void AddRows ()
    {
      _bocList.AddRows (_newValues);

      Assert.That (ReferenceEquals (_values, _bocList.Value), Is.False);
      Assert.That (_bocList.Value.Count, Is.EqualTo (7));
      Assert.That (_bocList.Value[0], Is.SameAs (_values[0]));
      Assert.That (_bocList.Value[1], Is.SameAs (_values[1]));
      Assert.That (_bocList.Value[2], Is.SameAs (_values[2]));
      Assert.That (_bocList.Value[3], Is.SameAs (_values[3]));
      Assert.That (_bocList.Value[4], Is.SameAs (_values[4]));

      Assert.That (_bocList.Value[5], Is.SameAs (_newValues[0]));
      Assert.That (_bocList.Value[6], Is.SameAs (_newValues[1]));

      Assert.That (_bocList.IsListEditModeActive, Is.True);
    }

    [Test]
    public void RemoveRowWithIndex ()
    {
      _bocList.RemoveRow (2);

      Assert.That (ReferenceEquals (_values, _bocList.Value), Is.False);
      Assert.That (_bocList.Value.Count, Is.EqualTo (4));
      Assert.That (_bocList.Value[0], Is.SameAs (_values[0]));
      Assert.That (_bocList.Value[1], Is.SameAs (_values[1]));
      Assert.That (_bocList.Value[2], Is.SameAs (_values[3]));
      Assert.That (_bocList.Value[3], Is.SameAs (_values[4]));

      Assert.That (_bocList.IsListEditModeActive, Is.True);
    }

    [Test]
    public void RemoveRowWithBusinessObject ()
    {
      _bocList.RemoveRow (_values[2]);

      Assert.That (ReferenceEquals (_values, _bocList.Value), Is.False);
      Assert.That (_bocList.Value.Count, Is.EqualTo (4));
      Assert.That (_bocList.Value[0], Is.SameAs (_values[0]));
      Assert.That (_bocList.Value[1], Is.SameAs (_values[1]));
      Assert.That (_bocList.Value[2], Is.SameAs (_values[3]));
      Assert.That (_bocList.Value[3], Is.SameAs (_values[4]));

      Assert.That (_bocList.IsListEditModeActive, Is.True);
    }

    [Test]
    public void RemoveRowsWithNoRows ()
    {
      _bocList.RemoveRows (new IBusinessObject[0]);

      Assert.That (ReferenceEquals (_values, _bocList.Value), Is.False);
      Assert.That (_bocList.Value.Count, Is.EqualTo (5));
      Assert.That (_bocList.Value[0], Is.SameAs (_values[0]));
      Assert.That (_bocList.Value[1], Is.SameAs (_values[1]));
      Assert.That (_bocList.Value[2], Is.SameAs (_values[2]));
      Assert.That (_bocList.Value[3], Is.SameAs (_values[3]));
      Assert.That (_bocList.Value[4], Is.SameAs (_values[4]));

      Assert.That (_bocList.IsListEditModeActive, Is.True);
    }

    [Test]
    public void RemoveRowsWithSingleRow ()
    {
      _bocList.RemoveRows (new IBusinessObject[] {_values[2]});

      Assert.That (ReferenceEquals (_values, _bocList.Value), Is.False);
      Assert.That (_bocList.Value.Count, Is.EqualTo (4));
      Assert.That (_bocList.Value[0], Is.SameAs (_values[0]));
      Assert.That (_bocList.Value[1], Is.SameAs (_values[1]));
      Assert.That (_bocList.Value[2], Is.SameAs (_values[3]));
      Assert.That (_bocList.Value[3], Is.SameAs (_values[4]));

      Assert.That (_bocList.IsListEditModeActive, Is.True);
    }

    [Test]
    public void RemoveRowsWithMultipleRows ()
    {
      _bocList.RemoveRows (new IBusinessObject[] {_values[1], _values[3]});

      Assert.That (ReferenceEquals (_values, _bocList.Value), Is.False);
      Assert.That (_bocList.Value.Count, Is.EqualTo (3));
      Assert.That (_bocList.Value[0], Is.SameAs (_values[0]));
      Assert.That (_bocList.Value[1], Is.SameAs (_values[2]));
      Assert.That (_bocList.Value[2], Is.SameAs (_values[4]));

      Assert.That (_bocList.IsListEditModeActive, Is.True);
    }
  }
}
