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
  public class BocCheckBoxTest: BocTest
  {
    private BocCheckBox _bocCheckBox;
    private TypeWithBoolean _businessObject;
    private IBusinessObjectDataSource _dataSource;
    private IBusinessObjectBooleanProperty _propertyBooleanValue;
    private IBusinessObjectBooleanProperty _propertyNullableBooleanValue;

    public BocCheckBoxTest ()
    {
    }


    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocCheckBox = new BocCheckBox();
      _bocCheckBox.ID = "BocCheckBox";
      NamingContainer.Controls.Add(_bocCheckBox);

      _businessObject = TypeWithBoolean.Create();

      _propertyBooleanValue = (IBusinessObjectBooleanProperty)((IBusinessObject)_businessObject).BusinessObjectClass.GetPropertyDefinition("BooleanValue");
      _propertyNullableBooleanValue = (IBusinessObjectBooleanProperty)((IBusinessObject)_businessObject).BusinessObjectClass.GetPropertyDefinition("NullableBooleanValue");

      _dataSource = new StubDataSource(((IBusinessObject)_businessObject).BusinessObjectClass);
      _dataSource.BusinessObject = (IBusinessObject)_businessObject;
    }

    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _bocCheckBox.ReadOnly = true;
      string[] actual = _bocCheckBox.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(0));
    }

    [Test]
    public void GetTrackedClientIDsInEditMode ()
    {
      _bocCheckBox.ReadOnly = false;
      string[] actual = _bocCheckBox.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(1));
      Assert.That(actual[0], Is.EqualTo(((IBocCheckBox)_bocCheckBox).GetValueName()));
    }

    [Test]
    public void SetValueToTrue ()
    {
      _bocCheckBox.IsDirty = false;
      _bocCheckBox.Value = true;
      Assert.That(_bocCheckBox.Value, Is.EqualTo(true));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToFalse ()
    {
      _bocCheckBox.IsDirty = false;
      _bocCheckBox.Value = false;
      Assert.That(_bocCheckBox.Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocCheckBox.DefaultValue = false;
      _bocCheckBox.IsDirty = false;
      _bocCheckBox.Value = null;
      Assert.That(_bocCheckBox.Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNullableBooleanTrue ()
    {
      _bocCheckBox.IsDirty = false;
      _bocCheckBox.Value = true;
      Assert.That(_bocCheckBox.Value, Is.EqualTo(true));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNullableBooleanFalse ()
    {
      _bocCheckBox.IsDirty = false;
      _bocCheckBox.Value = false;
      Assert.That(_bocCheckBox.Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNullableBooleanNull ()
    {
      _bocCheckBox.DefaultValue = false;
      _bocCheckBox.IsDirty = false;
      _bocCheckBox.Value = null;
      Assert.That(_bocCheckBox.Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }


    [Test]
    public void IBusinessObjectBoundControl_SetValueToTrue ()
    {
      _bocCheckBox.IsDirty = false;
      ((IBusinessObjectBoundControl)_bocCheckBox).Value = true;
      Assert.That(((IBusinessObjectBoundControl)_bocCheckBox).Value, Is.EqualTo(true));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToFalse ()
    {
      _bocCheckBox.IsDirty = false;
      ((IBusinessObjectBoundControl)_bocCheckBox).Value = false;
      Assert.That(((IBusinessObjectBoundControl)_bocCheckBox).Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNull ()
    {
      _bocCheckBox.DefaultValue = false;
      _bocCheckBox.IsDirty = false;
      ((IBusinessObjectBoundControl)_bocCheckBox).Value = null;
      Assert.That(((IBusinessObjectBoundControl)_bocCheckBox).Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableBooleanTrue ()
    {
      _bocCheckBox.IsDirty = false;
      ((IBusinessObjectBoundControl)_bocCheckBox).Value = true;
      Assert.That(((IBusinessObjectBoundControl)_bocCheckBox).Value, Is.EqualTo(true));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableBooleanFalse ()
    {
      _bocCheckBox.IsDirty = false;
      ((IBusinessObjectBoundControl)_bocCheckBox).Value = false;
      Assert.That(((IBusinessObjectBoundControl)_bocCheckBox).Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableBooleanNull ()
    {
      _bocCheckBox.DefaultValue = false;
      _bocCheckBox.IsDirty = false;
      ((IBusinessObjectBoundControl)_bocCheckBox).Value = null;
      Assert.That(((IBusinessObjectBoundControl)_bocCheckBox).Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocCheckBox.Value = false;
      Assert.That(_bocCheckBox.HasValue, Is.True);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocCheckBox.Value = null;
      Assert.That(_bocCheckBox.HasValue, Is.True);
    }


    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.BooleanValue = true;
      _bocCheckBox.DataSource = _dataSource;
      _bocCheckBox.Property = _propertyBooleanValue;
      _bocCheckBox.Value = false;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadValue(true);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueTrue ()
    {
      _businessObject.BooleanValue = true;
      _bocCheckBox.DataSource = _dataSource;
      _bocCheckBox.Property = _propertyBooleanValue;
      _bocCheckBox.Value = false;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadValue(false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(_businessObject.BooleanValue));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueFalse ()
    {
      _businessObject.BooleanValue = false;
      _bocCheckBox.DataSource = _dataSource;
      _bocCheckBox.Property = _propertyBooleanValue;
      _bocCheckBox.Value = true;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadValue(false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(_businessObject.BooleanValue));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableBooelanTrue ()
    {
      _businessObject.NullableBooleanValue = true;
      _bocCheckBox.DataSource = _dataSource;
      _bocCheckBox.Property = _propertyNullableBooleanValue;
      _bocCheckBox.Value = false;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadValue(false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(_businessObject.NullableBooleanValue));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableBooelanFalse ()
    {
      _businessObject.NullableBooleanValue = false;
      _bocCheckBox.DataSource = _dataSource;
      _bocCheckBox.Property = _propertyNullableBooleanValue;
      _bocCheckBox.Value = true;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadValue(false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(_businessObject.NullableBooleanValue));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableBooelanNull ()
    {
      _businessObject.NullableBooleanValue = null;
      _bocCheckBox.DefaultValue = false;
      _bocCheckBox.DataSource = _dataSource;
      _bocCheckBox.Property = _propertyNullableBooleanValue;
      _bocCheckBox.Value = true;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadValue(false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      _bocCheckBox.DataSource = null;
      _bocCheckBox.Property = _propertyBooleanValue;
      _bocCheckBox.Value = true;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadValue(false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(true));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      _bocCheckBox.DataSource = _dataSource;
      _bocCheckBox.Property = null;
      _bocCheckBox.Value = true;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadValue(false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(true));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocCheckBox.DataSource = _dataSource;
      _bocCheckBox.Property = _propertyBooleanValue;
      _bocCheckBox.Value = true;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadValue(false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      bool value = true;
      _bocCheckBox.Value = false;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadUnboundValue(value, true);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueTrue ()
    {
      bool value = true;
      _bocCheckBox.Value = false;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadUnboundValue(value, false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(value));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueFalse ()
    {
      bool value = false;
      _bocCheckBox.Value = true;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadUnboundValue(value, false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(value));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNull ()
    {
      _bocCheckBox.DefaultValue = false;
      _bocCheckBox.Value = true;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadUnboundValue(null, false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableBooelanTrue ()
    {
      bool? value = true;
      _bocCheckBox.Value = false;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadUnboundValue(value, false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(value));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableBooelanFalse ()
    {
      bool? value = false;
      _bocCheckBox.Value = true;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadUnboundValue(value, false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(value));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableBooelanNull ()
    {
      bool? value = null;
      _bocCheckBox.DefaultValue = false;
      _bocCheckBox.Value = true;
      _bocCheckBox.IsDirty = true;

      _bocCheckBox.LoadUnboundValue(value, false);
      Assert.That(_bocCheckBox.Value, Is.EqualTo(false));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      _businessObject.BooleanValue = true;
      _bocCheckBox.DataSource = _dataSource;
      _bocCheckBox.Property = _propertyBooleanValue;
      _bocCheckBox.Value = false;
      _bocCheckBox.IsDirty = false;

      var result = _bocCheckBox.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_businessObject.BooleanValue, Is.EqualTo(true));
      Assert.That(_bocCheckBox.IsDirty, Is.False);
    }

    [Test]
    public void GetKeyValueName ()
    {
      Assert.That(((IBocCheckBox)_bocCheckBox).GetValueName(), Is.EqualTo("NamingContainer_BocCheckBox_Value"));
    }

    [Test]
    public void CreateValidators_UsesValidatorFactory ()
    {
      var control = new BocCheckBox();
      var serviceLocatorMock = new Mock<IServiceLocator>();
      var factoryMock = new Mock<IBocCheckBoxValidatorFactory>();
      serviceLocatorMock.Setup(m => m.GetInstance<IBocCheckBoxValidatorFactory>()).Returns(factoryMock.Object).Verifiable();
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
