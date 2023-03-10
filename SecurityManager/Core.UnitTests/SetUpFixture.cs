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
using System.ComponentModel.Design;
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Reflection;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Persistence;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    public static string TestDomainConnectionString
    {
      get { return DatabaseConfiguration.UpdateConnectionString("Initial Catalog=RemotionSecurityManager"); }
    }

    public static string MasterConnectionString
    {
      get { return DatabaseConfiguration.UpdateConnectionString("Initial Catalog=master"); }
    }

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      try
      {
        var serviceLocator = DefaultServiceLocator.Create();
        serviceLocator.RegisterSingle<ISecurityProvider>(() => new NullSecurityProvider());
        serviceLocator.RegisterSingle<IPrincipalProvider>(() => new NullPrincipalProvider());
        ServiceLocator.SetLocatorProvider(() => serviceLocator);

        var providers = new ProviderCollection<StorageProviderDefinition>();
        providers.Add(new RdbmsProviderDefinition("SecurityManager", new SecurityManagerSqlStorageObjectFactory(), TestDomainConnectionString));
        var storageConfiguration = new StorageConfiguration(providers, providers["SecurityManager"]);
        storageConfiguration.StorageGroups.Add(new StorageGroupElement(new SecurityManagerStorageGroupAttribute(), "SecurityManager"));

        DomainObjectsConfiguration.SetCurrent(new FakeDomainObjectsConfiguration(storage: storageConfiguration));

        var rootAssemblyFinder = new FixedRootAssemblyFinder(new RootAssembly(typeof(BaseSecurityManagerObject).Assembly, true));
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
                    MappingReflector.CreateDomainObjectCreator()),
                new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage))));

        SqlConnection.ClearAllPools();

        DatabaseAgent masterAgent = new DatabaseAgent(MasterConnectionString);
        masterAgent.ExecuteBatchFile("SecurityManagerCreateDB.sql", false, DatabaseConfiguration.GetReplacementDictionary());
        DatabaseAgent databaseAgent = new DatabaseAgent(TestDomainConnectionString);
        databaseAgent.ExecuteBatchFile("SecurityManagerSetupDB.sql", true, DatabaseConfiguration.GetReplacementDictionary());
        databaseAgent.ExecuteBatchFile("SecurityManagerSetupConstraints.sql", true, DatabaseConfiguration.GetReplacementDictionary());
        databaseAgent.ExecuteBatchFile("SecurityManagerSetupDBSpecialTables.sql", true, DatabaseConfiguration.GetReplacementDictionary());
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown ()
    {
      SqlConnection.ClearAllPools();
    }
  }
}
