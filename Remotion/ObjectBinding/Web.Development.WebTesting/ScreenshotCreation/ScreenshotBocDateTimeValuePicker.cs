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
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Marker class for the screenshot fluent API. Represents a <see cref="BocDateTimeValueControlObject"/> date picker.
  /// </summary>
  public class ScreenshotBocDateTimeValuePicker : ISelfResolvable
  {
    private readonly IFluentScreenshotElementWithCovariance<BocDateTimeValueControlObject> _fluentDateTimeValue;

    public ScreenshotBocDateTimeValuePicker (
        [NotNull] IFluentScreenshotElementWithCovariance<BocDateTimeValueControlObject> fluentDateTimeValue)
    {
      ArgumentUtility.CheckNotNull("fluentDateTimeValue", fluentDateTimeValue);

      _fluentDateTimeValue = fluentDateTimeValue;
    }

    public IFluentScreenshotElementWithCovariance<BocDateTimeValueControlObject> FluentDateTimeValue
    {
      get { return _fluentDateTimeValue; }
    }

    public BocDateTimeValueControlObject DateTimeValue
    {
      get { return _fluentDateTimeValue.Target; }
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveBrowserCoordinates ()
    {
      return ((IFluentScreenshotElement<ElementScope>) _fluentDateTimeValue.GetDatePicker().GetElement()).ResolveBrowserCoordinates();
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveDesktopCoordinates (IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull("locator", locator);

      return ((IFluentScreenshotElement<ElementScope>) _fluentDateTimeValue.GetDatePicker().GetElement()).ResolveDesktopCoordinates(locator);
    }
  }
}