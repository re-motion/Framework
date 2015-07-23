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
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocReferenceValueTest : BocTest
  {
    private class GetObjectService : IGetObjectService
    {
      private readonly IBusinessObjectWithIdentity _objectToReturn;

      public GetObjectService (IBusinessObjectWithIdentity objectToReturn)
      {
        _objectToReturn = objectToReturn;
      }

      public IBusinessObjectWithIdentity GetObject (BindableObjectClassWithIdentity classWithIdentity, string uniqueIdentifier)
      {
        return _objectToReturn;
      }
    }

    private BocReferenceValueMock _bocReferenceValue;
    private TypeWithReference _businessObject;
    private IBusinessObjectDataSource _dataSource;
    private IBusinessObjectReferenceProperty _propertyReferenceValue;

    public BocReferenceValueTest ()
    {
    }

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocReferenceValue = new BocReferenceValueMock();
      _bocReferenceValue.ID = "BocReferenceValue";
      _bocReferenceValue.ShowOptionsMenu = false;
      _bocReferenceValue.Command.Type = CommandType.None;
      _bocReferenceValue.Command.Show = CommandShow.Always;
      _bocReferenceValue.InternalValue = Guid.Empty.ToString();
      NamingContainer.Controls.Add (_bocReferenceValue);

      _businessObject = TypeWithReference.Create();

      _propertyReferenceValue =
          (IBusinessObjectReferenceProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue");

      _dataSource = new StubDataSource (((IBusinessObject) _businessObject).BusinessObjectClass);
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;

      ((IBusinessObject) _businessObject).BusinessObjectClass.BusinessObjectProvider.AddService<IGetObjectService>
          (new GetObjectService ((IBusinessObjectWithIdentity) TypeWithReference.Create()));
      ((IBusinessObject) _businessObject).BusinessObjectClass.BusinessObjectProvider.AddService<IBusinessObjectWebUIService>
          (new ReflectionBusinessObjectWebUIService());
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelUndefined ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
      _bocReferenceValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.False);
      Assert.That (WcagHelperMock.HasError, Is.False);
    }

    [Test]
    public void EvaluateWaiConformityLevelA ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _bocReferenceValue.ShowOptionsMenu = true;
      _bocReferenceValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.False);
      Assert.That (WcagHelperMock.HasError, Is.False);
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelAWithAutoPostBackTrue ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocReferenceValue.DropDownListStyle.AutoPostBack = true;
      _bocReferenceValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.True);
      Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
      Assert.That (WcagHelperMock.Control, Is.SameAs (_bocReferenceValue));
      Assert.That (WcagHelperMock.Property, Is.EqualTo ("DropDownListStyle.AutoPostBack"));
    }


    [Test]
    public void EvaluateWaiConformityDebugExceptionLevelAWithShowOptionsMenuTrue ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocReferenceValue.ShowOptionsMenu = true;
      _bocReferenceValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasError, Is.True);
      Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
      Assert.That (WcagHelperMock.Control, Is.SameAs (_bocReferenceValue));
      Assert.That (WcagHelperMock.Property, Is.EqualTo ("ShowOptionsMenu"));
    }


    [Test]
    public void IsOptionsMenuInvisibleWithWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _bocReferenceValue.ShowOptionsMenu = true;
      _bocReferenceValue.OptionsMenuItems.Add (new WebMenuItem());
      Assert.That (_bocReferenceValue.HasOptionsMenu, Is.False);
    }

    [Test]
    public void IsOptionsMenuVisibleWithoutWcagOverride ()
    {
      ControlInvoker invoker = new ControlInvoker (_bocReferenceValue);
      invoker.InitRecursive();

      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
      _bocReferenceValue.ShowOptionsMenu = true;
      _bocReferenceValue.OptionsMenuItems.Add (new WebMenuItem());
      Assert.That (_bocReferenceValue.HasOptionsMenu, Is.True);
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelAWithEventCommand ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocReferenceValue.Command.Type = CommandType.Event;
      _bocReferenceValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasError, Is.True);
      Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
      Assert.That (WcagHelperMock.Control, Is.SameAs (_bocReferenceValue));
      Assert.That (WcagHelperMock.Property, Is.EqualTo ("Command"));
    }

    [Test]
    public void IsEventCommandDisabledWithWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _bocReferenceValue.Command.Type = CommandType.Event;
      Assert.That (((IBocReferenceValueBase) _bocReferenceValue).IsCommandEnabled(), Is.False);
    }

    [Test]
    public void IsEventCommandEnabledWithoutWcagOverride ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _bocReferenceValue.DataSource = _dataSource;
      _bocReferenceValue.Property = _propertyReferenceValue;
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
      _bocReferenceValue.Command.Type = CommandType.Event;
      Assert.That (((IBocReferenceValueBase) _bocReferenceValue).IsCommandEnabled(), Is.True);
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelAWithWxeFunctionCommand ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocReferenceValue.Command.Type = CommandType.WxeFunction;
      _bocReferenceValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasError, Is.True);
      Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
      Assert.That (WcagHelperMock.Control, Is.SameAs (_bocReferenceValue));
      Assert.That (WcagHelperMock.Property, Is.EqualTo ("Command"));
    }

    [Test]
    public void IsWxeFunctionCommandDisabledWithWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _bocReferenceValue.Command.Type = CommandType.WxeFunction;
      Assert.That (((IBocReferenceValueBase) _bocReferenceValue).IsCommandEnabled(), Is.False);
    }

    [Test]
    public void IsWxeFunctionCommandEnabledWithoutWcagOverride ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _bocReferenceValue.DataSource = _dataSource;
      _bocReferenceValue.Property = _propertyReferenceValue;
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
      _bocReferenceValue.Command.Type = CommandType.WxeFunction;
      Assert.That (((IBocReferenceValueBase) _bocReferenceValue).IsCommandEnabled(), Is.True);
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelAWithHrefCommand ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocReferenceValue.Command.Type = CommandType.Href;
      _bocReferenceValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.False);
      Assert.That (WcagHelperMock.HasError, Is.False);
    }

    [Test]
    public void EvaluateWaiConformityDebugLevelAWithoutCommand ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocReferenceValue.Command = null;
      _bocReferenceValue.EvaluateWaiConformity();

      Assert.That (WcagHelperMock.HasWarning, Is.False);
      Assert.That (WcagHelperMock.HasError, Is.False);
    }


    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _bocReferenceValue.ReadOnly = true;
      string[] actual = _bocReferenceValue.GetTrackedClientIDs();
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (0));
    }

    [Test]
    public void GetTrackedClientIDsInEditMode ()
    {
      _bocReferenceValue.ReadOnly = false;
      string[] actual = _bocReferenceValue.GetTrackedClientIDs();
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (1));
      Assert.That (actual[0], Is.EqualTo (((IBocReferenceValue)_bocReferenceValue).GetValueName()));
    }


    [Test]
    public void SetValueToObject ()
    {
      IBusinessObjectWithIdentity referencedObject = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _bocReferenceValue.IsDirty = false;
      _bocReferenceValue.Value = referencedObject;
      Assert.That (_bocReferenceValue.Value, Is.EqualTo (referencedObject));
      Assert.That (_bocReferenceValue.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocReferenceValue.IsDirty = false;
      _bocReferenceValue.Value = null;
      Assert.That (_bocReferenceValue.Value, Is.EqualTo (null));
      Assert.That (_bocReferenceValue.IsDirty, Is.True);
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      IBusinessObjectWithIdentity referencedObject = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _bocReferenceValue.Value = referencedObject;
      Assert.That (_bocReferenceValue.HasValue, Is.True);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocReferenceValue.Value = null;
      Assert.That (_bocReferenceValue.HasValue, Is.False);
    }

    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _bocReferenceValue.DataSource = _dataSource;
      _bocReferenceValue.Property = _propertyReferenceValue;
      _bocReferenceValue.Value = null;
      _bocReferenceValue.IsDirty = true;

      _bocReferenceValue.LoadValue (true);
      Assert.That (_bocReferenceValue.Value, Is.EqualTo (null));
      Assert.That (_bocReferenceValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithObject ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _bocReferenceValue.DataSource = _dataSource;
      _bocReferenceValue.Property = _propertyReferenceValue;
      _bocReferenceValue.Value = null;
      _bocReferenceValue.IsDirty = true;

      _bocReferenceValue.LoadValue (false);
      Assert.That (_bocReferenceValue.Value, Is.EqualTo (_businessObject.ReferenceValue));
      Assert.That (_bocReferenceValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithNull ()
    {
      _businessObject.ReferenceValue = null;
      _bocReferenceValue.DataSource = _dataSource;
      _bocReferenceValue.Property = _propertyReferenceValue;
      _bocReferenceValue.Value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _bocReferenceValue.IsDirty = true;

      _bocReferenceValue.LoadValue (false);
      Assert.That (_bocReferenceValue.Value, Is.EqualTo (_businessObject.ReferenceValue));
      Assert.That (_bocReferenceValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      var value = (IBusinessObjectWithIdentity) TypeWithReference.Create ();
      _bocReferenceValue.DataSource = null;
      _bocReferenceValue.Property = _propertyReferenceValue;
      _bocReferenceValue.Value = value;
      _bocReferenceValue.IsDirty = true;

      _bocReferenceValue.LoadValue (false);
      Assert.That (_bocReferenceValue.Value, Is.EqualTo (value));
      Assert.That (_bocReferenceValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      var value = (IBusinessObjectWithIdentity) TypeWithReference.Create ();
      _bocReferenceValue.DataSource = _dataSource;
      _bocReferenceValue.Property = null;
      _bocReferenceValue.Value = value;
      _bocReferenceValue.IsDirty = true;

      _bocReferenceValue.LoadValue (false);
      Assert.That (_bocReferenceValue.Value, Is.EqualTo (value));
      Assert.That (_bocReferenceValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocReferenceValue.DataSource = _dataSource;
      _bocReferenceValue.Property = _propertyReferenceValue;
      _bocReferenceValue.Value = (IBusinessObjectWithIdentity) TypeWithReference.Create ();
      _bocReferenceValue.IsDirty = true;

      _bocReferenceValue.LoadValue (false);
      Assert.That (_bocReferenceValue.Value, Is.EqualTo (null));
      Assert.That (_bocReferenceValue.IsDirty, Is.False);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      IBusinessObjectWithIdentity value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _bocReferenceValue.Property = _propertyReferenceValue;
      _bocReferenceValue.Value = null;
      _bocReferenceValue.IsDirty = true;

      _bocReferenceValue.LoadUnboundValue (value, true);
      Assert.That (_bocReferenceValue.Value, Is.EqualTo (null));
      Assert.That (_bocReferenceValue.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithObject ()
    {
      IBusinessObjectWithIdentity value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _bocReferenceValue.Property = _propertyReferenceValue;
      _bocReferenceValue.Value = null;
      _bocReferenceValue.IsDirty = true;

      _bocReferenceValue.LoadUnboundValue (value, false);
      Assert.That (_bocReferenceValue.Value, Is.EqualTo (value));
      Assert.That (_bocReferenceValue.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      IBusinessObjectWithIdentity value = null;
      _bocReferenceValue.Property = _propertyReferenceValue;
      _bocReferenceValue.Value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _bocReferenceValue.IsDirty = true;

      _bocReferenceValue.LoadUnboundValue (value, false);
      Assert.That (_bocReferenceValue.Value, Is.EqualTo (value));
      Assert.That (_bocReferenceValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndInterimTrue ()
    {
      var value = TypeWithReference.Create ();
      _businessObject.ReferenceValue = value;
      _bocReferenceValue.DataSource = _dataSource;
      _bocReferenceValue.Property = _propertyReferenceValue;
      _bocReferenceValue.Value = null;
      _bocReferenceValue.IsDirty = true;

      var result = _bocReferenceValue.SaveValue (true);
      Assert.That (result, Is.False);
      Assert.That (_businessObject.ReferenceValue, Is.EqualTo (value));
      Assert.That (_bocReferenceValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndInterimFalse ()
    {
      var value = TypeWithReference.Create ();
      _businessObject.ReferenceValue = value;
      _bocReferenceValue.DataSource = _dataSource;
      _bocReferenceValue.Property = _propertyReferenceValue;
      _bocReferenceValue.Value = null;
      _bocReferenceValue.IsDirty = true;

      var result = _bocReferenceValue.SaveValue (false);
      Assert.That (result, Is.True);
      Assert.That (_businessObject.ReferenceValue, Is.EqualTo (null));
      Assert.That (_bocReferenceValue.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndNotValid ()
    {
      var value = TypeWithReference.Create ();
      _businessObject.ReferenceValue = value;
      _bocReferenceValue.DataSource = _dataSource;
      _bocReferenceValue.Property = _propertyReferenceValue;
      _bocReferenceValue.Value = null;
      _bocReferenceValue.IsDirty = true;
      _bocReferenceValue.RegisterValidator (new AlwaysInvalidValidator());

      var result = _bocReferenceValue.SaveValue (false);
      Assert.That (result, Is.False);
      Assert.That (_businessObject.ReferenceValue, Is.EqualTo (value));
      Assert.That (_bocReferenceValue.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      var value = TypeWithReference.Create ();
      _businessObject.ReferenceValue = value;
      _bocReferenceValue.DataSource = _dataSource;
      _bocReferenceValue.Property = _propertyReferenceValue;
      _bocReferenceValue.Value = null;
      _bocReferenceValue.IsDirty = false;

      var result = _bocReferenceValue.SaveValue (false);
      Assert.That (result, Is.True);
      Assert.That (_businessObject.ReferenceValue, Is.EqualTo (value));
      Assert.That (_bocReferenceValue.IsDirty, Is.False);
    }

    [Test]
    public void GetValidationValue_ValueSet ()
    {
        var value = TypeWithReference.Create ("Name");
      _bocReferenceValue.Value = (IBusinessObjectWithIdentity) value;

      Assert.That (_bocReferenceValue.ValidationValue, Is.EqualTo (value.UniqueIdentifier));
    }

    [Test]
    public void GetValidationValue_ValueNull ()
    {
      _bocReferenceValue.Value = null;

      Assert.That (_bocReferenceValue.ValidationValue, Is.Null);
    }

    [Test]
    public void GetValueName ()
    {
      Assert.That (((IBocReferenceValue)_bocReferenceValue).GetValueName(), Is.EqualTo ("NamingContainer_BocReferenceValue_Value"));
    }
  }
}