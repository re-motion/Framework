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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.Database;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public abstract class DatabaseTest
  {
    public const string DatabaseName = "TestDomain";
    public const string DefaultStorageProviderID = "DefaultStorageProvider";
    public const string c_testDomainProviderID = "TestDomain";
    public const string c_unitTestStorageProviderStubID = "UnitTestStorageProviderStub";

    public const string SchemaGenerationFirstStorageProviderID = "SchemaGenerationFirstStorageProvider";
    public const string SchemaGenerationSecondStorageProviderID = "SchemaGenerationSecondStorageProvider";
    public const string SchemaGenerationThirdStorageProviderID = "SchemaGenerationThirdStorageProvider";
    public const string SchemaGenerationInternalStorageProviderID = "SchemaGenerationInternalStorageProvider";

    private readonly DatabaseAgent _databaseAgent;
    private readonly string _createTestDataFileName;
    private bool _isDatabaseModifyable;

    protected DatabaseTest (DatabaseAgent databaseAgent, string createTestDataFileName)
    {
      ArgumentUtility.CheckNotNull ("databaseAgent", databaseAgent);
      ArgumentUtility.CheckNotNullOrEmpty ("createTestDataFileName", createTestDataFileName);

      _databaseAgent = databaseAgent;
      _createTestDataFileName = createTestDataFileName;
    }

    [SetUp]
    public virtual void SetUp ()
    {
    }

    [TearDown]
    public virtual void TearDown ()
    {
      if (_isDatabaseModifyable)
      {
        _databaseAgent.ExecuteBatchFile (_createTestDataFileName, true, DatabaseConfiguration.GetReplacementDictionary());
      }
    }

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp ()
    {
    }

    [TestFixtureTearDown]
    public virtual void TestFixtureTearDown ()
    {
      if (_isDatabaseModifyable)
      {
        _databaseAgent.SetDatabaseReadOnly (DatabaseName);
        _isDatabaseModifyable = false;
      }
    }

    protected DatabaseAgent DatabaseAgent
    {
      get { return _databaseAgent; }
    }


    public static string TestDomainConnectionString
    {
      get { return string.Format ("Integrated Security=SSPI;Initial Catalog=TestDomain;Data Source={0}; Max Pool Size=1;", DatabaseConfiguration.DataSource); }
    }

    public static string MasterConnectionString
    {
      get { return string.Format ("Integrated Security=SSPI;Initial Catalog=master;Data Source={0}; Max Pool Size=1;", DatabaseConfiguration.DataSource); }
    }

    public static string SchemaGenerationConnectionString1
    {
      get { return string.Format ("Integrated Security=SSPI;Initial Catalog=SchemaGenerationTestDomain1;Data Source={0}; Max Pool Size=1;", DatabaseConfiguration.DataSource); }
    }

    public static string SchemaGenerationConnectionString2
    {
      get { return string.Format ("Integrated Security=SSPI;Initial Catalog=SchemaGenerationTestDomain2;Data Source={0}; Max Pool Size=1;", DatabaseConfiguration.DataSource); }
    }

    public static string SchemaGenerationConnectionString3
    {
      get { return string.Format ("Integrated Security=SSPI;Initial Catalog=SchemaGenerationTestDomain3;Data Source={0}; Max Pool Size=1;", DatabaseConfiguration.DataSource); }
    }

    protected void SetDatabaseModifyable ()
    {
      if (!_isDatabaseModifyable)
      {
        _isDatabaseModifyable = true;
        _databaseAgent.SetDatabaseReadWrite (DatabaseName);
      }
    }

    protected IDbCommand CreateCommand (string table, Guid id, IDbConnection connection)
    {
      IDbCommand command = connection.CreateCommand();
      command.CommandText = string.Format ("SELECT * FROM [{0}] where ID = @id", table);

      IDbDataParameter parameter = command.CreateParameter();
      parameter.ParameterName = "@id";
      parameter.Value = id;
      command.Parameters.Add (parameter);

      return command;
    }
  }
}