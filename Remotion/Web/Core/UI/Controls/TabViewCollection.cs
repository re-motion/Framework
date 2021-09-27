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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{

public class TabViewCollection: ViewCollection
{
  public TabViewCollection (Control owner)
    : this ((TabbedMultiView.MultiView) owner)
  {
  }

  internal TabViewCollection (TabbedMultiView.MultiView owner)
    : base (owner)
  {
  }

  public override void Add (Control control)
  {
    TabView view = ArgumentUtility.CheckNotNullAndType<TabView> ("control", control);
    base.Add (view);
    Owner.OnTabViewInserted (view);
  }

  public override void AddAt(int index, Control control)
  {
    TabView view = ArgumentUtility.CheckNotNullAndType<TabView> ("control", control);
    base.AddAt (index, view);
    Owner.OnTabViewInserted (view);
  }

  public override void Remove (Control control)
  {
    TabView view = ArgumentUtility.CheckNotNullAndType<TabView> ("control", control);
    Owner.OnTabViewRemove (view);
    base.Remove (control);
    Owner.OnTabViewRemoved (view);
  }

  public override void RemoveAt (int index)
  {
    if (index < 0 || index > this.Count)
      throw new ArgumentOutOfRangeException ("index");
    TabView view = (TabView) this[index];
    Owner.OnTabViewRemove (view);
    base.RemoveAt (index);
    Owner.OnTabViewRemoved (view);
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new TabView this[int index]
  {
    get { return (TabView) base[index]; }
  }

  private new TabbedMultiView.MultiView Owner
  {
    get { return (TabbedMultiView.MultiView) base.Owner; }
  }
}

}
