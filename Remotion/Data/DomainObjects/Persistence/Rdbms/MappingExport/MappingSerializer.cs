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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport
{
  /// <summary>
  /// The <see cref="MappingSerializer"/> is responsible for creating an <see cref="XDocument"/> that contains an xml representation of the class to database mapping.
  /// </summary>
  public class MappingSerializer
  {
    private readonly Func<RdbmsProviderDefinition, IEnumSerializer> _enumSerializerFactory;
    private readonly Func<RdbmsProviderDefinition, IEnumSerializer, IStorageProviderSerializer> _storageProviderSerializerFactory;

    public MappingSerializer (
        Func<RdbmsProviderDefinition, IEnumSerializer> enumSerializerFactory,
        Func<RdbmsProviderDefinition, IEnumSerializer, IStorageProviderSerializer> storageProviderSerializerFactory)
    {
      ArgumentUtility.CheckNotNull("enumSerializerFactory", enumSerializerFactory);
      ArgumentUtility.CheckNotNull("storageProviderSerializerFactory", storageProviderSerializerFactory);

      _enumSerializerFactory = enumSerializerFactory;
      _storageProviderSerializerFactory = storageProviderSerializerFactory;
    }

    public XDocument Serialize (IEnumerable<TypeDefinition> typeDefinitions)
    {
      ArgumentUtility.CheckNotNull("typeDefinitions", typeDefinitions);

      var classDefinitionsByStorageProvider = typeDefinitions
          .Where(td => td.StorageEntityDefinition.StorageProviderDefinition is RdbmsProviderDefinition)
          .GroupBy(td => (RdbmsProviderDefinition)td.StorageEntityDefinition.StorageProviderDefinition);

      var mappingElement = new XElement(Constants.Namespace + "mapping");
      var enumElements = new List<XElement>();

      foreach (var group in classDefinitionsByStorageProvider)
      {
        var rdbmsProviderDefinition = @group.Key;
        var enumSerializer = _enumSerializerFactory(rdbmsProviderDefinition);
        var storageProviderSerializer = _storageProviderSerializerFactory(rdbmsProviderDefinition, enumSerializer);

        mappingElement.Add(storageProviderSerializer.Serialize(@group, rdbmsProviderDefinition));
        enumElements.AddRange(enumSerializer.Serialize());
      }
      mappingElement.Add(enumElements);

      return new XDocument(mappingElement);
    }
  }
}
