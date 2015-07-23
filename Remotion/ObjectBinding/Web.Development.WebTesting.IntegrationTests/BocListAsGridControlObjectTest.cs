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
using Coypu;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocListAsGridControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByID ("body_DataEditControl_JobList_Normal");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByIndex (1);
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().First();
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetListAsGrid().Single();
        Assert.Fail ("Should not be able to unambigously find a BOC list.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByDisplayName ("Jobs");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByDomainProperty ("Jobs");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByDomainProperty ("Jobs", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      Assert.That (bocList.IsReadOnly(), Is.False);
    }

    [Test]
    public void TestGetColumnDefinitions ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      Assert.That (
          bocList.GetColumnDefinitions().Select(cd => cd.Title),
          Is.EquivalentTo (new[] { "I_ndex", null, "Command", "Menu", "Title", "StartDate", "EndDate", "DisplayName", "TitleWithCmd" }));
    }

    [Test]
    public void TestGetDisplayedRows ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var rows = bocList.GetDisplayedRows();
      Assert.That (rows.Count, Is.EqualTo (5));
      Assert.That (rows[1].GetCell ("DisplayName").GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestGetNumberOfRows ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      Assert.That (bocList.GetNumberOfRows(), Is.EqualTo (5));
    }

    [Test]
    public void TestEmptyList ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Empty");
      Assert.That (bocList.GetNumberOfRows(), Is.EqualTo (0));
      Assert.That (bocList.IsEmpty(), Is.True);
      Assert.That (bocList.GetEmptyMessage(), Is.EqualTo ("A wonderful empty list."));
    }

    [Test]
    public void TestSelectAllAndDeselectAll ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      
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

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");

      var row = bocList.GetRow ("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98");
      Assert.That (row.GetCell (8).GetText(), Is.EqualTo ("CEO"));

      row = bocList.GetRow (1);
      Assert.That (row.GetCell (8).GetText(), Is.EqualTo ("Programmer"));
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
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

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
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

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);

      var cell = row.GetCell ("DisplayName");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = row.GetCell (8);
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = row.GetCell().WithColumnTitle ("DisplayName");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = row.GetCell().WithColumnTitleContains ("layNam");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestRowSelectAndDeselect ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);

      row.Select();
      row.GetCell (3).ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndicesLabel").Text, Is.EqualTo ("1"));

      row.Select();
      row.GetCell (3).ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndicesLabel").Text, Is.EqualTo ("1"));

      row.Deselect();
      row.GetCell (3).ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndicesLabel").Text, Is.EqualTo ("NoneSelected"));
    }

    [Test]
    public void TestRowGetRowDropDownMenu ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);
      var dropDownMenu = row.GetDropDownMenu();
      dropDownMenu.SelectItem ("RowMenuItemCmd2");

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("RowContextMenuClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("RowMenuItemCmd2|Row menu 2"));
    }

    [Test]
    public void TestCellGetText ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var cell = bocList.GetRow (2).GetCell (8);

      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestCellPerformCommand ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var cell = bocList.GetRow (2).GetCell (3);

      cell.ExecuteCommand();

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CellCommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("RowCmd"));
    }

    [Test]
    public void TestEditableCellGetControl ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var editableRow = bocList.GetRow (2);
      var editableCell = editableRow.GetCell (5);

      var bocText = editableCell.GetTextValue().First();
      bocText.FillWith ("NewTitle");

      Assert.That (bocText.GetText(), Is.EqualTo ("NewTitle"));
    }

    private WxePageObject Start ()
    {
      return Start ("BocListAsGrid");
    }
  }
}