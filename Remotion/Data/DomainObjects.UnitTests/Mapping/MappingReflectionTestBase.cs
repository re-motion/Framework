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
using Moq;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class MappingReflectionTestBase
  {
    public const string DefaultStorageProviderID = "DefaultStorageProvider";
    public const string c_testDomainProviderID = "TestDomain";
    public const string c_nonPersistentTestDomainProviderID = "NonPersistentTestDomain";
    public const string c_unitTestStorageProviderStubID = "UnitTestStorageProviderStub";

    protected Mock<IClassIDProvider> ClassIDProviderStub { get; private set; }
    protected Mock<IDomainModelConstraintProvider> DomainModelConstraintProviderStub { get; private set; }
    protected Mock<IPropertyDefaultValueProvider> PropertyDefaultValueProviderStub { get; private set; }
    protected Mock<ISortExpressionDefinitionProvider> SortExpressionDefinitionProviderStub { get; private set; }
    protected ReflectionBasedMappingObjectFactory MappingObjectFactory { get; private set; }
    protected Mock<IDomainObjectCreator> DomainObjectCreatorStub { get; private set; }
    protected IPropertyMetadataProvider PropertyMetadataProvider { get; private set; }

    [SetUp]
    public virtual void SetUp ()
    {
      DomainObjectsConfiguration.SetCurrent(TestMappingConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent(TestMappingConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent(null);

      ClassIDProviderStub = new Mock<IClassIDProvider>();
      DomainModelConstraintProviderStub = new Mock<IDomainModelConstraintProvider>();
      PropertyDefaultValueProviderStub = new Mock<IPropertyDefaultValueProvider>();
      DomainObjectCreatorStub = new Mock<IDomainObjectCreator>();
      SortExpressionDefinitionProviderStub = new Mock<ISortExpressionDefinitionProvider>();
      PropertyMetadataProvider = new PropertyMetadataReflector();

      MappingObjectFactory = new ReflectionBasedMappingObjectFactory(
          Configuration.NameResolver,
          ClassIDProviderStub.Object,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub.Object,
          PropertyDefaultValueProviderStub.Object,
          SortExpressionDefinitionProviderStub.Object,
          DomainObjectCreatorStub.Object);
    }

    [TearDown]
    public virtual void TearDown ()
    {
    }

    [OneTimeSetUp]
    public virtual void OneTimeSetUp ()
    {
      TestMappingConfiguration.EnsureInitialized();
      DomainObjectsConfiguration.SetCurrent(TestMappingConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent(TestMappingConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent(null);
      FakeMappingConfiguration.Reset();
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return TestMappingConfiguration.Instance.GetDomainObjectIDs(); }
    }

    protected IMappingConfiguration Configuration
    {
      get { return MappingConfiguration.Current; }
    }

    protected StorageProviderDefinition TestDomainStorageProviderDefinition
    {
      get { return DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_testDomainProviderID]; }
    }

    protected StorageProviderDefinition UnitTestDomainStorageProviderDefinition
    {
      get { return DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_unitTestStorageProviderStubID]; }
    }
  }
}
