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
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocEnumValueTest : BocTest
  {
    private BocEnumValue _bocEnumValue;
    private TypeWithEnum _businessObject;
    private IBusinessObjectDataSource _dataSource;
    private IBusinessObjectEnumerationProperty _propertyEnumValue;

    public BocEnumValueTest ()
    {
    }


    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocEnumValue = new BocEnumValue();
      _bocEnumValue.ID = "BocEnumValue";
      NamingContainer.Controls.Add(_bocEnumValue);

      _businessObject = TypeWithEnum.Create();

      _propertyEnumValue =
          (IBusinessObjectEnumerationProperty)((IBusinessObject)_businessObject).BusinessObjectClass.GetPropertyDefinition("EnumValue");

      _dataSource = new StubDataSource(((IBusinessObject)_businessObject).BusinessObjectClass);
      _dataSource.BusinessObject = (IBusinessObject)_businessObject;
    }

    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _bocEnumValue.ReadOnly = true;
      string[] actual = _bocEnumValue.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(0));
    }

    [Test]
    public void GetTrackedClientIDsInEditModeAsDropDownList ()
    {
      _bocEnumValue.ReadOnly = false;
      _bocEnumValue.ListControlStyle.ControlType = ListControlType.DropDownList;
      string[] actual = _bocEnumValue.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(1));
      Assert.That(actual[0], Is.EqualTo(((IBocEnumValue)_bocEnumValue).GetValueName()));
    }

    [Test]
    public void GetTrackedClientIDsInEditModeAsListBox ()
    {
      _bocEnumValue.ReadOnly = false;
      _bocEnumValue.ListControlStyle.ControlType = ListControlType.ListBox;
      string[] actual = _bocEnumValue.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(1));
      Assert.That(actual[0], Is.EqualTo(((IBocEnumValue)_bocEnumValue).GetValueName()));
    }

    [Test]
    public void GetTrackedClientIDsInEditModeAsRadioButtonList ()
    {
      _bocEnumValue.ReadOnly = false;
      _bocEnumValue.ListControlStyle.ControlType = ListControlType.RadioButtonList;
      Assert.That(_propertyEnumValue, Is.Not.Null, "Could not find property 'EnumValue'.");
      Assert.IsTrue(
          typeof(IBusinessObjectEnumerationProperty).IsAssignableFrom(_propertyEnumValue.GetType()),
          "Property 'EnumValue' of invalid type.");
      _bocEnumValue.Property = _propertyEnumValue;

      string[] actual = _bocEnumValue.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(3));
      var valueName = ((IBocEnumValue)_bocEnumValue).GetValueName();
      Assert.That(actual[0], Is.EqualTo(valueName + "_0"));
      Assert.That(actual[1], Is.EqualTo(valueName + "_1"));
      Assert.That(actual[2], Is.EqualTo(valueName + "_2"));
    }

    [Test]
    public void SetValueToEnum ()
    {
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.IsDirty = false;
      _bocEnumValue.Value = TestEnum.Second;
      Assert.That(_bocEnumValue.Value, Is.EqualTo(TestEnum.Second));
      Assert.That(_bocEnumValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.IsDirty = false;
      _bocEnumValue.Value = null;
      Assert.That(_bocEnumValue.Value, Is.EqualTo(null));
      Assert.That(_bocEnumValue.IsDirty, Is.True);
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = TestEnum.Second;
      Assert.That(_bocEnumValue.HasValue, Is.True);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = null;
      Assert.That(_bocEnumValue.HasValue, Is.False);
    }


    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.EnumValue = TestEnum.Second;
      _bocEnumValue.DataSource = _dataSource;
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = null;
      _bocEnumValue.IsDirty = true;

      _bocEnumValue.LoadValue(true);
      Assert.That(_bocEnumValue.Value, Is.EqualTo(null));
      Assert.That(_bocEnumValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithEnum ()
    {
      _businessObject.EnumValue = TestEnum.Second;
      _bocEnumValue.DataSource = _dataSource;
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = null;
      _bocEnumValue.IsDirty = true;

      _bocEnumValue.LoadValue(false);
      Assert.That(_bocEnumValue.Value, Is.EqualTo(_businessObject.EnumValue));
      Assert.That(_bocEnumValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      _bocEnumValue.DataSource = null;
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = TestEnum.Second;
      _bocEnumValue.IsDirty = true;

      _bocEnumValue.LoadValue(false);
      Assert.That(_bocEnumValue.Value, Is.EqualTo(TestEnum.Second));
      Assert.That(_bocEnumValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      _dataSource.BusinessObject = null;
      _bocEnumValue.DataSource = _dataSource;
      // BocEnumValue requires a property when setting the value
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = TestEnum.Second;
      _bocEnumValue.Property = null;
      _bocEnumValue.IsDirty = true;

      _bocEnumValue.LoadValue(false);
      Assert.That(_bocEnumValue.Value, Is.EqualTo(TestEnum.Second));
      Assert.That(_bocEnumValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocEnumValue.DataSource = _dataSource;
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = TestEnum.Second;
      _bocEnumValue.IsDirty = true;

      _bocEnumValue.LoadValue(false);
      Assert.That(_bocEnumValue.Value, Is.EqualTo(null));
      Assert.That(_bocEnumValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      TestEnum value = TestEnum.Second;
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = null;
      _bocEnumValue.IsDirty = true;

      _bocEnumValue.LoadUnboundValue(value, true);
      Assert.That(_bocEnumValue.Value, Is.EqualTo(null));
      Assert.That(_bocEnumValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithEnum ()
    {
      TestEnum value = TestEnum.Second;
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = null;
      _bocEnumValue.IsDirty = true;

      _bocEnumValue.LoadUnboundValue(value, false);
      Assert.That(_bocEnumValue.Value, Is.EqualTo(value));
      Assert.That(_bocEnumValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      TestEnum? value = null;
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = TestEnum.Second;
      _bocEnumValue.IsDirty = true;

      _bocEnumValue.LoadUnboundValue(value, false);
      Assert.That(_bocEnumValue.Value, Is.EqualTo(value));
      Assert.That(_bocEnumValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndInterimTrue ()
    {
      _businessObject.EnumValue = TestEnum.Second;
      _bocEnumValue.DataSource = _dataSource;
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = TestEnum.First;
      _bocEnumValue.IsDirty = true;

      var result = _bocEnumValue.SaveValue(true);
      Assert.That(result, Is.False);
      Assert.That(_businessObject.EnumValue, Is.EqualTo(TestEnum.Second));
      Assert.That(_bocEnumValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndInterimFalse ()
    {
      _businessObject.EnumValue = TestEnum.Second;
      _bocEnumValue.DataSource = _dataSource;
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = TestEnum.First;
      _bocEnumValue.IsDirty = true;

      var result = _bocEnumValue.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_businessObject.EnumValue, Is.EqualTo(TestEnum.First));
      Assert.That(_bocEnumValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndNotValid ()
    {
      _businessObject.EnumValue = TestEnum.Second;
      _bocEnumValue.DataSource = _dataSource;
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = TestEnum.First;
      _bocEnumValue.IsDirty = true;
      _bocEnumValue.RegisterValidator(new AlwaysInvalidValidator());

      var result = _bocEnumValue.SaveValue(false);
      Assert.That(result, Is.False);
      Assert.That(_businessObject.EnumValue, Is.EqualTo(TestEnum.Second));
      Assert.That(_bocEnumValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      _businessObject.EnumValue = TestEnum.Second;
      _bocEnumValue.DataSource = _dataSource;
      _bocEnumValue.Property = _propertyEnumValue;
      _bocEnumValue.Value = TestEnum.First;
      _bocEnumValue.IsDirty = false;

      var result = _bocEnumValue.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_businessObject.EnumValue, Is.EqualTo(TestEnum.Second));
      Assert.That(_bocEnumValue.IsDirty, Is.False);
    }

    [Test]
    public void GetValueName ()
    {
      Assert.That(((IBocEnumValue)_bocEnumValue).GetValueName(), Is.EqualTo("NamingContainer_BocEnumValue_Value"));
    }

    [Test]
    public void CreateValidators_UsesValidatorFactory ()
    {
      var control = new BocEnumValue();
      var serviceLocatorMock = new Mock<IServiceLocator>();
      var factoryMock = new Mock<IBocEnumValueValidatorFactory>();
      serviceLocatorMock.Setup(m => m.GetInstance<IBocEnumValueValidatorFactory>()).Returns(factoryMock.Object).Verifiable();
      factoryMock.Setup(f => f.CreateValidators(control, false)).Returns(new List<BaseValidator>()).Verifiable();

      using (new ServiceLocatorScope(serviceLocatorMock.Object))
      {
        control.CreateValidators();
      }

      factoryMock.Verify();
      serviceLocatorMock.Verify();
    }

    [Test]
    [TestCase(ListControlType.DropDownList, TestName = "GetNullItemText_WithTextFromProperty_ForDropDownList_ReturnsTextFromProperty")]
    [TestCase(ListControlType.ListBox, TestName = "GetNullItemText_WithTextFromProperty_ForListBox_ReturnsTextFromProperty")]
    [TestCase(ListControlType.RadioButtonList, TestName = "GetNullItemText_WithTextFromProperty_ForRadioButtonList_ReturnsTextFromProperty")]
    public void GetNullItemText_WithTextFromProperty (ListControlType listControlType)
    {
      var control = new BocEnumValue();
      control.UndefinedItemText = PlainTextString.CreateFromText("The undefined item");
      control.ListControlStyle.ControlType = listControlType;

      Assert.That(((IBocEnumValue)control).GetNullItemText(), Is.EqualTo(PlainTextString.CreateFromText("The undefined item")));
    }

    [Test]
    [TestCase(ListControlType.DropDownList, TestName = "GetNullItemText_WithTextFromResourceManager_ForDropDownList_ReturnsTextFromResourceManager")]
    [TestCase(ListControlType.ListBox, TestName = "GetNullItemText_WithTextFromResourceManager_ForListBox_ReturnsTextFromResourceManager")]
    [TestCase(ListControlType.RadioButtonList, TestName = "GetNullItemText_WithTextFromResourceManager_ForRadioButtonList_ReturnsTextFromResourceManager")]
    public void GetNullItemText_WithTextFromResourceManager (ListControlType listControlType)
    {
      var control = new BocEnumValue();
      control.ListControlStyle.ControlType = listControlType;

      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That(((IBocEnumValue)control).GetNullItemText().IsEmpty, Is.False);
      }
    }
  }
}
