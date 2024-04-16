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
using System.Text.RegularExpressions;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Logical
{
  /// <summary>
  /// Validates that the given <see cref="TypeDefinition"/> has a valid class-ID containing only letters, digits, and underscores.
  /// </summary>
  public class CheckForClassIDIsValidValidationRule : ITypeDefinitionValidationRule
  {
    private const string ClassIDPattern = @"^(?:\p{L}|_)(?:\p{L}|_|\p{N})*$";
    private static readonly Regex s_validClassID = new Regex(ClassIDPattern, RegexOptions.Compiled);

    public CheckForClassIDIsValidValidationRule ()
    {
    }

    public MappingValidationResult Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      if (typeDefinition is not ClassDefinition classDefinition)
        return MappingValidationResult.CreateValidResult();

      if (!s_validClassID.IsMatch(classDefinition.ID))
      {
        return MappingValidationResult.CreateInvalidResultForType(
            typeDefinition.Type,
            "The Class-ID '{0}' is not valid. Valid Class-IDs must start with a letter or underscore and containing only letters, digits, and underscores.",
            classDefinition.ID);
      }

      return MappingValidationResult.CreateValidResult();
    }
  }
}
