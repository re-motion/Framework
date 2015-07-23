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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders
{
  /// <summary>
  /// Reads data from an <see cref="IDataReader"/> and converts it into <see cref="IQueryResultRow"/> instances.
  /// </summary>
  public class QueryResultRowReader : IObjectReader<IQueryResultRow>
  {
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;

    public QueryResultRowReader (IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      _storageTypeInformationProvider = storageTypeInformationProvider;
    }

    public IStorageTypeInformationProvider StorageTypeInformationProvider
    {
      get { return _storageTypeInformationProvider; }
    }

    public IQueryResultRow Read (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      if (dataReader.Read())
        return CreateResultRowFromReader (dataReader);
      else
        return null;
    }

    public IEnumerable<IQueryResultRow> ReadSequence (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      while (dataReader.Read())
        yield return CreateResultRowFromReader (dataReader);
    }

    protected virtual IQueryResultRow CreateResultRowFromReader (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      return new QueryResultRow (dataReader, _storageTypeInformationProvider);
    }
  }
}