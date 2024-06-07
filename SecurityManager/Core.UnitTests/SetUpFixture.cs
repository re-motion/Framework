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

        var serviceLocator = DefaultServiceLocator.Create();
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
                MappingReflector.Create(
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
