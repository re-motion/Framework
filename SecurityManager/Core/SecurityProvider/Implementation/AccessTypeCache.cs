// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Collections.Caching;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.SecurityProvider.Implementation
{
  public sealed class AccessTypeCache : RepositoryBase<AccessTypeCache.Data, UserRevisionKey, GuidRevisionValue>
  {
    public sealed class Data : RevisionBasedData
    {
      private readonly ICache<ISecurityContext, AccessType[]> _items;

      internal Data (GuidRevisionValue revision)
          : base(revision)
      {
        _items = CacheFactory.CreateWithSynchronization<ISecurityContext, AccessType[]>();
      }

      public ICache<ISecurityContext, AccessType[]> Items
      {
        get { return _items; }
      }
    }

    //TODO RM-5521: test, implement ICache-interface with delegating members to "Items", generalize as RevisionBasedCache<TKey, TRevisionProvider>
    private readonly UserRevisionKey _revisionKey;

    public AccessTypeCache (IRevisionProvider<UserRevisionKey, GuidRevisionValue> revisionProvider, string userName)
        : base(revisionProvider)
    {
      ArgumentUtility.CheckNotNullOrEmpty("userName", userName);

      _revisionKey = new UserRevisionKey(userName);
    }

    public ICache<ISecurityContext, AccessType[]> Items
    {
      get { return GetCachedData(_revisionKey).Items; }
    }

    protected override Data LoadData (GuidRevisionValue revision)
    {
      return new Data(revision);
    }
  }
}
