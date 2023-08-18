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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.Serialization
{
  [TestFixture]
  public class SerializationInfoConcreteMixinTypeIdentifierDeserializerTest
  {
    private MethodInfo _simpleMethod;
    private MethodInfo _genericMethod;
    private MethodInfo _methodOnGenericClosedWithReferenceType;
    private MethodInfo _methodOnGenericClosedWithValueType;

    private SerializationInfoConcreteMixinTypeIdentifierSerializer _serializer;
    private SerializationInfoConcreteMixinTypeIdentifierDeserializer _deserializer;
    private SerializationInfo _serializationInfo;

    [SetUp]
    public void SetUp ()
    {
      _simpleMethod = typeof(BaseType1).GetMethod("VirtualMethod", Type.EmptyTypes);
      _genericMethod = typeof(BaseType7).GetMethod("One");
      _methodOnGenericClosedWithReferenceType = typeof(GenericClassWithAllKindsOfMembers<string>).GetMethod("Method");
      _methodOnGenericClosedWithValueType = typeof(GenericClassWithAllKindsOfMembers<int>).GetMethod("Method");

#pragma warning disable SYSLIB0050
      _serializationInfo = new SerializationInfo(typeof(ConcreteMixinTypeIdentifier), new FormatterConverter());
#pragma warning restore SYSLIB0050
      _serializer = new SerializationInfoConcreteMixinTypeIdentifierSerializer(_serializationInfo, "identifier");
      _deserializer = new SerializationInfoConcreteMixinTypeIdentifierDeserializer(_serializationInfo, "identifier");
    }

    [Test]
    public void GetMixinType ()
    {
      _serializer.AddMixinType(typeof(BT1Mixin1));
      Assert.That(_deserializer.GetMixinType(), Is.SameAs(typeof(BT1Mixin1)));
    }

    [Test]
    public void GetOverriders ()
    {
      _serializer.AddOverriders(new HashSet<MethodInfo> { _simpleMethod });
      Assert.That(_deserializer.GetOverriders().ToArray(), Is.EquivalentTo(new[] { _simpleMethod }));
    }

    [Test]
    public void GetOverriders_GenericMethod ()
    {
      _serializer.AddOverriders(new HashSet<MethodInfo> { _genericMethod });
      Assert.That(_deserializer.GetOverriders().ToArray(), Is.EquivalentTo(new[] { _genericMethod }));
    }

    [Test]
    public void GetOverriders_MethodOnClosedGenericType_ReferenceType ()
    {
      _serializer.AddOverriders(new HashSet<MethodInfo> { _methodOnGenericClosedWithReferenceType });
      Assert.That(_deserializer.GetOverriders().ToArray(), Is.EquivalentTo(new[] { _methodOnGenericClosedWithReferenceType }));
    }

    [Test]
    public void GetOverriders_MethodOnClosedGenericType_ValueType ()
    {
      _serializer.AddOverriders(new HashSet<MethodInfo> { _methodOnGenericClosedWithValueType });
      Assert.That(_deserializer.GetOverriders().ToArray(), Is.EquivalentTo(new[] { _methodOnGenericClosedWithValueType }));
    }

    [Test]
    public void GetOverridden ()
    {
      _serializer.AddOverridden(new HashSet<MethodInfo> { _simpleMethod });
      Assert.That(_deserializer.GetOverridden().ToArray(), Is.EquivalentTo(new[] { _simpleMethod }));
    }
  }
}
