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
using NUnit.Framework;
using Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  [TestFixture]
  public class ConstructorInjection_DefaultServiceLocatorTest : TestBase
  {
    [Test]
    public void GetInstance_ConstructorWithOneParameter_PerformsIndirectResolutionCalls ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestImplementationWithOneConstructorParameter));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register (serviceConfigurationEntry);
      serviceLocator.Register (CreateInstanceService());

      var instance = serviceLocator.GetInstance<ITestType>();

      Assert.That (instance, Is.TypeOf<TestImplementationWithOneConstructorParameter>());
      Assert.That (((TestImplementationWithOneConstructorParameter) instance).InstanceService, Is.TypeOf<InstanceService>());
    }

    [Test]
    public void GetInstance_ConstructorWithMultipleParameters_PerformsIndirectResolutionCalls()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestImplementationWithMultipleConstructorParameters));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register (serviceConfigurationEntry);
      serviceLocator.Register (CreateInstanceService());
      serviceLocator.Register (CreateSingletonService());
      serviceLocator.Register (CreateMultipleService());

      var instance = serviceLocator.GetInstance<ITestType>();

      Assert.That (instance, Is.TypeOf<TestImplementationWithMultipleConstructorParameters>());
      var typedInstance = (TestImplementationWithMultipleConstructorParameters) instance;

      Assert.That (typedInstance.InstanceService1, Is.TypeOf<InstanceService>());
      Assert.That (typedInstance.InstanceService2, Is.TypeOf<InstanceService>());
      Assert.That (typedInstance.SingletonService, Is.TypeOf<SingletonService>());
      Assert.That (typedInstance.MultipleService, Is.Not.Empty);
      Assert.That (typedInstance.MultipleService, Has.All.TypeOf<MultipleService>());
    }
    
    [Test]
    public void GetInstance_ConstructorWithRecursiveParameter_PerformsIndirectResolutionCalls ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestImplementationWithRecursiceConstructorParameter));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register (serviceConfigurationEntry);
      serviceLocator.Register (CreateParameterizedService());
      serviceLocator.Register (CreateSingleService());

      var instance = serviceLocator.GetInstance<ITestType>();

      Assert.That (instance, Is.TypeOf<TestImplementationWithRecursiceConstructorParameter>());
      var typedInstance = (TestImplementationWithRecursiceConstructorParameter) instance;
      Assert.That (typedInstance.ParameterizedService, Is.TypeOf<ParameterizedService>());
      Assert.That (typedInstance.ParameterizedService.SingleService, Is.TypeOf<SingleService>());
    }

    [Test]
    public void GetInstance_ConstructorWithParameterWithInstanceLifetime_UsesDifferentInstances ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestImplementationWithMultipleConstructorParameters));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register (serviceConfigurationEntry);
      serviceLocator.Register (CreateInstanceService());
      serviceLocator.Register (CreateSingletonService());
      serviceLocator.Register (CreateMultipleService());

      var instance1 = serviceLocator.GetInstance<ITestType>();
      var instance2 = serviceLocator.GetInstance<ITestType>();

      Assert.That (instance1, Is.Not.SameAs (instance2));
      Assert.That (instance1, Is.TypeOf<TestImplementationWithMultipleConstructorParameters>());
      var typedInstance1 = (TestImplementationWithMultipleConstructorParameters) instance1;
      var typedInstance2 = (TestImplementationWithMultipleConstructorParameters) instance2;

      Assert.That (typedInstance1.InstanceService1, Is.TypeOf (typedInstance1.InstanceService2.GetType()));
      Assert.That (typedInstance1.InstanceService1, Is.Not.SameAs (typedInstance1.InstanceService2));

      Assert.That (typedInstance1.InstanceService1, Is.Not.SameAs (typedInstance2.InstanceService1));
    }
    
    [Test]
    public void GetInstance_ConstructorWithParameterWithSingletonLifetime_UsesSameInstance ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestImplementationWithMultipleConstructorParameters));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register (serviceConfigurationEntry);
      serviceLocator.Register (CreateInstanceService());
      serviceLocator.Register (CreateSingletonService());
      serviceLocator.Register (CreateMultipleService());

      var instance1 = serviceLocator.GetInstance<ITestType>();
      var instance2 = serviceLocator.GetInstance<ITestType>();

      Assert.That (instance1, Is.Not.SameAs (instance2));
      Assert.That (instance1, Is.TypeOf<TestImplementationWithMultipleConstructorParameters>());
      var typedInstance1 = (TestImplementationWithMultipleConstructorParameters) instance1;
      var typedInstance2 = (TestImplementationWithMultipleConstructorParameters) instance2;

      Assert.That (typedInstance1.SingletonService, Is.SameAs (typedInstance2.SingletonService));
    }

    [Test]
    public void GetInstance_CompoundWithAdditionalConstructorParameters_PerformsIndirectResolutionCalls ()
    {
      var serviceConfigurationEntry = CreateCompoundServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestCompoundWithAdditionalConstructorParameters),
          new[] { typeof (TestImplementation1) });

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register (serviceConfigurationEntry);
      serviceLocator.Register (CreateSingleService());
      serviceLocator.Register (CreateMultipleService());

      var instance = serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance, Is.TypeOf<TestCompoundWithAdditionalConstructorParameters>());
      var compoundInstance = (TestCompoundWithAdditionalConstructorParameters) instance;
      Assert.That (compoundInstance.InnerObjects, Is.Not.Empty);

      Assert.That (compoundInstance.SingleService, Is.TypeOf<SingleService>());

      Assert.That (compoundInstance.MultipleService, Is.Not.Empty);
      Assert.That (compoundInstance.MultipleService, Has.All.TypeOf<MultipleService>());
    }

    [Test]
    public void GetInstance_DecoratorWithAdditionalConstructorParameters_PerformsIndirectResolutionCalls ()
    {
      var serviceConfigurationEntry = CreateDecoratorServiceConfigurationEntry (
          typeof (ITestType),
          new[] { typeof (TestDecoratorWithAdditionalConstructorParameters) },
          typeof (TestImplementation1));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register (serviceConfigurationEntry);
      serviceLocator.Register (CreateSingleService());
      serviceLocator.Register (CreateMultipleService());

      var instance = serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance, Is.TypeOf<TestDecoratorWithAdditionalConstructorParameters>());
      var decoratorInstance = (TestDecoratorWithAdditionalConstructorParameters) instance;
      Assert.That (decoratorInstance.DecoratedObject, Is.TypeOf<TestImplementation1>());

      Assert.That (decoratorInstance.SingleService, Is.TypeOf<SingleService>());

      Assert.That (decoratorInstance.MultipleService, Is.Not.Empty);
      Assert.That (decoratorInstance.MultipleService, Has.All.TypeOf<MultipleService>());
    }
  }
}