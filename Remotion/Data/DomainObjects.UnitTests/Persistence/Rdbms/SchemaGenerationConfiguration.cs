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
using System.Diagnostics;
using System.Reflection;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2014;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  public class SchemaGenerationConfiguration
  {
    private static SchemaGenerationConfiguration s_instance;

    public static SchemaGenerationConfiguration Instance
    {
      get
      {
        if (s_instance == null)
        {
          Debugger.Break();
          throw new InvalidOperationException("SchemaGenerationConfiguration has not been Initialized by invoking Initialize()");
        }
        return s_instance;
      }
    }

    public static void Initialize ()
    {
      s_instance = new SchemaGenerationConfiguration();
    }

    private readonly StorageConfiguration _storageConfiguration;
    private readonly MappingConfiguration _mappingConfiguration;
    private readonly FakeDomainObjectsConfiguration _domainObjectsConfiguration;

    public SchemaGenerationConfiguration ()
    {
      var sqlStorageObjectFactory = new SqlStorageObjectFactory();
      var storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>
                                                {
                                                    new RdbmsProviderDefinition(
                                                        DatabaseTest.SchemaGenerationFirstStorageProviderID,
                                                        sqlStorageObjectFactory,
                                                        DatabaseTest.SchemaGenerationConnectionString1),
                                                    new RdbmsProviderDefinition(
                                                        DatabaseTest.SchemaGenerationSecondStorageProviderID,
                                                        sqlStorageObjectFactory,
                                                        DatabaseTest.SchemaGenerationConnectionString2),
                                                    new RdbmsProviderDefinition(
                                                        DatabaseTest.SchemaGenerationThirdStorageProviderID,
                                                        sqlStorageObjectFactory,
                                                        DatabaseTest.SchemaGenerationConnectionString3),
                                                    new RdbmsProviderDefinition(
                                                        DatabaseTest.SchemaGenerationInternalStorageProviderID,
                                                        sqlStorageObjectFactory,
                                                        DatabaseTest.SchemaGenerationConnectionString1)
                                                };

      _storageConfiguration = new StorageConfiguration(
          storageProviderDefinitionCollection,
          storageProviderDefinitionCollection[DatabaseTest.SchemaGenerationFirstStorageProviderID]);
      _storageConfiguration.StorageGroups.Add(
          new StorageGroupElement(
              new FirstStorageGroupAttribute(),
              DatabaseTest.SchemaGenerationFirstStorageProviderID));
      _storageConfiguration.StorageGroups.Add(
          new StorageGroupElement(
              new SecondStorageGroupAttribute(),
              DatabaseTest.SchemaGenerationSecondStorageProviderID));
      _storageConfiguration.StorageGroups.Add(
          new StorageGroupElement(
              new ThirdStorageGroupAttribute(),
              DatabaseTest.SchemaGenerationThirdStorageProviderID));
      _storageConfiguration.StorageGroups.Add(
          new StorageGroupElement(
              new InternalStorageGroupAttribute(),
              DatabaseTest.SchemaGenerationInternalStorageProviderID));

      var typeDiscoveryService = GetTypeDiscoveryService(GetType().Assembly);

      _mappingConfiguration = new MappingConfiguration(
          MappingReflectorObjectMother.CreateMappingReflector(typeDiscoveryService),
          new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(_storageConfiguration)));
      _domainObjectsConfiguration = new FakeDomainObjectsConfiguration(_storageConfiguration);
    }

    public MappingConfiguration GetMappingConfiguration ()
    {
      return _mappingConfiguration;
    }

    public StorageConfiguration GetPersistenceConfiguration ()
    {
      return _storageConfiguration;
    }

    public FakeDomainObjectsConfiguration GetDomainObjectsConfiguration ()
    {
      return _domainObjectsConfiguration;
    }

    public ITypeDiscoveryService GetTypeDiscoveryService (params Assembly[] rootAssemblies)
    {
      var baseTypeDiscoveryService = new FixedTypeDiscoveryService(Assembly.GetExecutingAssembly().GetTypes());

      return FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist(
          baseTypeDiscoveryService,
          "Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain");
    }

  }
}
