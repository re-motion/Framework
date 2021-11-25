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
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.RuleCollectors;

namespace Remotion.Validation.MetaValidation
{
  /// <summary>
  /// Default implementation of the <see cref="IObjectMetaValidationRuleValidator"/> interface based on <see cref="IObjectMetaValidationRule"/>s.
  /// </summary>
  public class ObjectMetaValidationRuleValidator : IObjectMetaValidationRuleValidator
  {
    private readonly IObjectMetaValidationRuleCollector[] _addedObjectMetaValidationRuleCollectors;

    public ObjectMetaValidationRuleValidator (IObjectMetaValidationRuleCollector[] objectMetaValidationRuleCollectors)
    {
      ArgumentUtility.CheckNotNull("objectMetaValidationRuleCollectors", objectMetaValidationRuleCollectors);

      _addedObjectMetaValidationRuleCollectors = objectMetaValidationRuleCollectors;
    }

    public IEnumerable<MetaValidationRuleValidationResult> Validate (IAddingObjectValidationRuleCollector[] addingObjectValidationRulesCollectors)
    {
      ArgumentUtility.CheckNotNull("addingObjectValidationRulesCollectors", addingObjectValidationRulesCollectors);

      var objectValidatorsByValidatedType = addingObjectValidationRulesCollectors.ToLookup(c => c.ValidatedType, c => c.Validators);

      return from metaValidationRulesByValidatedType in _addedObjectMetaValidationRuleCollectors.ToLookup(c => c.ValidatedType)
          let validatedType = metaValidationRulesByValidatedType.Key
          let metaValidationRules = GetAllMetaValidationRules(metaValidationRulesByValidatedType)
          from metaValidationRule in metaValidationRules
              .SelectMany(mvr => mvr.Validate(objectValidatorsByValidatedType[validatedType].SelectMany(v => v)))
          select metaValidationRule;
    }

    private IEnumerable<IObjectMetaValidationRule> GetAllMetaValidationRules (
        IGrouping<ITypeInformation, IObjectMetaValidationRuleCollector> metaValidationRules)
    {
      return metaValidationRules.SelectMany(g => g.MetaValidationRules);
    }
  }
}