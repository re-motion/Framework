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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Represents a <see cref="BocTreeViewNodeControlObject"/> for the screenshot creation API.
  /// </summary>
  public class ScreenshotBocTreeViewNodeControlObject : ISelfResolvable
  {
    private readonly IFluentScreenshotElement<BocTreeViewNodeControlObject> _fluentWebTreeView;

    public ScreenshotBocTreeViewNodeControlObject (IFluentScreenshotElement<BocTreeViewNodeControlObject> fluentWebTreeView)
    {
      _fluentWebTreeView = fluentWebTreeView;
    }

    public BocTreeViewNodeControlObject BocTreeViewNode
    {
      get { return _fluentWebTreeView.Target; }
    }

    public IFluentScreenshotElement<BocTreeViewNodeControlObject> FluentBocTreeViewNode
    {
      get { return _fluentWebTreeView; }
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveBrowserCoordinates ()
    {
      return _fluentWebTreeView.ResolveBrowserCoordinates();
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveDesktopCoordinates (IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull ("locator", locator);

      return _fluentWebTreeView.ResolveDesktopCoordinates (locator);
    }
  }
}