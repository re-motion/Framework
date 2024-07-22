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
using System.ComponentModel.Design;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.Validation.Attributes;
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class DiscoveryServiceBasedValidationRuleCollectorReflectorTest
  {
    private Mock<ITypeDiscoveryService> _typeDiscoveryServiceStub;

    public class FakeValidationRuleCollector<T> : ValidationRuleCollectorBase<T>
    {
      public FakeValidationRuleCollector ()
      {
      }
    }

    [SetUp]
    public void SetUp ()
    {
      _typeDiscoveryServiceStub = new Mock<ITypeDiscoveryService>();
    }

    [Test]
    public void GetValidationRuleCollectors_WithFakeTypeDiscoveryService ()
    {
      var appliedWithAttributeTypes = new[]
                                      {
                                          typeof(IPersonValidationRuleCollector1), typeof(PersonValidationRuleCollector2),
                                          typeof(PersonValidationRuleCollector1),
                                          typeof(CustomerValidationRuleCollector1), typeof(CustomerValidationRuleCollector2)
                                      };
      _typeDiscoveryServiceStub.Setup(stub => stub.GetTypes(typeof(IValidationRuleCollector), false)).Returns(appliedWithAttributeTypes);

      var typeCollectorProvider = new DiscoveryServiceBasedValidationRuleCollectorReflector(
          _typeDiscoveryServiceStub.Object,
          new ClassTypeAwareValidatedTypeResolverDecorator(
              new GenericTypeAwareValidatedTypeResolverDecorator(new NullValidatedTypeResolver())));

      Assert.That(typeCollectorProvider.GetCollectorsForType(typeof(IPerson)), Is.EqualTo(new[] { typeof(IPersonValidationRuleCollector1) }));

      Assert.That(
          typeCollectorProvider.GetCollectorsForType(typeof(Person)),
          Is.EquivalentTo(new[] { typeof(PersonValidationRuleCollector2), typeof(PersonValidationRuleCollector1) })); //ApplyWithClass(typeof(Person))!

      Assert.That(
          typeCollectorProvider.GetCollectorsForType(typeof(Customer)),
          Is.EqualTo(new[] { typeof(CustomerValidationRuleCollector1), typeof(CustomerValidationRuleCollector2) }));
    }

    [Test]
    public void GetValidationRuleCollectors_InvalidCollectorWithoutGenericArgument ()
    {
      _typeDiscoveryServiceStub.Setup(stub => stub.GetTypes(typeof(IValidationRuleCollector), false)).Returns(new[] { typeof(Person) });

      var validatedTypeResolverStub = new Mock<IValidatedTypeResolver>();
      validatedTypeResolverStub.Setup(stub => stub.GetValidatedType(typeof(Person))).Returns((Type)null);

      Assert.That(
          () =>
              new DiscoveryServiceBasedValidationRuleCollectorReflector(
                  _typeDiscoveryServiceStub.Object,
                  validatedTypeResolverStub.Object).GetCollectorsForType(
                      typeof(IValidationRuleCollector)),
          Throws.InvalidOperationException.And.Message.EqualTo(
              "No validated type could be resolved for collector 'Remotion.Validation.UnitTests.TestDomain.Person'."));
    }

    [Test]
    public void GetValidationRuleCollectors_AbstractAndInterfaceAndOpenGenericCollectorsAndProgrammaticallyCollectorsAreFiltered ()
    {
      var programmaticallyCollectorType = TypeUtility.CreateDynamicTypeWithCustomAttribute(
          typeof(FakeValidationRuleCollector<>).MakeGenericType(typeof(Address)),
          "Remotion.Validation.UnitTests.DynamicInvalidCollector1",
          typeof(ApplyProgrammaticallyAttribute),
          new Type[0],
          new object[0]);

      _typeDiscoveryServiceStub
          .Setup(stub => stub.GetTypes(typeof(IValidationRuleCollector), false))
          .Returns(
              new[]
              {
                  typeof(IPerson),
                  typeof(ValidationRuleCollectorBase<>),
                  typeof(FakeValidationRuleCollector<>),
                  programmaticallyCollectorType
              });

      var typeCollectorProvider = new DiscoveryServiceBasedValidationRuleCollectorReflector(
          _typeDiscoveryServiceStub.Object,
             new ClassTypeAwareValidatedTypeResolverDecorator(
                  new GenericTypeAwareValidatedTypeResolverDecorator(new NullValidatedTypeResolver())));

      var result =
          typeCollectorProvider.GetCollectorsForType(typeof(Person))
              .Concat(typeCollectorProvider.GetCollectorsForType(typeof(Person)))
              .ToArray();

      Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void GetValidationRuleCollectors_GenericTypeNotAssignableFromClassType ()
    {
      var collectorType = TypeUtility.CreateDynamicTypeWithCustomAttribute(
          typeof(FakeValidationRuleCollector<>).MakeGenericType(typeof(Address)),
          "Remotion.Validation.UnitTests.DynamicInvalidCollector2",
          typeof(ApplyWithClassAttribute),
          new[] { typeof(Type) },
          new[] { typeof(IPerson) });

      _typeDiscoveryServiceStub.Setup(stub => stub.GetTypes(typeof(IValidationRuleCollector), false)).Returns(new[] { collectorType });

      Assert.That(
          () =>
              new DiscoveryServiceBasedValidationRuleCollectorReflector(
                  _typeDiscoveryServiceStub.Object,
                    new ClassTypeAwareValidatedTypeResolverDecorator(
                          new GenericTypeAwareValidatedTypeResolverDecorator(new NullValidatedTypeResolver()))).GetCollectorsForType(
                              typeof(IValidationRuleCollector)),
          Throws.TypeOf<InvalidOperationException>().And.Message.EqualTo(
              "Invalid 'ApplyWithClassAttribute'-definition for collector 'Remotion.Validation.UnitTests.DynamicInvalidCollector2': "
              + "type 'Remotion.Validation.UnitTests.TestDomain.Address' "
              + "is not assignable from 'Remotion.Validation.UnitTests.TestDomain.IPerson'."));
    }

    [Test]
    public void GetValidationRuleCollectors_WithRemotionDiscoveryService ()
    {
      var typeCollectorProvider = new DiscoveryServiceBasedValidationRuleCollectorReflector(
          SafeServiceLocator.Current.GetInstance<ITypeDiscoveryService>(),
          new ClassTypeAwareValidatedTypeResolverDecorator(
                  new GenericTypeAwareValidatedTypeResolverDecorator(new NullValidatedTypeResolver())));

      var result = typeCollectorProvider.GetCollectorsForType(typeof(Person)).ToArray();

      Assert.That(result.Count(), Is.EqualTo(2));
      Assert.That(
          result,
          Is.EquivalentTo(
              new[]
              {
                  typeof(PersonValidationRuleCollector2), typeof(PersonValidationRuleCollector1)
              }));
    }

    [Test]
    public void GetValidationRuleCollectors_NoValidatedTypeFound_ExceptionIsThrown ()
    {
      _typeDiscoveryServiceStub
          .Setup(stub => stub.GetTypes(typeof(IValidationRuleCollector), false))
          .Returns(new[] { typeof(CustomerValidationRuleCollector1) });

      var validatedTypeResolverStub = new Mock<IValidatedTypeResolver>();
      validatedTypeResolverStub.Setup(stub => stub.GetValidatedType(typeof(CustomerValidationRuleCollector1))).Returns((Type)null);

      var typeCollectorProvider = new DiscoveryServiceBasedValidationRuleCollectorReflector(_typeDiscoveryServiceStub.Object, validatedTypeResolverStub.Object);
      Assert.That(
          () => typeCollectorProvider.GetCollectorsForType(typeof(Customer)),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "No validated type could be resolved for collector 'Remotion.Validation.UnitTests.TestDomain.Collectors.CustomerValidationRuleCollector1'."));
    }
  }
}
