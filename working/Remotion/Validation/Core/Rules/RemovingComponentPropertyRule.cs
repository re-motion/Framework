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
using System.Reflection;
using System.Text;
using FluentValidation.Internal;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Validation.Rules
{
  /// <summary>
  /// Default implementation of the <see cref="IRemovingComponentPropertyRule"/> interface.
  /// </summary>
  public sealed class RemovingComponentPropertyRule : IRemovingComponentPropertyRule
  {
    private readonly IPropertyInformation _property;
    private readonly Type _collectorType;
    private readonly List<ValidatorRegistration> _registeredValidators;

    public static RemovingComponentPropertyRule Create<TValidatedType, TProperty> (Expression<Func<TValidatedType, TProperty>> expression, Type collectorType)
    {
      var member = expression.GetMember() as PropertyInfo;
      if (member == null)
        throw new InvalidOperationException (string.Format ("A '{0}' can only created for property members.", typeof(RemovingComponentPropertyRule).Name));

      return new RemovingComponentPropertyRule (member, collectorType);
    }

    public RemovingComponentPropertyRule (PropertyInfo member, Type collectorType)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      ArgumentUtility.CheckNotNull ("collectorType", collectorType);

      _property = PropertyInfoAdapter.Create(member);
      _collectorType = collectorType;
      _registeredValidators = new List<ValidatorRegistration>();
    }

    public IPropertyInformation Property
    {
      get { return _property; }
    }

    public Type CollectorType
    {
      get { return _collectorType; }
    }

    public IEnumerable<ValidatorRegistration> Validators
    {
      get { return _registeredValidators.AsReadOnly(); }
    }

    public void RegisterValidator (Type validatorType)
    {
      RegisterValidator (validatorType, null);
    }

    public void RegisterValidator (Type validatorType, Type collectorTypeToRemoveFrom)
    {
      ArgumentUtility.CheckNotNull ("validatorType", validatorType);

      _registeredValidators.Add (new ValidatorRegistration (validatorType, collectorTypeToRemoveFrom));
    }

    public override string ToString ()
    {
      var sb = new StringBuilder (GetType().Name);
      sb.Append (": ");
      sb.Append (Property.DeclaringType != null ? Property.DeclaringType.FullName + "#" : string.Empty);
      sb.Append (Property.Name);

      return sb.ToString();
    }
  }
}