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
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.EditableRowSupport
{
  [TestFixture]
  public class EditModeControllerInRowEditModeTest : EditModeControllerTestBase
  {
    [Test]
    public void GetFactoriesFromOwnerControl ()
    {
      EditModeHost.EditModeDataSourceFactory = new EditableRowDataSourceFactory();
      EditModeHost.EditModeControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(0, Columns);
      EditableRow row = (EditableRow)Controller.Controls[0];

      Assert.That(row.DataSourceFactory, Is.SameAs(EditModeHost.EditModeDataSourceFactory));
      Assert.That(row.ControlFactory, Is.SameAs(EditModeHost.EditModeControlFactory));
    }

    [Test]
    public void SwitchRowIntoEditMode ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      Assert.That(Controller.Controls.Count, Is.EqualTo(1));
      Assert.That(Controller.Controls[0] is EditableRow, Is.True);

      EditableRow row = (EditableRow)Controller.Controls[0];
      Assert.That(row.ID, Is.EqualTo("Controller_Row_2"));

      Assert.That(ActualEvents.Count, Is.EqualTo(0));
      Assert.That(EditModeHost.FocusedControl, Is.Not.Null);
      Assert.That(EditModeHost.FocusedControl.FocusID, Is.EqualTo("NamingContainer_Controller_Row_2_0_Value"));
    }

    [Test]
    public void SwitchRowIntoEditModeWithValueNull ()
    {
      Invoker.InitRecursive();
      EditModeHost.Value = null;
      Assert.That(
          () => Controller.SwitchRowIntoEditMode(0, Columns),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot initialize row edit mode: The BocList 'BocList' does not have a Value."));
    }

    [Test]
    public void SwitchRowIntoEditModeWithIndexToHigh ()
    {
      Invoker.InitRecursive();
      Assert.That(
          () => Controller.SwitchRowIntoEditMode(5, Columns),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void SwitchRowIntoEditModeWithIndexToLow ()
    {
      Invoker.InitRecursive();
      Assert.That(
          () => Controller.SwitchRowIntoEditMode(-1, Columns),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void SwitchRowIntoEditModeWhileRowEditModeIsActiveOnOtherRowWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(1, Values[1]));
      expectedEvents.Add(FormatValidateEditableRows());
      expectedEvents.Add(FormatChangesSavedEventMessage(1, Values[1]));
      expectedEvents.Add(FormatEndRowEditModeCleanUp(1));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(1, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(1));

      SetValues((EditableRow)Controller.Controls[0], "New Value B", "200");

      Controller.SwitchRowIntoEditMode(2, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      CheckValues(Values[1], "New Value B", 200);
    }

    [Test]
    public void SwitchRowIntoEditModeWhileRowEditModeIsActiveOnOtherRowWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(1, Values[1]));
      expectedEvents.Add(FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(1, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(1));

      SetValues((EditableRow)Controller.Controls[0], "New Value B", "");

      Controller.SwitchRowIntoEditMode(2, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(1));

      CheckValues(Values[1], "B", 2);
    }

    [Test]
    public void SwitchRowIntoEditModeWhileRowEditModeIsActiveOnThisRowWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatValidateEditableRows());
      expectedEvents.Add(FormatChangesSavedEventMessage(2, Values[2]));
      expectedEvents.Add(FormatEndRowEditModeCleanUp(2));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      SetValues((EditableRow)Controller.Controls[0], "New Value C", "300");

      Controller.SwitchRowIntoEditMode(2, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      CheckValues(Values[2], "New Value C", 300);
    }

    [Test]
    public void SwitchRowIntoEditModeWhileRowEditModeIsActiveOnThisRowWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      SetValues((EditableRow)Controller.Controls[0], "New Value C", "");

      Controller.SwitchRowIntoEditMode(2, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      CheckValues(Values[2], "C", 3);
    }

    [Test]
    public void SwitchRowIntoEditModeWhileListEditModeIsActiveWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(0, Values[0]));
      expectedEvents.Add(FormatChangesSavingEventMessage(1, Values[1]));
      expectedEvents.Add(FormatChangesSavingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatChangesSavingEventMessage(3, Values[3]));
      expectedEvents.Add(FormatChangesSavingEventMessage(4, Values[4]));
      expectedEvents.Add(FormatValidateEditableRows());
      expectedEvents.Add(FormatChangesSavedEventMessage(0, Values[0]));
      expectedEvents.Add(FormatChangesSavedEventMessage(1, Values[1]));
      expectedEvents.Add(FormatChangesSavedEventMessage(2, Values[2]));
      expectedEvents.Add(FormatChangesSavedEventMessage(3, Values[3]));
      expectedEvents.Add(FormatChangesSavedEventMessage(4, Values[4]));
      expectedEvents.Add(FormatEndListEditModeCleanUp());

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      SetValues((EditableRow)Controller.Controls[0], "New Value A", "100");
      SetValues((EditableRow)Controller.Controls[1], "New Value B", "200");
      SetValues((EditableRow)Controller.Controls[2], "New Value C", "300");
      SetValues((EditableRow)Controller.Controls[3], "New Value D", "400");
      SetValues((EditableRow)Controller.Controls[4], "New Value E", "500");

      Controller.SwitchRowIntoEditMode(2, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      CheckValues(Values[0], "New Value A", 100);
      CheckValues(Values[1], "New Value B", 200);
      CheckValues(Values[2], "New Value C", 300);
      CheckValues(Values[3], "New Value D", 400);
      CheckValues(Values[4], "New Value E", 500);
    }

    [Test]
    public void SwitchRowIntoEditModeWhileListEditModeIsActiveWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(0, Values[0]));
      expectedEvents.Add(FormatChangesSavingEventMessage(1, Values[1]));
      expectedEvents.Add(FormatChangesSavingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatChangesSavingEventMessage(3, Values[3]));
      expectedEvents.Add(FormatChangesSavingEventMessage(4, Values[4]));
      expectedEvents.Add(FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      SetValues((EditableRow)Controller.Controls[0], "New Value A", "");
      SetValues((EditableRow)Controller.Controls[1], "New Value B", "");
      SetValues((EditableRow)Controller.Controls[2], "New Value C", "");
      SetValues((EditableRow)Controller.Controls[3], "New Value D", "");
      SetValues((EditableRow)Controller.Controls[4], "New Value E", "");

      Controller.SwitchRowIntoEditMode(2, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));

      CheckValues(Values[0], "A", 1);
      CheckValues(Values[1], "B", 2);
      CheckValues(Values[2], "C", 3);
      CheckValues(Values[3], "D", 4);
      CheckValues(Values[4], "E", 5);
    }

    [Test]
    public void SwitchRowIntoEditModeWithDisabledAutoFocus ()
    {
      Invoker.InitRecursive();
      EditModeHost.IsAutoFocusOnSwitchToEditModeEnabled = false;
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);

      Assert.That(EditModeHost.FocusedControl, Is.Null);
    }

    [Test]
    public void AddAndEditRow ()
    {
      Invoker.InitRecursive();

      Assert.That(Controller.AddAndEditRow(NewValues[0], Columns), Is.True);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(5));
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(6));
      Assert.That(EditModeHost.Value[5], Is.SameAs(NewValues[0]));

      Assert.That(Controller.Controls.Count, Is.EqualTo(1));
      Assert.That(Controller.Controls[0] is EditableRow, Is.True);

      EditableRow row = (EditableRow)Controller.Controls[0];
      Assert.That(row.ID, Is.EqualTo("Controller_Row_5"));

      Assert.That(ActualEvents.Count, Is.EqualTo(0));
      Assert.That(EditModeHost.FocusedControl, Is.Not.Null);
      Assert.That(EditModeHost.FocusedControl.FocusID, Is.EqualTo("NamingContainer_Controller_Row_5_0_Value"));
    }

    [Test]
    public void AddAndEditRowWhileRowEditModeIsActiveWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatValidateEditableRows());
      expectedEvents.Add(FormatChangesSavedEventMessage(2, Values[2]));
      expectedEvents.Add(FormatEndRowEditModeCleanUp(2));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      SetValues((EditableRow)Controller.Controls[0], "New Value C", "300");

      Assert.That(Controller.AddAndEditRow(NewValues[0], Columns), Is.True);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(5));
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(6));
      Assert.That(EditModeHost.Value[5], Is.SameAs(NewValues[0]));

      CheckValues(Values[2], "New Value C", 300);
    }

    [Test]
    public void AddAndEditRowWhileRowEditModeIsActiveWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      SetValues((EditableRow)Controller.Controls[0], "New Value C", "");

      Assert.That(Controller.AddAndEditRow(NewValues[0], Columns), Is.False);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));

      CheckValues(Values[2], "C", 3);
    }

    [Test]
    public void AddAndEditRowWhileListEditModeIsActiveWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(0, Values[0]));
      expectedEvents.Add(FormatChangesSavingEventMessage(1, Values[1]));
      expectedEvents.Add(FormatChangesSavingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatChangesSavingEventMessage(3, Values[3]));
      expectedEvents.Add(FormatChangesSavingEventMessage(4, Values[4]));
      expectedEvents.Add(FormatValidateEditableRows());
      expectedEvents.Add(FormatChangesSavedEventMessage(0, Values[0]));
      expectedEvents.Add(FormatChangesSavedEventMessage(1, Values[1]));
      expectedEvents.Add(FormatChangesSavedEventMessage(2, Values[2]));
      expectedEvents.Add(FormatChangesSavedEventMessage(3, Values[3]));
      expectedEvents.Add(FormatChangesSavedEventMessage(4, Values[4]));
      expectedEvents.Add(FormatEndListEditModeCleanUp());

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      SetValues((EditableRow)Controller.Controls[0], "New Value A", "100");
      SetValues((EditableRow)Controller.Controls[1], "New Value B", "200");
      SetValues((EditableRow)Controller.Controls[2], "New Value C", "300");
      SetValues((EditableRow)Controller.Controls[3], "New Value D", "400");
      SetValues((EditableRow)Controller.Controls[4], "New Value E", "500");

      Assert.That(Controller.AddAndEditRow(NewValues[0], Columns), Is.True);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(5));
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(6));
      Assert.That(EditModeHost.Value[5], Is.SameAs(NewValues[0]));

      CheckValues(Values[0], "New Value A", 100);
      CheckValues(Values[1], "New Value B", 200);
      CheckValues(Values[2], "New Value C", 300);
      CheckValues(Values[3], "New Value D", 400);
      CheckValues(Values[4], "New Value E", 500);
    }

    [Test]
    public void AddAndEditRowWhileListEditModeIsActiveWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(0, Values[0]));
      expectedEvents.Add(FormatChangesSavingEventMessage(1, Values[1]));
      expectedEvents.Add(FormatChangesSavingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatChangesSavingEventMessage(3, Values[3]));
      expectedEvents.Add(FormatChangesSavingEventMessage(4, Values[4]));
      expectedEvents.Add(FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      SetValues((EditableRow)Controller.Controls[0], "New Value A", "");
      SetValues((EditableRow)Controller.Controls[1], "New Value B", "");
      SetValues((EditableRow)Controller.Controls[2], "New Value C", "");
      SetValues((EditableRow)Controller.Controls[3], "New Value D", "");
      SetValues((EditableRow)Controller.Controls[4], "New Value E", "");

      Assert.That(Controller.AddAndEditRow(NewValues[0], Columns), Is.False);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));

      CheckValues(Values[0], "A", 1);
      CheckValues(Values[1], "B", 2);
      CheckValues(Values[2], "C", 3);
      CheckValues(Values[3], "D", 4);
      CheckValues(Values[4], "E", 5);
    }

    [Test]
    public void AddAndEditRowIgnoresAutoFocusFlag ()
    {
      Invoker.InitRecursive();
      EditModeHost.IsAutoFocusOnSwitchToEditModeEnabled = false;

      Assert.That(Controller.AddAndEditRow(NewValues[0], Columns), Is.True);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(5));
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(6));
      Assert.That(EditModeHost.Value[5], Is.SameAs(NewValues[0]));

      Assert.That(Controller.Controls.Count, Is.EqualTo(1));
      Assert.That(Controller.Controls[0] is EditableRow, Is.True);

      EditableRow row = (EditableRow)Controller.Controls[0];
      Assert.That(row.ID, Is.EqualTo("Controller_Row_5"));

      Assert.That(ActualEvents.Count, Is.EqualTo(0));
      Assert.That(EditModeHost.FocusedControl, Is.Not.Null);
      Assert.That(EditModeHost.FocusedControl.FocusID, Is.EqualTo("NamingContainer_Controller_Row_5_0_Value"));
    }

    [Test]
    public void EndRowEditModeAndSaveChangesWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatValidateEditableRows());
      expectedEvents.Add(FormatChangesSavedEventMessage(2, Values[2]));
      expectedEvents.Add(FormatEndRowEditModeCleanUp(2));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      SetValues((EditableRow)Controller.Controls[0], "New Value C", "300");
      Controller.EndRowEditMode(true, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.False);

      CheckValues(Values[2], "New Value C", 300);
    }

    [Test]
    public void EndRowEditModeAndDiscardChangesWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesCancelingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(2, Values[2]));
      expectedEvents.Add(FormatEndRowEditModeCleanUp(2));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      SetValues((EditableRow)Controller.Controls[0], "New Value C", "300");
      Controller.EndRowEditMode(false, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.False);

      CheckValues(Values[2], "C", 3);
    }

    [Test]
    public void EndRowEditModeWithNewRowAndSaveChangesWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(5, NewValues[0]));
      expectedEvents.Add(FormatValidateEditableRows());
      expectedEvents.Add(FormatChangesSavedEventMessage(5, NewValues[0]));
      expectedEvents.Add(FormatEndRowEditModeCleanUp(5));

      Invoker.InitRecursive();
      Controller.AddAndEditRow(NewValues[0], Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(5));

      SetValues((EditableRow)Controller.Controls[0], "New Value F", "600");
      Controller.EndRowEditMode(true, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.False);

      Assert.That(EditModeHost.Value.Count, Is.EqualTo(6));
      CheckValues(NewValues[0], "New Value F", 600);
    }

    [Test]
    public void EndRowEditModeWithNewRowAndDiscardChangesWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesCancelingEventMessage(5, NewValues[0]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(-1, NewValues[0]));
      expectedEvents.Add(FormatEndRowEditModeCleanUp(5));

      Invoker.InitRecursive();
      Controller.AddAndEditRow(NewValues[0], Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(5));

      SetValues((EditableRow)Controller.Controls[0], "New Value F", "600");
      Controller.EndRowEditMode(false, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.False);

      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));
      CheckValues(NewValues[0], "F", 6);
    }

    [Test]
    public void EndRowEditModeAndSaveChangesWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      SetValues((EditableRow)Controller.Controls[0], "New Value C", "");
      Controller.EndRowEditMode(true, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      CheckValues(Values[2], "C", 3);
    }

    [Test]
    public void EndRowEditModeAndDiscardChangesWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesCancelingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(2, Values[2]));
      expectedEvents.Add(FormatEndRowEditModeCleanUp(2));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      SetValues((EditableRow)Controller.Controls[0], "New Value C", "");
      Controller.EndRowEditMode(false, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.False);

      CheckValues(Values[2], "C", 3);
    }

    [Test]
    public void EndRowEditModeWithoutBeingActive ()
    {
      Invoker.InitRecursive();

      Assert.That(Controller.IsRowEditModeActive, Is.False);

      Controller.EndRowEditMode(true, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.False);
      Assert.That(ActualEvents.Count, Is.EqualTo(0));
    }

    [Test]
    public void EndRowEditModeWithoutBeingActiveAndValueNull ()
    {
      Invoker.InitRecursive();
      EditModeHost.Value = null;

      Assert.That(Controller.IsRowEditModeActive, Is.False);

      Controller.EndRowEditMode(true, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.False);
      Assert.That(ActualEvents.Count, Is.EqualTo(0));
    }


    [Test]
    public void EnsureEditModeRestored ()
    {
      Assert.That(Controller.IsRowEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.RowEditMode, new List<string> { "2" }, false));
      Assert.That(Controller.IsRowEditModeActive, Is.True);

      Controller.EnsureEditModeRestored(Columns);
      Assert.That(Controller.IsRowEditModeActive, Is.True);
    }

    [Test]
    public void EnsureEditModeRestoredWithMissingRow_ThrowsInvalidOperationException ()
    {
      Assert.That(Controller.IsRowEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.RowEditMode, new List<string> { "6" }, false));
      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(
          () => Controller.EnsureEditModeRestored(Columns),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot create edit mode controls for the row with ID '6'. The BocList 'BocList' does not contain the row in its Value collection."));
    }

    [Test]
    public void EnsureEditModeRestoredWithValueNull_ThrowsInvalidOperationException ()
    {
      Assert.That(Controller.IsRowEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.RowEditMode, new List<string> { "6" }, false));
      Assert.That(Controller.IsRowEditModeActive, Is.True);
      EditModeHost.Value = null;
      Assert.That(
          () => Controller.EnsureEditModeRestored(Columns),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot restore edit mode: The BocList 'BocList' does not have a Value."));
    }

    [Test]
    public void EnsureEditModeRestored_CallsLoadValueWithInterimTrue ()
    {
      var editedObject = (IBusinessObject)EditModeHost.Value[2];
      var dataSourceStub = new Mock<IBusinessObjectReferenceDataSource>();
      dataSourceStub.SetupProperty(_ => _.BusinessObject);
      dataSourceStub.Object.BusinessObject = editedObject;
      EditModeHost.EditModeDataSourceFactory = new Mock<EditableRowDataSourceFactory>().Object;
      Mock.Get(EditModeHost.EditModeDataSourceFactory).Setup(_ => _.Create(editedObject)).Returns(dataSourceStub.Object);

      Assert.That(Controller.IsRowEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.RowEditMode, new List<string> { "2" }, false));
      Assert.That(Controller.IsRowEditModeActive, Is.True);

      Controller.EnsureEditModeRestored(Columns);
      Assert.That(Controller.IsRowEditModeActive, Is.True);

      dataSourceStub.Verify(_ => _.LoadValues(true), Times.AtLeastOnce());
    }

    [Test]
    public void AddRows ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));

      Controller.AddRows(NewValues, Columns);

      Assert.That(EditModeHost.Value.Count, Is.EqualTo(7));
      Assert.That(EditModeHost.Value[5], Is.SameAs(NewValues[0]));
      Assert.That(EditModeHost.Value[6], Is.SameAs(NewValues[1]));
      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      Assert.That(ActualEvents.Count, Is.EqualTo(0));
    }

    [Test]
    public void AddRow ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));

      Assert.That(Controller.AddRow(NewValues[0], Columns), Is.EqualTo(5));

      Assert.That(EditModeHost.Value.Count, Is.EqualTo(6));
      Assert.That(EditModeHost.Value[5], Is.SameAs(NewValues[0]));
      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      Assert.That(ActualEvents.Count, Is.EqualTo(0));
    }


    [Test]
    public void RemoveRows ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));
      Assert.That(
          () => Controller.RemoveRows(new IBusinessObject[] {Values[2]}),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot remove rows while the BocList 'BocList' is in row edit mode. Call EndEditMode() before removing the rows."));
    }

    [Test]
    public void RemoveRow ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));
      Assert.That(
          () => Controller.RemoveRow(Values[2]),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot remove rows while the BocList 'BocList' is in row edit mode. Call EndEditMode() before removing the rows."));
    }

    [Test]
    public void ValidateWithValidValues ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      SetValues((EditableRow)Controller.Controls[0], "New Value C", "300");

      Assert.That(Controller.Validate(), Is.True);
    }

    [Test]
    public void ValidateWithInvalidValues ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      SetValues((EditableRow)Controller.Controls[0], "New Value C", "");

      Assert.That(Controller.Validate(), Is.False);
    }


    [Test]
    public void PrepareValidation ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      EditableRow editableRow = (EditableRow)Controller.Controls[0];

      BocTextValue stringValueField = (BocTextValue)editableRow.GetEditControl(0);
      BocTextValue int32ValueField = (BocTextValue)editableRow.GetEditControl(1);

      Controller.PrepareValidation();

      Assert.That(stringValueField.Text, Is.EqualTo(stringValueField.Text));
      Assert.That(int32ValueField.Text, Is.EqualTo(int32ValueField.Text));
    }


    [Test]
    public void IsRequired ()
    {
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);

      Assert.That(Controller.IsRequired(0), Is.False);
      Assert.That(Controller.IsRequired(1), Is.True);
    }

    [Test]
    public void IsDirty ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      EditableRow row = (EditableRow)Controller.Controls[0];
      Remotion.ObjectBinding.Web.UI.Controls.BocTextValue stringValueField =
          (Remotion.ObjectBinding.Web.UI.Controls.BocTextValue)row.GetEditControl(0);
      stringValueField.Value = "New Value";

      Assert.That(Controller.IsDirty(), Is.True);
    }

    [Test]
    public void GetTrackedIDs ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      string id = "NamingContainer_Controller_Row_{0}_{1}_Value";
      string[] trackedIDs = new string[2];
      trackedIDs[0] = string.Format(id, 2, 0);
      trackedIDs[1] = string.Format(id, 2, 1);

      Assert.That(Controller.GetTrackedClientIDs(), Is.EqualTo(trackedIDs));
    }


    [Test]
    public void SaveAndLoadControlState ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);
      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));

      object viewState = ControllerInvoker.SaveControlState();
      Assert.That(viewState, Is.Not.Null);

      Controller.EndRowEditMode(false, Columns);
      Assert.That(Controller.IsRowEditModeActive, Is.False);

      ControllerInvoker.LoadControlState(viewState);
      Assert.That(Controller.IsRowEditModeActive, Is.True);
      Assert.That(Controller.GetEditedRow().Index, Is.EqualTo(2));
    }
  }
}
