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
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.RoleCollectors
{
  [TestFixture]
  public class RemovingPropertyValidationRuleCollectorTest
  {
    private Expression<Func<Customer, string>> _userNameExpression;
    private Expression<Func<Customer, string>> _lastNameExpression;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollector;
    private IPropertyInformation _property;

    [SetUp]
    public void SetUp ()
    {
      _property = PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("UserName"));

      _userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.UserName);
      _lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName);

      _removingPropertyValidationRuleCollector = RemovingPropertyValidationRuleCollector.Create (_userNameExpression, typeof (RemovingPropertyValidationRuleCollectorTest));
    }

    [Test]
    public void Initialization_PropertyDeclaredInSameClass ()
    {
      Assert.That (_removingPropertyValidationRuleCollector.Property.Equals(_property), Is.True);
      Assert.That (_removingPropertyValidationRuleCollector.Property, Is.EqualTo (_property));
      Assert.That (_removingPropertyValidationRuleCollector.CollectorType, Is.EqualTo (typeof (RemovingPropertyValidationRuleCollectorTest)));
      Assert.That (_removingPropertyValidationRuleCollector.Validators.Any(), Is.False);
    }

    [Test]
    public void Create_MemberInfoIsNoPropertyInfo_ExceptionIsThrown ()
    {
      var dummyExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.Dummy ());

      Assert.That (
          () => RemovingPropertyValidationRuleCollector.Create (dummyExpression, typeof (CustomerValidationRuleCollector1)),
          Throws.ArgumentException.With.Message.EqualTo ("Must be a MemberExpression.\r\nParameter name: expression"));
    }

    [Test]
    public void Create_PropertyDeclaredInBaseClass ()
    {
      var componentPropertyRule = AddingPropertyValidationRuleCollector.Create (_lastNameExpression, typeof (CustomerValidationRuleCollector1));
      var propertyInfo = ((PropertyInfoAdapter) componentPropertyRule.Property).PropertyInfo;

      //TODO-5906 simplify assertion with PropertyInfoAdapter compare
      Assert.That (
          MemberInfoEqualityComparer<MemberInfo>.Instance.Equals (propertyInfo, typeof (Customer).GetMember ("LastName")[0]),
          Is.True);
    }

    [Test]
    public void RegisterValidator ()
    {
      _removingPropertyValidationRuleCollector.RegisterValidator (typeof (StubPropertyValidator));
      _removingPropertyValidationRuleCollector.RegisterValidator (typeof (NotEmptyValidator), typeof (CustomerValidationRuleCollector1));

      Assert.That (_removingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo (2));

      Assert.That (_removingPropertyValidationRuleCollector.Validators.ElementAt (0).ValidatorType, Is.EqualTo (typeof (StubPropertyValidator)));
      Assert.That (_removingPropertyValidationRuleCollector.Validators.ElementAt (0).CollectorTypeToRemoveFrom, Is.Null);

      Assert.That (_removingPropertyValidationRuleCollector.Validators.ElementAt (1).ValidatorType, Is.EqualTo (typeof (NotEmptyValidator)));
      Assert.That (
          _removingPropertyValidationRuleCollector.Validators.ElementAt (1).CollectorTypeToRemoveFrom,
          Is.EqualTo (typeof (CustomerValidationRuleCollector1)));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void RegisterValidator_ValidatorTypeDoesNotImplementIPropertyValidator_ThrowsArgumentException ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void RegisterValidator_CollectorTypeDoesNotImplementIValidationRuleCollector_ThrowsArgumentException ()
    {
    }

    [Test]
    public void To_String ()
    {
      var result = _removingPropertyValidationRuleCollector.ToString();

      Assert.That (
          result,
          Is.EqualTo ("RemovingPropertyValidationRuleCollector: Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }
  }
}