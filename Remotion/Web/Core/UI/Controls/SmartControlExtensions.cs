// // This file is part of the re-motion Core Framework (www.re-motion.org)
// // Copyright (c) rubicon IT GmbH, www.rubicon.eu
// //
// // The re-motion Core Framework is free software; you can redistribute it
// // and/or modify it under the terms of the GNU Lesser General Public License
// // as published by the Free Software Foundation; either version 2.1 of the
// // License, or (at your option) any later version.
// //
// // re-motion is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU Lesser General Public License for more details.
// //
// // You should have received a copy of the GNU Lesser General Public License
// // along with re-motion; if not, see http://www.gnu.org/licenses.
// //
using System;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Defines extension methods for the <see cref="ISmartControl"/> interface.
  /// </summary>
  public static class SmartControlExtensions
  {
    /// <summary>
    /// Assigns a single <paramref name="labelID"/> to a <paramref name="control"/>.
    /// </summary>
    [Obsolete("Use ISmartControl.AssignLabels(IEnumerable<string>) instead. (Version 3.10.0)")]
    public static void AssignLabel (this ISmartControl control, string labelID)
    {
      ArgumentUtility.CheckNotNull("control", control);
      ArgumentUtility.CheckNotNullOrEmpty("labelID", labelID);

      control.AssignLabels(EnumerableUtility.Singleton(labelID));
    }
  }
}
