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
using System.Reflection;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Reflection
{
  /// <summary>
  /// Validates that the domain object does not have a legacy infrastructure constructor taking a single data container argument.
  /// </summary>
  public class DomainObjectTypeDoesNotHaveLegacyInfrastructureConstructorValidationRule : ITypeDefinitionValidationRule
  {
    public DomainObjectTypeDoesNotHaveLegacyInfrastructureConstructorValidationRule ()
    {

    }

    public MappingValidationResult Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      return Validate(typeDefinition.Type);
    }

    private MappingValidationResult Validate (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      if (!type.IsAbstract || (type.IsAbstract && Attribute.IsDefined(type, typeof(InstantiableAttribute), false)))
      {
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.ExactBinding;
        ConstructorInfo? legacyLoadConstructor = type.GetConstructor(flags, null, new[] { typeof(DataContainer) }, null);
        if (legacyLoadConstructor != null)
        {
          return MappingValidationResult.CreateInvalidResultForType(
              type,
              "The domain object type has a legacy infrastructure constructor for loading (a nonpublic constructor taking a single DataContainer "
              + "argument). The reflection-based mapping does not use this constructor any longer and requires it to be removed.");
        }
      }
      return MappingValidationResult.CreateValidResult();
    }
  }
}
