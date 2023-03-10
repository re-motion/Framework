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
using System.ComponentModel.Design;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2014;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects.TestDomain;
using Remotion.Reflection;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects
{
  [SetUpFixture]
  public class SetUpFixture
  {
    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      try
      {
        ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition>();
        providers.Add(new RdbmsProviderDefinition("Development.Data.DomainObjects", new SqlStorageObjectFactory(), "ConnectionString"));
        StorageConfiguration storageConfiguration = new StorageConfiguration(providers, providers["Development.Data.DomainObjects"]);

        DomainObjectsConfiguration.SetCurrent(
            new FakeDomainObjectsConfiguration(
                new MappingLoaderConfiguration(),
                storageConfiguration,
                new QueryConfiguration()));

        var rootAssemblyFinder = new FixedRootAssemblyFinder(new RootAssembly(typeof(TestDomainObject).Assembly, true));
        var assemblyLoader = new FilteringAssemblyLoader(ApplicationAssemblyLoaderFilter.Instance);
        var assemblyFinder = new CachingAssemblyFinderDecorator(new AssemblyFinder(rootAssemblyFinder, assemblyLoader));
        ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService(assemblyFinder);

        MappingConfiguration.SetCurrent(
            new MappingConfiguration(
                new MappingReflector(
                    typeDiscoveryService,
                    new ClassIDProvider(),
                    new ReflectionBasedMemberInformationNameResolver(),
                    new PropertyMetadataReflector(),
                    new DomainModelConstraintProvider(),
                    new LegacyPropertyDefaultValueProvider(),
                    new SortExpressionDefinitionProvider(),
                    new ThrowingDomainObjectCreator()),
                new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage))));
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }
  }
}
