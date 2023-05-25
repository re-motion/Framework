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
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Validators;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class ObjectValidatorFactoryTest
  {
    [Test]
    public void Create_ReturnsValidator ()
    {
      var fakeCustomerValidator = new FakeCustomerValidator();
      ObjectValidationRuleInitializationParameters actualInitializationParameters = null;
      Func<ObjectValidationRuleInitializationParameters, IObjectValidator> validatorFactoryFunc = parameters =>
      {
        actualInitializationParameters = parameters;
        return fakeCustomerValidator;
      };

      var validatedType = TypeAdapter.Create(typeof(Person));

      var validationMessageFactoryMock = new Mock<IValidationMessageFactory>(MockBehavior.Strict);
      validationMessageFactoryMock
          .Setup(_ => _.CreateValidationMessageForObjectValidator(fakeCustomerValidator, validatedType))
          .Returns(new InvariantValidationMessage("test"));

      var result = ObjectValidatorFactory.Create<IObjectValidator>(validatedType, validatorFactoryFunc, validationMessageFactoryMock.Object);

      Assert.That(result, Is.SameAs(fakeCustomerValidator));
      Assert.That(actualInitializationParameters, Is.Not.Null);
      Assert.That(actualInitializationParameters.ValidationMessage.ToString(), Is.EqualTo("test"));
    }

    [Test]
    public void Create_WithFactoryDelegateReturnsNull_ThrowsInvalidOperationException ()
    {
      Func<ObjectValidationRuleInitializationParameters, IObjectValidator> factoryDelegate = r => null;

      var validatedType = TypeAdapter.Create(typeof(Person));

      var validationMessageFactoryMock = new Mock<IValidationMessageFactory>();
      validationMessageFactoryMock
          .Setup(_ => _.CreateValidationMessageForObjectValidator(null, validatedType))
          .Returns(It.IsAny<ValidationMessage>());

      Assert.That(
          () => ObjectValidatorFactory.Create<IObjectValidator>(validatedType, factoryDelegate, validationMessageFactoryMock.Object),
          Throws.InvalidOperationException.With.Message.EqualTo("validatorFactory evaluated and returned null."));
    }

    [Test]
    public void Create_WithValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
      var stub = new StubObjectValidator();
      Func<ObjectValidationRuleInitializationParameters, IObjectValidator> factoryDelegate = r => stub;

      var validatedType = TypeAdapter.Create(typeof(Person));

      var validationMessageFactoryMock = new Mock<IValidationMessageFactory>();
      validationMessageFactoryMock
          .Setup(_ => _.CreateValidationMessageForObjectValidator(factoryDelegate(It.IsAny<ObjectValidationRuleInitializationParameters>()), validatedType))
          .Returns((ValidationMessage)null);

      Assert.That(
          () => ObjectValidatorFactory.Create<IObjectValidator>(validatedType, factoryDelegate, validationMessageFactoryMock.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The IValidationMessageFactory did not return a result for StubObjectValidator applied to type 'Remotion.Validation.UnitTests.TestDomain.Person'."));
    }
  }
}
