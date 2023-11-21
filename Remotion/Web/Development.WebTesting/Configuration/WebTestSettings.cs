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
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Provides access to <see cref="IWebTestSettings"/>.
  /// </summary>
  public static class WebTestSettings
  {
    private static readonly object s_lock = new();

    private static IWebTestSettings? s_current;

    /// <summary>
    /// Gets the current <see cref="IWebTestSettings"/> and can be set to override the default settings.
    /// </summary>
    [AllowNull]
    public static IWebTestSettings Current
    {
      get
      {
        var current = s_current;
        if (current != null)
          return current;

        lock (s_lock)
        {
          return s_current ??= CreateDefaultWebTestSettings();
        }
      }
      set
      {
        lock (s_lock)
        {
          s_current = value;
        }
      }
    }

    private static IWebTestSettings CreateDefaultWebTestSettings ()
    {
      var settings = (WebTestConfigurationSection)ConfigurationManager.GetSection("remotion.webTesting");
      Assertion.IsNotNull(settings, "Configuration section 'remotion.webTesting' missing.");
      return settings;
    }
  }
}
