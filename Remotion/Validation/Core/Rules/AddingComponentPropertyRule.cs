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
using System.Linq.Expressions;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Validation.Rules
{
  public static class AddingComponentPropertyRule
  {
    public static AddingComponentPropertyRule<TValidatedType, TProperty> Create<TValidatedType, TProperty> (
        Expression<Func<TValidatedType, TProperty>> expression,
        Type collectorType)
    {
      var propertyInfo = MemberInfoFromExpressionUtility.GetProperty (expression);

      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("collectorType", collectorType, typeof (IValidationRuleCollector));

      // TODO RM-5906: Replace with IPropertyInformation.GetGetMethod().GetFastInvoker.
      // TODO RM-5906: Add cache, try to unify with ValidationAttributesBasedPropertyRuleReflector and DomainObjectAttributesBasedValidationPropertyRuleReflector

      var parameterExpression = Expression.Parameter (typeof (object), "t");

      // object o => (object) (TheType o).TheProperty
      var propertyExpression = Expression.Lambda<Func<object, object>> (
          Expression.Convert (
              Expression.MakeMemberAccess (
                  Expression.Convert (parameterExpression, typeof (TValidatedType)),
                  propertyInfo),
              typeof (object)),
          parameterExpression);

      return new AddingComponentPropertyRule<TValidatedType, TProperty> (
          PropertyInfoAdapter.Create (propertyInfo),
          propertyExpression.Compile(),
          collectorType);
    }
  }
}