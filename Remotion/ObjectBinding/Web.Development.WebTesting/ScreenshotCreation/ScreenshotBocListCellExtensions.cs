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
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides fluent extension methods for controlling a <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/> cell.
  /// </summary>
  public static class ScreenshotBocListCellExtensions
  {
    /// <summary>
    /// Returns the error marker for this cell.
    /// </summary>
    /// <exception cref="InvalidOperationException">The error marker could not be found.</exception>
    public static FluentScreenshotElement<ElementScope> GetErrorMarker<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocListCell<TList, TRow, TCell>> fluentCell)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull("fluentCell", fluentCell);

      var result = fluentCell.Target.Cell.Scope.FindCss(".validationErrorMarker > img", Options.NoWait);
      if (!result.Exists(Options.NoWait))
        throw new InvalidOperationException("Can not find a error marker for this cell.");

      return result.ForElementScopeScreenshot();
    }
  }
}
