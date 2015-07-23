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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Development.UnitTesting.Expressions;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe
{
  [TestFixture]
  public class WrappingAccessorInterceptorTest
  {
    private MethodInfo _interceptedMethod;
    private string _propertyName;

    private WrappingAccessorInterceptor _interceptor;

    private MutableType _proxyType;

    [SetUp]
    public void SetUp ()
    {
      _interceptedMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((object o) => o.Equals (null));
      _propertyName = "abc";

      _interceptor = new WrappingAccessorInterceptor (_interceptedMethod, _propertyName);

      _proxyType = MutableTypeObjectMother.Create();
    }

    [Test]
    public void Intercept_AddsOverride_AndWrapsBaseCallInTryFinally ()
    {
      _interceptor.Intercept (_proxyType);

      Assert.That (_proxyType.AddedMethods, Has.Count.EqualTo (1));
      var method = _proxyType.AddedMethods.Single();

      var expectedBody =
          Expression.Block (
              Expression.Call (typeof (CurrentPropertyManager), "PreparePropertyAccess", null, Expression.Constant (_propertyName)),
              Expression.TryFinally (
                  Expression.Call (
                      new ThisExpression (_proxyType),
                      NonVirtualCallMethodInfoAdapter.Adapt (_interceptedMethod),
                      Expression.Parameter (typeof (object), "obj")),
                  Expression.Call (typeof (CurrentPropertyManager), "PropertyAccessFinished", null)));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, method.Body);
    }
  }
}