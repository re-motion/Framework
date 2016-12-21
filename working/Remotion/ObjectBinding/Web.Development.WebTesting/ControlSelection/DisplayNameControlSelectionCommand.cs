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
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the control of the given <typeparamref name="TControlObject"/> type bearing the given display name
  /// within the given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class DisplayNameControlSelectionCommand<TControlObject> : IControlSelectionCommand<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly IDisplayNameControlSelector<TControlObject> _controlSelector;
    private readonly string _displayName;

    public DisplayNameControlSelectionCommand (
        [NotNull] IDisplayNameControlSelector<TControlObject> controlSelector,
        [NotNull] string displayName)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("displayName", displayName);

      _controlSelector = controlSelector;
      _displayName = displayName;
    }

    /// <inheritdoc/>
    public TControlObject Select (ControlSelectionContext context)
    {
      return _controlSelector.SelectPerDisplayName (context, _displayName);
    }
  }
}