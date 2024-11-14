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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

/// <summary>
/// Can create <see cref="IDataParameterDefinition"/> instances that can handle simple (atomic) parameter values.
/// </summary>
public class SimpleDataParameterDefinitionFactory : IDataParameterDefinitionFactory
{
  public IStorageTypeInformationProvider StorageTypeInformationProvider { get; }

  public SimpleDataParameterDefinitionFactory (IStorageTypeInformationProvider storageTypeInformationProvider)
  {
    ArgumentUtility.CheckNotNull(nameof(storageTypeInformationProvider), storageTypeInformationProvider);

    StorageTypeInformationProvider = storageTypeInformationProvider;
  }

  public IDataParameterDefinition CreateDataParameterDefinition (QueryParameter queryParameter, IQuery query)
  {
    ArgumentUtility.CheckNotNull(nameof(queryParameter), queryParameter);
    ArgumentUtility.CheckNotNull(nameof(query), query);

    try
    {
      var storageTypeInformation = StorageTypeInformationProvider.GetStorageType(queryParameter.Value);
      return new SimpleDataParameterDefinition(storageTypeInformation);
    }
    catch (NotSupportedException)
    {
      throw new NotSupportedException($"Objects of type '{queryParameter.Value!.GetType()}' cannot be used as data parameter value.");
    }
  }
}
