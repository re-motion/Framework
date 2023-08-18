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
#if NETFRAMEWORK
using System.Runtime.Serialization;
#else
using System.Runtime.CompilerServices;
#endif
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe.Implementation;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  public class TypeFactoryTest
  {
    [Test]
    public void GetConcreteType_NoTypeGeneratedIfNoConfig ()
    {
      Assert.That(MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance(typeof(object)), Is.False);
      Assert.That(TypeFactory.GetConcreteType(typeof(object)), Is.SameAs(typeof(object)));
    }

    [Test]
    public void GetConcreteType_NoTypeGenerated_IfGeneratedTypeIsGiven ()
    {
      Type concreteType = TypeFactory.GetConcreteType(typeof(BaseType1));
      Assert.That(TypeFactory.GetConcreteType(concreteType), Is.SameAs(concreteType));
    }

    [Test]
    public void InitializeUnconstructedInstance_ConstructionSemantics ()
    {
      var concreteType = TypeFactory.GetConcreteType(typeof(BaseType3));
#if NETFRAMEWORK
      var target = (BaseType3)FormatterServices.GetSafeUninitializedObject(concreteType);
#else
      var target = (BaseType3)RuntimeHelpers.GetUninitializedObject(concreteType);
#endif

// ReSharper disable SuspiciousTypeConversion.Global
      TypeFactory.InitializeUnconstructedInstance(target as IMixinTarget, InitializationSemantics.Construction);
// ReSharper restore SuspiciousTypeConversion.Global

      var mixin = Mixin.Get<BT3Mixin1>(target);
      Assert.That(mixin, Is.Not.Null, "Mixin must have been created");
      Assert.That(mixin.Target, Is.SameAs(target), "Mixin must have been initialized");
    }

    [Test]
    public void InitializeUnconstructedInstance_DeserializationSemantics ()
    {
      var concreteType = TypeFactory.GetConcreteType(typeof(TargetType));
#if NETFRAMEWORK
      var target = FormatterServices.GetSafeUninitializedObject(concreteType);
#else
      var target = RuntimeHelpers.GetUninitializedObject(concreteType);
#endif
      // Simulate a deserialzed instance.
      var mixins = new object[] { new DeserializationMixin() };
      PrivateInvoke.SetNonPublicField(target, "__extensions", mixins);

      TypeFactory.InitializeUnconstructedInstance(target as IMixinTarget, InitializationSemantics.Deserialization);

      var mixin = Mixin.Get<DeserializationMixin>(target);
      Assert.That(mixin, Is.SameAs(mixins[0]));
      Assert.That(mixin.Target, Is.SameAs(target), "Mixin must have been initialized");
    }

    [Uses(typeof(DeserializationMixin))]
    [Serializable]
    public class TargetType { }

    [Serializable]
    public class DeserializationMixin : Mixin<object>
    {
      public new object Target
      {
        get { return base.Target; }
      }
    }
  }
}
