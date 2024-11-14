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
using System.Threading;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  public abstract class RepositoryBase<TData, TRevisionKey, TRevisionValue>
      where TData : RepositoryBase<TData, TRevisionKey, TRevisionValue>.RevisionBasedData
      where TRevisionKey : IRevisionKey
      where TRevisionValue : IRevisionValue
  {
    // analyze if .NET 4.0 MemCache etc would be useful when refactoring the infrastructure into caching library

    public abstract class RevisionBasedData
    {
      private readonly TRevisionValue _revision;

      protected RevisionBasedData (TRevisionValue revision)
      {
        _revision = revision;
      }

      public TRevisionValue Revision
      {
        get { return _revision; }
      }
    }

    protected enum Revision
    {
      Stale,
      Invalidate
    }

    private readonly IRevisionProvider<TRevisionKey, TRevisionValue> _revisionProvider;
    private readonly object _syncRoot = new object();
    private TData? _cachedData;

    protected RepositoryBase (IRevisionProvider<TRevisionKey, TRevisionValue> revisionProvider)
    {
      ArgumentUtility.CheckNotNull("revisionProvider", revisionProvider);

      _revisionProvider = revisionProvider;
    }

    protected abstract TData LoadData (TRevisionValue revision);

    protected TData GetCachedData (TRevisionKey revisionKey, Revision revision = Revision.Stale)
    {
      ArgumentUtility.CheckNotNull("revisionKey", revisionKey);

      if (revision == Revision.Invalidate)
        _revisionProvider.InvalidateRevision(revisionKey);

      var currentRevision = _revisionProvider.GetRevision(revisionKey);

      // Volatile access at this point is not actually required, but only added for completeness.
      // If the cached data happens to be stale, the revision-check would indicate staleness 
      // and the subsequent the synchronized access would then force the use of the current value for the second check.
      var localData = Volatile.Read(ref _cachedData);

      if (localData == null || !localData.Revision.IsCurrent(currentRevision))
      {
        lock (_syncRoot)
        {
          if (_cachedData == null || !_cachedData.Revision.IsCurrent(currentRevision))
            _cachedData = LoadData(currentRevision);
          localData = _cachedData;
        }
      }
      return localData;
    }
  }
}
