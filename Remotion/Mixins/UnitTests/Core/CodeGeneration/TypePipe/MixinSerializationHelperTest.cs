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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.ServiceLocation;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe
{
  [TestFixture]
  public class MixinSerializationHelperTest
  {
    private SerializationInfo _serializationInfo;
    private StreamingContext _context;
    private FakeConcreteMixinType _concreteMixin;
    private ConcreteMixinTypeIdentifier _identifier;

    private IPipeline _pipeline;

    [SetUp]
    public void SetUp ()
    {
#pragma warning disable SYSLIB0050
      _serializationInfo = new SerializationInfo(typeof(object), new FormatterConverter());
#pragma warning restore SYSLIB0050
      _context = new StreamingContext();
      _concreteMixin = new FakeConcreteMixinType();
      var classContext = ClassContextObjectMother.Create(typeof(ClassOverridingMixinMembers), typeof(FakeConcreteMixinType));
      _identifier = DefinitionObjectMother.GetTargetClassDefinition(classContext).Mixins[0].GetConcreteMixinTypeIdentifier();

      _pipeline = SafeServiceLocator.Current.GetInstance<IPipelineFactory>()
          .Create(
          "MixinSerializationHelper",
          new MixinParticipant(
              SafeServiceLocator.Current.GetInstance<IConfigurationProvider>(),
              SafeServiceLocator.Current.GetInstance<IMixinTypeProvider>(),
              SafeServiceLocator.Current.GetInstance<ITargetTypeModifier>(),
              SafeServiceLocator.Current.GetInstance<IConcreteTypeMetadataImporter>()));
      SafeServiceLocator.Current.GetInstance<IPipelineRegistry>().Register(_pipeline);
    }

    [TearDown]
    public void TearDown ()
    {
      SafeServiceLocator.Current.GetInstance<IPipelineRegistry>().Unregister(_pipeline.ParticipantConfigurationID);
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SetsType ()
    {
      CallGetObjectDataForGeneratedTypes(true);

      Assert.That(_serializationInfo.FullTypeName, Is.EqualTo(typeof(MixinSerializationHelper).FullName));
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SerializesIdentifier ()
    {
      CallGetObjectDataForGeneratedTypes(true);
      var identifierDeserializer = new SerializationInfoConcreteMixinTypeIdentifierDeserializer(_serializationInfo, "__identifier");

      var deserializedIdentifier = ConcreteMixinTypeIdentifier.Deserialize(identifierDeserializer);
      Assert.That(deserializedIdentifier, Is.EqualTo(_identifier));
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SerializesParticipantConfigurationID ()
    {
      CallGetObjectDataForGeneratedTypes(true);

      var deserializedIdentifier = _serializationInfo.GetString("__participantConfigurationID");
      Assert.That(deserializedIdentifier, Is.EqualTo(_pipeline.ParticipantConfigurationID));
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SerializeBaseMembers_True ()
    {
      _concreteMixin.I = 7431;
      CallGetObjectDataForGeneratedTypes(true);
      Assert.That(_serializationInfo.GetValue("__baseMemberValues", typeof(object[])), Has.Member(7431));
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SerializeBaseMembers_False ()
    {
      CallGetObjectDataForGeneratedTypes(false);
      Assert.That(_serializationInfo.GetValue("__baseMemberValues", typeof(object[])), Is.Null);
    }

    [Test]
    public void Initialization_SignalsOnDeserializing ()
    {
      CallGetObjectDataForGeneratedTypes(true);

      var helper = new MixinSerializationHelper(_serializationInfo, _context);
      var deserializedObject = (FakeConcreteMixinType)helper.GetRealObject(_context);

      Assert.That(deserializedObject.OnDeserializingCalled, Is.True);
    }

    [Test]
    public void Initialization_BaseMembersSerialized_CallsNoCtor ()
    {
      CallGetObjectDataForGeneratedTypes(true);

      var helper = new MixinSerializationHelper(_serializationInfo, _context);
      var deserializedObject = (FakeConcreteMixinType)helper.GetRealObject(_context);

      Assert.That(deserializedObject.CtorCalled, Is.False);
    }

    [Test]
    public void Initialization_BaseMembersNotSerialized_CallsSerializationCtor ()
    {
      CallGetObjectDataForGeneratedTypes(false);

      var helper = new MixinSerializationHelper(_serializationInfo, _context);
      var deserializedObject = (FakeConcreteMixinType)helper.GetRealObject(_context);

      Assert.That(deserializedObject.CtorCalled, Is.True);
      Assert.That(deserializedObject.SerializationCtorCalled, Is.True);
    }

    [Test]
    public void GetRealObject_ReturnsConcreteMixinTypeInstance ()
    {
      CallGetObjectDataForGeneratedTypes(true);

      var helper = new MixinSerializationHelper(_serializationInfo, _context);

      var realObject = helper.GetRealObject(_context);
      Assert.That(realObject.GetType(), Is.SameAs(_pipeline.ReflectionService.GetAdditionalType(_identifier)));
    }

    [Test]
    public void OnDeserialization_RestoresBaseMembers ()
    {
      _concreteMixin.I = 4711;
      CallGetObjectDataForGeneratedTypes(true);
      var helper = new MixinSerializationHelper(_serializationInfo, _context);

      var deserializedObject = (MixinWithAbstractMembers)helper.GetRealObject(_context);
      Assert.That(deserializedObject.I, Is.EqualTo(0));

      helper.OnDeserialization(null);
      Assert.That(deserializedObject.I, Is.EqualTo(4711));
    }

    [Test]
    public void OnDeserialization_RestoresBaseMembers_NotWhenNoBaseMembers ()
    {
      _concreteMixin.I = 4711;
      CallGetObjectDataForGeneratedTypes(false);
      var helper = new MixinSerializationHelper(_serializationInfo, _context);

      var deserializedObject = (MixinWithAbstractMembers)helper.GetRealObject(_context);
      Assert.That(deserializedObject.I, Is.EqualTo(0));

      helper.OnDeserialization(null);
      Assert.That(deserializedObject.I, Is.EqualTo(0));
    }

    [Test]
    public void OnDeserialization_RaisesEvents ()
    {
      CallGetObjectDataForGeneratedTypes(true);
      var helper = new MixinSerializationHelper(_serializationInfo, _context);

      var deserializedObject = (FakeConcreteMixinType)helper.GetRealObject(_context);
      Assert.That(deserializedObject.OnDeserializedCalled, Is.False);
      Assert.That(deserializedObject.OnDeserializationCalled, Is.False);

      helper.OnDeserialization(null);

      Assert.That(deserializedObject.OnDeserializedCalled, Is.True);
      Assert.That(deserializedObject.OnDeserializationCalled, Is.True);
    }

    private void CallGetObjectDataForGeneratedTypes (bool serializeBaseMembers)
    {
      MixinSerializationHelper.GetObjectDataForGeneratedTypes(
          _serializationInfo, _context, _concreteMixin, _identifier, serializeBaseMembers, _pipeline.ParticipantConfigurationID);
    }

    [IgnoreForMixinConfiguration]
    [Serializable]
    public class FakeConcreteMixinType : MixinWithAbstractMembers, ISerializable, IDeserializationCallback
    {
      public interface IOverriddenMethods
      {
      }

      [NonSerialized]
      public bool OnDeserializingCalled = false;
      [NonSerialized]
      public bool OnDeserializedCalled = false;
      [NonSerialized]
      public bool OnDeserializationCalled = false;
      [NonSerialized]
      public bool CtorCalled = true;
      [NonSerialized]
      public bool SerializationCtorCalled = false;

      public FakeConcreteMixinType ()
      {
      }

      protected FakeConcreteMixinType (SerializationInfo info, StreamingContext context)
      {
        SerializationCtorCalled = true;
      }

      [OnDeserializing]
      public void OnDeserializing (StreamingContext context)
      {
        OnDeserializingCalled = true;
      }

      [OnDeserialized]
      public void OnDeserialized (StreamingContext context)
      {
        OnDeserializedCalled = true;
      }

      public void OnDeserialization (object sender)
      {
        OnDeserializationCalled = true;
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        throw new NotImplementedException();
      }

      protected override string AbstractMethod (int i)
      {
        throw new NotImplementedException();
      }

      protected override string AbstractProperty
      {
        get { throw new NotImplementedException(); }
      }

      protected override event Func<string> AbstractEvent
      {
        add { throw new NotImplementedException(); }
        remove { throw new NotImplementedException(); }
      }

      protected override string RaiseEvent ()
      {
        throw new NotImplementedException();
      }
    }
  }
}
