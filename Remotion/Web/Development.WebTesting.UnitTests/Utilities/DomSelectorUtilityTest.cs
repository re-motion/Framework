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
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.UnitTests
{
  [TestFixture]
  public class DomSelectorUtilityTest
  {
    [Test]
    public void CreateMatchValueForXPath_ShouldReturnCorrectlyEnclosedParameter ()
    {
      var stringParameter = "stringParameter";

      var actualValue = DomSelectorUtility.CreateMatchValueForXPath(stringParameter);

      Assert.That(actualValue, Is.EqualTo("'stringParameter'"));
    }

    [Test]
    public void CreateMatchValueForXPath_WithDoubleQuote_ShouldReturnCorrectlyEnclosedParameter ()
    {
      var stringParameter = "string\"Parameter";

      var actualValue = DomSelectorUtility.CreateMatchValueForXPath(stringParameter);

      Assert.That(actualValue, Is.EqualTo("'string\"Parameter'"));
    }

    [Test]
    public void CreateMatchValueForXPath_WithSingleQuote_ShouldReturnCorrectlyEnclosedParameter ()
    {
      var stringParameter = "string'Parameter";

      var actualValue = DomSelectorUtility.CreateMatchValueForXPath(stringParameter);

      Assert.That(actualValue, Is.EqualTo("\"string'Parameter\""));
    }

    [Test]
    public void CreateMatchValueForXPath_WithSingleQuoteAndDoubleQuote_ShouldReturnCorrectlyEnclosedParameter ()
    {
      var stringParameter = "string'Para\"meter";

      var actualValue = DomSelectorUtility.CreateMatchValueForXPath(stringParameter);

      Assert.That(actualValue, Is.EqualTo("concat('string',\"'\",'Para\"meter')"));
    }

    [Test]
    public void CreateMatchValueForCssSelector_ShouldCorrectlyEncloseParameter ()
    {
      var stringParameter = "stringParameter";

      var actualValue = DomSelectorUtility.CreateMatchValueForCssSelector(stringParameter);

      Assert.That(actualValue, Is.EqualTo("'stringParameter'"));
    }


    [Test]
    public void CreateMatchValueForCssSelector_WithSingleQuote_ShouldReturnCorrectlyEnclosedAndEscapedParameter ()
    {
      var stringParameter = "string'Parameter";

      var actualValue = DomSelectorUtility.CreateMatchValueForCssSelector(stringParameter);

      Assert.That(actualValue, Is.EqualTo("'string\\'Parameter'"));
    }
  }
}