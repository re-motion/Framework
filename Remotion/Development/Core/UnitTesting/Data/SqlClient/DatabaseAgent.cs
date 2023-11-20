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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Data.SqlClient
{
  /// <summary>Use the <see cref="DatabaseAgent"/> for setting up the database during unit testing.</summary>
  public class DatabaseAgent
  {
    private string _connectionString;
    private string? _fileName = null;

    public DatabaseAgent (string connectionString)
    {
      ArgumentUtility.CheckNotNullOrEmpty("connectionString", connectionString);

      _connectionString = connectionString;
    }

    public void SetConnectionString (string connectionString)
    {
      ArgumentUtility.CheckNotNullOrEmpty("connectionString", connectionString);

      _connectionString = connectionString;
    }

    public void SetDatabaseReadWrite (string database)
    {
      ArgumentUtility.CheckNotNullOrEmpty("database", database);
      ExecuteCommand(string.Format("ALTER DATABASE [{0}] SET READ_WRITE WITH ROLLBACK IMMEDIATE", database));
    }

    public void SetDatabaseReadOnly (string database)
    {
      ArgumentUtility.CheckNotNullOrEmpty("database", database);
      ExecuteCommand(string.Format("ALTER DATABASE [{0}] SET READ_ONLY WITH ROLLBACK IMMEDIATE", database));
    }

    public int ExecuteBatchFile (string sqlFileName, bool useTransaction)
    {
      return ExecuteBatchFile(sqlFileName, useTransaction, new Dictionary<string, string>());
    }

    public int ExecuteBatchFile (string sqlFileName, bool useTransaction, IDictionary<string, string> replacementDictionary)
    {
      ArgumentUtility.CheckNotNullOrEmpty("sqlFileName", sqlFileName);
      ArgumentUtility.CheckNotNull("replacementDictionary", replacementDictionary);

      _fileName = sqlFileName;
      if (!Path.IsPathRooted(sqlFileName))
      {
        var assembly = typeof(DatabaseAgent).Assembly;
#if NETFRAMEWORK
        var assemblyNameWithoutShadowCopy = assembly.GetName(copiedName: false);
        var codeBaseUri = new Uri(assemblyNameWithoutShadowCopy.EscapedCodeBase!);
        var assemblyDirectory = Path.GetDirectoryName(codeBaseUri.LocalPath)!;
#else
        var assemblyLocation = assembly.Location;
        Assertion.IsFalse(string.IsNullOrEmpty(assemblyLocation), "typeof(DatabaseAgent).Assembly does not have a location on disk.");
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        Assertion.IsNotNull(assemblyDirectory, "typeof(DatabaseAgent).Assembly.Location ('{0}') does not contain a valid directory name.", assemblyLocation);
#endif
        sqlFileName = Path.Combine(assemblyDirectory, sqlFileName);
      }
      return ExecuteBatchString(File.ReadAllText(sqlFileName, Encoding.UTF8), useTransaction, replacementDictionary);
    }

    public int ExecuteBatchString (string commandBatch, bool useTransaction)
    {
      return ExecuteBatchString(commandBatch, useTransaction, new Dictionary<string, string>());
    }

    public int ExecuteBatchString (string commandBatch, bool useTransaction, IDictionary<string, string> replacementDictionary)
    {
      ArgumentUtility.CheckNotNull("commandBatch", commandBatch);
      ArgumentUtility.CheckNotNull("replacementDictionary", replacementDictionary);

      foreach (var replacement in replacementDictionary)
        commandBatch = commandBatch.Replace(replacement.Key, replacement.Value);

      var count = 0;
      using (IDbConnection connection = CreateConnection())
      {
        connection.Open();
        if (useTransaction)
        {
          using (IDbTransaction transaction = connection.BeginTransaction())
          {
            count = ExecuteBatchString(connection, commandBatch, transaction);
            transaction.Commit();
          }
        }
        else
          count = ExecuteBatchString(connection, commandBatch, null);
      }

      return count;
    }

    protected virtual IDbConnection CreateConnection ()
    {
      return new SqlConnection(_connectionString);
    }

    protected virtual IDbCommand CreateCommand (IDbConnection connection, string commandText, IDbTransaction? transaction)
    {
      IDbCommand command = connection.CreateCommand();
      command.CommandType = CommandType.Text;
      command.CommandText = commandText;
      command.Transaction = transaction;
      return command;
    }

    public int ExecuteCommand (string commandText)
    {
      ArgumentUtility.CheckNotNullOrEmpty("commandText", commandText);

      using (IDbConnection connection = CreateConnection())
      {
        connection.Open();
        return ExecuteCommand(connection, commandText, null);
      }
    }

    public object? ExecuteScalarCommand (string commandText)
    {
      ArgumentUtility.CheckNotNullOrEmpty("commandText", commandText);

      using (IDbConnection connection = CreateConnection())
      {
        connection.Open();
        return ExecuteScalarCommand(connection, commandText, null);
      }
    }

    /// <summary>
    /// Opens a <see cref="NoDatabaseWriteSection"/> ensuring that no database writes occur while the section is open.
    /// </summary>
    /// <seealso cref="NoDatabaseWriteSection"/>
    public IDisposable OpenNoDatabaseWriteSection () => new NoDatabaseWriteSection(this, GetLastUsedTimestamp());

    public byte[] GetLastUsedTimestamp () => (byte[])ExecuteScalarCommand("SELECT @@DBTS")!;

    protected virtual int ExecuteBatchString (IDbConnection connection, string commandBatch, IDbTransaction? transaction)
    {
      ArgumentUtility.CheckNotNull("connection", connection);
      ArgumentUtility.CheckNotNullOrEmpty("commandBatch", commandBatch);

      var count = 0;
      foreach (var command in GetCommandTextBatches(commandBatch))
      {
        if (command.Content != null)
        {
          try
          {
            count += ExecuteCommand(connection, command.Content, transaction);
          }
          catch (Exception ex)
          {
            throw new SqlBatchCommandException(
                string.Format(
                    "Could not execute batch command from row {0} to row {1}{2}. (Error message: {3})",
                    command.StartRowNumber,
                    command.EndRowNumber,
                    !string.IsNullOrEmpty(_fileName) ? " in file '" + _fileName + "'" : string.Empty,
                    ex.Message),
                ex);
          }
        }
      }
      return count;
    }

    protected virtual int ExecuteCommand (IDbConnection connection, string commandText, IDbTransaction? transaction)
    {
      using (IDbCommand command = CreateCommand(connection, commandText, transaction))
      {
        return command.ExecuteNonQuery();
      }
    }

    protected virtual object? ExecuteScalarCommand (IDbConnection connection, string commandText, IDbTransaction? transaction)
    {
      using (IDbCommand command = CreateCommand(connection, commandText, transaction))
      {
        return command.ExecuteScalar();
      }
    }

    private IEnumerable<BatchCommand> GetCommandTextBatches (string commandBatch)
    {
      var lineNumber = 1;
      var command = new BatchCommand(lineNumber, commandBatch.Length);
      foreach (var line in commandBatch.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None))
      {
        if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
        {
          var batch = command;
          command = new BatchCommand(lineNumber + 1, commandBatch.Length);
          yield return batch;
        }
        else
          command.AppendCommandBatchLine(line.Trim());
        lineNumber++;
      }

      yield return command;
    }
  }
}
