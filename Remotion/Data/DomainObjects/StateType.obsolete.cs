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
  [Obsolete ("Use DomainObjectState or DataContainerState instead. (Version: 1.21.8)", true)]
  public enum StateType
  {
    /// <summary>
    /// The <see cref="DomainObject"/> has not changed since it was loaded.
    /// </summary>
    [Obsolete ("Use DomainObject.State.IsUnchanged or DataContainer.State.IsUnchanged instead. (Version: 1.21.8)", true)]
    Unchanged = 0,
    /// <summary>
    /// The <see cref="DomainObject"/> has been changed since it was loaded.
    /// </summary>
    [Obsolete ("Use DomainObject.State.IsChanged or DataContainer.State.IsChanged instead. (Version: 1.21.8)", true)]
    Changed = 1,
    /// <summary>
    /// The <see cref="DomainObject"/> has been instanciated and has not been committed.
    /// </summary>
    [Obsolete ("Use DomainObject.State.IsNew or DataContainer.State.IsNew instead. (Version: 1.21.8)", true)]
    New = 2,
    /// <summary>
    /// The <see cref="DomainObject"/> has been deleted.
    /// </summary>
    [Obsolete ("Use DomainObject.State.IsDeleted or DataContainer.State.IsDeleted instead. (Version: 1.21.8)", true)]
    Deleted = 3,
    /// <summary>
    /// The <see cref="DomainObject"/> reference is no longer or not yet valid for use in this transaction.
    /// </summary>
    [Obsolete ("Use DomainObject.State.IsInvalid or DataContainer.State.IsDiscarded instead. (Version: 1.21.8)", true)]
    Invalid = 4,
    /// <summary>
    /// The <see cref="DomainObject"/>'s data has not been loaded yet into the <see cref="ClientTransaction"/>. It will be loaded when needed,
    /// e.g. when a property value or relation is accessed, or when 
    /// <see cref="DomainObjectExtensions.EnsureDataAvailable"/> is called for the <see cref="IDomainObject"/>.
    /// </summary>
    [Obsolete ("Use DomainObject.State.IsNotLoadedYet instead. (Version: 1.21.8)", true)]
    NotLoadedYet = 5,
  }
}
