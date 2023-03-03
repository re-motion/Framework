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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Validators;

namespace Remotion.Validation
{
  public static class DefaultValidatorExtensions
  {
    /// <summary>
    /// Adds a <see cref="NotNullValidator"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> NotNull<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder)
    {
      return ruleBuilder.SetValidator(p => new NotNullValidator(p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="NotEmptyCollectionValidator"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string[]> NotEmpty<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string[]> ruleBuilder)
    {
      return ruleBuilder.SetValidator(p => new NotEmptyCollectionValidator(p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="NotEmptyBinaryValidator"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, byte[]> NotEmpty<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, byte[]> ruleBuilder)
    {
      return ruleBuilder.SetValidator(p => new NotEmptyBinaryValidator(p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="NotEmptyCollectionValidator"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    /// <remarks>
    /// This overload is for properties whose <see cref="Type"/> is assignable to <see cref="IReadOnlyCollection{T}"/>, but not to <see cref="ICollection"/>.
    /// Note that if a <see cref="Type"/> is assignable to both <see cref="ICollection{T}"/> and <see cref="IReadOnlyCollection{T}"/>, but not to
    /// <see cref="ICollection"/>, a call to NotEmpty() is ambiguous.
    /// </remarks>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, IReadOnlyCollection<TItem>> NotEmpty<TValidatedType, TItem> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, IReadOnlyCollection<TItem>> ruleBuilder)
    {
      return ruleBuilder.SetValidator(p => new NotEmptyCollectionValidator(p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="NotEmptyCollectionValidator"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    /// <remarks>
    /// This overload is for properties whose <see cref="Type"/> is assignable to <see cref="ICollection{T}"/>, but not to <see cref="ICollection"/>.
    /// Note that if a <see cref="Type"/> is assignable to both <see cref="ICollection{T}"/> and <see cref="IReadOnlyCollection{T}"/>, but not to
    /// <see cref="ICollection"/>, a call to NotEmpty() is ambiguous. 
    /// </remarks>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, ICollection<TItem>> NotEmpty<TValidatedType, TItem> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, ICollection<TItem>> ruleBuilder)
    {
      return ruleBuilder.SetValidator(p => new NotEmptyCollectionValidator(p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="NotEmptyCollectionValidator"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    /// <remarks>
    /// This overload is for properties whose <see cref="Type"/> is assignable to <see cref="ICollection"/>. Since <typeparamref name="TCollection"/>
    /// matches the property's exact type, this overload is chosen over those for <see cref="ICollection{T}"/> and <see cref="IReadOnlyCollection{T}"/>,
    /// breaking ambiguity.
    /// </remarks>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TCollection> NotEmpty<TValidatedType, TCollection> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TCollection> ruleBuilder)
        where TCollection : ICollection
    {
      return ruleBuilder.SetValidator(p => new NotEmptyCollectionValidator(p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="NotEmptyOrWhitespaceValidator"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string> NotEmptyOrWhitespace<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string> ruleBuilder)
    {
      return ruleBuilder.SetValidator(p => new NotEmptyOrWhitespaceValidator(p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="NotEmptyOrWhitespaceValidator"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string[]> NotEmptyOrWhitespace<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string[]> ruleBuilder)
    {
      return ruleBuilder.SetValidator(p => new NotEmptyOrWhitespaceValidator(p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="LengthValidator"/> with the given <paramref name="min"/> and <paramref name="max"/> length to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string> Length<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string> ruleBuilder,
        int min,
        int max)
    {
      return ruleBuilder.SetValidator(p => new LengthValidator(min, max, p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="MinimumLengthValidator"/> with the given <paramref name="min"/> length to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string> MinLength<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string> ruleBuilder,
        int min)
    {
      return ruleBuilder.SetValidator(p => new MinimumLengthValidator(min, p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="MaximumLengthValidator"/> with the given <paramref name="max"/> length to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string> MaxLength<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string> ruleBuilder,
        int max)
    {
      return ruleBuilder.SetValidator(p => new MaximumLengthValidator(max, p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="ExactLengthValidator"/> with the given <paramref name="length"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string> ExactLength<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string> ruleBuilder,
        int length)
    {
      return ruleBuilder.SetValidator(p => new ExactLengthValidator(length, p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="EqualValidator"/> with the given <paramref name="valueToCompare"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> Equal<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty valueToCompare,
        IEqualityComparer? comparer = null)
        where TProperty : notnull
    {
      return ruleBuilder.SetValidator(p => new EqualValidator(valueToCompare, p.ValidationMessage, comparer));
    }

    /// <summary>
    /// Adds a <see cref="NotEqualValidator"/> with the given <paramref name="valueToCompare"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> NotEqual<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty valueToCompare,
        IEqualityComparer? comparer = null)
        where TProperty : notnull
    {
      return ruleBuilder.SetValidator(p => new NotEqualValidator(valueToCompare, p.ValidationMessage, comparer));
    }

    /// <summary>
    /// Adds a <see cref="LessThanValidator"/> with the given <paramref name="valueToCompare"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> LessThan<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new LessThanValidator(valueToCompare, p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="LessThanOrEqualValidator"/> with the given <paramref name="valueToCompare"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> LessThanOrEqual<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new LessThanOrEqualValidator(valueToCompare, p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="GreaterThanValidator"/> with the given <paramref name="valueToCompare"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> GreaterThan<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new GreaterThanValidator(valueToCompare, p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="GreaterThanOrEqualValidator"/> with the given <paramref name="valueToCompare"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> GreaterThanOrEqual<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new GreaterThanOrEqualValidator(valueToCompare, p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="ExclusiveRangeValidator"/> with the given <paramref name="from"/> and <paramref name="to"/> values to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ExclusiveBetween<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty from,
        TProperty to)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new ExclusiveRangeValidator(from, to, p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="InclusiveRangeValidator"/> with the given <paramref name="from"/> and <paramref name="to"/> values to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> InclusiveBetween<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty from,
        TProperty to)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new InclusiveRangeValidator(from, to, p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="DecimalValidator"/> with the given <paramref name="maxIntegerPlaces"/> and <paramref name="maxDecimalPlaces"/> (including or
    /// excluding training zeros according to <paramref name="ignoreTrailingZeros"/>) to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> DecimalValidator<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        int maxIntegerPlaces,
        int maxDecimalPlaces,
        bool ignoreTrailingZeros = false)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator(p => new DecimalValidator(maxIntegerPlaces, maxDecimalPlaces, ignoreTrailingZeros, p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="RegularExpressionValidator"/> with the given <paramref name="expression"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, string> Matches<TValidatedType> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, string> ruleBuilder,
        [JetBrains.Annotations.RegexPattern] string expression)
    {
      return ruleBuilder.SetValidator(p => new RegularExpressionValidator(new Regex(expression), p.ValidationMessage));
    }

    /// <summary>
    /// Adds a <see cref="PredicateValidator"/> with the given <paramref name="predicate"/> to the <paramref name="ruleBuilder"/>.
    /// </summary>
    public static IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> Must<TValidatedType, TProperty> (
        this IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        PredicateValidator.Predicate predicate)
    {
      return ruleBuilder.SetValidator(p => new PredicateValidator(predicate, p.ValidationMessage));
    }

    // TODO RM-6590: Implement comparison- and equal-validators for delegate-based comparison values. See FluentValidation for example. Note: might require dropping the interfaces. Will require renaming the existing validators
  }
}
