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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class CoypuElementScopeFindExtensionsTest : IntegrationTest
  {
    [Test]
    public void FindTagWithAttribute_ShouldFindTagWithAttributeValue ()
    {
      var home = Start();
      var tagSelector = "span";
      var attributeName = "test-attribute";
      var attributeValue = "WithoutSingleQuote";

      var foundElement = home.Scope.FindTagWithAttribute(tagSelector, attributeName, attributeValue);

      Assert.That(foundElement.Exists());
    }
    
    [Test]
    public void FindTagWithAttribute_ShouldFindTagWithAttributeValue_ContainingSingleQuote ()
    {
      var home = Start();
      var tagSelector = "span";
      var attributeName = "test-attribute";
      var attributeValue = "With'SingleQuote";

      var foundElement = home.Scope.FindTagWithAttribute(tagSelector, attributeName, attributeValue);

      Assert.That(foundElement.Exists());
    }
    
    [Test]
    public void FindTagWithAttributeUsingOperator_ShouldFindTagWithAttributeValue ()
    {
      var home = Start();
      var tagSelector = "span";
      var op = CssComparisonOperator.Equals;
      var attributeName = "test-attribute";
      var attributeValue = "WithoutSingleQuote";

      var foundElement = home.Scope.FindTagWithAttributeUsingOperator(tagSelector, op, attributeName, attributeValue);

      Assert.That(foundElement.Exists());
    }
    
    [Test]
    public void FindTagWithAttributeUsingOperator_ShouldFindTagWithAttributeValue_ContainingSingleQuote ()
    {
      var home = Start();
      var tagSelector = "span";
      var op = CssComparisonOperator.Equals;
      var attributeName = "test-attribute";
      var attributeValue = "With'SingleQuote";

      var foundElement = home.Scope.FindTagWithAttributeUsingOperator(tagSelector, op, attributeName, attributeValue);

      Assert.That(foundElement.Exists());
    }
    
    [Test]
    public void FindTagWithAttributes_ShouldFindTagWithAttributeValue ()
    {
      var home = Start();
      var tagSelector = "span";
      var attributesDictionary = new Dictionary<string, string>();
      attributesDictionary.Add("test-attribute", "WithoutSingleQuote");

      var foundElement = home.Scope.FindTagWithAttributes(tagSelector, attributesDictionary);

      Assert.That(foundElement.Exists());
    }
    
    [Test]
    public void FindTagWithAttributes_ShouldFindTagWithAttributeValue_ContainingSingleQuote ()
    {
      var home = Start();
      var tagSelector = "span";
      var attributesDictionary = new Dictionary<string, string>();
      attributesDictionary.Add("test-attribute", "With'SingleQuote");

      var foundElement = home.Scope.FindTagWithAttributes(tagSelector, attributesDictionary);

      Assert.That(foundElement.Exists());
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("CoypuElementScopeFindExtensionsTest.wxe");
    }
  }
}
