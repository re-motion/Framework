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
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{

[TestFixture]
public class WebTabCollectionTest: WebControlTest
{
  private WebTabStrip _tabStrip;
  private WebTab _tab0;
  private WebTab _tab1;
  private WebTab _tab2;
  private WebTab _tab3;
  private WebTab _tabNew;

  public WebTabCollectionTest ()
  {
  }

  protected override void SetUpPage ()
  {
    base.SetUpPage();
    _tabStrip = new WebTabStrip();
    _tab0 = new WebTab("Tab0", WebString.CreateFromText("Tab 0"));
    _tab1 = new WebTab("Tab1", WebString.CreateFromText("Tab 1"));
    _tab2 = new WebTab("Tab2", WebString.CreateFromText("Tab 2"));
    _tab3 = new WebTab("Tab3", WebString.CreateFromText("Tab 3"));
    _tabNew = new WebTab("Tab5", WebString.CreateFromText("Tab 5"));
  }

  [Test]
  public void AddTabs ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tab1.IsSelected = true;
    _tabStrip.Tabs.Add(_tab3);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(4));
    Assert.That(_tabStrip.SelectedTab, Is.Not.Null);
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab1));
  }

  [Test]
  public void InsertFirstTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab2.IsSelected = true;

    _tabStrip.Tabs.Insert(0, _tabNew);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(5));
    Assert.That(_tabStrip.Tabs[0], Is.SameAs(_tabNew), "Wrong tab at position 0.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }

  [Test]
  public void InsertMiddleTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab2.IsSelected = true;

    _tabStrip.Tabs.Insert(2, _tabNew);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(5));
    Assert.That(_tabStrip.Tabs[2], Is.SameAs(_tabNew), "Wrong tab at position 2.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }

  [Test]
  public void InsertLastTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab2.IsSelected = true;

    _tabStrip.Tabs.Insert(4, _tabNew);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(5));
    Assert.That(_tabStrip.Tabs[4], Is.SameAs(_tabNew), "Wrong tab at position 4.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }

  [Test]
  public void ReplaceFirstTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab2.IsSelected = true;

    _tabStrip.Tabs[0] = _tabNew;

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(4));
    Assert.That(_tabStrip.Tabs[0], Is.SameAs(_tabNew), "Wrong tab at position 0.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }

  [Test]
  public void ReplaceMiddleTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab3.IsSelected = true;

    _tabStrip.Tabs[1] = _tabNew;

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(4));
    Assert.That(_tabStrip.Tabs[1], Is.SameAs(_tabNew), "Wrong tab at position 1.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab3), "Wrong tab selected.");
  }

  [Test]
  public void ReplaceLastTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab0.IsSelected = true;

    _tabStrip.Tabs[3] = _tabNew;

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(4));
    Assert.That(_tabStrip.Tabs[3], Is.SameAs(_tabNew), "Wrong tab at position 3.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab0), "Wrong tab selected.");
  }

  [Test]
  public void ReplaceFirstTabWithTabBeingSelected ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab0.IsSelected = true;

    _tabStrip.Tabs[0] = _tabNew;

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(4));
    Assert.That(_tabStrip.Tabs[0], Is.SameAs(_tabNew), "Wrong tab at position 0.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab1), "Wrong tab selected.");
  }

  [Test]
  public void ReplaceMiddleTabWithTabBeingSelected ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab1.IsSelected = true;

    _tabStrip.Tabs[1] = _tabNew;

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(4));
    Assert.That(_tabStrip.Tabs[1], Is.SameAs(_tabNew), "Wrong tab at position 1.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }

  [Test]
  public void ReplaceLastTabWithTabBeingSelected ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab3.IsSelected = true;

    _tabStrip.Tabs[3] = _tabNew;

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(4));
    Assert.That(_tabStrip.Tabs[3], Is.SameAs(_tabNew), "Wrong tab at position 3.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }

  [Test]
  public void ReplaceOnlyTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tab0.IsSelected = true;

    _tabStrip.Tabs[0] = _tabNew;

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(1));
    Assert.That(_tabStrip.Tabs[0], Is.SameAs(_tabNew), "Wrong tab at position 0.");
    Assert.IsNull(_tabStrip.SelectedTab, "Tab selected.");
  }

  [Test]
  public void RemoveFirstTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab2.IsSelected = true;

    _tabStrip.Tabs.RemoveAt(0);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(3));
    Assert.That(_tabStrip.Tabs[0], Is.SameAs(_tab1), "Wrong tab at position 0.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }

  [Test]
  public void RemoveMiddleTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab3.IsSelected = true;

    _tabStrip.Tabs.RemoveAt(1);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(3));
    Assert.That(_tabStrip.Tabs[1], Is.SameAs(_tab2), "Wrong tab at position 1.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab3), "Wrong tab selected.");
  }

  [Test]
  public void RemoveLastTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab0.IsSelected = true;

    _tabStrip.Tabs.RemoveAt(3);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(3));

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab0), "Wrong tab selected.");
  }

  [Test]
  public void RemoveFirstTabWithTabBeingSelected ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab0.IsSelected = true;

    _tabStrip.Tabs.RemoveAt(0);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(3));
    Assert.That(_tabStrip.Tabs[0], Is.SameAs(_tab1), "Wrong tab at position 0.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab1), "Wrong tab selected.");
  }

  [Test]
  public void RemoveMiddleTabWithTabBeingSelected ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab1.IsSelected = true;

    _tabStrip.Tabs.RemoveAt(1);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(3));
    Assert.That(_tabStrip.Tabs[1], Is.SameAs(_tab2), "Wrong tab at position 1.");

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }

  [Test]
  public void RemoveLastTabWithTabBeingSelected ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab3.IsSelected = true;

    _tabStrip.Tabs.RemoveAt(3);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(3));

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }

  [Test]
  public void RemoveOnlyTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tab0.IsSelected = true;

    _tabStrip.Tabs.RemoveAt(0);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(0));
    Assert.IsNull(_tabStrip.SelectedTab, "Tab selected.");
  }

  [Test]
  public void ClearTabs ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab2.IsSelected = true;

    _tabStrip.Tabs.Clear();

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(0));
    Assert.IsNull(_tabStrip.SelectedTab, "Tab selected.");
  }

  [Test]
  public void AddFirstTabAsInvisibleTab ()
  {
    _tab0.IsVisible = false;
    _tabStrip.Tabs.Add(_tab0);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(1));
    Assert.That(_tabStrip.SelectedTab, Is.Null);
  }

  [Test]
  public void AddSecondTabAfterInvisibleTab ()
  {
    _tab0.IsVisible = false;
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(2));
    Assert.That(_tabStrip.SelectedTab, Is.Not.Null);
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab1));
  }

  [Test]
  public void SelectInvisibleTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tab1.IsVisible = false;
    Assert.That(
        () => _tab1.IsSelected = true,
        Throws.InvalidOperationException);
  }

  [Test]
  public void HideTabFollowedByInvisibleTab_WithFirstTabAsLastSelectable ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tab1.IsSelected = true;
    _tab2.IsVisible = false;
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab1));
    _tab1.IsVisible = false;

    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab0));
  }

  [Test]
  public void HideTabPrecededByInvisibleTab_WithFirstTabAsLastSelectable ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tab1.IsVisible = false;
    _tab2.IsSelected = true;
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2));
    _tab2.IsVisible = false;

    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab0));
  }

  [Test]
  public void HideTabFollowedByInvisibleTab_WithLastTabAsLastSelectable ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab1.IsSelected = true;
    _tab2.IsVisible = false;
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab1));
    _tab1.IsVisible = false;

    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab3));
  }

  [Test]
  public void HideFirstTabWithTabBeingSelected ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab0.IsSelected = true;

    _tab0.IsVisible = false;

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(4));

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab1), "Wrong tab selected.");
  }

  [Test]
  public void HideMiddleTabWithTabBeingSelected ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab1.IsSelected = true;

    _tab1.IsVisible = false;

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }

  [Test]
  public void HideLastTabWithTabBeingSelected ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab3.IsSelected = true;

    _tab3.IsVisible = false;

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(4));

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }

  [Test]
  public void AddFirstTabAsDisabledTab ()
  {
    _tab0.IsDisabled = true;
    _tabStrip.Tabs.Add(_tab0);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(1));
    Assert.That(_tabStrip.SelectedTab, Is.Null);
  }

  [Test]
  public void AddSecondTabAfterDisabledTab ()
  {
    _tab0.IsDisabled = true;
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(2));
    Assert.That(_tabStrip.SelectedTab, Is.Not.Null);
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab1));
  }

  [Test]
  public void SelectDisabledTab ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tab1.IsDisabled = true;
    Assert.That(
        () => _tab1.IsSelected = true,
        Throws.InvalidOperationException);
  }

  [Test]
  public void DisableFirstTabWithTabBeingSelected ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab0.IsSelected = true;

    _tab0.IsDisabled = true;

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(4));

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab1), "Wrong tab selected.");
  }

  [Test]
  public void DisableMiddleTabWithTabBeingSelected ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab1.IsSelected = true;

    _tab1.IsDisabled = true;

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }

  [Test]
  public void DisableLastTabWithTabBeingSelected ()
  {
    _tabStrip.Tabs.Add(_tab0);
    _tabStrip.Tabs.Add(_tab1);
    _tabStrip.Tabs.Add(_tab2);
    _tabStrip.Tabs.Add(_tab3);
    _tab3.IsSelected = true;

    _tab3.IsDisabled = true;

    Assert.That(_tabStrip.Tabs.Count, Is.EqualTo(4));

    Assert.IsNotNull(_tabStrip.SelectedTab, "No tab selected.");
    Assert.That(_tabStrip.SelectedTab, Is.SameAs(_tab2), "Wrong tab selected.");
  }
}
}
