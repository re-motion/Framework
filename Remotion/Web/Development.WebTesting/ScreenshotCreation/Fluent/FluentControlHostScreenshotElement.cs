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
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent
{
  /// <summary>
  /// A <see cref="FluentScreenshotElement{T}"/> which is restricted to <see cref="IControlHost"/>s.
  /// As itself is inherited from <see cref="IControlHost"/> it will provide all extension 
  /// methods redirecting the calls to the <see cref="FluentScreenshotElement{T}"/> target.
  /// </summary>
  public class FluentControlHostScreenshotElement<T> : FluentScreenshotElement<T>, IControlHost
      where T : IControlHost
  {
    public FluentControlHostScreenshotElement (IFluentScreenshotElement<T> fluentElement)
        : base(fluentElement.Target, fluentElement.Resolver, fluentElement.MinimumElementVisibility)
    {
    }

    public FluentControlHostScreenshotElement (
        [NotNull] T target,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
        : base(target, resolver, minimumElementVisibility)
    {
    }

    /// <inheritdoc />
    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull("controlSelectionCommand", controlSelectionCommand);

      return ((IFluentScreenshotElementWithCovariance<T>)this).Target.GetControl(controlSelectionCommand);
    }

    /// <inheritdoc />
    public TControlObject? GetControlOrNull<TControlObject> (IControlOptionalSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull("controlSelectionCommand", controlSelectionCommand);

      return ((IFluentScreenshotElementWithCovariance<T>)this).Target.GetControlOrNull(controlSelectionCommand);
    }

    /// <inheritdoc />
    public bool HasControl (IControlExistsCommand controlSelectionCommand)
    {
      ArgumentUtility.CheckNotNull("controlSelectionCommand", controlSelectionCommand);

      return ((IFluentScreenshotElementWithCovariance<T>)this).Target.HasControl(controlSelectionCommand);
    }
  }
}
