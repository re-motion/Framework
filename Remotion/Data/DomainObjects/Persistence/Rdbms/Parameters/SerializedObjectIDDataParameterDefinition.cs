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
using Remotion.Data.DomainObjects.Infrastructure.ObjectIDStringSerialization;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

/// <summary>
/// Can create <see cref="IDbDataParameter"/> instances for <see cref="ObjectID"/> parameter values that refer to <see cref="DomainObject"/>s stored outside the current
/// <see cref="StorageProviderDefinition"/>.
/// </summary>
public class SerializedObjectIDDataParameterDefinition : IDataParameterDefinition
{
  public IStorageTypeInformation StorageTypeInformation { get; }

  public SerializedObjectIDDataParameterDefinition (IStorageTypeInformation storageTypeInformation)
  {
    ArgumentUtility.CheckNotNull(nameof(storageTypeInformation), storageTypeInformation);
    ArgumentUtility.CheckTypeIsAssignableFrom("storageTypeInformation.DotNetType", storageTypeInformation.DotNetType, typeof(string));

    StorageTypeInformation = storageTypeInformation;
  }

  public object GetParameterValue (object? value)
  {
    var objectID = ArgumentUtility.CheckType<ObjectID>(nameof(value), value);
    if (objectID == null)
      return DBNull.Value;

    var serializedObjectID = ObjectIDStringSerializer.Instance.Serialize(objectID);
    return StorageTypeInformation.ConvertToStorageType(serializedObjectID);
  }

  public IDbDataParameter CreateDataParameter (IDbCommand command, string parameterName, object parameterValue)
  {
    ArgumentUtility.CheckNotNull(nameof(command), command);
    ArgumentUtility.CheckNotNullOrEmpty(nameof(parameterName), parameterName);
    ArgumentUtility.CheckNotNull(nameof(parameterValue), parameterValue);

    var parameter = command.CreateParameter();
    parameter.ParameterName = parameterName;
    parameter.Value = parameterValue;
    parameter.DbType = StorageTypeInformation.StorageDbType;
    parameter.Size = StorageTypeInformation.StorageTypeLength ?? SqlStorageTypeInformationProvider.StorageTypeLengthRepresentingMax;
    return parameter;
  }
}
