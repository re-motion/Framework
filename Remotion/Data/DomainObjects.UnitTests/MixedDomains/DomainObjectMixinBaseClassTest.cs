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
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class DomainObjectMixinBaseClassTest : ClientTransactionBaseTest
  {
    private ClassWithAllDataTypes _loadedClassWithAllDataTypes;
    private ClassWithAllDataTypes _newClassWithAllDataTypes;
    private MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes> _loadedClassWithAllDataTypesMixin;
    private MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes> _newClassWithAllDataTypesMixin;

    public override void SetUp ()
    {
      base.SetUp();
      _loadedClassWithAllDataTypes = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      _newClassWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      _loadedClassWithAllDataTypesMixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>>(_loadedClassWithAllDataTypes);
      _newClassWithAllDataTypesMixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>>(_newClassWithAllDataTypes);
    }

    [Test]
    public void MixinIsApplied ()
    {
      Assert.That(_loadedClassWithAllDataTypesMixin, Is.Not.Null);
      Assert.That(_newClassWithAllDataTypesMixin, Is.Not.Null);
    }

    [Test]
    public void InvalidMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof(ClassWithAllDataTypes)).AddMixins(typeof(MixinWithAccessToDomainObjectProperties<Official>)).EnterScope())
      {
        Assert.That(
            () => TypeFactory.GetConcreteType(typeof(ClassWithAllDataTypes)),
            Throws.InstanceOf<ValidationException>());
      }
    }

    [Test]
    public void This ()
    {
      Assert.That(_loadedClassWithAllDataTypesMixin.Target, Is.SameAs(_loadedClassWithAllDataTypes));
      Assert.That(_newClassWithAllDataTypesMixin.Target, Is.SameAs(_newClassWithAllDataTypes));
    }

    [Test]
    public void ID ()
    {
      Assert.That(_loadedClassWithAllDataTypesMixin.ID, Is.SameAs(_loadedClassWithAllDataTypes.ID));
      Assert.That(_newClassWithAllDataTypesMixin.ID, Is.SameAs(_newClassWithAllDataTypes.ID));
    }

    [Test]
    public void GetPublicDomainObjectType ()
    {
      Assert.That(_loadedClassWithAllDataTypesMixin.GetPublicDomainObjectType(), Is.SameAs(_loadedClassWithAllDataTypes.GetPublicDomainObjectType()));
      Assert.That(_newClassWithAllDataTypesMixin.GetPublicDomainObjectType(), Is.SameAs(_newClassWithAllDataTypes.GetPublicDomainObjectType()));
    }

    [Test]
    public void State ()
    {
      Assert.That(_loadedClassWithAllDataTypesMixin.State, Is.EqualTo(_loadedClassWithAllDataTypes.State));
      Assert.That(_newClassWithAllDataTypesMixin.State, Is.EqualTo(_newClassWithAllDataTypes.State));

      ++_loadedClassWithAllDataTypes.Int32Property;
      Assert.That(_loadedClassWithAllDataTypesMixin.State, Is.EqualTo(_loadedClassWithAllDataTypes.State));

      _loadedClassWithAllDataTypes.Delete();
      Assert.That(_loadedClassWithAllDataTypesMixin.State, Is.EqualTo(_loadedClassWithAllDataTypes.State));
    }

    [Test]
    public void Properties ()
    {
      Assert.That(
          _loadedClassWithAllDataTypesMixin.Properties[typeof(ClassWithAllDataTypes), "StringProperty"].GetValue<string>(),
          Is.EqualTo(_loadedClassWithAllDataTypes.StringProperty));
      Assert.That(
          _newClassWithAllDataTypes.Properties[typeof(ClassWithAllDataTypes), "StringProperty"].GetValue<string>(),
          Is.EqualTo(_newClassWithAllDataTypes.StringProperty));
    }

    [Test]
    public void PropertiesThrowsBeforeOnDomainObjectReferenceInitializingWasCalled ()
    {
      var mixin = new MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>();
      Assert.That(
          () => mixin.Properties,
          Throws.InvalidOperationException.With.Message.EqualTo(
              "This member cannot be used until the OnDomainObjectReferenceInitializing event has been executed."));
    }

    [Test]
    public void OnDomainObjectReferenceInitializing ()
    {
      Assert.That(_loadedClassWithAllDataTypesMixin.OnDomainObjectReferenceInitializingCalled, Is.True);
      Assert.That(_newClassWithAllDataTypesMixin.OnDomainObjectReferenceInitializingCalled, Is.True);
    }

    [Test]
    public void OnDomainObjectCreated ()
    {
      Assert.That(_loadedClassWithAllDataTypesMixin.OnDomainObjectCreatedCalled, Is.False);
      Assert.That(_newClassWithAllDataTypesMixin.OnDomainObjectCreatedCalled, Is.True);
    }

    [Test]
    public void OnDomainObjectLoaded ()
    {
      Assert.That(_loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedCalled, Is.True);
      Assert.That(_loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedLoadMode, Is.EqualTo(LoadMode.WholeDomainObjectInitialized));

      _loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedCalled = false;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        ++_loadedClassWithAllDataTypes.Int32Property;
      }

      Assert.That(_loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedCalled, Is.True);
      Assert.That(_loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedLoadMode, Is.EqualTo(LoadMode.DataContainerLoadedOnly));

      Assert.That(_newClassWithAllDataTypesMixin.OnDomainObjectLoadedCalled, Is.False);
    }
  }
}
