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
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure.Factories;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.ScreenshotCreation;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocListControlObjectTest : IntegrationTest
  {
    [TestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>), "GetTests")]
    [TestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>), "GetTests")]
    [TestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>), "GetTests")]
    [TestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>), "GetTests")]
    [TestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>), "GetTests")]
    [TestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>), "GetTests")]
    [TestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocListSelector, BocListControlObject>), "GetTests")]
    public void TestControlSelectors (TestCaseFactoryBase.TestSetupAction<BocListSelector, BocListControlObject> testAction)
    {
      testAction (Helper, e => e.Lists(), "list");
    }

    /// <summary>
    /// Annotates the three different container of an BocList: the MenuBlock on the right (red),
    /// the Navigator on the bottom left (blue) and the table container on the top left (green).
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest ()
    {
      var home = Start();

      var control = home.Lists().GetByLocalID ("JobList_Normal");
      var fluentControl = control.ForScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<ScreenshotBocList<BocListControlObject, BocListRowControlObject, BocListCellControlObject>>,
          BocListControlObjectTest> (
              fluentControl,
              ScreenshotTestingType.Both,
              (builder, target) =>
              {
                builder.AnnotateBox (target, Pens.Black, WebPadding.Inner);

                builder.AnnotateBox (target.GetMenuBlock(), Pens.Red, WebPadding.Inner);
                builder.AnnotateBox (target.GetNavigator(), Pens.Blue, WebPadding.Inner);
                builder.AnnotateBox (target.GetTableContainer(), Pens.Green, WebPadding.Inner);

                builder.Crop (target, new WebPadding (1));
              });
    }

    /// <summary>
    /// Tests that the navigator can be correctly annotated
    /// and that the fluent API works as intended.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest_Navigator ()
    {
      var home = Start();

      // List with a navigator
      var control = home.Lists().GetByLocalID ("JobList_Normal");
      var fluentControl = control.ForScreenshot();

      // List without a navigator
      var fluentControlWithoutNavigator = home.Lists().GetByLocalID ("JobList_WithRadioButtons").ForScreenshot();
      Assert.That (() => fluentControlWithoutNavigator.GetNavigator(), Throws.InvalidOperationException);

      Helper
          .RunScreenshotTestExact
          <FluentScreenshotElement<ScreenshotBocListNavigator<BocListControlObject, BocListRowControlObject, BocListCellControlObject>>,
              BocListControlObjectTest> (
                  fluentControl.GetNavigator(),
                  ScreenshotTestingType.Both,
                  (builder, target) =>
                  {
                    builder.AnnotateBox (target, Pens.Black, WebPadding.Inner);

                    builder.AnnotateBox (target.GetFirstPageButton(), Pens.Orange, WebPadding.Inner);
                    builder.AnnotateBox (target.GetPreviousPageButton(), Pens.Red, WebPadding.Inner);
                    builder.AnnotateBox (target.GetNextPageButton(), Pens.Violet, WebPadding.Inner);
                    builder.AnnotateBox (target.GetLastPageButton(), Pens.Aquamarine, WebPadding.Inner);

                    builder.AnnotateBox (target.GetPageInformationText(), Pens.Blue, WebPadding.Inner);
                    builder.AnnotateBox (target.GetPageNumberInput(), Pens.Green, WebPadding.Inner);

                    builder.Crop (target, new WebPadding (1));
                  });
    }

    /// <summary>
    /// Tests that the menu-block can be correctly annotated
    /// and that the fluent API works as intended.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest_MenuBlock ()
    {
      var home = Start();

      // List with a menu-block
      var control = home.Lists().GetByLocalID ("JobList_Normal");
      var fluentControl = control.ForScreenshot();

      // List without a menu-block
      var fluentControlWithoutMenuBlock = home.Lists().GetByLocalID ("JobList_Special").ForScreenshot();
      Assert.That (() => fluentControlWithoutMenuBlock.GetMenuBlock(), Throws.InvalidOperationException);

      Helper
          .RunScreenshotTestExact
          <FluentScreenshotElement<ScreenshotBocListMenuBlock<BocListControlObject, BocListRowControlObject, BocListCellControlObject>>,
              BocListControlObjectTest> (
                  fluentControl.GetMenuBlock(),
                  ScreenshotTestingType.Both,
                  (builder, target) =>
                  {
                    builder.AnnotateBox (target, Pens.Black, WebPadding.Inner);

                    builder.AnnotateBox (target.GetDropDownMenu(), Pens.Red, WebPadding.Inner);
                    builder.AnnotateBox (target.GetViewsMenu(), Pens.Green, WebPadding.Inner);

                    // The list menu is bigger than the surrounding div which can result in a cut off element
                    builder.MinimumVisibility = ElementVisibility.PartiallyVisible;
                    builder.AnnotateBox (target.GetListMenu(), Pens.Blue, WebPadding.Inner);
                    builder.MinimumVisibility = ElementVisibility.FullyVisible;

                    builder.Crop (target);
                  });
    }

    /// <summary>
    /// Tests that the table-container can be correctly annotated
    /// and that the fluent API works as intended.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest_TableContainer ()
    {
      var home = Start();

      var control = home.Lists().GetByLocalID ("JobList_Normal");
      var fluentControl = control.ForScreenshot();

      Helper
          .RunScreenshotTestExact
          <FluentScreenshotElement<ScreenshotBocListTableContainer<BocListControlObject, BocListRowControlObject, BocListCellControlObject>>,
              BocListControlObjectTest> (
                  fluentControl.GetTableContainer(),
                  ScreenshotTestingType.Both,
                  (builder, target) =>
                  {
                    builder.AnnotateBox (target, Pens.Black, WebPadding.Inner);

                    builder.AnnotateBox (target.GetHeaderRow(), Pens.Red, WebPadding.Inner);
                    builder.AnnotateBox (target.GetRow (2), Pens.Blue, WebPadding.Inner);
                    builder.AnnotateBox (target.GetColumn (2), Pens.Green, WebPadding.Inner);
                    builder.AnnotateBox (target.GetColumn (4, false), Pens.Violet, WebPadding.Inner);
                    builder.AnnotateBox (target.GetHeaderRow().GetCell (5), Pens.Orange, WebPadding.Inner);
                    builder.AnnotateBox (target.GetColumn (3).GetCell (1), Pens.Aquamarine, WebPadding.Inner);

                    builder.Crop (target);
                  });
    }

    [Test]
    public void TestSelectAfterClickOnSortColumnCheckBox ()
    {
      var home = Start();

      var bocListWithCheckboxes = home.Lists().GetByLocalID ("JobList_Normal");

      bocListWithCheckboxes.ClickOnSortColumn ("StartDate");
      bocListWithCheckboxes.ClickOnSortColumn ("Title");

      Assert.That (bocListWithCheckboxes.GetRow (2).GetCell (6).GetText(), Is.EqualTo ("Programmer"));
      Assert.That (() => bocListWithCheckboxes.GetRow (2).Select(), Throws.Nothing);
      Assert.That (bocListWithCheckboxes.GetRow (2).IsSelected, Is.True);
    }

    [Test]
    public void TestSelectAfterClickOnSortColumnRadioButton ()
    {
      var home = Start();

      var bocListWithRadiobuttons = home.Lists().GetByLocalID ("JobList_WithRadioButtons");

      bocListWithRadiobuttons.ClickOnSortColumn ("StartDate");
      bocListWithRadiobuttons.ClickOnSortColumn ("Title");

      Assert.That (bocListWithRadiobuttons.GetRow (2).GetCell (2).GetText(), Is.EqualTo ("Programmer"));
      Assert.That (() => bocListWithRadiobuttons.GetRow (2).Select(), Throws.Nothing);
      Assert.That (bocListWithRadiobuttons.GetRow (2).IsSelected, Is.True);
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      Assert.That (bocList.IsReadOnly(), Is.False);

      bocList = home.Lists().GetByLocalID ("JobList_ReadOnly");
      Assert.That (bocList.IsReadOnly(), Is.True);
    }

    [Test]
    public void TestGetColumnDefinitions ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      Assert.That (
          bocList.GetColumnDefinitions().Select (cd => cd.Title),
          Is.EquivalentTo (new[] { "I_ndex", null, null, "Command", "Menu", "Title", "StartDate", "EndDate", "DisplayName", "TitleWithCmd" }));
    }

    [Test]
    public void TestGetDisplayedRows ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var rows = bocList.GetDisplayedRows();
      Assert.That (rows.Count, Is.EqualTo (2));
      Assert.That (rows[1].GetCell ("Title").GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestGetNumberOfRows ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      Assert.That (bocList.GetNumberOfRows(), Is.EqualTo (2));
    }

    [Test]
    public void TestEmptyList ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Empty");
      Assert.That (bocList.GetNumberOfRows(), Is.EqualTo (0));
      Assert.That (bocList.IsEmpty(), Is.True);
      Assert.That (bocList.GetEmptyMessage(), Is.EqualTo ("A wonderful empty list."));
    }

    [Test]
    public void TestPaging ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      Assert.That (bocList.GetNumberOfPages(), Is.EqualTo (3));

      bocList.GoToNextPage();
      Assert.That (bocList.GetCurrentPage(), Is.EqualTo (2));

      bocList.GoToPreviousPage();
      Assert.That (bocList.GetCurrentPage(), Is.EqualTo (1));

      bocList.GoToLastPage();
      Assert.That (bocList.GetCurrentPage(), Is.EqualTo (3));
      Assert.That (bocList.GetNumberOfRows(), Is.EqualTo (1));

      bocList.GoToFirstPage();
      Assert.That (bocList.GetCurrentPage(), Is.EqualTo (1));

      bocList.GoToSpecificPage (3);
      Assert.That (bocList.GetCurrentPage(), Is.EqualTo (3));
    }

    [Test]
    public void TestSelectAllAndDeselectAll ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      var firstRow = bocList.GetRow (1);
      var lastRow = bocList.GetRow (bocList.GetNumberOfRows());
      Assert.That (firstRow.IsSelected, Is.False);
      Assert.That (lastRow.IsSelected, Is.False);

      bocList.SelectAll();

      Assert.That (firstRow.IsSelected, Is.True);
      Assert.That (lastRow.IsSelected, Is.True);

      bocList.SelectAll();

      Assert.That (firstRow.IsSelected, Is.True);
      Assert.That (lastRow.IsSelected, Is.True);

      bocList.DeselectAll();

      Assert.That (firstRow.IsSelected, Is.False);
      Assert.That (lastRow.IsSelected, Is.False);
    }

    [Test]
    public void TestGetRow ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      var row = bocList.GetRow ("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98");
      Assert.That (row.GetCell (6).GetText(), Is.EqualTo ("CEO"));

      row = bocList.GetRow (1);
      Assert.That (row.GetCell (6).GetText(), Is.EqualTo ("Programmer"));
    }

    [Test]
    public void TestGetRowWhere ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      var row = bocList.GetRowWhere ("Title", "CEO");
      Assert.That (row.GetCell (6).GetText(), Is.EqualTo ("CEO"));

      row = bocList.GetRowWhere().ColumnWithIndexContains (6, "CEO");
      Assert.That (row.GetCell (6).GetText(), Is.EqualTo ("CEO"));

      row = bocList.GetRowWhere().ColumnWithTitleContainsExactly ("Title", "CEO");
      Assert.That (row.GetCell (6).GetText(), Is.EqualTo ("CEO"));

      row = bocList.GetRowWhere().ColumnWithTitleContains ("Title", "EO");
      Assert.That (row.GetCell (6).GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestGetCellWhere ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      var cell = bocList.GetCellWhere ("Title", "CEO");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = bocList.GetCellWhere().ColumnWithIndexContains (6, "CEO");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = bocList.GetCellWhere().ColumnWithTitleContainsExactly ("Title", "CEO");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = bocList.GetCellWhere().ColumnWithTitleContains ("Title", "EO");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestGetCellWhereWithVariousColumnDefinitions ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Special");

      var cell = bocList.GetCellWhere ("DateRange", "01.01.2000 until 31.12.2004");
      Assert.That (cell.GetText(), Is.EqualTo ("01.01.2000 until 31.12.2004"));

      cell = bocList.GetCellWhere ("CustomCell", "Custom XXXX");
      Assert.That (cell.GetText(), Is.EqualTo ("Custom XXXX"));
    }

    [Test]
    public void TestBocListWithNoFakeTableHeader ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_NoFakeTableHeader");

      bocList.ClickOnSortColumn ("EndDate");
      Assert.That (bocList.GetRow (1).GetCell (1).GetText(), Is.EqualTo ("CEO"));

      bocList.ClickOnSortColumn ("EndDate");
      bocList.ClickOnSortColumn ("EndDate");
      Assert.That (bocList.GetRow (1).GetCell (1).GetText(), Is.EqualTo ("Programmer"));

      var row = bocList.GetRowWhere ("Title", "Developer");
      Assert.That (row.GetCell ("DisplayName").GetText(), Is.EqualTo ("Developer"));

      var columnTitles = bocList.GetColumnDefinitions().Select (cd => cd.Title);
      Assert.That (columnTitles, Is.EquivalentTo (new[] { "Title", "StartDate", "EndDate", "DisplayName" }));
    }

    [Test]
    public void TestClickOnSortColumn ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      bocList.ClickOnSortColumn ("StartDate");
      bocList.ClickOnSortColumn ("Title");
      Assert.That (bocList.GetRow (2).GetCell (6).GetText(), Is.EqualTo ("Programmer"));

      bocList.ClickOnSortColumn (6);
      Assert.That (bocList.GetRow (2).GetCell (6).GetText(), Is.EqualTo ("Clerk"));

      bocList.ClickOnSortColumnByTitle ("Title");
      bocList.ClickOnSortColumnByTitle ("StartDate");
      Assert.That (bocList.GetRow (2).GetCell (6).GetText(), Is.EqualTo ("Developer"));
    }

    [Test]
    public void TestChangeViewTo ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      bocList.ChangeViewToByLabel ("View 1");
      Assert.That (home.Scope.FindIdEndingWith ("SelectedViewLabel").Text, Is.EqualTo ("ViewCmd1"));

      bocList.ChangeViewTo (2);
      Assert.That (home.Scope.FindIdEndingWith ("SelectedViewLabel").Text, Is.EqualTo ("ViewCmd2"));

      bocList.ChangeViewTo ("ViewCmd1");
      Assert.That (home.Scope.FindIdEndingWith ("SelectedViewLabel").Text, Is.EqualTo ("ViewCmd1"));
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var dropDownMenu = bocList.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("-1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("ListMenuOrOptionsClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|Option command 2"));
    }

    [Test]
    public void TestGetListMenu ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var listMenu = bocList.GetListMenu();
      listMenu.SelectItem ("ListMenuCmd3");

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("-1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("ListMenuOrOptionsClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("ListMenuCmd3|LM cmd 3"));
    }

    [Test]
    public void TestRowGetCell ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);

      var cell = row.GetCell ("Title");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = row.GetCell (6);
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = row.GetCell().WithColumnTitle ("Title");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = row.GetCell().WithColumnTitleContains ("ithCm");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestRowSelectAndDeselectForCheckBox ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);

      row.Select();
      row.GetCell (4).ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndicesLabel").Text, Is.EqualTo ("1"));

      row.Select();
      row.GetCell (4).ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndicesLabel").Text, Is.EqualTo ("1"));

      row.Deselect();
      row.GetCell (4).ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndicesLabel").Text, Is.EqualTo ("NoneSelected"));
    }

    [Test]
    public void TestRowSelectAndDeselectForRadioButton ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_WithRadioButtons");
      var row = bocList.GetRow (2);

      row.Select();
      bocList.GetListMenu().SelectItem ("ListMenuCmd1");
      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndexForRadioButtonLabel").Text, Is.EqualTo ("1"));

      row.Select();
      bocList.GetListMenu().SelectItem ("ListMenuCmd1");
      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndexForRadioButtonLabel").Text, Is.EqualTo ("1"));

      row.Deselect();
      bocList.GetListMenu().SelectItem ("ListMenuCmd1");
      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndexForRadioButtonLabel").Text, Is.EqualTo ("NoneSelected"));
    }

    [Test]
    public void TestRowClickSelectCheckboxOnSpecificPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      bocList.GoToSpecificPage (3);
      bocList.GetRow (1).Select();
      bocList.GetRow (1).GetCell (4).ExecuteCommand(); // trigger postback

      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndicesLabel").Text, Is.EqualTo ("4"));
    }

    [Test]
    public void TestRowGetRowDropDownMenu ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);
      var dropDownMenu = row.GetDropDownMenu();
      dropDownMenu.SelectItem ("RowMenuItemCmd2");

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("RowContextMenuClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("RowMenuItemCmd2|Row menu 2"));
    }

    [Test]
    public void TestRowEdit ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);

      Assert.That (home.Scope.FindIdEndingWith ("EditModeLabel").Text, Is.EqualTo ("False"));

      row.Edit();
      Assert.That (home.Scope.FindIdEndingWith ("EditModeLabel").Text, Is.EqualTo ("True"));
    }

    [Test]
    public void TestRowHasClass ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      var row1 = bocList.GetRow (1);
      Assert.That (row1.StyleInfo.HasCssClass ("bocListDataRow"), Is.True);
      Assert.That (row1.StyleInfo.HasCssClass ("odd"), Is.True);
      Assert.That (row1.StyleInfo.HasCssClass ("oddDoesNotHaveThisClass"), Is.False);

      var row2 = bocList.GetRow (2);
      Assert.That (row2.StyleInfo.HasCssClass ("bocListDataRow"), Is.True);
      Assert.That (row2.StyleInfo.HasCssClass ("even"), Is.True);
      Assert.That (row2.StyleInfo.HasCssClass ("evenDoesNotHaveThisClass"), Is.False);
    }

    [Test]
    public void TestRowGetBackgroundColor ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      var row1 = bocList.GetRow (1);
      Assert.That (row1.StyleInfo.GetBackgroundColor(), Is.EqualTo (WebColor.Transparent)); // yep, style information is on the cells only!
    }

    [Test]
    public void TestRowGetTextColor ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var row = bocList.GetRow (1);
      Assert.That (row.StyleInfo.GetTextColor(), Is.EqualTo (WebColor.Black));
    }

    [Test]
    public void TestEditableRowSave ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var editableRow = bocList.GetRow (2).Edit();

      editableRow.Save();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("InLineEdit"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("Saved"));
    }

    [Test]
    public void TestEditableRowCancel ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var editableRow = bocList.GetRow (2).Edit();

      editableRow.Cancel();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("InLineEdit"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("Canceled"));
    }

    [Test]
    public void TestEditableRowGetCell ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var editableRow = bocList.GetRow (2).Edit();

      var cell = editableRow.GetCell ("Title");
      Assert.That (cell.TextValues().First().GetText(), Is.EqualTo ("CEO"));

      cell = editableRow.GetCell (6);
      Assert.That (cell.TextValues().First().GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestCellGetText ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var cell = bocList.GetRow (2).GetCell (9);

      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestCellPerformCommand ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var cell = bocList.GetRow (2).GetCell (4);

      cell.ExecuteCommand();

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CellCommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("RowCmd"));
    }

    [Test]
    public void TestCellHasClass ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var cell = bocList.GetRow (1).GetCell (1);

      Assert.That (cell.StyleInfo.HasCssClass ("bocListDataCell"), Is.True);
      Assert.That (cell.StyleInfo.HasCssClass ("doesNotHaveThisClass"), Is.False);
    }

    [Test]
    public void TestCellGetBackgroundColor ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      var cell1 = bocList.GetRow (1).GetCell (1);
      Assert.That (cell1.StyleInfo.GetBackgroundColor(), Is.EqualTo (WebColor.White));

      var cell2 = bocList.GetRow (2).GetCell (1);
      Assert.That (cell2.StyleInfo.GetBackgroundColor(), Is.EqualTo (WebColor.FromRgb (240, 240, 240)));
    }

    [Test]
    public void TestCellGetTextColor ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var cell = bocList.GetRow (1).GetCell (1);
      Assert.That (cell.StyleInfo.GetTextColor(), Is.EqualTo (WebColor.Black));
    }

    [Test]
    public void TestEditableCellGetControl ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");
      var editableRow = bocList.GetRow (2).Edit();
      var editableCell = editableRow.GetCell (6);

      var bocText = editableCell.TextValues().First();
      bocText.FillWith ("NewTitle");

      editableRow.Save();
      Assert.That (bocList.GetCellWhere ("Title", "NewTitle").GetText(), Is.EqualTo ("NewTitle"));
    }

    private WxePageObject Start ()
    {
      return Start ("BocList");
    }
  }
}