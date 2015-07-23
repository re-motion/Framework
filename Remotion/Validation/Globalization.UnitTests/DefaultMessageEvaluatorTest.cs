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
using FluentValidation.Resources;
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Validation.Globalization.UnitTests.TestHelpers;

namespace Remotion.Validation.Globalization.UnitTests
{
  [TestFixture]
  public class DefaultMessageEvaluatorTest
  {
    private DefaultMessageEvaluator _evaluator;

    [SetUp]
    public void SetUp ()
    {
      _evaluator = new DefaultMessageEvaluator();
    }

    [Test]
    public void NotNullValidator ()
    {
      CheckValidatorDefaultMessage (new NotNullValidator());
    }

    [Test]
    public void NotEmptyValidator ()
    {
      CheckValidatorDefaultMessage (new NotEmptyValidator (null));
    }

    [Test]
    public void NotEqualtValidator ()
    {
      CheckValidatorDefaultMessage (new NotEqualValidator ("test"));
    }

    [Test]
    public void EqualValidator ()
    {
      CheckValidatorDefaultMessage (new EqualValidator ("test"));
    }

    [Test]
    public void CreditCardValidator ()
    {
      CheckValidatorDefaultMessage (new CreditCardValidator());
    }

    [Test]
    public void CheckEmailValidator ()
    {
      CheckValidatorDefaultMessage (new EmailValidator());
    }

    [Test]
    public void ExactLengthValidator ()
    {
      CheckValidatorDefaultMessage (new ExactLengthValidator (10));
    }

    [Test]
    public void LengthValidator ()
    {
      CheckValidatorDefaultMessage (new LengthValidator (5, 10));
    }

    [Test]
    public void MaximumLengthValidator ()
    {
      CheckValidatorDefaultMessage (new MaximumLengthValidator (10));
    }

    [Test]
    public void MinimumLengthValidator ()
    {
      CheckValidatorDefaultMessage (new MinimumLengthValidator (5));
    }

    [Test]
    public void ExclusiveBetweenValidator ()
    {
      CheckValidatorDefaultMessage (new ExclusiveBetweenValidator (5, 10));
    }

    [Test]
    public void InclusiveBetweenValidator ()
    {
      CheckValidatorDefaultMessage (new InclusiveBetweenValidator (5, 10));
    }

    [Test]
    public void LessThanValidator ()
    {
      CheckValidatorDefaultMessage (new LessThanValidator (5));
    }

    [Test]
    public void LessThanOrEqualValidator ()
    {
      CheckValidatorDefaultMessage (new LessThanOrEqualValidator (5));
    }

    [Test]
    public void GreaterThanValidator ()
    {
      CheckValidatorDefaultMessage (new GreaterThanValidator (5));
    }

    [Test]
    public void GreaterThanOrEqualValidator ()
    {
      CheckValidatorDefaultMessage (new GreaterThanOrEqualValidator (5));
    }

    [Test]
    public void PredicateValidator ()
    {
      CheckValidatorDefaultMessage (new PredicateValidator((o1, o2, o3) => true));
    }

    [Test]
    public void RegularExpressionValidator ()
    {
      CheckValidatorDefaultMessage (new RegularExpressionValidator (""));
    }

    [Test]
    public void ScalePrecisionValidator ()
    {
      CheckValidatorDefaultMessage (new ScalePrecisionValidator (2, 5));
    }

    [Test]
    public void UnknownValidator ()
    {
      Assert.That (_evaluator.HasDefaultMessageAssigned (new StubPropertyValidator()), Is.False);
    }

    private void CheckValidatorDefaultMessage (IPropertyValidator validator)
    {
      Assert.That (_evaluator.HasDefaultMessageAssigned (validator), Is.True);

      validator.ErrorMessageSource = new StaticStringSource ("New Message");
      Assert.That (_evaluator.HasDefaultMessageAssigned (validator), Is.False);
    }

  }
}