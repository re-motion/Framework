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
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  public class ComposedObjectTest
  {
    [Test]
    public void Ctor ()
    {
      var instance = ObjectFactory.Create<ClassDerivedFromComposedObject>(ParamList.Empty);

      Assert.That(instance, Is.InstanceOf(typeof(ClassDerivedFromComposedObject)));
      Assert.That(instance, Is.InstanceOf(typeof(ClassDerivedFromComposedObject.IClassDerivedFromComposedObject)));
      Assert.That(Mixin.Get<ClassDerivedFromComposedObject.Mixin1>(instance), Is.Not.Null);
    }

    [Test]
    public void Ctor_ChecksObjectFactoryCreateUsed ()
    {
      Assert.That(
          () => new ClassDerivedFromComposedObject(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Type 'Remotion.Mixins.UnitTests.Core.TestDomain.ClassDerivedFromComposedObject' is not associated with the composed interface "
                  + "'IClassDerivedFromComposedObject'. You should instantiate the class via the ObjectFactory class or the NewObject method. If you manually "
                  + "created a mixin configuration, don't forget to add the composed interface."));
    }

    [Test]
    public void Ctor_ChecksComposedInterfaceAvailable ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        Assert.That(
            () => ObjectFactory.Create<ClassDerivedFromComposedObject>(ParamList.Empty),
            Throws.InvalidOperationException
                .With.Message.EqualTo(
                    "Type 'Remotion.Mixins.UnitTests.Core.TestDomain.ClassDerivedFromComposedObject' is not associated with the composed interface "
                    + "'IClassDerivedFromComposedObject'. You should instantiate the class via the ObjectFactory class or the NewObject method. If you manually "
                    + "created a mixin configuration, don't forget to add the composed interface."));
      }
    }

    [Test]
    public void NewObject ()
    {
      ClassDerivedFromComposedObject.IClassDerivedFromComposedObject instance = ClassDerivedFromComposedObject.NewObject();

      Assert.That(instance, Is.InstanceOf(typeof(ClassDerivedFromComposedObject)));
      Assert.That(Mixin.Get<ClassDerivedFromComposedObject.Mixin1>(instance), Is.Not.Null);
    }

    [Test]
    public void This ()
    {
      var instance = ObjectFactory.Create<ClassDerivedFromComposedObject>(ParamList.Empty);

      var result = instance.This;

      Assert.That(result, Is.SameAs(instance));
      Assert.That(result, Is.InstanceOf(typeof(ClassDerivedFromComposedObject.IClassDerivedFromComposedObject)));
    }

    [Test]
    public void IntegrationTest ()
    {
      var instance = ClassDerivedFromComposedObject.NewObject();

      Assert.That(instance.MT(), Is.EqualTo("ClassDerivedFromComposedObject.MT"));
      Assert.That(instance.M1(), Is.EqualTo("Mixin1.M1"));
      Assert.That(instance.M2(), Is.EqualTo("Mixin2.M2"));
    }

  }
}
