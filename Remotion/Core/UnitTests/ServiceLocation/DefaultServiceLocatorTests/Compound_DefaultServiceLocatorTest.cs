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
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  [TestFixture]
  public class Compound_DefaultServiceLocatorTest : TestBase
  {
    [Test]
    public void GetInstance_InstantiatesImplementationsInOrderOfRegistration ()
    {
      var serviceConfigurationEntry = CreateCompoundServiceConfigurationEntry(
          typeof(ITestType),
          typeof(TestCompound),
          new[] { typeof(TestImplementation1), typeof(TestImplementation2) });

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var instance = serviceLocator.GetInstance(typeof(ITestType));

      Assert.That(instance, Is.TypeOf<TestCompound>());
      var compoundInstance = (TestCompound)instance;
      Assert.That(
          compoundInstance.InnerObjects.Select(c => c.GetType()),
          Is.EqualTo(new[] { typeof(TestImplementation1), typeof(TestImplementation2) }));
    }

    [Test]
    public void GetInstance_CompoundWithEmptyImplementationsList_InstantiatesEmptyCompound ()
    {
      var serviceConfigurationEntry = CreateCompoundServiceConfigurationEntry(
          typeof(ITestType),
          typeof(TestCompound),
          new Type[0]);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var instance = serviceLocator.GetInstance(typeof(ITestType));

      Assert.That(instance, Is.TypeOf<TestCompound>());
      var compoundInstance = (TestCompound)instance;
      Assert.That(compoundInstance.InnerObjects, Is.Empty);
    }

    [Test]
    public void GetInstance_CompoundIsInstance_ImplementationIsSingleton_ImplementationsBehaveAsSingleton ()
    {
      var compound = new ServiceImplementationInfo(typeof(TestCompound), LifetimeKind.InstancePerDependency, RegistrationType.Compound);
      var implementation = new ServiceImplementationInfo(typeof(TestImplementation1), LifetimeKind.Singleton, RegistrationType.Multiple);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestType), compound, implementation);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var instance1 = serviceLocator.GetInstance(typeof(ITestType));
      var instance2 = serviceLocator.GetInstance(typeof(ITestType));

      Assert.That(instance1, Is.Not.SameAs(instance2));
      Assert.That(((TestCompound)instance1).InnerObjects, Is.EqualTo(((TestCompound)instance2).InnerObjects));
    }

    [Test]
    public void GetInstance_CompoundIsSingleton_ImplementationIsInstancePerDependency_ImplementationsBehaveAsSingleton ()
    {
      var compound = new ServiceImplementationInfo(typeof(TestCompound), LifetimeKind.Singleton, RegistrationType.Compound);
      var implementation = new ServiceImplementationInfo(typeof(TestImplementation1), LifetimeKind.InstancePerDependency, RegistrationType.Multiple);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestType), compound, implementation);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var instance1 = serviceLocator.GetInstance(typeof(ITestType));
      var instance2 = serviceLocator.GetInstance(typeof(ITestType));

      Assert.That(instance1, Is.SameAs(instance2));
      Assert.That(((TestCompound)instance1).InnerObjects, Is.EqualTo(((TestCompound)instance2).InnerObjects));
    }

    [Test]
    public void GetAllInstances_ThrowsActivationException ()
    {
      var serviceConfigurationEntry = CreateCompoundServiceConfigurationEntry(
          typeof(ITestType),
          typeof(TestCompound),
          new Type[0]);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      Assert.That(
          () => serviceLocator.GetAllInstances(typeof(ITestType)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo(
              "A compound implementation is configured for service type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType'. "
              + "Use GetInstance() to retrieve the implementation."));
    }

    [Test]
    public void Register_CompoundWithoutPublicConstructor_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateCompoundServiceConfigurationEntry(
          typeof(ITestCompoundWithErrors),
          typeof(TestCompoundWithoutPublicConstructor),
          new Type[0]);

      var serviceLocator = CreateServiceLocator();

      Assert.That(
          () => serviceLocator.Register(serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestCompoundWithoutPublicConstructor' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'System.Collections.Generic.IEnumerable`1[Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestCompoundWithErrors]'."));
    }

    [Test]
    public void Register_CompoundWithConstructorWithoutArguments_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateCompoundServiceConfigurationEntry(
          typeof(ITestCompoundWithErrors),
          typeof(TestCompoundWithConstructorWithoutArguments),
          new Type[0]);

      var serviceLocator = CreateServiceLocator();

      Assert.That(
          () => serviceLocator.Register(serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestCompoundWithConstructorWithoutArguments' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'System.Collections.Generic.IEnumerable`1[Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestCompoundWithErrors]'."));
    }

    [Test]
    public void Register_CompoundWithConstructorWithoutMatchingArgument_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateCompoundServiceConfigurationEntry(
          typeof(ITestCompoundWithErrors),
          typeof(TestCompoundWithConstructorWithoutMatchingArgument),
          new Type[0]);

      var serviceLocator = CreateServiceLocator();

      Assert.That(
          () => serviceLocator.Register(serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestCompoundWithConstructorWithoutMatchingArgument' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'System.Collections.Generic.IEnumerable`1[Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestCompoundWithErrors]'."));
    }

    [Test]
    public void Register_CompoundWithMultipleRegistrations_ThrowsInvalidOperationException ()
    {
      var compound1 = new ServiceImplementationInfo(
          typeof(TestCompoundWithMultipleRegistrations1),
          LifetimeKind.InstancePerDependency,
          RegistrationType.Compound);
      var compound2 = new ServiceImplementationInfo(
          typeof(TestCompoundWithMultipleRegistrations2),
          LifetimeKind.InstancePerDependency,
          RegistrationType.Compound);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestCompoundWithErrors), compound1, compound2);

      var serviceLocator = CreateServiceLocator();

      Assert.That(
          () => serviceLocator.Register(serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Cannot register multiple implementations with registration type 'Compound' "
              + "for service type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestCompoundWithErrors'."));
    }
  }
}
