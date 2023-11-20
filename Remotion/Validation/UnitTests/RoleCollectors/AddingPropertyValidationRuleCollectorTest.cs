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
using System;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;

namespace Remotion.Validation.UnitTests.RoleCollectors
{
  [TestFixture]
  public class AddingPropertyValidationRuleCollectorTest
  {
    private Expression<Func<Customer, string>> _userNameExpression;
    private Expression<Func<Customer, string>> _lastNameExpression;
    private AddingPropertyValidationRuleCollector<Customer, string> _addingPropertyValidationRuleCollector;

    [SetUp]
    public void SetUp ()
    {
      _userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.UserName);
      _lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.LastName);

      _addingPropertyValidationRuleCollector = AddingPropertyValidationRuleCollector.Create(_userNameExpression, typeof(CustomerValidationRuleCollector1));
    }

    [Test]
    public void Create_CollectorTypeDoesNotImplementIValidationRuleCollector_ThrowsArgumentException ()
    {
      Assert.That(
          () => AddingPropertyValidationRuleCollector.Create(_userNameExpression, typeof(Customer)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'collectorType' is a 'Remotion.Validation.UnitTests.TestDomain.Customer', "
                  + "which cannot be assigned to type 'Remotion.Validation.IValidationRuleCollector'.",
                  "collectorType"));
    }

    [Test]
    public void Create_MemberInfoIsNoPropertyInfo_ExceptionIsThrown ()
    {
      var dummyExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.Dummy());

      Assert.That(
          () => AddingPropertyValidationRuleCollector.Create(dummyExpression, typeof(CustomerValidationRuleCollector1)),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("Must be a MemberExpression.", "expression"));
    }

    [Test]
    public void Create_PropertyDeclaredInBaseClass ()
    {
      var componentPropertyRule = AddingPropertyValidationRuleCollector.Create(_lastNameExpression, typeof(CustomerValidationRuleCollector1));
      var propertyInfo = ((PropertyInfoAdapter)componentPropertyRule.Property).PropertyInfo;

      //TODO-5906 simplify assertion with PropertyInfoAdapter compare
      Assert.That(
          MemberInfoEqualityComparer<MemberInfo>.Instance.Equals(propertyInfo, typeof(Customer).GetMember("LastName")[0]),
          Is.True);
      Assert.That(propertyInfo.DeclaringType, Is.EqualTo(typeof(Person)));
    }

    [Test]
    public void Create_BuildsFuncForPropertyAccess ()
    {
      var type = new Customer() { UserName = "Test" };

      Assert.That(_addingPropertyValidationRuleCollector.PropertyFunc(type), Is.EqualTo("Test"));
    }
  }
}
