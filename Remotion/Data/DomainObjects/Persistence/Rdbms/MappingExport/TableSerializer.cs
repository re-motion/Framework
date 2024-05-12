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
using System.Linq;
using System.Xml.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport
{
  /// <summary>
  /// Default implementation of the <see cref="ITableSerializer"/> interface.
  /// </summary>
  public class TableSerializer : ITableSerializer
  {
    private readonly IPropertySerializer _propertySerializer;

    public TableSerializer (IPropertySerializer propertySerializer)
    {
      ArgumentUtility.CheckNotNull("propertySerializer", propertySerializer);

      _propertySerializer = propertySerializer;
    }

    public IEnumerable<XElement> Serialize (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      var tableDefinition = GetTableDefinition(typeDefinition);
      if (tableDefinition == null)
        yield break;

      var storageProviderDefinition = (RdbmsProviderDefinition)typeDefinition.StorageEntityDefinition.StorageProviderDefinition;
      var persistenceModelProvider = storageProviderDefinition.Factory.CreateRdbmsPersistenceModelProvider(storageProviderDefinition);

      yield return new XElement(
          Constants.Namespace + "table",
          new XAttribute("name", tableDefinition.TableName.EntityName),
          GetPersistentPropertyDefinitions(typeDefinition)
              .Select(p => _propertySerializer.Serialize(p, persistenceModelProvider))
          );
    }

    private TableDefinition? GetTableDefinition (TypeDefinition typeDefinition)
    {
      if (typeDefinition.StorageEntityDefinition is FilterViewDefinition)
        return ((FilterViewDefinition)typeDefinition.StorageEntityDefinition).GetBaseTable();

      if (typeDefinition.StorageEntityDefinition is TableDefinition)
        return (TableDefinition)typeDefinition.StorageEntityDefinition;

      return null;
    }

    private IEnumerable<PropertyDefinition> GetPersistentPropertyDefinitions (TypeDefinition classDefinition)
    {
      return classDefinition.GetPropertyDefinitions().Where(p => p.StorageClass == StorageClass.Persistent);
    }
  }
}
