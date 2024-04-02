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
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;

/// <summary>
/// Defines an API for resolving the <see cref="StorageAccessType"/>s used by the <see cref="IPersistenceStrategy"/>.
/// </summary>
/// <remarks>
/// To override the default <see cref="StorageAccessType"/>s defined in <see cref="DefaultStorageAccessResolver"/>, a new implementation of <see cref="IStorageAccessResolver"/>
/// should be created and injected into IoC.
/// </remarks>
public interface IStorageAccessResolver
{
  /// <summary>
  /// Gets the type of access an <see cref="IPersistenceStrategy"/> uses for its <see cref="IPersistenceStrategy.LoadObjectData(Remotion.Data.DomainObjects.ObjectID)"/> operation.
  /// </summary>
  StorageAccessType ResolveStorageAccessForLoadingDomainObjectsByObjectID ();

  /// <summary>
  /// Gets the type of access an <see cref="IPersistenceStrategy"/> uses for its <see cref="IPersistenceStrategy.ResolveObjectRelationData"/> and
  /// <see cref="IPersistenceStrategy.ResolveCollectionRelationData"/> operations.
  /// </summary>
  StorageAccessType ResolveStorageAccessForLoadingDomainObjectRelation ();

  /// <summary>
  /// Gets the type of access an <see cref="IPersistenceStrategy"/> uses for its <see cref="IPersistenceStrategy.ExecuteCollectionQuery"/>,
  /// <see cref="IPersistenceStrategy.ExecuteCustomQuery"/>, and <see cref="IFetchEnabledPersistenceStrategy.ExecuteFetchQuery"/> operations.
  /// </summary>
  StorageAccessType ResolveStorageAccessForQuery (IQuery query);
}
