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
using Moq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class ClassTypeAwareValidatedTypeResolverDecoratorTest
  {
    private Mock<IValidatedTypeResolver> _decoratedResolverMock;
    private ClassTypeAwareValidatedTypeResolverDecorator _resolver;

    [SetUp]
    public void SetUp ()
    {
      _decoratedResolverMock = new Mock<IValidatedTypeResolver>(MockBehavior.Strict);
      _resolver = new ClassTypeAwareValidatedTypeResolverDecorator(_decoratedResolverMock.Object);
    }

    [Test]
    public void GetValidatedType_CollectorWitApplyWithClassAttribute ()
    {
      var collectorTypeWithApplyWithClassAttribute = typeof(PersonValidationRuleCollector2);

      var result = _resolver.GetValidatedType(collectorTypeWithApplyWithClassAttribute);

      _decoratedResolverMock.Verify();
      Assert.That(result, Is.EqualTo(typeof(Person)));
    }

    [Test]
    public void GetValidatedType_CollectorWitApplyWithClassAttribute_ReturnedTypeNotAssignableToGenericType ()
    {
      var collectorTypeWithApplyWithClassAttribute = typeof(InvalidValidationRuleCollector);

      _decoratedResolverMock.Setup(mock => mock.GetValidatedType(collectorTypeWithApplyWithClassAttribute)).Returns(typeof(Customer)).Verifiable();
      Assert.That(
          () => _resolver.GetValidatedType(collectorTypeWithApplyWithClassAttribute),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Invalid 'ApplyWithClassAttribute'-definition for collector 'Remotion.Validation.UnitTests.TestDomain.Collectors.InvalidValidationRuleCollector': "
                  + "type 'Remotion.Validation.UnitTests.TestDomain.Address' is not assignable from 'Remotion.Validation.UnitTests.TestDomain.Customer'."));
    }

    [Test]
    public void GetValidatedType_CollectorWitApplyWithClassAttribute_WithoutGenericType ()
    {
      var collectorTypeWithApplyWithClassAttribute = typeof(InvalidValidationRuleRuleCollector2);

      _decoratedResolverMock.Setup(mock => mock.GetValidatedType(collectorTypeWithApplyWithClassAttribute)).Returns(typeof(Customer)).Verifiable();

      var result = _resolver.GetValidatedType(collectorTypeWithApplyWithClassAttribute);

      Assert.That(result, Is.EqualTo(typeof(Address)));
    }

    [Test]
    public void GetValidatedType_CollectorWithoutApplyWithClassAttribute ()
    {
      var collectorTypeWithApplyWithClassAttribute = typeof(PersonValidationRuleCollector1);

      _decoratedResolverMock.Setup(mock => mock.GetValidatedType(collectorTypeWithApplyWithClassAttribute)).Returns(typeof(Person)).Verifiable();

      var result = _resolver.GetValidatedType(collectorTypeWithApplyWithClassAttribute);

      _decoratedResolverMock.Verify();
      Assert.That(result, Is.EqualTo(typeof(Person)));
    }
  }
}
