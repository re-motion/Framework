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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders
{
  /// <summary>
  /// Reads data from an <see cref="IDataReader"/> and converts it into timestamp instances.
  /// The command whose data is converted must return an ID and a timestamp (as defined by the given <see cref="IRdbmsStoragePropertyDefinition"/>
  /// instances).
  /// </summary>
  public class TimestampReader : IObjectReader<Tuple<ObjectID, object>>
  {
    private readonly IRdbmsStoragePropertyDefinition _idProperty;
    private readonly IRdbmsStoragePropertyDefinition _timestampProperty;
    private readonly IColumnOrdinalProvider _columnOrdinalProvider;

    public TimestampReader (
        IRdbmsStoragePropertyDefinition idProperty, IRdbmsStoragePropertyDefinition timestampProperty, IColumnOrdinalProvider columnOrdinalProvider)
    {
      ArgumentUtility.CheckNotNull ("idProperty", idProperty);
      ArgumentUtility.CheckNotNull ("timestampProperty", timestampProperty);
      ArgumentUtility.CheckNotNull ("columnOrdinalProvider", columnOrdinalProvider);

      _idProperty = idProperty;
      _timestampProperty = timestampProperty;
      _columnOrdinalProvider = columnOrdinalProvider;
    }

    public IRdbmsStoragePropertyDefinition IDProperty
    {
      get { return _idProperty; }
    }

    public IRdbmsStoragePropertyDefinition TimestampProperty
    {
      get { return _timestampProperty; }
    }

    public IColumnOrdinalProvider ColumnOrdinalProvider
    {
      get { return _columnOrdinalProvider; }
    }

    public Tuple<ObjectID, object> Read (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      if (dataReader.Read ())
        return GetTimestampTuple (new ColumnValueReader (dataReader, _columnOrdinalProvider));
      else
        return null;
    }

    public IEnumerable<Tuple<ObjectID, object>> ReadSequence (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      var columnValueProvider = new ColumnValueReader (dataReader, _columnOrdinalProvider);
      while (dataReader.Read ())
      {
        yield return GetTimestampTuple (columnValueProvider);
      }
    }

    private Tuple<ObjectID, object> GetTimestampTuple (IColumnValueProvider columnValueProvider)
    {
      var objectIDValue = (ObjectID) _idProperty.CombineValue (columnValueProvider);
      if (objectIDValue == null)
        return null;

      var timestampValue = _timestampProperty.CombineValue (columnValueProvider);
      return Tuple.Create (objectIDValue, timestampValue);
    }
  }
}