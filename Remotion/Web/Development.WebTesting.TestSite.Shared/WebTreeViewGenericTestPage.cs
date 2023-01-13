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
using Remotion.Web.Development.WebTesting.TestSite.Shared.GenericPages;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite.Shared
{
  /// <summary>
  /// Custom <see cref="IGenericTestPage{TOptions}"/> for a <see cref="WebTreeView"/>.
  /// </summary>
  public class WebTreeViewGenericTestPage : SimpleGenericTestPage<WebTreeView>
  {
    public WebTreeViewGenericTestPage ()
    {
    }

    public override WebTreeView CreateControl (GenericTestOptions options)
    {
      var control = base.CreateControl(options);
      control.Nodes.Add(new WebTreeNode { ItemID = "ItemA", Text = WebString.CreateFromText("A") });
      return control;
    }
  }
}
