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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.IntegrationTests
{
  public class ValidationIntegrationTestBase
  {
    [OneTimeSetUp]
    public void OneTimeSetup ()
    {
      StandardConfiguration.EnsureInitialized();
    }

    protected void ValidateMapping (string testDomainNamespaceSuffix)
    {
      var testDomainNamespace = "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration." + testDomainNamespaceSuffix;
      var typeDiscoveryService = GetTypeDiscoveryService(testDomainNamespace, GetType().Assembly);
      Assert.That(typeDiscoveryService.GetTypes(typeof(DomainObject), true), Is.Not.Empty, "Namespace '{0}' has no DomainObjects.", testDomainNamespaceSuffix);
      new MappingConfiguration(
          MappingReflectorObjectMother.CreateMappingReflector(typeDiscoveryService),
          new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(StandardConfiguration.Instance.GetPersistenceConfiguration())));
    }

    private ITypeDiscoveryService GetTypeDiscoveryService (string testDomainNamespace, params Assembly[] rootAssemblies)
    {
      var rootAssemblyFinder = new FixedRootAssemblyFinder(rootAssemblies.Select(asm => new RootAssembly(asm, true)).ToArray());
      var assemblyLoader = new FilteringAssemblyLoader(ApplicationAssemblyLoaderFilter.Instance);
      var assemblyFinder = new CachingAssemblyFinderDecorator(new AssemblyFinder(rootAssemblyFinder, assemblyLoader));
      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService(assemblyFinder);

      return FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist(typeDiscoveryService, testDomainNamespace);
    }
  }
}
