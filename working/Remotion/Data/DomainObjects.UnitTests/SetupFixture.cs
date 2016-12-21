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
using log4net;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Database;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private StandardMappingDatabaseAgent _standardMappingDatabaseAgent;
    private DoubleCheckedLockingContainer<IMappingConfiguration> _previousMappingConfigurationContainer;

    [SetUp]
    public void SetUp ()
    {
      try
      {
        ServiceLocator.SetLocatorProvider (() => null);

        LogManager.ResetConfiguration ();
        Assert.That (LogManager.GetLogger (typeof (LoggingClientTransactionListener)).IsDebugEnabled, Is.False);

        StandardConfiguration.Initialize();
        TableInheritanceConfiguration.Initialize ();

        SqlConnection.ClearAllPools();

        var masterAgent = new DatabaseAgent (DatabaseTest.MasterConnectionString);
        masterAgent.ExecuteBatchFile ("DataDomainObjects_CreateDB.sql", false, DatabaseConfiguration.GetReplacementDictionary());  
        var testDomainAgent = new DatabaseAgent (DatabaseTest.TestDomainConnectionString);
        testDomainAgent.ExecuteBatchFile ("DataDomainObjects_SetupDB.sql", true, DatabaseConfiguration.GetReplacementDictionary());

        _standardMappingDatabaseAgent = new StandardMappingDatabaseAgent (DatabaseTest.TestDomainConnectionString);
        string sqlFileName = StandardMappingTest.CreateTestDataFileName;
        _standardMappingDatabaseAgent.ExecuteBatchFile (sqlFileName, true, DatabaseConfiguration.GetReplacementDictionary());
        string sqlFileName1 = TableInheritanceMappingTest.CreateTestDataFileName;
        _standardMappingDatabaseAgent.ExecuteBatchFile (sqlFileName1, true, DatabaseConfiguration.GetReplacementDictionary());
        _standardMappingDatabaseAgent.SetDatabaseReadOnly (DatabaseTest.DatabaseName);

        // We don't want the tests to initialize a default mapping; therefore, modify MappingConfiguration.s_mappingConfiguration so that it will 
        // throw when asked to generate a new MappingConfiguration.

        _previousMappingConfigurationContainer = (DoubleCheckedLockingContainer<IMappingConfiguration>) PrivateInvoke.GetNonPublicStaticField (
            typeof (MappingConfiguration), 
            "s_mappingConfiguration");
        var throwingMappingConfigurationContainer = new DoubleCheckedLockingContainer<IMappingConfiguration> (
            () =>
            {
              throw new InvalidOperationException (
                  "This test failed to setup the mapping configuration. Did you forget to derive from StandardMappingTest or to call base.SetUp?");
            });
        PrivateInvoke.SetNonPublicStaticField (typeof (MappingConfiguration), "s_mappingConfiguration", throwingMappingConfigurationContainer);
      }
      catch (Exception ex)
      {
        Console.WriteLine ("SetUpFixture failed: " + ex);
        Console.WriteLine ();
        throw;
      }
    }

    [TearDown]
    public void TearDown ()
    {
      PrivateInvoke.SetNonPublicStaticField (typeof (MappingConfiguration), "s_mappingConfiguration", _previousMappingConfigurationContainer);

      _standardMappingDatabaseAgent.SetDatabaseReadWrite (DatabaseTest.DatabaseName);
      SqlConnection.ClearAllPools();
    }
  }
}
