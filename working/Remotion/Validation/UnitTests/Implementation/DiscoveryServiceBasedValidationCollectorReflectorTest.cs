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
using NUnit.Framework;
using Remotion.Validation.Attributes;
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class DiscoveryServiceBasedValidationCollectorReflectorTest
  {
    private ITypeDiscoveryService _typeDescoveryServiceStub;

    public class FakeValidationCollector<T> : ComponentValidationCollector<T>
    {
      public FakeValidationCollector ()
      {
      }
    }

    [SetUp]
    public void SetUp ()
    {
      _typeDescoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService>();
    }

    [Test]
    public void GetComponentValidationCollectors_WithFakeTypeDiscoveryService ()
    {
      var appliedWithAttributeTypes = new[]
                                      {
                                          typeof (IPersonValidationCollector1), typeof (IPersonValidationCollector2),
                                          typeof (PersonValidationCollector1),
                                          typeof (CustomerValidationCollector1), typeof (CustomerValidationCollector2)
                                      };
      _typeDescoveryServiceStub.Stub (stub => stub.GetTypes (typeof (IComponentValidationCollector), true)).Return (appliedWithAttributeTypes);

      var typeCollectorProvider = DiscoveryServiceBasedValidationCollectorReflector.Create (
          _typeDescoveryServiceStub,
          new ClassTypeAwareValidatedTypeResolverDecorator (
              new GenericTypeAwareValidatedTypeResolverDecorator (new NullValidatedTypeResolver())));

      Assert.That (typeCollectorProvider.GetCollectorsForType (typeof (IPerson)), Is.EqualTo (new[] { typeof (IPersonValidationCollector1) }));

      Assert.That (
          typeCollectorProvider.GetCollectorsForType (typeof (Person)),
          Is.EquivalentTo (new[] { typeof (IPersonValidationCollector2), typeof (PersonValidationCollector1) })); //ApplyWithClass(typeof(Person))!

      Assert.That (
          typeCollectorProvider.GetCollectorsForType (typeof (Customer)),
          Is.EqualTo (new[] { typeof (CustomerValidationCollector1), typeof (CustomerValidationCollector2) }));
    }

    [Test]
    public void GetComponentValidationCollectors_InvalidCollectorWithoutGenericArgument ()
    {
      _typeDescoveryServiceStub.Stub (stub => stub.GetTypes (typeof (IComponentValidationCollector), true)).Return (new[] { typeof (Person) });

      var validatedTypeResolverStub = MockRepository.GenerateStub<IValidatedTypeResolver>();
      validatedTypeResolverStub.Stub (stub => stub.GetValidatedType (typeof (Person))).Return (null);

      Assert.That (
          () =>
              DiscoveryServiceBasedValidationCollectorReflector.Create (
                  _typeDescoveryServiceStub,
                  validatedTypeResolverStub).GetCollectorsForType (
                      typeof (IComponentValidationCollector)),
          Throws.InvalidOperationException.And.Message.EqualTo (
              "No validated type could be resolved for collector 'Remotion.Validation.UnitTests.TestDomain.Person'."));
    }

    [Test]
    public void GetComponentValidationCollectors_AbstractAndInterfaceAndOpenGenericCollectorsAndProgrammaticallyCollectorsAreFiltered ()
    {
      var programmaticallyCollectorType = TypeUtility.CreateDynamicTypeWithCustomAttribute (
          typeof (FakeValidationCollector<>).MakeGenericType (typeof (Address)),
          "Remotion.Validation.UnitTests.DynamicInvalidCollector1",
          typeof (ApplyProgrammaticallyAttribute),
          new Type[0],
          new object[0]);

      _typeDescoveryServiceStub.Stub (stub => stub.GetTypes (typeof (IComponentValidationCollector), true))
          .Return (
              new[]
              {
                  typeof (IPerson),
                  typeof (ComponentValidationCollector<>),
                  typeof (FakeValidationCollector<>),
                  programmaticallyCollectorType
              });

      var typeCollectorProvider = DiscoveryServiceBasedValidationCollectorReflector.Create (
          _typeDescoveryServiceStub,
             new ClassTypeAwareValidatedTypeResolverDecorator (
                  new GenericTypeAwareValidatedTypeResolverDecorator (new NullValidatedTypeResolver())));

      var result =
          typeCollectorProvider.GetCollectorsForType (typeof (Person))
              .Concat (typeCollectorProvider.GetCollectorsForType (typeof (Person)))
              .ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetComponentValidationCollectors_GenericTypeNotAssignableFromClassType ()
    {
      var collectorType = TypeUtility.CreateDynamicTypeWithCustomAttribute (
          typeof (FakeValidationCollector<>).MakeGenericType (typeof (Address)),
          "Remotion.Validation.UnitTests.DynamicInvalidCollector2",
          typeof (ApplyWithClassAttribute),
          new[] { typeof (Type) },
          new[] { typeof (IPerson) });

      _typeDescoveryServiceStub.Stub (stub => stub.GetTypes (typeof (IComponentValidationCollector), true)).Return (new[] { collectorType });

      Assert.That (
          () =>
              DiscoveryServiceBasedValidationCollectorReflector.Create (
                  _typeDescoveryServiceStub,
                    new ClassTypeAwareValidatedTypeResolverDecorator (
                          new GenericTypeAwareValidatedTypeResolverDecorator (new NullValidatedTypeResolver()))).GetCollectorsForType (
                              typeof (IComponentValidationCollector)),
          Throws.TypeOf<InvalidOperationException>().And.Message.EqualTo (
              "Invalid 'ApplyWithClassAttribute'-definition for collector 'Remotion.Validation.UnitTests.DynamicInvalidCollector2': "
              + "type 'Remotion.Validation.UnitTests.TestDomain.Address' "
              + "is not assignable from 'Remotion.Validation.UnitTests.TestDomain.IPerson'."));
    }

    [Test]
    public void GetComponentValidationCollectors_WithRemotionDiscoveryService ()
    {
      var typeCollectorProvider = new DiscoveryServiceBasedValidationCollectorReflector (
          new ClassTypeAwareValidatedTypeResolverDecorator (
                  new GenericTypeAwareValidatedTypeResolverDecorator (new NullValidatedTypeResolver())));

      var result = typeCollectorProvider.GetCollectorsForType (typeof (Person)).ToArray();

      Assert.That (result.Count(), Is.EqualTo (2));
      Assert.That (
          result,
          Is.EquivalentTo (
              new[]
              {
                  typeof (IPersonValidationCollector2), typeof (PersonValidationCollector1)
              }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "No validated type could be resolved for collector 'Remotion.Validation.UnitTests.TestDomain.Collectors.CustomerValidationCollector1'.")]
    public void GetComponentValidationCollectors_NoValidatedTypeFound_ExceptionIsThrown ()
    {
      _typeDescoveryServiceStub.Stub (stub => stub.GetTypes (typeof (IComponentValidationCollector), true))
          .Return (new[] { typeof (CustomerValidationCollector1) });

      var validatedTypeResolverStub = MockRepository.GenerateStub<IValidatedTypeResolver>();
      validatedTypeResolverStub.Stub (stub => stub.GetValidatedType (typeof (CustomerValidationCollector1))).Return (null);

      var typeCollectorProvider = DiscoveryServiceBasedValidationCollectorReflector.Create (_typeDescoveryServiceStub, validatedTypeResolverStub);

      typeCollectorProvider.GetCollectorsForType (typeof (Customer));
    }
  }
}