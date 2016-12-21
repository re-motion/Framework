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
using System.Drawing.Design;
using Remotion.Utilities;
using Remotion.Web.UI.Design;

namespace Remotion.Web.UI.Controls
{
  [Editor (typeof (MainMenuTabCollectionEditor), typeof (UITypeEditor))]
  public class MainMenuTabCollection : WebTabCollection
  {
    /// <summary> Initializes a new instance. </summary>
    public MainMenuTabCollection (IControl ownerControl, Type[] supportedTypes)
        : base (ownerControl, supportedTypes)
    {
    }

    /// <summary> Initializes a new instance. </summary>
    public MainMenuTabCollection (IControl ownerControl)
        : this (ownerControl, new[] { typeof (SubMenuTab) })
    {
    }

    public int Add (MainMenuTab tab)
    {
      return base.Add (tab);
    }

    public void AddRange (params MainMenuTab[] tabs)
    {
      base.AddRange (tabs);
    }

    public void Insert (int index, MainMenuTab tab)
    {
      base.Insert (index, tab);
    }

    //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
    protected internal new MainMenuTab this [int index]
    {
      get { return (MainMenuTab) List[index]; }
      set { List[index] = value; }
    }
  }

  [Editor (typeof (SubMenuTabCollectionEditor), typeof (UITypeEditor))]
  public class SubMenuTabCollection : WebTabCollection
  {
    private MainMenuTab _parent;

    /// <summary> Initializes a new instance. </summary>
    public SubMenuTabCollection (IControl ownerControl, Type[] supportedTypes)
        : base (ownerControl, supportedTypes)
    {
    }

    /// <summary> Initializes a new instance. </summary>
    public SubMenuTabCollection (IControl ownerControl)
        : this (ownerControl, new Type[] { typeof (SubMenuTab) })
    {
    }

    protected override void OnInsertComplete (int index, object value)
    {
      SubMenuTab tab = ArgumentUtility.CheckNotNullAndType<SubMenuTab> ("value", value);

      base.OnInsertComplete (index, value);
      tab.SetParent (_parent);
    }

    protected override void OnSetComplete (int index, object oldValue, object newValue)
    {
      SubMenuTab tab = ArgumentUtility.CheckNotNullAndType<SubMenuTab> ("newValue", newValue);

      base.OnSetComplete (index, oldValue, newValue);
      tab.SetParent (_parent);
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public MainMenuTab Parent
    {
      get { return _parent; }
    }

    protected internal void SetParent (MainMenuTab parent)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      _parent = parent;
      for (int i = 0; i < InnerList.Count; i++)
        ((SubMenuTab) InnerList[i]).SetParent (_parent);
    }

    public int Add (SubMenuTab tab)
    {
      return base.Add (tab);
    }

    public void AddRange (params SubMenuTab[] tabs)
    {
      base.AddRange (tabs);
    }

    public void Insert (int index, SubMenuTab tab)
    {
      base.Insert (index, tab);
    }

    //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
    protected internal new SubMenuTab this [int index]
    {
      get { return (SubMenuTab) List[index]; }
      set { List[index] = value; }
    }
  }
}
