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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.Validation.UnitTests
{
  [TestFixture]
  public class DomainObjectAttributesBasedValidationRuleCollectorProviderTest
  {
    private DomainObjectAttributesBasedValidationRuleCollectorProvider _provider;
    private IValidationMessageFactory _validationMessageFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _validationMessageFactoryStub = MockRepository.GenerateStub<IValidationMessageFactory>();
      _validationMessageFactoryStub
          .Stub (_ => _.CreateValidationMessageForPropertyValidator (Arg<IPropertyValidator>.Is.NotNull, Arg<IPropertyInformation>.Is.NotNull))
          .Return (new InvariantValidationMessage ("Fake Message"));

      _provider = new DomainObjectAttributesBasedValidationRuleCollectorProvider(new DomainModelConstraintProvider(), _validationMessageFactoryStub);
    }

    [Test]
    public void CreatePropertyRuleReflector_NoDomainObject ()
    {
      var result = _provider.GetValidationRuleCollectors (new[] { typeof (NoDomainObject) })
          .SelectMany (t => t)
          .ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void CreatePropertyRuleReflector_DomainObjectWithoutAnnotatedProperties ()
    {
      var result = _provider.GetValidationRuleCollectors (new[] { typeof (DomainObjectWithoutAnnotatedProperties) })
          .SelectMany (t => t)
          .ToArray ();

      Assert.That (result.Any (), Is.False);
    }

    [Test]
    public void CreatePropertyRuleReflector_DomainObjectWithAnnotatedProperties ()
    {
      var result = _provider.GetValidationRuleCollectors (new[] { typeof (TypeWithDomainObjectAttributes) })
          .SelectMany (t => t)
          .SingleOrDefault();
      
      Assert.That (result, Is.Not.Null);
      Assert.That (result.Collector.ValidatedType, Is.EqualTo (typeof (TypeWithDomainObjectAttributes)));
      Assert.That (result.Collector.AddedPropertyRules.Count, Is.EqualTo (12));
    }

    [Test]
    public void CreatePropertyRuleReflectorForDomainObject_PropertiesAreOverridden ()
    {
      var result =
          _provider.GetValidationRuleCollectors (new[] { typeof (DerivedTypeWithDomainObjectAttributes) })
              .SelectMany (c => c)
              .SingleOrDefault();

      Assert.That (result, Is.Not.Null);
      Assert.That (result.Collector.ValidatedType, Is.EqualTo (typeof (DerivedTypeWithDomainObjectAttributes)));
      Assert.That (result.Collector.AddedPropertyRules.Select (r => r.Property.Name).Distinct(), Is.EquivalentTo (new[] { "PropertyInDerivedType" }));
    }

    [Test]
    public void CreatePropertyRuleReflectorForDomainObjectMixin_PropertiesArePartOfDifferentInterfaces ()
    {
      var results =
          _provider.GetValidationRuleCollectors (new[] { typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfDifferentInterfaces) })
              .SelectMany (c => c)
              .ToArray();

      Assert.That (results, Is.Not.Empty);
      Assert.That (
          results.Select (r => r.Collector.ValidatedType),
          Is.EquivalentTo (
              new[]
              {
                  typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfDifferentInterfaces1),
                  typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfDifferentInterfaces2)
              }));
      Assert.That (
          results.SelectMany (r => r.Collector.AddedPropertyRules.Select (c => c.Property.Name).Distinct()),
          Is.EquivalentTo (new[] { "PropertyWithMandatoryAttribute", "PropertyWithMandatoryStringPropertyAttribute" }));
    }

    [Test]
    public void CreatePropertyRuleReflectorForDomainObjectMixin_WithoutInterface_ExceptionIsThrown ()
    {
      Assert.That (
          () => _provider.GetValidationRuleCollectors (new[] { typeof (MixinTypeWithDomainObjectAttributes_WithoutInterface) }),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Annotated properties of mixin 'Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain.MixinTypeWithDomainObjectAttributes_WithoutInterface' have to be part of an interface."));
    }

    [Test]
    public void CreatePropertyRuleReflectorForDomainObjectMixin_SomeAnnotatedPropertiesNotPartOfAnInterface_ExceptionIsThrown ()
    {
      Assert.That (
          () => _provider.GetValidationRuleCollectors (new[] { typeof (MixinTypeWithDomainObjectAttributes_SomeAnnotatedPropertiesNotPartOfInterface) }),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Annotated properties of mixin 'Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain.MixinTypeWithDomainObjectAttributes_SomeAnnotatedPropertiesNotPartOfInterface' have to be part of an interface."));
    }

    [Test]
    public void CreatePropertyRuleReflectorForDomainObjectMixin_AllAnnotatedPropertiesNotPartOfAnInterface_ExceptionIsThrown ()
    {
      Assert.That (
          () => _provider.GetValidationRuleCollectors (new[] { typeof (MixinTypeWithDomainObjectAttributes_AllAnnotatedPropertiesNotPartOfInterface) }),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Annotated properties of mixin 'Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain.MixinTypeWithDomainObjectAttributes_AllAnnotatedPropertiesNotPartOfInterface' have to be part of an interface."));
    }

    [Test]
    public void CreatePropertyRuleReflectorForDomainObjectMixin_AnnotatedPropertiesPartOfAnInterface ()
    {
      var result =
          _provider.GetValidationRuleCollectors (new[] { typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface) })
              .SelectMany (c => c)
              .SingleOrDefault();

      Assert.That (result, Is.Not.Null);
      Assert.That (result.Collector.ValidatedType, Is.EqualTo (typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)));
      Assert.That (result.Collector.AddedPropertyRules.Count, Is.EqualTo (12));
    }

    [Test]
    public void CreatePropertyRuleReflectorForDomainObjectMixin_PropertiesAreOverridden ()
    {
      var result =
          _provider.GetValidationRuleCollectors (new[] { typeof (DerivedMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface) })
              .SelectMany (c => c)
              .SingleOrDefault();

      Assert.That (result, Is.Not.Null);
      Assert.That (
          result.Collector.ValidatedType,
          Is.EqualTo (typeof (IDerivedMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)));
      Assert.That (
          result.Collector.AddedPropertyRules.Select (r => r.Property.Name).Distinct(),
          Is.EquivalentTo (new[] { "PropertyInDerivedType" }));
    }

    [Test]
    public void CreatePropertyRuleReflectorForConcreteTypeBasedOnDomainObjectAndMixin_AnnotatedPropertiesPartOfAnInterface_DoesNotIncludePropertiesCopeisToConcreteType ()
    {
      using (MixinConfiguration
          .BuildNew()
          .ForClass<MixinTarget_AnnotatedPropertiesPartOfInterface>()
          .AddMixin<MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface>()
          .EnterScope())
      {
        var result =
            _provider.GetValidationRuleCollectors (
                new[]
                {
                    MixinTypeUtility.GetConcreteMixedType (typeof (MixinTarget_AnnotatedPropertiesPartOfInterface)),
                    typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)
                })
                .SelectMany (c => c)
                .ToArray();

        var resultForConcreteType = result.SingleOrDefault (r => MixinTypeUtility.IsGeneratedConcreteMixedType (r.Collector.ValidatedType));
        Assert.That (resultForConcreteType, Is.Null);

        var resultForMixin = result.SingleOrDefault (
            r => r.Collector.ValidatedType == typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

        Assert.That (resultForMixin, Is.Not.Null);
        Assert.That (resultForMixin.Collector.AddedPropertyRules.Count, Is.EqualTo (12));
      }
    }
  }
}