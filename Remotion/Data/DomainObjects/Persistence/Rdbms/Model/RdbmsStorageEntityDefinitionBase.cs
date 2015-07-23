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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// <see cref="RdbmsStorageEntityDefinitionBase"/> is the base-class for all entity definitions.
  /// </summary>
  public abstract class RdbmsStorageEntityDefinitionBase : IRdbmsStorageEntityDefinition
  {
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly EntityNameDefinition _viewName;

    private readonly ObjectIDStoragePropertyDefinition _objectIDProperty;
    private readonly IRdbmsStoragePropertyDefinition _timestampProperty;
    private readonly ReadOnlyCollection<IRdbmsStoragePropertyDefinition> _dataProperties;

    private readonly ReadOnlyCollection<IIndexDefinition> _indexes;
    private readonly ReadOnlyCollection<EntityNameDefinition> _synonyms;

    protected RdbmsStorageEntityDefinitionBase (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition viewName,
        ObjectIDStoragePropertyDefinition objectIDProperty,
        IRdbmsStoragePropertyDefinition timestampProperty,
        IEnumerable<IRdbmsStoragePropertyDefinition> dataProperties,
        IEnumerable<IIndexDefinition> indexes,
        IEnumerable<EntityNameDefinition> synonyms)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("objectIDProperty", objectIDProperty);
      ArgumentUtility.CheckNotNull ("timestampProperty", timestampProperty);
      ArgumentUtility.CheckNotNull ("dataProperties", dataProperties);
      ArgumentUtility.CheckNotNull ("synonyms", synonyms);

      _storageProviderDefinition = storageProviderDefinition;
      _viewName = viewName;

      _objectIDProperty = objectIDProperty;
      _timestampProperty = timestampProperty;
      _dataProperties = dataProperties.ToList().AsReadOnly();
      _indexes = indexes.ToList().AsReadOnly();
      _synonyms = synonyms.ToList().AsReadOnly();
    }

    public abstract void Accept (IRdbmsStorageEntityDefinitionVisitor visitor);

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public string StorageProviderID
    {
      get { return _storageProviderDefinition.Name; }
    }

    public EntityNameDefinition ViewName
    {
      get { return _viewName; }
    }

    public ObjectIDStoragePropertyDefinition ObjectIDProperty
    {
      get { return _objectIDProperty; }
    }

    public IRdbmsStoragePropertyDefinition TimestampProperty
    {
      get { return _timestampProperty; }
    }

    public IEnumerable<IRdbmsStoragePropertyDefinition> DataProperties
    {
      get { return _dataProperties; }
    }

    public IEnumerable<IRdbmsStoragePropertyDefinition> GetAllProperties ()
    {
      yield return _objectIDProperty;
      yield return _timestampProperty;

      foreach (var storagePropertyDefinition in _dataProperties)
        yield return storagePropertyDefinition;
    }

    public IEnumerable<ColumnDefinition> GetAllColumns ()
    {
      return GetAllProperties().SelectMany (p => p.GetColumns());
    }

    public ReadOnlyCollection<IIndexDefinition> Indexes
    {
      get { return _indexes; }
    }

    public ReadOnlyCollection<EntityNameDefinition> Synonyms
    {
      get { return _synonyms; }
    }
  }
}