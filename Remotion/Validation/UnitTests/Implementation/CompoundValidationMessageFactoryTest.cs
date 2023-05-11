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
using Moq;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundValidationMessageFactoryTest
  {
    [Test]
    public void CreateValidationMessageForPropertyValidator_ReturnsMessageFromFactory ()
    {
      var validatorStub = new Mock<IPropertyValidator>();
      var propertyStub = new Mock<IPropertyInformation>();

      var expectedMessage = new InvariantValidationMessage("Oh no, a validation error!");

      var factoryStub = new Mock<IValidationMessageFactory>();
      factoryStub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(validatorStub.Object, propertyStub.Object))
          .Returns(expectedMessage);

      var factory = new CompoundValidationMessageFactory(new[] { factoryStub.Object });

      var result = factory.CreateValidationMessageForPropertyValidator(validatorStub.Object, propertyStub.Object);

      Assert.That(result, Is.SameAs(expectedMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMultipleFactories_ReturnsMessageFromFirstFactory ()
    {
      var validatorStub = new Mock<IPropertyValidator>();
      var propertyStub = new Mock<IPropertyInformation>();

      var expectedMessage = new InvariantValidationMessage("Oh no, a validation error!");

      var factory1Stub = new Mock<IValidationMessageFactory>();
      factory1Stub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(validatorStub.Object, propertyStub.Object))
          .Returns(expectedMessage);

      var extraMessage = new InvariantValidationMessage("I sure hope this is not displayed.");

      var factory2Stub = new Mock<IValidationMessageFactory>();
      factory2Stub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(validatorStub.Object, propertyStub.Object))
          .Returns(extraMessage);

      var factory = new CompoundValidationMessageFactory(new[] { factory1Stub.Object, factory2Stub.Object });

      var result = factory.CreateValidationMessageForPropertyValidator(validatorStub.Object, propertyStub.Object);

      Assert.That(result, Is.EqualTo(expectedMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithFirstFactoryReturningNullMessage_ReturnsMessageFromSecondFactory ()
    {
      var validatorStub = new Mock<IPropertyValidator>();
      var propertyStub = new Mock<IPropertyInformation>();

      var factory1Stub = new Mock<IValidationMessageFactory>();
      factory1Stub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(validatorStub.Object, propertyStub.Object))
          .Returns((ValidationMessage)null);

      var expectedMessage = new InvariantValidationMessage("Oh no, a validation error!");

      var factory2Stub = new Mock<IValidationMessageFactory>();
      factory2Stub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(validatorStub.Object, propertyStub.Object))
          .Returns(expectedMessage);

      var factory = new CompoundValidationMessageFactory(new[] { factory1Stub.Object, factory2Stub.Object });

      var result = factory.CreateValidationMessageForPropertyValidator(validatorStub.Object, propertyStub.Object);

      Assert.That(result, Is.EqualTo(expectedMessage));
    }

    [Test]
    public void CreateValidationMessageForObjectValidator_ReturnsMessageFromSavedFactory ()
    {
      var validatorStub = new Mock<IObjectValidator>();
      var typeInfoStub = new Mock<ITypeInformation>();

      var expectedMessage = new InvariantValidationMessage("Oh no, a validation error!");

      var factoryStub = new Mock<IValidationMessageFactory>();
      factoryStub
          .Setup(_ => _.CreateValidationMessageForObjectValidator(validatorStub.Object, typeInfoStub.Object))
          .Returns(expectedMessage);

      var factory = new CompoundValidationMessageFactory(new[] { factoryStub.Object });

      var result = factory.CreateValidationMessageForObjectValidator(validatorStub.Object, typeInfoStub.Object);

      Assert.That(result, Is.EqualTo(expectedMessage));
    }

    [Test]
    public void CreateValidationMessageForObjectValidator_WithSeveralFactories_ReturnsMessageFromFirstFactory ()
    {
      var validatorStub = new Mock<IObjectValidator>();
      var typeInfoStub = new Mock<ITypeInformation>();

      var expectedMessage = new InvariantValidationMessage("Oh no, a validation error!");

      var factory1Stub = new Mock<IValidationMessageFactory>();
      factory1Stub
          .Setup(_ => _.CreateValidationMessageForObjectValidator(validatorStub.Object, typeInfoStub.Object))
          .Returns(expectedMessage);

      var extraMessage = new InvariantValidationMessage("I sure hope this is not displayed.");

      var factory2Stub = new Mock<IValidationMessageFactory>();
      factory2Stub
          .Setup(_ => _.CreateValidationMessageForObjectValidator(validatorStub.Object, typeInfoStub.Object))
          .Returns(extraMessage);

      var factory = new CompoundValidationMessageFactory(new[] { factory1Stub.Object, factory2Stub.Object });

      var result = factory.CreateValidationMessageForObjectValidator(validatorStub.Object, typeInfoStub.Object);

      Assert.That(result, Is.EqualTo(expectedMessage));
    }

    [Test]
    public void CreateValidationMessageForObjectValidator_WithFirstFactoryReturningNullMessage_ReturnsMessageFromSecondFactory ()
    {
      var validatorStub = new Mock<IObjectValidator>();
      var typeInfoStub = new Mock<ITypeInformation>();

      var factory1Stub = new Mock<IValidationMessageFactory>();
      factory1Stub
          .Setup(_ => _.CreateValidationMessageForObjectValidator(validatorStub.Object, typeInfoStub.Object))
          .Returns((ValidationMessage)null);

      var expectedMessage = new InvariantValidationMessage("Oh no, a validation error!");

      var factory2Stub = new Mock<IValidationMessageFactory>();
      factory2Stub
          .Setup(_ => _.CreateValidationMessageForObjectValidator(validatorStub.Object, typeInfoStub.Object))
          .Returns(expectedMessage);

      var factory = new CompoundValidationMessageFactory(new[] { factory1Stub.Object, factory2Stub.Object });

      var result = factory.CreateValidationMessageForObjectValidator(validatorStub.Object, typeInfoStub.Object);

      Assert.That(result, Is.EqualTo(expectedMessage));
    }
  }
}
