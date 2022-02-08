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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Web.UI
{
  /// <summary>
  /// Defines extension methods for <see cref="ISmartPage"/>.
  /// </summary>
  public static class SmartPageExtensions
  {
    /// <summary>
    /// Evaluates <see cref="ISmartPage"/>.<see cref="ISmartPage.GetDirtyStates"/> and returns <see langword="true" /> if the result is not empty.
    /// </summary>
    /// <param name="smartPage">The <see cref="ISmartPage"/> for which the dirty state is evaluated. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if <see cref="ISmartPage"/>.<see cref="ISmartPage.GetDirtyStates"/> returns items, otherwise <see langword="false" />.</returns>
    public static bool EvaluateDirtyState (this ISmartPage smartPage)
    {
      ArgumentUtility.CheckNotNull("smartPage", smartPage);

      return smartPage.GetDirtyStates().Any();
    }
  }
}
