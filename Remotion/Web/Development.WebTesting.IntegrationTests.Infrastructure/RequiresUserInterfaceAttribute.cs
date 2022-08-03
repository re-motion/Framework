// The MIT License (MIT)
// 
// Copyright (c) RUBICON IT GmbH, www.rubicon.eu
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions: 
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software. 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.
using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RequiresUserInterfaceAttribute : NUnitAttribute, IApplyToTest
  {
    public const string CategoryName = "RequiresUserInterface";
    public const string PropertyName = "RequiresUserInterface";

    public static bool GetPropertyValue (TestContext testContext)
    {
      return testContext.Test.Properties.Get(PropertyName) as bool? ?? false;
    }

    public RequiresUserInterfaceAttribute ()
    {
    }

    public void ApplyToTest (Test test)
    {
      test.Properties.Add(PropertyNames.Category, CategoryName);
      test.Properties.Set(PropertyName, true);
    }
  }
}
