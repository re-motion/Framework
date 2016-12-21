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
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders
{
  /// <summary>
  /// The <see cref="NameBasedColumnOrdinalProvider"/> calculates the index of a <see cref="ColumnDefinition"/> based on its column name.
  /// </summary>
  public class NameBasedColumnOrdinalProvider : IColumnOrdinalProvider
  {
    public int GetOrdinal (ColumnDefinition columnDefinition, IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      try
      {
        return dataReader.GetOrdinal (columnDefinition.Name);
      }
      catch (IndexOutOfRangeException ex)
      {
        var message = string.Format (
          "The column '{0}' was not found in the query result. The included columns are: {1}.",
          columnDefinition.Name,
          string.Join (", ", Enumerable.Range (0, dataReader.FieldCount).Select (dataReader.GetName)));
        throw new RdbmsProviderException (message, ex);
      }
      
    }
  }
}