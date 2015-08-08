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

namespace Remotion.Web.Contracts.DiagnosticMetadata
{
  /// <summary>
  /// Diagnostic metadata attribute names used by various renderers.
  /// </summary>
  public static class DiagnosticMetadataAttributes
  {
    public static readonly string CommandName = "data-command-name";
    public static readonly string Content = "data-content";
    public static readonly string ControlType = "data-control-type";
    public static readonly string IndexInCollection = "data-index";
    public static readonly string IsReadOnly = "data-is-readonly";
    public static readonly string ItemID = "data-item-id";
    public static readonly string TriggersNavigation = "data-triggers-navigation";
    public static readonly string TriggersPostBack = "data-triggers-postback";

    public static readonly string WebTreeViewNumberOfChildren = "data-webtreeview-number-of-children";
    public static readonly string WebTreeViewIsSelectedNode = "data-webtreeview-is-selected-node";
    public static readonly string WebTreeViewWellKnownAnchor = "data-webtreeview-wellknown-anchor";
  }
}