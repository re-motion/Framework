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
using System.Reflection;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Resources;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.Validation.Globalization.UnitTests.TestDomain;
using Remotion.Validation.Globalization.UnitTests.TestHelpers;
using Rhino.Mocks;

namespace Remotion.Validation.Globalization.UnitTests
{
  [TestFixture]
  public class PropertyDisplayNameGlobalizationServiceTest
  {
    private IMemberInformationGlobalizationService _memberInformationGlobalizationServiceMock;
    private PropertyDisplayNameGlobalizationService _service;
    private PropertyRule _propertyRule;
    private IValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _memberInformationGlobalizationServiceMock = MockRepository.GenerateStrictMock<IMemberInformationGlobalizationService>();

      _validationRule = MockRepository.GenerateStub<IValidationRule>();
      _propertyRule = PropertyRule.Create (ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName));

      _service = new PropertyDisplayNameGlobalizationService (_memberInformationGlobalizationServiceMock);
    }

    [Test]
    public void ApplyLocalization_NoPropertyRule ()
    {
      _service.ApplyMetadata (_validationRule, typeof (Customer));

      _memberInformationGlobalizationServiceMock.VerifyAllExpectations();
    }

    [Test]
    public void ApplyLocalization_DisplayNameAlreadySet ()
    {
      _propertyRule.DisplayName = new StaticStringSource ("Dummy");

      _service.ApplyMetadata (_propertyRule, typeof (Customer));

      Assert.That (_propertyRule.DisplayName.GetString(), Is.EqualTo ("Dummy"));
      _memberInformationGlobalizationServiceMock.VerifyAllExpectations();
    }

    [Test]
    public void ApplyLocalization_DisplayNameIsAssigned ()
    {
      _memberInformationGlobalizationServiceMock
          .Expect (
              mock => mock.TryGetPropertyDisplayName (
                  Arg<IPropertyInformation>.Matches (pi => ((PropertyInfoAdapter) pi).PropertyInfo == (PropertyInfo) _propertyRule.Member),
                  Arg<ITypeInformation>.Matches (ti => ((TypeAdapter) ti).Type == typeof (Customer)),
                  out Arg<string>.Out ("LocalizedPropertyName").Dummy))
          .Return (true);
      Assert.That (_propertyRule.DisplayName, Is.TypeOf (typeof (LazyStringSource)));

      _service.ApplyMetadata (_propertyRule, typeof (Customer));

      Assert.That (_propertyRule.DisplayName, Is.Not.Null);
      Assert.That (_propertyRule.DisplayName, Is.TypeOf (typeof (PropertyRuleDisplayNameStringSource)));
      Assert.That (_propertyRule.DisplayName.GetString(), Is.EqualTo ("LocalizedPropertyName"));
      _memberInformationGlobalizationServiceMock.VerifyAllExpectations();
    }
  }
}