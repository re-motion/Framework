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
using Moq;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.Attributes.Validation;
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Attributes.Validation
{
  [TestFixture]
  public class LengthValidationAttributeTest
  {
    private Mock<IValidationMessageFactory> _validationMessageFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _validationMessageFactoryStub = new Mock<IValidationMessageFactory>();
    }

    [Test]
    public void GetAndSetMinimumLength ()
    {
      var lengthValidationAttribute = new LengthValidationAttribute();

      Assert.That(lengthValidationAttribute.MinLength, Is.EqualTo(0));
      lengthValidationAttribute.MinLength = 10;
      Assert.That(lengthValidationAttribute.MinLength, Is.EqualTo(10));
    }

    [Test]
    public void GetAndSetMaximumLength ()
    {
      var lengthValidationAttribute = new LengthValidationAttribute();

      Assert.That(lengthValidationAttribute.MaxLength, Is.EqualTo(0));
      lengthValidationAttribute.MaxLength = 20;
      Assert.That(lengthValidationAttribute.MaxLength, Is.EqualTo(20));
    }

    [Test]
    public void GetPropertyValidator_WithoutMinAndMaxValues_ReturnsEmptyResult ()
    {
      var propertyInformation = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("LastName"));
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(It.IsAny<IPropertyValidator>(), propertyInformation))
          .Returns(validationMessageStub.Object);

      var lengthValidationAttribute = new LengthValidationAttribute();
      var result = lengthValidationAttribute.GetPropertyValidators(propertyInformation, _validationMessageFactoryStub.Object).ToArray();

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetPropertyValidator_WithNotEqualMinAndMaxValues_ReturnsLengthValidator ()
    {
      var propertyInformation = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("LastName"));
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(It.IsAny<LengthValidator>(), propertyInformation))
          .Returns(validationMessageStub.Object);

      var lengthValidationAttribute = new LengthValidationAttribute() { MinLength = 10, MaxLength = 20 };
      var result = lengthValidationAttribute.GetPropertyValidators(propertyInformation, _validationMessageFactoryStub.Object).ToArray();

      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(LengthValidator)));
      Assert.That(((LengthValidator)result[0]).Min, Is.EqualTo(10));
      Assert.That(((LengthValidator)result[0]).Max, Is.EqualTo(20));
      Assert.That(((LengthValidator)result[0]).ValidationMessage, Is.Not.Null);

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      Assert.That(((LengthValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    public void GetPropertyValidator_WithEqualMinAndMaxValues_ReturnsExactLengthValidator ()
    {
      var propertyInformation = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("LastName"));
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(It.IsAny<ExactLengthValidator>(), propertyInformation))
          .Returns(validationMessageStub.Object);

      var exactLengthValidationAttribute = new LengthValidationAttribute() { MinLength = 10, MaxLength = 10 };
      var result = exactLengthValidationAttribute.GetPropertyValidators(propertyInformation, _validationMessageFactoryStub.Object).ToArray();

      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(ExactLengthValidator)));
      Assert.That(((ExactLengthValidator)result[0]).Length, Is.EqualTo(10));
      Assert.That(((ExactLengthValidator)result[0]).ValidationMessage, Is.Not.Null);

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      Assert.That(((ExactLengthValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    public void GetPropertyValidator_WithMinValue_ReturnsMinimumLengthValidator ()
    {
      var propertyInformation = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("LastName"));
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(It.IsAny<MinimumLengthValidator>(), propertyInformation))
          .Returns(validationMessageStub.Object);

      var minimumLengthValidationAttribute = new LengthValidationAttribute() { MinLength = 10 };
      var result = minimumLengthValidationAttribute.GetPropertyValidators(propertyInformation, _validationMessageFactoryStub.Object).ToArray();

      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(MinimumLengthValidator)));
      Assert.That(((MinimumLengthValidator)result[0]).Min, Is.EqualTo(10));
      Assert.That(((MinimumLengthValidator)result[0]).ValidationMessage, Is.Not.Null);

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      Assert.That(((MinimumLengthValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    public void GetPropertyValidator_WithMaxValue_ReturnsMaximumLengthValidator ()
    {
      var propertyInformation = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("LastName"));
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(It.IsAny<MaximumLengthValidator>(), propertyInformation))
          .Returns(validationMessageStub.Object);

      var maximumLengthValidationAttribute = new LengthValidationAttribute() { MaxLength = 10 };
      var result = maximumLengthValidationAttribute.GetPropertyValidators(propertyInformation, _validationMessageFactoryStub.Object).ToArray();

      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(MaximumLengthValidator)));
      Assert.That(((MaximumLengthValidator)result[0]).Max, Is.EqualTo(10));
      Assert.That(((MaximumLengthValidator)result[0]).ValidationMessage, Is.Not.Null);

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      Assert.That(((MaximumLengthValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    public void GetPropertyValidator_CustomMessage ()
    {
      var propertyInformation = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("LastName"));

      var lengthValidationAttribute = new LengthValidationAttribute { MinLength = 10, MaxLength = 20, ErrorMessage = "CustomMessage" };
      var result = lengthValidationAttribute.GetPropertyValidators(propertyInformation, _validationMessageFactoryStub.Object).ToArray();

      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(((LengthValidator)result[0]).ValidationMessage, Is.InstanceOf<InvariantValidationMessage>());
      Assert.That(((LengthValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("CustomMessage"));
    }

    [Test]
    public void GetPropertyValidator_WithValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
      var propertyInformation = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("LastName"));
      _validationMessageFactoryStub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(It.IsAny<LengthValidator>(), propertyInformation))
          .Returns((ValidationMessage)null);

      var lengthValidationAttribute = new LengthValidationAttribute() { MinLength = 10, MaxLength = 20 };
      Assert.That(() => lengthValidationAttribute.GetPropertyValidators(propertyInformation, _validationMessageFactoryStub.Object), Throws.InvalidOperationException);
    }
  }
}
