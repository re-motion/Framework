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
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting.BrowserLog;

namespace Remotion.Web.Development.WebTesting.UnitTests.BrowserLog;

[TestFixture]
public class BrowserLogMinimumLevelAttributeTest
{
  [Test]
  public void ApplyValue_SetsNewValue ()
  {
    var attribute = new BrowserLogMinimumLevelAttribute(LogLevel.Info);
    var properties = new Dictionary<string, object>();

    attribute.ApplyValue(properties);
    Assert.That(properties.ContainsKey(BrowserLogMinimumLevelAttribute.PropertyKey), Is.True);
    Assert.That(properties[BrowserLogMinimumLevelAttribute.PropertyKey], Is.EqualTo(LogLevel.Info));
  }

  [Test]
  public void ApplyValue_OverwritesExistingValue ()
  {
    var attribute = new BrowserLogMinimumLevelAttribute(LogLevel.Warning);
    var properties = new Dictionary<string, object>()
                     {
                         { BrowserLogMinimumLevelAttribute.PropertyKey, LogLevel.Info }
                     };

    attribute.ApplyValue(properties);
    Assert.That(properties.ContainsKey(BrowserLogMinimumLevelAttribute.PropertyKey), Is.True);
    Assert.That(properties[BrowserLogMinimumLevelAttribute.PropertyKey], Is.EqualTo(LogLevel.Warning));
  }
}
