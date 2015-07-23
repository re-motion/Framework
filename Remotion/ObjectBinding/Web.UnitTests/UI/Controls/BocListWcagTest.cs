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
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{

[TestFixture]
public class BocListWcagTest: BocTest
{
  private BocListMock _bocList;

  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocList = new BocListMock();
    _bocList.ID = "BocList";
    _bocList.ShowOptionsMenu = false;
    _bocList.ShowListMenu = false;
    _bocList.ShowAvailableViewsList = false;
    _bocList.PageSize = null;
    _bocList.EnableSorting = false;
    _bocList.RowMenuDisplay = RowMenuDisplay.Disabled;
    _bocList.Selection = RowSelection.Disabled;
    Page.Controls.Add (_bocList);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocList.PageSize = 1;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

	  Assert.That (WcagHelperMock.HasWarning, Is.False);
	  Assert.That (WcagHelperMock.HasError, Is.False);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.PageSize = 1;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

	  Assert.That (WcagHelperMock.HasWarning, Is.False);
	  Assert.That (WcagHelperMock.HasError, Is.False);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithShowOptionsMenuTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.ShowOptionsMenu = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

	  Assert.That (WcagHelperMock.HasError, Is.True);
	  Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
	  Assert.That (WcagHelperMock.Control, Is.SameAs (_bocList));
	  Assert.That (WcagHelperMock.Property, Is.EqualTo ("ShowOptionsMenu"));
  }

  [Test]
  public void IsOptionsMenuInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.ShowOptionsMenu = true;
    _bocList.OptionsMenuItems.Add (new WebMenuItem());
    Assert.That (_bocList.HasOptionsMenu, Is.False);
  }

  [Test]
  public void IsOptionsMenuVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.ShowOptionsMenu = true;
    _bocList.OptionsMenuItems.Add (new WebMenuItem());
    Assert.That (_bocList.HasOptionsMenu, Is.True);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithShowListMenuTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.ShowListMenu = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

	  Assert.That (WcagHelperMock.HasError, Is.True);
	  Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
	  Assert.That (WcagHelperMock.Control, Is.SameAs (_bocList));
	  Assert.That (WcagHelperMock.Property, Is.EqualTo ("ShowListMenu"));
  }

  [Test]
  public void IsListMenuInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.ShowListMenu = true;
    _bocList.ListMenuItems.Add (new WebMenuItem());
    Assert.That (_bocList.HasListMenu, Is.False);
  }

  [Test]
  public void IsListMenuVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.ShowListMenu = true;
    _bocList.ListMenuItems.Add (new WebMenuItem());
    Assert.That (_bocList.HasListMenu, Is.True);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithShowAvailableViewsListTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.ShowAvailableViewsList = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

	  Assert.That (WcagHelperMock.HasError, Is.True);
	  Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
	  Assert.That (WcagHelperMock.Control, Is.SameAs (_bocList));
	  Assert.That (WcagHelperMock.Property, Is.EqualTo ("ShowAvailableViewsList"));
  }

  [Test]
  public void IsAvailableViewsListInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.ShowAvailableViewsList = true;
    _bocList.AvailableViews.Add (new BocListView ());
    Assert.That (_bocList.HasAvailableViewsList, Is.False);
  }

  [Test]
  public void IsAvailableViewsListVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.ShowAvailableViewsList = true;
    _bocList.AvailableViews.Add (new BocListView ());
    _bocList.AvailableViews.Add (new BocListView ());
    Assert.That (_bocList.HasAvailableViewsList, Is.True);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithPageSizeNotNull()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.PageSize = 1;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

	  Assert.That (WcagHelperMock.HasError, Is.True);
	  Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
	  Assert.That (WcagHelperMock.Control, Is.SameAs (_bocList));
	  Assert.That (WcagHelperMock.Property, Is.EqualTo ("PageSize"));
  }

  [Test]
  public void IsPagingDisabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.PageSize = 1;
    Assert.That (_bocList.IsPagingEnabled, Is.False);
  }

  [Test]
  public void IsPagingEnabledWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.PageSize = 1;
    Assert.That (_bocList.IsPagingEnabled, Is.True);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithClientSideSortingEnabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.EnableSorting = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

	  Assert.That (WcagHelperMock.HasWarning, Is.True);
	  Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
	  Assert.That (WcagHelperMock.Control, Is.SameAs (_bocList));
	  Assert.That (WcagHelperMock.Property, Is.EqualTo ("EnableSorting"));
  }

  [Test]
  public void IsClientSideSortingEnabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.EnableSorting = true;
    Assert.That (_bocList.IsClientSideSortingEnabled, Is.True);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithRowMenuDisplayAutomatic()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.RowMenuDisplay = RowMenuDisplay.Automatic;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

	  Assert.That (WcagHelperMock.HasError, Is.True);
	  Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
	  Assert.That (WcagHelperMock.Control, Is.SameAs (_bocList));
	  Assert.That (WcagHelperMock.Property, Is.EqualTo ("RowMenuDisplay"));
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithDropDownMenuColumn()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocDropDownMenuColumnDefinition dropDownMenuColumn = new BocDropDownMenuColumnDefinition();
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {dropDownMenuColumn});

	  Assert.That (WcagHelperMock.HasError, Is.True);
	  Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
	  Assert.That (WcagHelperMock.Control, Is.SameAs (_bocList));
	  Assert.That (WcagHelperMock.Property, Is.EqualTo ("Columns[0]"));
  }

  [Test]
  public void EvaluateWaiConformityDebugLevelAWithRowEditModeColumn()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocRowEditModeColumnDefinition rowEditModeColumn = new BocRowEditModeColumnDefinition();
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {rowEditModeColumn});

    Assert.That (WcagHelperMock.HasError, Is.True);
    Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
    Assert.That (WcagHelperMock.Control, Is.SameAs (_bocList));
    Assert.That (WcagHelperMock.Property, Is.EqualTo ("Columns[0]"));
  }

  [Test]
  public void EvaluateWaiConformityDebugLevelAWithCommandColumnSetToEvent()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.Event;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {commandColumn});

    Assert.That (WcagHelperMock.HasError, Is.True);
    Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
    Assert.That (WcagHelperMock.Control, Is.SameAs (_bocList));
    Assert.That (WcagHelperMock.Property, Is.EqualTo ("Columns[0].Command"));
  }

  [Test]
  public void EvaluateWaiConformityDebugLevelAWithCommandColumnSetToWxeFunction()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.WxeFunction;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {commandColumn});

    Assert.That (WcagHelperMock.HasError, Is.True);
    Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
    Assert.That (WcagHelperMock.Control, Is.SameAs (_bocList));
    Assert.That (WcagHelperMock.Property, Is.EqualTo ("Columns[0].Command"));
  }

  [Test]
  public void EvaluateWaiConformityDebugLevelAWithCommandColumnSetToHref()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.Href;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {commandColumn});

    Assert.That (WcagHelperMock.HasWarning, Is.False);
    Assert.That (WcagHelperMock.HasError, Is.False);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithCommandColumnWithoutCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command = null;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {commandColumn});

	  Assert.That (WcagHelperMock.HasWarning, Is.False);
	  Assert.That (WcagHelperMock.HasError, Is.False);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelDoubleAWithSelectionEnabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
    _bocList.Selection = RowSelection.SingleRadioButton;
    _bocList.Index = RowIndex.Disabled;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

	  Assert.That (WcagHelperMock.HasError, Is.True);
	  Assert.That (WcagHelperMock.Priority, Is.EqualTo (2));
	  Assert.That (WcagHelperMock.Control, Is.SameAs (_bocList));
	  Assert.That (WcagHelperMock.Property, Is.EqualTo ("Selection"));
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelDoubleAWithSelectionDisabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
    _bocList.Selection = RowSelection.Disabled;
    _bocList.Index = RowIndex.Disabled;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

	  Assert.That (WcagHelperMock.HasWarning, Is.False);
	  Assert.That (WcagHelperMock.HasError, Is.False);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelDoubleAWithSelectionAndIndexEnabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
    _bocList.Selection = RowSelection.SingleRadioButton;
    _bocList.Index = RowIndex.InitialOrder;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

	  Assert.That (WcagHelperMock.HasWarning, Is.False);
	  Assert.That (WcagHelperMock.HasError, Is.False);
  }

}

}
