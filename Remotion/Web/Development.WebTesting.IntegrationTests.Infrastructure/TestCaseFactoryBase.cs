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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using JetBrains.Annotations;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// Represents a class which contains test that are executed by using the <see cref="TestCaseSourceAttribute"/>.
  /// </summary>
  public abstract class TestCaseFactoryBase: IEnumerable
  {
    /// <summary>
    /// Marks a methods as a test method.
    /// </summary>
    /// <remarks>
    /// Every test in the sub class of <see cref="TestCaseFactoryBase"/> must be marked 
    /// with <see cref="TestMethodAttribute"/> otherwise it is not included in the test set.
    /// Test methods can be marked with NUnit attributes <see cref="CategoryAttribute"/>, <see cref="IgnoreAttribute"/>
    /// or <see cref="ExplicitAttribute"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    protected class TestMethodAttribute : Attribute
    {
      public TestMethodAttribute ()
      {
        Name = null;
      }

      /// <summary>
      /// The name of the test, or <see langword="null"/> if the method name should be used as test name.
      /// </summary>
      [CanBeNull]
      public string Name { get; set; }

      /// <summary>
      /// Provides a way to change the <see cref="TestCaseData"/> that is generated per test.
      /// </summary>
      public virtual void Apply (TestCaseData data)
      {
      }
    }

    private WebTestHelper _helper;
    private PageObject _home;

    protected TestCaseFactoryBase ()
    {
    }

    /// <summary>
    /// The <see cref="WebTestHelper"/> for the current test session.
    /// </summary>
    protected WebTestHelper Helper
    {
      get { return _helper; }
    }

    /// <summary>
    /// The <see cref="IControlHost"/> created for this page.
    /// </summary>
    protected PageObject Home
    {
      get { return _home; }
    }

    /// <summary>
    /// Returns the test prefix which will be prepended to each test name, or <see langword="null"/> if no prefix should be used.
    /// </summary>
    [CanBeNull]
    protected abstract string TestPrefix { get; }

    /// <summary>
    /// Returns all tests in this type.
    /// </summary>
    /// <remarks>
    /// Invoked via <see cref="TestCaseSourceAttribute"/>.
    /// </remarks>
    [UsedImplicitly]
    protected virtual IEnumerable<TestCaseData> GetTests ()
    {
      return GetTests<TestMethodAttribute>(CreateTestCaseData);
    }

    protected IEnumerable<TestCaseData> GetTests<T> (Func<T, MethodInfo, TestCaseData> testCaseDataFactory)
        where T : TestMethodAttribute
    {
      foreach (var method in GetType().GetMethods())
      {
        var testCaseAttribute = (TestMethodAttribute)Attribute.GetCustomAttribute(method, typeof(TestMethodAttribute), true);
        if (testCaseAttribute != null)
        {
          if (method.ReturnType != typeof(void) || method.GetParameters().Length != 0)
            throw new NotSupportedException("Only methods with a void() signature are allowed as test methods.");

          if (testCaseAttribute.GetType() != typeof(T))
          {
            throw new InvalidOperationException(
                string.Format(
                    "Method '{0}.{1}'s TestMethodAttribute must be a '{2}' but is a '{3}'.",
                    GetType().Name,
                    method.Name,
                    typeof(T).Name,
                    testCaseAttribute.GetType().Name));
          }

          var testCaseData = testCaseDataFactory((T)testCaseAttribute, method);
          if (testCaseData != null)
          {
            // Handle TestName
            if (testCaseData.TestName == null)
            {
              if (testCaseAttribute.Name != null)
                testCaseData.SetName(testCaseAttribute.Name);
              else
              {
                var prefix = TestPrefix;
                if (string.IsNullOrWhiteSpace(prefix))
                  testCaseData.SetName(method.Name);
                else
                  testCaseData.SetName(string.Format("{0}_{1}", prefix, method.Name));
              }
            }

            // Handle CategoryAttribute
            var categoryAttribute = (CategoryAttribute)Attribute.GetCustomAttribute(method, typeof(CategoryAttribute));
            if (categoryAttribute != null)
              testCaseData.SetCategory(categoryAttribute.Name);

            // Handle IgnoreAttribute
            var ignoreAttribute = (IgnoreAttribute)Attribute.GetCustomAttribute(method, typeof(IgnoreAttribute));
            if (ignoreAttribute != null)
            {
              var dummyTest = new TestSuite("");
              ignoreAttribute.ApplyToTest(dummyTest);
              testCaseData.Ignore((string)dummyTest.Properties.Get(PropertyNames.SkipReason));
            }

            // Handle ExplicitAttribute
            var explicitAttribute = (ExplicitAttribute)Attribute.GetCustomAttribute(method, typeof(ExplicitAttribute));
            if (explicitAttribute != null)
            {
              var dummyTest = new TestSuite("");
              explicitAttribute.ApplyToTest(dummyTest);
              testCaseData.Explicit((string)dummyTest.Properties.Get(PropertyNames.SkipReason));
            }

            testCaseAttribute.Apply(testCaseData);

            yield return testCaseData;
          }
        }
      }
    }

    /// <summary>
    /// Starts the specified method using the 
    /// </summary>
    protected void PrepareTest ([NotNull] TestMethodAttribute attribute, [NotNull] WebTestHelper helper, [NotNull] string url)
    {
      ArgumentUtility.CheckNotNull("attribute", attribute);
      ArgumentUtility.CheckNotNull("helper", helper);
      ArgumentUtility.CheckNotNull("url", url);

      helper.MainBrowserSession.Window.Visit(url);
      helper.AcceptPossibleModalDialog();

      _helper = helper;
      _home = helper.CreateInitialPageObject<HtmlPageObject>(helper.MainBrowserSession);
    }

    /// <summary>
    /// Runs the specified test method.
    /// </summary>
    protected virtual void RunTest ([NotNull] MethodInfo method)
    {
      ArgumentUtility.CheckNotNull("method", method);

      try
      {
        method.Invoke(this, new object[0]);
      }
      catch (TargetInvocationException ex)
      {
        if (ex.InnerException != null)
          ExceptionDispatchInfo.Capture(ex.InnerException).Throw();

        throw;
      }
    }

    private TestCaseData CreateTestCaseData ([NotNull] TestMethodAttribute attribute, [NotNull] MethodInfo method)
    {
      ArgumentUtility.CheckNotNull("attribute", attribute);
      ArgumentUtility.CheckNotNull("method", method);

      return new TestCaseData(
          (TestSetupAction)((helper, url) =>
          {
            PrepareTest(attribute, helper, url);
            RunTest(method);
          }));
    }

    public IEnumerator GetEnumerator ()
    {
      return GetTests().GetEnumerator();
    }
  }
}
