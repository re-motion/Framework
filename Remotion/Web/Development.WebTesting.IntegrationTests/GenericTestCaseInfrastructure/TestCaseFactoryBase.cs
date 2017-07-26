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
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure
{
  /// <summary>
  /// Represents a class which contains test that are executed by using the <see cref="TestCaseSourceAttribute"/>.
  /// </summary>
  public abstract class TestCaseFactoryBase
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
    [AttributeUsage (AttributeTargets.Method)]
    protected class TestMethodAttribute : Attribute
    {
      public TestMethodAttribute ()
      {
        Name = null;
        PageType = GenericPageTypes.All;
      }

      /// <summary>
      /// The name of the test, or <see langword="null"/> if the method name should be used as test name.
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// Defines which page sections will be shown when using the generic page.
      /// </summary>
      public GenericPageTypes PageType { get; set; }
    }

    public delegate void TestSetupAction<in TControlSelector, TControl> (
        WebTestHelper webTestHelper,
        Func<IControlHost, IFluentControlSelector<TControlSelector, TControl>> controlSelectorFactory,
        string controlName)
        where TControlSelector : IControlSelector
        where TControl : ControlObject;

    protected TestCaseFactoryBase ()
    {
    }

    /// <summary>
    /// Returns the test prefix which will be prepended to each test, or <see langword="null"/> if no prefix should be used.
    /// </summary>
    protected abstract string GetTestPrefix ();

    /// <summary>
    /// Returns a setup <see cref="TestCaseData"/> created from the specified <see cref="TestCaseOptions"/>.
    /// </summary>
    protected abstract TestCaseData SetupTestCaseData (TestCaseOptions options);

    /// <summary>
    /// Returns all marked tests in this type.
    /// </summary>
    protected IEnumerable<TestCaseData> GetTests ()
    {
      foreach (var method in GetType().GetMethods())
      {
        var testCaseAttribute = (TestMethodAttribute) Attribute.GetCustomAttribute (method, typeof (TestMethodAttribute));
        if (testCaseAttribute != null)
        {
          if (method.ReturnType != typeof (void) || method.GetParameters().Length != 0)
            throw new NotSupportedException ("Only methods with a void() signature are allowed as test methods.");

          var options = new TestCaseOptions { TargetMethod = method, PageType = testCaseAttribute.PageType };

          var testCaseData = SetupTestCaseData (options);
          if (testCaseData != null)
          {
            // Handle TestName
            if (testCaseData.TestName == null)
            {
              if (testCaseAttribute.Name != null)
                testCaseData.SetName (testCaseAttribute.Name);
              else
              {
                var prefix = GetTestPrefix();
                if (string.IsNullOrWhiteSpace (prefix))
                  testCaseData.SetName (method.Name);
                else
                  testCaseData.SetName (string.Format ("{0}_{1}", prefix, method.Name));
              }
            }

            // Handle CategoryAttribute
            var categoryAttribute = (CategoryAttribute) Attribute.GetCustomAttribute (method, typeof (CategoryAttribute));
            if (categoryAttribute != null)
              testCaseData.SetCategory (categoryAttribute.Name);

            // Handle IgnoreAttribute
            var ignoreAttribute = (IgnoreAttribute) Attribute.GetCustomAttribute (method, typeof (IgnoreAttribute));
            if (ignoreAttribute != null)
              testCaseData.Ignore (ignoreAttribute.Reason);

            // Handle ExplicitAttribute
            var explicitAttribute = (ExplicitAttribute) Attribute.GetCustomAttribute (method, typeof (ExplicitAttribute));
            if (explicitAttribute != null)
              testCaseData.MakeExplicit (explicitAttribute.Reason);

            // Add generic test case category
            testCaseData.SetCategory (TestConstants.Category);

            yield return testCaseData;
          }
        }
      }
    }
  }
}