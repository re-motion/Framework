// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization.TestDomain;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization;

public abstract class SortingOptimizationTestBase
{
  protected MappingConfiguration GetMappingConfiguration ()
  {
    var storageSettings = SafeServiceLocator.Current.GetInstance<IStorageSettings>();
    var typeDiscoveryService = GetTypeDiscoveryService(GetType().Assembly);
    var mappingConfiguration = MappingConfiguration.Create(
        MappingReflectorObjectMother.CreateMappingReflector(typeDiscoveryService),
        new PersistenceModelLoader(storageSettings),
        SafeServiceLocator.Current.GetInstance<ISortingOptimizationNodeFactory>());
    return mappingConfiguration;
  }

  protected TableDefinition GetTableDefinition (ClassDefinition classDefinition)
  {
    return InlineRdbmsStorageEntityDefinitionVisitor.Visit<TableDefinition>(
        (IRdbmsStorageEntityDefinition)classDefinition.StorageEntityDefinition,
        (table, _) => table,
        (filterView, continuation) => continuation(filterView.BaseEntity),
        (unionView, _) => { throw new AssertionException($"Could not determine {nameof(TableDefinition)} for {classDefinition.ID}"); },
        (emptyView, _) => { throw new AssertionException($"Could not determine {nameof(TableDefinition)} for {classDefinition.ID}"); });
  }

  private static ITypeDiscoveryService GetTypeDiscoveryService (params Assembly[] rootAssemblies)
  {
    var rootAssemblyFinder = new FixedRootAssemblyFinder(rootAssemblies.Select(asm => new RootAssembly(asm, true)).ToArray());
    var assemblyLoader = new FilteringAssemblyLoader(ApplicationAssemblyLoaderFilter.Instance);
    var assemblyFinder = new CachingAssemblyFinderDecorator(new AssemblyFinder(rootAssemblyFinder, assemblyLoader));
    ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService(assemblyFinder);

    string[] whitelistedNamespaces = [typeof(SortingOptimizationDomainBase).Namespace];

    return new FilteringTypeDiscoveryService(
        typeDiscoveryService,
        type =>
        {
          var @namespace = type.Namespace ?? string.Empty;
          return whitelistedNamespaces.Any(t => @namespace.StartsWith(t));
        });
  }
}
