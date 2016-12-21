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
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.Implementation.TestDomain;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class InvolvedTypeProviderTest
  {
    private IInvolvedTypeProvider _involvedTypeProvider;

    [SetUp]
    public void SetUp ()
    {
      _involvedTypeProvider = new InvolvedTypeProvider (
          new CompoundValidationTypeFilter (new IValidationTypeFilter[] { new LoadFilteredValidationTypeFilter() }));
    }

    [Test]
    public void GetAffectedType_NoBaseTypesAndNoInterfacesAndMixins ()
    {
      var result = _involvedTypeProvider.GetTypes (typeof (TypeWithoutBaseType)).SelectMany (t => t).ToList();

      Assert.That (result, Is.EqualTo (new[] { typeof (TypeWithoutBaseType) }));
    }

    [Test]
    public void GetAffectedType_WithBaseTypesAndNoInterfacesAndMixins ()
    {
      var result = _involvedTypeProvider.GetTypes (typeof (TypeWithSeveralBaseTypes)).SelectMany (t => t).ToList();

      Assert.That (result, Is.EqualTo (new[] { typeof (BaseType2), typeof (BaseType1), typeof (TypeWithSeveralBaseTypes) }));
    }

    [Test]
    public void GetAffectedType_WithOneInterfacesAndNoBaseTypesAndMixins ()
    {
      var result = _involvedTypeProvider.GetTypes (typeof (TypeWithOneInterface)).SelectMany (t => t).ToList();

      Assert.That (result, Is.EqualTo (new[] { typeof (ITypeWithOneInterface), typeof (TypeWithOneInterface) }));
    }

    [Test]
    public void GetAffectedType_WithSeveralInterfacesAndNoBaseTypesAndMixins ()
    {
      var result = _involvedTypeProvider.GetTypes (typeof (TypeWithSeveralInterfaces)).SelectMany (t => t).ToList();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  typeof (ITypeWithServeralInterfaces1), typeof (ITypeWithSeveralInterfaces2), typeof (ITypeWithSeveralInterfaces3),
                  typeof (TypeWithSeveralInterfaces)
              }));
    }

    [Test]
    public void GetAffectedType_WithSeveralInterfacesHavingBaseInterfacesAndNoBaseTypesAndMixins ()
    {
      var result =
          _involvedTypeProvider.GetTypes (typeof (TypeWithSeveralInterfacesImplementingInterface)).SelectMany (t => t)
              .ToList();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  typeof (IBaseBaseInterface1), typeof (IBaseInterface1), typeof (IBaseInterface2), typeof (IBaseInterface3),
                  typeof (ITypeWithSeveralInterfacesImplementingInterface1), typeof (ITypeWithSeveralInterfacesImplementingInterface2),
                  typeof (ITypeWithSeveralInterfacesImplementingInterface3), typeof (TypeWithSeveralInterfacesImplementingInterface)
              }));
    }

    [Test]
    public void GetAffectedType_WithTwoInterfacesHavingCommonBaseInterface ()
    {
      var result =
          _involvedTypeProvider.GetTypes (typeof (TypeWithTwoInterfacesHavingCommingBaseInterface)).SelectMany (t => t)
              .ToList();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  typeof (ICommonBaseBaseInterface), typeof (ITypeWithTwoInterfacesBaseInterface1),
                  typeof (ITypeWithTwoInterfacesBaseInterface2), typeof (TypeWithTwoInterfacesHavingCommingBaseInterface)
              }));
    }

    [Test]
    public void GetAffectedType_WithBaseTypeAndSeveralInterfacesAndNoMixins ()
    {
      var result =
          _involvedTypeProvider.GetTypes (typeof (TypeWithSeveralInterfacesAndBaseType)).SelectMany (t => t).ToList();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  typeof (ITypeWithServeralInterfaces1), typeof (ITypeWithSeveralInterfaces2), typeof (ITypeWithSeveralInterfaces3),
                  typeof (TypeWithSeveralInterfaces), typeof (ITypeWithSeveralInterfacesAndBaseTypes1),
                  typeof (ITypeWithSeveralInterfacesAndBaseTypes2),
                  typeof (ITypeWithSeveralInterfacesAndBaseTypes3), typeof (TypeWithSeveralInterfacesAndBaseType)
              }));
    }

    [Test]
    [Ignore ("Include reimplemented interfaces (with Mono.Cecil ??)")]
    public void GetAffectedType_WithBaseTypeAndSeveralInterfacesReImplementingInterfaceAndNoMixins ()
    {
      var result =
          _involvedTypeProvider.GetTypes (
              typeof (TypeWithSeveralInterfacesAndBaseTypeReImplementingInterface)).SelectMany (t => t).ToList();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  typeof (ITypeWithServeralInterfaces1), typeof (ITypeWithSeveralInterfaces2), typeof (ITypeWithSeveralInterfaces3),
                  typeof (TypeWithSeveralInterfaces), typeof (ITypeWithServeralInterfaces1), typeof (ITypeWithSeveralInterfacesAndBaseTypes1),
                  typeof (ITypeWithSeveralInterfacesAndBaseTypes2), typeof (ITypeWithSeveralInterfacesAndBaseTypes3),
                  typeof (TypeWithSeveralInterfacesAndBaseTypeReImplementingInterface)
              }));
    }

    [Test]
    public void GetAffectedType_WithoutFilter_TerminatesWhenBaseTypeIsNull ()
    {
      var validationTypeFilterStub = MockRepository.GenerateStub<IValidationTypeFilter>();
      validationTypeFilterStub.Stub (stub => stub.IsValidatableType (Arg<Type>.Is.Anything)).Return (true);

      var involvedTypeProvider = new InvolvedTypeProvider (validationTypeFilterStub);

      var result = involvedTypeProvider.GetTypes (typeof (TypeWithOneInterface)).SelectMany (t => t).ToList();

      Assert.That (result, Is.EqualTo (new[] { typeof(object), typeof (ITypeWithOneInterface), typeof (TypeWithOneInterface) }));
    }
  }
}