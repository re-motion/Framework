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
using System.Drawing;
using System.Linq;
using Coypu;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  [RequiresUserInterface]
  public class BocListAsGridControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(DisabledTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [TestCaseSource(typeof(ReadOnlyTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [TestCaseSource(typeof(LabelTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [TestCaseSource(typeof(ValidationErrorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocListAsGridSelector, BocListAsGridControlObject> testAction)
    {
      testAction(Helper, e => e.ListAsGrids(), "listAsGrid");
    }

    [Test]
    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [TestCaseSource(typeof(DomainPropertyControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [TestCaseSource(typeof(DisplayNameControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocListAsGridSelector, BocListAsGridControlObject> testAction)
    {
      testAction(Helper, e => e.ListAsGrids(), "listAsGrid");
    }

    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_DerivedType ()
    {
      var home = Start();
      var controlObjectContext = home.ListAsGrids().GetByLocalID("JobList_Normal").Context;
      var controlObject = new DerivedBocListAsGridControlObject(controlObjectContext);
      var fluentControlObject = controlObject.ForScreenshot();
      var derivedControlObject = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocList<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject>(
              fluentControlObject.GetTarget().FluentList, fluentControlObject.GetTarget().FluentElement));

      var fluentTableContainer = derivedControlObject.GetTableContainer();
      Assert.That(fluentTableContainer, Is.Not.Null);
      var derivedTableContainer = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListTableContainer<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject>(
              fluentTableContainer.GetTarget().FluentList, fluentTableContainer.GetTarget().FluentElement));
      var fluentHeaderRow = derivedTableContainer.GetHeaderRow();
      Assert.That(fluentHeaderRow, Is.Not.Null);
      var derivedHeaderRow = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListHeaderRow<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject>(
              fluentTableContainer.GetTarget().FluentList,
              fluentHeaderRow.GetTarget().FluentElement),
          minimumElementVisibility: ((IFluentScreenshotElement)fluentHeaderRow).MinimumElementVisibility);
      Assert.That(derivedHeaderRow.GetCell(), Is.Not.Null);
      Assert.That(derivedHeaderRow.GetCell(1), Is.Not.Null);
      Assert.That(derivedHeaderRow.GetCell("RowCmd"), Is.Not.Null);
      Assert.That(derivedTableContainer.GetRowCount(), Is.Not.Null);
      Assert.That(derivedTableContainer.GetRow(), Is.Not.Null);
      Assert.That(derivedTableContainer.GetRow("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98"), Is.Not.Null);
      var fluentRow = derivedTableContainer.GetRow(1);
      Assert.That(fluentRow, Is.Not.Null);
      var derivedRow = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListRow<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject>(
              fluentRow.GetTarget().FluentList,
              fluentRow.GetTarget().FluentRow),
          minimumElementVisibility: ((IFluentScreenshotElement)fluentHeaderRow).MinimumElementVisibility);
      Assert.That(derivedRow.GetCell(), Is.Not.Null);
      Assert.That(derivedRow.GetCell(1), Is.Not.Null);
      Assert.That(derivedRow.GetCell("RowCmd"), Is.Not.Null);
      Assert.That(derivedTableContainer.GetColumn(), Is.Not.Null);
      Assert.That(derivedTableContainer.GetColumn("RowCmd"), Is.Not.Null);
      var fluentColumn = derivedTableContainer.GetColumn(1);
      Assert.That(fluentColumn, Is.Not.Null);
      var derivedColumn = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListColumn<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject>(
              fluentTableContainer.GetTarget().FluentList,
              includeHeader : false,
              columnIndex : 1),
          minimumElementVisibility: ((IFluentScreenshotElement)fluentHeaderRow).MinimumElementVisibility);
      Assert.That(derivedColumn.GetCell(), Is.Not.Null);
      Assert.That(derivedColumn.GetCell(1), Is.Not.Null);
      Assert.That(derivedColumn.GetCell("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98"), Is.Not.Null);

      var fluentNavigator = derivedControlObject.GetNavigator();
      Assert.That(fluentNavigator, Is.Not.Null);
      var derivedNavigator = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListNavigator<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject>(
              fluentNavigator.GetTarget().FluentList, fluentNavigator.GetTarget().FluentElement));
      Assert.That(derivedNavigator.GetFirstPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetLastPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetNextPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetPreviousPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetPageInformationText(), Is.Not.Null);

      var fluentMenuBlock = derivedControlObject.GetMenuBlock();
      Assert.That(fluentMenuBlock, Is.Not.Null);
      var derivedMenuBlock = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListMenuBlock<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject>(
              fluentMenuBlock.GetTarget().FluentList, fluentMenuBlock.GetTarget().FluentElement));
      Assert.That(derivedMenuBlock.GetDropDownMenu(), Is.Not.Null);
      Assert.That(derivedMenuBlock.GetListMenu(), Is.Not.Null);

      var fluentDropDown = derivedMenuBlock.GetViewsMenu();
      Assert.That(fluentDropDown, Is.Not.Null);
      var derivedDropDown = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListDropDown<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject>(
              fluentDropDown.GetTarget().FluentList, fluentDropDown.GetTarget().FluentElement));
      Assert.That(() => derivedDropDown.Open(), Throws.Nothing);
    }

    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_DerivedTypeGeneric ()
    {
      var home = Start();
      var controlObjectContext = home.ListAsGrids().GetByLocalID("JobList_Normal").Context;
      var controlObject = new DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>(controlObjectContext);
      var fluentControlObject =
          controlObject.ForBocListAsGridScreenshot<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject>();
      var derivedControlObject = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocList<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject,
              BocListAsGridCellControlObject>(
              fluentControlObject.GetTarget().FluentList,
              fluentControlObject.GetTarget().FluentElement));

      var fluentTableContainer = derivedControlObject.GetTableContainer();
      Assert.That(fluentTableContainer, Is.Not.Null);
      var derivedTableContainer = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListTableContainer<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject,
              BocListAsGridCellControlObject>(
              fluentTableContainer.GetTarget().FluentList,
              fluentTableContainer.GetTarget().FluentElement));
      var fluentHeaderRow = derivedTableContainer.GetHeaderRow();
      Assert.That(fluentHeaderRow, Is.Not.Null);
      var derivedHeaderRow = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListHeaderRow<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject,
              BocListAsGridCellControlObject>(
              fluentTableContainer.GetTarget().FluentList,
              fluentHeaderRow.GetTarget().FluentElement),
          minimumElementVisibility: ((IFluentScreenshotElement)fluentHeaderRow).MinimumElementVisibility);
      Assert.That(derivedHeaderRow.GetCell(), Is.Not.Null);
      Assert.That(derivedHeaderRow.GetCell(1), Is.Not.Null);
      Assert.That(derivedHeaderRow.GetCell("RowCmd"), Is.Not.Null);
      Assert.That(derivedTableContainer.GetRowCount(), Is.Not.Null);
      Assert.That(derivedTableContainer.GetRow(), Is.Not.Null);
      Assert.That(derivedTableContainer.GetRow("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98"), Is.Not.Null);
      var fluentRow = derivedTableContainer.GetRow(1);
      Assert.That(fluentRow, Is.Not.Null);
      var derivedRow = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListRow<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject,
              BocListAsGridCellControlObject>(
              fluentRow.GetTarget().FluentList,
              fluentRow.GetTarget().FluentRow),
          minimumElementVisibility: ((IFluentScreenshotElement)fluentHeaderRow).MinimumElementVisibility);
      Assert.That(derivedRow.GetCell(), Is.Not.Null);
      Assert.That(derivedRow.GetCell(1), Is.Not.Null);
      Assert.That(derivedRow.GetCell("RowCmd"), Is.Not.Null);
      Assert.That(derivedTableContainer.GetColumn(), Is.Not.Null);
      Assert.That(derivedTableContainer.GetColumn("RowCmd"), Is.Not.Null);
      var fluentColumn = derivedTableContainer.GetColumn(1);
      Assert.That(fluentColumn, Is.Not.Null);
      var derivedColumn = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListColumn<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject,
              BocListAsGridCellControlObject>(
              fluentTableContainer.GetTarget().FluentList,
              includeHeader: false,
              columnIndex: 1),
          minimumElementVisibility: ((IFluentScreenshotElement)fluentHeaderRow).MinimumElementVisibility);
      Assert.That(derivedColumn.GetCell(), Is.Not.Null);
      Assert.That(derivedColumn.GetCell(1), Is.Not.Null);
      Assert.That(derivedColumn.GetCell("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98"), Is.Not.Null);

      var fluentNavigator = derivedControlObject.GetNavigator();
      Assert.That(fluentNavigator, Is.Not.Null);
      var derivedNavigator = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListNavigator<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject,
              BocListAsGridCellControlObject>(
              fluentNavigator.GetTarget().FluentList,
              fluentNavigator.GetTarget().FluentElement));
      Assert.That(derivedNavigator.GetFirstPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetLastPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetNextPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetPreviousPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetPageInformationText(), Is.Not.Null);

      var fluentMenuBlock = derivedControlObject.GetMenuBlock();
      Assert.That(fluentMenuBlock, Is.Not.Null);
      var derivedMenuBlock = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListMenuBlock<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject,
              BocListAsGridCellControlObject>(
              fluentMenuBlock.GetTarget().FluentList,
              fluentMenuBlock.GetTarget().FluentElement));
      Assert.That(derivedMenuBlock.GetDropDownMenu(), Is.Not.Null);
      Assert.That(derivedMenuBlock.GetListMenu(), Is.Not.Null);

      var fluentDropDown = derivedMenuBlock.GetViewsMenu();
      Assert.That(fluentDropDown, Is.Not.Null);
      var derivedDropDown = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListDropDown<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject,
              BocListAsGridCellControlObject>(
              fluentDropDown.GetTarget().FluentList,
              fluentDropDown.GetTarget().FluentElement));
      Assert.That(() => derivedDropDown.Open(), Throws.Nothing);
    }

    [Ignore("TODO: RM-8902 is required for this test to work on build agent because the screenshot is too big.")]
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_ValidationMarkerAndMessages ()
    {
      var home = Start();

      home.WebButtons().GetByLocalID("ValidationTestCaseCellButton").Click();

      var control = home.ListAsGrids().GetByLocalID("JobList_Validation");
      var fluentControl = control.ForScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<ScreenshotBocList<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject>>,
          BocListAsGridControlObject>(
              fluentControl,
              ScreenshotTestingType.Both,
              (builder, target) =>
              {
                var row = target.GetTableContainer().GetRow().WithIndex(1);

                builder.AnnotateBox(row.GetErrors(), Pens.Red, WebPadding.Inner);
                builder.AnnotateBox(row.GetCell(6), Pens.Yellow, WebPadding.Inner);
                builder.AnnotateBox(row.GetCell(6).GetErrorMarker(), Pens.Blue, WebPadding.Inner);
                builder.AnnotateBox(row.GetErrorMarker(), Pens.Green, WebPadding.Inner);
                builder.AnnotateBox(target.GetTableContainer().GetHeaderRow().GetErrorMarker(), Pens.Orange, WebPadding.Inner);

                builder.Crop(row.GetCell(3), new WebPadding(0, 25, 0, 30), isRestrictedByParent: false);
              });
    }

    [Test]
    public void TestGetColumnDefinitions ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      Assert.That(
          bocList.GetColumnDefinitions().Select(cd => cd.Title),
          Is.EquivalentTo(new[] { "I_ndex", "", "Command", "Menu", "Title", "StartDate", "EndDate", "DisplayName", "TitleWithCmd" }));
    }

    [Test]
    public void TestGetColumnDefinitionsWithRowHeaders ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_EmptyWithRowHeaders");
      Assert.That(
          bocList.GetColumnDefinitions().Select(cd => (cd.Title, cd.IsRowHeader)),
          Is.EquivalentTo(new[] { ("Title", true), ("StartDate", false), ("EndDate", false), ("DisplayName", true) }));
    }

    [Test]
    public void TestGetDisplayedRows ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var rows = bocList.GetDisplayedRows();
      Assert.That(rows.Count, Is.EqualTo(8));
      Assert.That(rows[1].GetCell("DisplayName").GetText(), Is.EqualTo("CEO"));
    }

    [Test]
    public void TestGetRowCellCommandDefaultCompletionDetectionStrategy ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var cell = bocList.GetDisplayedRows()[1].GetCell(3);

      var cellCommand = cell.GetCommand();
      var completionDetection = new CompletionDetectionStrategyTestHelper(cellCommand);
      cellCommand.Click();

      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
    }

    [Test]
    public void TestGetNumberOfRows ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      Assert.That(bocList.GetNumberOfRows(), Is.EqualTo(8));
    }

    [Test]
    public void TestEmptyList ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Empty");
      Assert.That(bocList.GetNumberOfRows(), Is.EqualTo(0));
      Assert.That(bocList.IsEmpty(), Is.True);
      Assert.That(bocList.GetEmptyMessage(), Is.EqualTo("A wonderful empty list."));
    }

    [Test]
    public void TestSelectAllAndDeselectAll ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var firstRow = bocList.GetRow(1);
      var lastRow = bocList.GetRow(bocList.GetNumberOfRows());
      var completionDetection = new CompletionDetectionStrategyTestHelper(bocList);

      Assert.That(firstRow.IsSelected, Is.False);
      Assert.That(lastRow.IsSelected, Is.False);

      bocList.SelectAll();

      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(firstRow.IsSelected, Is.True);
      Assert.That(lastRow.IsSelected, Is.True);

      bocList.SelectAll();

      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(firstRow.IsSelected, Is.True);
      Assert.That(lastRow.IsSelected, Is.True);

      bocList.DeselectAll();

      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(firstRow.IsSelected, Is.False);
      Assert.That(lastRow.IsSelected, Is.False);
    }

    [Test]
    public void TestGetRow ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");

      var row = bocList.GetRow("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98");
      Assert.That(row.GetCell(5).GetText(), Is.EqualTo("CEO"));

      row = bocList.GetRow(1);
      Assert.That(row.GetCell(5).GetText(), Is.EqualTo("Programmer"));
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var dropDownMenu = bocList.GetDropDownMenu();
      dropDownMenu.SelectItem("OptCmd2");

      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("JobList_Normal"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderRowLabel").Text, Is.EqualTo("-1"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("ListMenuOrOptionsClick"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("OptCmd2|Option command 2"));
    }

    [Test]
    public void TestGetListMenu ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var listMenu = bocList.GetListMenu();
      listMenu.SelectItem("ListMenuCmd3");

      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("JobList_Normal"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderRowLabel").Text, Is.EqualTo("-1"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("ListMenuOrOptionsClick"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("ListMenuCmd3|LM cmd 3"));
    }

    [Test]
    public void TestRowGetCell ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var row = bocList.GetRow(2);

      var cell = row.GetCell("DisplayName");
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));

      cell = row.GetCell(5);
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));

      cell = row.GetCell().WithColumnTitle("DisplayName");
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));

      cell = row.GetCell().WithColumnTitleContains("layNam");
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));
    }

    [Test]
    public void TestRowSelectAndDeselect ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var row = bocList.GetRow(2);
      var completionDetection = new CompletionDetectionStrategyTestHelper(row);

      row.Select();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());

      row.GetCell(3).ExecuteCommand();
      Assert.That(home.Scope.FindIdEndingWith("SelectedIndicesLabel").Text, Is.EqualTo("1"));

      row.Select();
      row.GetCell(3).ExecuteCommand();
      Assert.That(home.Scope.FindIdEndingWith("SelectedIndicesLabel").Text, Is.EqualTo("1"));

      row.Deselect();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());

      row.GetCell(3).ExecuteCommand();
      Assert.That(home.Scope.FindIdEndingWith("SelectedIndicesLabel").Text, Is.EqualTo("NoneSelected"));
    }

    [Test]
    public void TestRowGetRowDropDownMenu ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var row = bocList.GetRow(2);
      var dropDownMenu = row.GetDropDownMenu();
      dropDownMenu.SelectItem("RowMenuItemCmd2");

      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("JobList_Normal"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderRowLabel").Text, Is.EqualTo("1"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("RowContextMenuClick"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("RowMenuItemCmd2|Row menu 2"));
    }

    [Test]
    public void TestCellGetText ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var cell = bocList.GetRow(2).GetCell(5);

      Assert.That(cell.GetText(), Is.EqualTo("CEO"));
    }

    [Test]
    public void TestCellPerformCommand ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var cell = bocList.GetRow(2).GetCell(3);

      cell.ExecuteCommand();

      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("JobList_Normal"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderRowLabel").Text, Is.EqualTo("1"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("CellCommandClick"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("RowCmd"));
    }

    [Test]
    public void TestEditableCellGetControl ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var editableRow = bocList.GetRow(2);
      var editableCell = editableRow.GetCell(6);

      var bocText = editableCell.TextValues().First();
      bocText.FillWith("NewTitle");

      Assert.That(bocText.GetText(), Is.EqualTo("NewTitle"));
    }

    [Test]
    public void TestGetCellViaPropertyPath ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var rows = bocList.GetDisplayedRows();
      var cell = rows[0].GetCell().WithDomainPropertyPath("DisplayName");

      Assert.That(cell.GetText(), Is.EqualTo("Programmer"));
    }

    [Test]
    public void TestGetCurrentPage ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(1));
    }

    [Test]
    public void TestGetCurrentPage_WithoutNavigator ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Empty");
      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(1));
    }

    [Test]
    public void TestGetNumberOfPages ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");

      Assert.That(bocList.GetNumberOfPages(), Is.EqualTo(1));
    }

    [Test]
    public void TestGetNumberOfPages_WithoutNavigator ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Empty");
      Assert.That(bocList.GetNumberOfPages(), Is.EqualTo(1));
    }

    [Test]
    public void TestGoToSpecificPage ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");

      Assert.That(
          () => bocList.GoToSpecificPage(1),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List is currently in edit mode.").Message));
    }

    [Test]
    public void TestGoToFirstPage ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");

      Assert.That(
          () => bocList.GoToFirstPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List is currently in edit mode.").Message));
    }

    [Test]
    public void TestGoToPreviousPage ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");

      Assert.That(
          () => bocList.GoToPreviousPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List is currently in edit mode.").Message));
    }

    [Test]
    public void TestGoToNextPage ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");

      Assert.That(
          () => bocList.GoToNextPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List is currently in edit mode.").Message));
    }

    [Test]
    public void TestGoToLastPage ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");

      Assert.That(
          () => bocList.GoToLastPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List is currently in edit mode.").Message));
    }

    [Test]
    public void GetValidationErrors_NoValidationFailures ()
    {
      var home = Start();
      var bocList = home.ListAsGrids().GetByLocalID("JobList_Validation");

      Assert.That(bocList.GetRow(1).GetValidationErrors(), Is.Empty);
    }

    [Test]
    public void GetValidationErrors_RowValidationFailure ()
    {
      var home = Start();
      var bocList = home.ListAsGrids().GetByLocalID("JobList_Validation");

      home.WebButtons().GetByLocalID("ValidationTestCaseRowButton").Click();

      var expectedValidationFailures = new []
                                       {
                                           new BocListValidationError(
                                               "Localized row validation failure message",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               null,
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               null)
                                       };

      var bocListValidationFailures = bocList.GetRow(1).GetValidationErrors();
      Assert.That(bocListValidationFailures, Is.EqualTo(expectedValidationFailures));
    }

    [Test]
    public void GetValidationErrors_CellValidationFailureOnRow ()
    {
      var home = Start();
      var bocList = home.ListAsGrids().GetByLocalID("JobList_Validation");

      home.WebButtons().GetByLocalID("ValidationTestCaseCellButton").Click();

      var expectedValidationFailures = new []
                                       {
                                           new BocListValidationError(
                                               "DisplayName: Localized cell validation failure message",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               "DisplayName",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               "DisplayName")
                                       };

      var bocListValidationFailures = bocList.GetRow(1).GetValidationErrors();
      Assert.That(bocListValidationFailures, Is.EqualTo(expectedValidationFailures));
    }

    [Test]
    public void GetValidationErrors_CellValidationFailure ()
    {
      var home = Start();
      var bocList = home.ListAsGrids().GetByLocalID("JobList_Validation");

      home.WebButtons().GetByLocalID("ValidationTestCaseCellButton").Click();

      var expectedValidationFailures = new []
                                       {
                                           new BocListValidationError(
                                               "Localized cell validation failure message",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               "DisplayName",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               "DisplayName")
                                       };

      var bocListValidationFailures = bocList.GetRow(1).GetCell().WithColumnTitle("DisplayName").GetValidationErrors();
      Assert.That(bocListValidationFailures, Is.EqualTo(expectedValidationFailures));
    }

    [Test]
    public void GetValidationError_EditableRowAndNoValidationFailures ()
    {
      var home = Start();
      var bocList = home.ListAsGrids().GetByLocalID("JobList_Validation");

      var row = bocList.GetRow(1);

      Assert.That(row.GetValidationErrors(), Is.Empty);
    }

    [Test]
    public void GetValidationErrors_EditableRowAndRowValidationFailure ()
    {
      var home = Start();
      var bocList = home.ListAsGrids().GetByLocalID("JobList_Validation");

      var row = bocList.GetRow(1);

      home.WebButtons().GetByLocalID("ValidationTestCaseRowButton").Click();

      var expectedValidationFailures = new []
                                       {
                                           new BocListValidationError(
                                               "Localized row validation failure message",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               null,
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               null)
                                       };

      Assert.That(row.GetValidationErrors(), Is.EqualTo(expectedValidationFailures));
    }

    [Test]
    public void GetValidationErrors_EditableRowAndCellValidationFailureOnRow ()
    {
      var home = Start();
      var bocList = home.ListAsGrids().GetByLocalID("JobList_Validation");

      var row = bocList.GetRow(1);

      home.WebButtons().GetByLocalID("ValidationTestCaseCellButton").Click();

      var expectedValidationFailures = new []
                                       {
                                           new BocListValidationError(
                                               "DisplayName: Localized cell validation failure message",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               "DisplayName",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               "DisplayName")
                                       };

      Assert.That(row.GetValidationErrors(), Is.EqualTo(expectedValidationFailures));
    }

    [Test]
    public void GetValidationErrors_EditableRowAndCellValidationFailure ()
    {
      var home = Start();
      var bocList = home.ListAsGrids().GetByLocalID("JobList_Validation");

      var row = bocList.GetRow(1);

      home.WebButtons().GetByLocalID("ValidationTestCaseCellButton").Click();

      var expectedValidationFailures = new []
                                       {
                                           new BocListValidationError(
                                               "Localized cell validation failure message",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               "DisplayName",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               "DisplayName")
                                       };

      Assert.That(row.GetCell().WithColumnTitle("DisplayName").GetValidationErrors(), Is.EqualTo(expectedValidationFailures));
    }

    [Test]
    public void GetValidationErrors_EditableRowWithEditModeValidationFailure ()
    {
      var home = Start();
      var bocList = home.ListAsGrids().GetByLocalID("JobList_Validation");

      var editableRow = bocList.GetRow(1);

      var dateTimeControl = editableRow.GetCell("StartDate").DateTimeValues().First();
      dateTimeControl.SetDate("");

      home.WebButtons().GetByLocalID("ValidateButton").Click();

      var expectedValidationErrors = new[]
                                     {
                                         new BocListValidationError(
                                             "StartDate: Enter a date.",
                                             "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                             "StartDate",
                                             "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                             "StartDate")
                                     };

      var validationErrors = editableRow.GetValidationErrors();
      Assert.That(validationErrors, Is.EqualTo(expectedValidationErrors));
    }

    private WxePageObject Start ()
    {
      return Start("BocListAsGrid");
    }

    private class DerivedBocListAsGridControlObject : BocListAsGridControlObject
    {
      public DerivedBocListAsGridControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedBocListAsGridControlObject<TBocListRowControlObject> : BocListAsGridControlObject<TBocListRowControlObject>
        where TBocListRowControlObject : BocListAsGridRowControlObject
    {
      public DerivedBocListAsGridControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedBocListAsGridRowControlObject : BocListAsGridRowControlObject
    {
      public DerivedBocListAsGridRowControlObject (IBocListRowControlObjectHostAccessor accessor, ControlObjectContext context)
          : base(accessor, context)
      {
      }
    }

    private class DerivedScreenshotBocList<TList, TRow, TCell> : ScreenshotBocList<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocList (
          IFluentScreenshotElementWithCovariance<TList> fluentList,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base(fluentList, fluentElement)
      {
      }
    }

    private class DerivedScreenshotBocListTableContainer<TList, TRow, TCell> : ScreenshotBocListTableContainer<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocListTableContainer (
          IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base(fluentList, fluentElement)
      {
      }
    }

    private class DerivedScreenshotBocListHeaderRow<TList, TRow, TCell> : ScreenshotBocListHeaderRow<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocListHeaderRow (
          IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base(fluentList, fluentElement)
      {
      }
    }

    private class DerivedScreenshotBocListRow<TList, TRow, TCell> : ScreenshotBocListRow<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocListRow (
          IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
          IFluentScreenshotElement<TRow> fluentRow)
          : base(fluentList, fluentRow)
      {
      }
    }

    private class DerivedScreenshotBocListColumn<TList, TRow, TCell> : ScreenshotBocListColumn<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocListColumn (
          IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
          int columnIndex,
          bool includeHeader)
          : base(fluentList, columnIndex, includeHeader)
      {
      }
    }

    private class DerivedScreenshotBocListNavigator<TList, TRow, TCell> : ScreenshotBocListNavigator<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocListNavigator (
          IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base(fluentList, fluentElement)
      {
      }
    }

    private class DerivedScreenshotBocListMenuBlock<TList, TRow, TCell> : ScreenshotBocListMenuBlock<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocListMenuBlock (
          IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base(fluentList, fluentElement)
      {
      }
    }

    private class DerivedScreenshotBocListDropDown<TList, TRow, TCell> : ScreenshotBocListDropDown<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocListDropDown (
          IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base(fluentList, fluentElement)
      {
      }
    }
  }
}
