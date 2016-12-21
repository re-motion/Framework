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
using FluentValidation.Validators;
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Validation.MetaValidation;

namespace Remotion.Validation.Rules
{
  /// <summary>
  /// Defines a rule which ensures the consistency of the merged set of <see cref="IPropertyValidator"/>s for the <see cref="Property"/>. 
  /// The rules belong to a component via the <see cref="CollectorType"/> and are applied to the validation specification if the component is used within the application.
  /// </summary>
  /// <seealso cref="AddingComponentPropertyMetaValidationRule"/>
  public interface IAddingComponentPropertyMetaValidationRule
  {
    /// <summary>
    /// Gets the <see cref="Type"/> of the <see cref="IComponentValidationCollector"/> with which the rule is associated.
    /// </summary>
    Type CollectorType { get; }

    /// <summary>
    /// Gets the property for which the validation specification will be verified.
    /// </summary>
    IPropertyInformation Property { get; }

    /// <summary>
    /// Gets the set of <see cref="IMetaValidationRule"/>s registered for the <see cref="Property"/> by the <see cref="CollectorType"/>.
    /// </summary>
    IEnumerable<IMetaValidationRule> MetaValidationRules { get; }

    /// <summary>
    /// Registers a <see cref="IMetaValidationRule"/> for the <see cref="Property"/> by the <see cref="CollectorType"/>.
    /// </summary>
    void RegisterMetaValidationRule ([NotNull] IMetaValidationRule metaValidationRule);
  }
}