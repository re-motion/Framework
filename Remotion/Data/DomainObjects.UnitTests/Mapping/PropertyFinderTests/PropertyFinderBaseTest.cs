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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.MixinTestDomain;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class PropertyFinderBaseTest : PropertyFinderBaseTestBase
  {
    private IPersistentMixinFinder _persistentMixinFinderStub;

    [SetUp]
    public void SetUp ()
    {
      _persistentMixinFinderStub = MockRepository.GenerateStub<IPersistentMixinFinder>();
    }

    [Test]
    public void Initialize ()
    {
      bool includeBaseProperties = BooleanObjectMother.GetRandomBoolean();
      bool includeMixinProperties = BooleanObjectMother.GetRandomBoolean ();
      var propertyFinder = new StubPropertyFinderBase (
          typeof (ClassWithDifferentProperties), includeBaseProperties, includeMixinProperties, _persistentMixinFinderStub);

      Assert.That (propertyFinder.Type, Is.SameAs (typeof (ClassWithDifferentProperties)));
      Assert.That (propertyFinder.IncludeBaseProperties, Is.EqualTo (includeBaseProperties));
      Assert.That (propertyFinder.IncludeMixinProperties, Is.EqualTo (includeMixinProperties));
    }

    [Test]
    public void FindPropertyInfos_ForInheritanceRoot ()
    {
      var propertyFinder = new StubPropertyFinderBase (typeof (ClassWithDifferentProperties), true, false, _persistentMixinFinderStub);

      Assert.That (
          propertyFinder.FindPropertyInfos (),
          Is.EquivalentTo (
              new[]
                  {
                      GetProperty (typeof (ClassWithDifferentPropertiesNotInMapping), "BaseString"),
                      GetProperty (typeof (ClassWithDifferentPropertiesNotInMapping), "BaseUnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithDifferentPropertiesNotInMapping), "BasePrivateUnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithDifferentProperties), "Int32"),
                      GetProperty (typeof (ClassWithDifferentProperties), "String"),
                      GetProperty (typeof (ClassWithDifferentProperties), "UnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithDifferentProperties), "PrivateString")
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForDerivedClass ()
    {
      var propertyFinder = new StubPropertyFinderBase (typeof (ClassWithDifferentProperties), false, false, _persistentMixinFinderStub);

      Assert.That (
          propertyFinder.FindPropertyInfos (),
          Is.EquivalentTo (
              new[]
                  {
                      GetProperty (typeof (ClassWithDifferentProperties), "Int32"),
                      GetProperty (typeof (ClassWithDifferentProperties), "String"),
                      GetProperty (typeof (ClassWithDifferentProperties), "UnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithDifferentProperties), "PrivateString")
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForClassWithInterface ()
    {
      var propertyFinder = new StubPropertyFinderBase (typeof (ClassWithInterface), false, false, _persistentMixinFinderStub);

      Assert.That (
          propertyFinder.FindPropertyInfos (),
          Is.EquivalentTo (
              new[]
                  {
                      GetProperty (typeof (ClassWithInterface), "Property"),
                      GetProperty (typeof (ClassWithInterface), "ImplicitProperty"),
                      GetProperty (typeof (ClassWithInterface), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.IInterfaceWithProperties.ExplicitManagedProperty")
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForDomainObjectType ()
    {
      var propertyFinder = new StubPropertyFinderBase (typeof (DomainObject), true, false, _persistentMixinFinderStub);

      Assert.That (propertyFinder.FindPropertyInfos (), Is.Not.Empty.And.Member (PropertyInfoAdapter.Create (typeof (DomainObject).GetProperty ("ID"))));
    }

    [Test]
    public void FindPropertyInfos_ForObjectType ()
    {
      var propertyFinder = new StubPropertyFinderBase (typeof (object), true, false, _persistentMixinFinderStub);

      Assert.That (propertyFinder.FindPropertyInfos (), Is.Empty);
    }

    [Test]
    public void FindPropertyInfos_ForNonDomainObject ()
    {
      var propertyFinder = new StubPropertyFinderBase (typeof (int), true, false, _persistentMixinFinderStub);

      Assert.That (propertyFinder.FindPropertyInfos (), Is.Empty);
    }

    [Test]
    public void FindPropertyInfos_WithMixins_ForInheritanceRoot ()
    {
      var persistentMixinFinder = new PersistentMixinFinder (typeof (TargetClassA));
      var propertyFinder = new StubPropertyFinderBase (typeof (TargetClassA), true, true, persistentMixinFinder);

      Assert.That (
          propertyFinder.FindPropertyInfos (),
          Is.EquivalentTo (
              new[]
                  {
                      GetProperty (typeof (TargetClassBase), "P0"),
                      GetProperty (typeof (MixinBase), "P0a"),
                      GetProperty (typeof (TargetClassA), "P1"),
                      GetProperty (typeof (TargetClassA), "P2"),
                      GetProperty (typeof (MixinA), "P5"),
                      GetProperty (typeof (MixinC), "P7"),
                      GetProperty (typeof (MixinD), "P8"),
                  }));
    }

    [Test]
    public void FindPropertyInfos_WithMixins_ForNonInheritanceRoot ()
    {
      var persistentMixinFinder = new PersistentMixinFinder (typeof (TargetClassA));
      var propertyFinder = new StubPropertyFinderBase (typeof (TargetClassA), false, true, persistentMixinFinder);

      Assert.That (
          propertyFinder.FindPropertyInfos (),
          Is.EquivalentTo (
              new[]
                  {
                      GetProperty (typeof (TargetClassA), "P1"),
                      GetProperty (typeof (TargetClassA), "P2"),
                      GetProperty (typeof (MixinA), "P5"),
                      GetProperty (typeof (MixinC), "P7"),
                      GetProperty (typeof (MixinD), "P8"),
                  }));
    }

    [Test]
    public void FindPropertyInfos_WithMixins_ForDerived ()
    {
      var persistentMixinFinder = new PersistentMixinFinder (typeof (TargetClassB));
      var propertyFinder = new StubPropertyFinderBase (typeof (TargetClassB), false, true, persistentMixinFinder);

      Assert.That (
          propertyFinder.FindPropertyInfos (),
          Is.EquivalentTo (
              new[]
                  {
                      GetProperty (typeof (TargetClassB), "P3"),
                      GetProperty (typeof (TargetClassB), "P4"),
                      GetProperty (typeof (MixinB), "P6"),
                      GetProperty (typeof (MixinE), "P9"),
                  }));
    }
    
  }
}
