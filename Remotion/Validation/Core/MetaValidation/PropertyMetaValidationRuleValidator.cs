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
    private readonly IPropertyMetaValidationRuleCollector[] _addedPropertyMetaValidationRuleCollector;
    private readonly ISystemPropertyMetaValidationRuleProviderFactory _systemPropertyMetaValidationRuleProviderFactory;

    public PropertyMetaValidationRuleValidator (
        IPropertyMetaValidationRuleCollector[] propertyMetaValidationRuleCollector,
        ISystemPropertyMetaValidationRuleProviderFactory systemPropertyMetaValidationRuleProviderFactory)
    {
      ArgumentUtility.CheckNotNull ("propertyMetaValidationRuleCollector", propertyMetaValidationRuleCollector);
      ArgumentUtility.CheckNotNull ("systemPropertyMetaValidationRuleProviderFactory", systemPropertyMetaValidationRuleProviderFactory);

      _addedPropertyMetaValidationRuleCollector = propertyMetaValidationRuleCollector;
      _systemPropertyMetaValidationRuleProviderFactory = systemPropertyMetaValidationRuleProviderFactory;
    }

    public IEnumerable<MetaValidationRuleValidationResult> Validate (IAddingPropertyValidationRuleCollector[] addingPropertyValidationRulesCollector)
    {
      ArgumentUtility.CheckNotNull ("addingPropertyValidationRulesCollector", addingPropertyValidationRulesCollector);

      var propertyRulesByMemberInfo = addingPropertyValidationRulesCollector.ToLookup (pr => pr.Property, pr => pr.Validators);

      return from propertyRuleGroup in _addedPropertyMetaValidationRuleCollector.ToLookup (pr => pr.Property)
             let metaValidationRules = GetAllMetaValidationRules (propertyRuleGroup)
             from metaValidationRule in metaValidationRules
                 .SelectMany (mvr => mvr.Validate (propertyRulesByMemberInfo[propertyRuleGroup.Key].SelectMany (v => v)))
             select metaValidationRule;
    }

    private IEnumerable<IPropertyMetaValidationRule> GetAllMetaValidationRules (
        IGrouping<IPropertyInformation, IPropertyMetaValidationRuleCollector> propertyRuleGroup)
    {
      return _systemPropertyMetaValidationRuleProviderFactory.Create (propertyRuleGroup.Key)
          .GetSystemPropertyMetaValidationRules()
          .Concat (propertyRuleGroup.SelectMany (pr => pr.MetaValidationRules));
    }
  }
}