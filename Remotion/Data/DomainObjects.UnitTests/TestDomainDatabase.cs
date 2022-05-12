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
using System.Data.SqlClient;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Database;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests
{
  /// <summary>
  /// Provides methods for setting up the test domain database for the integration tests.
  /// The initialization takes some time so it is only executed if the tests demand it (see <see cref="DatabaseTest"/>).
  /// </summary>
  public static class TestDomainDatabase
  {
    private static readonly object s_lock = new();

    private static bool s_isInitialized;
    private static StandardMappingDatabaseAgent s_standardMappingDatabaseAgent;

    public static void EnsureDatabaseInitialized ()
    {
      lock (s_lock)
      {
        if (s_isInitialized)
          return;

        StandardConfiguration.EnsureInitialized();
        TableInheritanceConfiguration.EnsureInitialized();

        SqlConnection.ClearAllPools();

        var masterAgent = new DatabaseAgent(DatabaseTest.MasterConnectionString);
        masterAgent.ExecuteBatchFile("Database\\DataDomainObjects_CreateDB.sql", false, DatabaseConfiguration.GetReplacementDictionary());
        var testDomainAgent = new DatabaseAgent(DatabaseTest.TestDomainConnectionString);
        testDomainAgent.ExecuteBatchFile("Database\\DataDomainObjects_TestDomain_TearDownDB.sql", true, DatabaseConfiguration.GetReplacementDictionary());
        testDomainAgent.ExecuteBatchFile("Database\\DataDomainObjects_TableInheritanceTestDomain_TearDownDB.sql", true, DatabaseConfiguration.GetReplacementDictionary());
        testDomainAgent.ExecuteBatchFile("Database\\DataDomainObjects_TestDomain_SetupDB.sql", true, DatabaseConfiguration.GetReplacementDictionary());
        testDomainAgent.ExecuteBatchFile("Database\\DataDomainObjects_TableInheritanceTestDomain_SetupDB.sql", true, DatabaseConfiguration.GetReplacementDictionary());
        testDomainAgent.ExecuteBatchFile("Database\\DataDomainObjects_Patches.sql", true, DatabaseConfiguration.GetReplacementDictionary());

        s_standardMappingDatabaseAgent = new StandardMappingDatabaseAgent(DatabaseTest.TestDomainConnectionString);
        string sqlFileName = StandardMappingTest.CreateTestDataFileName;
        s_standardMappingDatabaseAgent.ExecuteBatchFile(sqlFileName, true, DatabaseConfiguration.GetReplacementDictionary());
        string sqlFileName1 = TableInheritanceMappingTest.CreateTestDataFileName;
        s_standardMappingDatabaseAgent.ExecuteBatchFile(sqlFileName1, true, DatabaseConfiguration.GetReplacementDictionary());

        // We don't want the tests to initialize a default mapping; therefore, modify MappingConfiguration.s_fields.Current so that it will 
        // throw when asked to generate a new MappingConfiguration.

        var throwingMappingConfigurationContainer = new DoubleCheckedLockingContainer<IMappingConfiguration>(
            () =>
            {
              throw new InvalidOperationException(
                  "This test failed to setup the mapping configuration. Did you forget to derive from StandardMappingTest or to call base.SetUp?");
            });
        var fields = PrivateInvoke.GetNonPublicStaticField(typeof(MappingConfiguration), "s_fields");
        Assertion.IsNotNull(fields);
        PrivateInvoke.SetPublicField(fields, "Current", throwingMappingConfigurationContainer);

        s_isInitialized = true;
      }
    }
  }
}
