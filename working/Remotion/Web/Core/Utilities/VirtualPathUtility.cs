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
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Utilities
{
  /// <summary>
  /// Provides utility methods for common virtual path operations.
  /// </summary>
  public static class VirtualPathUtility
  {
    /// <summary>
    /// Gets the virtual path for a <paramref name="path"/> relative to the <paramref name="control"/>'s <see cref="Control.TemplateSourceDirectory"/>.
    /// </summary>
    /// <param name="control">The <see cref="IControl"/> that acts as the base for the virtual path. Must not be <see langword="null" />.</param>
    /// <param name="path">The path (virtual or relative) to resolve. Must not be <see langword="null" /> or empty.</param>
    /// <returns>
    /// The virtual path based on the <paramref name="control"/>'s <see cref="Control.TemplateSourceDirectory"/> and the <paramref name="path"/>
    /// <para>- or -</para>
    /// just the value of <paramref name="path"/> if <paramref name="path"/> is already a virtual path.
    /// </returns>
    public static string GetVirtualPath (IControl control, string path)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);

      string appRelativeTemplateSourceDirectory = control.AppRelativeTemplateSourceDirectory;
      if (string.IsNullOrEmpty (appRelativeTemplateSourceDirectory))
      {
        throw new InvalidOperationException (
            string.Format (
                "The 'AppRelativeTemplateSourceDirectory' property of the {0} '{1}' is not set. "
                + "This can happen if the control's TemplateControl was not instantiated using System.Web.UI.Control.LoadControl (string).",
                control.GetType().Name,
                control.ID));
      }

      var templateSourceDirectory = System.Web.VirtualPathUtility.AppendTrailingSlash (appRelativeTemplateSourceDirectory);
      return System.Web.VirtualPathUtility.Combine (templateSourceDirectory, path);
    }
  }
}