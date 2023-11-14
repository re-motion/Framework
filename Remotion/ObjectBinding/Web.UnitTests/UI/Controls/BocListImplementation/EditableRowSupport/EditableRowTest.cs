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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.EditableRowSupport
{
  [TestFixture]
  public class EditableRowTest : BocTest
  {
    private FakeEditModeHost _editModeHost;

    private EditableRow _editableRow;

    private IBusinessObject _value01;

    private BindableObjectClass _typeWithAllDataTypesClass;

    private IBusinessObjectPropertyPath _typeWithAllDataTypesStringValuePath;
    private IBusinessObjectPropertyPath _typeWithAllDataTypesInt32ValuePath;

    private BocSimpleColumnDefinition _typeWithAllDataTypesStringValueSimpleColumn;
    private BocSimpleColumnDefinition _typeWithAllDataTypesStringValueSimpleColumnAsDynamic;
    private BocSimpleColumnDefinition _typeWithAllDataTypesInt32ValueSimpleColumn;

    private BocCompoundColumnDefinition _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
    private BocCustomColumnDefinition _typeWithAllDataTypesStringValueCustomColumn;
    private BocCommandColumnDefinition _commandColumn;
    private BocRowEditModeColumnDefinition _rowEditModeColumn;
    private BocDropDownMenuColumnDefinition _dropDownMenuColumn;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _editModeHost = new FakeEditModeHost();
      _editModeHost.ID = "BocList";
      _editModeHost.RowIDProvider = new FakeRowIDProvider();
      _editModeHost.EditModeControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();
      _editModeHost.EditModeDataSourceFactory = new EditableRowDataSourceFactory();
      _editModeHost.EnableOptionalValidators = null;

      _editableRow = new EditableRow(_editModeHost);
      _editableRow.ID = "Row";
      NamingContainer.Controls.Add(_editableRow);

      _value01 = (IBusinessObject)TypeWithAllDataTypes.Create("A", 1);

      _typeWithAllDataTypesClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(TypeWithAllDataTypes));

      _typeWithAllDataTypesStringValuePath = BusinessObjectPropertyPath.CreateStatic(_typeWithAllDataTypesClass, "String");
      _typeWithAllDataTypesInt32ValuePath = BusinessObjectPropertyPath.CreateStatic(_typeWithAllDataTypesClass, "Int32");

      _typeWithAllDataTypesStringValueSimpleColumn = new BocSimpleColumnDefinition();
      _typeWithAllDataTypesStringValueSimpleColumn.SetPropertyPath(_typeWithAllDataTypesStringValuePath);

      _typeWithAllDataTypesStringValueSimpleColumnAsDynamic = new BocSimpleColumnDefinition();
      _typeWithAllDataTypesStringValueSimpleColumnAsDynamic.SetPropertyPath(BusinessObjectPropertyPath.CreateDynamic("StringValue"));
      _typeWithAllDataTypesStringValueSimpleColumnAsDynamic.IsDynamic = true;

      _typeWithAllDataTypesInt32ValueSimpleColumn = new BocSimpleColumnDefinition();
      _typeWithAllDataTypesInt32ValueSimpleColumn.SetPropertyPath(_typeWithAllDataTypesInt32ValuePath);

      _typeWithAllDataTypesStringValueFirstValueCompoundColumn = new BocCompoundColumnDefinition();
      _typeWithAllDataTypesStringValueFirstValueCompoundColumn.PropertyPathBindings.Add(
          new PropertyPathBinding(_typeWithAllDataTypesStringValuePath));
      _typeWithAllDataTypesStringValueFirstValueCompoundColumn.PropertyPathBindings.Add(
          new PropertyPathBinding(_typeWithAllDataTypesStringValuePath));
      _typeWithAllDataTypesStringValueFirstValueCompoundColumn.FormatString = "{0}, {1}";

      _typeWithAllDataTypesStringValueCustomColumn = new BocCustomColumnDefinition();
      _typeWithAllDataTypesStringValueCustomColumn.SetPropertyPath(_typeWithAllDataTypesStringValuePath);
      _typeWithAllDataTypesStringValueCustomColumn.IsSortable = true;

      _commandColumn = new BocCommandColumnDefinition();
      _rowEditModeColumn = new BocRowEditModeColumnDefinition();
      _dropDownMenuColumn = new BocDropDownMenuColumnDefinition();
    }


    [Test]
    public void Initialize ()
    {
      Assert.That(_editableRow.DataSourceFactory, Is.Null);
      Assert.That(_editableRow.ControlFactory, Is.Null);
    }


    [Test]
    public void CreateControlsWithEmptyColumns ()
    {
      Invoker.InitRecursive();

      Assert.That(_editableRow.HasEditControls(), Is.False);
      Assert.That(_editableRow.HasValidators(), Is.False);

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      _editableRow.CreateControls(_value01, new BocColumnDefinition[0]);

      Assert.That(_editableRow.HasEditControls(), Is.True);
      Assert.That(_editableRow.HasValidators(), Is.True);

      Assert.That(_editableRow.DataSourceFactory, Is.Not.Null);
      Assert.That(_editableRow.ControlFactory, Is.Not.Null);

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      Assert.That(dataSource, Is.Not.Null);
      Assert.That(dataSource.BusinessObject, Is.SameAs(_value01));
    }

    [Test]
    public void CreateControlsWithColumns ()
    {
      Invoker.InitRecursive();

      Assert.That(_editableRow.HasEditControls(), Is.False);
      Assert.That(_editableRow.HasValidators(), Is.False);
      Assert.That(_editableRow.HasEditControl(0), Is.False);

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[8];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
      columns[2] = _typeWithAllDataTypesStringValueCustomColumn;
      columns[3] = _commandColumn;
      columns[4] = _rowEditModeColumn;
      columns[5] = _dropDownMenuColumn;
      columns[6] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[7] = _typeWithAllDataTypesStringValueSimpleColumnAsDynamic;

      _editableRow.CreateControls(_value01, columns);

      Assert.That(_editableRow.HasEditControls(), Is.True);
      Assert.That(_editableRow.HasValidators(), Is.True);

      Assert.That(_editableRow.DataSourceFactory, Is.Not.Null);
      Assert.That(_editableRow.ControlFactory, Is.Not.Null);

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      Assert.That(dataSource, Is.Not.Null);
      Assert.That(dataSource.BusinessObject, Is.SameAs(_value01));

      Assert.That(_editableRow.HasEditControl(0), Is.True);
      Assert.That(_editableRow.HasEditControl(1), Is.False);
      Assert.That(_editableRow.HasEditControl(2), Is.False);
      Assert.That(_editableRow.HasEditControl(3), Is.False);
      Assert.That(_editableRow.HasEditControl(4), Is.False);
      Assert.That(_editableRow.HasEditControl(5), Is.False);
      Assert.That(_editableRow.HasEditControl(6), Is.True);
      Assert.That(_editableRow.HasEditControl(7), Is.False);

      IBusinessObjectBoundEditableWebControl textBoxFirstValue = _editableRow.GetEditControl(0);
      Assert.That(textBoxFirstValue is BocTextValue, Is.True);
      Assert.That(textBoxFirstValue.DataSource, Is.SameAs(dataSource));
      Assert.That(textBoxFirstValue.Property, Is.SameAs(_typeWithAllDataTypesStringValuePath.Properties.Last()));
      Assert.That(((BocTextValue)textBoxFirstValue).EnableOptionalValidators, Is.Null);

      IBusinessObjectBoundEditableWebControl textBoxSecondValue = _editableRow.GetEditControl(6);
      Assert.That(textBoxSecondValue is BocTextValue, Is.True);
      Assert.That(textBoxSecondValue.DataSource, Is.SameAs(dataSource));
      Assert.That(textBoxSecondValue.Property, Is.SameAs(_typeWithAllDataTypesInt32ValuePath.Properties.Last()));
      Assert.That(((BocTextValue)textBoxSecondValue).EnableOptionalValidators, Is.Null);
    }

    [Test]
    public void CreateControlsDataSourceFactoryNull ()
    {
      Invoker.InitRecursive();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();
      Assert.That(
          () => _editableRow.CreateControls(_value01, new BocColumnDefinition[0]),
          Throws.InvalidOperationException
              .With.Message.EqualTo("BocList 'BocList': DataSourceFactory has not been set prior to invoking CreateControls()."));
    }

    [Test]
    public void CreateControlsControlFactoryNull ()
    {
      Invoker.InitRecursive();
      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      Assert.That(
          () => _editableRow.CreateControls(_value01, new BocColumnDefinition[0]),
          Throws.InvalidOperationException
              .With.Message.EqualTo("BocList 'BocList': ControlFactory has not been set prior to invoking CreateControls()."));
    }

    [Test]
    public void CreateControlsWithEnableOptionalValidatorsTrue ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();
      _editModeHost.EnableOptionalValidators = true;

      BocColumnDefinition[] columns = new BocColumnDefinition[1];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;

      _editableRow.CreateControls(_value01, columns);

      Assert.That(_editableRow.HasEditControl(0), Is.True);

      IBusinessObjectBoundEditableWebControl textBoxFirstValue = _editableRow.GetEditControl(0);
      Assert.That(textBoxFirstValue is BocTextValue, Is.True);
      Assert.That(((BocTextValue)textBoxFirstValue).EnableOptionalValidators, Is.True);
    }

    [Test]
    public void CreateControlsWithEnableOptionalValidatorsFalse ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();
      _editModeHost.EnableOptionalValidators = false;

      BocColumnDefinition[] columns = new BocColumnDefinition[1];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;

      _editableRow.CreateControls(_value01, columns);

      Assert.That(_editableRow.HasEditControl(0), Is.True);

      IBusinessObjectBoundEditableWebControl textBoxFirstValue = _editableRow.GetEditControl(0);
      Assert.That(textBoxFirstValue is BocTextValue, Is.True);
      Assert.That(((BocTextValue)textBoxFirstValue).EnableOptionalValidators, Is.False);
    }

    [Test]
    public void EnsureValidators ()
    {
      Invoker.InitRecursive();

      Assert.That(_editableRow.HasValidators(), Is.False);

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[7];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
      columns[2] = _typeWithAllDataTypesStringValueCustomColumn;
      columns[3] = _commandColumn;
      columns[4] = _rowEditModeColumn;
      columns[5] = _dropDownMenuColumn;
      columns[6] = _typeWithAllDataTypesInt32ValueSimpleColumn;

      _editableRow.CreateControls(_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      Assert.That(_editableRow.HasValidators(), Is.True);

      Assert.That(_editableRow.HasValidators(0), Is.True);
      Assert.That(_editableRow.HasValidators(1), Is.False);
      Assert.That(_editableRow.HasValidators(2), Is.False);
      Assert.That(_editableRow.HasValidators(3), Is.False);
      Assert.That(_editableRow.HasValidators(4), Is.False);
      Assert.That(_editableRow.HasValidators(5), Is.False);
      Assert.That(_editableRow.HasValidators(6), Is.True);

      ControlCollection validators0 = _editableRow.GetValidators(0);
      Assert.That(validators0.Cast<BaseValidator>().Select(v => v.GetType()),
          Is.EqualTo(
              new[]
              {
                  typeof(ControlCharactersCharactersValidator),
                  typeof(BusinessObjectBoundEditableWebControlValidationResultDispatchingValidator)
              }));

      ControlCollection validators6 = _editableRow.GetValidators(6);
      Assert.That(
          validators6.Cast<BaseValidator>().Select(v => v.GetType()),
          Is.EqualTo(
              new[]
              {
                  typeof(RequiredFieldValidator),
                  typeof(NumericValidator),
                  typeof(BusinessObjectBoundEditableWebControlValidationResultDispatchingValidator)
              }));
    }

    [Test]
    public void EnsureValidatorsWithoutCreateControls ()
    {
      Invoker.InitRecursive();

      Assert.That(_editableRow.HasValidators(), Is.False);

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      _editableRow.EnsureValidatorsRestored();

      Assert.That(_editableRow.HasValidators(), Is.False);

      BocColumnDefinition[] columns = new BocColumnDefinition[7];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
      columns[2] = _typeWithAllDataTypesStringValueCustomColumn;
      columns[3] = _commandColumn;
      columns[4] = _rowEditModeColumn;
      columns[5] = _dropDownMenuColumn;
      columns[6] = _typeWithAllDataTypesInt32ValueSimpleColumn;

      _editableRow.CreateControls(_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      Assert.That(_editableRow.HasValidators(), Is.True);

      Assert.That(_editableRow.HasValidators(0), Is.True);
      Assert.That(_editableRow.HasValidators(1), Is.False);
      Assert.That(_editableRow.HasValidators(2), Is.False);
      Assert.That(_editableRow.HasValidators(3), Is.False);
      Assert.That(_editableRow.HasValidators(4), Is.False);
      Assert.That(_editableRow.HasValidators(5), Is.False);
      Assert.That(_editableRow.HasValidators(6), Is.True);

      ControlCollection validators0 = _editableRow.GetValidators(0);
      Assert.That(validators0, Is.Not.Null);
      Assert.That(
          validators0.Cast<BaseValidator>().Select(v => v.GetType()),
          Is.EqualTo(new[] { typeof(ControlCharactersCharactersValidator), typeof(BusinessObjectBoundEditableWebControlValidationResultDispatchingValidator) }));

      ControlCollection validators6 = _editableRow.GetValidators(6);
      Assert.That(validators6.Cast<BaseValidator>().Select(v => v.GetType()),
          Is.EqualTo(
              new[]
              {
                  typeof(RequiredFieldValidator),
                  typeof(NumericValidator),
                  typeof(BusinessObjectBoundEditableWebControlValidationResultDispatchingValidator)
              }));
    }

    [Test]
    public void ControlInit ()
    {
      Assert.That(_editableRow.HasControls(), Is.False);
      Assert.That(_editableRow.HasEditControls(), Is.False);
      Assert.That(_editableRow.HasValidators(), Is.False);

      Invoker.InitRecursive();

      Assert.That(_editableRow.HasControls(), Is.False);
      Assert.That(_editableRow.HasEditControls(), Is.False);
      Assert.That(_editableRow.HasValidators(), Is.False);
    }

    [Test]
    public void ControlLoad ()
    {
      Assert.That(_editableRow.HasControls(), Is.False);
      Assert.That(_editableRow.HasEditControls(), Is.False);
      Assert.That(_editableRow.HasValidators(), Is.False);

      Invoker.LoadRecursive();

      Assert.That(_editableRow.HasControls(), Is.False);
      Assert.That(_editableRow.HasEditControls(), Is.False);
      Assert.That(_editableRow.HasValidators(), Is.False);
    }

    [Test]
    public void LoadValue ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;

      _editableRow.CreateControls(_value01, columns);

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues(false);

      BocTextValue textBoxStringValue = (BocTextValue)_editableRow.GetEditControl(0);
      BocTextValue textBoxInt32Value = (BocTextValue)_editableRow.GetEditControl(1);

      Assert.That(textBoxStringValue.Value, Is.EqualTo("A"));
      Assert.That(textBoxInt32Value.Value, Is.EqualTo(1));
    }

    [Test]
    public void SaveValue ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;

      _editableRow.CreateControls(_value01, columns);

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues(false);

      BocTextValue textBoxStringValue = (BocTextValue)_editableRow.GetEditControl(0);
      BocTextValue textBoxInt32Value = (BocTextValue)_editableRow.GetEditControl(1);

      Assert.That(textBoxStringValue.Value, Is.EqualTo("A"));
      Assert.That(textBoxInt32Value.Value, Is.EqualTo(1));

      textBoxStringValue.Value = "New Value A";
      textBoxInt32Value.Value = "100";

      dataSource.SaveValues(false);

      Assert.That(((TypeWithAllDataTypes)_value01).String, Is.EqualTo("New Value A"));
      Assert.That(((TypeWithAllDataTypes)_value01).Int32, Is.EqualTo(100));
    }


    [Test]
    public void HasEditControl ()
    {
      Invoker.InitRecursive();

      Assert.That(_editableRow.HasEditControls(), Is.False);
      Assert.That(_editableRow.HasEditControl(0), Is.False);
      Assert.That(_editableRow.HasEditControl(1), Is.False);

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);

      Assert.That(_editableRow.HasEditControls(), Is.True);
      Assert.That(_editableRow.HasEditControl(0), Is.True);
      Assert.That(_editableRow.HasEditControl(1), Is.False);
    }

    [Test]
    public void HasEditControlWithNegativeIndex ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      Assert.That(
          () => _editableRow.HasEditControl(-1),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void HasEditControlWithIndexOutOfPositiveRange ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      Assert.That(
          () => _editableRow.HasEditControl(3),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }


    [Test]
    public void GetEditControl ()
    {
      Invoker.InitRecursive();

      Assert.That(_editableRow.HasEditControls(), Is.False);
      Assert.That(_editableRow.HasEditControl(0), Is.False);
      Assert.That(_editableRow.HasEditControl(0), Is.False);

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);

      Assert.That(_editableRow.HasEditControls(), Is.True);
      Assert.That(_editableRow.HasEditControl(0), Is.True);
      Assert.That(_editableRow.HasEditControl(1), Is.False);

      IBusinessObjectBoundEditableWebControl control = _editableRow.GetEditControl(0);
      Assert.That(control, Is.Not.Null);
      Assert.That(control is BocTextValue, Is.True);

      Assert.That(_editableRow.GetEditControl(1), Is.Null);
    }

    [Test]
    public void GetEditControlWithNegativeIndex ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      Assert.That(
          () => _editableRow.HasEditControl(-1),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void GetEditControlWithIndexOutOfPositiveRange ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      Assert.That(
          () => _editableRow.HasEditControl(3),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }


    [Test]
    public void HasValidators ()
    {
      Invoker.InitRecursive();

      Assert.That(_editableRow.HasValidators(), Is.False);
      Assert.That(_editableRow.HasValidators(0), Is.False);
      Assert.That(_editableRow.HasValidators(1), Is.False);

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);

      Assert.That(_editableRow.HasValidators(), Is.True);
      Assert.That(_editableRow.HasValidators(0), Is.True);
      Assert.That(_editableRow.HasValidators(1), Is.False);

      _editableRow.EnsureValidatorsRestored();

      Assert.That(_editableRow.HasValidators(), Is.True);
      Assert.That(_editableRow.HasValidators(0), Is.True);
      Assert.That(_editableRow.HasValidators(1), Is.False);
    }

    [Test]
    public void HasValidatorsWithoutCreateControls ()
    {
      Invoker.InitRecursive();

      Assert.That(_editableRow.HasValidators(), Is.False);
      Assert.That(_editableRow.HasValidators(0), Is.False);
      Assert.That(_editableRow.HasValidators(1), Is.False);

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      _editableRow.EnsureValidatorsRestored();

      Assert.That(_editableRow.HasValidators(), Is.False);
      Assert.That(_editableRow.HasValidators(0), Is.False);
      Assert.That(_editableRow.HasValidators(1), Is.False);

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);

      Assert.That(_editableRow.HasValidators(), Is.True);
      Assert.That(_editableRow.HasValidators(0), Is.True);
      Assert.That(_editableRow.HasValidators(1), Is.False);

      _editableRow.EnsureValidatorsRestored();

      Assert.That(_editableRow.HasValidators(), Is.True);
      Assert.That(_editableRow.HasValidators(0), Is.True);
      Assert.That(_editableRow.HasValidators(1), Is.False);
    }

    [Test]
    public void HasValidatorsWithNegativeIndex ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      _editableRow.EnsureValidatorsRestored();
      Assert.That(
          () => _editableRow.HasValidators(-1),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void HasValidatorsWithIndexOutOfPositiveRange ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      _editableRow.EnsureValidatorsRestored();
      Assert.That(
          () => _editableRow.HasValidators(3),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }


    [Test]
    public void GetValidators ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      Assert.That(_editableRow.HasValidators(), Is.True);
      Assert.That(_editableRow.HasValidators(0), Is.True);
      Assert.That(_editableRow.HasValidators(1), Is.False);

      ControlCollection validators = _editableRow.GetValidators(0);
      Assert.That(validators.Cast<BaseValidator>().Select(v => v.GetType()),
          Is.EqualTo(
              new[]
              {
                  typeof(RequiredFieldValidator),
                  typeof(NumericValidator),
                  typeof(BusinessObjectBoundEditableWebControlValidationResultDispatchingValidator)
              }));

      Assert.That(_editableRow.GetValidators(1), Is.Null);
    }

    [Test]
    public void GetValidatorsWithoutCreateControls ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      _editableRow.EnsureValidatorsRestored();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      Assert.That(_editableRow.HasValidators(), Is.True);
      Assert.That(_editableRow.HasValidators(0), Is.True);
      Assert.That(_editableRow.HasValidators(1), Is.False);

      ControlCollection validators = _editableRow.GetValidators(0);
      Assert.That(validators.Cast<BaseValidator>().Select(v => v.GetType()),
          Is.EqualTo(
              new[]
              {
                  typeof(RequiredFieldValidator),
                  typeof(NumericValidator),
                  typeof(BusinessObjectBoundEditableWebControlValidationResultDispatchingValidator)
              }));

      Assert.That(_editableRow.GetValidators(1), Is.Null);
    }

    [Test]
    public void GetValidatorsWithNegativeIndex ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      _editableRow.EnsureValidatorsRestored();
      Assert.That(
          () => _editableRow.GetValidators(-1),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void GetValidatorsWithIndexOutOfPositiveRange ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[2];
      columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[1] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      _editableRow.EnsureValidatorsRestored();
      Assert.That(
          () => _editableRow.GetValidators(3),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }


    [Test]
    public void IsRequired ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[2] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);

      Assert.That(_editableRow.IsRequired(0), Is.False);
      Assert.That(_editableRow.IsRequired(1), Is.True);
      Assert.That(_editableRow.IsRequired(2), Is.False);
    }


    [Test]
    public void IsDirty ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[2] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues(false);

      Assert.That(_editableRow.IsDirty(), Is.False);

      BocTextValue textBoxStringValue = (BocTextValue)_editableRow.GetEditControl(0);
      textBoxStringValue.Value = "a";

      Assert.That(_editableRow.IsDirty(), Is.True);
    }

    [Test]
    public void GetTrackedIDs ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[2] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues(false);

      string id = "NamingContainer_Row_{0}_Value";
      string[] trackedIDs = new string[2];
      trackedIDs[0] = string.Format(id, 0);
      trackedIDs[1] = string.Format(id, 1);

      Assert.That(_editableRow.GetTrackedClientIDs(), Is.EqualTo(trackedIDs));
    }


    [Test]
    public void ValidateWithValidValues ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[2] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues(false);

      SetValues(_editableRow, "A", "300");

      Assert.That(_editableRow.Validate(), Is.True);
    }

    [Test]
    public void ValidateWithInvalidValues ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[2] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues(false);

      SetValues(_editableRow, "A", "");

      Assert.That(_editableRow.Validate(), Is.False);
    }


    [Test]
    public void PrepareValidation ()
    {
      Invoker.InitRecursive();

      _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      _editableRow.ControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
      columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
      columns[2] = _commandColumn;

      _editableRow.CreateControls(_value01, columns);
      _editableRow.EnsureValidatorsRestored();

      IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
      dataSource.LoadValues(false);

      BocTextValue stringValueField = (BocTextValue)_editableRow.GetEditControl(0);
      BocTextValue int32ValueField = (BocTextValue)_editableRow.GetEditControl(1);

      _editableRow.PrepareValidation();

      Assert.That(stringValueField.Text, Is.EqualTo(stringValueField.Text));
      Assert.That(int32ValueField.Text, Is.EqualTo(int32ValueField.Text));
    }


    private void SetValues (EditableRow row, string stringValue, string int32Value)
    {
      BocTextValue stringValueField = (BocTextValue)row.GetEditControl(0);
      stringValueField.Text = stringValue;

      BocTextValue int32ValueField = (BocTextValue)row.GetEditControl(1);
      int32ValueField.Text = int32Value;
    }
  }
}
