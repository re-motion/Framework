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
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class LogContextInfoTest
  {
    private IPropertyValidator _propertyValidatorStub1;
    private ValidatorRegistrationWithContext[] _validatorRegistrationWithContext;
    private LogContextInfo _logContextInfo;

    [SetUp]
    public void SetUp ()
    {
      _propertyValidatorStub1 = MockRepository.GenerateStub<IPropertyValidator>();
      _validatorRegistrationWithContext = new ValidatorRegistrationWithContext[0];

      _logContextInfo = new LogContextInfo (_propertyValidatorStub1, _validatorRegistrationWithContext);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_logContextInfo.RemvovedValidator, Is.SameAs (_propertyValidatorStub1));
      Assert.That (_logContextInfo.RemovingValidatorRegistrationsWithContext, Is.SameAs (_validatorRegistrationWithContext));
    }
  }
}