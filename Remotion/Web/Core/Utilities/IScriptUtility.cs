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
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Utilities
{
  /// <summary>
  /// Defines the API for registering various javascript-controlled features on the page.
  /// </summary>
 public interface IScriptUtility
  {
    /// <summary>
    /// Registers the client side scripts for the current theme.
    /// Call this method during or before the pre-render phase of the page life cycle.
    /// </summary>
    /// <param name="control">The <see cref="IControl"/> for which the include file is registered.</param>
    /// <param name="htmlHeadAppender">The <see cref="HtmlHeadAppender"/> to use for registering the include file.</param>
    void RegisterJavaScriptInclude (IControl control, HtmlHeadAppender htmlHeadAppender);
  }
}
