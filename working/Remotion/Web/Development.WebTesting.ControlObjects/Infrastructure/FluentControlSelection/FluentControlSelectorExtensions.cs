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
using Remotion.Web.Development.WebTesting.ControlSelection;

// ReSharper disable once CheckNamespace (assembly should have a more general name like ".Remotion", however, we have not found a good name yet)
namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Default extension methods for all re-motion-provided <see cref="IControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>
  /// implementations.
  /// </summary>
  public static class FluentControlSelectorExtensions
  {
    /// <summary>
    /// Extension method for selecting a control by item ID (using the
    /// <see cref="ItemIDControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>).
    /// </summary>
    public static TControlObject ByItemID<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string itemID)
        where TControlSelector : IItemIDControlSelector<TControlObject> where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return fluentControlSelector.GetControl (new ItemIDControlSelectionCommandBuilder<TControlSelector, TControlObject> (itemID));
    }
  }
}