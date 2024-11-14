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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

/// <summary>
/// Can create <see cref="IDataParameterDefinition"/> instances that can handle <see cref="ObjectID"/> parameter values.
/// </summary>
public class ObjectIDDataParameterDefinitionFactory : IDataParameterDefinitionFactory
{
  public IStorageSettings StorageSettings { get; }
  public StorageProviderDefinition StorageProviderDefinition { get; }
  public IStorageTypeInformationProvider StorageTypeInformationProvider { get; }
  public IDataParameterDefinitionFactory NextDataParameterDefinitionFactory { get; }

  public ObjectIDDataParameterDefinitionFactory (StorageProviderDefinition storageProviderDefinition,
      IStorageTypeInformationProvider storageTypeInformationProvider,
      IStorageSettings storageSettings,
      IDataParameterDefinitionFactory nextDataParameterDefinitionFactory)
  {
    ArgumentUtility.CheckNotNull(nameof(storageProviderDefinition), storageProviderDefinition);
    ArgumentUtility.CheckNotNull(nameof(storageTypeInformationProvider), storageTypeInformationProvider);
    ArgumentUtility.CheckNotNull(nameof(storageSettings), storageSettings);
    NextDataParameterDefinitionFactory = nextDataParameterDefinitionFactory;

    StorageSettings = storageSettings;
    StorageProviderDefinition = storageProviderDefinition;
    StorageTypeInformationProvider = storageTypeInformationProvider;
  }

  public IDataParameterDefinition CreateDataParameterDefinition (QueryParameter queryParameter, IQuery query)
  {
    ArgumentUtility.CheckNotNull(nameof(queryParameter), queryParameter);
    ArgumentUtility.CheckNotNull(nameof(query), query);

    var objectID = queryParameter.Value as ObjectID;
    if (objectID != null)
      return CreateDataParameterDefinition(objectID);

    return NextDataParameterDefinitionFactory.CreateDataParameterDefinition(queryParameter, query);
  }

  private IDataParameterDefinition CreateDataParameterDefinition (ObjectID objectID)
  {
    var relatedStorageProviderDefinition = StorageSettings.GetStorageProviderDefinition(objectID.ClassDefinition);
    if (StorageProviderDefinition == relatedStorageProviderDefinition)
      return CreateSameProviderRelationStoragePropertyDefinition();

    return CreateCrossProviderRelationStoragePropertyDefinition();
  }

  private IDataParameterDefinition CreateSameProviderRelationStoragePropertyDefinition ()
  {
    var storageTypeInformation = StorageTypeInformationProvider.GetStorageTypeForID(true);
    return new ObjectIDDataParameterDefinition(storageTypeInformation);
  }

  private IDataParameterDefinition CreateCrossProviderRelationStoragePropertyDefinition ()
  {
    var storageTypeInfo = StorageTypeInformationProvider.GetStorageTypeForSerializedObjectID(true);
    return new SerializedObjectIDDataParameterDefinition(storageTypeInfo);
  }
}
