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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation
{
  /// <summary>
  /// The <see cref="SortExpressionValidator"/> validates the sort expression text of the <see cref="DBBidirectionalRelationAttribute"/> 
  /// on the <see cref="PropertyInfo"/> of the <see cref="TypeDefinition"/>s.
  /// </summary>
  public class SortExpressionValidator : ISortExpressionValidator
  {
    private readonly ReadOnlyCollection<IRelationDefinitionValidatorRule> _validationRules;

    public SortExpressionValidator (params IRelationDefinitionValidatorRule[] validationRules)
    {
      ArgumentUtility.CheckNotNull("validationRules", validationRules);

      _validationRules = Array.AsReadOnly(validationRules);
    }

    public ReadOnlyCollection<IRelationDefinitionValidatorRule> ValidationRules
    {
      get { return _validationRules; }
    }

    public IEnumerable<MappingValidationResult> Validate (IEnumerable<RelationDefinition> relationDefinitions)
    {
      ArgumentUtility.CheckNotNull("relationDefinitions", relationDefinitions);

      return from rule in _validationRules
             from relationDefinition in relationDefinitions
             let result = rule.Validate(relationDefinition)
             where !result.IsValid
             select result;
    }
  }
}
