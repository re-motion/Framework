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

namespace Remotion.SecurityManager.SecurityProvider.Implementation
{
  public sealed class SecurityContextCache : RepositoryBase<SecurityContextCache.Data, RevisionKey, GuidRevisionValue>
  {
    public sealed class Data : RevisionBasedData
    {
      private readonly ICache<ISecurityPrincipal, AccessTypeCache> _items;

      internal Data (GuidRevisionValue revision)
          : base(revision)
      {
        _items = CacheFactory.CreateWithSynchronization<ISecurityPrincipal, AccessTypeCache>();
      }

      public ICache<ISecurityPrincipal, AccessTypeCache> Items
      {
        get { return _items; }
      }
    }

    //TODO RM-5521: test, implement ICache-interface with delegating members to "Items", generalize as RevisionBasedCache<TKey, TRevisionProvider>
    private readonly RevisionKey _revisionKey = new RevisionKey();

    public SecurityContextCache (IRevisionProvider<RevisionKey, GuidRevisionValue> revisionProvider)
        : base(revisionProvider)
    {
    }

    public ICache<ISecurityPrincipal, AccessTypeCache> Items
    {
      get { return GetCachedData(_revisionKey).Items; }
    }

    protected override Data LoadData (GuidRevisionValue revision)
    {
      return new Data(revision);
    }
  }
}
