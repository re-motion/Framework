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
using FluentValidation.Internal;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.Validation.Globalization.UnitTests.TestDomain;
using Remotion.Validation.Globalization.UnitTests.TestHelpers;
using Rhino.Mocks;

namespace Remotion.Validation.Globalization.UnitTests
{
  [TestFixture]
  public class PropertyRuleDisplayNameStringSourceTest
  {
    private PropertyRule _propertyRule;
    private Type _typeToValidate;
    private IMemberInformationGlobalizationService _memberInformationGlobalizationServiceMock;
    private PropertyRuleDisplayNameStringSource _stringSource;

    [SetUp]
    public void SetUp ()
    {
      _propertyRule = PropertyRule.Create (ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.FirstName));
      _typeToValidate = typeof (Customer);
      _memberInformationGlobalizationServiceMock = MockRepository.GenerateStrictMock<IMemberInformationGlobalizationService>();

      _stringSource = new PropertyRuleDisplayNameStringSource (_propertyRule, _typeToValidate, _memberInformationGlobalizationServiceMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_stringSource.ResourceName, Is.EqualTo (_typeToValidate.FullName));
      Assert.That (_stringSource.ResourceType, Is.EqualTo (_memberInformationGlobalizationServiceMock.GetType()));
    }

    [Test]
    public void GetString ()
    {
      _memberInformationGlobalizationServiceMock
          .Expect (
              mock => mock.TryGetPropertyDisplayName (
                  Arg<IPropertyInformation>.Matches (pi => pi.Name == "FirstName"),
                  Arg<ITypeInformation>.Matches (ti => ti.Name == "Customer"),
                  out Arg<string>.Out ("FakeLocalizedPropertyName").Dummy))
          .Return (true);

      var result = _stringSource.GetString();

      Assert.That (result, Is.EqualTo ("FakeLocalizedPropertyName"));
    }
  }
}