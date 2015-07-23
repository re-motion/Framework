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
using Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.Validation.UnitTests
{
  [TestFixture]
  public class DomainObjectAttributesBasedValidationCollectorProviderTest
  {
    private DomainObjectAttributesBasedValidationCollectorProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new DomainObjectAttributesBasedValidationCollectorProvider();
    }

    [Test]
    public void CreatePropertyRuleReflector_NoDomainObject ()
    {
      var result = _provider.GetValidationCollectors (new[] { typeof (NoDomainObject) })
          .SelectMany (t => t)
          .ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void CreatePropertyRuleReflector_DomainObjectWithoutAnnotatedProperties ()
    {
      var result = _provider.GetValidationCollectors (new[] { typeof (DomainObjectWithoutAnnotatedProperties) })
          .SelectMany (t => t)
          .ToArray ();

      Assert.That (result.Any (), Is.False);
    }

    [Test]
    public void CreatePropertyRuleReflector_DomainObjectWithAnnotatedProperties ()
    {
      var result = _provider.GetValidationCollectors (new[] { typeof (TypeWithDomainObjectAttributes) })
          .SelectMany (t => t)
          .SingleOrDefault();
      
      Assert.That (result, Is.Not.Null);
      Assert.That (result.Collector.ValidatedType, Is.EqualTo (typeof (TypeWithDomainObjectAttributes)));
      Assert.That (result.Collector.AddedPropertyRules.Count, Is.EqualTo (10));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Annotated properties of mixin 'MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesNotPartOfInterface' have to be part of an interface.")]
    public void CreatePropertyRuleReflectorForDomainObjectMixin_AnnotatedPropertiesNotPartOfAnInterface_ExceptionIsThrown ()
    {
      _provider.GetValidationCollectors (new[] { typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesNotPartOfInterface) });
    }

    [Test]
    public void CreatePropertyRuleReflectorForDomainObjectMixin_AnnotatedPropertiesPartOfAnInterface ()
    {
      var result =
          _provider.GetValidationCollectors (new[] { typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface) })
              .SelectMany (c => c)
              .SingleOrDefault();

      Assert.That (result, Is.Not.Null);
      Assert.That (result.Collector.ValidatedType, Is.EqualTo (typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)));
      Assert.That (result.Collector.AddedPropertyRules.Count, Is.EqualTo (10));
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
            _provider.GetValidationCollectors (
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
        Assert.That (resultForMixin.Collector.AddedPropertyRules.Count, Is.EqualTo (10));
      }
    }
  }
}