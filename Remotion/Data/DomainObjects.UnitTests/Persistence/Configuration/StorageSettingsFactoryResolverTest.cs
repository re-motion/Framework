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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration
{
  [TestFixture]
  public class StorageSettingsFactoryResolverTest
  {
    [Test]
    public void Resolve_ReturnsValueFromServiceLocator ()
    {
      var storageSettingsFactoryResolver = new StorageSettingsFactoryResolver();

      var storageSettingsFactoryStub = Mock.Of<IStorageSettingsFactory>();

      var defaultServiceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      defaultServiceLocator.RegisterSingle(() => storageSettingsFactoryStub);

      using (new ServiceLocatorScope(defaultServiceLocator))
      {
        var result = storageSettingsFactoryResolver.Resolve();

        Assert.That(result, Is.SameAs(storageSettingsFactoryStub));
      }
    }

    [Test]
    public void Resolve_Throws ()
    {
      var storageSettingsFactoryResolver = new StorageSettingsFactoryResolver();
      var defaultServiceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();

      using (new ServiceLocatorScope(defaultServiceLocator))
      {
        Assert.That(
            () => storageSettingsFactoryResolver.Resolve(),
            Throws.InstanceOf<ConfigurationException>()
                .With.Message
                .StartsWith("Could not locate implementation of IStorageSettingsFactory in the IoC container. Example implementation on how to set up a basic implementation:"));
      }
    }
  }
}
