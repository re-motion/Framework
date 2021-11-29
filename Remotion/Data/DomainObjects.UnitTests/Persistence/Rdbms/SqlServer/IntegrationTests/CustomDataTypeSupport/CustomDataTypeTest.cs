﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2012;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Database;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport.TestDomain;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport
{
  [TestFixture]
  public class CustomDataTypeTest : CustomStorageObjectFactoryTestBase
  {
    private ServiceLocatorScope _serviceLocatorScope;

    public CustomDataTypeTest ()
        : base(CreateEmptyTestDataFileName)
    {
    }

    protected override SqlStorageObjectFactory CreateSqlStorageObjectFactory ()
    {
      return new CustomDataTypeStorageObjectFactory();
    }

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle<IPersistableDataValidator>(() => new SimpleDataTypePropertyMaxLengthValidator());
      serviceLocator.RegisterSingle<IDataContainerValidator>(() => new SimpleDataTypePropertyMaxLengthValidator());
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    public override void TestFixtureTearDown ()
    {
      _serviceLocatorScope.Dispose();
      base.TestFixtureTearDown();
    }

    [Test]
    public void InsertAndGetObject_WithCompoundDataType ()
    {
      SetDatabaseModifyable();

      ObjectID id;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = ClassWithCustomDataType.NewObject();
        obj.CompoundDataTypeValue = new CompoundDataType("StringValue", 50);
        ClientTransaction.Current.Commit();

        id = obj.ID;
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = ClassWithCustomDataType.GetObject(ClientTransaction.Current, id);
        Assert.That(obj.CompoundDataTypeValue, Is.Not.Null);
        Assert.That(obj.CompoundDataTypeValue.StringValue, Is.EqualTo("StringValue"));
        Assert.That(obj.CompoundDataTypeValue.Int32Value, Is.EqualTo(50));
      }
    }

    [Test]
    public void InsertAndGetObject_WithCompoundDataType_Null ()
    {
      SetDatabaseModifyable();

      ObjectID id;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = ClassWithCustomDataType.NewObject();
        obj.CompoundDataTypeValue = null;
        ClientTransaction.Current.Commit();

        id = obj.ID;
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = ClassWithCustomDataType.GetObject(ClientTransaction.Current, id);
        Assert.That(obj.CompoundDataTypeValue, Is.Null);
      }
    }

    [Test]
    public void InsertAndGetObject_WithSimpleDataType ()
    {
      SetDatabaseModifyable();

      ObjectID id;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = ClassWithCustomDataType.NewObject();
        obj.SimpleDataTypeValue = new SimpleDataType("StringValue");
        ClientTransaction.Current.Commit();

        id = obj.ID;
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = ClassWithCustomDataType.GetObject(ClientTransaction.Current, id);
        Assert.That(obj.SimpleDataTypeValue, Is.Not.Null);
        Assert.That(obj.SimpleDataTypeValue.StringValue, Is.EqualTo("StringValue"));
      }
    }

    [Test]
    public void ValidateMaxLength_WithSimpleDataType ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = ClassWithCustomDataType.NewObject();
        obj.SimpleDataTypeValue = new SimpleDataType(new string('x', 200));
        Assert.That(() => ClientTransaction.Current.Commit(), Throws.TypeOf<PropertyValueTooLongException>());
      }
    }

    [Test]
    public void LinqQuery_WithSimpleDataType ()
    {
      SetDatabaseModifyable();

      ObjectID id;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = ClassWithCustomDataType.NewObject();
        obj.SimpleDataTypeValue = new SimpleDataType("StringValue");
        ClientTransaction.Current.Commit();

        id = obj.ID;
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var result = QueryFactory.CreateLinqQuery<ClassWithCustomDataType>()
            .SingleOrDefault(o => o.SimpleDataTypeValue == new SimpleDataType("StringValue"));
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ID, Is.EqualTo(id));
      }
    }

    [Test]
    public void CreateSetupScript ()
    {
      var scriptGenerator = new ScriptGenerator(
          pd => pd.Factory.CreateSchemaScriptBuilder(pd),
          new RdbmsStorageEntityDefinitionProvider(),
          new ScriptToStringConverter());
      var scripts = scriptGenerator.GetScripts(MappingConfiguration.Current.GetTypeDefinitions()).ToArray();
      Assert.That(scripts.Length, Is.EqualTo(1));
      Assert.That(
          scripts[0].SetUpScript,
          Is.EqualTo(
              @"USE DBPrefix_TestDomain
-- Create all tables
CREATE TABLE [dbo].[CustomDataType_ClassWithCustomDataType]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [CompoundDataTypeValueStringValue] nvarchar (100) NULL,
  [CompoundDataTypeValueInt32Value] int NULL,
  [SimpleDataTypeValue] nvarchar (100) NULL,
  CONSTRAINT [PK_CustomDataType_ClassWithCustomDataType] PRIMARY KEY CLUSTERED ([ID])
)
-- Create foreign key constraints for tables that were created above
-- Create a view for every class
GO
CREATE VIEW [dbo].[CustomDataType_ClassWithCustomDataTypeView] ([ID], [ClassID], [Timestamp], [CompoundDataTypeValueStringValue], [CompoundDataTypeValueInt32Value], [SimpleDataTypeValue])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CompoundDataTypeValueStringValue], [CompoundDataTypeValueInt32Value], [SimpleDataTypeValue]
    FROM [dbo].[CustomDataType_ClassWithCustomDataType]
  WITH CHECK OPTION
GO
-- Create indexes for tables that were created above
-- Create synonyms for tables that were created above
".ApplyDatabaseConfiguration()));
    }
  }
}