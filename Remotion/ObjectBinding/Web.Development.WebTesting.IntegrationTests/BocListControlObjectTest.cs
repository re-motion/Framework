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
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  [RequiresUserInterface]
  public class BocListControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(DisabledTestCaseFactory<BocListSelector, BocListControlObject>))]
    [TestCaseSource(typeof(ReadOnlyTestCaseFactory<BocListSelector, BocListControlObject>))]
    [TestCaseSource(typeof(LabelTestCaseFactory<BocListSelector, BocListControlObject>))]
    [TestCaseSource(typeof(ValidationErrorTestCaseFactory<BocListSelector, BocListControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocListSelector, BocListControlObject> testAction)
    {
      testAction(Helper, e => e.Lists(), "list");
    }

    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>))]
    [TestCaseSource(typeof(DomainPropertyControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>))]
    [TestCaseSource(typeof(DisplayNameControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocListSelector, BocListControlObject> testAction)
    {
      testAction(Helper, e => e.Lists(), "list");
    }

    /// <summary>
    /// Annotates the three different container of an BocList: the MenuBlock on the right (red),
    /// the Navigator on the bottom left (blue) and the table container on the top left (green).
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest ()
    {
      var home = Start();

      var control = home.Lists().GetByLocalID("JobList_Normal");
      var fluentControl = control.ForScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<ScreenshotBocList<BocListControlObject, BocListRowControlObject, BocListCellControlObject>>,
          BocListControlObjectTest>(
              fluentControl,
              ScreenshotTestingType.Both,
              (builder, target) =>
              {
                builder.AnnotateBox(target, Pens.Black, WebPadding.Inner);

                builder.AnnotateBox(target.GetMenuBlock(), Pens.Red, WebPadding.Inner);
                builder.AnnotateBox(target.GetNavigator(), Pens.Blue, WebPadding.Inner);
                builder.AnnotateBox(target.GetTableContainer(), Pens.Green, WebPadding.Inner);

                builder.Crop(target, new WebPadding(1));
              });
    }

    /// <summary>
    /// Tests that the navigator can be correctly annotated
    /// and that the fluent API works as intended.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_Navigator ()
    {
      var home = Start();

      // List with a navigator
      var control = home.Lists().GetByLocalID("JobList_Normal");
      var fluentControl = control.ForScreenshot();

      // List without a navigator
      var fluentControlWithoutNavigator = home.Lists().GetByLocalID("JobList_WithRadioButtons").ForScreenshot();
      Assert.That(() => fluentControlWithoutNavigator.GetNavigator(), Throws.InvalidOperationException);

      Helper
          .RunScreenshotTestExact
          <FluentScreenshotElement<ScreenshotBocListNavigator<BocListControlObject, BocListRowControlObject, BocListCellControlObject>>,
              BocListControlObjectTest>(
                  fluentControl.GetNavigator(),
                  ScreenshotTestingType.Both,
                  (builder, target) =>
                  {
                    builder.AnnotateBox(target, Pens.Black, WebPadding.Inner);

                    builder.AnnotateBox(target.GetFirstPageButton(), Pens.Orange, WebPadding.Inner);
                    builder.AnnotateBox(target.GetPreviousPageButton(), Pens.Red, WebPadding.Inner);
                    builder.AnnotateBox(target.GetNextPageButton(), Pens.Violet, WebPadding.Inner);
                    builder.AnnotateBox(target.GetLastPageButton(), Pens.Aquamarine, WebPadding.Inner);

                    builder.AnnotateBox(target.GetPageInformationText(), Pens.Blue, WebPadding.Inner);
                    builder.AnnotateBox(target.GetPageNumberInput(), Pens.Green, WebPadding.Inner);

                    builder.Crop(target, new WebPadding(1));
                  });
    }

    /// <summary>
    /// Tests that the menu-block can be correctly annotated
    /// and that the fluent API works as intended.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_MenuBlock ()
    {
      var home = Start();

      // List with a menu-block
      var control = home.Lists().GetByLocalID("JobList_Normal");
      var fluentControl = control.ForScreenshot();

      // List without a menu-block
      var fluentControlWithoutMenuBlock = home.Lists().GetByLocalID("JobList_Special").ForScreenshot();
      Assert.That(() => fluentControlWithoutMenuBlock.GetMenuBlock(), Throws.InvalidOperationException);

      Helper
          .RunScreenshotTestExact
          <FluentScreenshotElement<ScreenshotBocListMenuBlock<BocListControlObject, BocListRowControlObject, BocListCellControlObject>>,
              BocListControlObjectTest>(
                  fluentControl.GetMenuBlock(),
                  ScreenshotTestingType.Both,
                  (builder, target) =>
                  {
                    builder.AnnotateBox(target, Pens.Black, WebPadding.Inner);

                    builder.AnnotateBox(target.GetDropDownMenu(), Pens.Red, WebPadding.Inner);
                    builder.AnnotateBox(target.GetViewsMenu(), Pens.Green, WebPadding.Inner);

                    // The list menu is bigger than the surrounding div which can result in a cut off element
                    builder.MinimumVisibility = ElementVisibility.PartiallyVisible;
                    builder.AnnotateBox(target.GetListMenu(), Pens.Blue, WebPadding.Inner);
                    builder.MinimumVisibility = ElementVisibility.FullyVisible;

                    builder.Crop(target);
                  });
    }

    /// <summary>
    /// Tests that the table-container can be correctly annotated
    /// and that the fluent API works as intended.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_TableContainer ()
    {
      var home = Start();

      var control = home.Lists().GetByLocalID("JobList_Normal");
      var fluentControl = control.ForScreenshot();

      Helper
          .RunScreenshotTestExact
          <FluentScreenshotElement<ScreenshotBocListTableContainer<BocListControlObject, BocListRowControlObject, BocListCellControlObject>>,
              BocListControlObjectTest>(
                  fluentControl.GetTableContainer(),
                  ScreenshotTestingType.Both,
                  (builder, target) =>
                  {
                    builder.AnnotateBox(target, Pens.Black, WebPadding.Inner);

                    builder.AnnotateBox(target.GetHeaderRow(), Pens.Red, WebPadding.Inner);
                    builder.AnnotateBox(target.GetRow(2), Pens.Blue, WebPadding.Inner);
                    builder.AnnotateBox(target.GetColumn(2), Pens.Green, WebPadding.Inner);
                    builder.AnnotateBox(target.GetColumn(4, false), Pens.Violet, WebPadding.Inner);
                    builder.AnnotateBox(target.GetHeaderRow().GetCell(5), Pens.Orange, WebPadding.Inner);
                    builder.AnnotateBox(target.GetColumn(3).GetCell(1), Pens.Aquamarine, WebPadding.Inner);

                    builder.Crop(target);
                  });
    }

    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_DerivedType ()
    {
      var home = Start();
      var controlObjectContext = home.Lists().GetByLocalID("JobList_Normal").Context;
      var controlObject = new DerivedBocListControlObject(controlObjectContext);
      var fluentControlObject = controlObject.ForScreenshot();
      var derivedControlObject = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocList<BocListControlObject, BocListRowControlObject, BocListCellControlObject>(
              fluentControlObject.GetTarget().FluentList, fluentControlObject.GetTarget().FluentElement));

      var fluentTableContainer = derivedControlObject.GetTableContainer();
      Assert.That(fluentTableContainer, Is.Not.Null);
      var derivedTableContainer = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListTableContainer<BocListControlObject, BocListRowControlObject, BocListCellControlObject>(
              fluentTableContainer.GetTarget().FluentList, fluentTableContainer.GetTarget().FluentElement));
      var fluentHeaderRow = derivedTableContainer.GetHeaderRow();
      Assert.That(fluentHeaderRow, Is.Not.Null);
      var derivedHeaderRow = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListHeaderRow<BocListControlObject, BocListRowControlObject, BocListCellControlObject>(
              fluentHeaderRow.GetTarget().FluentList,
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
          new DerivedScreenshotBocListRow<BocListControlObject, BocListRowControlObject, BocListCellControlObject>(
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
          new DerivedScreenshotBocListColumn<BocListControlObject, BocListRowControlObject, BocListCellControlObject>(
              fluentColumn.GetTarget().FluentList,
              includeHeader : false,
              columnIndex : 1),
          minimumElementVisibility: ((IFluentScreenshotElement)fluentHeaderRow).MinimumElementVisibility);
      Assert.That(derivedColumn.GetCell(), Is.Not.Null);
      Assert.That(derivedColumn.GetCell(1), Is.Not.Null);
      Assert.That(derivedColumn.GetCell("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98"), Is.Not.Null);

      var fluentNavigator = derivedControlObject.GetNavigator();
      Assert.That(fluentNavigator, Is.Not.Null);
      var derivedNavigator = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListNavigator<BocListControlObject, BocListRowControlObject, BocListCellControlObject>(
              fluentNavigator.GetTarget().FluentList, fluentNavigator.GetTarget().FluentElement));
      Assert.That(derivedNavigator.GetFirstPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetLastPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetNextPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetPreviousPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetPageInformationText(), Is.Not.Null);
      Assert.That(derivedNavigator.GetPageNumberInput(), Is.Not.Null);

      var fluentMenuBlock = derivedControlObject.GetMenuBlock();
      Assert.That(fluentMenuBlock, Is.Not.Null);
      var derivedMenuBlock = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListMenuBlock<BocListControlObject, BocListRowControlObject, BocListCellControlObject>(
              fluentMenuBlock.GetTarget().FluentList, fluentMenuBlock.GetTarget().FluentElement));
      Assert.That(derivedMenuBlock.GetDropDownMenu(), Is.Not.Null);
      Assert.That(derivedMenuBlock.GetListMenu(), Is.Not.Null);

      var fluentDropDown = derivedMenuBlock.GetViewsMenu();
      Assert.That(fluentDropDown, Is.Not.Null);
      var derivedDropDown = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListDropDown<BocListControlObject, BocListRowControlObject, BocListCellControlObject>(
              fluentDropDown.GetTarget().FluentList, fluentDropDown.GetTarget().FluentElement));
      Assert.That(() => derivedDropDown.Open(), Throws.Nothing);
    }

    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_DerivedTypeGeneric ()
    {
      var home = Start();
      var controlObjectContext = home.Lists().GetByLocalID("JobList_Normal").Context;
      var controlObject = new DerivedBocListControlObject<DerivedBocListRowControlObject>(controlObjectContext);
      var fluentControlObject = controlObject.ForBocListScreenshot<DerivedBocListControlObject<DerivedBocListRowControlObject>, DerivedBocListRowControlObject>();
      var derivedControlObject = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocList<DerivedBocListControlObject<DerivedBocListRowControlObject>, DerivedBocListRowControlObject, BocListCellControlObject>(
              fluentControlObject.GetTarget().FluentList, fluentControlObject.GetTarget().FluentElement));

      var fluentTableContainer = derivedControlObject.GetTableContainer();
      Assert.That(fluentTableContainer, Is.Not.Null);
      var derivedTableContainer = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListTableContainer<DerivedBocListControlObject<DerivedBocListRowControlObject>, DerivedBocListRowControlObject, BocListCellControlObject>(
              fluentTableContainer.GetTarget().FluentList, fluentTableContainer.GetTarget().FluentElement));
      var fluentHeaderRow = derivedTableContainer.GetHeaderRow();
      Assert.That(fluentHeaderRow, Is.Not.Null);
      var derivedHeaderRow = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListHeaderRow<DerivedBocListControlObject<DerivedBocListRowControlObject>, DerivedBocListRowControlObject, BocListCellControlObject>(
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
          new DerivedScreenshotBocListRow<DerivedBocListControlObject<DerivedBocListRowControlObject>, DerivedBocListRowControlObject, BocListCellControlObject>(
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
          new DerivedScreenshotBocListColumn<DerivedBocListControlObject<DerivedBocListRowControlObject>, DerivedBocListRowControlObject, BocListCellControlObject>(
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
          new DerivedScreenshotBocListNavigator<DerivedBocListControlObject<DerivedBocListRowControlObject>, DerivedBocListRowControlObject, BocListCellControlObject>(
              fluentNavigator.GetTarget().FluentList, fluentNavigator.GetTarget().FluentElement));
      Assert.That(derivedNavigator.GetFirstPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetLastPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetNextPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetPreviousPageButton(), Is.Not.Null);
      Assert.That(derivedNavigator.GetPageInformationText(), Is.Not.Null);
      Assert.That(derivedNavigator.GetPageNumberInput(), Is.Not.Null);

      var fluentMenuBlock = derivedControlObject.GetMenuBlock();
      Assert.That(fluentMenuBlock, Is.Not.Null);
      var derivedMenuBlock = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListMenuBlock<DerivedBocListControlObject<DerivedBocListRowControlObject>, DerivedBocListRowControlObject, BocListCellControlObject>(
              fluentMenuBlock.GetTarget().FluentList, fluentMenuBlock.GetTarget().FluentElement));
      Assert.That(derivedMenuBlock.GetDropDownMenu(), Is.Not.Null);
      Assert.That(derivedMenuBlock.GetListMenu(), Is.Not.Null);

      var fluentDropDown = derivedMenuBlock.GetViewsMenu();
      Assert.That(fluentDropDown, Is.Not.Null);
      var derivedDropDown = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocListDropDown<DerivedBocListControlObject<DerivedBocListRowControlObject>, DerivedBocListRowControlObject, BocListCellControlObject>(
              fluentDropDown.GetTarget().FluentList, fluentDropDown.GetTarget().FluentElement));
      Assert.That(() => derivedDropDown.Open(), Throws.Nothing);
    }

    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_ValidationMarkerAndMessages ()
    {
      var home = Start();

      home.WebButtons().GetByLocalID("ValidationTestCaseCellButton").Click();

      var control = home.Lists().GetByLocalID("JobList_Validation");
      var fluentControl = control.ForScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<ScreenshotBocList<BocListControlObject, BocListRowControlObject, BocListCellControlObject>>,
          BocListControlObjectTest>(
              fluentControl,
              ScreenshotTestingType.Both,
              (builder, target) =>
              {
                var row = target.GetTableContainer().GetRow().WithIndex(1);

                builder.AnnotateBox(row.GetErrors(), Pens.Red, WebPadding.Inner);
                builder.AnnotateBox(row.GetCell(7), Pens.Yellow, WebPadding.Inner);
                builder.AnnotateBox(row.GetCell(7).GetErrorMarker(), Pens.Blue, WebPadding.Inner);
                builder.AnnotateBox(row.GetErrorMarker(), Pens.Green, WebPadding.Inner);
                builder.AnnotateBox(target.GetTableContainer().GetHeaderRow().GetErrorMarker(), Pens.Orange, WebPadding.Inner);

                builder.Crop(row, new WebPadding(0, 25, 0, 30), isRestrictedByParent: false);
              });
    }

    [Test]
    public void TestSelectAfterClickOnSortColumnCheckBox ()
    {
      var home = Start();

      var bocListWithCheckboxes = home.Lists().GetByLocalID("JobList_Normal");
      var completionDetection = new CompletionDetectionStrategyTestHelper(bocListWithCheckboxes);

      bocListWithCheckboxes.ClickOnSortColumn("StartDate");
      bocListWithCheckboxes.ClickOnSortColumn("Title");

      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(bocListWithCheckboxes.GetRow(2).GetCell(6).GetText(), Is.EqualTo("Programmer"));
      Assert.That(() => bocListWithCheckboxes.GetRow(2).Select(), Throws.Nothing);
      Assert.That(bocListWithCheckboxes.GetRow(2).IsSelected, Is.True);
    }

    [Test]
    public void TestSelectAfterClickOnSortColumnRadioButton ()
    {
      var home = Start();

      var bocListWithRadiobuttons = home.Lists().GetByLocalID("JobList_WithRadioButtons");
      var completionDetection = new CompletionDetectionStrategyTestHelper(bocListWithRadiobuttons);

      bocListWithRadiobuttons.ClickOnSortColumn("StartDate");
      bocListWithRadiobuttons.ClickOnSortColumn("Title");

      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(bocListWithRadiobuttons.GetRow(2).GetCell(2).GetText(), Is.EqualTo("Programmer"));
      Assert.That(() => bocListWithRadiobuttons.GetRow(2).Select(), Throws.Nothing);
      Assert.That(bocListWithRadiobuttons.GetRow(2).IsSelected, Is.True);
    }

    [Test]
    public void TestGetColumnDefinitions ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      Assert.That(
          bocList.GetColumnDefinitions().Select(cd => cd.Title),
          Is.EquivalentTo(new[] { "I_ndex", "", "", "Command", "Menu", "Title", "StartDate", "EndDate", "DisplayName", "TitleWithCmd" }));
    }

    [Test]
    public void TestGetColumnDefinitionsWithRowHeaders ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("ChildrenList_EmptyWithRowHeaders");
      Assert.That(
          bocList.GetColumnDefinitions().Select(cd => (cd.Title, cd.IsRowHeader)),
          Is.EquivalentTo(new[] { ("", false), ("Command", false), ("LastName", true), ("FirstName", true), ("Partner", false) }));
    }

    [Test]
    public void TestGetDisplayedRows ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var rows = bocList.GetDisplayedRows();
      Assert.That(rows.Count, Is.EqualTo(2));
      Assert.That(rows[1].GetCell("Title").GetText(), Is.EqualTo("CEO"));
    }

    [Test]
    public void TestGetCellViaPropertyPath ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var rows = bocList.GetDisplayedRows();
      var result = rows[1].GetCell().WithDomainPropertyPath("StartDate");

      Assert.That(result.GetText(), Is.EqualTo("01.01.2005"));
    }

    [Test]
    public void TestGetCellViaNestedPropertyPath ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("NestedPropertyPathIdentifier");
      var rows = bocList.GetDisplayedRows();
      var result = rows[0].GetCell().WithDomainPropertyPath("Partner.FirstName");

      Assert.That(result.GetText(), Is.EqualTo("A"));
    }

    [Test]
    public void TestGetEditableCellViaPropertyPath ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var editableRow = bocList.GetRow(2).Edit();
      var editableCell = editableRow.GetCell().WithDomainPropertyPath("StartDate");

      Assert.That(editableCell.DateTimeValues().First().GetDateTimeAsString(), Is.EqualTo("01.01.2005"));
    }

    [Test]
    public void TestGetCompoundCellViaPropertyPath ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Special");
      var rows = bocList.GetDisplayedRows();
      var cell = rows[0].GetCell().WithDomainPropertyPaths("StartDate", "EndDate");

      Assert.That(cell.GetText(), Is.EqualTo("01.01.2000 until 31.12.2004"));
    }

    [Test]
    public void TestGetNumberOfRows ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      Assert.That(bocList.GetNumberOfRows(), Is.EqualTo(2));
    }

    [Test]
    public void TestEmptyList ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Empty");
      Assert.That(bocList.GetNumberOfRows(), Is.EqualTo(0));
      Assert.That(bocList.IsEmpty(), Is.True);
      Assert.That(bocList.GetEmptyMessage(), Is.EqualTo("A wonderful empty list."));
    }

    [Test]
    public void TestPaging ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var completionDetection = new CompletionDetectionStrategyTestHelper(bocList);

      Assert.That(bocList.GetNumberOfPages(), Is.EqualTo(4));

      bocList.GoToNextPage();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(2));

      bocList.GoToPreviousPage();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(1));

      bocList.GoToLastPage();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(4));
      Assert.That(bocList.GetNumberOfRows(), Is.EqualTo(2));

      bocList.GoToFirstPage();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(1));

      bocList.GoToSpecificPage(3);
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(3));
    }

    [Test]
    public void TestSelectAllAndDeselectAll ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
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

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      var row = bocList.GetRow("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("CEO"));

      row = bocList.GetRow(1);
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("Programmer"));
    }

    [Test]
    public void TestGetRowWhere ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      var row = bocList.GetRowWhere("Title", "CEO");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("CEO"));

      row = bocList.GetRowWhere().ColumnWithItemIDContainsExactly("Title", "Programmer");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("Programmer"));

      row = bocList.GetRowWhere().ColumnWithItemIDContains("Title", "gra");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("Programmer"));

      row = bocList.GetRowWhere().ColumnWithIndexContainsExactly(6, "CEO");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("CEO"));

      row = bocList.GetRowWhere().ColumnWithIndexContains(6, "EO");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("CEO"));

      row = bocList.GetRowWhere().ColumnWithTitleContainsExactly("Title", "CEO");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("CEO"));

      row = bocList.GetRowWhere().ColumnWithTitleContains("Title", "EO");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("CEO"));
    }

    [Test]
    public void TestEditableGetRowWhere ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      var row = bocList.GetRowWhere("Title", "CEO");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("CEO"));

      row.Edit();

      row = bocList.GetRowWhere().ColumnWithIndexContainsExactly(6, "CEO");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("CEO"));

      row = bocList.GetRowWhere().ColumnWithIndexContains(6, "EO");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("CEO"));

      row = bocList.GetRowWhere().ColumnWithTitleContainsExactly("Title", "CEO");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("CEO"));

      row = bocList.GetRowWhere().ColumnWithTitleContains("Title", "EO");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("CEO"));

      bocList.GetRowWhere("Title", "Programmer").Edit();

      row = bocList.GetRowWhere().ColumnWithItemIDContainsExactly("Title", "Programmer");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("Programmer"));

      row = bocList.GetRowWhere().ColumnWithItemIDContains("Title", "gra");
      Assert.That(row.GetCell(6).GetText(), Is.EqualTo("Programmer"));
    }

    [Test]
    public void TestGetRowWhereContainsMultipleMatches_MatchesFirst ()
    {
      const string customFiveX = "Custom XXXXX";
      const string customFourX = "Custom XXXX";

      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Special");

      var row = bocList.GetRowWhere().ColumnWithItemIDContains("CustomCell", customFourX);
      Assert.That(row.GetCell(2).GetText(), Is.EqualTo(customFiveX));

      row = bocList.GetRowWhere().ColumnWithIndexContains(2, customFourX);
      Assert.That(row.GetCell(2).GetText(), Is.EqualTo(customFiveX));

      row = bocList.GetRowWhere().ColumnWithTitleContains("Custom cell", customFourX);
      Assert.That(row.GetCell(2).GetText(), Is.EqualTo(customFiveX));
    }

    [Test]
    public void TestGetRowMatchesExactly ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      // Set Timeout to Zero so we don't have to wait the full timeout for the exception
      bocList.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;

      Assert.That(
          () => bocList.GetRowWhere("Title", "EO"),
          Throws.Exception.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(
                          Driver,
                          "Unable to find css: .bocListTable .bocListTableBody .bocListDataRow .bocListDataCell[data-boclist-cell-index='7'] *[data-boclist-cell-contents='EO']")
                      .Message));
    }

    [Test]
    public void TestGetCellWhere ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      var cell = bocList.GetCellWhere("Title", "CEO");
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContainsExactly("Title", "Programmer");
      Assert.That(cell.GetText(), Is.EqualTo("Programmer"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContains("Title", "gra");
      Assert.That(cell.GetText(), Is.EqualTo("Programmer"));

      cell = bocList.GetCellWhere().ColumnWithIndexContainsExactly(6, "CEO");
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));

      cell = bocList.GetCellWhere().ColumnWithIndexContains(6, "gra");
      Assert.That(cell.GetText(), Is.EqualTo("Programmer"));

      cell = bocList.GetCellWhere().ColumnWithTitleContainsExactly("Title", "CEO");
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));

      cell = bocList.GetCellWhere().ColumnWithTitleContains("Title", "EO");
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));
    }

    [Test]
    public void TestEditableGetCellWhere ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      var cell = bocList.GetCellWhere("Title", "CEO");
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));

      bocList.GetRowWhere("Title", "CEO").Edit();

      cell = bocList.GetCellWhere().ColumnWithIndexContainsExactly(6, "CEO");
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));

      cell = bocList.GetCellWhere().ColumnWithTitleContainsExactly("Title", "CEO");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("CEO"));

      cell = bocList.GetCellWhere().ColumnWithTitleContains("Title", "EO");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("CEO"));

      bocList.GetRowWhere("Title", "Programmer").Edit();

      cell = bocList.GetCellWhere().ColumnWithItemIDContainsExactly("Title", "Programmer");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("Programmer"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContains("Title", "gra");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("Programmer"));

      cell = bocList.GetCellWhere().ColumnWithIndexContains(6, "gra");
      Assert.That(cell.GetText(), Is.EqualTo("Programmer"));
    }

    [Test]
    public void TestGetCellWhere_WithSingleQuote ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");

      var cell = bocList.GetCellWhere("Title", "With'SingleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContainsExactly("Title", "With'SingleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContains("Title", "ith'SingleQuot");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContainsExactly(1, "With'SingleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContains(1, "ith'SingleQuot");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContainsExactly("Title", "With'SingleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContains("Title", "ith'SingleQuot");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));
    }

    [Test]
    public void TestEditableGetCellWhere_WithSingleQuote ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");

      bocList.GetRow(5).Edit();

      var cell = bocList.GetCellWhere("Title", "With'SingleQuote");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContainsExactly("Title", "With'SingleQuote");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContains("Title", "ith'SingleQuot");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContainsExactly(1, "With'SingleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContains(1, "ith'SingleQuot");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContainsExactly("Title", "With'SingleQuote");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContains("Title", "ith'SingleQuot");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With'SingleQuote"));
    }

    [Test]
    public void TestGetCellWhere_WithDoubleQuote ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");

      var cell = bocList.GetCellWhere("Title", "With\"DoubleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With\"DoubleQuote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContainsExactly("Title", "With\"DoubleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With\"DoubleQuote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContains("Title", "ith\"DoubleQuot");
      Assert.That(cell.GetText(), Is.EqualTo("With\"DoubleQuote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContainsExactly(1, "With\"DoubleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With\"DoubleQuote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContains(1, "ith\"DoubleQuot");
      Assert.That(cell.GetText(), Is.EqualTo("With\"DoubleQuote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContainsExactly("Title", "With\"DoubleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With\"DoubleQuote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContains("Title", "ith\"DoubleQuot");
      Assert.That(cell.GetText(), Is.EqualTo("With\"DoubleQuote"));
    }

    [Test]
    public void TestEditableGetCellWhere_WithDoubleQuote ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");

      bocList.GetRow(8).Edit();

      var cell = bocList.GetCellWhere("Title", "With\"DoubleQuote");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With\"DoubleQuote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContainsExactly("Title", "With\"DoubleQuote");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With\"DoubleQuote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContains("Title", "ith\"DoubleQuot");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With\"DoubleQuote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContainsExactly(1, "With\"DoubleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With\"DoubleQuote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContains(1, "ith\"DoubleQuot");
      Assert.That(cell.GetText(), Is.EqualTo("With\"DoubleQuote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContainsExactly("Title", "With\"DoubleQuote");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With\"DoubleQuote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContains("Title", "ith\"DoubleQuot");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With\"DoubleQuote"));
    }

    [Test]
    public void TestGetCellWhere_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");

      var cell = bocList.GetCellWhere("Title", "With'SingleQuoteAndWith\"Double1Quote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContainsExactly("Title", "With'SingleQuoteAndWith\"Double1Quote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContains("Title", "ith'SingleQuoteAndWith\"Double1Quot");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContainsExactly(1, "With'SingleQuoteAndWith\"Double1Quote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContains(1, "ith'SingleQuoteAndWith\"Double1Quote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContainsExactly("Title", "With'SingleQuoteAndWith\"Double1Quote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContains("Title", "ith'SingleQuoteAndWith\"Double1Quot");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));
    }

    [Test]
    public void TestEditableGetCellWhere_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");

      bocList.GetRow(6).Edit();

      var cell = bocList.GetCellWhere("Title", "With'SingleQuoteAndWith\"Double1Quote");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContainsExactly("Title", "With'SingleQuoteAndWith\"Double1Quote");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContains("Title", "ith'SingleQuoteAndWith\"Double1Quot");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContainsExactly(1, "With'SingleQuoteAndWith\"Double1Quote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContains(1, "ith'SingleQuoteAndWith\"Double1Quote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContainsExactly("Title", "With'SingleQuoteAndWith\"Double1Quote");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContains("Title", "ith'SingleQuoteAndWith\"Double1Quot");
      Assert.That(cell.Scope.FindCss("input").Value, Is.EqualTo("With'SingleQuoteAndWith\"Double1Quote"));
    }

    [Test]
    public void TestGetCellWhere_WithoutDiagnosticMetadata ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      var cell = bocList.GetCellWhere("RowCmd", "Row command");
      Assert.That(cell.GetText(), Is.EqualTo("Row command"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContainsExactly("RowCmd", "Row command");
      Assert.That(cell.GetText(), Is.EqualTo("Row command"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContains("RowCmd", "ow comman");
      Assert.That(cell.GetText(), Is.EqualTo("Row command"));

      cell = bocList.GetCellWhere().ColumnWithIndexContainsExactly(4, "Row command");
      Assert.That(cell.GetText(), Is.EqualTo("Row command"));

      cell = bocList.GetCellWhere().ColumnWithIndexContains(4, "ow comman");
      Assert.That(cell.GetText(), Is.EqualTo("Row command"));

      cell = bocList.GetCellWhere().ColumnWithTitleContainsExactly("Command", "Row command");
      Assert.That(cell.GetText(), Is.EqualTo("Row command"));

      cell = bocList.GetCellWhere().ColumnWithTitleContains("Command", "ow comman");
      Assert.That(cell.GetText(), Is.EqualTo("Row command"));
    }

    [Test]
    public void TestGetCellWhere_WithoutDiagnosticMetadata_WithSingleQuote ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_ColumnsWithoutDiagnosticMetadata");

      var cell = bocList.GetCellWhere("EditRow", "With'SingleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContainsExactly("EditRow", "With'SingleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContains("EditRow", "ith'SingleQuot");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContainsExactly(1, "With'SingleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContains(1, "ith'SingleQuot");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContainsExactly("Edit", "With'SingleQuote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContains("Edit", "ith'SingleQuot");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuote"));
    }

    [Test]
    public void TestGetCellWhere_WithoutDiagnosticMetadata_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_ColumnsWithoutDiagnosticMetadata");

      var cell = bocList.GetCellWhere("RowCmd", "With'SingleQuoteAndDouble\"Quote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndDouble\"Quote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContainsExactly("RowCmd", "With'SingleQuoteAndDouble\"Quote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndDouble\"Quote"));

      cell = bocList.GetCellWhere().ColumnWithItemIDContains("RowCmd", "ith'SingleQuoteAndDouble\"Quote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndDouble\"Quote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContainsExactly(2, "With'SingleQuoteAndDouble\"Quote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndDouble\"Quote"));

      cell = bocList.GetCellWhere().ColumnWithIndexContains(2, "ith'SingleQuoteAndDouble\"Quot");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndDouble\"Quote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContainsExactly("Command", "With'SingleQuoteAndDouble\"Quote");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndDouble\"Quote"));

      cell = bocList.GetCellWhere().ColumnWithTitleContains("Command", "ith'SingleQuoteAndDouble\"Quot");
      Assert.That(cell.GetText(), Is.EqualTo("With'SingleQuoteAndDouble\"Quote"));
    }

    [Test]
    public void TestGetCellWhereContainsMultipleMatches_MatchesFirst ()
    {
      const string customFiveX = "Custom XXXXX";
      const string customFourX = "Custom XXXX";

      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Special");

      var cell = bocList.GetCellWhere().ColumnWithItemIDContains("CustomCell", customFourX);
      Assert.That(cell.GetText(), Is.EqualTo(customFiveX));

      cell = bocList.GetCellWhere().ColumnWithIndexContains(2, customFourX);
      Assert.That(cell.GetText(), Is.EqualTo(customFiveX));

      cell = bocList.GetCellWhere().ColumnWithTitleContains("Custom cell", customFourX);
      Assert.That(cell.GetText(), Is.EqualTo(customFiveX));
    }

    [Test]
    public void TestGetCellMatchesExactly ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      // Set Timeout to Zero so we don't have to wait the full timeout for the exception
      bocList.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;

      Assert.That(
          () => bocList.GetCellWhere("Title", "EO"),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(
                          Driver,
                          "Unable to find css: .bocListTable .bocListTableBody .bocListDataRow .bocListDataCell[data-boclist-cell-index='7'] *[data-boclist-cell-contents='EO']")
                      .Message));
    }

    [Test]
    public void TestGetCellWhereWithVariousColumnDefinitions ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Special");

      var cell = bocList.GetCellWhere("DateRange", "01.01.2000 until 31.12.2004");
      Assert.That(cell.GetText(), Is.EqualTo("01.01.2000 until 31.12.2004"));

      cell = bocList.GetCellWhere("CustomCell", "Custom XXXX");
      Assert.That(cell.GetText(), Is.EqualTo("Custom XXXX"));
    }

    [Test]
    public void TestBocListWithNoFakeTableHeader ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");

      bocList.ClickOnSortColumn("EndDate");
      Assert.That(bocList.GetRow(1).GetCell(2).GetText(), Is.EqualTo("CEO"));

      bocList.ClickOnSortColumn("EndDate");
      bocList.ClickOnSortColumn("EndDate");
      Assert.That(bocList.GetRow(1).GetCell(2).GetText(), Is.EqualTo("Programmer"));

      var row = bocList.GetRowWhere("Title", "Developer");
      Assert.That(row.GetCell("DisplayName").GetText(), Is.EqualTo("Developer"));

      var columnTitles = bocList.GetColumnDefinitions().Select(cd => cd.Title);
      Assert.That(columnTitles, Is.EquivalentTo(new[] { "Edit", "Title", "StartDate", "EndDate", "DisplayName" }));
    }

    [Test]
    public void TestClickOnSortColumn ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var completionDetection = new CompletionDetectionStrategyTestHelper(bocList);

      bocList.ClickOnSortColumn("StartDate");
      bocList.ClickOnSortColumn("Title");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(bocList.GetRow(2).GetCell(7).GetText(), Is.EqualTo("Programmer"));

      bocList.ClickOnSortColumn(7);
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(bocList.GetRow(2).GetCell(7).GetText(), Is.EqualTo("Clerk"));

      bocList.ClickOnSortColumnByTitle("Title");
      bocList.ClickOnSortColumnByTitle("StartDate");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(bocList.GetRow(2).GetCell(7).GetText(), Is.EqualTo("Developer"));
    }

    [Test]
    public void TestChangeViewTo ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var completionDetection = new CompletionDetectionStrategyTestHelper(bocList);

      bocList.ChangeViewToByLabel("View 1");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("SelectedViewLabel").Text, Is.EqualTo("ViewCmd1"));

      bocList.ChangeViewTo(2);
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("SelectedViewLabel").Text, Is.EqualTo("ViewCmd2"));

      bocList.ChangeViewTo("ViewCmd1");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("SelectedViewLabel").Text, Is.EqualTo("ViewCmd1"));
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var dropDownMenu = bocList.GetDropDownMenu();
      dropDownMenu.SelectItem("OptCmd2");

      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("JobList_Normal"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderRowLabel").Text, Is.EqualTo("-1"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("ListMenuOrOptionsClick"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("OptCmd2|Option command 2"));
    }

    [Test]
    public void TestDropDownItemDisabledWithRequiredSelection ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      var dropDownMenu = bocList.GetDropDownMenu();

      dropDownMenu.Open();
      var menuItem = GetDropDownMenuItem3();
      Assert.That(menuItem.IsDisabled, Is.True);

      var row1 = bocList.GetRow(1);
      row1.Select();
      menuItem = GetDropDownMenuItem3();

      Assert.That(menuItem.IsDisabled, Is.False);

      var row2 = bocList.GetRow(2);
      row2.Select();
      menuItem = GetDropDownMenuItem3();

      Assert.That(menuItem.IsDisabled, Is.True);

      ItemDefinition GetDropDownMenuItem3 () => dropDownMenu.GetItemDefinitions().Single(x => x.ItemID == "OptCmd3");
    }

    [Test]
    public void TestListItemDisabledWithRequiredSelection ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      var listMenu = bocList.GetListMenu();

      var menuItem = GetListMenuItem2();
      Assert.That(menuItem.IsDisabled, Is.True);

      var row1 = bocList.GetRow(1);
      row1.Select();
      menuItem = GetListMenuItem2();

      Assert.That(menuItem.IsDisabled, Is.False);

      var row2 = bocList.GetRow(2);
      row2.Select();
      menuItem = GetListMenuItem2();

      Assert.That(menuItem.IsDisabled, Is.True);

      ItemDefinition GetListMenuItem2 () => listMenu.GetItemDefinitions().Single(x => x.ItemID == "ListMenuCmd2");
    }

    [Test]
    public void TestGetListMenu ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
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

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var row = bocList.GetRow(2);

      var cell = row.GetCell("Title");
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));

      cell = row.GetCell(6);
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));

      cell = row.GetCell().WithColumnTitle("Title");
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));

      cell = row.GetCell().WithColumnTitleContains("ithCm");
      Assert.That(cell.GetText(), Is.EqualTo("CEO"));
    }

    [Test]
    public void TestRowSelectAndDeselectForCheckBox ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var row = bocList.GetRow(2);
      var completionDetection = new CompletionDetectionStrategyTestHelper(row);

      row.Select();
      row.GetCell(4).ExecuteCommand();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("SelectedIndicesLabel").Text, Is.EqualTo("1"));

      row.Select();
      row.GetCell(4).ExecuteCommand();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("SelectedIndicesLabel").Text, Is.EqualTo("1"));

      row.Deselect();
      row.GetCell(4).ExecuteCommand();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("SelectedIndicesLabel").Text, Is.EqualTo("NoneSelected"));
    }

    [Test]
    public void TestRowSelectForRadioButton ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_WithRadioButtons");
      var row = bocList.GetRow(2);
      var completionDetection = new CompletionDetectionStrategyTestHelper(row);

      row.Select();
      bocList.GetListMenu().SelectItem("ListMenuCmd1");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("SelectedIndexForRadioButtonLabel").Text, Is.EqualTo("1"));

      row.Select();
      bocList.GetListMenu().SelectItem("ListMenuCmd1");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("SelectedIndexForRadioButtonLabel").Text, Is.EqualTo("1"));
      Assert.That(row.IsSelected, Is.True);

      var otherRow = bocList.GetRow(1);

      otherRow.Select();
      bocList.GetListMenu().SelectItem("ListMenuCmd1");
      Assert.That(home.Scope.FindIdEndingWith("SelectedIndexForRadioButtonLabel").Text, Is.EqualTo("0"));
      Assert.That(otherRow.IsSelected, Is.True);

      Assert.That(row.IsSelected, Is.False);
    }

    [Test]
    public void TestRowDeselect_ForRadioButton ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_WithRadioButtons");
      var row = bocList.GetRow(2);

      Assert.That(
          () => row.Deselect(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(
                      Driver,
                      "Unable to de-select the row because the list uses radio buttons for row selection instead of checkboxes.").Message));
    }

    [Test]
    public void TestRowClickSelectCheckboxOnSpecificPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      bocList.GoToSpecificPage(3);
      bocList.GetRow(1).Select();
      bocList.GetRow(1).GetCell(4).ExecuteCommand(); // trigger postback

      Assert.That(home.Scope.FindIdEndingWith("SelectedIndicesLabel").Text, Is.EqualTo("4"));
    }

    [Test]
    public void TestRowGetRowDropDownMenu ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var row = bocList.GetRow(2);
      var dropDownMenu = row.GetDropDownMenu();
      dropDownMenu.SelectItem("RowMenuItemCmd2");

      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("JobList_Normal"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderRowLabel").Text, Is.EqualTo("1"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("RowContextMenuClick"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("RowMenuItemCmd2|Row menu 2"));
    }

    [Test]
    public void TestRowEdit ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var row = bocList.GetRow(2);

      Assert.That(home.Scope.FindIdEndingWith("EditModeLabel").Text, Is.EqualTo("False"));

      row.Edit();
      Assert.That(home.Scope.FindIdEndingWith("EditModeLabel").Text, Is.EqualTo("True"));
    }

    [Test]
    public void TestReadOnly_RowEditShouldThrow ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_ReadOnly");
      Assert.That(bocList.IsReadOnly(), Is.True);

      var row = bocList.GetRow(2);
      Assert.That(
          () => row.Edit(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(
                      Driver,
                      "The control is currently in a read-only state. Therefore, the operation is not possible.").Message));
    }

    [Test]
    public void TestRowHasClass ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      var row1 = bocList.GetRow(1);
      Assert.That(row1.StyleInfo.HasCssClass("bocListDataRow"), Is.True);
      Assert.That(row1.StyleInfo.HasCssClass("odd"), Is.True);
      Assert.That(row1.StyleInfo.HasCssClass("oddDoesNotHaveThisClass"), Is.False);

      var row2 = bocList.GetRow(2);
      Assert.That(row2.StyleInfo.HasCssClass("bocListDataRow"), Is.True);
      Assert.That(row2.StyleInfo.HasCssClass("even"), Is.True);
      Assert.That(row2.StyleInfo.HasCssClass("evenDoesNotHaveThisClass"), Is.False);
    }

    [Test]
    public void TestRowGetBackgroundColor ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      var row1 = bocList.GetRow(1);
      Assert.That(row1.StyleInfo.GetBackgroundColor(), Is.EqualTo(WebColor.Transparent)); // yep, style information is on the cells only!
    }

    [Test]
    public void TestRowGetTextColor ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var row = bocList.GetRow(1);
      Assert.That(row.StyleInfo.GetTextColor(), Is.EqualTo(WebColor.Black));
    }

    [Test]
    public void TestEditableRowSave ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var editableRow = bocList.GetRow(2).Edit();

      editableRow.Save();
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("JobList_Normal"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderRowLabel").Text, Is.EqualTo("1"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("InLineEdit"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("Saved"));
    }

    [Test]
    public void TestEditableRowCancel ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var editableRow = bocList.GetRow(2).Edit();

      editableRow.Cancel();
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("JobList_Normal"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderRowLabel").Text, Is.EqualTo("1"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("InLineEdit"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("Canceled"));
    }

    [Test]
    public void TestEditableRowGetCell ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var editableRow = bocList.GetRow(2).Edit();

      var cell = editableRow.GetCell("Title");
      Assert.That(cell.TextValues().First().GetText(), Is.EqualTo("CEO"));

      cell = editableRow.GetCell(7);
      Assert.That(cell.TextValues().First().GetText(), Is.EqualTo("CEO"));
    }

    [Test]
    public void TestCellGetText ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var cell = bocList.GetRow(2).GetCell(6);

      Assert.That(cell.GetText(), Is.EqualTo("CEO"));
    }

    [Test]
    public void TestCellPerformCommand ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var cell = bocList.GetRow(2).GetCell(4);

      cell.ExecuteCommand();

      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("JobList_Normal"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderRowLabel").Text, Is.EqualTo("1"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("CellCommandClick"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("RowCmd"));
    }

    [Test]
    public void TestCellHasClass ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var cell = bocList.GetRow(1).GetCell(1);

      Assert.That(cell.StyleInfo.HasCssClass("bocListDataCell"), Is.True);
      Assert.That(cell.StyleInfo.HasCssClass("doesNotHaveThisClass"), Is.False);
    }

    [Test]
    public void TestCellGetBackgroundColor ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      var cell1 = bocList.GetRow(1).GetCell(1);
      Assert.That(cell1.StyleInfo.GetBackgroundColor(), Is.EqualTo(WebColor.White));

      var cell2 = bocList.GetRow(2).GetCell(1);
      Assert.That(cell2.StyleInfo.GetBackgroundColor(), Is.EqualTo(WebColor.FromRgb(240, 240, 240)));
    }

    [Test]
    public void TestCellGetTextColor ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var cell = bocList.GetRow(1).GetCell(1);
      Assert.That(cell.StyleInfo.GetTextColor(), Is.EqualTo(WebColor.Black));
    }

    [Test]
    public void TestEditableCellGetControl ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var editableRow = bocList.GetRow(2).Edit();
      var editableCell = editableRow.GetCell(7);

      var bocText = editableCell.TextValues().First();
      bocText.FillWith("NewTitle");

      editableRow.Save();
      Assert.That(bocList.GetCellWhere("Title", "NewTitle").GetText(), Is.EqualTo("NewTitle"));
    }

    [Test]
    public void TestGetCurrentPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(1));
    }

    [Test]
    public void TestGetCurrentPage_WithoutNavigator ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");
      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(1));
    }

    [Test]
    public void TestGetCurrentPage_InEditMode ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      //Enter edit mode
      bocList.GetRow(2).Edit();

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(1));
    }

    [Test]
    public void TestGetNumberOfPages ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      Assert.That(bocList.GetNumberOfPages(), Is.EqualTo(4));
    }

    [Test]
    public void TestGetNumberOfPages_WithoutNavigator ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");
      Assert.That(bocList.GetNumberOfPages(), Is.EqualTo(1));
    }

    [Test]
    public void TestGetNumberOfPages_InEditMode ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      //Enter edit mode
      bocList.GetRow(2).Edit();

      Assert.That(bocList.GetNumberOfPages(), Is.EqualTo(4));
    }


    [Test]
    public void TestGoToSpecificPage_FirstPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      //Navigate to next Page to be able to Test GoToSpecificPage (1)
      bocList.GoToNextPage();

      bocList.GoToSpecificPage(1);

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(1));
    }

    [Test]
    public void TestGoToSpecificPage_LastPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      bocList.GoToSpecificPage(3);

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(3));
    }

    [Test]
    public void TestGoToSpecificPage_WithoutNavigator ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");
      Assert.That(
          () => bocList.GoToSpecificPage(1),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List only has one page.").Message));
    }

    [Test]
    public void TestGoToSpecificPage_InEditMode ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      //Enter edit mode
      bocList.GetRow(2).Edit();

      Assert.That(
          () => bocList.GoToSpecificPage(3),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List is currently in edit mode.").Message));
    }

    [Test]
    public void TestGoToSpecificPage_CurrentPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      Assert.That(
          () => bocList.GoToSpecificPage(1),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "List is already on page '1'.").Message));
    }

    [Test]
    public void TestGoToSpecificPage_PageNumberGreaterThanNumberofPages ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var pageNumberGreaterThanNumberOfPages = 5000;


      Assert.That(
          () => bocList.GoToSpecificPage(pageNumberGreaterThanNumberOfPages),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(
                      Driver,
                      $"Unable to change page number to '{pageNumberGreaterThanNumberOfPages}'. Page number must be between '1' and '4'.").Message));
    }

    [Test]
    public void TestGoToSpecificPage_PageNumberLesserThanNumberofPages ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      var pageNumberLesserThanNumberOfPages = 0;


      Assert.That(
          () => bocList.GoToSpecificPage(pageNumberLesserThanNumberOfPages),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(
                      Driver,
                      $"Unable to change page number to '{pageNumberLesserThanNumberOfPages}'. Page number must be between '1' and '4'.").Message));
    }

    [Test]
    public void TestGoToFirstPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      bocList.GoToSpecificPage(2);

      bocList.GoToFirstPage();

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(1));
    }

    [Test]
    public void TestGoToFirstPage_WithoutNavigator ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");
      Assert.That(
          () => bocList.GoToFirstPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List only has one page.").Message));
    }

    [Test]
    public void TestGoToFirstPage_InEditMode ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      //Enter edit mode
      bocList.GetRow(2).Edit();

      Assert.That(
          () => bocList.GoToFirstPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List is currently in edit mode.").Message));
    }

    [Test]
    public void TestGoToFirstPage_OnFirstPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      Assert.That(
          () => bocList.GoToFirstPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(
                      Driver,
                      "Unable to change page number to the first page, as the list is already on the first page.").Message));
    }

    [Test]
    public void TestGoToPreviousPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      bocList.GoToSpecificPage(2);

      bocList.GoToPreviousPage();

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(1));
    }

    [Test]
    public void TestGoToPreviousPage_WithoutNavigator ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");
      Assert.That(
          () => bocList.GoToPreviousPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List only has one page.").Message));
    }

    [Test]
    public void TestGoToPreviousPage_InEditMode ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      //Enter edit mode
      bocList.GetRow(2).Edit();

      Assert.That(
          () => bocList.GoToPreviousPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List is currently in edit mode.").Message));
    }

    [Test]
    public void TestGoToPreviousPage_OnFirstPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      Assert.That(
          () => bocList.GoToPreviousPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change page number to the previous page, as the list is already on the first page.")
                      .Message));
    }

    [Test]
    public void TestGoToNextPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      bocList.GoToNextPage();

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(2));
    }

    [Test]
    public void TestGoToNextPage_WithoutNavigator ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");
      Assert.That(
          () => bocList.GoToNextPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List only has one page.").Message));
    }

    [Test]
    public void TestGoToNextPage_InEditMode ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      //Enter edit mode
      bocList.GetRow(2).Edit();

      Assert.That(
          () => bocList.GoToNextPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List is currently in edit mode.").Message));
    }

    [Test]
    public void TestGoToNextPage_OnLastPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      bocList.GoToLastPage();

      Assert.That(
          () => bocList.GoToNextPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change page number to the next page, as the list is already on the last page.").Message));
    }

    [Test]
    public void TestGoToLastPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      bocList.GoToLastPage();

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(4));
    }

    [Test]
    public void TestGoToLastPage_WithoutNavigator ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_NoFakeTableHeader");
      Assert.That(
          () => bocList.GoToLastPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List only has one page.").Message));
    }

    [Test]
    public void TestGoToLastPage_InEditMode ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      //Enter edit mode
      bocList.GetRow(2).Edit();

      Assert.That(
          () => bocList.GoToLastPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change current page of the list. List is currently in edit mode.").Message));
    }

    [Test]
    public void TestGoToLastPage_OnLastPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");
      bocList.GoToSpecificPage(4);

      Assert.That(
          () => bocList.GoToLastPage(),
          Throws.Exception.TypeOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to change page number to the last page, as the list is already on the last page.").Message));
    }

    [Test]
    public void GetColumnDefinitions_ShouldAlwaysFetchTheColumnsAnew ()
    {
      var home = Start();
      var bocList = home.Lists().GetByLocalID("JobList_Empty_VariableColumns");

      var view1Columns = bocList.GetColumnDefinitions();
      bocList.ChangeViewTo(2);
      var view2Columns = bocList.GetColumnDefinitions();

      Assert.That(view1Columns.Count, Is.EqualTo(2));
      Assert.That(view2Columns.Count, Is.EqualTo(3));
    }

    [Test]
    public void GetValidationErrors_NoValidationFailures ()
    {
      var home = Start();
      var bocList = home.Lists().GetByLocalID("JobList_Validation");

      Assert.That(bocList.GetRow(1).GetValidationErrors(), Is.Empty);
    }

    [Test]
    public void GetValidationErrors_RowValidationFailure ()
    {
      var home = Start();
      var bocList = home.Lists().GetByLocalID("JobList_Validation");

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
      var bocList = home.Lists().GetByLocalID("JobList_Validation");

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
      var bocList = home.Lists().GetByLocalID("JobList_Validation");

      home.WebButtons().GetByLocalID("ValidationTestCaseCellButton").Click();

      var expectedValidationErrors = new []
                                       {
                                           new BocListValidationError(
                                               "Localized cell validation failure message",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               "DisplayName",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               "DisplayName")
                                       };

      var bocListValidationErrors = bocList.GetRow(1).GetCell().WithColumnTitle("DisplayName").GetValidationErrors();
      Assert.That(bocListValidationErrors, Is.EqualTo(expectedValidationErrors));
    }

    [Test]
    public void GetValidationError_EditableRowAndNoValidationFailures ()
    {
      var home = Start();
      var bocList = home.Lists().GetByLocalID("JobList_Validation");

      var row = bocList.GetRow(1);
      var editableRow = row.Edit();

      Assert.That(editableRow.GetValidationErrors(), Is.Empty);
    }

    [Test]
    public void GetValidationErrors_EditableRowAndRowValidationFailure ()
    {
      var home = Start();
      var bocList = home.Lists().GetByLocalID("JobList_Validation");

      var row = bocList.GetRow(1);
      var editableRow = row.Edit();

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

      Assert.That(editableRow.GetValidationErrors(), Is.EqualTo(expectedValidationFailures));
    }

    [Test]
    public void GetValidationErrors_EditableRowAndCellValidationFailureOnRow ()
    {
      var home = Start();
      var bocList = home.Lists().GetByLocalID("JobList_Validation");

      var row = bocList.GetRow(1);
      var editableRow = row.Edit();

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

      Assert.That(editableRow.GetValidationErrors(), Is.EqualTo(expectedValidationFailures));
    }

    [Test]
    public void GetValidationErrors_EditableRowAndCellValidationFailure ()
    {
      var home = Start();
      var bocList = home.Lists().GetByLocalID("JobList_Validation");

      var row = bocList.GetRow(1);
      var editableRow = row.Edit();

      home.WebButtons().GetByLocalID("ValidationTestCaseCellButton").Click();

      var expectedValidationErrors = new []
                                       {
                                           new BocListValidationError(
                                               "Localized cell validation failure message",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               "DisplayName",
                                               "89dc8cd2-30e0-4bb3-92a0-4587f32492f5",
                                               "DisplayName")
                                       };

      var bocListValidationErrors = editableRow.GetCell().WithColumnTitle("DisplayName").GetValidationErrors();
      Assert.That(bocListValidationErrors, Is.EqualTo(expectedValidationErrors));
    }

    [Test]
    public void GetValidationErrors_EditableRowWithEditModeValidationFailure ()
    {
      var home = Start();
      var bocList = home.Lists().GetByLocalID("JobList_Validation");

      var row = bocList.GetRow(1);
      var editableRow = row.Edit();

      var dateTimeControl = editableRow.GetCell("StartDate").DateTimeValues().First();
      dateTimeControl.SetDate("");

      editableRow.Save();

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
      return Start("BocList");
    }

    private class DerivedBocListControlObject : BocListControlObject
    {
      public DerivedBocListControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedBocListControlObject<TBocListRowControlObject> : BocListControlObject<TBocListRowControlObject>
        where TBocListRowControlObject : BocListRowControlObject
    {
      public DerivedBocListControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedBocListRowControlObject : BocListRowControlObject
    {
      public DerivedBocListRowControlObject (IBocListRowControlObjectHostAccessor accessor, ControlObjectContext context)
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
