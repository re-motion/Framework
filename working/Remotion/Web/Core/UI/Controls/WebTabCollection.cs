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
using System.Collections;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  //[Editor (typeof (WebTabCollectionEditor), typeof (UITypeEditor))]
  public class WebTabCollection : ControlItemCollection, IControlStateManager
  {
    private WebTabStrip _tabStrip;

    /// <summary> Initializes a new instance. </summary>
    /// <param name="ownerControl"> Owner control. </param>
    /// <param name="supportedTypes">
    ///   Supported types must either be <see cref="WebTab"/> or derived from <see cref="WebTab"/>. 
    ///   Must not be <see langword="null"/> or contain items that are <see langword="null"/>.
    /// </param>
    public WebTabCollection (IControl ownerControl, Type[] supportedTypes)
        : base (ownerControl, supportedTypes)
    {
      for (int i = 0; i < supportedTypes.Length; i++)
      {
        Type type = supportedTypes[i];
        if (!typeof (WebTab).IsAssignableFrom (type))
        {
          throw new ArgumentException (
              string.Format ("Type '{0}' at index {1} is not compatible with type 'WebTab'.", type.FullName, i), "supportedTypes");
        }
      }
    }

    /// <summary> Initializes a new instance. </summary>
    public WebTabCollection (IControl ownerControl)
        : this (ownerControl, new[] { typeof (WebTab) })
    {
    }

    //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
    protected internal new WebTab this [int index]
    {
      get { return (WebTab) List[index]; }
      set { List[index] = value; }
    }

    protected override void ValidateNewValue (object value)
    {
      WebTab tab = ArgumentUtility.CheckNotNullAndType<WebTab> ("value", value);
      EnsureDesignModeTabInitialized (tab);
      if (string.IsNullOrEmpty (tab.ItemID))
        throw new ArgumentException ("The tab does not have an 'ItemID'. It can therfor not be inserted into the collection.", "value");
      base.ValidateNewValue (value);
    }

    private void EnsureDesignModeTabInitialized (WebTab tab)
    {
      ArgumentUtility.CheckNotNull ("tab", tab);
      if (! tab.HasItemID()
          && (_tabStrip != null && ControlHelper.IsDesignMode (_tabStrip)
              || OwnerControl != null && ControlHelper.IsDesignMode (OwnerControl)))
      {
        int index = InnerList.Count;
        do
        {
          index++;
          string itemID = "Tab" + index;
          if (Find (itemID) == null)
          {
            tab.ItemID = itemID;
            if (string.IsNullOrEmpty (tab.Text))
              tab.Text = "Tab " + index;
            break;
          }
        } while (true);
      }
    }

    protected override void OnInsertComplete (int index, object value)
    {
      WebTab tab = ArgumentUtility.CheckNotNullAndType<WebTab> ("value", value);
      base.OnInsertComplete (index, value);
      tab.SetTabStrip (_tabStrip);
      InitalizeSelectedTab();
    }

    protected override void OnSetComplete (int index, object oldValue, object newValue)
    {
      WebTab oldTab = ArgumentUtility.CheckNotNullAndType<WebTab> ("oldValue", oldValue);
      WebTab newTab = ArgumentUtility.CheckNotNullAndType<WebTab> ("newValue", newValue);

      base.OnSetComplete (index, oldValue, newValue);

      newTab.SetTabStrip (_tabStrip);
      oldTab.SetTabStrip (null);

      DeselectTab (oldTab, index);
    }

    protected override void OnRemoveComplete (int index, object value)
    {
      WebTab tab = ArgumentUtility.CheckNotNullAndType<WebTab> ("value", value);

      base.OnRemoveComplete (index, value);

      tab.SetTabStrip (null);
      if (_tabStrip != null && tab.IsSelected)
      {
        bool wasLastTab = index == InnerList.Count;
        if (wasLastTab)
        {
          if (InnerList.Count > 1)
          {
            WebTab lastTab = (WebTab) InnerList[index - 1];
            _tabStrip.SetSelectedTabInternal (lastTab);
          }
          else
            _tabStrip.SetSelectedTabInternal (null);
        }
        else
        {
          WebTab nextTab = (WebTab) InnerList[index];
          _tabStrip.SetSelectedTabInternal (nextTab);
        }
      }
    }

    protected override void OnClear ()
    {
      base.OnClear();
      for (int i = 0; i < InnerList.Count; i++)
      {
        WebTab tab = (WebTab) InnerList[i];
        tab.SetTabStrip (null);
      }
      if (_tabStrip != null)
        _tabStrip.SetSelectedTabInternal (null);
    }

    internal void DeselectTabInternal (WebTab tab)
    {
      ArgumentUtility.CheckNotNull ("tab", tab);
      if (tab.TabStrip != null && tab.TabStrip != _tabStrip)
        throw new ArgumentException ("The tab is not part of this collection's Tabstrip", "tab");

      DeselectTab (tab, IndexOf (tab));
    }

    /// <summary> Deselects a <see cref="WebTab"/> whose position in the list is still occupied. </summary>
    protected void DeselectTab (WebTab tab, int index)
    {
      ArgumentUtility.CheckNotNull ("tab", tab);

      if (_tabStrip != null && tab.IsSelected)
      {
        bool isLastTab = index + 1 == InnerList.Count;
        if (isLastTab)
        {
          if (InnerList.Count > 1)
          {
            WebTab penultimateTab = (WebTab) InnerList[index - 1];
            _tabStrip.SetSelectedTabInternal (penultimateTab);
          }
          else
            _tabStrip.SetSelectedTabInternal (null);
        }
        else
        {
          WebTab nextTab = (WebTab) InnerList[index + 1];
          _tabStrip.SetSelectedTabInternal (nextTab);
        }
      }
    }

    public void AddRange (WebTabCollection values)
    {
      base.AddRange (values);
    }

    protected internal void SetTabStrip (WebTabStrip tabStrip)
    {
      ArgumentUtility.CheckNotNull ("tabStrip", tabStrip);

      _tabStrip = tabStrip;
      for (int i = 0; i < InnerList.Count; i++)
        ((WebTab) InnerList[i]).SetTabStrip (_tabStrip);
      InitalizeSelectedTab();
    }

    /// <summary>
    ///   Finds the <see cref="WebTab"/> with a <see cref="WebTab.ItemID"/> of <paramref name="id"/>.
    /// </summary>
    /// <param name="id"> The ID to look for. </param>
    /// <returns> A <see cref="WebTab"/> or <see langword="null"/> if no matching tab was found. </returns>
    public new WebTab Find (string id)
    {
      return (WebTab) base.Find (id);
    }

    private void InitalizeSelectedTab ()
    {
      if (_tabStrip != null
          && (_tabStrip.Page == null || ! _tabStrip.Page.IsPostBack)
          && _tabStrip.SelectedTab == null)
      {
        for (int i = 0; i < InnerList.Count; i++)
        {
          WebTab tab = (WebTab) InnerList[i];
          if (tab.IsVisible && ! tab.IsDisabled)
          {
            _tabStrip.SetSelectedTabInternal (tab);
            break;
          }
        }
      }
    }

    void IControlStateManager.LoadControlState (object state)
    {
      LoadControlState (state);
    }

    protected virtual void LoadControlState (object state)
    {
      if (state == null)
        return;

      Pair[] tabsState = (Pair[]) state;
      for (int i = 0; i < tabsState.Length; i++)
      {
        Pair pair = tabsState[i];
        string itemID = (string) pair.First;
        WebTab tab = Find (itemID);
        if (tab != null)
          ((IControlStateManager) tab).LoadControlState (pair.Second);
      }
    }

    object IControlStateManager.SaveControlState ()
    {
      return SaveControlState();
    }

    protected virtual object SaveControlState ()
    {
      ArrayList tabsState = new ArrayList();
      for (int i = 0; i < InnerList.Count; i++)
      {
        WebTab tab = (WebTab) InnerList[i];
        object tabStateValue = ((IControlStateManager) tab).SaveControlState();
        if (tabStateValue != null)
        {
          Pair pair = new Pair();
          pair.First = tab.ItemID;
          pair.Second = tabStateValue;
          tabsState.Add (pair);
        }
      }
      if (tabsState.Count == 0)
        return null;
      else
        return tabsState.ToArray (typeof (Pair));
    }
  }
}
