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
using System.Linq;
using NUnit.Framework;
using Remotion.Validation.Attributes;
using Remotion.Validation.Implementation;
using Remotion.Validation.Mixins.Implementation;
using Remotion.Validation.Mixins.UnitTests.TestDomain;
using Remotion.Validation.Mixins.UnitTests.TestDomain.Collectors;
using Rhino.Mocks;

namespace Remotion.Validation.Mixins.UnitTests.Implementation
{
  [TestFixture]
  public class DiscoveryServiceBasedValidationCollectorReflectorTest
  {
    private ITypeDiscoveryService _typeDescoveryServiceStub;

    public class FakeValidationCollector<T> : ComponentValidationCollector<T>
    {
      public FakeValidationCollector ()
      {
      }
    }

    [SetUp]
    public void SetUp ()
    {
      _typeDescoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService>();
    }

    [Test]
    public void GetComponentValidationCollectors_WithFakeTypeDiscoveryService ()
    {
      var appliedWithAttributeTypes = new[]
                                      {
                                          typeof (CustomerMixinTargetValidationCollector1), typeof (CustomerMixinIntroducedValidationCollector1),
                                          typeof (CustomerMixinIntroducedValidationCollector2),
                                          typeof (PersonValidationCollector1)
                                      };
      _typeDescoveryServiceStub.Stub (stub => stub.GetTypes (typeof (IComponentValidationCollector), true)).Return (appliedWithAttributeTypes);

      var typeCollectorProvider = DiscoveryServiceBasedValidationCollectorReflector.Create (
          _typeDescoveryServiceStub,
          new MixinTypeAwareValidatedTypeResolverDecorator (
              new ClassTypeAwareValidatedTypeResolverDecorator (
                  new GenericTypeAwareValidatedTypeResolverDecorator (new NullValidatedTypeResolver()))));

      Assert.That (
          typeCollectorProvider.GetCollectorsForType (typeof (ICustomerIntroduced)),
          Is.EqualTo (new[] { typeof (CustomerMixinIntroducedValidationCollector2) }));

      Assert.That (
          typeCollectorProvider.GetCollectorsForType (typeof (CustomerMixin)),
          Is.EquivalentTo (new[] { typeof (CustomerMixinTargetValidationCollector1), typeof (CustomerMixinIntroducedValidationCollector1) }));
      //ApplyWithMixin attribute!
    }

    
  }
}