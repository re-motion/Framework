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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.Serialization
{
  [TestFixture]
  public class SerializationInfoConcreteMixinTypeIdentifierSerializerTest
  {
    private MethodInfo _simpleMethod;
    private MethodInfo _genericMethod;

    private SerializationInfo _serializationInfo;
    private SerializationInfoConcreteMixinTypeIdentifierSerializer _serializer;

    [SetUp]
    public void SetUp ()
    {
      _simpleMethod = typeof(BaseType1).GetMethod("VirtualMethod", Type.EmptyTypes);
      _genericMethod = typeof(BaseType7).GetMethod("One");

#pragma warning disable SYSLIB0050
      _serializationInfo = new SerializationInfo(typeof(ConcreteMixinTypeIdentifier), new FormatterConverter());
#pragma warning restore SYSLIB0050
      _serializer = new SerializationInfoConcreteMixinTypeIdentifierSerializer(_serializationInfo, "identifier");
    }

    [Test]
    public void AddMixinType ()
    {
      _serializer.AddMixinType(typeof(BT1Mixin1));
      Assert.That(_serializationInfo.GetString("identifier.MixinType"), Is.EqualTo(typeof(BT1Mixin1).AssemblyQualifiedName));
    }

    [Test]
    public void AddOverriders ()
    {
      _serializer.AddOverriders(new HashSet<MethodInfo> { _simpleMethod });

      Assert.That(_serializationInfo.GetInt32("identifier.Overriders.Count"), Is.EqualTo(1));
      Assert.That(_serializationInfo.GetString("identifier.Overriders[0].DeclaringType"), Is.EqualTo(typeof(BaseType1).AssemblyQualifiedName));
      Assert.That(_serializationInfo.GetString("identifier.Overriders[0].Name"), Is.EqualTo("VirtualMethod"));
      Assert.That(_serializationInfo.GetString("identifier.Overriders[0].Signature"), Is.EqualTo("System.String VirtualMethod()"));
    }

    [Test]
    public void AddOverriders_ClosedGeneric ()
    {
      Assert.That(
          () => _serializer.AddOverriders(new HashSet<MethodInfo> { _genericMethod.MakeGenericMethod(typeof(int)) }),
          Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void AddOverridden ()
    {
      _serializer.AddOverridden(new HashSet<MethodInfo> { _simpleMethod });

      Assert.That(_serializationInfo.GetInt32("identifier.Overridden.Count"), Is.EqualTo(1));
      Assert.That(_serializationInfo.GetString("identifier.Overridden[0].DeclaringType"), Is.EqualTo(typeof(BaseType1).AssemblyQualifiedName));
      Assert.That(_serializationInfo.GetString("identifier.Overridden[0].Name"), Is.EqualTo("VirtualMethod"));
      Assert.That(_serializationInfo.GetString("identifier.Overridden[0].Signature"), Is.EqualTo("System.String VirtualMethod()"));
    }

    [Test]
    public void AddOverridden_ClosedGeneric ()
    {
      Assert.That(
          () => _serializer.AddOverridden(new HashSet<MethodInfo> { _genericMethod.MakeGenericMethod(typeof(int)) }),
          Throws.InstanceOf<NotSupportedException>());
    }
  }
}
