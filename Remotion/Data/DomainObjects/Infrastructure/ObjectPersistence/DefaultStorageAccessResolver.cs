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
using Remotion.Data.DomainObjects.Queries;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Always requests <see cref="StorageAccessType.ReadWrite"/> for every operation.
  /// </summary>
  [ImplementationFor(typeof(IStorageAccessResolver), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Single)]
  [Serializable]
  public class DefaultStorageAccessResolver : IStorageAccessResolver
  {
    public StorageAccessType ResolveStorageAccessForLoadingDomainObjectsByObjectID ()
    {
      return StorageAccessType.ReadWrite;
    }

    public StorageAccessType ResolveStorageAccessForLoadingDomainObjectRelation ()
    {
      return StorageAccessType.ReadWrite;
    }

    public StorageAccessType ResolveStorageAccessForQuery (IQuery query)
    {
      return StorageAccessType.ReadWrite;
    }
  }
}
