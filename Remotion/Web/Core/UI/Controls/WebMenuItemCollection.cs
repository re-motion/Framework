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
using System.ComponentModel;
using System.Drawing.Design;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.UI.Design;

namespace Remotion.Web.UI.Controls
{
  /// <summary> A collection of <see cref="WebMenuItem"/> objects. </summary>
  [Editor (typeof (WebMenuItemCollectionEditor), typeof (UITypeEditor))]
  public class WebMenuItemCollection : ControlItemCollection
  {
    /// <summary> Sorts the <paramref name="menuItems"/> by their categories." </summary>
    /// <param name="menuItems"> Must not be <see langword="null"/> or contain items that are <see langword="null"/>. </param>
    /// <param name="generateSeparators"> <see langword="true"/> to generate a separator before starting a new category. </param>
    /// <returns> The <paramref name="menuItems"/>, sorted by their categories. </returns>
    public static WebMenuItem[] GroupMenuItems (WebMenuItem[] menuItems, bool generateSeparators)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("menuItems", menuItems);

      //  <string category, ArrayList menuItems>
      NameObjectCollection groupedMenuItems = new NameObjectCollection();
      ArrayList categories = new ArrayList();

      for (int i = 0; i < menuItems.Length; i++)
      {
        WebMenuItem menuItem = menuItems[i];

        string category = menuItem.Category ?? string.Empty;
        ArrayList menuItemsForCategory;
        if (groupedMenuItems.Contains (category))
          menuItemsForCategory = (ArrayList) groupedMenuItems[category];
        else
        {
          menuItemsForCategory = new ArrayList();
          groupedMenuItems.Add (category, menuItemsForCategory);
          categories.Add (category);
        }
        menuItemsForCategory.Add (menuItem);
      }

      ArrayList arrayList = new ArrayList();
      bool isFirst = true;
      for (int i = 0; i < categories.Count; i++)
      {
        string category = (string) categories[i];
        if (generateSeparators)
        {
          if (isFirst)
            isFirst = false;
          else
            arrayList.Add (WebMenuItem.GetSeparator());
        }
        arrayList.AddRange ((ArrayList) groupedMenuItems[category]);
      }
      return (WebMenuItem[]) arrayList.ToArray (typeof (WebMenuItem));
    }

    /// <summary> Initializes a new instance. </summary>
    public WebMenuItemCollection (IControl ownerControl, Type[] supportedTypes)
        : base (ownerControl, supportedTypes)
    {
    }

    /// <summary> Initializes a new instance. </summary>
    public WebMenuItemCollection (IControl ownerControl)
        : this (ownerControl, new[] { typeof (WebMenuItem) })
    {
    }

    public new WebMenuItem[] ToArray ()
    {
      return (WebMenuItem[]) InnerList.ToArray (typeof (WebMenuItem));
    }

    //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
    protected internal new WebMenuItem this [int index]
    {
      get { return (WebMenuItem) List[index]; }
      set { List[index] = value; }
    }

    /// <summary> Sorts the <see cref="WebMenuItem"/> objects by their categories." </summary>
    /// <param name="generateSeparators"> <see langword="true"/> to generate a separator before starting a new category. </param>
    /// <returns> The <see cref="WebMenuItem"/> objects, sorted by their categories. </returns>
    public WebMenuItem[] GroupMenuItems (bool generateSeparators)
    {
      return GroupMenuItems (ToArray(), generateSeparators);
    }
  }
}
