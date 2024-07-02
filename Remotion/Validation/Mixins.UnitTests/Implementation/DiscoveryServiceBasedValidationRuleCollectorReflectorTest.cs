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
using System.ComponentModel.Design;
using Moq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Mixins.Implementation;
using Remotion.Validation.Mixins.UnitTests.TestDomain;
using Remotion.Validation.Mixins.UnitTests.TestDomain.Collectors;

namespace Remotion.Validation.Mixins.UnitTests.Implementation
{
  [TestFixture]
  public class DiscoveryServiceBasedValidationRuleCollectorReflectorTest
  {
    private Mock<ITypeDiscoveryService> _typeDiscoveryServiceStub;

    public class FakeValidationRuleCollector<T> : ValidationRuleCollectorBase<T>
    {
      public FakeValidationRuleCollector ()
      {
      }
    }

    [SetUp]
    public void SetUp ()
    {
      _typeDiscoveryServiceStub = new Mock<ITypeDiscoveryService>();
    }

    [Test]
    public void GetValidationRuleCollectors_WithFakeTypeDiscoveryService ()
    {
      var appliedWithAttributeTypes = new[]
                                      {
                                          typeof(CustomerMixinTargetValidationRuleCollector1), typeof(CustomerMixinIntroducedValidationRuleCollector1),
                                          typeof(CustomerMixinIntroducedValidationRuleCollector2),
                                          typeof(PersonValidationRuleCollector1)
                                      };
      _typeDiscoveryServiceStub.Setup(stub => stub.GetTypes(typeof(IValidationRuleCollector), false)).Returns(appliedWithAttributeTypes);

      var typeCollectorProvider = new DiscoveryServiceBasedValidationRuleCollectorReflector(
          _typeDiscoveryServiceStub.Object,
          new MixinTypeAwareValidatedTypeResolverDecorator(
              new ClassTypeAwareValidatedTypeResolverDecorator(
                  new GenericTypeAwareValidatedTypeResolverDecorator(new NullValidatedTypeResolver()))));

      Assert.That(
          typeCollectorProvider.GetCollectorsForType(typeof(ICustomerIntroduced)),
          Is.EqualTo(new[] { typeof(CustomerMixinIntroducedValidationRuleCollector2) }));

      Assert.That(
          typeCollectorProvider.GetCollectorsForType(typeof(CustomerMixin)),
          Is.EquivalentTo(new[] { typeof(CustomerMixinTargetValidationRuleCollector1), typeof(CustomerMixinIntroducedValidationRuleCollector1) }));
      //ApplyWithMixin attribute!
    }


  }
}
