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
using Remotion.ObjectBinding.Web.UnitTests.Domain;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocDropDownMenuTest : BocTest
  {
    private BocDropDownMenu _bocDropDownMenu;
    private TypeWithReference _businessObject;
    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectReferenceProperty _propertyReferenceValue;

    public BocDropDownMenuTest ()
    {
    }


    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocDropDownMenu = new BocDropDownMenu();
      _bocDropDownMenu.ID = "BocDropDownMenu";
      NamingContainer.Controls.Add(_bocDropDownMenu);

      _businessObject = TypeWithReference.Create();

      _propertyReferenceValue =
          (IBusinessObjectReferenceProperty)((IBusinessObject)_businessObject).BusinessObjectClass.GetPropertyDefinition("ReferenceValue");

      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject)_businessObject;
    }

    [Test]
    public void SetValueToObject ()
    {
      IBusinessObject referencedObject = (IBusinessObject)TypeWithReference.Create();
      _bocDropDownMenu.Value = referencedObject;
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(referencedObject));
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocDropDownMenu.Value = null;
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(null));
    }


    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocDropDownMenu.Value = (IBusinessObject)TypeWithReference.Create();
      Assert.That(_bocDropDownMenu.HasValue, Is.True);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocDropDownMenu.Value = null;
      Assert.That(_bocDropDownMenu.HasValue, Is.False);
    }


    [Test]
    public void LoadValueAndInterimTrueWithObject ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Property = _propertyReferenceValue;
      _bocDropDownMenu.Value = null;

      _bocDropDownMenu.LoadValue(true);
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(_businessObject.ReferenceValue));
    }

    [Test]
    public void LoadValueAndInterimTrueWithObjectAndNoProperty ()
    {
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Value = null;

      _bocDropDownMenu.LoadValue(true);
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(_businessObject));
    }

    [Test]
    public void LoadValueAndInterimTrueWithNull ()
    {
      _businessObject.ReferenceValue = null;
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Property = _propertyReferenceValue;
      _bocDropDownMenu.Value = (IBusinessObject)TypeWithReference.Create();

      _bocDropDownMenu.LoadValue(true);
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(_businessObject.ReferenceValue));
    }

    [Test]
    public void LoadValueAndInterimFalseWithObject ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Property = _propertyReferenceValue;
      _bocDropDownMenu.Value = null;

      _bocDropDownMenu.LoadValue(false);
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(_businessObject.ReferenceValue));
    }


    [Test]
    public void LoadValueAndInterimFalseWithObjectAndNoProperty ()
    {
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Value = null;

      _bocDropDownMenu.LoadValue(false);
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(_businessObject));
    }

    [Test]
    public void LoadValueAndInterimFalseWithNull ()
    {
      _businessObject.ReferenceValue = null;
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Property = _propertyReferenceValue;
      _bocDropDownMenu.Value = (IBusinessObject)TypeWithReference.Create();

      _bocDropDownMenu.LoadValue(false);
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(_businessObject.ReferenceValue));
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      var value = (IBusinessObjectWithIdentity)TypeWithReference.Create();
      _bocDropDownMenu.DataSource = null;
      _bocDropDownMenu.Property = _propertyReferenceValue;
      _bocDropDownMenu.Value = value;

      _bocDropDownMenu.LoadValue(false);
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(value));
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocDropDownMenu.DataSource = _dataSource;
      _bocDropDownMenu.Property = _propertyReferenceValue;
      _bocDropDownMenu.Value = (IBusinessObjectWithIdentity)TypeWithReference.Create();

      _bocDropDownMenu.LoadValue(false);
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(null));
    }


    [Test]
    public void LoadUnboundValueAndInterimTrueWithObject ()
    {
      IBusinessObject value = (IBusinessObject)TypeWithReference.Create();
      _bocDropDownMenu.Value = null;

      _bocDropDownMenu.LoadUnboundValue(value, true);
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(value));
    }

    [Test]
    public void LoadUnboundValueAndInterimTrueWithNull ()
    {
      IBusinessObject value = null;
      _bocDropDownMenu.Value = (IBusinessObject)TypeWithReference.Create();

      _bocDropDownMenu.LoadUnboundValue(value, true);
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(value));
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithObject ()
    {
      IBusinessObject value = (IBusinessObject)TypeWithReference.Create();
      _bocDropDownMenu.Value = null;

      _bocDropDownMenu.LoadUnboundValue(value, false);
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(value));
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      IBusinessObject value = null;
      _bocDropDownMenu.Value = (IBusinessObject)TypeWithReference.Create();

      _bocDropDownMenu.LoadUnboundValue(value, false);
      Assert.That(_bocDropDownMenu.Value, Is.EqualTo(value));
    }
  }
}
