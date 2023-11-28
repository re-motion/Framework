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
using System.Linq.Expressions;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Database;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.Utilities;


namespace Remotion.Data.DomainObjects.UnitTests
{
  public abstract class StandardMappingTest : DatabaseTest
  {
    public const string CreateTestDataFileName = "Database\\DataDomainObjects_CreateTestData.sql";

    protected ServiceLocatorScope Scope;

    protected StandardMappingTest ()
        : base(new StandardMappingDatabaseAgent(TestDomainConnectionString), CreateTestDataFileName)
    {
    }

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();


      var defaultServiceLocator = DefaultServiceLocator.Create();
      defaultServiceLocator.RegisterSingle(() => StandardConfiguration.Instance.GetStorageSettings());

      Scope = new ServiceLocatorScope(defaultServiceLocator);

      MappingConfiguration.SetCurrent(StandardConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent(null);
    }

    public override void SetUp ()
    {
      base.SetUp();

      MappingConfiguration.SetCurrent(StandardConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent(null);

    }

    public override void TearDown ()
    {
      DomainObjectsConfiguration.SetCurrent(null);
      MappingConfiguration.SetCurrent(null);
      ConfigurationWrapper.SetCurrent(null);

      base.TearDown();
    }

    public override void TestFixtureTearDown ()
    {
      DomainObjectsConfiguration.SetCurrent(null);
      MappingConfiguration.SetCurrent(null);
      ConfigurationWrapper.SetCurrent(null);

      Scope.Dispose();

      base.TestFixtureTearDown();
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return StandardConfiguration.Instance.GetDomainObjectIDs(); }
    }

    protected IMappingConfiguration Configuration
    {
      get { return MappingConfiguration.Current; }
    }

    protected IQueryDefinitionRepository Queries
    {
      get { return StandardConfiguration.Instance.GetQueries(); }
    }

    protected IStorageSettings StorageSettings
    {
      get { return StandardConfiguration.Instance.GetStorageSettings(); }
    }

    protected RdbmsProviderDefinition TestDomainStorageProviderDefinition
    {
      get { return (RdbmsProviderDefinition)StandardConfiguration.Instance.GetStorageSettings().GetStorageProviderDefinition(c_testDomainProviderID); }
    }

    protected NonPersistentProviderDefinition NonPersistentStorageProviderDefinition
    {
      get { return (NonPersistentProviderDefinition)StandardConfiguration.Instance.GetStorageSettings().GetStorageProviderDefinition(c_nonPersistentTestDomainProviderID); }
    }

    protected UnitTestStorageProviderStubDefinition UnitTestStorageProviderDefinition
    {
      get
      {
        return (UnitTestStorageProviderStubDefinition)StandardConfiguration.Instance.GetStorageSettings().GetStorageProviderDefinition(c_unitTestStorageProviderStubID);
      }
    }

    protected PropertyDefinition GetPropertyDefinition (Type declaringType, string shortPropertyName)
    {
      var propertyDefinition = GetTypeDefinition(declaringType)
          .PropertyAccessorDataCache
          .GetMandatoryPropertyAccessorData(declaringType, shortPropertyName)
          .PropertyDefinition;
      Assertion.IsNotNull(propertyDefinition, "Property '{0}.{1}' is not a mapped property.", declaringType, shortPropertyName);
      return propertyDefinition;
    }

    protected PropertyDefinition GetPropertyDefinition (Type classType, Type declaringType, string shortPropertyName)
    {
      var propertyDefinition = GetTypeDefinition(classType)
          .PropertyAccessorDataCache
          .GetMandatoryPropertyAccessorData(declaringType, shortPropertyName)
          .PropertyDefinition;
      Assertion.IsNotNull(propertyDefinition, "Property '{0}.{1}' is not a mapped property.", declaringType, shortPropertyName);
      return propertyDefinition;
    }

    protected IRelationEndPointDefinition GetSomeEndPointDefinition ()
    {
      return GetEndPointDefinition(typeof(Order), "OrderItems");
    }

    protected IRelationEndPointDefinition GetEndPointDefinition (Type classType, string shortPropertyName)
    {
      return GetEndPointDefinition(classType, classType, shortPropertyName);
    }

    protected IRelationEndPointDefinition GetEndPointDefinition (Type classType, Type propertyDeclaringType, string shortPropertyName)
    {
      var endPointDefinition = GetTypeDefinition(classType)
          .PropertyAccessorDataCache
          .GetMandatoryPropertyAccessorData(propertyDeclaringType, shortPropertyName)
          .RelationEndPointDefinition;
      Assertion.IsNotNull(
          endPointDefinition, "Property '{0}.{1}' is not a relation end-point on '{2}'.", propertyDeclaringType, shortPropertyName, classType.Name);
      return endPointDefinition;
    }

    protected RelationEndPointDefinition GetNonVirtualEndPointDefinition (Type declaringType, string shortPropertyName)
    {
      return (RelationEndPointDefinition)GetEndPointDefinition(declaringType, shortPropertyName);
    }

    protected RelationDefinition GetRelationDefinition (Type declaringType, string shortPropertyName)
    {
      var endPointDefinition = GetEndPointDefinition(declaringType, shortPropertyName);
      return endPointDefinition.RelationDefinition;
    }

    protected object GetPropertyValue (DataContainer dataContainer, Type declaringType, string shortPropertyName)
    {
      return dataContainer.GetValue(GetPropertyDefinition(declaringType, shortPropertyName));
    }

    protected void SetPropertyValue (DataContainer dataContainer, Type declaringType, string shortPropertyName, object value)
    {
      dataContainer.SetValue(GetPropertyDefinition(declaringType, shortPropertyName), value);
    }

    protected PropertyAccessor GetPropertyAccessor<TDomainObject, TValue> (
        TDomainObject domainObject, Expression<Func<TDomainObject, TValue>> propertyExpression, ClientTransaction clientTransaction)
        where TDomainObject: IDomainObject
    {
      var propertyAccessorData = GetPropertyAccessorData(domainObject, propertyExpression);
      return new PropertyAccessor(domainObject, propertyAccessorData, clientTransaction);
    }

    protected PropertyAccessorData GetPropertyAccessorData<TDomainObject, TValue> (TDomainObject domainObject, Expression<Func<TDomainObject, TValue>> propertyExpression)
      where TDomainObject : IDomainObject
    {
      return domainObject.ID.ClassDefinition.PropertyAccessorDataCache.ResolveMandatoryPropertyAccessorData(propertyExpression);
    }

    protected string GetPropertyIdentifier (Type declaringType, string shortPropertyName)
    {
      return declaringType.FullName + "." + shortPropertyName;
    }

    protected ClassDefinition GetTypeDefinition (Type classType)
    {
      return Configuration.GetTypeDefinition(classType);
    }

  }
}
