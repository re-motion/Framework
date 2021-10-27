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
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Services
{
  /// <summary>
  /// Represents a <see cref="WebMenuItem"/> sent over a web service interface.
  /// </summary>
  public class WebMenuItemProxy
  {
    /// <summary>
    /// Creates a <see cref="WebMenuItemProxy"/> for a specific ID.
    /// </summary>
    /// <param name="id">The ID of the <see cref="WebMenuItem"/>. Must not be <see langword="null" /> or empty.</param>
    /// <param name="isDisabled">A flag indicating if the <see cref="WebMenuItem"/> should be rendered in a disabled state.</param>
    /// <returns>An <see cref="WebMenuItemProxy"/> representing a <see cref="WebMenuItem"/> in a web service interface. </returns>
    public static WebMenuItemProxy Create (string id, bool isDisabled)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      return new WebMenuItemProxy (id, isDisabled);
    }

    public string ID { get; }

    public bool IsDisabled { get; }

    private WebMenuItemProxy (string id, bool isDisabled)
    {
      ID = id;
      IsDisabled = isDisabled;
    }

#nullable disable
    [Obsolete ("Default ctor for ASMX WSDL page.", true)]
    private WebMenuItemProxy ()
    {
    }
#nullable restore
  }
}