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
  public class NotEqualValidationAttributeTest
  {
    private NotEqualValidationAttribute _attribute;
    private Mock<IValidationMessageFactory> _validationMessageFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _attribute = new NotEqualValidationAttribute("test");
      _validationMessageFactoryStub = new Mock<IValidationMessageFactory>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_attribute.Value, Is.EqualTo("test"));
    }

    [Test]
    public void GetPropertyValidator ()
    {
      var propertyInformation = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("LastName"));
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(It.IsAny<NotEqualValidator>(), propertyInformation))
          .Returns(validationMessageStub.Object);

      var result = _attribute.GetPropertyValidators(propertyInformation, _validationMessageFactoryStub.Object).ToArray();

      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(NotEqualValidator)));
      Assert.That(((NotEqualValidator) result[0]).ComparisonValue, Is.EqualTo("test"));
      Assert.That(((NotEqualValidator) result[0]).ValidationMessage, Is.Not.Null);

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      Assert.That(((NotEqualValidator) result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    public void GetPropertyValidator_CustomErrorMessage ()
    {
      var propertyInformation = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("LastName"));
      _attribute.ErrorMessage = "CustomMessage";

      var result = _attribute.GetPropertyValidators(propertyInformation, _validationMessageFactoryStub.Object).ToArray();

      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(((NotEqualValidator) result[0]).ValidationMessage, Is.InstanceOf<InvariantValidationMessage>());
      Assert.That(((NotEqualValidator) result[0]).ValidationMessage.ToString(), Is.EqualTo("CustomMessage"));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetPropertyValidator_WithValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }
  }
}