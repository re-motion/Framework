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
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocMultilineTextValueTest : BocTest
  {
    private BocMultilineTextValue _bocMultilineTextValue;
    private TypeWithString _businessObject;
    private IBusinessObjectDataSource _dataSource;
    private IBusinessObjectStringProperty _propertyStringArray;

    public BocMultilineTextValueTest ()
    {
    }


    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocMultilineTextValue = new BocMultilineTextValue();
      _bocMultilineTextValue.ID = "BocMultilineTextValue";
      NamingContainer.Controls.Add(_bocMultilineTextValue);

      _businessObject = TypeWithString.Create();

      _propertyStringArray =
          (IBusinessObjectStringProperty)((IBusinessObject)_businessObject).BusinessObjectClass.GetPropertyDefinition("StringArray");

      _dataSource = new StubDataSource(((IBusinessObject)_businessObject).BusinessObjectClass);
      _dataSource.BusinessObject = (IBusinessObject)_businessObject;
    }

    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _bocMultilineTextValue.ReadOnly = true;
      string[] actual = _bocMultilineTextValue.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(0));
    }

    [Test]
    public void GetTrackedClientIDsInEditMode ()
    {
      _bocMultilineTextValue.ReadOnly = false;
      string[] actual = _bocMultilineTextValue.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(1));
      Assert.That(actual[0], Is.EqualTo(((IBocTextValueBase)_bocMultilineTextValue).GetValueName()));
    }

    [Test]
    public void SetValueToString ()
    {
      string[] value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.IsDirty = false;
      _bocMultilineTextValue.Value = value;
      Assert.That(_bocMultilineTextValue.Value, Is.EqualTo(value));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocMultilineTextValue.IsDirty = false;
      _bocMultilineTextValue.Value = null;
      Assert.That(_bocMultilineTextValue.Value, Is.EqualTo(null));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.True);
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocMultilineTextValue.Value = new[] { "x" };
      Assert.That(_bocMultilineTextValue.HasValue, Is.True);
    }

    [Test]
    public void HasValue_TextContainsOnlyWhitespace_ReturnsFalse ()
    {
      _bocMultilineTextValue.Text = " \r\n ";
      Assert.That(_bocMultilineTextValue.HasValue, Is.False);
      Assert.That(_bocMultilineTextValue.Value, Is.Null);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocMultilineTextValue.Value = null;
      Assert.That(_bocMultilineTextValue.HasValue, Is.False);
    }

    [Test]
    public void Value_TrimsWhitespaceAndNewLines ()
    {
      _bocMultilineTextValue.Text = " \r\n \r\n a\r\n b \r\n\r\n d \r\n \r\n ";
      Assert.That(_bocMultilineTextValue.Value, Is.EqualTo(new[] { "a", " b ", "", " d" }));
    }

    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.StringArray = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadValue(true);
      Assert.That(_bocMultilineTextValue.Value, Is.EqualTo(null));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithString ()
    {
      _businessObject.StringArray = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadValue(false);
      Assert.That(_bocMultilineTextValue.Value, Is.EqualTo(_businessObject.StringArray));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithNull ()
    {
      _businessObject.StringArray = null;
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadValue(false);
      Assert.That(_bocMultilineTextValue.Value, Is.EqualTo(_businessObject.StringArray));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      _bocMultilineTextValue.DataSource = null;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadValue(false);
      Assert.That(_bocMultilineTextValue.Value, Is.EqualTo(new[] { "Foo", "Bar" }));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = null;
      _bocMultilineTextValue.Value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadValue(false);
      Assert.That(_bocMultilineTextValue.Value, Is.EqualTo(new[] { "Foo", "Bar" }));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadValue(false);
      Assert.That(_bocMultilineTextValue.Value, Is.EqualTo(null));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.False);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      string[] value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadUnboundValue(value, true);
      Assert.That(_bocMultilineTextValue.Value, Is.EqualTo(null));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithString ()
    {
      string[] value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadUnboundValue(value, false);
      Assert.That(_bocMultilineTextValue.Value, Is.EqualTo(value));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      string[] value = null;
      _bocMultilineTextValue.Value = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.IsDirty = true;

      _bocMultilineTextValue.LoadUnboundValue(value, false);
      Assert.That(_bocMultilineTextValue.Value, Is.EqualTo(value));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndInterimTrue ()
    {
      _businessObject.StringArray = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;

      var result = _bocMultilineTextValue.SaveValue(true);
      Assert.That(result, Is.False);
      Assert.That(_businessObject.StringArray, Is.EqualTo(new[] { "Foo", "Bar" }));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndInterimFalse ()
    {
      _businessObject.StringArray = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;

      var result = _bocMultilineTextValue.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_businessObject.StringArray, Is.EqualTo(null));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndNotValid ()
    {
      _businessObject.StringArray = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = true;
      _bocMultilineTextValue.RegisterValidator(new AlwaysInvalidValidator());

      var result = _bocMultilineTextValue.SaveValue(false);
      Assert.That(result, Is.False);
      Assert.That(_businessObject.StringArray, Is.EqualTo(new[] { "Foo", "Bar" }));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      _businessObject.StringArray = new[] { "Foo", "Bar" };
      _bocMultilineTextValue.DataSource = _dataSource;
      _bocMultilineTextValue.Property = _propertyStringArray;
      _bocMultilineTextValue.Value = null;
      _bocMultilineTextValue.IsDirty = false;

      var result = _bocMultilineTextValue.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_businessObject.StringArray, Is.EqualTo(new[] { "Foo", "Bar" }));
      Assert.That(_bocMultilineTextValue.IsDirty, Is.False);
    }

    [Test]
    public void CreateValidators_UsesValidatorFactory ()
    {
      var control = new BocMultilineTextValue();
      var serviceLocatorMock = new Mock<IServiceLocator>();
      var factoryMock = new Mock<IBocMultilineTextValueValidatorFactory>();
      serviceLocatorMock.Setup(m => m.GetInstance<IBocMultilineTextValueValidatorFactory>()).Returns(factoryMock.Object).Verifiable();
      factoryMock.Setup(f => f.CreateValidators(control, false)).Returns(new List<BaseValidator>()).Verifiable();

      using (new ServiceLocatorScope(serviceLocatorMock.Object))
      {
        control.CreateValidators();
      }

      factoryMock.Verify();
      serviceLocatorMock.Verify();
    }
  }
}
