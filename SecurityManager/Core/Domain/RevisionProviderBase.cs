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
using Remotion.Collections;
using Remotion.Context;
using Remotion.Data.DomainObjects;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  /// <threadsafety static="true" instance="true"/>
  public abstract class RevisionProviderBase<TRevisionKey> : IRevisionProvider<TRevisionKey, GuidRevisionValue>
      where TRevisionKey : IRevisionKey
  {
    private readonly SafeContextSingleton<SimpleDataStore<TRevisionKey, GuidRevisionValue>> _cachedRevisions;

    protected RevisionProviderBase ()
    {
      //RM-5640: Rewrite with tests
      // Inject an invalidation object where implementations can register for an invalidation of all cached revisions.
      // While this is a potential memory leak, the revision providers are used with singleton-semantics anyway when instantiated via IoC,
      // so this shouldn't be an issue.

      _cachedRevisions = new SafeContextSingleton<SimpleDataStore<TRevisionKey, GuidRevisionValue>> (
          SafeContextKeys.SecurityManagerRevision + "_" + Guid.NewGuid().ToString(),
          () => new SimpleDataStore<TRevisionKey, GuidRevisionValue>());
    }

    public GuidRevisionValue GetRevision (TRevisionKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      var revisions = GetCachedRevisions();
      return revisions.GetOrCreateValue (key, GetRevisionFromDatabase);
    }

    public void InvalidateRevision (TRevisionKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      var revisions = GetCachedRevisions();
      revisions.Remove (key);
    }

    public void InvalidateAllRevisions ()
    {
      var revisions = GetCachedRevisions();
      revisions.Clear();
    }

    private SimpleDataStore<TRevisionKey, GuidRevisionValue> GetCachedRevisions ()
    {
      return _cachedRevisions.Current;
    }

    private GuidRevisionValue GetRevisionFromDatabase (TRevisionKey key)
    {
      var value = (Guid?) ClientTransaction.CreateRootTransaction().QueryManager.GetScalar (Revision.GetGetRevisionQuery (key));

      if (value.HasValue)
        return new GuidRevisionValue (value.Value);

      return new GuidRevisionValue (Guid.Empty);
    }

  }
}