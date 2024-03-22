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
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using Moq;
using NUnit.Framework;
using Remotion.Globalization.Implementation;
using Remotion.Globalization.UnitTests.TestDomain;

namespace Remotion.Globalization.UnitTests
{
  [TestFixture]
  public class ResourceManagerExtensionsTest
  {
    private Mock<IResourceManager> _resourceManagerMock;
    private string _fakeResourceID;
    private MemoryAppender _memoryAppender;

    [SetUp]
    public void SetUp ()
    {
      _fakeResourceID = "fakeID";

      _resourceManagerMock = new Mock<IResourceManager>();

      _memoryAppender = new MemoryAppender();

      LoggerMatchFilter acceptFilter = new LoggerMatchFilter();
      acceptFilter.LoggerToMatch = typeof(ResourceLogger).FullName;
      acceptFilter.AcceptOnMatch = true;
      _memoryAppender.AddFilter(acceptFilter);

      DenyAllFilter denyFilter = new DenyAllFilter();
      _memoryAppender.AddFilter(denyFilter);

      BasicConfigurator.Configure(_memoryAppender);
    }

    [TearDown]
    public void TearDown ()
    {
      LogManager.ResetConfiguration();
    }

    [Test]
    public void GetAllStrings ()
    {
      var fakeResult = new Dictionary<string, string>();
      _resourceManagerMock
          .Setup(mock => mock.GetAllStrings(null))
          .Returns(fakeResult)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetAllStrings();

      _resourceManagerMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult));
    }

    [Test]
    public void GetString_ResourceExists ()
    {
      var outValue = "Test";

      _resourceManagerMock
          .Setup(mock => mock.TryGetString(_fakeResourceID, out outValue))
          .Returns(true)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetString(_fakeResourceID);

      _resourceManagerMock.Verify();
      Assert.That(result, Is.EqualTo("Test"));
    }

    [Test]
    public void GetString_ResourceDoesNotExist ()
    {
      string outValue = null;

      _resourceManagerMock
          .Setup(mock => mock.TryGetString(_fakeResourceID, out outValue))
          .Returns(false)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetString(_fakeResourceID);

      _resourceManagerMock.Verify();
      Assert.That(result, Is.EqualTo(_fakeResourceID));

      LoggingEvent[] events = _memoryAppender.GetEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      Assert.That(events[0].Level, Is.EqualTo(Level.Debug));
      Assert.That(
          events[0].RenderedMessage,
          Is.EqualTo("No resource entry exists for the following element: ID: 'fakeID'"));
    }

    [Test]
    public void GetStringOrDefault_ResourceExists ()
    {
      var outValue = "Test";

      _resourceManagerMock.Setup(mock => mock.TryGetString(_fakeResourceID, out outValue)).Returns(true).Verifiable();

      var result = _resourceManagerMock.Object.GetStringOrDefault(_fakeResourceID);

      _resourceManagerMock.Verify();
      Assert.That(result, Is.EqualTo("Test"));
    }

    [Test]
    public void GetStringOrDefault_ResourceDoesNotExist ()
    {
      string outValue = null;

      _resourceManagerMock
          .Setup(mock => mock.TryGetString(_fakeResourceID, out outValue))
          .Returns(false)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetStringOrDefault(_fakeResourceID);

      _resourceManagerMock.Verify();
      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetString_EnumOverload_ResourceExists ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;
      var enumResourceID = ResourceIdentifiersAttribute.GetResourceIdentifier(enumValue);
      var outValue = "Test";

      _resourceManagerMock
          .Setup(mock => mock.TryGetString(enumResourceID, out outValue))
          .Returns(true)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetString(enumValue);

      _resourceManagerMock.Verify();
      Assert.That(result, Is.EqualTo("Test"));
    }

    [Test]
    public void GetString_EnumOverload_ResourceDoesNotExist ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;
      var enumResourceID = ResourceIdentifiersAttribute.GetResourceIdentifier(enumValue);
      var outValue = "Test";

      _resourceManagerMock
          .Setup(mock => mock.TryGetString(enumResourceID, out outValue))
          .Returns(false)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetString(enumValue);

      _resourceManagerMock.Verify();
      Assert.That(result, Is.EqualTo(enumResourceID));
    }

    [Test]
    public void GetStringOrDefault_EnumOverload_ResourceExists ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;
      var enumResourceID = ResourceIdentifiersAttribute.GetResourceIdentifier(enumValue);
      var outValue = "Test";

      _resourceManagerMock
          .Setup(mock => mock.TryGetString(enumResourceID, out outValue))
          .Returns(true)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetStringOrDefault(enumValue);

      _resourceManagerMock.Verify();
      Assert.That(result, Is.EqualTo("Test"));
    }

    [Test]
    public void GetStringOrDefault_EnumOverload_ResourceDoesNotExist ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;
      var enumResourceID = ResourceIdentifiersAttribute.GetResourceIdentifier(enumValue);
      var outValue = "Test";

      _resourceManagerMock
          .Setup(mock => mock.TryGetString(enumResourceID, out outValue))
          .Returns(false)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetStringOrDefault(enumValue);

      _resourceManagerMock.Verify();
      Assert.That(result, Is.Null);
    }

    [Test]
    public void ContainsString_ResourceExists ()
    {
      var outValue = "Test";

      _resourceManagerMock
          .Setup(mock => mock.TryGetString(_fakeResourceID, out outValue))
          .Returns(true)
          .Verifiable();

      var result = _resourceManagerMock.Object.ContainsString(_fakeResourceID);

      _resourceManagerMock.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void ContainsString_ResourceDoesNotExist ()
    {
      string outValue = null;

      _resourceManagerMock
          .Setup(mock => mock.TryGetString(_fakeResourceID, out outValue))
          .Returns(false)
          .Verifiable();

      var result = _resourceManagerMock.Object.ContainsString(_fakeResourceID);

      _resourceManagerMock.Verify();
      Assert.That(result, Is.False);
    }

    [Test]
    public void ContainsString_EnumOverload_ResourceExists ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;
      var enumResourceID = ResourceIdentifiersAttribute.GetResourceIdentifier(enumValue);
      var outValue = "Test";

      _resourceManagerMock
          .Setup(mock => mock.TryGetString(enumResourceID, out outValue))
          .Returns(true)
          .Verifiable();

      var result = _resourceManagerMock.Object.ContainsString(enumValue);

      _resourceManagerMock.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void ContainsString_EnumOverload_ResourceDoesNotExist ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;
      var enumResourceID = ResourceIdentifiersAttribute.GetResourceIdentifier(enumValue);
      var outValue = "Test";

      _resourceManagerMock
          .Setup(mock => mock.TryGetString(enumResourceID, out outValue))
          .Returns(false)
          .Verifiable();

      var result = _resourceManagerMock.Object.ContainsString(enumValue);

      _resourceManagerMock.Verify();
      Assert.That(result, Is.False);
    }
  }
}
