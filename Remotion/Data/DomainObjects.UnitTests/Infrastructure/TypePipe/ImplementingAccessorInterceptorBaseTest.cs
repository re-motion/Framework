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
using System.Collections.Generic;
using System.Reflection;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Development.UnitTesting.Expressions;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe
{
  [TestFixture]
  public class ImplementingAccessorInterceptorBaseTest
  {
    private MethodInfo _interceptedMethod;
    private string _propertyName;
    private Type _propertyType;

    private Mock<ImplementingAccessorInterceptorBase> _interceptorPartialMock;

    private MutableType _proxyType;

    [SetUp]
    public void SetUp ()
    {
      _interceptedMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod((object o) => o.Equals(null));
      _propertyName = "abc";
      _propertyType = typeof(int);

      _interceptorPartialMock = new Mock<ImplementingAccessorInterceptorBase>(_interceptedMethod, _propertyName, _propertyType) { CallBase = true };

      _proxyType = MutableTypeObjectMother.Create(typeof(DomainObject));
    }

    [Test]
    public void CreateBody_BuildsCorrectBody_UsingAbstractTemplateMembers ()
    {
      var accessorImplementationMethod =
          NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition((PropertyAccessor a) => a.SetValue<object>(null));
      var arguments = new Expression[] { Expression.Parameter(typeof(int), "param") };
      var expectedCtx = new MethodBodyModificationContext(_proxyType, false, new ParameterExpression[0], Type.EmptyTypes, typeof(int), null, null);
      _interceptorPartialMock
          .Protected()
          .SetupGet<MethodInfo>("AccessorImplementationMethod")
          .Returns(accessorImplementationMethod);
      _interceptorPartialMock
          .Protected()
          .Setup<IEnumerable<Expression>>("GetArguments", true, ItExpr.IsAny<MethodBodyModificationContext>())
          .Callback((MethodBodyModificationContext ctx) => Assert.That(ctx, Is.SameAs(expectedCtx)))
          .Returns(arguments);

      var result = (Expression)PrivateInvoke.InvokeNonPublicMethod(_interceptorPartialMock.Object, "CreateBody", expectedCtx);

      var expectedbody =
          Expression.Call(
              Expression.Call(
                  Expression.Property(
                      new ThisExpression(_proxyType),
                      typeof(DomainObject).GetProperty("Properties", BindingFlags.Instance | BindingFlags.NonPublic)),
                  "get_Item",
                  null,
                  Expression.Constant("abc")),
              "SetValue",
              new[] { typeof(int) },
              arguments);
      ExpressionTreeComparer.CheckAreEqualTrees(expectedbody, result);
    }
  }
}
