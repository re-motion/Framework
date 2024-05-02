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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

/// <summary>
/// Can create <see cref="IDbDataParameter"/> instances for <see cref="ObjectID"/> parameter values that refer to <see cref="DomainObject"/>s stored with the same
/// <see cref="StorageProviderDefinition"/>.
/// </summary>
public class ObjectIDDataParameterDefinition : IDataParameterDefinition
{
  public IStorageTypeInformation ValueStorageTypeInformation { get; }

  /// <summary>
  /// Creates a new <see cref="ObjectIDDataParameterDefinition"/> instance.
  /// </summary>
  /// <param name="storageTypeInformation">The <see cref="IStorageTypeInformation"/> associated with the <see cref="Type"/> of the <see cref="ObjectID"/>'s <see cref="ObjectID.Value"/>.</param>
  public ObjectIDDataParameterDefinition (IStorageTypeInformation storageTypeInformation)
  {
    ArgumentUtility.CheckNotNull(nameof(storageTypeInformation), storageTypeInformation);

    ValueStorageTypeInformation = storageTypeInformation;
  }

  /// <inheritdoc />
  public object GetParameterValue (object? value)
  {
    var objectID = ArgumentUtility.CheckType<ObjectID?>(nameof(value), value);
    if (objectID == null)
      return DBNull.Value;

    var objectIDValue = objectID.Value;
    ArgumentUtility.CheckType("objectID.Value", objectIDValue, ValueStorageTypeInformation.DotNetType);

    return ValueStorageTypeInformation.ConvertToStorageType(objectIDValue);
  }

  /// <inheritdoc />
  public IDbDataParameter CreateDataParameter (IDbCommand command, string parameterName, object parameterValue)
  {
    ArgumentUtility.CheckNotNull(nameof(command), command);
    ArgumentUtility.CheckNotNullOrEmpty(nameof(parameterName), parameterName);
    ArgumentUtility.CheckNotNull(nameof(parameterValue), parameterValue);

    var parameter = command.CreateParameter();
    parameter.ParameterName = parameterName;
    parameter.Value = parameterValue;
    parameter.DbType = ValueStorageTypeInformation.StorageDbType;
    if (ValueStorageTypeInformation.StorageTypeLength.HasValue)
      parameter.Size = ValueStorageTypeInformation.StorageTypeLength.Value;

    return parameter;
  }
}
