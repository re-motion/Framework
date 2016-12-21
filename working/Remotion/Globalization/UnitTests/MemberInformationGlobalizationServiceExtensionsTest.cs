﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using NUnit.Framework;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Globalization.UnitTests
{
  [TestFixture]
  public class MemberInformationGlobalizationServiceExtensionsTest
  {
    private IMemberInformationGlobalizationService _serviceStub;
    private ITypeInformation _typeInformationForResourceResolutionStub;
    private ITypeInformation _typeInformationStub;
    private IPropertyInformation _propertyInformationStub;
    private MemoryAppender _memoryAppender;

    [SetUp]
    public void SetUp ()
    {
      _typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      _typeInformationStub.Stub (stub => stub.Name).Return ("TypeName");
      _typeInformationStub.Stub (stub => stub.FullName).Return ("FullTypeName");

      _propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      _propertyInformationStub.Stub (stub => stub.Name).Return ("PropertyName");

      _typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      _serviceStub = MockRepository.GenerateStub<IMemberInformationGlobalizationService> ();

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
    public void TearDown()
    {
      LogManager.ResetConfiguration();
    }

    [Test]
    public void ContainsPropertyDisplayName_NoResourceFound_ReturnsFalse ()
    {
      _serviceStub
          .Stub (
              _ => _.TryGetPropertyDisplayName (
                  Arg.Is (_propertyInformationStub),
                  Arg.Is (_typeInformationForResourceResolutionStub),
                  out Arg<string>.Out (null).Dummy))
          .Return (false);

      var result = _serviceStub.ContainsPropertyDisplayName (_propertyInformationStub, _typeInformationForResourceResolutionStub);

      Assert.That (result, Is.False);
    }

    [Test]
    public void ContainsPropertyDisplayName_ResourceFound_ReturnsTrue ()
    {
      _serviceStub
          .Stub (
              _ => _.TryGetPropertyDisplayName (
                  Arg.Is (_propertyInformationStub),
                  Arg.Is (_typeInformationForResourceResolutionStub),
                  out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      var result = _serviceStub.ContainsPropertyDisplayName (_propertyInformationStub, _typeInformationForResourceResolutionStub);

      Assert.That (result, Is.True);
    }

    [Test]
    public void GetPropertyDisplayName_NoResourceFound_ReturnsShortPropertyName ()
    {
      _serviceStub
          .Stub (
              _ => _.TryGetPropertyDisplayName (
                  Arg.Is (_propertyInformationStub),
                  Arg.Is (_typeInformationForResourceResolutionStub),
                  out Arg<string>.Out (null).Dummy))
          .Return (false);

      _propertyInformationStub.Stub (_ => _.DeclaringType).Return (_typeInformationStub);

      var result = _serviceStub.GetPropertyDisplayName (_propertyInformationStub, _typeInformationForResourceResolutionStub);

      Assert.That (result, Is.EqualTo ("PropertyName"));

      LoggingEvent[] events = _memoryAppender.GetEvents();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Debug));
      Assert.That (
          events[0].RenderedMessage,
          Is.EqualTo ("No resource entry exists for the following element: Property: 'PropertyName' (Type: 'FullTypeName')"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFound_ReturnsLocalizedValue ()
    {
      _serviceStub
          .Stub (
              _ => _.TryGetPropertyDisplayName (
                  Arg.Is (_propertyInformationStub),
                  Arg.Is (_typeInformationForResourceResolutionStub),
                  out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      var result = _serviceStub.GetPropertyDisplayName (_propertyInformationStub, _typeInformationForResourceResolutionStub);

      Assert.That (result, Is.EqualTo ("expected"));
    }

    [Test]
    public void GetPropertyDisplayNameOrDefault_NoResourceFound_ReturnsNull ()
    {
      _serviceStub
          .Stub (
              _ => _.TryGetPropertyDisplayName (
                  Arg.Is (_propertyInformationStub),
                  Arg.Is (_typeInformationForResourceResolutionStub),
                  out Arg<string>.Out (null).Dummy))
          .Return (false);

      var result = _serviceStub.GetPropertyDisplayNameOrDefault (_propertyInformationStub, _typeInformationForResourceResolutionStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetPropertyDisplayNameOrDefault_ResourceFound_ReturnsLocalizedValue ()
    {
      _serviceStub
          .Stub (
              _ => _.TryGetPropertyDisplayName (
                  Arg.Is (_propertyInformationStub),
                  Arg.Is (_typeInformationForResourceResolutionStub),
                  out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      var result = _serviceStub.GetPropertyDisplayNameOrDefault (_propertyInformationStub, _typeInformationForResourceResolutionStub);

      Assert.That (result, Is.EqualTo ("expected"));
    }


    [Test]
    public void ContainsTypeDisplayName_NoResourceFound_ReturnsFalse ()
    {
      _serviceStub
          .Stub (
              _ => _.TryGetTypeDisplayName (
                  Arg.Is (_typeInformationStub),
                  Arg.Is (_typeInformationForResourceResolutionStub),
                  out Arg<string>.Out (null).Dummy))
          .Return (false);

      var result = _serviceStub.ContainsTypeDisplayName (_typeInformationStub, _typeInformationForResourceResolutionStub);

      Assert.That (result, Is.False);
    }

    [Test]
    public void ContainsTypeDisplayName_ResourceFound_ReturnsTrue ()
    {
      _serviceStub
          .Stub (
              _ => _.TryGetTypeDisplayName (
                  Arg.Is (_typeInformationStub),
                  Arg.Is (_typeInformationForResourceResolutionStub),
                  out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      var result = _serviceStub.ContainsTypeDisplayName (_typeInformationStub, _typeInformationForResourceResolutionStub);

      Assert.That (result, Is.True);
    }

    [Test]
    public void GetTypeDisplayName_NoResourceFound_ReturnsShortTypeName ()
    {
      _serviceStub
          .Stub (
              _ => _.TryGetTypeDisplayName (
                  Arg.Is (_typeInformationStub),
                  Arg.Is (_typeInformationForResourceResolutionStub),
                  out Arg<string>.Out (null).Dummy))
          .Return (false);

      var result = _serviceStub.GetTypeDisplayName (_typeInformationStub, _typeInformationForResourceResolutionStub);

      Assert.That (result, Is.EqualTo ("TypeName"));

      LoggingEvent[] events = _memoryAppender.GetEvents();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Debug));
      Assert.That (
          events[0].RenderedMessage,
          Is.EqualTo ("No resource entry exists for the following element: Type: 'FullTypeName'"));
    }

    [Test]
    public void GetTypeDisplayName_ResourceFound_ReturnsLocalizedValue ()
    {
      _serviceStub
          .Stub (
              _ => _.TryGetTypeDisplayName (
                  Arg.Is (_typeInformationStub),
                  Arg.Is (_typeInformationForResourceResolutionStub),
                  out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      var result = _serviceStub.GetTypeDisplayName (_typeInformationStub, _typeInformationForResourceResolutionStub);

      Assert.That (result, Is.EqualTo ("expected"));
    }

    [Test]
    public void GetTypeDisplayNameOrDefault_NoResourceFound_ReturnsNull ()
    {
      _serviceStub
          .Stub (
              _ => _.TryGetTypeDisplayName (
                  Arg.Is (_typeInformationStub),
                  Arg.Is (_typeInformationForResourceResolutionStub),
                  out Arg<string>.Out (null).Dummy))
          .Return (false);

      var result = _serviceStub.GetTypeDisplayNameOrDefault (_typeInformationStub, _typeInformationForResourceResolutionStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetTypeDisplayNameOrDefault_ResourceFound_ReturnsLocalizedValue ()
    {
      _serviceStub
          .Stub (
              _ => _.TryGetTypeDisplayName (
                  Arg.Is (_typeInformationStub),
                  Arg.Is (_typeInformationForResourceResolutionStub),
                  out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      var result = _serviceStub.GetTypeDisplayNameOrDefault (_typeInformationStub, _typeInformationForResourceResolutionStub);

      Assert.That (result, Is.EqualTo ("expected"));
    }
  }
}