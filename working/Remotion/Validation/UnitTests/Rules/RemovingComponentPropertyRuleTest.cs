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
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;

namespace Remotion.Validation.UnitTests.Rules
{
  [TestFixture]
  public class RemovingComponentPropertyRuleTest
  {
    private Expression<Func<Customer, string>> _userNameExpression;
    private Expression<Func<Customer, string>> _lastNameExpression;
    private IRemovingComponentPropertyRule _removingComponentPropertyRule;
    private IPropertyInformation _property;

    [SetUp]
    public void SetUp ()
    {
      _property = PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("UserName"));

      _userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.UserName);
      _lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName);

      _removingComponentPropertyRule = RemovingComponentPropertyRule.Create (_userNameExpression, typeof (RemovingComponentPropertyRuleTest));
    }

    [Test]
    public void Initialization_PropertyDeclaredInSameClass ()
    {
      Assert.That (_removingComponentPropertyRule.Property.Equals(_property), Is.True);
      Assert.That (_removingComponentPropertyRule.Property, Is.EqualTo (_property));
      Assert.That (_removingComponentPropertyRule.CollectorType, Is.EqualTo (typeof (RemovingComponentPropertyRuleTest)));
      Assert.That (_removingComponentPropertyRule.Validators.Any(), Is.False);
    }

    [Test]
    public void Create_MemberInfoIsNoPropertyInfo_ExceptionIsThrown ()
    {
      var dummyExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.Dummy ());

      Assert.Throws<InvalidOperationException> (
          () => RemovingComponentPropertyRule.Create (dummyExpression, typeof (CustomerValidationCollector1)),
          "An 'RemovingComponentPropertyRule' can only created for property members.");
    }

    [Test]
    public void Create_PropertyDeclaredInBaseClass ()
    {
      var componentPropertyRule = AddingComponentPropertyRule.Create (_lastNameExpression, typeof (RemovingComponentPropertyRuleTest));

      Assert.That (
          MemberInfoEqualityComparer<MemberInfo>.Instance.Equals (componentPropertyRule.Member, typeof (Customer).GetMember ("LastName")[0]),
          Is.True);
      Assert.That (componentPropertyRule.PropertyName, Is.EqualTo ("LastName"));
      Assert.That (componentPropertyRule.Expression, Is.SameAs (_lastNameExpression));
    }

    [Test]
    public void RegisterValidator ()
    {
      _removingComponentPropertyRule.RegisterValidator (typeof (StubPropertyValidator));
      _removingComponentPropertyRule.RegisterValidator (typeof (NotEmptyValidator), typeof (CustomerValidationCollector1));

      Assert.That (_removingComponentPropertyRule.Validators.Count(), Is.EqualTo (2));

      Assert.That (_removingComponentPropertyRule.Validators.ElementAt (0).ValidatorType, Is.EqualTo (typeof (StubPropertyValidator)));
      Assert.That (_removingComponentPropertyRule.Validators.ElementAt (0).CollectorTypeToRemoveFrom, Is.Null);

      Assert.That (_removingComponentPropertyRule.Validators.ElementAt (1).ValidatorType, Is.EqualTo (typeof (NotEmptyValidator)));
      Assert.That (
          _removingComponentPropertyRule.Validators.ElementAt (1).CollectorTypeToRemoveFrom,
          Is.EqualTo (typeof (CustomerValidationCollector1)));
    }

    [Test]
    public void To_String ()
    {
      var result = _removingComponentPropertyRule.ToString();

      Assert.That (
          result,
          Is.EqualTo ("RemovingComponentPropertyRule: Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }
  }
}