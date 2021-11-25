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
  public class GenericTypeAwareValidatedTypeResolverDecoratorTest
  {
    private Mock<IValidatedTypeResolver> _decoratedResolverMock;
    private GenericTypeAwareValidatedTypeResolverDecorator _resolver;

    [SetUp]
    public void SetUp ()
    {
      _decoratedResolverMock = new Mock<IValidatedTypeResolver>(MockBehavior.Strict);
      _resolver = new GenericTypeAwareValidatedTypeResolverDecorator(_decoratedResolverMock.Object);
    }

    [Test]
    public void GetValidatedType_CollectorWithoutGenericArgument ()
    {
      _decoratedResolverMock.Setup(mock => mock.GetValidatedType(typeof (IValidationRuleCollector))).Returns(typeof (string)).Verifiable();

      var result = _resolver.GetValidatedType(typeof (IValidationRuleCollector));

      _decoratedResolverMock.Verify();
      Assert.That(result, Is.EqualTo(typeof (string)));
    }

    [Test]
    public void GetValidatedType_CollectorWithGenericArgument ()
    {
      var collectorTypeWithApplyWithClassAttribute = typeof (PersonValidationRuleCollector2);

      var result = _resolver.GetValidatedType(collectorTypeWithApplyWithClassAttribute);

      _decoratedResolverMock.Verify();
      Assert.That(result, Is.EqualTo(typeof (IPerson)));
    }
    
  }
}