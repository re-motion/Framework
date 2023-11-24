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
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public abstract class TableInheritanceMappingTest : DatabaseTest
  {
    public const string CreateTestDataFileName = "Database\\DataDomainObjects_CreateTableInheritanceTestData.sql";
    public const string TableInheritanceTestDomainProviderID = "TableInheritanceTestDomain";

    private ClientTransactionScope _transactionScope;

    protected TableInheritanceMappingTest ()
      : base(new DatabaseAgent(TestDomainConnectionString), CreateTestDataFileName)
    {
    }

    protected RdbmsProviderDefinition TableInheritanceTestDomainStorageProviderDefinition
    {
      get
      {
        return (RdbmsProviderDefinition)SafeServiceLocator.Current.GetInstance<IStorageSettings>().GetStorageProviderDefinition(TableInheritanceTestDomainProviderID);
      }
    }


    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();
      MappingConfiguration.SetCurrent(StandardConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent(null);
    }

    public override void SetUp ()
    {
      base.SetUp();
      MappingConfiguration.SetCurrent(TableInheritanceConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent(null);
      _transactionScope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    public override void TearDown ()
    {
      _transactionScope.Leave();
      base.TearDown();
    }

    protected TableInheritanceDomainObjectIDs DomainObjectIDs
    {
      get { return TableInheritanceConfiguration.Instance.GetDomainObjectIDs(); }
    }

    protected IMappingConfiguration Configuration
    {
      get { return MappingConfiguration.Current; }
    }

    protected PropertyDefinition GetPropertyDefinition (Type declaringType, string shortPropertyName)
    {
      var propertyDefinition = Configuration
          .GetTypeDefinition(declaringType)
          .PropertyAccessorDataCache
          .GetMandatoryPropertyAccessorData(declaringType, shortPropertyName)
          .PropertyDefinition;
      Assertion.IsNotNull(propertyDefinition, "Property '{0}.{1}' is not a mapped property.", declaringType, shortPropertyName);
      return propertyDefinition;
    }

    protected IRelationEndPointDefinition GetEndPointDefinition (Type declaringType, string shortPropertyName)
    {
      var endPointDefinition = Configuration
          .GetTypeDefinition(declaringType)
          .PropertyAccessorDataCache
          .GetMandatoryPropertyAccessorData(declaringType, shortPropertyName)
          .RelationEndPointDefinition;
      Assertion.IsNotNull(endPointDefinition, "Property '{0}.{1}' is not a relation end-point.", declaringType, shortPropertyName);
      return endPointDefinition;
    }
  }
}
