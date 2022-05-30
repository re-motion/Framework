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
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Validators
{
  [TestFixture]
  public class NotNullValidatorTest : ValidatorTestBase
  {
    [Test]
    public void Validate_WithObject_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new object());
      var validator = new NotNullValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithNull_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(null);
      var validator = new NotNullValidator(new InvariantValidationMessage("Custom validation message."));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      //TODO RM-5906: Assert ValidatedObject, ValidatedProperty, ValidatedValue
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must not be null."));
      Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo("Custom validation message."));
    }
  }
}
