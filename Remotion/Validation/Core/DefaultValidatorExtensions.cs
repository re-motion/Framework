﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Collections;
using System.Text.RegularExpressions;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Validators;

namespace Remotion.Validation
{
  public static class DefaultValidatorExtensions
  {
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> NotNull<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder)
    {
      return ruleBuilder.SetValidator(p => new NotNullValidator(p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> NotEmpty<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder)
    {
      return ruleBuilder.SetValidator(p => new NotEmptyValidator(p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string> Length<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string> ruleBuilder,
        int min,
        int max)
    {
      return ruleBuilder.SetValidator(p => new LengthValidator(min, max, p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string> MinLength<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string> ruleBuilder,
        int min)
    {
      return ruleBuilder.SetValidator(p => new MinimumLengthValidator(min, p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string> MaxLength<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string> ruleBuilder,
        int max)
    {
      return ruleBuilder.SetValidator(p => new MaximumLengthValidator(max, p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string> ExactLength<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string> ruleBuilder,
        int length)
    {
      return ruleBuilder.SetValidator(p => new ExactLengthValidator(length, p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> Equal<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty toCompare,
        IEqualityComparer? comparer = null)
        where TProperty : notnull
    {
      return ruleBuilder.SetValidator(p => new EqualValidator(toCompare, p.ValidationMessage, comparer));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> NotEqual<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty toCompare,
        IEqualityComparer? comparer = null)
        where TProperty : notnull
    {
      return ruleBuilder.SetValidator(p => new NotEqualValidator(toCompare, p.ValidationMessage, comparer));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> LessThan<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new LessThanValidator(valueToCompare, p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> LessThanOrEqual<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new LessThanOrEqualValidator(valueToCompare, p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> GreaterThan<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new GreaterThanValidator(valueToCompare, p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> GreaterThanOrEqual<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new GreaterThanOrEqualValidator(valueToCompare, p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ExclusiveBetween<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty from,
        TProperty to)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new ExclusiveRangeValidator(from, to, p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> InclusiveBetween<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty from,
        TProperty to)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new InclusiveRangeValidator(from, to, p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> DecimalValidator<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        int maxIntegerPlaces,
        int maxDecimalPlaces,
        bool ignoreTrailingZeros = false)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new DecimalValidator(maxIntegerPlaces, maxDecimalPlaces, ignoreTrailingZeros, p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string> Matches<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string> ruleBuilder,
        [JetBrains.Annotations.RegexPattern] string expression)
    {
      return ruleBuilder.SetValidator(p => new RegularExpressionValidator(new Regex(expression), p.ValidationMessage));
    }

    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> Must<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        PredicateValidator.Predicate predicate)
    {
      return ruleBuilder.SetValidator(p => new PredicateValidator(predicate, p.ValidationMessage));
    }

    // TODO RM-6590: Implement comparison- and equal-validators for delegate-based comparison values. See FluentValidation for example. Note: might require dropping the interfaces. Will require renaming the existing validators
  }
}