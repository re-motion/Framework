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
using NUnit.Framework;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.ObjectBinding.Validation.UnitTests
{
  [TestFixture]
  public class PropertyValidatorToBusinessObjectPropertyConstraintConverterTest
  {
    [Test]
    public void Convert_WithDifferentMaxLengthValidators_ContainsLowerMaxLength ()
    {
      var validators = new []
                       {
                         new MaximumLengthValidator(7, new InvariantValidationMessage("Max length is 7")),
                         new MaximumLengthValidator(5, new InvariantValidationMessage("Max length is 5")),
                         new MaximumLengthValidator(7, new InvariantValidationMessage("Max length is 7"))
                       };

      var converter = new PropertyValidatorToBusinessObjectPropertyConstraintConverter();

      var constraints = converter.Convert(validators).ToArray();

      Assert.That(constraints.Length, Is.EqualTo(1));
      Assert.That(constraints[0], Is.InstanceOf<BusinessObjectPropertyValueLengthConstraint>());
      Assert.That(((BusinessObjectPropertyValueLengthConstraint)constraints[0]).MaxLength, Is.EqualTo(5));
    }

    [Test]
    public void Convert_WithoutValidators_ContainsNoConstraints ()
    {
      var validators = Array.Empty<IPropertyValidator>();

      var converter = new PropertyValidatorToBusinessObjectPropertyConstraintConverter();

      var constraints = converter.Convert(validators).ToArray();

      Assert.That(constraints, Is.Empty);
    }

    [Test]
    public void Convert_WithRequiredValidators_ContainsRequiredConstraint ()
    {
      var validators = new []
                       {
                         new NotNullValidator(new InvariantValidationMessage("Required")),
                         new NotNullValidator(new InvariantValidationMessage("Required number 2")),
                       };

      var converter = new PropertyValidatorToBusinessObjectPropertyConstraintConverter();

      var constraints = converter.Convert(validators).ToArray();

      Assert.That(constraints.Length, Is.EqualTo(1));
      Assert.That(constraints[0], Is.InstanceOf<BusinessObjectPropertyValueRequiredConstraint>());
    }

    [Test]
    public void Convert_WithRequiredAndLengthValidators_ContainsRequiredAndLengthConstraint ()
    {
      var validators = new IPropertyValidator[]
                       {
                         new NotNullValidator(new InvariantValidationMessage("Required")),
                         new NotNullValidator(new InvariantValidationMessage("Required number 2")),
                         new MaximumLengthValidator(5, new InvariantValidationMessage("Max length is 5"))
                       };

      var converter = new PropertyValidatorToBusinessObjectPropertyConstraintConverter();

      var constraints = converter.Convert(validators).ToArray();

      Assert.That(constraints.Length, Is.EqualTo(2));
      Assert.That(
          constraints,
          Has.Some.InstanceOf<BusinessObjectPropertyValueLengthConstraint>()
              .And.Some.InstanceOf<BusinessObjectPropertyValueRequiredConstraint>());
    }
  }
}
