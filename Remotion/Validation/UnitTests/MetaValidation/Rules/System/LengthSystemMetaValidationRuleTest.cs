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
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.MetaValidation.Rules.System;
using Remotion.Validation.UnitTests.TestDomain;

namespace Remotion.Validation.UnitTests.MetaValidation.Rules.System
{
  [TestFixture]
  public class LengthSystemMetaValidationRuleTest
  {
    private LengthSystemMetaValidationRule _rule;
    private MaximumLengthValidator _maxLengthValidator1;
    private MaximumLengthValidator _maxLengthValidator2;
    private MinimumLengthValidator _minLengthValidator1;
    private MinimumLengthValidator _minLengthValidator2;

    [SetUp]
    public void SetUp ()
    {
      _maxLengthValidator1 = new MaximumLengthValidator (50);
      _maxLengthValidator2 = new MaximumLengthValidator (60);

      _minLengthValidator1 = new MinimumLengthValidator (10);
      _minLengthValidator2 = new MinimumLengthValidator (20);

      _rule = new LengthSystemMetaValidationRule (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("UserName")));
    }

    [Test]
    public void Validate_NoRules ()
    {
      var result = _rule.Validate (new LengthValidator[0]).ToArray().Single();

      Assert.That (result.IsValid, Is.True);
    }

    [Test]
    public void Validate_ValidRules ()
    {
      var result = _rule.Validate (new LengthValidator[] { _minLengthValidator1, _maxLengthValidator2 }).ToArray().Single();

      Assert.That (result.IsValid, Is.True);
    }

    [Test]
    public void Validate_VInvalidRules ()
    {
      var result =
          _rule.Validate (new LengthValidator[] { _minLengthValidator1, _minLengthValidator2, _maxLengthValidator1, _maxLengthValidator2 })
              .ToArray()
              .Single();

      Assert.That (result.IsValid, Is.False);
      Assert.That (
          result.Message,
          Is.EqualTo (
              "'LengthSystemMetaValidationRule' failed for member 'Remotion.Validation.UnitTests.TestDomain.Customer.UserName'."));
    }
  }
}