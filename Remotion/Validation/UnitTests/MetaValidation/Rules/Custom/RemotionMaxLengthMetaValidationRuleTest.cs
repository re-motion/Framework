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
using Remotion.Validation.MetaValidation.Rules.Custom;
using Remotion.Validation.UnitTests.TestDomain;

namespace Remotion.Validation.UnitTests.MetaValidation.Rules.Custom
{
  [TestFixture]
  public class RemotionMaxLengthMetaValidationRuleTest
  {
    private RemotionMaxLengthMetaValidationRule _rule;

    [SetUp]
    public void SetUp ()
    {
      _rule = new RemotionMaxLengthMetaValidationRule (typeof (Customer).GetProperty ("UserName"), 50);
    }

    [Test]
    public void Validate_NoRule ()
    {
      var result = _rule.Validate (new MaximumLengthValidator[0]).ToArray().Single();

      Assert.That (result.IsValid, Is.False);
      Assert.That (
          result.Message,
          Is.EqualTo (
              "'RemotionMaxLengthMetaValidationRule' failed for property 'Remotion.Validation.UnitTests.TestDomain.Customer.UserName': No max-length validation rules defined."));
    }

    [Test]
    public void Validate_MaxLengthExceeds ()
    {
      var result = _rule.Validate (new[] { new MaximumLengthValidator (60) }).ToArray().Single();

      Assert.That (result.IsValid, Is.False);
      Assert.That (
          result.Message,
          Is.EqualTo (
              "'RemotionMaxLengthMetaValidationRule' failed for property 'Remotion.Validation.UnitTests.TestDomain.Customer.UserName': "
              + "Max-length validation rule value '60' exceeds meta validation rule max-length value of '50'."));
    }

    [Test]
    public void Validate_ValidMaxLength ()
    {
      var result = _rule.Validate (new[] { new MaximumLengthValidator (40) }).ToArray().Single();

      Assert.That (result.IsValid, Is.True);
    }
  }
}