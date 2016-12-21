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
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Rules
{
  [TestFixture]
  public class AddingComponentPropertyMetaValidationRuleTest
  {
    private IPropertyInformation _property;
    private Expression<Func<Customer, string>> _userNameExpression;
    private AddingComponentPropertyMetaValidationRule _rule;

    [SetUp]
    public void SetUp ()
    {
      _property = PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("UserName"));
      _userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.UserName);
      _rule = AddingComponentPropertyMetaValidationRule.Create (_userNameExpression, typeof (RemovingComponentPropertyRuleTest));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_rule.Property.Equals(_property), Is.True);
      Assert.That (_rule.Property, Is.EqualTo (_property));
      Assert.That (_rule.CollectorType, Is.EqualTo (typeof (RemovingComponentPropertyRuleTest)));
      Assert.That (_rule.MetaValidationRules.Any(), Is.False);
    }

    [Test]
    public void Create_MemberInfoIsNoPropertyInfo_ExceptionIsThrown ()
    {
      var dummyExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.Dummy ());

      Assert.Throws<InvalidOperationException> (
          () => AddingComponentPropertyMetaValidationRule.Create (dummyExpression, typeof (CustomerValidationCollector1)),
          "An 'AddingComponentPropertyMetaValidationRule' can only created for property members.");
    }

    [Test]
    public void RegisterMetaValidationRule ()
    {
      var metaValidationRuleStub1 = MockRepository.GenerateStub<IMetaValidationRule>();
      var metaValidationRuleStub2 = MockRepository.GenerateStub<IMetaValidationRule>();
      Assert.That (_rule.MetaValidationRules.Count(), Is.EqualTo (0));

      _rule.RegisterMetaValidationRule (metaValidationRuleStub1);
      _rule.RegisterMetaValidationRule (metaValidationRuleStub2);

      Assert.That (_rule.MetaValidationRules.Count(), Is.EqualTo (2));
      Assert.That (
          _rule.MetaValidationRules,
          Is.EquivalentTo (new[] { metaValidationRuleStub1, metaValidationRuleStub2 }));
    }

    [Test]
    public void To_String ()
    {
      var result = _rule.ToString();

      Assert.That (
          result,
          Is.EqualTo (
              "AddingComponentPropertyMetaValidationRule: Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }
  }
}