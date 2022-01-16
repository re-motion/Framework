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
using System.IO;
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.Hotkey
{
  /// <summary>
  /// Extension methods for the <see cref="IHotkeyFormatter"/> interface.
  /// </summary>
  public static class HotkeyFormatterExtensions
  {
    /// <summary>
    /// Gets the hotkey-formatted <see cref="String"/> from the <paramref name="value"/>.
    /// </summary>
    /// <param name="hotkeyFormatter">The <see cref="IHotkeyFormatter"/> implementation to be used. Must not be <see langword="null" />.</param>
    /// <param name="value">The <see cref="WebString"/> to use as basis.</param>
    /// <returns>The hotkey-formatted <see cref="WebString"/>.</returns>
    /// <remarks>Use <see cref="IHotkeyFormatter"/>.<see cref="IHotkeyFormatter.WriteTo"/> instead, when the <see cref="HtmlTextWriter"/> is available.</remarks>
    public static WebString GetFormattedText (this IHotkeyFormatter hotkeyFormatter, WebString value)
    {
      ArgumentUtility.CheckNotNull("hotkeyFormatter", hotkeyFormatter);

      using var stringWriter = new StringWriter();
      using var htmlTextWriter = new HtmlTextWriter(stringWriter);

      hotkeyFormatter.WriteTo(htmlTextWriter, value);
      htmlTextWriter.Flush();

      return WebString.CreateFromHtml(stringWriter.ToString());
    }
  }
}
