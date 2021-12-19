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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Providers;
using Remotion.Validation.Validators;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Uses the <see cref="ILengthConstrainedPropertyAttribute"/> and the <see cref="INullablePropertyAttribute"/> to build <see cref="LengthValidator"/> 
  /// and <see cref="NotNullValidator"/> for properties.
  /// </summary>
  [ImplementationFor(typeof(IValidationRuleCollectorProvider), Lifetime = LifetimeKind.Singleton, Position = 0, RegistrationType = RegistrationType.Multiple)]
  public class DomainObjectAttributesBasedValidationRuleCollectorProvider : AttributeBasedValidationRuleCollectorProviderBase
  {
    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;
    private readonly IValidationMessageFactory _validationMessageFactory;

    private interface IDummyInterface
    {
      object DummyProperty { get; }
    }

    private const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly PropertyInfo s_dummyProperty = MemberInfoFromExpressionUtility.GetProperty(((IDummyInterface o) => o.DummyProperty));

    public DomainObjectAttributesBasedValidationRuleCollectorProvider (
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IValidationMessageFactory validationMessageFactory)
    {
      ArgumentUtility.CheckNotNull("domainModelConstraintProvider", domainModelConstraintProvider);
      ArgumentUtility.CheckNotNull("validationMessageFactory", validationMessageFactory);

      _domainModelConstraintProvider = domainModelConstraintProvider;
      _validationMessageFactory = validationMessageFactory;
    }

    protected override ILookup<Type, IAttributesBasedValidationPropertyRuleReflector> CreatePropertyRuleReflectors (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull("types", types);

      return types
          .Where(type => !MixinTypeUtility.IsGeneratedConcreteMixedType(type))
          .SelectMany(t => CreatePropertyRuleReflectors(t, t.GetInterfaces()))
          .ToLookup(r => r.Type, r => r.PropertyRuleReflector);
    }

    private IEnumerable<(Type Type, IAttributesBasedValidationPropertyRuleReflector PropertyRuleReflector)> CreatePropertyRuleReflectors (
        Type annotatedType,
        IReadOnlyCollection<Type> interfaceTypes)
    {
      if (ReflectionUtility.IsDomainObject(annotatedType))
      {
        return annotatedType.GetProperties(PropertyBindingFlags | BindingFlags.DeclaredOnly)
            .Select(PropertyInfoAdapter.Create)
            .Where(p => p.IsOriginalDeclaration())
            .Select(p => p.AsRuntimePropertyInfo()!)
            .Where(p => HasValidationRulesOnProperty(p))
            .Select(
                p => (
                    Type: annotatedType,
                    PropertyRuleReflector: (IAttributesBasedValidationPropertyRuleReflector)new DomainObjectAttributesBasedValidationPropertyRuleReflector(
                        p,
                        p,
                        _domainModelConstraintProvider,
                        _validationMessageFactory)));
      }

      if (typeof(IDomainObjectMixin).IsAssignableFrom(annotatedType) && !annotatedType.IsInterface)
      {
        var implementedInterfaces = interfaceTypes.Where(i => i.IsAssignableFrom(annotatedType));

        var interfacePropertyMappings = implementedInterfaces
            .SelectMany(
                t =>
                {
                  Assertion.IsTrue(t.IsInterface);
                  return t.GetProperties();
                })
            .Select(p => (IPropertyInformation)PropertyInfoAdapter.Create(p))
            .Select(p => new { InterfaceProperty = p, ImplementationProperty = Assertion.IsNotNull(p.FindInterfaceImplementation(annotatedType)) })
            .ToList();

        var annotatedProperties = annotatedType.GetProperties(PropertyBindingFlags | BindingFlags.DeclaredOnly)
            .Select(p => (IPropertyInformation)PropertyInfoAdapter.Create(p))
            .Where(p => p.IsOriginalDeclaration())
            .Where(p => HasValidationRulesOnProperty(p.AsRuntimePropertyInfo()!));
        var annotatedPropertiesSet = new HashSet<IPropertyInformation>(annotatedProperties);

        var interfaceImplementations = new HashSet<IPropertyInformation>(interfacePropertyMappings.Select(m=>m.ImplementationProperty));
        if (annotatedPropertiesSet.Any(p => !interfaceImplementations.Contains(p)))
        {
          throw new InvalidOperationException(
              string.Format("Annotated properties of mixin '{0}' have to be part of an interface.", annotatedType.GetFullNameSafe()));
        }

        var interfacePropertyMappingsForAnnotatedProperties = interfacePropertyMappings
            .Where(mapping => annotatedPropertiesSet.Contains(mapping.ImplementationProperty));

        return interfacePropertyMappingsForAnnotatedProperties.Select(
            mapping => (
                Type: mapping.InterfaceProperty.DeclaringType!.AsRuntimeType()!,
                PropertyRuleReflector: (IAttributesBasedValidationPropertyRuleReflector)new DomainObjectAttributesBasedValidationPropertyRuleReflector(
                    mapping.InterfaceProperty.AsRuntimePropertyInfo()!,
                    mapping.ImplementationProperty.AsRuntimePropertyInfo()!,
                    _domainModelConstraintProvider,
                    _validationMessageFactory)));
      }

      return Enumerable.Empty<ValueTuple<Type, IAttributesBasedValidationPropertyRuleReflector>>();
    }

    private bool HasValidationRulesOnProperty (PropertyInfo property)
    {
      // The interface property does not matter in this particular instance, so any property could be passed into the reflector.
      var reflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          s_dummyProperty,
          property,
          _domainModelConstraintProvider,
          _validationMessageFactory);

      return reflector.GetRemovablePropertyValidators().Any()
        || reflector.GetNonRemovablePropertyValidators().Any()
        || reflector.GetRemovingValidatorRegistrations().Any();
    }
  }
}
