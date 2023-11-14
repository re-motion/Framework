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
using Remotion.Development.Mixins.UnitTesting;
using Remotion.Development.Moq.UnitTesting;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.Serialization
{
  [TestFixture]
  public class SerializationCallbackTest : CodeGenerationBaseTest
  {
    private TargetTypeWithSerializationCallbacks _instance;

    public override void SetUp ()
    {
      base.SetUp();
      _instance = ObjectFactory.Create<TargetTypeWithSerializationCallbacks>(ParamList.Empty);
    }

    [Test]
    public void SerializationCallbacks_AreInvokedOnTargetClass ()
    {
      new SerializationCallbackTester<TargetTypeWithSerializationCallbacks>(_instance, TargetTypeWithSerializationCallbacks.SetStaticReceiver)
          .Test_SerializationCallbacks();
    }

    [Test]
    public void DeserializationCallbacks_AreInvokedOnTargetClass ()
    {
      new SerializationCallbackTester<TargetTypeWithSerializationCallbacks>(_instance, TargetTypeWithSerializationCallbacks.SetStaticReceiver)
          .Test_DeserializationCallbacks();
    }

    [Test]
    public void SerializationCallbacks_AreInvokedOnMixinClass ()
    {
      new SerializationCallbackTester<TargetTypeWithSerializationCallbacks>(_instance, MixinWithSerializationCallbacks.SetStaticReceiver)
          .Test_SerializationCallbacks();
    }

    [Test]
    public void DeserializationCallbacks_AreInvokedOnMixinClass ()
    {
      new SerializationCallbackTester<TargetTypeWithSerializationCallbacks>(_instance, MixinWithSerializationCallbacks.SetStaticReceiver)
          .Test_DeserializationCallbacks();
    }

    [Uses(typeof(AbstractMixinWithSerializationCallbacks))]
    [Serializable]
    public class TargetClassForAbstractMixinWithSerializationCallbacks
    {
    }

    [Test]
    public void SerializationCallbacks_AreInvokedOnGeneratedMixinClass ()
    {
      TargetClassForAbstractMixinWithSerializationCallbacks instance =
          ObjectFactory.Create<TargetClassForAbstractMixinWithSerializationCallbacks>(ParamList.Empty);
      new SerializationCallbackTester<TargetClassForAbstractMixinWithSerializationCallbacks>(instance, AbstractMixinWithSerializationCallbacks.SetStaticReceiver)
          .Test_SerializationCallbacks();
    }

    [Test]
    public void DeserializationCallbacks_AreInvokedOnGeneratedMixinClass ()
    {
      TargetClassForAbstractMixinWithSerializationCallbacks instance =
          ObjectFactory.Create<TargetClassForAbstractMixinWithSerializationCallbacks>(ParamList.Empty);
      new SerializationCallbackTester<TargetClassForAbstractMixinWithSerializationCallbacks>(instance, AbstractMixinWithSerializationCallbacks.SetStaticReceiver)
          .Test_DeserializationCallbacks();
    }
  }
}
