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
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  /// <summary>
  /// Fluent selection extension methods.
  /// </summary>
  public static class FluentControlSelectorExtensionsForIntegrationTests
  {
    public static FluentControlSelector<AnchorSelector, AnchorControlObject> GetAnchor (this IControlHost host)
    {
      return new FluentControlSelector<AnchorSelector, AnchorControlObject> (host, new AnchorSelector());
    }

    public static FluentControlSelector<CommandSelector, CommandControlObject> GetCommand (this IControlHost host)
    {
      return new FluentControlSelector<CommandSelector, CommandControlObject> (host, new CommandSelector());
    }

    public static FluentControlSelector<DropDownListSelector, DropDownListControlObject> GetDropDownList (this IControlHost host)
    {
      return new FluentControlSelector<DropDownListSelector, DropDownListControlObject> (host, new DropDownListSelector());
    }

    public static FluentControlSelector<DropDownMenuSelector, DropDownMenuControlObject> GetDropDownMenu (this IControlHost host)
    {
      return new FluentControlSelector<DropDownMenuSelector, DropDownMenuControlObject> (host, new DropDownMenuSelector());
    }

    public static FluentControlSelector<FormGridSelector, FormGridControlObject> GetFormGrid (this IControlHost host)
    {
      return new FluentControlSelector<FormGridSelector, FormGridControlObject> (host, new FormGridSelector());
    }

    public static FluentControlSelector<ImageSelector, ImageControlObject> GetImage (this IControlHost host)
    {
      return new FluentControlSelector<ImageSelector, ImageControlObject> (host, new ImageSelector());
    }

    public static FluentControlSelector<ImageButtonSelector, ImageButtonControlObject> GetImageButton (this IControlHost host)
    {
      return new FluentControlSelector<ImageButtonSelector, ImageButtonControlObject> (host, new ImageButtonSelector());
    }

    public static FluentControlSelector<LabelSelector, LabelControlObject> GetLabel (this IControlHost host)
    {
      return new FluentControlSelector<LabelSelector, LabelControlObject> (host, new LabelSelector());
    }

    public static FluentControlSelector<ListMenuSelector, ListMenuControlObject> GetListMenu (this IControlHost host)
    {
      return new FluentControlSelector<ListMenuSelector, ListMenuControlObject> (host, new ListMenuSelector());
    }

    public static FluentControlSelector<ScopeSelector, ScopeControlObject> GetScope (this IControlHost host)
    {
      return new FluentControlSelector<ScopeSelector, ScopeControlObject> (host, new ScopeSelector());
    }

    public static FluentControlSelector<SingleViewSelector, SingleViewControlObject> GetSingleView (this IControlHost host)
    {
      return new FluentControlSelector<SingleViewSelector, SingleViewControlObject> (host, new SingleViewSelector());
    }

    public static FluentControlSelector<TabbedMenuSelector, TabbedMenuControlObject> GetTabbedMenu (this IControlHost host)
    {
      return new FluentControlSelector<TabbedMenuSelector, TabbedMenuControlObject> (host, new TabbedMenuSelector());
    }

    public static FluentControlSelector<TabbedMultiViewSelector, TabbedMultiViewControlObject> GetTabbedMultiView (this IControlHost host)
    {
      return new FluentControlSelector<TabbedMultiViewSelector, TabbedMultiViewControlObject> (host, new TabbedMultiViewSelector());
    }

    public static FluentControlSelector<TextBoxSelector, TextBoxControlObject> GetTextBox (this IControlHost host)
    {
      return new FluentControlSelector<TextBoxSelector, TextBoxControlObject> (host, new TextBoxSelector());
    }

    public static FluentControlSelector<TreeViewSelector, TreeViewControlObject> GetTreeView (this IControlHost host)
    {
      return new FluentControlSelector<TreeViewSelector, TreeViewControlObject> (host, new TreeViewSelector());
    }

    public static FluentControlSelector<WebButtonSelector, WebButtonControlObject> GetWebButton (this IControlHost host)
    {
      return new FluentControlSelector<WebButtonSelector, WebButtonControlObject> (host, new WebButtonSelector());
    }

    public static FluentControlSelector<WebTabStripSelector, WebTabStripControlObject> GetWebTabStrip (this IControlHost host)
    {
      return new FluentControlSelector<WebTabStripSelector, WebTabStripControlObject> (host, new WebTabStripSelector());
    }

    public static FluentControlSelector<WebTreeViewSelector, WebTreeViewControlObject> GetWebTreeView (this IControlHost host)
    {
      return new FluentControlSelector<WebTreeViewSelector, WebTreeViewControlObject> (host, new WebTreeViewSelector());
    }
  }
}