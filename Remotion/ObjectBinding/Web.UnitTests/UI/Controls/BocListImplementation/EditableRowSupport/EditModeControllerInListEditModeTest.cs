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
using Moq;
using NUnit.Framework;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UnitTests.Domain;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.EditableRowSupport
{
  [TestFixture]
  public class EditModeControllerInListEditModeTest : EditModeControllerTestBase
  {
    [Test]
    public void SwitchListIntoEditMode ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      Assert.That(Controller.Controls.Count, Is.EqualTo(5));
      string idFormat = "Controller_Row_{0}";
      Assert.That(Controller.Controls[0].ID, Is.EqualTo(string.Format(idFormat, 0)));
      Assert.That(Controller.Controls[1].ID, Is.EqualTo(string.Format(idFormat, 1)));
      Assert.That(Controller.Controls[2].ID, Is.EqualTo(string.Format(idFormat, 2)));
      Assert.That(Controller.Controls[3].ID, Is.EqualTo(string.Format(idFormat, 3)));
      Assert.That(Controller.Controls[4].ID, Is.EqualTo(string.Format(idFormat, 4)));

      Assert.That(ActualEvents.Count, Is.EqualTo(0));
      Assert.That(EditModeHost.FocusedControl, Is.Not.Null);
      Assert.That(EditModeHost.FocusedControl.FocusID, Is.EqualTo("NamingContainer_Controller_Row_0_0_Value"));
    }

    [Test]
    public void SwitchListIntoEditModeWithValueEmpty ()
    {
      Invoker.InitRecursive();
      EditModeHost.Value = new IBusinessObject[0];
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(Controller.Controls.Count, Is.EqualTo(0));

      Assert.That(ActualEvents.Count, Is.EqualTo(0));
      Assert.That(EditModeHost.FocusedControl, Is.Null);
    }

    [Test]
    public void SwitchListIntoEditModeWithValueNull ()
    {
      Invoker.InitRecursive();
      EditModeHost.Value = null;
      Assert.That(
          () => Controller.SwitchListIntoEditMode(Columns),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot initialize list edit mode: The BocList 'BocList' does not have a Value."));
    }

    [Test]
    public void SwitchListIntoEditModeWhileListEditModeIsActiveWithValidValues ()
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

      Controller.SwitchListIntoEditMode(Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      CheckValues(Values[0], "New Value A", 100);
      CheckValues(Values[1], "New Value B", 200);
      CheckValues(Values[2], "New Value C", 300);
      CheckValues(Values[3], "New Value D", 400);
      CheckValues(Values[4], "New Value E", 500);
    }

    [Test]
    public void SwitchListIntoEditModeWhileListEditModeIsActiveWithInvalidValues ()
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

      Controller.SwitchListIntoEditMode(Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      CheckValues(Values[0], "A", 1);
      CheckValues(Values[1], "B", 2);
      CheckValues(Values[2], "C", 3);
      CheckValues(Values[3], "D", 4);
      CheckValues(Values[4], "E", 5);
    }

    [Test]
    public void SwitchListIntoEditModeWhileRowEditModeIsActiveWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatValidateEditableRows());
      expectedEvents.Add(FormatChangesSavedEventMessage(2, Values[2]));
      expectedEvents.Add(FormatEndRowEditModeCleanUp(2));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      var editedRow = Controller.GetEditedRow();
      Assert.That(editedRow.Index, Is.EqualTo(2));
      Assert.That(editedRow.BusinessObject, Is.EqualTo(Values[2]));

      SetValues((EditableRow)Controller.Controls[0], "New Value C", "300");

      Controller.SwitchListIntoEditMode(Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      CheckValues(Values[2], "New Value C", 300);
    }

    [Test]
    public void SwitchListIntoEditModeWhileRowEditModeIsActiveWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesSavingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      var editedRow1 = Controller.GetEditedRow();
      Assert.That(editedRow1.Index, Is.EqualTo(2));
      Assert.That(editedRow1.BusinessObject, Is.EqualTo(Values[2]));

      SetValues((EditableRow)Controller.Controls[0], "New Value C", "");

      Controller.SwitchListIntoEditMode(Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsRowEditModeActive, Is.True);
      var editedRow2 = Controller.GetEditedRow();
      Assert.That(editedRow2.Index, Is.EqualTo(2));
      Assert.That(editedRow2.BusinessObject, Is.EqualTo(Values[2]));

      CheckValues(Values[2], "C", 3);
    }

    [Test]
    public void SwitchListIntoEditModeWithDisabledAutoFocus ()
    {
      Invoker.InitRecursive();
      EditModeHost.IsAutoFocusOnSwitchToEditModeEnabled = false;
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      Assert.That(EditModeHost.FocusedControl, Is.Null);
    }

    [Test]
    public void EndListEditModeAndSaveChangesWithValidValues ()
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
      Controller.EndListEditMode(true, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsListEditModeActive, Is.False);

      CheckValues(Values[0], "New Value A", 100);
      CheckValues(Values[1], "New Value B", 200);
      CheckValues(Values[2], "New Value C", 300);
      CheckValues(Values[3], "New Value D", 400);
      CheckValues(Values[4], "New Value E", 500);
    }

    [Test]
    public void EndListEditModeAndDiscardChangesWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesCancelingEventMessage(0, Values[0]));
      expectedEvents.Add(FormatChangesCancelingEventMessage(1, Values[1]));
      expectedEvents.Add(FormatChangesCancelingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatChangesCancelingEventMessage(3, Values[3]));
      expectedEvents.Add(FormatChangesCancelingEventMessage(4, Values[4]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(0, Values[0]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(1, Values[1]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(2, Values[2]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(3, Values[3]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(4, Values[4]));
      expectedEvents.Add(FormatEndListEditModeCleanUp());

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      SetValues((EditableRow)Controller.Controls[0], "New Value A", "100");
      SetValues((EditableRow)Controller.Controls[1], "New Value B", "200");
      SetValues((EditableRow)Controller.Controls[2], "New Value C", "300");
      SetValues((EditableRow)Controller.Controls[3], "New Value D", "400");
      SetValues((EditableRow)Controller.Controls[4], "New Value E", "500");
      Controller.EndListEditMode(false, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsListEditModeActive, Is.False);

      CheckValues(Values[0], "A", 1);
      CheckValues(Values[1], "B", 2);
      CheckValues(Values[2], "C", 3);
      CheckValues(Values[3], "D", 4);
      CheckValues(Values[4], "E", 5);
    }

    [Test]
    public void EndListEditModeAndSaveChangesWithInvalidValues ()
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
      Controller.EndListEditMode(true, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      CheckValues(Values[0], "A", 1);
      CheckValues(Values[1], "B", 2);
      CheckValues(Values[2], "C", 3);
      CheckValues(Values[3], "D", 4);
      CheckValues(Values[4], "E", 5);
    }

    [Test]
    public void EndListEditModeAndDiscardChangesWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatChangesCancelingEventMessage(0, Values[0]));
      expectedEvents.Add(FormatChangesCancelingEventMessage(1, Values[1]));
      expectedEvents.Add(FormatChangesCancelingEventMessage(2, Values[2]));
      expectedEvents.Add(FormatChangesCancelingEventMessage(3, Values[3]));
      expectedEvents.Add(FormatChangesCancelingEventMessage(4, Values[4]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(0, Values[0]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(1, Values[1]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(2, Values[2]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(3, Values[3]));
      expectedEvents.Add(FormatChangesCanceledEventMessage(4, Values[4]));
      expectedEvents.Add(FormatEndListEditModeCleanUp());

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      SetValues((EditableRow)Controller.Controls[0], "New Value A", "");
      SetValues((EditableRow)Controller.Controls[1], "New Value B", "");
      SetValues((EditableRow)Controller.Controls[2], "New Value C", "");
      SetValues((EditableRow)Controller.Controls[3], "New Value D", "");
      SetValues((EditableRow)Controller.Controls[4], "New Value E", "");
      Controller.EndListEditMode(false, Columns);

      CheckEvents(expectedEvents, ActualEvents);

      Assert.That(Controller.IsListEditModeActive, Is.False);

      CheckValues(Values[0], "A", 1);
      CheckValues(Values[1], "B", 2);
      CheckValues(Values[2], "C", 3);
      CheckValues(Values[3], "D", 4);
      CheckValues(Values[4], "E", 5);
    }

    [Test]
    public void EndListEditModeWithoutBeingActive ()
    {
      Invoker.InitRecursive();

      Assert.That(Controller.IsListEditModeActive, Is.False);

      Controller.EndListEditMode(true, Columns);

      Assert.That(Controller.IsListEditModeActive, Is.False);
      Assert.That(ActualEvents.Count, Is.EqualTo(0));
    }

    [Test]
    public void EndListEditModeWithoutBeingActiveAndValueNull ()
    {
      Invoker.InitRecursive();
      EditModeHost.Value = null;

      Assert.That(Controller.IsListEditModeActive, Is.False);

      Controller.EndListEditMode(true, Columns);

      Assert.That(Controller.IsListEditModeActive, Is.False);
      Assert.That(ActualEvents.Count, Is.EqualTo(0));
    }


    [Test]
    public void EnsureEditModeRestored ()
    {
      string idFormat = "Controller_Row_{0}";

      Assert.That(Controller.IsListEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.ListEditMode, new List<string> { "0", "1", "2", "3", "4" }, false));
      Assert.That(Controller.IsListEditModeActive, Is.True);

      Controller.EnsureEditModeRestored(Columns);
      Assert.That(Controller.IsListEditModeActive, Is.True);

      Assert.That(Controller.Controls[0].ID, Is.EqualTo(string.Format(idFormat, 0)));
      Assert.That(Controller.Controls[1].ID, Is.EqualTo(string.Format(idFormat, 1)));
      Assert.That(Controller.Controls[2].ID, Is.EqualTo(string.Format(idFormat, 2)));
      Assert.That(Controller.Controls[3].ID, Is.EqualTo(string.Format(idFormat, 3)));
      Assert.That(Controller.Controls[4].ID, Is.EqualTo(string.Format(idFormat, 4)));
    }

    [Test]
    public void EnsureEditModeRestoredWithMissingRow ()
    {
      string idFormat = "Controller_Row_{0}";

      Assert.That(Controller.IsListEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.ListEditMode, new List<string> { "0", "1", "2", "999", "3", "4" }, false));
      Assert.That(Controller.IsListEditModeActive, Is.True);

      Controller.EnsureEditModeRestored(Columns);
      Assert.That(Controller.IsListEditModeActive, Is.True);

      Assert.That(Controller.Controls[0].ID, Is.EqualTo(string.Format(idFormat, 0)));
      Assert.That(Controller.Controls[1].ID, Is.EqualTo(string.Format(idFormat, 1)));
      Assert.That(Controller.Controls[2].ID, Is.EqualTo(string.Format(idFormat, 2)));
      Assert.That(Controller.Controls[3].ID, Is.EqualTo(string.Format(idFormat, 3)));
      Assert.That(Controller.Controls[4].ID, Is.EqualTo(string.Format(idFormat, 4)));
    }

    [Test]
    public void EnsureEditModeRestoredWithAddedRow ()
    {
      string idFormat = "Controller_Row_{0}";

      Assert.That(Controller.IsListEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.ListEditMode, new List<string> { "0", "1", "3", "4" }, false));
      Assert.That(Controller.IsListEditModeActive, Is.True);

      Controller.EnsureEditModeRestored(Columns);
      Assert.That(Controller.IsListEditModeActive, Is.True);

      Assert.That(Controller.Controls[0].ID, Is.EqualTo(string.Format(idFormat, 0)));
      Assert.That(Controller.Controls[1].ID, Is.EqualTo(string.Format(idFormat, 1)));
      Assert.That(Controller.Controls[2].ID, Is.EqualTo(string.Format(idFormat, 3)));
      Assert.That(Controller.Controls[3].ID, Is.EqualTo(string.Format(idFormat, 4)));
      Assert.That(Controller.Controls[4].ID, Is.EqualTo(string.Format(idFormat, 2)));
    }

    [Test]
    public void EnsureEditModeRestored_CallsLoadValueWithInterimTrue ()
    {
      var dataSourceStub = new Mock<IBusinessObjectReferenceDataSource>();
      dataSourceStub.SetupProperty(_ => _.BusinessObject);
      dataSourceStub.Object.BusinessObject = new Mock<IBusinessObject>().Object;
      EditModeHost.EditModeDataSourceFactory = new Mock<EditableRowDataSourceFactory>().Object;
      Mock.Get(EditModeHost.EditModeDataSourceFactory)
                  .Setup(_ => _.Create(It.IsAny<IBusinessObject>()))
                  .Returns(dataSourceStub.Object);

      Assert.That(Controller.IsListEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.ListEditMode, new List<string> { "0", "1", "2", "3", "4" }, false));
      Assert.That(Controller.IsListEditModeActive, Is.True);

      Controller.EnsureEditModeRestored(Columns);
      Assert.That(Controller.IsListEditModeActive, Is.True);

      dataSourceStub.Verify(_ => _.LoadValues(true), Times.Exactly(EditModeHost.Value.Count));
    }

    [Test]
    public void EnsureEditModeRestored_CallsLoadValueWithInterimTrue_ForAddedRow ()
    {
      var addedBusinessObject = (IBusinessObject)EditModeHost.Value[2];
      var dataSourceStub = new Mock<IBusinessObjectReferenceDataSource>();
      dataSourceStub.SetupProperty(_ => _.BusinessObject);
      dataSourceStub.Object.BusinessObject = new Mock<IBusinessObject>().Object;
      var addedRowDataSourceStub = new Mock<IBusinessObjectReferenceDataSource>();
      addedRowDataSourceStub.SetupProperty(_ => _.BusinessObject);
      addedRowDataSourceStub.Object.BusinessObject = addedBusinessObject;
      EditModeHost.EditModeDataSourceFactory = new Mock<EditableRowDataSourceFactory>().Object;
      Mock.Get(EditModeHost.EditModeDataSourceFactory)
          .Setup(_ => _.Create(addedBusinessObject))
          .Returns(addedRowDataSourceStub.Object);
      Mock.Get(EditModeHost.EditModeDataSourceFactory)
          .Setup(_ => _.Create(It.Is<IBusinessObject>(_ => !object.ReferenceEquals(_, addedBusinessObject))))
          .Returns(dataSourceStub.Object);

      Assert.That(Controller.IsListEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.ListEditMode, new List<string> { "0", "1", "3", "4" }, false));
      Assert.That(Controller.IsListEditModeActive, Is.True);

      Controller.EnsureEditModeRestored(Columns);
      Assert.That(Controller.IsListEditModeActive, Is.True);

      dataSourceStub.Verify(_ => _.LoadValues(true), Times.Exactly(EditModeHost.Value.Count - 1));
      addedRowDataSourceStub.Verify(_ => _.LoadValues(false), Times.Once());
    }

    [Test]
    public void EnsureEditModeRestoredWithValueNull_ThrowsInvalidOperationException ()
    {
      Assert.That(Controller.IsListEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.ListEditMode, new List<string> { "0", "1", "2", "3", "4" }, false));
      Assert.That(Controller.IsListEditModeActive, Is.True);
      EditModeHost.Value = null;
      Assert.That(
          () => Controller.EnsureEditModeRestored(Columns),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot restore edit mode: The BocList 'BocList' does not have a Value."));
    }


    [Test]
    public void SynchronizeEditModeControls_WithAddedRow ()
    {
      string idFormat = "Controller_Row_{0}";

      Assert.That(Controller.IsListEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.ListEditMode, new List<string> { "0", "1", "2", "3", "4" }, false));
      Assert.That(Controller.IsListEditModeActive, Is.True);

      Controller.EnsureEditModeRestored(Columns);
      Assert.That(Controller.IsListEditModeActive, Is.True);

      EditModeHost.Value = Values.Concat(NewValues[0]).ToArray();
      Controller.SynchronizeEditModeControls(Columns);

      Assert.That(Controller.Controls[0].ID, Is.EqualTo(string.Format(idFormat, 0)));
      Assert.That(Controller.Controls[1].ID, Is.EqualTo(string.Format(idFormat, 1)));
      Assert.That(Controller.Controls[2].ID, Is.EqualTo(string.Format(idFormat, 2)));
      Assert.That(Controller.Controls[3].ID, Is.EqualTo(string.Format(idFormat, 3)));
      Assert.That(Controller.Controls[4].ID, Is.EqualTo(string.Format(idFormat, 4)));
      Assert.That(Controller.Controls[5].ID, Is.EqualTo(string.Format(idFormat, 5)));
      var editableRow = (EditableRow)Controller.Controls[5];
      Assert.That(
          ((TypeWithAllDataTypes)editableRow.GetDataSource().BusinessObject).String,
          Is.SameAs(((TypeWithAllDataTypes)EditModeHost.Value[5]).String));

      Assert.That(EditModeHost.FocusedControl, Is.Null);
    }

    [Test]
    public void SynchronizeEditModeControls_WithRemovedRow ()
    {
      string idFormat = "Controller_Row_{0}";

      Assert.That(Controller.IsListEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.ListEditMode, new List<string> { "0", "1", "2", "3", "4" }, false));
      Assert.That(Controller.IsListEditModeActive, Is.True);

      Controller.EnsureEditModeRestored(Columns);
      Assert.That(Controller.IsListEditModeActive, Is.True);

      EditModeHost.Value = Values.Take(4).ToArray();
      Controller.SynchronizeEditModeControls(Columns);

      Assert.That(Controller.Controls[0].ID, Is.EqualTo(string.Format(idFormat, 0)));
      Assert.That(Controller.Controls[1].ID, Is.EqualTo(string.Format(idFormat, 1)));
      Assert.That(Controller.Controls[2].ID, Is.EqualTo(string.Format(idFormat, 2)));
      Assert.That(Controller.Controls[3].ID, Is.EqualTo(string.Format(idFormat, 3)));
      var editableRow = (EditableRow)Controller.Controls[3];
      Assert.That(
          ((TypeWithAllDataTypes)editableRow.GetDataSource().BusinessObject).String,
          Is.SameAs(((TypeWithAllDataTypes)EditModeHost.Value[3]).String));
    }

    [Test]
    public void SynchronizeEditModeControls_CallsLoadValueWithInterimTrue_ForAddedRow ()
    {
      var addedBusinessObject = (IBusinessObject)NewValues[0];
      var dataSourceStub = new Mock<IBusinessObjectReferenceDataSource>();
      dataSourceStub.SetupProperty(_ => _.BusinessObject);
      dataSourceStub.Object.BusinessObject = new Mock<IBusinessObject>().Object;
      var addedRowDataSourceStub = new Mock<IBusinessObjectReferenceDataSource>();
      addedRowDataSourceStub.Object.BusinessObject = addedBusinessObject;
      EditModeHost.EditModeDataSourceFactory = new Mock<EditableRowDataSourceFactory>().Object;
      Mock.Get(EditModeHost.EditModeDataSourceFactory)
          .Setup(_ => _.Create(addedBusinessObject))
          .Returns(addedRowDataSourceStub.Object);
      Mock.Get(EditModeHost.EditModeDataSourceFactory)
          .Setup(_ => _.Create(It.Is<IBusinessObject>(_ => !object.ReferenceEquals(_, addedBusinessObject))))
          .Returns(dataSourceStub.Object);

      Assert.That(Controller.IsListEditModeActive, Is.False);
      ControllerInvoker.LoadControlState(CreateControlState(null, EditMode.ListEditMode, new List<string> { "0", "1", "2", "3", "4" }, false));
      Assert.That(Controller.IsListEditModeActive, Is.True);

      Controller.EnsureEditModeRestored(Columns);
      Assert.That(Controller.IsListEditModeActive, Is.True);

      EditModeHost.Value = Values.Concat(NewValues[0]).ToArray();
      Controller.SynchronizeEditModeControls(Columns);

      dataSourceStub.Verify(_ => _.LoadValues(true), Times.Exactly(EditModeHost.Value.Count - 1));
      addedRowDataSourceStub.Verify(_ => _.LoadValues(false), Times.Once());
    }


    [Test]
    public void AddRow ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));

      Assert.That(Controller.AddRow(NewValues[0], Columns), Is.EqualTo(5));

      Assert.That(EditModeHost.Value.Count, Is.EqualTo(6));
      Assert.That(EditModeHost.Value[5], Is.SameAs(NewValues[0]));

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(Controller.Controls.Count, Is.EqualTo(6));
      string idFormat = "Controller_Row_{0}";
      Assert.That(Controller.Controls[0].ID, Is.EqualTo(string.Format(idFormat, 0)));
      Assert.That(Controller.Controls[1].ID, Is.EqualTo(string.Format(idFormat, 1)));
      Assert.That(Controller.Controls[2].ID, Is.EqualTo(string.Format(idFormat, 2)));
      Assert.That(Controller.Controls[3].ID, Is.EqualTo(string.Format(idFormat, 3)));
      Assert.That(Controller.Controls[4].ID, Is.EqualTo(string.Format(idFormat, 4)));
      Assert.That(Controller.Controls[5].ID, Is.EqualTo(string.Format(idFormat, 5)));

      Assert.That(ActualEvents.Count, Is.EqualTo(0));

      Assert.That(EditModeHost.FocusedControl, Is.Not.Null);
      Assert.That(EditModeHost.FocusedControl.FocusID, Is.EqualTo("NamingContainer_Controller_Row_5_0_Value"));
    }

    [Test]
    public void AddRow_WithDisabledAutoFocus_IgnoresFlag ()
    {
      Invoker.InitRecursive();
      EditModeHost.IsAutoFocusOnSwitchToEditModeEnabled = false;
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));

      Assert.That(Controller.AddRow(NewValues[0], Columns), Is.EqualTo(5));

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(Controller.Controls.Count, Is.EqualTo(6));

      Assert.That(EditModeHost.FocusedControl, Is.Not.Null);
      Assert.That(EditModeHost.FocusedControl.FocusID, Is.EqualTo("NamingContainer_Controller_Row_5_0_Value"));
    }

    [Test]
    public void AddRow_CallsEditModeHost ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      IBusinessObject[] addedRows = null;
      EditModeHost.NotifyAddRows = objects =>
      {
        addedRows = objects;
        return new BocListRow[0];
      };
      Controller.AddRow(NewValues[0], Columns);

      Assert.That(addedRows, Is.EquivalentTo(new[] { NewValues[0] }));
    }


    [Test]
    public void AddRows ()
    {
      Invoker.InitRecursive();
      EditModeHost.RowIDProvider = new IndexBasedRowIDProvider(EditModeHost.Value.Cast<IBusinessObject>());
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));

      Controller.AddRows(NewValues, Columns);

      Assert.That(EditModeHost.Value.Count, Is.EqualTo(7));
      Assert.That(EditModeHost.Value[5], Is.SameAs(NewValues[0]));
      Assert.That(EditModeHost.Value[6], Is.SameAs(NewValues[1]));

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(Controller.Controls.Count, Is.EqualTo(7));
      string idFormat = "Controller_Row_{0}";
      Assert.That(Controller.Controls[0].ID, Is.EqualTo(string.Format(idFormat, 0)));
      Assert.That(Controller.Controls[1].ID, Is.EqualTo(string.Format(idFormat, 1)));
      Assert.That(Controller.Controls[2].ID, Is.EqualTo(string.Format(idFormat, 2)));
      Assert.That(Controller.Controls[3].ID, Is.EqualTo(string.Format(idFormat, 3)));
      Assert.That(Controller.Controls[4].ID, Is.EqualTo(string.Format(idFormat, 4)));
      Assert.That(Controller.Controls[5].ID, Is.EqualTo(string.Format(idFormat, 5)));
      Assert.That(Controller.Controls[6].ID, Is.EqualTo(string.Format(idFormat, 6)));

      Assert.That(ActualEvents.Count, Is.EqualTo(0));

      Assert.That(Controller.GetEditableRow(0), Is.SameAs(Controller.Controls[0]));
      Assert.That(Controller.GetEditableRow(1), Is.SameAs(Controller.Controls[1]));
      Assert.That(Controller.GetEditableRow(2), Is.SameAs(Controller.Controls[2]));
      Assert.That(Controller.GetEditableRow(3), Is.SameAs(Controller.Controls[3]));
      Assert.That(Controller.GetEditableRow(4), Is.SameAs(Controller.Controls[4]));
      Assert.That(Controller.GetEditableRow(5), Is.SameAs(Controller.Controls[5]));
      Assert.That(Controller.GetEditableRow(6), Is.SameAs(Controller.Controls[6]));

      Assert.That(EditModeHost.FocusedControl, Is.Not.Null);
      Assert.That(EditModeHost.FocusedControl.FocusID, Is.EqualTo("NamingContainer_Controller_Row_5_0_Value"));
    }

    [Test]
    public void AddRows_WithDisabledAutoFocus_IgnoresFlag ()
    {
      Invoker.InitRecursive();
      EditModeHost.IsAutoFocusOnSwitchToEditModeEnabled = false;
      EditModeHost.RowIDProvider = new IndexBasedRowIDProvider(EditModeHost.Value.Cast<IBusinessObject>());
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));

      Controller.AddRows(NewValues, Columns);

      Assert.That(EditModeHost.Value.Count, Is.EqualTo(7));

      Assert.That(EditModeHost.FocusedControl, Is.Not.Null);
      Assert.That(EditModeHost.FocusedControl.FocusID, Is.EqualTo("NamingContainer_Controller_Row_5_0_Value"));
    }

    [Test]
    public void AddRows_CallsEditModeHost ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      IBusinessObject[] addedRows = null;
      EditModeHost.NotifyAddRows = objects =>
      {
        addedRows = objects;
        return new BocListRow[0];
      };
      Controller.AddRows(NewValues, Columns);

      Assert.That(addedRows, Is.SameAs(NewValues));
    }


    [Test]
    public void RemoveRow ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));

      Controller.RemoveRow(Values[2]);

      Assert.That(EditModeHost.Value.Count, Is.EqualTo(4));
      Assert.That(EditModeHost.Value[0], Is.SameAs(Values[0]));
      Assert.That(EditModeHost.Value[1], Is.SameAs(Values[1]));
      Assert.That(EditModeHost.Value[2], Is.SameAs(Values[3]));
      Assert.That(EditModeHost.Value[3], Is.SameAs(Values[4]));

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(Controller.Controls.Count, Is.EqualTo(4));
      string idFormat = "Controller_Row_{0}";
      Assert.That(Controller.Controls[0].ID, Is.EqualTo(string.Format(idFormat, 0)));
      Assert.That(Controller.Controls[1].ID, Is.EqualTo(string.Format(idFormat, 1)));
      Assert.That(Controller.Controls[2].ID, Is.EqualTo(string.Format(idFormat, 3)));
      Assert.That(Controller.Controls[3].ID, Is.EqualTo(string.Format(idFormat, 4)));

      Assert.That(ActualEvents.Count, Is.EqualTo(0));
    }

    [Test]
    public void RemoveRow_CallsEditModeHost ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      var businessObject = Values[2];
      IBusinessObject[] removedRows = null;
      EditModeHost.NotifyRemoveRows = objects =>
      {
        removedRows = objects;
        return new BocListRow[0];
      };
      Controller.RemoveRow(businessObject);

      Assert.That(removedRows, Is.EqualTo(new[] { businessObject }).AsCollection);
    }

    [Test]
    public void RemoveRows ()
    {
      Invoker.InitRecursive();
      EditModeHost.RowIDProvider = new IndexBasedRowIDProvider(EditModeHost.Value.Cast<IBusinessObject>());
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(EditModeHost.Value.Count, Is.EqualTo(5));

      Controller.RemoveRows(new[] { Values[2] });

      Assert.That(EditModeHost.Value.Count, Is.EqualTo(4));
      Assert.That(EditModeHost.Value[0], Is.SameAs(Values[0]));
      Assert.That(EditModeHost.Value[1], Is.SameAs(Values[1]));
      Assert.That(EditModeHost.Value[2], Is.SameAs(Values[3]));
      Assert.That(EditModeHost.Value[3], Is.SameAs(Values[4]));

      Assert.That(Controller.IsListEditModeActive, Is.True);
      Assert.That(Controller.Controls.Count, Is.EqualTo(4));
      string idFormat = "Controller_Row_{0}";
      Assert.That(Controller.Controls[0].ID, Is.EqualTo(string.Format(idFormat, 0)));
      Assert.That(Controller.Controls[1].ID, Is.EqualTo(string.Format(idFormat, 1)));
      Assert.That(Controller.Controls[2].ID, Is.EqualTo(string.Format(idFormat, 3)));
      Assert.That(Controller.Controls[3].ID, Is.EqualTo(string.Format(idFormat, 4)));

      Assert.That(ActualEvents.Count, Is.EqualTo(0));

      Assert.That(Controller.GetEditableRow(0), Is.SameAs(Controller.Controls[0]));
      Assert.That(Controller.GetEditableRow(1), Is.SameAs(Controller.Controls[1]));
      Assert.That(Controller.GetEditableRow(2), Is.SameAs(Controller.Controls[2]));
      Assert.That(Controller.GetEditableRow(3), Is.SameAs(Controller.Controls[3]));
    }

    [Test]
    public void RemoveRows_CallsEditModeHost ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      var businessObjects = new[] { Values[2] };
      IBusinessObject[] removedRows = null;
      EditModeHost.NotifyRemoveRows = objects =>
      {
        removedRows = objects;
        return new BocListRow[0];
      };
      Controller.RemoveRows(businessObjects);

      Assert.That(removedRows, Is.SameAs(businessObjects));
    }

    [Test]
    public void ValidateWithValidValues ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      SetValues((EditableRow)Controller.Controls[0], "New Value A", "100");
      SetValues((EditableRow)Controller.Controls[1], "New Value B", "200");
      SetValues((EditableRow)Controller.Controls[2], "New Value C", "300");
      SetValues((EditableRow)Controller.Controls[3], "New Value D", "400");
      SetValues((EditableRow)Controller.Controls[4], "New Value E", "500");

      Assert.That(Controller.Validate(), Is.True);
    }

    [Test]
    public void ValidateWithInvalidValues ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      SetValues((EditableRow)Controller.Controls[0], "New Value A", "");
      SetValues((EditableRow)Controller.Controls[1], "New Value B", "");
      SetValues((EditableRow)Controller.Controls[2], "New Value C", "");
      SetValues((EditableRow)Controller.Controls[3], "New Value D", "");
      SetValues((EditableRow)Controller.Controls[4], "New Value E", "");

      Assert.That(Controller.Validate(), Is.False);
    }


    [Test]
    public void PrepareValidation ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode(2, Columns);

      for (int i = 0; i < Controller.Controls.Count; i++)
      {
        EditableRow editableRow = (EditableRow)Controller.Controls[i];

        BocTextValue stringValueField = (BocTextValue)editableRow.GetEditControl(0);
        BocTextValue int32ValueField = (BocTextValue)editableRow.GetEditControl(1);

        Controller.PrepareValidation();

        Assert.AreEqual(stringValueField.Text, stringValueField.Text, "Row {0}", i);
        Assert.AreEqual(int32ValueField.Text, int32ValueField.Text, "Row {0}", i);
      }
    }


    [Test]
    public void IsRequired ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      Assert.That(Controller.IsListEditModeActive, Is.True);

      Assert.That(Controller.IsRequired(0), Is.False);
      Assert.That(Controller.IsRequired(1), Is.True);
    }

    [Test]
    public void IsDirty ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      EditableRow row = (EditableRow)Controller.Controls[2];
      Remotion.ObjectBinding.Web.UI.Controls.BocTextValue stringValueField =
          (Remotion.ObjectBinding.Web.UI.Controls.BocTextValue)row.GetEditControl(0);
      stringValueField.Value = "New Value";

      Assert.That(Controller.IsDirty(), Is.True);
    }

    [Test]
    public void GetTrackedIDs ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);

      string id = "NamingContainer_Controller_Row_{0}_{1}_Value";
      string[] trackedIDs = new string[10];
      for (int i = 0; i < 5; i++)
      {
        trackedIDs[2 * i] = string.Format(id, i, 0);
        trackedIDs[2 * i + 1] = string.Format(id, i, 1);
      }

      Assert.That(Controller.GetTrackedClientIDs(), Is.EqualTo(trackedIDs));
    }


    [Test]
    public void SaveAndLoadControlState ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);
      Assert.That(Controller.IsListEditModeActive, Is.True);

      object viewState = ControllerInvoker.SaveControlState();
      Assert.That(viewState, Is.Not.Null);

      Controller.EndListEditMode(false, Columns);
      Assert.That(Controller.IsListEditModeActive, Is.False);

      ControllerInvoker.LoadControlState(viewState);
      Assert.That(Controller.IsListEditModeActive, Is.True);
    }

    [Test]
    public void SaveAndLoadControlStateAfterRemovingSingleRow ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);
      Controller.RemoveRow(Values[2]);
      Assert.That(Controller.IsListEditModeActive, Is.True);

      object viewState = ControllerInvoker.SaveControlState();
      Assert.That(viewState, Is.Not.Null);
      Assert.That(viewState is Object[], Is.True);
      object[] values = (object[])viewState;
      Assert.That(values.Length, Is.EqualTo(4));

      Controller.EndListEditMode(false, Columns);
      Assert.That(Controller.IsListEditModeActive, Is.False);

      ControllerInvoker.LoadControlState(viewState);
      Assert.That(Controller.IsListEditModeActive, Is.True);
    }

    [Test]
    public void SaveAndLoadControlStateAfterRemovingMultipleRows ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode(Columns);
      Controller.RemoveRows(new [] {Values[2], Values[3]});
      Assert.That(Controller.IsListEditModeActive, Is.True);

      object viewState = ControllerInvoker.SaveControlState();
      Assert.That(viewState, Is.Not.Null);
      Assert.That(viewState is Object[], Is.True);
      object[] values = (object[])viewState;
      Assert.That(values.Length, Is.EqualTo(4));

      Controller.EndListEditMode(false, Columns);
      Assert.That(Controller.IsListEditModeActive, Is.False);

      ControllerInvoker.LoadControlState(viewState);
      Assert.That(Controller.IsListEditModeActive, Is.True);
    }
  }
}
