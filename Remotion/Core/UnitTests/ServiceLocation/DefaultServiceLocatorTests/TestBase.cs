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
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  public class TestBase
  {
    protected DefaultServiceLocator CreateServiceLocator (IServiceConfigurationDiscoveryService serviceConfigurationDiscoveryService = null)
    {
      return new DefaultServiceLocator(
          serviceConfigurationDiscoveryService ?? new Mock<IServiceConfigurationDiscoveryService>(MockBehavior.Strict).Object,
          NullLoggerFactory.Instance);
    }

    protected ServiceConfigurationEntry CreateSingleServiceConfigurationEntry (
        Type serviceType,
        Type implementationType,
        LifetimeKind lifetimeKind = LifetimeKind.InstancePerDependency)
    {
      var implementation = new ServiceImplementationInfo(implementationType, lifetimeKind, RegistrationType.Single);
      return new ServiceConfigurationEntry(serviceType, implementation);
    }

    protected ServiceConfigurationEntry CreateMultipleServiceConfigurationEntry (
        Type serviceType,
        Type[] implementationTypes,
        LifetimeKind lifetimeKind = LifetimeKind.InstancePerDependency)
    {
      var implementations = implementationTypes.Select(t=> new ServiceImplementationInfo(t, lifetimeKind, RegistrationType.Multiple));
      return new ServiceConfigurationEntry(serviceType, implementations);
    }

    protected ServiceConfigurationEntry CreateCompoundServiceConfigurationEntry (
        Type serviceType,
        Type compoundType,
        Type[] implementationTypes,
        LifetimeKind lifetimeKind = LifetimeKind.InstancePerDependency)
    {
      var implementations = new[] { new ServiceImplementationInfo(compoundType, lifetimeKind, RegistrationType.Compound) }
          .Concat(implementationTypes.Select(t => new ServiceImplementationInfo(t, lifetimeKind, RegistrationType.Multiple)));
      return new ServiceConfigurationEntry(serviceType, implementations);
    }

    protected ServiceConfigurationEntry CreateDecoratorServiceConfigurationEntry (
        Type serviceType,
        Type[] decoratorTypes,
        Type implementationType,
        LifetimeKind lifetimeKind = LifetimeKind.InstancePerDependency)
    {
      var implementations = decoratorTypes.Select(t => new ServiceImplementationInfo(t, lifetimeKind, RegistrationType.Decorator))
          .Concat(new[] { new ServiceImplementationInfo(implementationType, lifetimeKind, RegistrationType.Single) });
      return new ServiceConfigurationEntry(serviceType, implementations);
    }

    protected ServiceConfigurationEntry CreateInstanceService ()
    {
      return new ServiceConfigurationEntry(
          typeof(InstanceService),
          new ServiceImplementationInfo(typeof(InstanceService), LifetimeKind.InstancePerDependency, RegistrationType.Single));
    }

    protected ServiceConfigurationEntry CreateSingletonService ()
    {
      return new ServiceConfigurationEntry(
          typeof(SingletonService),
          new ServiceImplementationInfo(typeof(SingletonService), LifetimeKind.Singleton, RegistrationType.Single));
    }

    protected ServiceConfigurationEntry CreateSingleService ()
    {
      return new ServiceConfigurationEntry(
          typeof(SingleService),
          new ServiceImplementationInfo(typeof(SingleService), LifetimeKind.Singleton, RegistrationType.Single));
    }

    protected ServiceConfigurationEntry CreateMultipleService ()
    {
      return new ServiceConfigurationEntry(
          typeof(MultipleService),
          new ServiceImplementationInfo(typeof(MultipleService), LifetimeKind.Singleton, RegistrationType.Multiple));
    }

    protected ServiceConfigurationEntry CreateParameterizedService ()
    {
      return new ServiceConfigurationEntry(
          typeof(ParameterizedService),
          new ServiceImplementationInfo(typeof(ParameterizedService), LifetimeKind.InstancePerDependency, RegistrationType.Single));
    }
  }
}
