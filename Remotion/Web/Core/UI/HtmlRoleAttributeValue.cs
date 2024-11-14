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

namespace Remotion.Web.UI
{
  /// <summary>
  /// Defines values for the <see cref="HtmlTextWriterAttribute2.Role"/> attribute.
  /// </summary>
  public static class HtmlRoleAttributeValue
  {
    public const string Button = "button";
    public const string Cell = "cell";
    public const string Checkbox = "checkbox";
    public const string Combobox = "combobox";
    public const string ColumnHeader = "columnheader";
    public const string Group = "group";
    public const string Listbox = "listbox";
    public const string Menu = "menu";
    public const string MenuBar = "menubar";
    public const string MenuItem = "menuitem";
    public const string None = "none";
    [Obsolete("Use HtmlRoleAttributeValue.None instead.")]
    public const string Presentation = "presentation";
    public const string Radio = "radio";
    public const string RadioGroup = "radiogroup";
    public const string Region = "region";
    public const string Row = "row";
    public const string RowGroup = "rowgroup";
    public const string RowHeader = "rowheader";
    public const string Tab = "tab";
    public const string Table = "table";
    public const string TabList = "tablist";
    public const string TabPanel = "tabpanel";
    public const string Textbox = "textbox";
    public const string Toolbar = "toolbar";
    public const string Tree = "tree";
    public const string TreeItem = "treeitem";
    public const string Alert = "alert";
  }
}
