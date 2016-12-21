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
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocDateTimeValueTest: BocTest
  {
    private BocDateTimeValueMock _bocDateTimeValue;
    private TypeWithDateTime _businessObject;
    private IBusinessObjectDataSource _dataSource;
    private IBusinessObjectDateTimeProperty _propertyDateTimeValue;
    private IBusinessObjectDateTimeProperty _propertyNullableDateTimeValue;

    public BocDateTimeValueTest()
    {
    }


    [SetUp]
    public override void SetUp()
    {
      base.SetUp();
      _bocDateTimeValue = new BocDateTimeValueMock();
      _bocDateTimeValue.ID = "BocDateTimeValue";
      NamingContainer.Controls.Add (_bocDateTimeValue);

      _businessObject = TypeWithDateTime.Create();

      _propertyDateTimeValue = (IBusinessObjectDateTimeProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("DateTimeValue");
      _propertyNullableDateTimeValue = (IBusinessObjectDateTimeProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("NullableDateTimeValue");

      _dataSource = new StubDataSource (((IBusinessObject) _businessObject).BusinessObjectClass);
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelUndefined()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
      _bocDateTimeValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.False);
      Assert.That (WcagHelperMock.HasError, Is.False);
    }

    [Test]
    public void EvaluateWaiConformityLevelA()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _bocDateTimeValue.DateTextBoxStyle.AutoPostBack = true;
      _bocDateTimeValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.False);
      Assert.That (WcagHelperMock.HasError, Is.False);
    }

    [Test]
    public void EvaluateWaiConformityDebugLevelDoubleAWithTimeTextBoxActive()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
      _bocDateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      _bocDateTimeValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasError, Is.True);
      Assert.That (WcagHelperMock.Priority, Is.EqualTo (2));
      Assert.That (WcagHelperMock.Control, Is.SameAs (_bocDateTimeValue));
      Assert.That (WcagHelperMock.Property, Is.EqualTo ("ActualValueType"));
    }

    [Test]
    public void EvaluateWaiConformityDebugLevelAWithDateTimeTextBoxStyleAutoPostBackTrue()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocDateTimeValue.DateTimeTextBoxStyle.AutoPostBack = true;
      _bocDateTimeValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.True);
      Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
      Assert.That (WcagHelperMock.Control, Is.SameAs (_bocDateTimeValue));
      Assert.That (WcagHelperMock.Property, Is.EqualTo ("DateTimeTextBoxStyle.AutoPostBack"));
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelAWithDateTextBoxStyleAutoPostBackTrue()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocDateTimeValue.DateTextBoxStyle.AutoPostBack = true;
      _bocDateTimeValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.True);
      Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
      Assert.That (WcagHelperMock.Control, Is.SameAs (_bocDateTimeValue));
      Assert.That (WcagHelperMock.Property, Is.EqualTo ("DateTextBoxStyle.AutoPostBack"));
    }

    [Test]
    public void EvaluateWaiConformityDebugLevelAWithTimeTextBoxStyleAutoPostBackTrue()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocDateTimeValue.TimeTextBoxStyle.AutoPostBack = true;
      _bocDateTimeValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.True);
      Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
      Assert.That (WcagHelperMock.Control, Is.SameAs (_bocDateTimeValue));
      Assert.That (WcagHelperMock.Property, Is.EqualTo ("TimeTextBoxStyle.AutoPostBack"));
    }

    [Test]
    public void GetTrackedClientIDsInReadOnlyMode()
    {
      _bocDateTimeValue.ReadOnly = true;
      string[] actual = _bocDateTimeValue.GetTrackedClientIDs();
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (0));
    }

    [Test]
    public void GetTrackedClientIDsInEditModeAndValueTypeIsDateTime()
    {
      _bocDateTimeValue.ReadOnly = false;
      _bocDateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      string[] actual = _bocDateTimeValue.GetTrackedClientIDs();
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (2));
      Assert.That (actual[0], Is.EqualTo (((IBocDateTimeValue) _bocDateTimeValue).GetDateValueName()));
      Assert.That (actual[1], Is.EqualTo (((IBocDateTimeValue)_bocDateTimeValue).GetTimeValueName()));
    }

    [Test]
    public void GetTrackedClientIDsInEditModeAndValueTypeIsDate()
    {
      _bocDateTimeValue.ReadOnly = false;
      _bocDateTimeValue.ValueType = BocDateTimeValueType.Date;
      string[] actual = _bocDateTimeValue.GetTrackedClientIDs();
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (1));
      Assert.That (actual[0], Is.EqualTo (((IBocDateTimeValue)_bocDateTimeValue).GetDateValueName()));
    }

    [Test]
    public void GetTrackedClientIDsInEditModeAndValueTypeIsUndefined()
    {
      _bocDateTimeValue.ReadOnly = false;
      _bocDateTimeValue.ValueType = BocDateTimeValueType.Undefined;
      string[] actual = _bocDateTimeValue.GetTrackedClientIDs();
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (2));
      Assert.That (actual[0], Is.EqualTo (((IBocDateTimeValue)_bocDateTimeValue).GetDateValueName()));
      Assert.That (actual[1], Is.EqualTo (((IBocDateTimeValue)_bocDateTimeValue).GetTimeValueName()));
    }


    [Test]
    public void SetValueToDateTime()
    {
      DateTime dateTime = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.IsDirty = false;
      _bocDateTimeValue.Value = dateTime;
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (dateTime));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNull()
    {
      _bocDateTimeValue.IsDirty = false;
      _bocDateTimeValue.Value = null;
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (null));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNullableDateTime()
    {
      DateTime? dateTime = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.IsDirty = false;
      _bocDateTimeValue.Value = dateTime;
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (dateTime));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNullableDateTimeNull()
    {
      _bocDateTimeValue.IsDirty = false;
      _bocDateTimeValue.Value = null;
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (null));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }


    [Test]
    public void IBusinessObjectBoundControl_SetValueToDateTime ()
    {
      DateTime dateTime = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocDateTimeValue).Value = dateTime;
      Assert.That (((IBusinessObjectBoundControl) _bocDateTimeValue).Value, Is.EqualTo (dateTime));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNull ()
    {
      _bocDateTimeValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocDateTimeValue).Value = null;
      Assert.That (((IBusinessObjectBoundControl) _bocDateTimeValue).Value, Is.EqualTo (null));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableDateTime ()
    {
      DateTime? dateTime = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocDateTimeValue).Value = dateTime;
      Assert.That (((IBusinessObjectBoundControl) _bocDateTimeValue).Value, Is.EqualTo (dateTime));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }

    [Test]
    public void IBusinessObjectBoundControl_SetValueToNullableDateTimeNull ()
    {
      _bocDateTimeValue.IsDirty = false;
      ((IBusinessObjectBoundControl) _bocDateTimeValue).Value = null;
      Assert.That (((IBusinessObjectBoundControl) _bocDateTimeValue).Value, Is.EqualTo (null));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocDateTimeValue.Value = DateTime.Now;
      Assert.That (_bocDateTimeValue.HasValue, Is.True);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocDateTimeValue.Value = null;
      Assert.That (_bocDateTimeValue.HasValue, Is.False);
    }


    [Test]
    public void LoadValueAndInterimTrue()
    {
      _businessObject.DateTimeValue = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyDateTimeValue;
      _bocDateTimeValue.Value = null;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadValue (true);
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (null));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDateTime()
    {
      _businessObject.DateTimeValue = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyDateTimeValue;
      _bocDateTimeValue.Value = null;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadValue (false);
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (_businessObject.DateTimeValue));
      Assert.That (_bocDateTimeValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableDateTime()
    {
      _businessObject.NullableDateTimeValue = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyNullableDateTimeValue;
      _bocDateTimeValue.Value = null;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadValue (false);
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (_businessObject.NullableDateTimeValue));
      Assert.That (_bocDateTimeValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableDateTimeNull()
    {
      _businessObject.NullableDateTimeValue = null;
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyNullableDateTimeValue;
      _bocDateTimeValue.Value = DateTime.Now;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadValue (false);
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (_businessObject.NullableDateTimeValue));
      Assert.That (_bocDateTimeValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      _bocDateTimeValue.DataSource = null;
      _bocDateTimeValue.Property = _propertyDateTimeValue;
      _bocDateTimeValue.Value = new DateTime (2000, 1, 1);
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadValue (false);
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (new DateTime (2000, 1, 1)));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = null;
      _bocDateTimeValue.Value = new DateTime (2000, 1, 1);
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadValue (false);
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (new DateTime (2000, 1, 1)));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyDateTimeValue;
      _bocDateTimeValue.Value = DateTime.Now;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadValue (false);
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (null));
      Assert.That (_bocDateTimeValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimTrue()
    {
      DateTime value = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.Value = null;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadUnboundValue (value, true);
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (null));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithDateTime()
    {
      DateTime value = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.Value = null;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadUnboundValue (value, false);
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (value));
      Assert.That (_bocDateTimeValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNull()
    {
      DateTime? value = null;
      _bocDateTimeValue.Value = DateTime.Now;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadUnboundValue (value, false);
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (value));
      Assert.That (_bocDateTimeValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableDateTime()
    {
      DateTime? value = new DateTime (2006, 1, 1, 1, 1, 1);
      _bocDateTimeValue.Value = null;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadUnboundValue (value, false);
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (value));
      Assert.That (_bocDateTimeValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableDateTimeNull()
    {
      DateTime? value = null;
      _bocDateTimeValue.Value = DateTime.Now;
      _bocDateTimeValue.IsDirty = true;

      _bocDateTimeValue.LoadUnboundValue (value, false);
      Assert.That (_bocDateTimeValue.Value, Is.EqualTo (value));
      Assert.That (_bocDateTimeValue.IsDirty, Is.False);
    }


    [Test]
    public void SaveValueAndInterimTrue ()
    {
      _businessObject.DateTimeValue = new DateTime (2000, 1, 1);
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyDateTimeValue;
      _bocDateTimeValue.Value = DateTime.Now;
      _bocDateTimeValue.IsDirty = true;

      var result = _bocDateTimeValue.SaveValue (true);
      Assert.That (result, Is.False);
      Assert.That (_businessObject.DateTimeValue, Is.EqualTo (new DateTime (2000, 1, 1)));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndInterimFalse ()
    {
      _businessObject.DateTimeValue = new DateTime (2000, 1, 1);
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyDateTimeValue;
      _bocDateTimeValue.Value = new DateTime (2011, 5, 5);
      _bocDateTimeValue.IsDirty = true;

      var result = _bocDateTimeValue.SaveValue (false);
      Assert.That (result, Is.True);
      Assert.That (_businessObject.DateTimeValue, Is.EqualTo (new DateTime (2011, 5, 5)));
      Assert.That (_bocDateTimeValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndNotValid ()
    {
      _businessObject.DateTimeValue = new DateTime (2000, 1, 1);
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyDateTimeValue;
      _bocDateTimeValue.Value = DateTime.Now;
      _bocDateTimeValue.IsDirty = true;
      _bocDateTimeValue.RegisterValidator (new AlwaysInvalidValidator());

      var result = _bocDateTimeValue.SaveValue (false);
      Assert.That (result, Is.False);
      Assert.That (_businessObject.DateTimeValue, Is.EqualTo (new DateTime (2000, 1, 1)));
      Assert.That (_bocDateTimeValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      _businessObject.DateTimeValue = new DateTime (2000, 1, 1);
      _bocDateTimeValue.DataSource = _dataSource;
      _bocDateTimeValue.Property = _propertyDateTimeValue;
      _bocDateTimeValue.Value = DateTime.Now;
      _bocDateTimeValue.IsDirty = false;

      var result = _bocDateTimeValue.SaveValue (false);
      Assert.That (result, Is.True);
      Assert.That (_businessObject.DateTimeValue, Is.EqualTo (new DateTime (2000, 1, 1)));
      Assert.That (_bocDateTimeValue.IsDirty, Is.False);
    }

    [Test]
    public void GetDateValueName ()
    {
      Assert.That (((IBocDateTimeValue)_bocDateTimeValue).GetDateValueName(), Is.EqualTo ("NamingContainer_BocDateTimeValue_DateValue"));
    }

    [Test]
    public void GetTimeValueName ()
    {
      Assert.That (((IBocDateTimeValue)_bocDateTimeValue).GetTimeValueName(), Is.EqualTo ("NamingContainer_BocDateTimeValue_TimeValue"));
    }

  }
}
