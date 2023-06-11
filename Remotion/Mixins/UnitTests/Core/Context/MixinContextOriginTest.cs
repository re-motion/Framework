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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Moq;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context
{
  [TestFixture]
  public class MixinContextOriginTest
  {
    private Assembly _someAssembly;

    [SetUp]
    public void SetUp ()
    {
      _someAssembly = GetType().Assembly;
    }

    [Test]
    public void CreateForCustomAttribute_OnMemberInfo ()
    {
      var origin = MixinContextOrigin.CreateForCustomAttribute(new UsesAttribute(typeof(NullMixin)), typeof(MixinContextOriginTest));

      Assert.That(origin.Kind, Is.EqualTo("UsesAttribute"));
      Assert.That(origin.Assembly, Is.EqualTo(typeof(MixinContextOriginTest).Assembly));
      Assert.That(origin.Location, Is.EqualTo("Remotion.Mixins.UnitTests.Core.Context.MixinContextOriginTest"));
    }

    [Test]
    public void CreateForCustomAttribute_OnAssembly ()
    {
      var origin = MixinContextOrigin.CreateForCustomAttribute(new MixAttribute(typeof(object), typeof(NullMixin)), _someAssembly);

      Assert.That(origin.Kind, Is.EqualTo("MixAttribute"));
      Assert.That(origin.Assembly, Is.EqualTo(_someAssembly));
      Assert.That(origin.Location, Is.EqualTo("assembly"));
    }

    [Test]
    public void CreateForMethod ()
    {
      var origin = MixinContextOrigin.CreateForMethod(MethodBase.GetCurrentMethod());

      Assert.That(origin.Kind, Is.EqualTo("Method"));
      Assert.That(origin.Assembly, Is.EqualTo(GetType().Assembly));
      Assert.That(origin.Location, Is.EqualTo("Void CreateForMethod(), declaring type: Remotion.Mixins.UnitTests.Core.Context.MixinContextOriginTest"));
    }

    [Test]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void CreateForStackFrame ()
    {
      var stackFrame = GetCallerStackFrame();
      var origin = MixinContextOrigin.CreateForStackFrame(stackFrame);

      Assert.That(origin.Kind, Is.EqualTo("Method"));
      Assert.That(origin.Assembly, Is.EqualTo(GetType().Assembly));
      Assert.That(origin.Location, Is.EqualTo("Void CreateForStackFrame(), declaring type: Remotion.Mixins.UnitTests.Core.Context.MixinContextOriginTest"));
    }

    [Test]
    public new void ToString ()
    {
      var origin = new MixinContextOrigin("SomeKind", _someAssembly, "some location");

#if NETFRAMEWORK
      var expectedCodeBase = _someAssembly.GetName().CodeBase;
      var expected = string.Format(
          "SomeKind, Location: 'some location' (Assembly: 'Remotion.Mixins.UnitTests', code base: {0})",
          expectedCodeBase);
#else
      var expectedLocation = _someAssembly.Location;
      var expected = string.Format("SomeKind, Location: 'some location' (assembly: 'Remotion.Mixins.UnitTests', location: {0})", expectedLocation);
#endif
      Assert.That(origin.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void Serialize ()
    {
      var origin = new MixinContextOrigin("SomeKind", _someAssembly, "some location");

      var serializerMock = new Mock<IMixinContextOriginSerializer>(MockBehavior.Strict);
      serializerMock.Setup(mock => mock.AddKind(origin.Kind)).Verifiable();
      serializerMock.Setup(mock => mock.AddAssembly(origin.Assembly)).Verifiable();
      serializerMock.Setup(mock => mock.AddLocation(origin.Location)).Verifiable();

      origin.Serialize(serializerMock.Object);

      serializerMock.Verify();
    }

    [Test]
    public void Deserialize ()
    {
      var deserializerStub = new Mock<IMixinContextOriginDeserializer>();
      deserializerStub.Setup(stub => stub.GetKind()).Returns("SomeKind");
      deserializerStub.Setup(stub => stub.GetAssembly()).Returns(_someAssembly);
      deserializerStub.Setup(stub => stub.GetLocation()).Returns("some location");

      var origin = MixinContextOrigin.Deserialize(deserializerStub.Object);

      Assert.That(origin.Kind, Is.EqualTo("SomeKind"));
      Assert.That(origin.Assembly, Is.EqualTo(_someAssembly));
      Assert.That(origin.Location, Is.EqualTo("some location"));
    }

    [Test]
    public void Equals_True ()
    {
      var origin1 = new MixinContextOrigin("some kind", GetType().Assembly, "some location");
      var origin2 = new MixinContextOrigin("some kind", GetType().Assembly, "some location");

      Assert.That(origin1.Equals(origin2), Is.True);
      Assert.That(origin1.Equals((object)origin2), Is.True);
    }

    [Test]
    public void Equals_False ()
    {
      var origin = new MixinContextOrigin("some kind", GetType().Assembly, "some location");
      var originWithDifferentKind = new MixinContextOrigin("some other kind", GetType().Assembly, "some location");
      var originWithDifferentAssembly = new MixinContextOrigin("some kind", typeof(object).Assembly, "some location");
      var originWithDifferentLocation = new MixinContextOrigin("some kind", GetType().Assembly, "some other location");

      Assert.That(origin.Equals(originWithDifferentKind), Is.False);
      Assert.That(origin.Equals(originWithDifferentAssembly), Is.False);
      Assert.That(origin.Equals(originWithDifferentLocation), Is.False);
      Assert.That(origin.Equals(null), Is.False);

      Assert.That(origin.Equals((object)originWithDifferentKind), Is.False);
      Assert.That(origin.Equals((object)originWithDifferentAssembly), Is.False);
      Assert.That(origin.Equals((object)originWithDifferentLocation), Is.False);
      Assert.That(origin.Equals((object)null), Is.False);
      Assert.That(origin.Equals("some other object"), Is.False);
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var origin1 = new MixinContextOrigin("some kind", GetType().Assembly, "some location");
      var origin2 = new MixinContextOrigin("some kind", GetType().Assembly, "some location");

      Assert.That(origin1.GetHashCode(), Is.EqualTo(origin2.GetHashCode()));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private StackFrame GetCallerStackFrame ()
    {
      return new StackFrame(1);
    }
  }
}
