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
using Moq;
using NUnit.Framework;

namespace Remotion.Web.Development.WebTesting.UnitTests;

[TestFixture]
public class TestContextExtensionTest
{
  [Test]
  [TestCase(42)]
  [TestCase("stringValue")]
  [TestCase(typeof(ITestContext))]
  public void GetValueOrDefault_KeyNotFound_ReturnsDefault (object defaultValue)
  {
    var testContextMock = new Mock<ITestContext>();
    testContextMock.Setup(_ => _.Properties).Returns(new Dictionary<string, object>());

    var result = testContextMock.Object.GetValueOrDefault("Key", defaultValue);
    Assert.That(result, Is.EqualTo(defaultValue));
  }

  [Test]
  [TestCase(17, 42)]
  [TestCase("stringValue", "defaultValue")]
  [TestCase(typeof(TestContextExtensions), typeof(ITestContext))]
  public void GetValueOrDefault_PropertyExistsInContext_ReturnsPropertyValue (object propertyValue, object defaultValue)
  {
    var testContextMock = new Mock<ITestContext>();
    testContextMock.Setup(_ => _.Properties).Returns(new Dictionary<string, object> { { "Key", propertyValue } });

    var result = testContextMock.Object.GetValueOrDefault("Key", defaultValue);
    Assert.That(result, Is.EqualTo(propertyValue));
  }

  [Test]
  public void GetValueOrDefault_PropertyValueIsNull_ThrowsException ()
  {
    var testContextMock = new Mock<ITestContext>();
    testContextMock.Setup(_ => _.Properties).Returns(new Dictionary<string, object> { { "Key", null } });

    Assert.That(
        () => testContextMock.Object.GetValueOrDefault("Key", "defaultValue"),
        Throws.InvalidOperationException.With.Message.EqualTo("Value for property 'Key' is null."));
  }

  [Test]
  public void GetValueOrDefault_PropertyValueHasWrongType_ThrowsException ()
  {
    var testContextMock = new Mock<ITestContext>();
    testContextMock.Setup(_ => _.Properties).Returns(new Dictionary<string, object> { { "Key", 42 } });

    Assert.That(
        () => testContextMock.Object.GetValueOrDefault("Key", "defaultValue"),
        Throws.InstanceOf<InvalidCastException>().With.Message.EqualTo($"Value for property 'Key' is of type unexpected type {typeof(int)} instead of {typeof(string)}"));
  }

  [Test]
  public void GetCollection_KeyNotFound_ReturnsEmptyCollection ()
  {
    var testContextMock = new Mock<ITestContext>();
    testContextMock.Setup(_ => _.Properties).Returns(new Dictionary<string, object>());

    var result = testContextMock.Object.GetCollection<string>("Key");
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.Empty);
  }

  [Test]
  public void GetCollection_PropertyExistsInContext_ReturnsPropertyValue ()
  {
    var testContextMock = new Mock<ITestContext>();
    var stringCollection = new[] { "one", "two", "three" };
    testContextMock.Setup(_ => _.Properties).Returns(
        new Dictionary<string, object>
        {
            { "Key", stringCollection }
        });

    var result = testContextMock.Object.GetCollection<string>("Key");
    Assert.That(result, Is.Not.Null);
    Assert.That(result, Is.EquivalentTo(stringCollection));
  }

  [Test]
  public void GetCollection_PropertyValueHasWrongType_ThrowsException ()
  {
    var testContextMock = new Mock<ITestContext>();
    var stringCollection = new[] { "one", "two", "three" };
    testContextMock.Setup(_ => _.Properties).Returns(
        new Dictionary<string, object>
        {
            { "Key", stringCollection }
        });

    Assert.That(
        () => testContextMock.Object.GetCollection<int>("Key"),
        Throws.InstanceOf<InvalidCastException>().With.Message.EqualTo(
            $"Value for property 'Key' is of type unexpected type {typeof(string[])}, which is incompatible with {typeof(IReadOnlyCollection<int>)}"));
  }
}
