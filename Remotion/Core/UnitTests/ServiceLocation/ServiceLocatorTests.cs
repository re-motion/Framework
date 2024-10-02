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
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class ServiceLocatorTests
  {
    [SetUp]
    public void SetUp ()
    {
      ServiceLocator.SetLocatorProvider(null);
    }

    [Test]
    public void IsLocationProviderSet_WithSetProvider_ReturnsTrue ()
    {
      ServiceLocator.SetLocatorProvider(() => new DefaultServiceLocator(DefaultServiceConfigurationDiscoveryService.Create(), NullLoggerFactory.Instance));

      Assert.That(ServiceLocator.IsLocationProviderSet, Is.True);
    }

    [Test]
    public void IsLocationProviderSet_WithNotSetProvider_ReturnsFalse ()
    {
      Assert.That(ServiceLocator.IsLocationProviderSet, Is.False);
    }

    [Test]
    public void GetCurrentServiceLocator_WithoutProviderSet_ThrowsInvalidOperationException ()
    {
      Assert.That(() => ServiceLocator.Current, Throws.InstanceOf<InvalidOperationException>());
    }
  }
}
