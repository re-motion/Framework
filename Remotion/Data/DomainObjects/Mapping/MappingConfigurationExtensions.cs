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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Provides extension methods used for <see cref="IMappingConfiguration"/>.
  /// </summary>
  public static class MappingConfigurationExtensions
  {
    public static TypeDefinition GetTypeDefinition (this IMappingConfiguration mappingConfiguration, Type type)
    {
      ArgumentUtility.CheckNotNull(nameof(mappingConfiguration), mappingConfiguration);
      ArgumentUtility.CheckNotNull(nameof(type), type);

      return mappingConfiguration.GetTypeDefinition(type, static type => CreateTypeMappingException(type.ToString()));
    }

    public static bool ContainsClassDefinition (this IMappingConfiguration mappingConfiguration, Type type)
    {
      ArgumentUtility.CheckNotNull(nameof(mappingConfiguration), mappingConfiguration);
      ArgumentUtility.CheckNotNull(nameof(type), type);

      return type.IsClass && mappingConfiguration.ContainsTypeDefinition(type);
    }

    public static ClassDefinition GetClassDefinition (this IMappingConfiguration mappingConfiguration, Type type)
    {
      ArgumentUtility.CheckNotNull(nameof(mappingConfiguration), mappingConfiguration);
      ArgumentUtility.CheckNotNull(nameof(type), type);
      if (!type.IsClass)
        throw new ArgumentException($"The specified type '{type.GetFullNameSafe()}' must be a class.", nameof(type));

      return (ClassDefinition)mappingConfiguration.GetTypeDefinition(type);
    }

    public static ClassDefinition GetClassDefinition (this IMappingConfiguration mappingConfiguration, Type type, Func<Type, Exception> missingClassDefinitionExceptionFactory)
    {
      ArgumentUtility.CheckNotNull(nameof(mappingConfiguration), mappingConfiguration);
      ArgumentUtility.CheckNotNull(nameof(type), type);
      ArgumentUtility.CheckNotNull(nameof(missingClassDefinitionExceptionFactory), missingClassDefinitionExceptionFactory);
      if (!type.IsClass)
        throw new ArgumentException($"The specified type '{type.GetFullNameSafe()}' must be a class.", nameof(type));

      return (ClassDefinition)mappingConfiguration.GetTypeDefinition(type, missingClassDefinitionExceptionFactory);
    }

    public static ClassDefinition GetClassDefinition (this IMappingConfiguration mappingConfiguration, string classID)
    {
      ArgumentUtility.CheckNotNull(nameof(mappingConfiguration), mappingConfiguration);
      ArgumentUtility.CheckNotNull(nameof(classID), classID);

      return mappingConfiguration.GetClassDefinition(classID, static id => CreateClassMappingException(id));
    }

    private static MappingException CreateClassMappingException (string id)
    {
      return new MappingException($"Mapping does not contain class '{id}'.");
    }

    private static MappingException CreateTypeMappingException (string type)
    {
      return new MappingException($"Mapping does not contain type '{type}'.");
    }
  }
}
