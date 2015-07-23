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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders
{
  /// <summary>
  /// Reads data from an <see cref="IDataReader"/> and converts it into <see cref="DataContainer"/> instances.
  /// The command whose data is converted must return an ID, a timestamp (as defined by the given <see cref="IRdbmsStoragePropertyDefinition"/> 
  /// instances), and values for each persistent property of the <see cref="ClassDefinition"/> matching the <see cref="ObjectID"/> read from the 
  /// <see cref="IDataReader"/>.
  /// </summary>
  public class DataContainerReader : IObjectReader<DataContainer>
  {
    private readonly IRdbmsStoragePropertyDefinition _idProperty;
    private readonly IRdbmsStoragePropertyDefinition _timestampProperty;
    private readonly IColumnOrdinalProvider _ordinalProvider;
    private readonly IRdbmsPersistenceModelProvider _persistenceModelProvider;
    private readonly IDataContainerValidator _dataContainerValidator;

    public DataContainerReader (
        IRdbmsStoragePropertyDefinition idProperty,
        IRdbmsStoragePropertyDefinition timestampProperty,
        IColumnOrdinalProvider ordinalProvider,
        IRdbmsPersistenceModelProvider persistenceModelProvider,
        IDataContainerValidator dataContainerValidator)
    {
      ArgumentUtility.CheckNotNull ("idProperty", idProperty);
      ArgumentUtility.CheckNotNull ("timestampProperty", timestampProperty);
      ArgumentUtility.CheckNotNull ("ordinalProvider", ordinalProvider);
      ArgumentUtility.CheckNotNull ("persistenceModelProvider", persistenceModelProvider);
      ArgumentUtility.CheckNotNull ("dataContainerValidator", dataContainerValidator);

      _idProperty = idProperty;
      _timestampProperty = timestampProperty;
      _ordinalProvider = ordinalProvider;
      _persistenceModelProvider = persistenceModelProvider;
      _dataContainerValidator = dataContainerValidator;
    }

    public IRdbmsStoragePropertyDefinition IDProperty
    {
      get { return _idProperty; }
    }

    public IRdbmsStoragePropertyDefinition TimestampProperty
    {
      get { return _timestampProperty; }
    }

    public IColumnOrdinalProvider OrdinalProvider
    {
      get { return _ordinalProvider; }
    }

    public IRdbmsPersistenceModelProvider PersistenceModelProvider
    {
      get { return _persistenceModelProvider; }
    }

    public IDataContainerValidator DataContainerValidator
    {
      get { return _dataContainerValidator; }
    }

    public virtual DataContainer Read (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      if (dataReader.Read())
        return CreateDataContainerFromReader (dataReader, new ColumnValueReader (dataReader, _ordinalProvider));
      else
        return null;
    }

    public virtual IEnumerable<DataContainer> ReadSequence (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      var columnValueReader = new ColumnValueReader (dataReader, _ordinalProvider);
      while (dataReader.Read())
      {
        yield return CreateDataContainerFromReader (dataReader, columnValueReader);
      }
    }

    protected virtual DataContainer CreateDataContainerFromReader (IDataReader dataReader, ColumnValueReader columnValueReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      var id = (ObjectID) _idProperty.CombineValue (columnValueReader);
      if (id == null)
        return null;

      var timestamp = _timestampProperty.CombineValue (columnValueReader);

      var dataContainer = DataContainer.CreateForExisting (
          id, 
          timestamp, 
          pd => pd.StorageClass == StorageClass.Persistent ? ReadPropertyValue (pd, columnValueReader, id) : pd.DefaultValue);

      _dataContainerValidator.Validate (dataContainer);

      return dataContainer;
    }

    private object ReadPropertyValue (PropertyDefinition propertyDefinition, IColumnValueProvider columnValueProvider, ObjectID id)
    {
      try
      {
        var storagePropertyDefinition = _persistenceModelProvider.GetStoragePropertyDefinition (propertyDefinition);
        return storagePropertyDefinition.CombineValue (columnValueProvider) ?? propertyDefinition.DefaultValue;
      }
      catch (Exception e)
      {
        var message = string.Format ("Error while reading property '{0}' of object '{1}': {2}", propertyDefinition.PropertyName, id, e.Message);
        throw new RdbmsProviderException (message, e);
      }
    }
  }
}