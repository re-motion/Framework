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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Reflection;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class StandardConfiguration
  {
    public const string ConnectionString = "Integrated Security=SSPI;Initial Catalog=PerformanceTestDomain;Data Source=localhost";

    public static void Initialize ()
    {
      var storageSettingsFactory = StorageSettingsFactory.CreateForSqlServer(ConnectionString);

      var defaultServiceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      defaultServiceLocator.RegisterSingle(() => storageSettingsFactory);

      ServiceLocator.SetLocatorProvider(() => defaultServiceLocator);

      var rootAssemblyFinder = new FixedRootAssemblyFinder(new RootAssembly(typeof(StandardConfiguration).Assembly, true));
      var assemblyLoader = new FilteringAssemblyLoader(ApplicationAssemblyLoaderFilter.Instance);
      var assemblyFinder = new CachingAssemblyFinderDecorator(new AssemblyFinder(rootAssemblyFinder, assemblyLoader));
      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService(assemblyFinder);
      MappingConfiguration mappingConfiguration = MappingConfiguration.Create(
          new MappingReflector(
              typeDiscoveryService,
              SafeServiceLocator.Current.GetInstance<IClassIDProvider>(),
              SafeServiceLocator.Current.GetInstance<IMemberInformationNameResolver>(),
              SafeServiceLocator.Current.GetInstance<IPropertyMetadataProvider>(),
              SafeServiceLocator.Current.GetInstance<IDomainModelConstraintProvider>(),
              SafeServiceLocator.Current.GetInstance<IPropertyDefaultValueProvider>(),
              SafeServiceLocator.Current.GetInstance<ISortExpressionDefinitionProvider>(),
              SafeServiceLocator.Current.GetInstance<IDomainObjectCreator>()),
         SafeServiceLocator.Current.GetInstance<IPersistenceModelLoader>());
      MappingConfiguration.SetCurrent(mappingConfiguration);
    }
  }
}
