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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.MetaValidation;

namespace Remotion.Validation.Rules
{
  /// <summary>
  /// Default implementation of the <see cref="IAddingComponentPropertyRule"/> interface.
  /// </summary>
  public sealed class AddingComponentPropertyRule
      // Note: this class has to inherit from PropertyRule to make IRuleBuilder.Configure work
      : PropertyRule,
          IAddingComponentPropertyRule
  {
    private readonly Type _collectorType;
    private readonly PropertyInfoAdapter _property;
    private bool _isHardConstraint;

    public static AddingComponentPropertyRule Create<TValidatedType, TProperty> (
        Expression<Func<TValidatedType, TProperty>> expression,
        Type collectorType)
    {
      var member = expression.GetMember() as PropertyInfo;
      if (member == null)
      {
        throw new InvalidOperationException (
            string.Format ("An '{0}' can only created for property members.", typeof (AddingComponentPropertyRule).Name));
      }

      var compiled = expression.Compile();

      return new AddingComponentPropertyRule (
          collectorType,
          member,
          compiled.CoerceToNonGeneric(),
          expression,
          () => ValidatorOptions.CascadeMode,
          typeof (TProperty),
          typeof (TValidatedType));
    }

    public AddingComponentPropertyRule (Type validatedType, PropertyInfo propertyInfo, Func<object, object> propertyFunc, Type collectorType)
        : this (
            collectorType,
            propertyInfo,
            propertyFunc,
            null,
            () => ValidatorOptions.CascadeMode,
            ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo).PropertyType,
            validatedType)
    {
    }

    private AddingComponentPropertyRule (
        Type collectorType,
        PropertyInfo propertyInfo,
        Func<object, object> propertyFunc,
        LambdaExpression expression,
        Func<CascadeMode> cascadeModeThunk,
        Type propertyType,
        Type validatedType)
        : base (propertyInfo, propertyFunc, expression, cascadeModeThunk, propertyType, validatedType)
    {
      ArgumentUtility.CheckNotNull ("collectorType", collectorType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      _collectorType = collectorType;
      _property = PropertyInfoAdapter.Create (propertyInfo);
      _isHardConstraint = false;
    }

    public IPropertyInformation Property
    {
      get { return _property; }
    }

    public Type CollectorType
    {
      get { return _collectorType; }
    }

    public bool IsHardConstraint
    {
      get { return _isHardConstraint; }
    }

    public void SetHardConstraint ()
    {
      _isHardConstraint = true;
    }

    public void RegisterValidator (IPropertyValidator validator)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);

      AddValidator (validator);
    }

    public void ApplyRemoveValidatorRegistrations (IPropertyValidatorExtractor propertyValidatorExtractor)
    {
      ArgumentUtility.CheckNotNull ("propertyValidatorExtractor", propertyValidatorExtractor);

      var validatorsToRemove = propertyValidatorExtractor.ExtractPropertyValidatorsToRemove (this).ToArray();
      CheckForHardConstraintViolation (validatorsToRemove);
      foreach (var validator in validatorsToRemove)
        RemoveValidator (validator);
    }

    private void CheckForHardConstraintViolation (IPropertyValidator[] validatorsToRemove)
    {
      if (_isHardConstraint && validatorsToRemove.Any())
      {
        throw new ValidationConfigurationException (
            string.Format (
                "Hard constraint validator(s) '{0}' on property '{1}.{2}' cannot be removed.",
                string.Join (", ", validatorsToRemove.Select (v => v.GetType().Name).ToArray()),
                Property.DeclaringType.FullName,
                Property.Name));
      }
    }

    public override string ToString ()
    {
      var sb = new StringBuilder (GetType().Name);
      if (_isHardConstraint)
        sb.Append (" (HARD CONSTRAINT)");
      sb.Append (": ");
      sb.Append (Property.DeclaringType != null ? Property.DeclaringType.FullName + "#" : string.Empty);
      sb.Append (Property.Name);

      return sb.ToString();
    }
  }
}