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
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Tracing
{
  /// <summary>
  /// Provides a wrapper for implementations of <see cref="DbDataReader"/>. The number of records read and the lifetime of the reader 
  /// are traced using <see cref="IPersistenceExtension"/> passed during the instantiation.
  /// </summary>
  public sealed class TracingDataReader : DbDataReader
  {
    private readonly DbDataReader _dataReader;
    private readonly IPersistenceExtension _persistenceExtension;
    private readonly Guid _connectionID;
    private readonly Guid _queryID;
    private readonly Stopwatch _stopwatch;
    private int _rowCount;

    public TracingDataReader (DbDataReader dataReader, IPersistenceExtension persistenceExtension, Guid connectionID, Guid queryID)
    {
      ArgumentNullException.ThrowIfNull(dataReader);
      ArgumentNullException.ThrowIfNull(persistenceExtension);

      _dataReader = dataReader;
      _persistenceExtension = persistenceExtension;
      _connectionID = connectionID;
      _queryID = queryID;
      _stopwatch = Stopwatch.StartNew();
    }

    public override int FieldCount => _dataReader.FieldCount;
    public override bool HasRows => _dataReader.HasRows;
    public override object this [int i] => _dataReader[i];
    public override object this [string name] => _dataReader[name];
    public override int Depth => _dataReader.Depth;
    public override bool IsClosed => _dataReader.IsClosed;
    public override int RecordsAffected => _dataReader.RecordsAffected;
    public DbDataReader WrappedInstance => _dataReader;
    public Guid ConnectionID => _connectionID;
    public Guid QueryID => _queryID;
    public IPersistenceExtension PersistenceExtension => _persistenceExtension;


    public override string GetName (int i)
    {
      return _dataReader.GetName(i);
    }

    public override string GetDataTypeName (int i)
    {
      return _dataReader.GetDataTypeName(i);
    }

    public override IEnumerator GetEnumerator ()
    {
      return _dataReader.GetEnumerator();
    }

    public override Type GetFieldType (int i)
    {
      return _dataReader.GetFieldType(i);
    }

    public override object GetValue (int i)
    {
      return _dataReader.GetValue(i);
    }

    public override int GetValues (object[] values)
    {
      return _dataReader.GetValues(values);
    }

    public override int GetOrdinal (string name)
    {
      return _dataReader.GetOrdinal(name);
    }

    public override bool GetBoolean (int i)
    {
      return _dataReader.GetBoolean(i);
    }

    public override byte GetByte (int i)
    {
      return _dataReader.GetByte(i);
    }

    public override long GetBytes (int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
    {
      return _dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
    }

    public override char GetChar (int i)
    {
      return _dataReader.GetChar(i);
    }

    public override long GetChars (int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
    {
      return _dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
    }

    public override Guid GetGuid (int i)
    {
      return _dataReader.GetGuid(i);
    }

    public override short GetInt16 (int i)
    {
      return _dataReader.GetInt16(i);
    }

    public override int GetInt32 (int i)
    {
      return _dataReader.GetInt32(i);
    }

    public override long GetInt64 (int i)
    {
      return _dataReader.GetInt64(i);
    }

    public override float GetFloat (int i)
    {
      return _dataReader.GetFloat(i);
    }

    public override double GetDouble (int i)
    {
      return _dataReader.GetDouble(i);
    }

    public override string GetString (int i)
    {
      return _dataReader.GetString(i);
    }

    public override decimal GetDecimal (int i)
    {
      return _dataReader.GetDecimal(i);
    }

    public override DateTime GetDateTime (int i)
    {
      return _dataReader.GetDateTime(i);
    }

    public override bool IsDBNull (int i)
    {
      return _dataReader.IsDBNull(i);
    }

    public override bool NextResult ()
    {
      return _dataReader.NextResult();
    }

    public override DataTable? GetSchemaTable ()
    {
      return _dataReader.GetSchemaTable();
    }

    public override void Close ()
    {
      TraceQueryCompleted();
      _dataReader.Close();
    }

    public override bool Read ()
    {
      bool hasRecord = _dataReader.Read();
      if (hasRecord)
        _rowCount++;
      return hasRecord;
    }

    protected override DbDataReader GetDbDataReader (int i)
    {
      return _dataReader.GetData(i);
    }

    protected override void Dispose (bool disposing)
    {
      Assertion.DebugAssert(disposing, "Type is sealed without an implemented Finalizer. 'disposing' flag is always true");

      TraceQueryCompleted();
      _dataReader.Dispose();
    }

    private void TraceQueryCompleted ()
    {
      if (_stopwatch.IsRunning)
      {
        _persistenceExtension.QueryCompleted(_connectionID, _queryID, _stopwatch.Elapsed, _rowCount);
        _stopwatch.Stop();
      }
    }
  }
}
