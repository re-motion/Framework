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
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  public class DatabaseHelper
  {
    public readonly string TearDownDBScript = Path.Combine(TestContext.CurrentContext.TestDirectory, "SecurityManagerTearDownDB.sql");
    public readonly string SetupDBScript = Path.Combine(TestContext.CurrentContext.TestDirectory, "SecurityManagerSetupDB.sql");
    public readonly string SetupConstraintsScript = Path.Combine(TestContext.CurrentContext.TestDirectory, "SecurityManagerSetupConstraints.sql");
    public readonly string TearDownDBSpecialTablesScript = Path.Combine(TestContext.CurrentContext.TestDirectory, "SecurityManagerTearDownDBSpecialTables.sql");
    public readonly string SetupDBSpecialTablesScript = Path.Combine(TestContext.CurrentContext.TestDirectory, "SecurityManagerSetupDBSpecialTables.sql");

    public void SetupDB ()
    {
      IDbConnection connection = GetConnection();
      IDbTransaction transaction = connection.BeginTransaction();

      try
      {
        ExecuteSql(ReadFile(TearDownDBSpecialTablesScript).ApplyDatabaseConfiguration(), connection, transaction);
        ExecuteSql(ReadFile(TearDownDBScript).ApplyDatabaseConfiguration(), connection, transaction);
        ExecuteSql(ReadFile(SetupDBScript).ApplyDatabaseConfiguration(), connection, transaction);
        ExecuteSql(ReadFile(SetupConstraintsScript).ApplyDatabaseConfiguration(), connection, transaction);
        ExecuteSql(ReadFile(SetupDBSpecialTablesScript).ApplyDatabaseConfiguration(), connection, transaction);
      }
      catch
      {
        transaction.Rollback();
        throw;
      }

      transaction.Commit();
    }

    private void ExecuteSql (string sql, IDbConnection connection, IDbTransaction transaction)
    {
      string[] sqlScriptParts = Regex.Split(sql, @"^[ \t]*GO[ \t]*(\r\n)?", RegexOptions.IgnoreCase | RegexOptions.Multiline);

      foreach (string sqlScriptPart in sqlScriptParts)
      {
        if (sqlScriptPart.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim() != string.Empty)
        {
          using (IDbCommand command = connection.CreateCommand())
          {
            command.Transaction = transaction;
            command.CommandText = sqlScriptPart;

            command.ExecuteNonQuery();
          }
        }
      }
    }

    private string ReadFile (string file)
    {
      using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
      {
        return reader.ReadToEnd();
      }
    }

    private IDbConnection GetConnection ()
    {
      var storageSettings = SafeServiceLocator.Current.GetInstance<IStorageSettings>();
      var providerDefinition = (RdbmsProviderDefinition)storageSettings.GetDefaultStorageProviderDefinition();
      Assertion.IsNotNull(providerDefinition, "IStorageSettings.GetDefaultStorageProviderDefinition() != null");
      IDbConnection connection = new SqlConnection(providerDefinition.ConnectionString);
      connection.Open();

      return connection;
    }
  }
}
