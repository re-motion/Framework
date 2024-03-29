﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Moq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Mixins.Implementation;
using Remotion.Validation.Mixins.UnitTests.TestDomain;
using Remotion.Validation.Mixins.UnitTests.TestDomain.Collectors;

namespace Remotion.Validation.Mixins.UnitTests.Implementation
{
  [TestFixture]
  public class MixinTypeAwareValidatedTypeResolverDecoratorTest
  {
    private Mock<IValidatedTypeResolver> _decoratedResolverMock;
    private MixinTypeAwareValidatedTypeResolverDecorator _resolver;

    [SetUp]
    public void SetUp ()
    {
      _decoratedResolverMock = new Mock<IValidatedTypeResolver>(MockBehavior.Strict);
      _resolver = new MixinTypeAwareValidatedTypeResolverDecorator(_decoratedResolverMock.Object);
    }

    [Test]
    public void GetValidatedType_CollectorWitApplyWithMixinAttribute ()
    {
      var result = _resolver.GetValidatedType(typeof(CustomerMixinIntroducedValidationRuleCollector1));

      _decoratedResolverMock.Verify();
      Assert.That(result, Is.EqualTo(typeof(CustomerMixin)));
    }

    [Test]
    public void GetValidatedType_CollectorWithoutApplyWithMixinAttribute ()
    {
      var collectorTypeWithApplyWithClassAttribute = typeof(PersonValidationRuleCollector1);

      _decoratedResolverMock.Setup(mock => mock.GetValidatedType(collectorTypeWithApplyWithClassAttribute)).Returns(typeof(Person)).Verifiable();

      var result = _resolver.GetValidatedType(collectorTypeWithApplyWithClassAttribute);

      _decoratedResolverMock.Verify();
      Assert.That(result, Is.EqualTo(typeof(Person)));
    }
  }
}
