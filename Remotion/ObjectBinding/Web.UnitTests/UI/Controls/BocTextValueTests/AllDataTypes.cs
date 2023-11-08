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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocTextValueTests
{
  [TestFixture]
  public class AllDataTypes : BocTest
  {
    private BocTextValue _bocTextValue;
    private IBusinessObject _businessObject;
    private BindableObjectDataSourceControl _dataSource;
    private CultureScope _cultureScope;

    public override void SetUp ()
    {
      base.SetUp();
      _bocTextValue = new BocTextValue();
      _bocTextValue.ID = "BocTextValue";
      NamingContainer.Controls.Add(_bocTextValue);

      _businessObject = (IBusinessObject)TypeWithAllDataTypes.Create();

      _dataSource = new BindableObjectDataSourceControl();
      _dataSource.Type = typeof(TypeWithAllDataTypes);
      _dataSource.BusinessObject = _businessObject;

      _cultureScope = new CultureScope("");
    }

    public override void TearDown ()
    {
      base.TearDown();
      _cultureScope.Dispose();
    }

    [Test]
    public void LoadAndSaveValue_WithString ()
    {
      LoadAndSaveValue("String", "Foo", "Bar");
    }

    [Test]
    public void LoadAndSaveValue_WithByte ()
    {
      LoadAndSaveValue("Byte", (Byte)1, (Byte)2);
    }

    [Test]
    public void LoadAndSaveValue_WithByte_NumberFormat ()
    {
      LoadAndSaveValue("Byte", (Byte)1, (Byte)100, " +100 ");
    }

    [Test]
    public void LoadAndSaveValue_WithInt16 ()
    {
      LoadAndSaveValue("Int16", (Int16)1, (Int16)2);
    }

    [Test]
    public void LoadAndSaveValue_WithInt16_NumberFormat ()
    {
      LoadAndSaveValue("Int16", (Int16)1, (Int16)20000, " +20,000 ");
    }

    [Test]
    public void LoadAndSaveValue_WithInt32 ()
    {
      LoadAndSaveValue("Int32", 1, 2);
    }

    [Test]
    public void LoadAndSaveValue_WithInt23_NumberFormat ()
    {
      LoadAndSaveValue("Int32", 1, 20000, " +20,000 ");
    }

    [Test]
    public void LoadAndSaveValue_WithInt64 ()
    {
      LoadAndSaveValue("Int64", 1L, 2L);
    }

    [Test]
    public void LoadAndSaveValue_WithInt64_NumberFormat ()
    {
      LoadAndSaveValue("Int64", 1L, 20000001L, " +20,000,001 ");
    }

    [Test]
    public void LoadAndSaveValue_WithDecimal ()
    {
      LoadAndSaveValue("Decimal", 1.1m, 2.1m);
    }

    [Test]
    public void LoadAndSaveValue_WithDecimal_NumberFormat ()
    {
      LoadAndSaveValue("Decimal", 1m, 20000001.456m, " +20,000,001.456 ");
    }

    [Test]
    public void LoadAndSaveValue_WithDouble ()
    {
      LoadAndSaveValue("Double", 1.1, 2.1);
    }

    [Test]
    public void LoadAndSaveValue_WithDouble_NumberFormat ()
    {
      LoadAndSaveValue("Double", 1.1, 20000001.456, " +20,000,001.456 ");
    }

    [Test]
    public void LoadAndSaveValue_WithDouble_Exponent ()
    {
      LoadAndSaveValue("Single", 1.1f, 20234001.45E3f, " +20,234,001.45E3 ");
    }

    [Test]
    public void LoadAndSaveValue_WithSingle ()
    {
      LoadAndSaveValue("Single", 1.1f, 2.1f);
    }

    [Test]
    public void LoadAndSaveValue_WithSingle_NumberFormat ()
    {
      LoadAndSaveValue("Single", 1.1f, 20001.456f, " +20,001.456 ");
    }

    [Test]
    public void LoadAndSaveValue_WithSingle_Exponent ()
    {
      LoadAndSaveValue("Single", 1.1f, 20001.45E3f, " +20,001.45E3 ");
    }

    [Test]
    public void LoadAndSaveValue_WithDate ()
    {
      LoadAndSaveValue("Date", new DateTime(2000, 1, 1).Date, new DateTime(2000, 1, 2).Date);
    }

    [Test]
    public void LoadAndSaveValue_WithDateTime ()
    {
      LoadAndSaveValue("DateTime", new DateTime(2000, 1, 1, 1, 1, 0), new DateTime(2000, 1, 2, 1, 1, 0));
    }

    private void LoadAndSaveValue<T> (string propertyidentifier, T initialValue, T newValue, string newValueAsString = null)
    {
      _businessObject.SetProperty(propertyidentifier, initialValue);
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _businessObject.BusinessObjectClass.GetPropertyDefinition(propertyidentifier);

      _bocTextValue.LoadValue(false);
      Assert.That(_bocTextValue.Value, Is.EqualTo(initialValue));
      Assert.That(_bocTextValue.IsDirty, Is.False);

      _bocTextValue.Text = newValueAsString ?? newValue.ToString();
      Assert.That(_bocTextValue.IsDirty, Is.True);

      _bocTextValue.SaveValue(false);
      Assert.That(_bocTextValue.Value, Is.EqualTo(newValue));
      Assert.That(_bocTextValue.IsDirty, Is.False);
      _bocTextValue.SaveValue(false);

      Assert.That(_businessObject.GetProperty(propertyidentifier), Is.EqualTo(newValue));
    }
  }
}
