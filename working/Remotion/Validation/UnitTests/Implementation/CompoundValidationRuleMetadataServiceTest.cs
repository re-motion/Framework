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
using FluentValidation;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundValidationRuleMetadataServiceTest
  {
    private IValidationRuleMetadataService _validationRuleGlobalizationServiceMock1;
    private IValidationRuleMetadataService _validationRuleGlobalizationServiceMock2;
    private IValidationRuleMetadataService _validationRuleGlobalizationServiceMock3;
    private CompoundValidationRuleMetadataService _service;
    private IValidationRule _validationRuleStub;
    private Type _typeToValidate;

    [SetUp]
    public void SetUp ()
    {
      _validationRuleStub = MockRepository.GenerateStub<IValidationRule>();
      _typeToValidate = typeof (Customer);

      _validationRuleGlobalizationServiceMock1 = MockRepository.GenerateStrictMock<IValidationRuleMetadataService>();
      _validationRuleGlobalizationServiceMock2 = MockRepository.GenerateStrictMock<IValidationRuleMetadataService>();
      _validationRuleGlobalizationServiceMock3 = MockRepository.GenerateStrictMock<IValidationRuleMetadataService>();

      _service =
          new CompoundValidationRuleMetadataService (
              new[] { _validationRuleGlobalizationServiceMock1, _validationRuleGlobalizationServiceMock2, _validationRuleGlobalizationServiceMock3 });
    }

    [Test]
    public void ApplyLocalization ()
    {
      _validationRuleGlobalizationServiceMock1.Expect (mock => mock.ApplyMetadata (_validationRuleStub, _typeToValidate)).Repeat.Once();
      _validationRuleGlobalizationServiceMock2.Expect (mock => mock.ApplyMetadata (_validationRuleStub, _typeToValidate)).Repeat.Once();
      _validationRuleGlobalizationServiceMock3.Expect (mock => mock.ApplyMetadata (_validationRuleStub, _typeToValidate)).Repeat.Once();

      _service.ApplyMetadata (_validationRuleStub, _typeToValidate);

      _validationRuleGlobalizationServiceMock1.VerifyAllExpectations();
      _validationRuleGlobalizationServiceMock2.VerifyAllExpectations();
      _validationRuleGlobalizationServiceMock3.VerifyAllExpectations();
    }
  }
}