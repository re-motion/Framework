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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// Represents the method that will handle the <see cref="DomainObjectCollection.Adding"/>, <see cref="DomainObjectCollection.Added"/>, 
/// <see cref="DomainObjectCollection.Removed"/> and <see cref="DomainObjectCollection.Removing"/> events of a <see cref="DomainObjectCollection"/>.
/// </summary>
public delegate void DomainObjectCollectionChangeEventHandler (object sender, DomainObjectCollectionChangeEventArgs args);

/// <summary>
/// Provides data for the <see cref="DomainObjectCollection.Adding"/>, <see cref="DomainObjectCollection.Added"/>, 
/// <see cref="DomainObjectCollection.Removing"/> and <see cref="DomainObjectCollection.Removed"/> event of a <see cref="DomainObjectCollection"/>.
/// </summary>
[Serializable]
public class DomainObjectCollectionChangeEventArgs : EventArgs
{
  private IDomainObject _domainObject;

  /// <summary>
  /// Initializes a new instance of the <b>DomainObjectCollectionChangingEventArgs</b> class.
  /// </summary>
  /// <param name="domainObject">
  /// The <see cref="Remotion.Data.DomainObjects.IDomainObject"/> that is being added or removed to the collection. Must not be <see langword="null"/>.
  /// </param>
  /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
  public DomainObjectCollectionChangeEventArgs (IDomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull("domainObject", domainObject);
    _domainObject = domainObject;
  }

  /// <summary>
  /// Gets the <see cref="Remotion.Data.DomainObjects.IDomainObject"/> that is being added or removed.
  /// </summary>
  public IDomainObject DomainObject
  {
    get { return _domainObject; }
  }
}
}
