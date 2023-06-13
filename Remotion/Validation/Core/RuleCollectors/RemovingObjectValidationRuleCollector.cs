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
using System.Text;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Validators;

namespace Remotion.Validation.RuleCollectors
{
  /// <summary>
  /// Default implementation of the <see cref="IRemovingObjectValidationRuleCollector"/> interface.
  /// </summary>
  public sealed class RemovingObjectValidationRuleCollector : IRemovingObjectValidationRuleCollector
  {
    public static RemovingObjectValidationRuleCollector Create<TValidatedType> (Type collectorType)
    {
      return new RemovingObjectValidationRuleCollector(TypeAdapter.Create(typeof(TValidatedType)), collectorType);
    }

    public ITypeInformation ValidatedType { get; }
    public Type CollectorType { get; }

    private readonly List<RemovingObjectValidatorRegistration> _registeredValidators;

    public RemovingObjectValidationRuleCollector (ITypeInformation validatedType, Type collectorType)
    {
      ArgumentUtility.CheckNotNull("validatedType", validatedType);
      ArgumentUtility.CheckNotNull("collectorType", collectorType); // TODO RM-5906: Add type check for IValidationRuleCollector

      ValidatedType = validatedType;
      CollectorType = collectorType;
      _registeredValidators = new List<RemovingObjectValidatorRegistration>();
    }

    public IEnumerable<RemovingObjectValidatorRegistration> Validators
    {
      get { return _registeredValidators.AsReadOnly(); }
    }

    public void RegisterValidator (Type validatorType, Type? collectorTypeToRemoveFrom, Func<IObjectValidator, bool>? validatorPredicate)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("validatorType", validatorType, typeof(IObjectValidator));
      ArgumentUtility.CheckTypeIsAssignableFrom("collectorTypeToRemoveFrom", collectorTypeToRemoveFrom, typeof(IValidationRuleCollector));

      _registeredValidators.Add(new RemovingObjectValidatorRegistration(validatorType, collectorTypeToRemoveFrom, validatorPredicate, this));
    }

    public override string ToString ()
    {
      var sb = new StringBuilder(GetType().Name);
      sb.Append(": ");
      sb.Append(ValidatedType.GetFullNameSafe());

      return sb.ToString();
    }
  }
}
