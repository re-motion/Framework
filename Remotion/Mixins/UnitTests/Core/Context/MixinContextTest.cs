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
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.Context
{
  [TestFixture]
  public class MixinContextTest
  {
    [Test]
    public void ExplicitDependencies_Empty ()
    {
      var mixinContext = MixinContextObjectMother.Create (explicitDependencies: Enumerable.Empty<Type> ());

      Assert.That (mixinContext.ExplicitDependencies.Count, Is.EqualTo (0));
      Assert.That (mixinContext.ExplicitDependencies, Has.No.Member(typeof (IBaseType2)));

      Assert.That (mixinContext.ExplicitDependencies, Is.Empty);
    }

    [Test]
    public void ExplicitInterfaceDependencies_NonEmpty ()
    {
      var mixinContext = MixinContextObjectMother.Create (explicitDependencies: new[] { typeof (IBT6Mixin2), typeof (IBT6Mixin3) });

      Assert.That (mixinContext.ExplicitDependencies.Count, Is.EqualTo (2));
      Assert.That (mixinContext.ExplicitDependencies, Has.Member (typeof (IBT6Mixin2)));
      Assert.That (mixinContext.ExplicitDependencies, Has.Member (typeof (IBT6Mixin3)));

      Assert.That(mixinContext.ExplicitDependencies, Is.EquivalentTo(new[] { typeof(IBT6Mixin2), typeof(IBT6Mixin3) }));
    }

    [Test]
    public void ExplicitMixinDependencies_NonEmpty ()
    {
      var mixinContext = MixinContextObjectMother.Create (explicitDependencies: new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) });

      Assert.That (mixinContext.ExplicitDependencies.Count, Is.EqualTo (2));
      Assert.That (mixinContext.ExplicitDependencies, Has.Member (typeof (BT6Mixin2)));
      Assert.That (mixinContext.ExplicitDependencies, Has.Member (typeof (BT6Mixin3<>)));

      Assert.That(mixinContext.ExplicitDependencies, Is.EquivalentTo(new[] { typeof(BT6Mixin2), typeof(BT6Mixin3<>) }));
    }

    [Test]
    public void Equals_True ()
    {
      var c1a = new MixinContext (
          MixinKind.Extending, 
          typeof (BT6Mixin1), 
          MemberVisibility.Private, 
          new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) },
          MixinContextOriginObjectMother.Create());
      var c1b = new MixinContext (
          MixinKind.Extending,
          typeof (BT6Mixin1),
          MemberVisibility.Private,
          new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) },
          MixinContextOriginObjectMother.Create());
      var c1WithDifferentDependencyOrder = new MixinContext (
          MixinKind.Extending, 
          typeof (BT6Mixin1), 
          MemberVisibility.Private,
          new[] { typeof (BT6Mixin3<>), typeof (BT6Mixin2) },
          MixinContextOriginObjectMother.Create ());
      var c1WithDifferentOrigin = new MixinContext (
          MixinKind.Extending,
          typeof (BT6Mixin1),
          MemberVisibility.Private,
          new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) },
          MixinContextOriginObjectMother.Create (kind: "some other kind"));

      var c2a = new MixinContext (
          MixinKind.Used,
          typeof (BT6Mixin1),
          MemberVisibility.Public,
          Enumerable.Empty<Type>(),
          MixinContextOriginObjectMother.Create());
      var c2b = new MixinContext (
          MixinKind.Used,
          typeof (BT6Mixin1),
          MemberVisibility.Public,
          Enumerable.Empty<Type>(),
          MixinContextOriginObjectMother.Create());

      Assert.That (c1b, Is.EqualTo (c1a));
      Assert.That (c1WithDifferentDependencyOrder, Is.EqualTo (c1a));

      Assert.That (c1WithDifferentOrigin.Origin, Is.Not.EqualTo (c1a.Origin));
      Assert.That (c1WithDifferentOrigin, Is.EqualTo (c1a));

      Assert.That (c2b, Is.EqualTo (c2a));
    }

    [Test]
    public void Equals_False()
    {
      var origin = MixinContextOriginObjectMother.Create();
      var c1 = new MixinContext (
          MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) }, origin);
      var c2 = new MixinContext (MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, new[] { typeof (BT6Mixin3<>) }, origin);
      var c3 = new MixinContext (MixinKind.Extending, typeof (BT6Mixin2), MemberVisibility.Private, new[] { typeof (BT6Mixin3<>) }, origin);
      var c4 = new MixinContext (MixinKind.Used, typeof (BT6Mixin2), MemberVisibility.Private, new[] { typeof (BT6Mixin3<>) }, origin);
      var c5 = new MixinContext (MixinKind.Used, typeof (BT6Mixin2), MemberVisibility.Public, new[] { typeof (BT6Mixin3<>) }, origin);

      Assert.That (c2, Is.Not.EqualTo (c1));
      Assert.That (c3, Is.Not.EqualTo (c2));
      Assert.That (c4, Is.Not.EqualTo (c3));
      Assert.That (c5, Is.Not.EqualTo (c4));
    }

    [Test]
    public void GetHashCode_Equal ()
    {
      var c1a = new MixinContext (
          MixinKind.Extending,
          typeof (BT6Mixin1),
          MemberVisibility.Private,
          new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) },
          MixinContextOriginObjectMother.Create());
      var c1b = new MixinContext (
          MixinKind.Extending,
          typeof (BT6Mixin1),
          MemberVisibility.Private,
          new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) },
          MixinContextOriginObjectMother.Create());
      var c1WithDifferentDependencyOrder = new MixinContext (
          MixinKind.Extending,
          typeof (BT6Mixin1),
          MemberVisibility.Private,
          new[] { typeof (BT6Mixin3<>), typeof (BT6Mixin2) },
          MixinContextOriginObjectMother.Create());
      var c1WithDifferentOrigin = new MixinContext (
          MixinKind.Extending,
          typeof (BT6Mixin1),
          MemberVisibility.Private,
          new[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) },
          MixinContextOriginObjectMother.Create (kind: "some different kind"));

      var c2a = new MixinContext (
          MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, Enumerable.Empty<Type>(), MixinContextOriginObjectMother.Create());
      var c2b = new MixinContext (
          MixinKind.Extending, typeof (BT6Mixin1), MemberVisibility.Private, Enumerable.Empty<Type>(), MixinContextOriginObjectMother.Create());

      var c3a = new MixinContext (
          MixinKind.Used,
          typeof (BT6Mixin1),
          MemberVisibility.Public,
          new[] { typeof (BT6Mixin3<>), typeof (BT6Mixin2) },
          MixinContextOriginObjectMother.Create());
      var c3b = new MixinContext (
          MixinKind.Used,
          typeof (BT6Mixin1),
          MemberVisibility.Public,
          new[] { typeof (BT6Mixin3<>), typeof (BT6Mixin2) },
          MixinContextOriginObjectMother.Create());

      Assert.That (c1b.GetHashCode (), Is.EqualTo (c1a.GetHashCode ()));
      Assert.That (c1WithDifferentDependencyOrder.GetHashCode (), Is.EqualTo (c1a.GetHashCode ()));

      Assert.That (c1WithDifferentOrigin.Origin, Is.Not.EqualTo (c1a.Origin));
      Assert.That (c1WithDifferentOrigin.GetHashCode (), Is.EqualTo (c1a.GetHashCode ()));

      Assert.That (c2b.GetHashCode (), Is.EqualTo (c2a.GetHashCode ()));
      Assert.That (c3b.GetHashCode (), Is.EqualTo (c3a.GetHashCode ()));
    }

    [Test]
    public void MixinKindProperty ()
    {
      var c1 = MixinContextObjectMother.Create (mixinKind: MixinKind.Extending);
      var c2 = MixinContextObjectMother.Create (mixinKind: MixinKind.Used);
      Assert.That (c1.MixinKind, Is.EqualTo (MixinKind.Extending));
      Assert.That (c2.MixinKind, Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void IntroducedMemberVisibility_Private ()
    {
      var context = MixinContextObjectMother.Create (introducedMemberVisibility: MemberVisibility.Private);
      Assert.That (context.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Private));
    }

    [Test]
    public void IntroducedMemberVisibility_Public ()
    {
      var context = MixinContextObjectMother.Create (introducedMemberVisibility: MemberVisibility.Public);
      Assert.That (context.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Public));
    }

    [Test]
    public void ApplyAdditionalExplicitDependencies ()
    {
      var context = MixinContextObjectMother.Create (explicitDependencies: new[] { typeof (int), typeof (double) });

      var result = context.ApplyAdditionalExplicitDependencies (new[] { typeof (string), typeof (double), typeof (float), typeof (float) });

      Assert.That (result.ExplicitDependencies, Is.EquivalentTo (new[] { typeof (int), typeof (double), typeof (string), typeof (float) }));
      Assert.That (context.ExplicitDependencies, Is.EquivalentTo (new[] { typeof (int), typeof (double) }));

      Assert.That (result.MixinType, Is.SameAs (context.MixinType));
      Assert.That (result.MixinKind, Is.EqualTo (context.MixinKind));
      Assert.That (result.IntroducedMemberVisibility, Is.EqualTo (context.IntroducedMemberVisibility));
      Assert.That (result.Origin, Is.SameAs (context.Origin));
    }

    [Test]
    public void Serialize()
    {
      var context = MixinContextObjectMother.Create();

      var serializer = MockRepository.GenerateMock<IMixinContextSerializer> ();
      context.Serialize (serializer);

      serializer.AssertWasCalled (mock => mock.AddMixinKind (context.MixinKind));
      serializer.AssertWasCalled (mock => mock.AddMixinType (context.MixinType));
      serializer.AssertWasCalled (mock => mock.AddIntroducedMemberVisibility (context.IntroducedMemberVisibility));
      serializer.AssertWasCalled (mock => mock.AddExplicitDependencies (Arg<IEnumerable<Type>>.List.Equal (context.ExplicitDependencies)));
      serializer.AssertWasCalled (mock => mock.AddOrigin (context.Origin));
    }

    [Test]
    public void Deserialize ()
    {
      var expectedContext = MixinContextObjectMother.Create();

      var deserializer = MockRepository.GenerateStrictMock<IMixinContextDeserializer> ();
      deserializer.Expect (mock => mock.GetMixinType ()).Return (expectedContext.MixinType);
      deserializer.Expect (mock => mock.GetMixinKind ()).Return (expectedContext.MixinKind);
      deserializer.Expect (mock => mock.GetIntroducedMemberVisibility ()).Return (expectedContext.IntroducedMemberVisibility);
      deserializer.Expect (mock => mock.GetExplicitDependencies ()).Return (expectedContext.ExplicitDependencies);
      deserializer.Expect (mock => mock.GetOrigin()).Return (expectedContext.Origin);

      var context = MixinContext.Deserialize(deserializer);

      deserializer.VerifyAllExpectations();
      Assert.That (context, Is.EqualTo (expectedContext));
    }

    [Test]
    public void Serialization_IsUpToDate()
    {
      var properties = typeof (MixinContext).GetProperties (BindingFlags.Public |BindingFlags.Instance);
      Assert.That (typeof (IMixinContextSerializer).GetMethods ().Length, Is.EqualTo (properties.Length));
      Assert.That (typeof (IMixinContextDeserializer).GetMethods ().Length, Is.EqualTo (properties.Length));
    }
  }
}
