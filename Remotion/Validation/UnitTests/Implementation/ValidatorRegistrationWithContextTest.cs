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
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Rules;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class ValidatorRegistrationWithContextTest
  {
    private ValidatorRegistration _validatorRegistration;
    private ValidatorRegistrationWithContext _registrationWithContext;
    private IRemovingComponentPropertyRule _removingPropertyRuleStub;

    [SetUp]
    public void SetUp ()
    {
      _validatorRegistration = new ValidatorRegistration (typeof (NotEqualValidator), null);
      _removingPropertyRuleStub = MockRepository.GenerateStub<IRemovingComponentPropertyRule>();
      _registrationWithContext = new ValidatorRegistrationWithContext (_validatorRegistration, _removingPropertyRuleStub);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_registrationWithContext.ValidatorRegistration, Is.SameAs (_validatorRegistration));
      Assert.That (_registrationWithContext.RemovingComponentPropertyRule, Is.SameAs(_removingPropertyRuleStub));
    }
  }
}