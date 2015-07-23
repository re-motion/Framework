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

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Indicates the state of a <see cref="DomainObject"/>.
  /// </summary>
  public enum StateType
  {
    /// <summary>
    /// The <see cref="DomainObject"/> has not changed since it was loaded.
    /// </summary>
    Unchanged = 0,
    /// <summary>
    /// The <see cref="DomainObject"/> has been changed since it was loaded.
    /// </summary>
    Changed = 1,
    /// <summary>
    /// The <see cref="DomainObject"/> has been instanciated and has not been committed.
    /// </summary>
    New = 2,
    /// <summary>
    /// The <see cref="DomainObject"/> has been deleted.
    /// </summary>
    Deleted = 3,
    /// <summary>
    /// The <see cref="DomainObject"/> reference is no longer or not yet valid for use in this transaction.
    /// </summary>
    Invalid = 4,
    /// <summary>
    /// The <see cref="DomainObject"/>'s data has not been loaded yet into the <see cref="ClientTransaction"/>. It will be loaded when needed,
    /// e.g. when a property value or relation is accessed, or when 
    /// <see cref="DomainObjectExtensions.EnsureDataAvailable"/> is called for the <see cref="IDomainObject"/>.
    /// </summary>
    NotLoadedYet = 5,
    [Obsolete ("This state is now called Invalid. (1.13.60)", true)]
    Discarded = 4
  }
}