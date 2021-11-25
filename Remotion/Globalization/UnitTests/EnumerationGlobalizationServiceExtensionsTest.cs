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
  public class EnumerationGlobalizationServiceExtensionsTest
  {
    private Mock<IEnumerationGlobalizationService> _serviceStub;
    private Enum _value;
    private MemoryAppender _memoryAppender;

    [SetUp]
    public void SetUp ()
    {
      _serviceStub = new Mock<IEnumerationGlobalizationService>();
      _value = EnumWithResources.Value1;

      _memoryAppender = new MemoryAppender();

      LoggerMatchFilter acceptFilter = new LoggerMatchFilter();
      acceptFilter.LoggerToMatch = typeof (ResourceLogger).FullName;
      acceptFilter.AcceptOnMatch = true;
      _memoryAppender.AddFilter (acceptFilter);

      DenyAllFilter denyFilter = new DenyAllFilter();
      _memoryAppender.AddFilter (denyFilter);

      BasicConfigurator.Configure (_memoryAppender);
    }

    [TearDown]
    public void TearDown ()
    {
      LogManager.ResetConfiguration();
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithResourceManager_ReturnsLocalizedValue ()
    {
      var outValue = "expected";

      _serviceStub
          .Setup (_ => _.TryGetEnumerationValueDisplayName (_value, out outValue))
          .Returns (true);

      Assert.That (_serviceStub.Object.GetEnumerationValueDisplayName (_value), Is.EqualTo ("expected"));
    }

    [Test]
    public void GetEnumerationValueDisplayNameOrDefault_WithResourceManager_ReturnsLocalizedValue ()
    {
      var outValue = "expected";

      _serviceStub
          .Setup (_ => _.TryGetEnumerationValueDisplayName (_value, out outValue))
          .Returns (true);

      Assert.That (_serviceStub.Object.GetEnumerationValueDisplayNameOrDefault (_value), Is.EqualTo ("expected"));
    }

    [Test]
    public void ContainsEnumerationValueDisplayName_WithResourceManager_ReturnsLocalizedValue ()
    {
      var outValue = "expected";

      _serviceStub
          .Setup (_ => _.TryGetEnumerationValueDisplayName (_value, out outValue))
          .Returns (true);

      Assert.That (_serviceStub.Object.ContainsEnumerationValueDisplayName (_value), Is.True);
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithoutResourceManager_ReturnsValueName ()
    {
      string outValue = null;

      _serviceStub
          .Setup (_ => _.TryGetEnumerationValueDisplayName (_value, out outValue))
          .Returns (false);

      Assert.That (_serviceStub.Object.GetEnumerationValueDisplayName (_value), Is.EqualTo ("Value1"));

      LoggingEvent[] events = _memoryAppender.GetEvents();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Debug));
      Assert.That (
          events[0].RenderedMessage,
          Is.EqualTo (
              "No resource entry exists for the following element: Enum value: 'Value1' (Type: 'Remotion.Globalization.UnitTests.TestDomain.EnumWithResources')"));
    }

    [Test]
    public void GetEnumerationValueDisplayNameOrDefault_WithoutResourceManager_ReturnsNull ()
    {
      string outValue = null;

      _serviceStub
          .Setup (_ => _.TryGetEnumerationValueDisplayName (_value, out outValue))
          .Returns (false);

      Assert.That (_serviceStub.Object.GetEnumerationValueDisplayNameOrDefault (_value), Is.Null);
    }

    [Test]
    public void ContainsEnumerationValueDisplayName_WithoutResourceManager_ReturnsFalse ()
    {
      string outValue = null;

      _serviceStub
          .Setup (_ => _.TryGetEnumerationValueDisplayName (_value, out outValue))
          .Returns (false);

      Assert.That (_serviceStub.Object.ContainsEnumerationValueDisplayName (_value), Is.False);
    }
  }
}