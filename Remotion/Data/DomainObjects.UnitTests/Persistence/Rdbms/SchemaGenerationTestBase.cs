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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  public class SchemaGenerationTestBase : DatabaseTest
  {
    public SchemaGenerationTestBase ()
        : base(new DatabaseAgent(SchemaGenerationConnectionString1), "Dummy.sql")
    {
    }

    [OneTimeSetUp]
    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();
    }

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      DomainObjectsConfiguration.SetCurrent(SchemaGenerationConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent(SchemaGenerationConfiguration.Instance.GetMappingConfiguration());
    }

    [TearDown]
    public override void TearDown ()
    {
      base.TearDown();
    }

    protected MappingConfiguration MappingConfiguration
    {
      get { return SchemaGenerationConfiguration.Instance.GetMappingConfiguration(); }
    }

    protected StorageConfiguration StorageConfiguration
    {
      get { return SchemaGenerationConfiguration.Instance.GetPersistenceConfiguration(); }
    }

    protected RdbmsProviderDefinition SchemaGenerationFirstStorageProviderDefinition
    {
      get { return (RdbmsProviderDefinition)SafeServiceLocator.Current.GetInstance<IStorageSettings>().GetStorageProviderDefinition(SchemaGenerationFirstStorageProviderID); }
    }

    protected RdbmsProviderDefinition SchemaGenerationSecondStorageProviderDefinition
    {
      get
      {
        return
            (RdbmsProviderDefinition)SafeServiceLocator.Current.GetInstance<IStorageSettings>().GetStorageProviderDefinition(SchemaGenerationSecondStorageProviderID);
      }
    }

    protected RdbmsProviderDefinition SchemaGenerationThirdStorageProviderDefinition
    {
      get { return (RdbmsProviderDefinition)SafeServiceLocator.Current.GetInstance<IStorageSettings>().GetStorageProviderDefinition(SchemaGenerationThirdStorageProviderID); }
    }

    protected RdbmsProviderDefinition SchemaGenerationInternalStorageProviderDefinition
    {
      get
      {
        return
            (RdbmsProviderDefinition)SafeServiceLocator.Current.GetInstance<IStorageSettings>().GetStorageProviderDefinition(SchemaGenerationInternalStorageProviderID);
      }
    }

    protected virtual TableScriptBuilder CreateTableBuilder ()
    {
      return new TableScriptBuilder(new SqlTableScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    protected virtual ViewScriptBuilder CreateViewBuilder ()
    {
      return new ViewScriptBuilder(
          new SqlTableViewScriptElementFactory(),
          new SqlUnionViewScriptElementFactory(),
          new SqlFilterViewScriptElementFactory(),
          new SqlEmptyViewScriptElementFactory(),
          new SqlCommentScriptElementFactory());
    }

    protected virtual ViewScriptBuilder CreateExtendedViewBuilder ()
    {
      return new ViewScriptBuilder(
          new ExtendedSqlTableViewScriptElementFactory(),
          new ExtendedSqlUnionViewScriptElementFactory(),
          new ExtendedSqlFilterViewScriptElementFactory(),
          new SqlEmptyViewScriptElementFactory(),
          new SqlCommentScriptElementFactory());
    }

    protected virtual ForeignKeyConstraintScriptBuilder CreateConstraintBuilder ()
    {
      return new ForeignKeyConstraintScriptBuilder(new SqlForeignKeyConstraintScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    protected virtual IndexScriptBuilder CreateIndexBuilder ()
    {
      return
          new IndexScriptBuilder(
              new SqlIndexScriptElementFactory(
                  new SqlIndexDefinitionScriptElementFactory(),
                  new SqlPrimaryXmlIndexDefinitionScriptElementFactory(),
                  new SqlSecondaryXmlIndexDefinitionScriptElementFactory()),
              new SqlCommentScriptElementFactory());
    }

    protected virtual SynonymScriptBuilder CreateSynonymBuilder ()
    {
      var sqlSynonymScriptElementFactory = new SqlSynonymScriptElementFactory();
      return new SynonymScriptBuilder(
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          new SqlCommentScriptElementFactory());
    }

    protected PropertyDefinition GetPropertyDefinition<TSourceObject, TPropertyType> (Expression<Func<TSourceObject, TPropertyType>> expression)
        where TSourceObject : DomainObject
    {
      var propertyInfo = MemberInfoFromExpressionUtility.GetProperty(expression);
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(TSourceObject));
      return classDefinition.ResolveProperty(PropertyInfoAdapter.Create(propertyInfo));
    }}
}
