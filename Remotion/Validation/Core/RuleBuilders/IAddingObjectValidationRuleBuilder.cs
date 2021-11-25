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
using System.Linq.Expressions;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation.RuleBuilders
{
  /// <summary>
  /// Provides an API for extending a validation rule with metadata, such as whether the validation rule can be removed by another component.
  /// </summary>
  ///<seealso cref="AddingObjectValidationRuleBuilder{TValidatedType}"/>
  public interface IAddingObjectValidationRuleBuilder<TValidatedType>
  {
    /// <summary>
    /// Associates a validator with this the Object for this rule builder.
    /// </summary>
    /// <param name="validatorFactory">A factory delegate that returns the validator to set.</param>
    /// <returns></returns>
    IAddingObjectValidationRuleBuilder<TValidatedType> SetValidator (
        Func<ObjectValidationRuleInitializationParameters, IObjectValidator> validatorFactory);

    /// <summary>
    /// Declares that the registered validation rule can be removed by another component.
    /// </summary>
    /// <returns>An object to continue the fluent specification.</returns>
    IAddingObjectValidationRuleBuilder<TValidatedType> CanBeRemoved ();

    /// <summary>
    /// Registers an <see cref="IObjectMetaValidationRule"/> for the given validators.
    /// </summary>
    /// <returns>An object to continue the fluent specification.</returns>
    IAddingObjectValidationRuleBuilder<TValidatedType> AddMetaValidationRule (IObjectMetaValidationRule metaValidationRule);

    /// <summary>
    /// Registers a delegate which will be used for performing consistency checks on the given validators.
    /// </summary>
    /// <returns>An object to continue the fluent specification.</returns>
    IAddingObjectValidationRuleBuilder<TValidatedType> AddMetaValidationRule (
        Func<IEnumerable<IObjectValidator>, MetaValidationRuleValidationResult> rule);

    /// <summary>
    /// Registers a predicate expression which will be used for performing consistency checks on validators of type <typeparamref name="TValidator"/>.
    /// </summary>
    /// <returns>An object to continue the fluent specification.</returns>
    /// <remarks>
    /// The infrastructure can include the logic encapsulated within the expression when generating the validation error message. 
    /// </remarks>
    IAddingObjectValidationRuleBuilder<TValidatedType> AddMetaValidationRule<TValidator> (
        Expression<Func<IEnumerable<TValidator>, bool>> metaValidationRuleExpression)
        where TValidator: IObjectValidator;
  }
}