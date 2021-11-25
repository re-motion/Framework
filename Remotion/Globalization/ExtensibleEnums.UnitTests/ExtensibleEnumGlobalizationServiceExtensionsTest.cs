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
using Remotion.Development.UnitTesting.Reflection;
using Remotion.ExtensibleEnums;
using Remotion.Globalization.ExtensibleEnums.UnitTests.TestDomain;
using Remotion.Globalization.Implementation;

namespace Remotion.Globalization.ExtensibleEnums.UnitTests
{
  [TestFixture]
  public class ExtensibleEnumGlobalizationServiceExtensionsTest
  {
    private Mock<IExtensibleEnumGlobalizationService> _serviceStub;
    private Mock<IExtensibleEnum> _valueStub;
    private MemoryAppender _memoryAppender;

    [SetUp]
    public void SetUp ()
    {
      _serviceStub = new Mock<IExtensibleEnumGlobalizationService>();
      _valueStub = new Mock<IExtensibleEnum>();

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
          .Setup (_ => _.TryGetExtensibleEnumValueDisplayName (_valueStub.Object, out outValue))
          .Returns (true);

      Assert.That (_serviceStub.Object.GetExtensibleEnumValueDisplayName (_valueStub.Object), Is.EqualTo ("expected"));
    }

    [Test]
    public void GetEnumerationValueDisplayNameOrDefault_WithResourceManager_ReturnsLocalizedValue ()
    {
      var outValue = "expected";

      _serviceStub
          .Setup (_ => _.TryGetExtensibleEnumValueDisplayName (_valueStub.Object, out outValue))
          .Returns (true);

      Assert.That (_serviceStub.Object.GetExtensibleEnumValueDisplayNameOrDefault (_valueStub.Object), Is.EqualTo ("expected"));
    }

    [Test]
    public void ContainsExtensibleEnumerationValueDisplayName_WithResourceManager_ReturnsLocalizedValue ()
    {
      var outValue = "expected";

      _serviceStub
          .Setup (_ => _.TryGetExtensibleEnumValueDisplayName (_valueStub.Object, out outValue))
          .Returns (true);

      Assert.That (_serviceStub.Object.ContainsExtensibleEnumValueDisplayName (_valueStub.Object), Is.True);
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithoutResourceManager_ReturnsValueName ()
    {
      var valueInfoStub = new Mock<IExtensibleEnumInfo>();
      valueInfoStub.Setup (_ => _.DefiningMethod).Returns (NormalizingMemberInfoFromExpressionUtility.GetMethod (() => ColorExtensions.Red (null)));
      string outValue = null;

      _valueStub.Setup (_ => _.ValueName).Returns ("expected");
      _valueStub.Setup (_ => _.GetValueInfo()).Returns (valueInfoStub.Object);

      _serviceStub
          .Setup (_ => _.TryGetExtensibleEnumValueDisplayName (_valueStub.Object, out outValue))
          .Returns (false);

      Assert.That (_serviceStub.Object.GetExtensibleEnumValueDisplayName (_valueStub.Object), Is.EqualTo ("expected"));

      LoggingEvent[] events = _memoryAppender.GetEvents();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Debug));
      Assert.That (
          events[0].RenderedMessage,
          Is.EqualTo (
              "No resource entry exists for the following element: Extensible enum value: 'expected' (Method: 'Red', Type: 'Remotion.Globalization.ExtensibleEnums.UnitTests.TestDomain.ColorExtensions')"));
    }

    [Test]
    public void GetEnumerationValueDisplayNameOrDefault_WithoutResourceManager_ReturnsNull ()
    {
      string outValue = null;

      _serviceStub
          .Setup (_ => _.TryGetExtensibleEnumValueDisplayName (_valueStub.Object, out outValue))
          .Returns (false);

      Assert.That (_serviceStub.Object.GetExtensibleEnumValueDisplayNameOrDefault (_valueStub.Object), Is.Null);
    }

    [Test]
    public void ContainsExtensibleEnumerationValueDisplayName_WithoutResourceManager_ReturnsFalse ()
    {
      string outValue = null;

      _serviceStub
          .Setup (_ => _.TryGetExtensibleEnumValueDisplayName (_valueStub.Object, out outValue))
          .Returns (false);

      Assert.That (_serviceStub.Object.ContainsExtensibleEnumValueDisplayName (_valueStub.Object), Is.False);
    }
  }
}