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
using FluentValidation.Validators;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Providers;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Uses the <see cref="ILengthConstrainedPropertyAttribute"/> and the <see cref="INullablePropertyAttribute"/> to build <see cref="LengthValidator"/> 
  /// and <see cref="NotNullValidator"/> for properties.
  /// </summary>
  [ImplementationFor (typeof (IValidationCollectorProvider), Lifetime = LifetimeKind.Singleton, Position = 0, RegistrationType = RegistrationType.Multiple)]
  public class DomainObjectAttributesBasedValidationCollectorProvider : AttributeBasedValidationCollectorProviderBase
  {
    private interface IDummyInterface
    {
      object DummyProperty { get; }
    }

    private const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly PropertyInfo s_dummyProperty = MemberInfoFromExpressionUtility.GetProperty (((IDummyInterface o) => o.DummyProperty));

    public DomainObjectAttributesBasedValidationCollectorProvider ()
    {
    }

    protected override ILookup<Type, IAttributesBasedValidationPropertyRuleReflector> CreatePropertyRuleReflectors (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      return types
          .Where (type => !MixinTypeUtility.IsGeneratedConcreteMixedType (type))
          .SelectMany (t => CreatePropertyRuleReflectors (t, t.GetInterfaces()))
          .ToLookup (r => r.Item1, r => r.Item2);
    }

    private IEnumerable<Tuple<Type, IAttributesBasedValidationPropertyRuleReflector>> CreatePropertyRuleReflectors (
        Type annotatedType,
        IReadOnlyCollection<Type> interfaceTypes)
    {
      if (ReflectionUtility.IsDomainObject (annotatedType))
      {
        return annotatedType.GetProperties (PropertyBindingFlags | BindingFlags.DeclaredOnly)
            .Where (HasValidationRulesOnProperty)
            .Select (
                p => new Tuple<Type, IAttributesBasedValidationPropertyRuleReflector> (
                    annotatedType,
                    new DomainObjectAttributesBasedValidationPropertyRuleReflector (p, p)));
      }

      if (typeof (IDomainObjectMixin).IsAssignableFrom (annotatedType) && !annotatedType.IsInterface)
      {
        var implementedInterfaces = interfaceTypes.Where (i => i.IsAssignableFrom (annotatedType)).ToList();
        var interfaceProperties = implementedInterfaces.SelectMany (t =>
        {
          Assertion.IsTrue (t.IsInterface);
          return t.GetProperties();
        }).Select (PropertyInfoAdapter.Create).ToList();
        var annotatedProperties = annotatedType.GetProperties (PropertyBindingFlags | BindingFlags.DeclaredOnly)
            .Where (HasValidationRulesOnProperty)
            .Select (PropertyInfoAdapter.Create)
            .ToDictionary (p => (IPropertyInformation) p);

        var propertyMapping = interfaceProperties
            .Select (p => new { InterfaceProperty = p, ImplementationProperty = p.FindInterfaceImplementation (annotatedType) })
            .Where (mapping => annotatedProperties.ContainsKey (mapping.ImplementationProperty))
            .ToList();

        if (annotatedProperties.Any() && !propertyMapping.Any())
        {
          throw new InvalidOperationException (
              string.Format ("Annotated properties of mixin '{0}' have to be part of an interface.", annotatedType.Name));
        }

        return propertyMapping.Select (
            mapping => new Tuple<Type, IAttributesBasedValidationPropertyRuleReflector> (
                mapping.InterfaceProperty.DeclaringType.AsRuntimeType(),
                new DomainObjectAttributesBasedValidationPropertyRuleReflector (
                    mapping.InterfaceProperty.AsRuntimePropertyInfo(),
                    mapping.ImplementationProperty.AsRuntimePropertyInfo())));
      }

      return Enumerable.Empty<Tuple<Type, IAttributesBasedValidationPropertyRuleReflector>>();
    }

    private bool HasValidationRulesOnProperty (PropertyInfo property)
    {
      // The interface property does not matter in this particular instance, so any property could be passed into the reflector.
      var reflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (s_dummyProperty, property);

      return reflector.GetAddingPropertyValidators().Any() 
        || reflector.GetHardConstraintPropertyValidators().Any()
        || reflector.GetRemovingPropertyRegistrations().Any();
    }
  }
}