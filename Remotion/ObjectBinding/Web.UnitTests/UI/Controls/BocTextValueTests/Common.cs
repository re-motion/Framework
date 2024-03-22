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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocTextValueTests
{
  [TestFixture]
  public class Common : BocTest
  {
    private BocTextValue _bocTextValue;
    private TypeWithString _businessObject;
    private IBusinessObjectDataSource _dataSource;
    private IBusinessObjectStringProperty _propertyStringValue;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocTextValue = new BocTextValue();
      _bocTextValue.ID = "BocTextValue";
      NamingContainer.Controls.Add(_bocTextValue);

      _businessObject = TypeWithString.Create();

      _propertyStringValue =
          (IBusinessObjectStringProperty)((IBusinessObject)_businessObject).BusinessObjectClass.GetPropertyDefinition("StringValue");

      _dataSource = new StubDataSource(((IBusinessObject)_businessObject).BusinessObjectClass);
      _dataSource.BusinessObject = (IBusinessObject)_businessObject;
    }

    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _bocTextValue.ReadOnly = true;
      string[] actual = _bocTextValue.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(0));
    }

    [Test]
    public void GetTrackedClientIDsInEditMode ()
    {
      _bocTextValue.ReadOnly = false;
      string[] actual = _bocTextValue.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(1));
      Assert.That(actual[0], Is.EqualTo(((IBocTextValueBase)_bocTextValue).GetValueName()));
    }


    [Test]
    public void SetValueToString ()
    {
      string value = "Foo Bar";
      _bocTextValue.IsDirty = false;
      _bocTextValue.Value = value;
      Assert.That(_bocTextValue.Value, Is.EqualTo(value));
      Assert.That(_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocTextValue.IsDirty = false;
      _bocTextValue.Value = null;
      Assert.That(_bocTextValue.Value, Is.EqualTo(null));
      Assert.That(_bocTextValue.IsDirty, Is.True);
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocTextValue.Value = "x";
      Assert.That(_bocTextValue.HasValue, Is.True);
    }

    [Test]
    public void HasValue_TextContainsInvaliData_ReturnsTrue ()
    {
      _bocTextValue.ValueType = BocTextValueType.Date;
      _bocTextValue.Text = "x";
      Assert.That(_bocTextValue.HasValue, Is.True);
    }

    [Test]
    public void HasValue_TextContainsOnlyWhitespace_ReturnsFalse ()
    {
      _bocTextValue.ValueType = BocTextValueType.Date;
      _bocTextValue.Text = "  ";
      Assert.That(_bocTextValue.HasValue, Is.False);
      Assert.That(_bocTextValue.Value, Is.Null);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocTextValue.Value = null;
      Assert.That(_bocTextValue.HasValue, Is.False);
    }


    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue(true);
      Assert.That(_bocTextValue.Value, Is.EqualTo(null));
      Assert.That(_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithString ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue(false);
      Assert.That(_bocTextValue.Value, Is.EqualTo(_businessObject.StringValue));
      Assert.That(_bocTextValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithNull ()
    {
      _businessObject.StringValue = null;
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue(false);
      Assert.That(_bocTextValue.Value, Is.EqualTo(_businessObject.StringValue));
      Assert.That(_bocTextValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      _bocTextValue.DataSource = null;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue(false);
      Assert.That(_bocTextValue.Value, Is.EqualTo("Foo Bar"));
      Assert.That(_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = null;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue(false);
      Assert.That(_bocTextValue.Value, Is.EqualTo("Foo Bar"));
      Assert.That(_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadValue(false);
      Assert.That(_bocTextValue.Value, Is.EqualTo(null));
      Assert.That(_bocTextValue.IsDirty, Is.False);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      string value = "Foo Bar";
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadUnboundValue(value, true);
      Assert.That(_bocTextValue.Value, Is.EqualTo(null));
      Assert.That(_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithString ()
    {
      string value = "Foo Bar";
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadUnboundValue(value, false);
      Assert.That(_bocTextValue.Value, Is.EqualTo(value));
      Assert.That(_bocTextValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      string value = null;
      _bocTextValue.Value = "Foo Bar";
      _bocTextValue.IsDirty = true;

      _bocTextValue.LoadUnboundValue(value, false);
      Assert.That(_bocTextValue.Value, Is.EqualTo(value));
      Assert.That(_bocTextValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndInterimTrue ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      var result = _bocTextValue.SaveValue(true);
      Assert.That(result, Is.False);
      Assert.That(_businessObject.StringValue, Is.EqualTo("Foo Bar"));
      Assert.That(_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndInterimFalse ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;

      var result = _bocTextValue.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_businessObject.StringValue, Is.EqualTo(null));
      Assert.That(_bocTextValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndNotValid ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = true;
      _bocTextValue.RegisterValidator(new AlwaysInvalidValidator());

      var result = _bocTextValue.SaveValue(false);
      Assert.That(result, Is.False);
      Assert.That(_businessObject.StringValue, Is.EqualTo("Foo Bar"));
      Assert.That(_bocTextValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      _businessObject.StringValue = "Foo Bar";
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _propertyStringValue;
      _bocTextValue.Value = null;
      _bocTextValue.IsDirty = false;

      var result = _bocTextValue.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_businessObject.StringValue, Is.EqualTo("Foo Bar"));
      Assert.That(_bocTextValue.IsDirty, Is.False);
    }

    [Test]
    public void GetValueName ()
    {
      Assert.That(((IBocTextValueBase)_bocTextValue).GetValueName(), Is.EqualTo("NamingContainer_BocTextValue_Value"));
    }
  }
}
