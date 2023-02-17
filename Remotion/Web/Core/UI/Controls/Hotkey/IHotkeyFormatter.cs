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
using System.Web.UI;
using Remotion.ServiceLocation;

namespace Remotion.Web.UI.Controls.Hotkey
{
  /// <summary>
  /// Defines the methods required for rendering a <see cref="WebString"/> with that contains an encoded hotkey.
  /// </summary>
  /// <remarks>
  /// <para>Use <see cref="IServiceLocator"/> to retrieve an instance of type <see cref="IHotkeyFormatter"/>.</para>
  /// <para>The default implementation (<see cref="UnderscoreHotkeyFormatter"/>) underlines the hotkey.</para>
  /// </remarks>
  public interface IHotkeyFormatter
  {
    char? GetAccessKey (WebString value);
    void WriteTo (HtmlTextWriter writer, WebString value);
  }
}
