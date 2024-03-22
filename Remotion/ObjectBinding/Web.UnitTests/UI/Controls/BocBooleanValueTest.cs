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
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocBooleanValueTest : BocTest
  {
    private BocBooleanValue _bocBooleanValue;
    private TypeWithBoolean _businessObject;
    private IBusinessObjectDataSource _dataSource;
    private IBusinessObjectBooleanProperty _propertyBooleanValue;
    private IBusinessObjectBooleanProperty _propertyNullableBooleanValue;

    public BocBooleanValueTest ()
    {
    }


    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocBooleanValue = new BocBooleanValue();
      _bocBooleanValue.ID = "BocBooleanValue";
      NamingContainer.Controls.Add(_bocBooleanValue);

      _businessObject = TypeWithBoolean.Create();

      _propertyBooleanValue =
          (IBusinessObjectBooleanProperty)((IBusinessObject)_businessObject).BusinessObjectClass.GetPropertyDefinition("BooleanValue");
      _propertyNullableBooleanValue =
          (IBusinessObjectBooleanProperty)((IBusinessObject)_businessObject).BusinessObjectClass.GetPropertyDefinition("NullableBooleanValue");

      _dataSource = new StubDataSource(((IBusinessObject)_businessObject).BusinessObjectClass);
      _dataSource.BusinessObject = (IBusinessObject)_businessObject;
    }

    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _bocBooleanValue.ReadOnly = true;
      string[] actual = _bocBooleanValue.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(0));
    }

    [Test]
    public void GetTrackedClientIDsInEditMode ()
    {
      _bocBooleanValue.ReadOnly = false;
      string[] actual = _bocBooleanValue.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(1));
      Assert.That(actual[0], Is.EqualTo(((IBocBooleanValue)_bocBooleanValue).GetValueName()));
    }


    [Test]
    public void SetValueToTrue ()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = true;
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(true));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToFalse ()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = false;
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(false));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = null;
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(null));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNullableBooleanTrue ()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = true;
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(true));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNullableBooleanFalse ()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = false;
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(false));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNullableBooleanNull ()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = null;
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(null));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }


    [Test]
    public void IBusinessObjectBoundControl_SetValueToTrue ()
    {
      _bocBooleanValue.IsDirty = false;
      ((IBusinessObjectBoundControl)_bocBooleanValue).Value = true;
      Assert.That(((IBusinessObjectBoundControl)_bocBooleanValue).Value, Is.EqualTo(true));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToFalse ()
    {
      _bocBooleanValue.IsDirty = false;
      ((IBusinessObjectBoundControl)_bocBooleanValue).Value = false;
      Assert.That(((IBusinessObjectBoundControl)_bocBooleanValue).Value, Is.EqualTo(false));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNull ()
    {
      _bocBooleanValue.IsDirty = false;
      ((IBusinessObjectBoundControl)_bocBooleanValue).Value = null;
      Assert.That(((IBusinessObjectBoundControl)_bocBooleanValue).Value, Is.EqualTo(null));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableBooleanTrue ()
    {
      _bocBooleanValue.IsDirty = false;
      ((IBusinessObjectBoundControl)_bocBooleanValue).Value = true;
      Assert.That(((IBusinessObjectBoundControl)_bocBooleanValue).Value, Is.EqualTo(true));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableBooleanFalse ()
    {
      _bocBooleanValue.IsDirty = false;
      ((IBusinessObjectBoundControl)_bocBooleanValue).Value = false;
      Assert.That(((IBusinessObjectBoundControl)_bocBooleanValue).Value, Is.EqualTo(false));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableBooleanNull ()
    {
      _bocBooleanValue.IsDirty = false;
      ((IBusinessObjectBoundControl)_bocBooleanValue).Value = null;
      Assert.That(((IBusinessObjectBoundControl)_bocBooleanValue).Value, Is.EqualTo(null));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocBooleanValue.Value = false;
      Assert.That(_bocBooleanValue.HasValue, Is.True);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocBooleanValue.Value = null;
      Assert.That(_bocBooleanValue.HasValue, Is.False);
    }


    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.BooleanValue = true;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyBooleanValue;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue(true);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(null));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueTrue ()
    {
      _businessObject.BooleanValue = true;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyBooleanValue;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue(false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(_businessObject.BooleanValue));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueFalse ()
    {
      _businessObject.BooleanValue = false;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyBooleanValue;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue(false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(_businessObject.BooleanValue));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableBooelanTrue ()
    {
      _businessObject.NullableBooleanValue = true;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyNullableBooleanValue;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue(false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(_businessObject.NullableBooleanValue));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableBooelanFalse ()
    {
      _businessObject.NullableBooleanValue = false;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyNullableBooleanValue;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue(false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(_businessObject.NullableBooleanValue));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableBooelanNull ()
    {
      _businessObject.NullableBooleanValue = null;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyNullableBooleanValue;
      _bocBooleanValue.Value = true;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue(false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(_businessObject.NullableBooleanValue));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      _bocBooleanValue.DataSource = null;
      _bocBooleanValue.Property = _propertyBooleanValue;
      _bocBooleanValue.Value = true;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue(false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(true));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = null;
      _bocBooleanValue.Value = true;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue(false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(true));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyBooleanValue;
      _bocBooleanValue.Value = true;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue(false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(null));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      bool value = true;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue(value, true);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(null));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueTrue ()
    {
      bool value = true;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue(value, false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(value));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueFalse ()
    {
      bool value = false;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue(value, false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(value));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNull ()
    {
      bool? value = null;
      _bocBooleanValue.Value = true;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue(value, false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(value));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableBooelanTrue ()
    {
      bool? value = true;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue(value, false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(value));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableBooelanFalse ()
    {
      bool? value = false;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue(value, false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(value));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableBooelanNull ()
    {
      bool? value = null;
      _bocBooleanValue.Value = true;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue(value, false);
      Assert.That(_bocBooleanValue.Value, Is.EqualTo(value));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndInterimTrue ()
    {
      _businessObject.BooleanValue = true;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyBooleanValue;
      _bocBooleanValue.Value = false;
      _bocBooleanValue.IsDirty = true;

      var result = _bocBooleanValue.SaveValue(true);
      Assert.That(result, Is.False);
      Assert.That(_businessObject.BooleanValue, Is.EqualTo(true));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndInterimFalse ()
    {
      _businessObject.BooleanValue = true;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyBooleanValue;
      _bocBooleanValue.Value = false;
      _bocBooleanValue.IsDirty = true;

      var result = _bocBooleanValue.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_businessObject.BooleanValue, Is.EqualTo(false));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndNotValid ()
    {
      _businessObject.BooleanValue = true;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyBooleanValue;
      _bocBooleanValue.Value = false;
      _bocBooleanValue.IsDirty = true;
      _bocBooleanValue.RegisterValidator(new AlwaysInvalidValidator());

      var result = _bocBooleanValue.SaveValue(false);
      Assert.That(result, Is.False);
      Assert.That(_businessObject.BooleanValue, Is.EqualTo(true));
      Assert.That(_bocBooleanValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      _businessObject.BooleanValue = true;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyBooleanValue;
      _bocBooleanValue.Value = false;
      _bocBooleanValue.IsDirty = false;

      var result = _bocBooleanValue.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_businessObject.BooleanValue, Is.EqualTo(true));
      Assert.That(_bocBooleanValue.IsDirty, Is.False);
    }

    [Test]
    public void GetValueName ()
    {
      Assert.That(((IBocBooleanValue)_bocBooleanValue).GetValueName(), Is.EqualTo("NamingContainer_BocBooleanValue_Value"));
    }

    [Test]
    public void GetTextValueName ()
    {
      Assert.That(((IBocBooleanValue)_bocBooleanValue).GetDisplayValueName(), Is.EqualTo("NamingContainer_BocBooleanValue_DisplayValue"));
    }

    [Test]
    public void CreateValidators_UsesValidatorFactory ()
    {
      var control = new BocBooleanValue();
      var serviceLocatorMock = new Mock<IServiceLocator>();
      var factoryMock = new Mock<IBocBooleanValueValidatorFactory>();
      serviceLocatorMock.Setup(m => m.GetInstance<IBocBooleanValueValidatorFactory>()).Returns(factoryMock.Object).Verifiable();
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
