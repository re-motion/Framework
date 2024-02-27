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
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Reflection;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Security;
using Remotion.Security.Development;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Persistence;
using Remotion.ServiceLocation;
using Remotion.Utilities;

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
        var securityProvider = new FakeSecurityProvider();
        var storageSettingsFactory = StorageSettingsFactory.CreateForSqlServer<SecurityManagerSqlStorageObjectFactory>(TestDomainConnectionString);

        var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
        serviceLocator.RegisterSingle<ISecurityProvider>(() => securityProvider);
        serviceLocator.RegisterSingle<IPrincipalProvider>(() => new NullPrincipalProvider());
        serviceLocator.RegisterSingle(() => storageSettingsFactory);
        ServiceLocator.SetLocatorProvider(() => serviceLocator);

        var rootAssemblyFinder = new FixedRootAssemblyFinder(new RootAssembly(typeof(BaseSecurityManagerObject).Assembly, true));
        var assemblyLoader = new FilteringAssemblyLoader(ApplicationAssemblyLoaderFilter.Instance);
        var assemblyFinder = new CachingAssemblyFinderDecorator(new AssemblyFinder(rootAssemblyFinder, assemblyLoader));
        ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService(assemblyFinder);

        MappingConfiguration.SetCurrent(
            MappingConfiguration.Create(
                new MappingReflector(
                    typeDiscoveryService,
                    SafeServiceLocator.Current.GetInstance<IClassIDProvider>(),
                    SafeServiceLocator.Current.GetInstance<IMemberInformationNameResolver>(),
                    SafeServiceLocator.Current.GetInstance<IPropertyMetadataProvider>(),
                    SafeServiceLocator.Current.GetInstance<IDomainModelConstraintProvider>(),
                    SafeServiceLocator.Current.GetInstance<IPropertyDefaultValueProvider>(),
                    SafeServiceLocator.Current.GetInstance<ISortExpressionDefinitionProvider>(),
                    SafeServiceLocator.Current.GetInstance<IDomainObjectCreator>()),
                SafeServiceLocator.Current.GetInstance<IPersistenceModelLoader>()));

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
