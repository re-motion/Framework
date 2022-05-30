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
  /// Default implementation of the <see cref="IPropertyMetaValidationRuleValidator"/> interface based on <see cref="IPropertyMetaValidationRule"/>s.
  /// </summary>
  public class PropertyMetaValidationRuleValidator : IPropertyMetaValidationRuleValidator
  {
    private readonly IPropertyMetaValidationRuleCollector[] _addedPropertyMetaValidationRuleCollectors;
    private readonly ISystemPropertyMetaValidationRuleProviderFactory _systemPropertyMetaValidationRuleProviderFactory;

    public PropertyMetaValidationRuleValidator (
        IPropertyMetaValidationRuleCollector[] propertyMetaValidationRuleCollectors,
        ISystemPropertyMetaValidationRuleProviderFactory systemPropertyMetaValidationRuleProviderFactory)
    {
      ArgumentUtility.CheckNotNull("propertyMetaValidationRuleCollectors", propertyMetaValidationRuleCollectors);
      ArgumentUtility.CheckNotNull("systemPropertyMetaValidationRuleProviderFactory", systemPropertyMetaValidationRuleProviderFactory);

      _addedPropertyMetaValidationRuleCollectors = propertyMetaValidationRuleCollectors;
      _systemPropertyMetaValidationRuleProviderFactory = systemPropertyMetaValidationRuleProviderFactory;
    }

    public IEnumerable<MetaValidationRuleValidationResult> Validate (IAddingPropertyValidationRuleCollector[] addingPropertyValidationRulesCollectors)
    {
      ArgumentUtility.CheckNotNull("addingPropertyValidationRulesCollectors", addingPropertyValidationRulesCollectors);

      var propertyRulesByValidatedProperty = addingPropertyValidationRulesCollectors.ToLookup(c => c.Property, c => c.Validators);

      var lookup = _addedPropertyMetaValidationRuleCollectors.ToLookup(c => c.Property);
      return from propertyRuleGroup in lookup
          let validatedProperty = propertyRuleGroup.Key
          let metaValidationRules = GetAllMetaValidationRules(propertyRuleGroup)
          from metaValidationRule in metaValidationRules
              .SelectMany(mvr => mvr.Validate(propertyRulesByValidatedProperty[validatedProperty].SelectMany(v => v)))
          select metaValidationRule;
    }

    private IEnumerable<IPropertyMetaValidationRule> GetAllMetaValidationRules (
        IGrouping<IPropertyInformation, IPropertyMetaValidationRuleCollector> propertyRuleGroup)
    {
      return _systemPropertyMetaValidationRuleProviderFactory.Create(propertyRuleGroup.Key)
          .GetSystemPropertyMetaValidationRules()
          .Concat(propertyRuleGroup.SelectMany(pr => pr.MetaValidationRules));
    }
  }
}
