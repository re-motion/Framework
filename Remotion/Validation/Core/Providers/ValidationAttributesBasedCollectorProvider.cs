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
using System.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Attributes.Validation;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Providers
{
  /// <summary>
  /// Uses attributes derived from the <see cref="AddingValidationAttributeBase"/> type to build constraints for the properties.
  /// </summary>
  [ImplementationFor (typeof (IValidationCollectorProvider), Lifetime = LifetimeKind.Singleton, Position = 1, RegistrationType = RegistrationType.Multiple)]
  public class ValidationAttributesBasedCollectorProvider : AttributeBasedValidationCollectorProviderBase
  {
    private const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    protected override ILookup<Type, IAttributesBasedValidationPropertyRuleReflector> CreatePropertyRuleReflectors (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      return types.SelectMany (
          t => t.GetProperties (PropertyBindingFlags | BindingFlags.DeclaredOnly)
              .Where (HasValidationRulesOnProperty)
              .Select (p => new { Type = t, Property = p }))
          .Select (r => new { r.Type, Reflector = new ValidationAttributesBasedPropertyRuleReflector (r.Property) })
          .ToLookup (r => r.Type, c => (IAttributesBasedValidationPropertyRuleReflector) c.Reflector);
    }

    private bool HasValidationRulesOnProperty (PropertyInfo property)
    {
      var reflector = new ValidationAttributesBasedPropertyRuleReflector (property);
      return reflector.GetAddingPropertyValidators().Any()
             || reflector.GetHardConstraintPropertyValidators().Any()
             || reflector.GetRemovingPropertyRegistrations().Any();
    }
  }
}