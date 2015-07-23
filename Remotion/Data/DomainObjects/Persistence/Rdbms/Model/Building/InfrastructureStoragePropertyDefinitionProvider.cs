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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="InfrastructureStoragePropertyDefinitionProvider"/> class is responsible to create 
  /// <see cref="IRdbmsStoragePropertyDefinition"/> objects for infrastructure columns.
  /// </summary>
  public class InfrastructureStoragePropertyDefinitionProvider : IInfrastructureStoragePropertyDefinitionProvider
  {
    private readonly ColumnDefinition _idColumnDefinition;
    private readonly ColumnDefinition _classIDColumnDefinition;
    private readonly ColumnDefinition _timestampColumnDefinition;

    private readonly ObjectIDStoragePropertyDefinition _objectIDStoragePropertyDefinition;
    private readonly IRdbmsStoragePropertyDefinition _timestampStoragePropertyDefinition;

    public InfrastructureStoragePropertyDefinitionProvider (
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);

      _idColumnDefinition = new ColumnDefinition (
          storageNameProvider.GetIDColumnName(),
          storageTypeInformationProvider.GetStorageTypeForID (false),
          true);
      _classIDColumnDefinition = new ColumnDefinition (
          storageNameProvider.GetClassIDColumnName(),
          storageTypeInformationProvider.GetStorageTypeForClassID (false),
          false);
      _timestampColumnDefinition = new ColumnDefinition (
          storageNameProvider.GetTimestampColumnName(),
          storageTypeInformationProvider.GetStorageTypeForTimestamp (false),
          false);

      _objectIDStoragePropertyDefinition = new ObjectIDStoragePropertyDefinition (
          new SimpleStoragePropertyDefinition (typeof (object), _idColumnDefinition), 
          new SimpleStoragePropertyDefinition (typeof (string), _classIDColumnDefinition));
      _timestampStoragePropertyDefinition = new SimpleStoragePropertyDefinition (typeof (object), _timestampColumnDefinition);
    }

    public ObjectIDStoragePropertyDefinition GetObjectIDStoragePropertyDefinition ()
    {
      return _objectIDStoragePropertyDefinition;
    }

    public IRdbmsStoragePropertyDefinition GetTimestampStoragePropertyDefinition ()
    {
      return _timestampStoragePropertyDefinition;
    }
  }
}