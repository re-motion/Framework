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
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.Validation.Implementation;
using Remotion.Validation.Mixins.Implementation;
using Remotion.Validation.Providers;

namespace Remotion.Data.DomainObjects.Validation.UnitTests
{
  [TestFixture]
  public class IValidationRuleCollectorProviderTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
    }

    [Test]
    public void GetInstance ()
    {
      var factory = _serviceLocator.GetInstance<IValidationRuleCollectorProvider>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(AggregatingValidationRuleCollectorProvider)));
      Assert.That(((AggregatingValidationRuleCollectorProvider)factory).InvolvedTypeProvider, Is.TypeOf(typeof(MixedInvolvedTypeProviderDecorator)));
      var validationCollectorProviders = ((AggregatingValidationRuleCollectorProvider)factory).ValidationCollectorProviders;
      Assert.That(validationCollectorProviders[0], Is.TypeOf(typeof(DomainObjectAttributesBasedValidationRuleCollectorProvider)));
      Assert.That(validationCollectorProviders[1], Is.TypeOf(typeof(ValidationAttributesBasedValidationRuleCollectorProvider)));
      Assert.That(validationCollectorProviders[2], Is.TypeOf(typeof(ApiBasedValidationRuleCollectorProvider)));
      var validationCollectorReflector = ((ApiBasedValidationRuleCollectorProvider)validationCollectorProviders[2]).ValidationRuleCollectorReflector;
      Assert.That(validationCollectorReflector, Is.TypeOf(typeof(DiscoveryServiceBasedValidationRuleCollectorReflector)));
      Assert.That(
          ((DiscoveryServiceBasedValidationRuleCollectorReflector)validationCollectorReflector).ValidatedTypeResolver,
          Is.TypeOf(typeof(MixinTypeAwareValidatedTypeResolverDecorator)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var factory1 = _serviceLocator.GetInstance<IValidationRuleCollectorProvider>();
      var factory2 = _serviceLocator.GetInstance<IValidationRuleCollectorProvider>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
