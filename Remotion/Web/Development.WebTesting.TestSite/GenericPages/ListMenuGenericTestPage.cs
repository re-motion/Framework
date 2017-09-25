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
using Remotion.Web.Development.WebTesting.TestSite.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite.GenericPages
{
  /// <summary>
  /// Custom <see cref="IGenericTestPage{TOptions}"/> for a <see cref="ListMenu"/>.
  /// </summary>
  public class ListMenuGenericTestPage : SimpleGenericTestPage<ListMenu>
  {
    public ListMenuGenericTestPage ()
    {
    }

    /// <inheritdoc />
    public override ListMenu CreateControl (GenericTestOptions options)
    {
      var control = base.CreateControl (options);
      control.MenuItems.Add (CreateWebMenuItem ("A"));
      control.MenuItems.Add (CreateWebMenuItem ("B"));
      return control;
    }

    private WebMenuItem CreateWebMenuItem (string name)
    {
      return new WebMenuItem
             {
                 ItemID = "Item" + name,
                 Category = "CatA",
                 Text = name
             };
    }
  }
}