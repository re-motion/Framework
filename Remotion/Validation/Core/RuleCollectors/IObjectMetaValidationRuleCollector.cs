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
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.RuleCollectors
{
  /// <summary>
  /// Defines a rule which ensures the consistency of the merged set of <see cref="IObjectValidator"/>s for the <see cref="ValidatedType"/>. 
  /// The rules belong to a component via the <see cref="CollectorType"/> and are applied to the validation specification if the component is used within the application.
  /// </summary>
  /// <seealso cref="ObjectMetaValidationRuleCollector"/>
  public interface IObjectMetaValidationRuleCollector
  {
    /// <summary>
    /// Gets the <see cref="Type"/> of the <see cref="IValidationRuleCollector"/> with which the rule is associated.
    /// </summary>
    Type CollectorType { get; }

    /// <summary>
    /// Gets the property for which the validation specification will be verified.
    /// </summary>
    ITypeInformation ValidatedType { get; }

    /// <summary>
    /// Gets the set of <see cref="IObjectMetaValidationRule"/>s registered for the <see cref="ValidatedType"/> by the <see cref="CollectorType"/>.
    /// </summary>
    IEnumerable<IObjectMetaValidationRule> MetaValidationRules { get; }

    /// <summary>
    /// Registers a <see cref="IObjectMetaValidationRule"/> for the <see cref="ValidatedType"/> by the <see cref="CollectorType"/>.
    /// </summary>
    void RegisterMetaValidationRule ([NotNull] IObjectMetaValidationRule metaValidationRule);
  }
}