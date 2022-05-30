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
using Remotion.Reflection;

namespace Remotion.Globalization.UnitTests
{
  [TestFixture]
  public class MemberInformationGlobalizationServiceExtensionsTest
  {
    private Mock<IMemberInformationGlobalizationService> _serviceStub;
    private Mock<ITypeInformation> _typeInformationForResourceResolutionStub;
    private Mock<ITypeInformation> _typeInformationStub;
    private Mock<IPropertyInformation> _propertyInformationStub;
    private MemoryAppender _memoryAppender;

    [SetUp]
    public void SetUp ()
    {
      _typeInformationStub = new Mock<ITypeInformation>();
      _typeInformationStub.Setup(stub => stub.Name).Returns("TypeName");
      _typeInformationStub.Setup(stub => stub.FullName).Returns("FullTypeName");

      _propertyInformationStub = new Mock<IPropertyInformation>();
      _propertyInformationStub.Setup(stub => stub.Name).Returns("PropertyName");

      _typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();

      _serviceStub = new Mock<IMemberInformationGlobalizationService>();

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
    public void ContainsPropertyDisplayName_NoResourceFound_ReturnsFalse ()
    {
      string outValue = null;

      _serviceStub
          .Setup(
              _ => _.TryGetPropertyDisplayName(
                  _propertyInformationStub.Object,
                  _typeInformationForResourceResolutionStub.Object,
                  out outValue))
          .Returns(false);

      var result = _serviceStub.Object.ContainsPropertyDisplayName(_propertyInformationStub.Object, _typeInformationForResourceResolutionStub.Object);

      Assert.That(result, Is.False);
    }

    [Test]
    public void ContainsPropertyDisplayName_ResourceFound_ReturnsTrue ()
    {
      var outValue = "expected";

      _serviceStub
          .Setup(
              _ => _.TryGetPropertyDisplayName(
                  _propertyInformationStub.Object,
                  _typeInformationForResourceResolutionStub.Object,
                  out outValue))
          .Returns(true);

      var result = _serviceStub.Object.ContainsPropertyDisplayName(_propertyInformationStub.Object, _typeInformationForResourceResolutionStub.Object);

      Assert.That(result, Is.True);
    }

    [Test]
    public void GetPropertyDisplayName_NoResourceFound_ReturnsShortPropertyName ()
    {
      string outValue = null;

      _serviceStub
          .Setup(
              _ => _.TryGetPropertyDisplayName(
                  _propertyInformationStub.Object,
                  _typeInformationForResourceResolutionStub.Object,
                  out outValue))
          .Returns(false);

      _propertyInformationStub.Setup(_ => _.DeclaringType).Returns(_typeInformationStub.Object);

      var result = _serviceStub.Object.GetPropertyDisplayName(_propertyInformationStub.Object, _typeInformationForResourceResolutionStub.Object);

      Assert.That(result, Is.EqualTo("PropertyName"));

      LoggingEvent[] events = _memoryAppender.GetEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      Assert.That(events[0].Level, Is.EqualTo(Level.Debug));
      Assert.That(
          events[0].RenderedMessage,
          Is.EqualTo("No resource entry exists for the following element: Property: 'PropertyName' (Type: 'FullTypeName')"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFound_ReturnsLocalizedValue ()
    {
      var outValue = "expected";

      _serviceStub
          .Setup(
              _ => _.TryGetPropertyDisplayName(
                  _propertyInformationStub.Object,
                  _typeInformationForResourceResolutionStub.Object,
                  out outValue))
          .Returns(true);

      var result = _serviceStub.Object.GetPropertyDisplayName(_propertyInformationStub.Object, _typeInformationForResourceResolutionStub.Object);

      Assert.That(result, Is.EqualTo("expected"));
    }

    [Test]
    public void GetPropertyDisplayNameOrDefault_NoResourceFound_ReturnsNull ()
    {
      string outValue = null;

      _serviceStub
          .Setup(
              _ => _.TryGetPropertyDisplayName(
                  _propertyInformationStub.Object,
                  _typeInformationForResourceResolutionStub.Object,
                  out outValue))
          .Returns(false);

      var result = _serviceStub.Object.GetPropertyDisplayNameOrDefault(_propertyInformationStub.Object, _typeInformationForResourceResolutionStub.Object);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetPropertyDisplayNameOrDefault_ResourceFound_ReturnsLocalizedValue ()
    {
      var outValue = "expected";

      _serviceStub
          .Setup(
              _ => _.TryGetPropertyDisplayName(
                  _propertyInformationStub.Object,
                  _typeInformationForResourceResolutionStub.Object,
                  out outValue))
          .Returns(true);

      var result = _serviceStub.Object.GetPropertyDisplayNameOrDefault(_propertyInformationStub.Object, _typeInformationForResourceResolutionStub.Object);

      Assert.That(result, Is.EqualTo("expected"));
    }


    [Test]
    public void ContainsTypeDisplayName_NoResourceFound_ReturnsFalse ()
    {
      string outValue = null;

      _serviceStub
          .Setup(
              _ => _.TryGetTypeDisplayName(
                  _typeInformationStub.Object,
                  _typeInformationForResourceResolutionStub.Object,
                  out outValue))
          .Returns(false);

      var result = _serviceStub.Object.ContainsTypeDisplayName(_typeInformationStub.Object, _typeInformationForResourceResolutionStub.Object);

      Assert.That(result, Is.False);
    }

    [Test]
    public void ContainsTypeDisplayName_ResourceFound_ReturnsTrue ()
    {
      var outValue = "expected";

      _serviceStub
          .Setup(
              _ => _.TryGetTypeDisplayName(
                  _typeInformationStub.Object,
                  _typeInformationForResourceResolutionStub.Object,
                  out outValue))
          .Returns(true);

      var result = _serviceStub.Object.ContainsTypeDisplayName(_typeInformationStub.Object, _typeInformationForResourceResolutionStub.Object);

      Assert.That(result, Is.True);
    }

    [Test]
    public void GetTypeDisplayName_NoResourceFound_ReturnsShortTypeName ()
    {
      string outValue = null;

      _serviceStub
          .Setup(
              _ => _.TryGetTypeDisplayName(
                  _typeInformationStub.Object,
                  _typeInformationForResourceResolutionStub.Object,
                  out outValue))
          .Returns(false);

      var result = _serviceStub.Object.GetTypeDisplayName(_typeInformationStub.Object, _typeInformationForResourceResolutionStub.Object);

      Assert.That(result, Is.EqualTo("TypeName"));

      LoggingEvent[] events = _memoryAppender.GetEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      Assert.That(events[0].Level, Is.EqualTo(Level.Debug));
      Assert.That(
          events[0].RenderedMessage,
          Is.EqualTo("No resource entry exists for the following element: Type: 'FullTypeName'"));
    }

    [Test]
    public void GetTypeDisplayName_ResourceFound_ReturnsLocalizedValue ()
    {
      var outValue = "expected";

      _serviceStub
          .Setup(
              _ => _.TryGetTypeDisplayName(
                  _typeInformationStub.Object,
                  _typeInformationForResourceResolutionStub.Object,
                  out outValue))
          .Returns(true);

      var result = _serviceStub.Object.GetTypeDisplayName(_typeInformationStub.Object, _typeInformationForResourceResolutionStub.Object);

      Assert.That(result, Is.EqualTo("expected"));
    }

    [Test]
    public void GetTypeDisplayNameOrDefault_NoResourceFound_ReturnsNull ()
    {
      string outValue = null;

      _serviceStub
          .Setup(
              _ => _.TryGetTypeDisplayName(
                  _typeInformationStub.Object,
                  _typeInformationForResourceResolutionStub.Object,
                  out outValue))
          .Returns(false);

      var result = _serviceStub.Object.GetTypeDisplayNameOrDefault(_typeInformationStub.Object, _typeInformationForResourceResolutionStub.Object);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetTypeDisplayNameOrDefault_ResourceFound_ReturnsLocalizedValue ()
    {
      var outValue = "expected";

      _serviceStub
          .Setup(
              _ => _.TryGetTypeDisplayName(
                  _typeInformationStub.Object,
                  _typeInformationForResourceResolutionStub.Object,
                  out outValue))
          .Returns(true);

      var result = _serviceStub.Object.GetTypeDisplayNameOrDefault(_typeInformationStub.Object, _typeInformationForResourceResolutionStub.Object);

      Assert.That(result, Is.EqualTo("expected"));
    }
  }
}
