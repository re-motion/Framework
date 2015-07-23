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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Adapts a scalar query result to implement the <see cref="IDatabaseResultRow"/> interface. 
  /// </summary>
  public class ScalarResultRowAdapter : IDatabaseResultRow
  {
    private readonly object _scalarValue;
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;

    public ScalarResultRowAdapter (object scalarValue, IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("scalarValue", scalarValue);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      _scalarValue = scalarValue;
      _storageTypeInformationProvider = storageTypeInformationProvider;
    }

    public object ScalarValue
    {
      get { return _scalarValue; }
    }

    public IStorageTypeInformationProvider StorageTypeInformationProvider
    {
      get { return _storageTypeInformationProvider; }
    }

    public T GetValue<T> (ColumnID columnID)
    {
      ArgumentUtility.CheckNotNull ("columnID", columnID);

      if (columnID.Position != 0)
      {
        var message = String.Format ("Only one scalar value is available, column ID '{0}' is invalid.", columnID);
        throw new IndexOutOfRangeException (message);
      }

      var storageTypeInformation = _storageTypeInformationProvider.GetStorageType (typeof (T));
      return (T) storageTypeInformation.ConvertFromStorageType (_scalarValue);
    }

    public T GetEntity<T> (params ColumnID[] columnIDs)
    {
      throw new NotSupportedException ("Scalar queries cannot return entities.");
    }
  }
}