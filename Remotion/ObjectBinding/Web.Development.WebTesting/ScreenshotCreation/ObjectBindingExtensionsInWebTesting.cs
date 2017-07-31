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
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

// ReSharper disable once CheckNamespace

namespace Remotion.Web.Development.WebTesting
{
  public static class ObjectBindingExtensionsInWebTesting
  {
    /// <summary>
    /// Starts the fluent screenshot API for the specified <paramref name="list"/>.
    /// </summary>
    public static FluentScreenshotElement<ScreenshotBocList<BocListControlObject, BocListRowControlObject, BocListCellControlObject>> ForScreenshot (
        [NotNull] this BocListControlObject list)
    {
      ArgumentUtility.CheckNotNull ("list", list);

      return
          SelfResolvableFluentScreenshot.Create (
              new ScreenshotBocList<BocListControlObject, BocListRowControlObject, BocListCellControlObject> (
                  FluentUtility.CreateFluentControlObject (list)));
    }

    /// <summary>
    /// Starts the fluent screenshot API for the specified <paramref name="listAsGrid"/>
    /// </summary>
    public static
        FluentScreenshotElement<ScreenshotBocList<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject>>
        ForScreenshot ([NotNull] this BocListAsGridControlObject listAsGrid)
    {
      ArgumentUtility.CheckNotNull ("listAsGrid", listAsGrid);

      return
          SelfResolvableFluentScreenshot.Create (
              new ScreenshotBocList<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject> (
                  FluentUtility.CreateFluentControlObject (listAsGrid)));
    }

    /// <summary>
    /// Returns the list of the specified <paramref name="fluentList"/>.
    /// </summary>
    public static TList GetTarget<TList, TRow, TCell> ([NotNull] this IFluentScreenshotElement<ScreenshotBocList<TList, TRow, TCell>> fluentList)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      return fluentList.Target.List;
    }

    // itodo more GetTarget
  }
}