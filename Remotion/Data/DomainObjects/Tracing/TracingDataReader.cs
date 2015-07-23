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
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Tracing
{
  /// <summary>
  /// Provides a wrapper for implementations of <see cref="IDataReader"/>. The number of records read and the lifetime of the reader 
  /// are traced using <see cref="IPersistenceExtension"/> passed during the instantiation.
  /// </summary>
  public class TracingDataReader : IDataReader
  {
    #region IDataRecord implementation

    public string GetName (int i)
    {
      return _dataReader.GetName (i);
    }

    public string GetDataTypeName (int i)
    {
      return _dataReader.GetDataTypeName (i);
    }

    public Type GetFieldType (int i)
    {
      return _dataReader.GetFieldType (i);
    }

    public object GetValue (int i)
    {
      return _dataReader.GetValue (i);
    }

    public int GetValues (object[] values)
    {
      return _dataReader.GetValues (values);
    }

    public int GetOrdinal (string name)
    {
      return _dataReader.GetOrdinal (name);
    }

    public bool GetBoolean (int i)
    {
      return _dataReader.GetBoolean (i);
    }

    public byte GetByte (int i)
    {
      return _dataReader.GetByte (i);
    }

    public long GetBytes (int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
      return _dataReader.GetBytes (i, fieldOffset, buffer, bufferoffset, length);
    }

    public char GetChar (int i)
    {
      return _dataReader.GetChar (i);
    }

    public long GetChars (int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
      return _dataReader.GetChars (i, fieldoffset, buffer, bufferoffset, length);
    }

    public Guid GetGuid (int i)
    {
      return _dataReader.GetGuid (i);
    }

    public short GetInt16 (int i)
    {
      return _dataReader.GetInt16 (i);
    }

    public int GetInt32 (int i)
    {
      return _dataReader.GetInt32 (i);
    }

    public long GetInt64 (int i)
    {
      return _dataReader.GetInt64 (i);
    }

    public float GetFloat (int i)
    {
      return _dataReader.GetFloat (i);
    }

    public double GetDouble (int i)
    {
      return _dataReader.GetDouble (i);
    }

    public string GetString (int i)
    {
      return _dataReader.GetString (i);
    }

    public decimal GetDecimal (int i)
    {
      return _dataReader.GetDecimal (i);
    }

    public DateTime GetDateTime (int i)
    {
      return _dataReader.GetDateTime (i);
    }

    public IDataReader GetData (int i)
    {
      return _dataReader.GetData (i);
    }

    public bool IsDBNull (int i)
    {
      return _dataReader.IsDBNull (i);
    }

    public int FieldCount
    {
      get { return _dataReader.FieldCount; }
    }

    public object this [int i]
    {
      get { return _dataReader[i]; }
    }

    public object this [string name]
    {
      get { return _dataReader[name]; }
    }

    #endregion

    #region IDataReader implementation

    public bool NextResult ()
    {
      return _dataReader.NextResult();
    }

    public DataTable GetSchemaTable ()
    {
      return _dataReader.GetSchemaTable();
    }

    public int Depth
    {
      get { return _dataReader.Depth; }
    }

    public bool IsClosed
    {
      get { return _dataReader.IsClosed; }
    }

    public int RecordsAffected
    {
      get { return _dataReader.RecordsAffected; }
    }

    #endregion

    private readonly IDataReader _dataReader;
    private readonly IPersistenceExtension _persistenceExtension;
    private readonly Guid _connectionID;
    private readonly Guid _queryID;
    private readonly Stopwatch _stopwatch;
    private int _rowCount;

    public TracingDataReader (IDataReader dataReader, IPersistenceExtension persistenceExtension, Guid connectionID, Guid queryID)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);
      ArgumentUtility.CheckNotNull ("persistenceExtension", persistenceExtension);

      _dataReader = dataReader;
      _persistenceExtension = persistenceExtension;
      _connectionID = connectionID;
      _queryID = queryID;
      _stopwatch = Stopwatch.StartNew();
    }

    public IDataReader WrappedInstance
    {
      get { return _dataReader; }
    }

    public Guid ConnectionID
    {
      get { return _connectionID; }
    }

    public Guid QueryID
    {
      get { return _queryID; }
    }

    public IPersistenceExtension PersistenceExtension
    {
      get { return _persistenceExtension; }
    }

    public void Dispose ()
    {
      TraceQueryCompleted();
      _dataReader.Dispose ();
    }

    public void Close ()
    {
      TraceQueryCompleted ();
      _dataReader.Close ();
    }

    public bool Read ()
    {
      bool hasRecord = _dataReader.Read();
      if (hasRecord)
        _rowCount++;
      return hasRecord;
    }

    private void TraceQueryCompleted ()
    {
      if (_stopwatch.IsRunning)
      {
        _persistenceExtension.QueryCompleted (_connectionID, _queryID, _stopwatch.Elapsed, _rowCount);
        _stopwatch.Stop ();
      }
    }
  }
}