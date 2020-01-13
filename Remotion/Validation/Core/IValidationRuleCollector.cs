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
using Remotion.Validation.RuleCollectors;

namespace Remotion.Validation
{
  /// <summary>
  /// Defines an API to retrieve the component-specific rules for providing the validation semantic of a <see cref="Type"/>.
  /// </summary>
  public interface IValidationRuleCollector
  {
    /// <summary>
    /// Returns the <see cref="Type"/> that gets validated.
    /// </summary>
    Type ValidatedType { get; }

    /// <summary>
    /// Gets the <see cref="IAddingPropertyValidationRuleCollector"/>s added to the validation semantic of the <see cref="ValidatedType"/>.
    /// </summary>
    IReadOnlyCollection<IAddingPropertyValidationRuleCollector> AddedPropertyRules { get; }

    /// <summary>
    /// Gets the <see cref="IPropertyMetaValidationRuleCollector"/>s added to the validation semantic of the <see cref="ValidatedType"/>.
    /// Meta validation rules are used to ensure that the validation semantic of the type is still consistent after all validation rules 
    /// from all components are applied.
    /// </summary>
    IReadOnlyCollection<IPropertyMetaValidationRuleCollector> PropertyMetaValidationRules { get; }

    /// <summary>
    /// Gets the <see cref="IRemovingPropertyValidationRuleCollector"/>s used to remove validation rules from the <see cref="ValidatedType"/>.
    /// </summary>
    IReadOnlyCollection<IRemovingPropertyValidationRuleCollector> RemovedPropertyRules { get; }

    /// <summary>
    /// Gets the <see cref="IAddingObjectValidationRuleCollector"/>s added to the validation semantic of the <see cref="ValidatedType"/>.
    /// </summary>
    IReadOnlyCollection<IAddingObjectValidationRuleCollector> AddedObjectRules { get; }

    /// <summary>
    /// Gets the <see cref="IObjectMetaValidationRuleCollector"/>s added to the validation semantic of the <see cref="ValidatedType"/>.
    /// Meta validation rules are used to ensure that the validation semantic of the type is still consistent after all validation rules 
    /// from all components are applied.
    /// </summary>
    IReadOnlyCollection<IObjectMetaValidationRuleCollector> ObjectMetaValidationRules { get; }

    /// <summary>
    /// Gets the <see cref="IRemovingObjectValidationRuleCollector"/>s used to remove validation rules from the <see cref="ValidatedType"/>.
    /// </summary>
    IReadOnlyCollection<IRemovingObjectValidationRuleCollector> RemovedObjectRules { get; }
  }
}