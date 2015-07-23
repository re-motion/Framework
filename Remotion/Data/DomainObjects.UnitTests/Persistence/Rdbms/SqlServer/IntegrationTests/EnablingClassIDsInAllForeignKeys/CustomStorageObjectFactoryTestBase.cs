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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.EnablingClassIDsInAllForeignKeys
{
  public abstract class CustomStorageObjectFactoryTestBase
  {
    private SqlStorageObjectFactory _storageObjectFactory;
    private RdbmsProviderDefinition _storageProviderDefinition;
    private MappingConfiguration _mappingConfiguration;

    [SetUp]
    public void SetUp ()
    {
      var mappingLoader = new MappingReflector (
          new FixedTypeDiscoveryService (GetReflectedTypes()),
          new ClassIDProvider(),
          new ReflectionBasedMemberInformationNameResolver(),
          new PropertyMetadataReflector(),
          new DomainModelConstraintProvider(),
          new ThrowingDomainObjectCreator());
      _storageObjectFactory = CreateSqlStorageObjectFactory ();
      var storageProviderDefinitionFinderStub = MockRepository.GenerateStub<IStorageProviderDefinitionFinder>();
      _storageProviderDefinition = new RdbmsProviderDefinition ("test", _storageObjectFactory, DatabaseTest.TestDomainConnectionString);
      storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (Arg<ClassDefinition>.Is.Anything, Arg<string>.Is.Anything))
          .Return (_storageProviderDefinition);
      var persistenceModelLoader = _storageObjectFactory.CreatePersistenceModelLoader (_storageProviderDefinition, storageProviderDefinitionFinderStub);
      _mappingConfiguration = new MappingConfiguration (mappingLoader, persistenceModelLoader);

      MappingConfiguration.SetCurrent (_mappingConfiguration);
    }

    [TearDown]
    public void TearDown ()
    {
      MappingConfiguration.SetCurrent (null);
    }

    protected MappingConfiguration MappingConfiguration
    {
      get { return _mappingConfiguration; }
    }

    protected IRdbmsStorageObjectFactory StorageObjectFactory
    {
      get { return _storageObjectFactory; }
    }

    protected RdbmsProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    protected abstract SqlStorageObjectFactory CreateSqlStorageObjectFactory ();

    protected virtual Type[] GetReflectedTypes ()
    {
      var thisType = GetType();
      var testDomainNamespace = thisType.Namespace + ".TestDomain";
      return thisType.Assembly.GetTypes().Where (t => t.Namespace != null && t.Namespace.StartsWith (testDomainNamespace)).ToArray();
    }

    protected RelationEndPointDefinition GetRelationEndPointDefinition<TSource, TRelated> (Expression<Func<TSource, TRelated>> propertyAccessExpression)
    {
      var typeDefinition = MappingConfiguration.GetTypeDefinition (typeof (TSource));
      var propertyInfoAdapter = PropertyInfoAdapter.Create (NormalizingMemberInfoFromExpressionUtility.GetProperty (propertyAccessExpression));
      return (RelationEndPointDefinition) typeDefinition.ResolveRelationEndPoint (propertyInfoAdapter);
    }
  }
}