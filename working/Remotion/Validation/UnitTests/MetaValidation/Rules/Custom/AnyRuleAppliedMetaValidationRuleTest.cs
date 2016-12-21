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
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.MetaValidation.Rules.Custom
{
  [TestFixture]
  public class AnyRuleAppliedMetaValidationRuleTest
  {
    private AnyRuleAppliedMetaValidationRule _rule;
    private IPropertyValidator _propertyValidatorStub1;
    private IPropertyValidator _propertyValidatorStub2;

    [SetUp]
    public void SetUp ()
    {
      _propertyValidatorStub1 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub2 = MockRepository.GenerateStub<IPropertyValidator>();

      _rule = new AnyRuleAppliedMetaValidationRule (typeof (Customer).GetProperty ("UserName"));
    }

    [Test]
    public void Validate_NoValidators ()
    {
      var result = _rule.Validate (new IPropertyValidator[0]).Single();

      Assert.That (result.IsValid, Is.False);
      Assert.That (
          result.Message,
          Is.EqualTo (
              "'AnyRuleAppliedMetaValidationRule' failed for property 'Remotion.Validation.UnitTests.TestDomain.Customer.UserName': "
              + "No validation rules defined."));
    }

    [Test]
    public void Validate_WithValidators ()
    {
      var result = _rule.Validate (new[] { _propertyValidatorStub1, _propertyValidatorStub2 }).Single();

      Assert.That (result.IsValid, Is.True);
    }
  }
}