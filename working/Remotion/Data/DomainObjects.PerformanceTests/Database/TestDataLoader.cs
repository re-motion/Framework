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
using System.IO;
using System.Text;

namespace Remotion.Data.DomainObjects.PerformanceTests.Database
{
public class TestDataLoader : IDisposable
{
  // types

  // static members and constants

  private const string c_testDomainFilename = @"Database\CreateTestData.sql";

  // member fields

  private SqlConnection _connection;
  private SqlTransaction _transaction;

  private bool _disposed = false;

  // construction and disposing

  public TestDataLoader (string connectionString)
  {
    _connection = new SqlConnection (connectionString);
    _connection.Open ();
  }

  // methods and properties

  public void Load ()
  {
    using (_transaction = _connection.BeginTransaction ())
    {
      ExecuteSqlFile (c_testDomainFilename);

      _transaction.Commit ();  
    }
  }

  private void ExecuteSqlFile (string sqlFile)
  {
    using (SqlCommand command = new SqlCommand (ReadFile (sqlFile), _connection, _transaction))
    {
      command.ExecuteNonQuery ();
    }
  }

  private string ReadFile (string file)
  {
    using (StreamReader reader = new StreamReader (file, Encoding.Default))
    {
      return reader.ReadToEnd ();
    }
  }

  #region IDisposable Members

  public void Dispose()
  {
    Dispose (true);
    GC.SuppressFinalize(this);
  }

  #endregion

  private void Dispose (bool disposing)
  {
    if (!_disposed && disposing)
    {
      if (_connection != null)
      {
        _connection.Close ();
        _connection = null;
      }

      if (_transaction != null)
      {
        _transaction.Dispose ();
        _transaction = null;
      }

      _disposed = true;
    }
  }
}
}
