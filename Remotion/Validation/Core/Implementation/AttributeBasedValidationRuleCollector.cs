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
using Remotion.Utilities;
using Remotion.Validation.Attributes;
using Remotion.Validation.RuleCollectors;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// TODO RM-5906: doc
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ApplyProgrammatically]
  public class AttributeBasedValidationRuleCollector : IValidationRuleCollector
  {
    private readonly Type _validatedType;
    private readonly IReadOnlyCollection<IAddingPropertyValidationRuleCollector> _addedPropertyRules;
    private readonly IReadOnlyCollection<IPropertyMetaValidationRuleCollector> _addedPropertyMetaValidationRules;
    private readonly IReadOnlyCollection<IRemovingPropertyValidationRuleCollector> _removedPropertyRules;

    public AttributeBasedValidationRuleCollector (
        Type validatedType,
        IEnumerable<IAddingPropertyValidationRuleCollector> addedPropertyRules,
        IEnumerable<IPropertyMetaValidationRuleCollector> addedPropertyMetaValidationRules,
        IEnumerable<IRemovingPropertyValidationRuleCollector> removedPropertyRules)
    {
      ArgumentUtility.CheckNotNull("validatedType", validatedType);
      ArgumentUtility.CheckNotNull("addedPropertyRules", addedPropertyRules);
      ArgumentUtility.CheckNotNull("addedPropertyMetaValidationRules", addedPropertyMetaValidationRules);
      ArgumentUtility.CheckNotNull("removedPropertyRules", removedPropertyRules);

      _validatedType = validatedType;
      _addedPropertyRules = addedPropertyRules.ToList().AsReadOnly();
      _addedPropertyMetaValidationRules = addedPropertyMetaValidationRules.ToList().AsReadOnly();
      _removedPropertyRules = removedPropertyRules.ToList().AsReadOnly();
    }

    public Type ValidatedType
    {
      get { return _validatedType; }
    }

    public IReadOnlyCollection<IAddingPropertyValidationRuleCollector> AddedPropertyRules
    {
      get { return _addedPropertyRules; }
    }

    public IReadOnlyCollection<IPropertyMetaValidationRuleCollector> PropertyMetaValidationRules
    {
      get { return _addedPropertyMetaValidationRules; }
    }

    public IReadOnlyCollection<IRemovingPropertyValidationRuleCollector> RemovedPropertyRules
    {
      get { return _removedPropertyRules; }
    }

    public IReadOnlyCollection<IAddingObjectValidationRuleCollector> AddedObjectRules => Array.Empty<IAddingObjectValidationRuleCollector>();

    public IReadOnlyCollection<IObjectMetaValidationRuleCollector> ObjectMetaValidationRules => Array.Empty<IObjectMetaValidationRuleCollector>();

    public IReadOnlyCollection<IRemovingObjectValidationRuleCollector> RemovedObjectRules => Array.Empty<IRemovingObjectValidationRuleCollector>();
  }
}