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
using System.Collections.Specialized;
using System.Linq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.EditableRowSupport
{
  public class EditModeControllerTestBase : BocTest
  {
    protected enum EditMode
    {
      None,
      RowEditMode,
      ListEditMode,
    }

    private StringCollection _actualEvents;

    private FakeEditModeHost _editModeHost;
    private EditModeController _controller;
    private ControlInvoker _controllerInvoker;

    private IBusinessObject[] _values;
    private IBusinessObject[] _newValues;

    private BindableObjectClass _class;

    private IBusinessObjectPropertyPath _stringValuePath;
    private IBusinessObjectPropertyPath _int32ValuePath;

    private BocColumnDefinition[] _columns;

    private BocSimpleColumnDefinition _stringValueSimpleColumn;
    private BocSimpleColumnDefinition _int32ValueSimpleColumn;

    public override void SetUp ()
    {
      base.SetUp();

      _actualEvents = new StringCollection();

      _values = new IBusinessObject[5];
      _values[0] = (IBusinessObject)TypeWithAllDataTypes.Create("A", 1);
      _values[1] = (IBusinessObject)TypeWithAllDataTypes.Create("B", 2);
      _values[2] = (IBusinessObject)TypeWithAllDataTypes.Create("C", 3);
      _values[3] = (IBusinessObject)TypeWithAllDataTypes.Create("D", 4);
      _values[4] = (IBusinessObject)TypeWithAllDataTypes.Create("E", 5);

      _newValues = new IBusinessObject[2];
      _newValues[0] = (IBusinessObject)TypeWithAllDataTypes.Create("F", 6);
      _newValues[1] = (IBusinessObject)TypeWithAllDataTypes.Create("G", 7);

      _class = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(TypeWithAllDataTypes));

      _stringValuePath = BusinessObjectPropertyPath.CreateStatic(_class, "String");
      _int32ValuePath = BusinessObjectPropertyPath.CreateStatic(_class, "Int32");

      _stringValueSimpleColumn = new BocSimpleColumnDefinition();
      _stringValueSimpleColumn.SetPropertyPath(_stringValuePath);

      _int32ValueSimpleColumn = new BocSimpleColumnDefinition();
      _int32ValueSimpleColumn.SetPropertyPath(_int32ValuePath);

      _columns = new BocColumnDefinition[2];
      _columns[0] = _stringValueSimpleColumn;
      _columns[1] = _int32ValueSimpleColumn;

      _editModeHost = new FakeEditModeHost();
      _editModeHost.ID = "BocList";

      _controller = new EditModeController(_editModeHost);
      _controller.ID = "Controller";
      NamingContainer.Controls.Add(_controller);

      _controllerInvoker = new ControlInvoker(_controller);

      _editModeHost.NotifyOnEditableRowChangesCanceled = (i, o) => _actualEvents.Add(FormatChangesCanceledEventMessage(i, o));
      _editModeHost.NotifyOnEditableRowChangesCanceling = (i, o) => _actualEvents.Add(FormatChangesCancelingEventMessage(i, o));
      _editModeHost.NotifyOnEditableRowChangesSaved = (i, o) => _actualEvents.Add(FormatChangesSavedEventMessage(i, o));
      _editModeHost.NotifyOnEditableRowChangesSaving = (i, o) => _actualEvents.Add(FormatChangesSavingEventMessage(i, o));
      _editModeHost.NotifyAddRows =
          objects =>
          {
            var oldLength = _editModeHost.Value.Count;
            _editModeHost.Value = ((IBusinessObject[])_editModeHost.Value).Concat(objects).ToArray();
            return ((IBusinessObject[])_editModeHost.Value).Select((o, i) => new BocListRow(i, o)).Skip(oldLength).ToArray();
          };
      _editModeHost.NotifyRemoveRows =
          objects =>
          {
            var removedRows = ((IBusinessObject[])_editModeHost.Value)
                .Select((o, i) => new BocListRow(i, o))
                .Where(r => objects.Contains(r.BusinessObject))
                .ToArray();
            _editModeHost.Value = ((IBusinessObject[])_editModeHost.Value).Except(objects).ToArray();
            return removedRows;
          };
      _editModeHost.NotifyEndRowEditModeCleanUp = i => _actualEvents.Add(FormatEndRowEditModeCleanUp(i));
      _editModeHost.NotifyEndListEditModeCleanUp = () => _actualEvents.Add(FormatEndListEditModeCleanUp());
      _editModeHost.NotifyValidateEditableRows = () => _actualEvents.Add(FormatValidateEditableRows());
      _editModeHost.Value =_values;
      _editModeHost.RowIDProvider = new FakeRowIDProvider();
      _editModeHost.EditModeControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();
      _editModeHost.EditModeDataSourceFactory = new EditableRowDataSourceFactory();
      _editModeHost.EnableEditModeValidator = true;
      _editModeHost.IsAutoFocusOnSwitchToEditModeEnabled = true;
      _editModeHost.AreCustomCellsValid = true;
    }

    protected StringCollection ActualEvents
    {
      get { return _actualEvents; }
    }

    protected FakeEditModeHost EditModeHost
    {
      get { return _editModeHost; }
    }

    protected EditModeController Controller
    {
      get { return _controller; }
    }

    public ControlInvoker ControllerInvoker
    {
      get { return _controllerInvoker; }
    }

    protected IBusinessObject[] Values
    {
      get { return _values; }
    }

    protected IBusinessObject[] NewValues
    {
      get { return _newValues; }
    }

    protected BocColumnDefinition[] Columns
    {
      get { return _columns; }
    }

    protected void SetValues (EditableRow row, string stringValue, string int32Value)
    {
      ArgumentUtility.CheckNotNull("row", row);

      BocTextValue stringValueField = (BocTextValue)row.GetEditControl(0);
      stringValueField.Text = stringValue;

      BocTextValue int32ValueField = (BocTextValue)row.GetEditControl(1);
      int32ValueField.Text = int32Value;
    }

    protected void CheckValues (IBusinessObject value, string stringValue, int int32Value)
    {
      TypeWithAllDataTypes typeWithAllDataTypes = ArgumentUtility.CheckNotNullAndType<TypeWithAllDataTypes>("value", value);

      Assert.That(typeWithAllDataTypes.String, Is.EqualTo(stringValue));
      Assert.That(typeWithAllDataTypes.Int32, Is.EqualTo(int32Value));
    }

    protected void CheckEvents (StringCollection expected, StringCollection actual)
    {
      Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    protected string FormatChangesCanceledEventMessage (int index, IBusinessObject businessObject)
    {
      return FormatEventMessage("ChangesCanceled", index, businessObject);
    }

    protected string FormatChangesCancelingEventMessage (int index, IBusinessObject businessObject)
    {
      return FormatEventMessage("ChangesCanceling", index, businessObject);
    }

    protected string FormatChangesSavedEventMessage (int index, IBusinessObject businessObject)
    {
      return FormatEventMessage("ChangesSaved", index, businessObject);
    }

    protected string FormatChangesSavingEventMessage (int index, IBusinessObject businessObject)
    {
      return FormatEventMessage("ChangesSaving", index, businessObject);
    }

    private string FormatEventMessage (string eventName, int index, IBusinessObject businessObject)
    {
      return string.Format("{0}: {1}, {2}", eventName, index, businessObject.ToString());
    }

    protected string FormatEndRowEditModeCleanUp (int index)
    {
      return string.Format("EndRowEditModeCleanUp ({0})", index);
    }

    protected string FormatEndListEditModeCleanUp ()
    {
      return "EndListEditModeCleanUp()";
    }

    protected string FormatValidateEditableRows ()
    {
      return "ValidateEditableRows ()";
    }

    protected object CreateControlState (object baseControlState, EditMode editMode, List<string> editedRowIDs, bool isEditNewRow)
    {
      object[] values = new object[4];

      values[0] = baseControlState;
      values[1] = editMode;
      values[2] = editedRowIDs;
      values[3] = isEditNewRow;

      return values;
    }
  }
}
