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

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of nodes, e.g. a web tree view.
  /// </summary>
  /// <seealso cref="T:Remotion.Web.Development.WebTesting.ControlObjects.WebTreeViewControlObject"/>
  public interface IControlObjectWithNodes<TNodeControlObject>
      where TNodeControlObject : ControlObject
  {
    /// <summary>
    /// Start of the fluent interface for selecting a node.
    /// </summary>
    IFluentControlObjectWithNodes<TNodeControlObject> GetNode ();

    /// <summary>
    /// Short for explicitly implemented <see cref="IFluentControlObjectWithNodes{TCellControlObject}.WithItemID"/>.
    /// </summary>
    TNodeControlObject GetNode ([NotNull] string itemID);

    /// <summary>
    /// Short for explicitly implemented <see cref="IFluentControlObjectWithNodes{TNodeControlObject}.WithIndex"/>.
    /// </summary>
    TNodeControlObject GetNode (int index);
  }

  /// <summary>
  /// Fluent interface for completing the <see cref="IControlObjectWithNodes{TNodeControlObject}.GetNode()"/> call.
  /// </summary>
  public interface IFluentControlObjectWithNodes<TNodeControlObject>
      where TNodeControlObject : ControlObject
  {
    /// <summary>
    /// Selects the node using the given <paramref name="itemID"/>.
    /// </summary>
    TNodeControlObject WithItemID ([NotNull] string itemID);

    /// <summary>
    /// Selects the node using the given <paramref name="index"/>.
    /// </summary>
    TNodeControlObject WithIndex (int index);

    /// <summary>
    /// Selects the node using the given <paramref name="displayText"/>.
    /// </summary>
    TNodeControlObject WithDisplayText ([NotNull] string displayText);

    /// <summary>
    /// Selects the node using the given <paramref name="containsDisplayText"/>.
    /// </summary>
    TNodeControlObject WithDisplayTextContains ([NotNull] string containsDisplayText);
  }
}