// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Remotion.Context;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;

namespace Remotion.Data.DomainObjects.Persistence.SortingOptimization;

/// <summary>
/// <see cref="LazyPersistenceModelSortingProviderWrapper"/> wraps another instance of <see cref="IPersistenceModelSortingProvider"/>
/// and delegates all calls to the wrapped instance. But <see cref="Initialize"/> is executed in a separate thread so that initialization
/// does not block the current thread. All other calls wait till the initialization is finished.
/// </summary>
public class LazyPersistenceModelSortingProviderWrapper : IPersistenceModelSortingProvider
{
  private Task<IPersistenceModelSortingProvider>? _initializeWrappedProviderTask;
  private Func<IReadOnlyList<ClassDefinition>, IPersistenceModelSortingProvider>? _wrappedProviderInitFunc;
  private readonly object _lockObject = new();

  public LazyPersistenceModelSortingProviderWrapper (IPersistenceModelSortingProvider wrappedProvider)
  {
    ArgumentNullException.ThrowIfNull(wrappedProvider);

    _wrappedProviderInitFunc = (classDefinitions) =>
    {
      wrappedProvider.Initialize(classDefinitions);
      return wrappedProvider;
    };
  }

  public void Initialize (IReadOnlyList<ClassDefinition> classDefinitions)
  {
    ArgumentNullException.ThrowIfNull(classDefinitions);

    lock (_lockObject)
    {
      if (_initializeWrappedProviderTask != null)
        throw new InvalidOperationException($"{nameof(Initialize)} has already been called.");
      _initializeWrappedProviderTask = SafeContext.Task.Run(() =>
      {
        var result = _wrappedProviderInitFunc!(classDefinitions);
        _wrappedProviderInitFunc = null;
        return result;
      });
    }
  }

  public int GetSortPosition (IStorageEntityDefinition storageEntityDefinition)
  {
    ArgumentNullException.ThrowIfNull(storageEntityDefinition);

    return WaitAndGetInitializedWrappedProvider().GetSortPosition(storageEntityDefinition);
  }

  public int GetSortPosition (ClassDefinition classDefinition)
  {
    ArgumentNullException.ThrowIfNull(classDefinition);

    return WaitAndGetInitializedWrappedProvider().GetSortPosition(classDefinition);
  }

  public IReadOnlyCollection<SortingOptimizationObjectIDPropertySpecification> GetPropertySpecificationsForForeignKeyProperties (ClassDefinition classDefinition)
  {
    ArgumentNullException.ThrowIfNull(classDefinition);

    return WaitAndGetInitializedWrappedProvider().GetPropertySpecificationsForForeignKeyProperties(classDefinition);
  }

  private IPersistenceModelSortingProvider WaitAndGetInitializedWrappedProvider ()
  {
    var initializeWrappedProviderTask = _initializeWrappedProviderTask;
    if (initializeWrappedProviderTask == null)
    {
      lock (_lockObject)
      {
        initializeWrappedProviderTask = _initializeWrappedProviderTask;
        if (initializeWrappedProviderTask == null)
          throw new InvalidOperationException($"Before accessing any methods, {nameof(Initialize)} has to be called.");
      }
    }

    initializeWrappedProviderTask.Wait();
    return initializeWrappedProviderTask.Result;
  }
}
