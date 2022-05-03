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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public class InterfaceReflector
  {
    private readonly Type _type;

    public InterfaceReflector (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      _type = type;
    }

    public InterfaceDefinition GetMetadata (IEnumerable<InterfaceDefinition> extendedInterfaces)
    {
      ArgumentUtility.CheckNotNull("extendedInterfaces", extendedInterfaces);

      var storageGroupAttribute = GetStorageGroupAttribute();
      var storageGroupType = storageGroupAttribute?.GetType();
      var defaultStorageClass = storageGroupAttribute?.DefaultStorageClass ?? DefaultStorageClass.Persistent;
      var interfaceDefinition = new InterfaceDefinition(
          _type,
          extendedInterfaces,
          storageGroupType,
          defaultStorageClass);

      // todo R2I Mapping: Add properties to interface definition
      interfaceDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());

      // todo R2I Mapping: Add relation end points to interface definition
      interfaceDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());

      return interfaceDefinition;
    }

    private StorageGroupAttribute? GetStorageGroupAttribute ()
    {
      return AttributeUtility.GetCustomAttributes<StorageGroupAttribute>(_type, true).FirstOrDefault();
    }
  }
}
