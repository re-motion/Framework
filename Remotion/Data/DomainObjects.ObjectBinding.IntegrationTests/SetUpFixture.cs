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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding.IntegrationTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    public static string TestDomainConnectionString
    {
      get
      {
        return DatabaseConfiguration.UpdateConnectionString("Initial Catalog=DBPrefix_RemotionDataDomainObjectsObjectBindingIntegrationTestDomain");
      }
    }

    public static string MasterConnectionString
    {
      get { return DatabaseConfiguration.UpdateConnectionString("Initial Catalog=master"); }
    }

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      try
      {
        BootstrapServiceConfiguration.SetLoggerFactory(NullLoggerFactory.Instance);

        var storageSettingsFactory = StorageSettingsFactory.CreateForSqlServer(TestDomainConnectionString);

        var defaultServiceLocator = DefaultServiceLocator.Create();
        defaultServiceLocator.RegisterSingle(() => storageSettingsFactory);

        ServiceLocator.SetLocatorProvider(() => defaultServiceLocator);

        SqlConnection.ClearAllPools();

        var scriptGenerator = new ScriptGenerator(
            pd => pd.Factory.CreateSchemaScriptBuilder(pd),
            new RdbmsStorageEntityDefinitionProvider(),
            new RdbmsStructuredTypeDefinitionProvider(),
            new ScriptToStringConverter());
        var scripts = scriptGenerator.GetScripts(MappingConfiguration.Current.GetTypeDefinitions()).Single();

        var masterAgent = new DatabaseAgent(MasterConnectionString);
        masterAgent.ExecuteBatchFile("Database\\CreateDB.sql", false, DatabaseConfiguration.GetReplacementDictionary());

        var databaseAgent = new DatabaseAgent(TestDomainConnectionString);
        databaseAgent.ExecuteBatchString(scripts.SetUpScript, true);
      }
      catch (Exception e)
      {
        Console.WriteLine("SetUpFixture failed: " + e);
        Console.WriteLine();
        throw;
      }
    }

    [OneTimeTearDown]
    public virtual void OneTimeTearDown ()
    {
      SqlConnection.ClearAllPools();
    }
  }
}
