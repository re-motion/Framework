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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI
{
  /// <summary>
  /// Defines states for <see cref="ISmartPage"/>.<see cref="ISmartPage.GetDirtyStates"/>.
  /// </summary>
  /// <seealso cref="WxePageDirtyStates"/>
  public static class SmartPageDirtyStates
  {
    /// <summary>
    /// Represents the current page and the tracked <see cref="IEditableControl"/> objects.
    /// </summary>
    public static readonly string CurrentPage = "CurrentPage"; // Any changes must be reflected in SmartPage.ts - SmartPage_DirtyStates enum

    /// <summary>
    /// Represents the client side of the tracked <see cref="IEditableControl"/> objects.
    /// </summary>
    public static readonly string ClientSide = "ClientSide"; // Any changes must be reflected in SmartPage.ts - SmartPage_DirtyStates enum
  }
}
