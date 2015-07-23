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
using NUnit.Framework;
using Remotion.UnitTests.ServiceLocation.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  [Explicit ("Performance tests")]
  public class ActivationPerformanceTest
  {
    [Test]
    public void New ()
    {
      var args = new object[]
                 {
                     new TestConstructorInjectionWithOneParameter (null),
                     new TestConstructorInjectionWithOneParameter (null),
                     new TestConcreteImplementationAttributeType()
                 };

      Func<object> factory = () => new TestConstructorInjectionWithThreeParameters (
                                       (ITestConstructorInjectionWithOneParameter) args[0],
                                       (ITestConstructorInjectionWithOneParameter) args[1],
                                       (ITestSingletonConcreteImplementationAttributeType) args[2]);

      int acc = 0;
      using (StopwatchScope.CreateScope ("New: elapsed: {elapsed}"))
      {
        for (int i = 0; i < 1000000; ++i)
        {
          var instance = factory();
          acc ^= instance.GetHashCode();
        }
      }
      Console.WriteLine (acc);
    }

    [Test]
    public void LinqExpression ()
    {
      var args = new object[]
                 {
                     new TestConstructorInjectionWithOneParameter (null),
                     new TestConstructorInjectionWithOneParameter (null),
                     new TestConcreteImplementationAttributeType()
                 };

      var ctorInfo = typeof (TestConstructorInjectionWithThreeParameters).GetConstructors ()[0];

      var ctorArgExpressions = ctorInfo.GetParameters().Select (p => (Expression) Expression.Constant (args[p.Position]));
      Expression<Func<object>> factoryExpression = Expression.Lambda<Func<object>> (Expression.New (ctorInfo, ctorArgExpressions));

      Func<object> factory = factoryExpression.Compile ();

      int acc = 0;
      using (StopwatchScope.CreateScope ("Built factory: elapsed: {elapsed}"))
      {
        for (int i = 0; i < 1000000; ++i)
        {
          var instance = factory ();
          acc ^= instance.GetHashCode ();
        }
      }
      Console.WriteLine (acc);
    }

    [Test]
    public void ConstructorInfoInvoke ()
    {
      var ctorInfo = typeof (TestConstructorInjectionWithThreeParameters).GetConstructors()[0];
      var args = new object[]
                 {
                     new TestConstructorInjectionWithOneParameter (null),
                     new TestConstructorInjectionWithOneParameter (null),
                     new TestConcreteImplementationAttributeType()
                 };

      Func<object> factory = () => ctorInfo.Invoke (args);

      int acc = 0;
      using (StopwatchScope.CreateScope ("CtorInfo: elapsed: {elapsed}"))
      {
        for (int i = 0; i < 1000000; ++i)
        {
          var instance = factory();
          acc ^= instance.GetHashCode();
        }
      }
      Console.WriteLine (acc);
    }

    [Test]
    public void ActivatorCreateInstance ()
    {
      var args = new object[]
                 {
                     new TestConstructorInjectionWithOneParameter (null),
                     new TestConstructorInjectionWithOneParameter (null),
                     new TestConcreteImplementationAttributeType()
                 };

      Func<object> factory = () => Activator.CreateInstance (typeof (TestConstructorInjectionWithThreeParameters), args);

      int acc = 0;
      using (StopwatchScope.CreateScope ("Activator: elapsed: {elapsed}"))
      {
        for (int i = 0; i < 1000000; ++i)
        {
          var instance = factory();
          acc ^= instance.GetHashCode();
        }
      }
      Console.WriteLine (acc);
    }
  }
}