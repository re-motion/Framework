﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the control of the given <typeparamref name="TControlObject"/> type bearing the given local ID within
  /// the given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class LocalIDControlSelectionCommand<TControlObject>
      : IControlSelectionCommand<TControlObject>,
        IControlOptionalSelectionCommand<TControlObject>,
        IControlExistsCommand
      where TControlObject : ControlObject
  {
    private readonly ILocalIDControlSelector<TControlObject> _controlSelector;
    private readonly string _localID;

    public LocalIDControlSelectionCommand ([NotNull] ILocalIDControlSelector<TControlObject> controlSelector, [NotNull] string localID)
    {
      ArgumentUtility.CheckNotNull("controlSelector", controlSelector);
      ArgumentUtility.CheckNotNullOrEmpty("localID", localID);

      _controlSelector = controlSelector;
      _localID = localID;
    }

    /// <inheritdoc/>
    public TControlObject Select (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      return _controlSelector.SelectPerLocalID(context, _localID);
    }

    /// <inheritdoc/>
    public TControlObject? SelectOptional (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      return _controlSelector.SelectOptionalPerLocalID(context, _localID);
    }

    /// <inheritdoc/>
    public bool Exists (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      return _controlSelector.ExistsPerLocalID(context, _localID);
    }
  }
}
