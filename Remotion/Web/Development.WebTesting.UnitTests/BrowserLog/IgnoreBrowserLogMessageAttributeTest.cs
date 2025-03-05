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
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.BrowserLog;

namespace Remotion.Web.Development.WebTesting.UnitTests.BrowserLog;

[TestFixture]
public class IgnoreBrowserLogMessageAttributeTest
{
  [Test]
  public void ApplyValue_AddsRegexMatchDelegateToEmptyList ()
  {
    var attribute = new IgnoreBrowserLogMessageAttribute(@"My number is \d+");
    var properties = new Dictionary<string, object>();

    attribute.ApplyValue(properties);
    Assert.That(properties.ContainsKey(IgnoreBrowserLogMessageAttribute.PropertyKey), Is.True);

    var regexCollection = properties[IgnoreBrowserLogMessageAttribute.PropertyKey] as IReadOnlyCollection<Regex>;
    Assert.That(regexCollection, Is.Not.Null);
    Assert.That(regexCollection.Count, Is.EqualTo(1));

    var regex = regexCollection.Single();
    Assert.That(regex.IsMatch("My number is 42"), Is.True);
  }

  [Test]
  public void ApplyValue_AppendsRegexMatchDelegateToListWithElements ()
  {
    var attribute = new IgnoreBrowserLogMessageAttribute(@"My number is \d+");
    var properties = new Dictionary<string, object>
                     {
                         {
                             IgnoreBrowserLogMessageAttribute.PropertyKey, new List<Regex> { new("never"), new("matches") }
                         }
                     };

    attribute.ApplyValue(properties);
    Assert.That(properties.ContainsKey(IgnoreBrowserLogMessageAttribute.PropertyKey), Is.True);

    var regexCollection = properties[IgnoreBrowserLogMessageAttribute.PropertyKey] as IReadOnlyCollection<Regex>;
    Assert.That(regexCollection, Is.Not.Null);
    Assert.That(regexCollection.Count, Is.EqualTo(3));

    var lastRegex = regexCollection.Last();
    Assert.That(lastRegex.IsMatch("My number is 42"), Is.True);
  }
}
