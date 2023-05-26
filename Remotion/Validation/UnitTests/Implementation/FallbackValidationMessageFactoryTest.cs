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
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class FallbackValidationMessageFactoryTest
  {
    [Test]
    public void CreateValidationMessageForPropertyValidator ()
    {
      var propertyValidatorMock = new Mock<IPropertyValidator>();
      var validatedPropertyMock = new Mock<IPropertyInformation>();

      var fallbackFactory = new FallbackValidationMessageFactory();
      var result = fallbackFactory.CreateValidationMessageForPropertyValidator(propertyValidatorMock.Object, validatedPropertyMock.Object);

      Assert.That(result, Is.InstanceOf<InvariantValidationMessage>());
      Assert.That(result.ToString(), Is.EqualTo($"IPropertyValidatorProxy: Validation error."));
    }

    [Test]
    public void CreateValidationMessageForObjectValidator ()
    {
      var objectValidatorMock = new Mock<IObjectValidator>();
      var validatedType = TypeAdapter.Create(typeof(Person));

      var fallbackFactory = new FallbackValidationMessageFactory();
      var result = fallbackFactory.CreateValidationMessageForObjectValidator(objectValidatorMock.Object, validatedType);

      Assert.That(result, Is.InstanceOf<InvariantValidationMessage>());
      Assert.That(result.ToString(), Is.EqualTo($"IObjectValidatorProxy: Validation error."));
    }

    [Test]
    public void CreateValidationMessageForObjectValidator_WithNullAsValidator_ThrowsArgumentNullException ()
    {
      var validatedType = TypeAdapter.Create(typeof(Person));

      var fallbackFactory = new FallbackValidationMessageFactory();

      Assert.That(() => fallbackFactory.CreateValidationMessageForObjectValidator(null, validatedType), Throws.ArgumentNullException);
    }

    [Test]
    public void CreateValidationMessageForObjectValidator_WithNullAsValidatedType_ThrowsArgumentNullException ()
    {
      var objectValidatorMock = new Mock<IObjectValidator>();

      var fallbackFactory = new FallbackValidationMessageFactory();

      Assert.That(() => fallbackFactory.CreateValidationMessageForObjectValidator(objectValidatorMock.Object, null), Throws.ArgumentNullException);
    }
  }
}
