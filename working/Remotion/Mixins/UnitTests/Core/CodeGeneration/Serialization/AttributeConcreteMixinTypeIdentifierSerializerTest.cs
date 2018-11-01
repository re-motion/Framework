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
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.Serialization
{
  [TestFixture]
  public class AttributeConcreteMixinTypeIdentifierSerializerTest
  {
    private MethodInfo _simpleMethod;
    private MethodInfo _genericMethod;

    private AttributeConcreteMixinTypeIdentifierSerializer _serializer;

    [SetUp]
    public void SetUp ()
    {
      _simpleMethod = typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes);
      _genericMethod = typeof (BaseType7).GetMethod ("One");

      _serializer = new AttributeConcreteMixinTypeIdentifierSerializer ();
    }

    [Test]
    public void AddMixinType ()
    {
      _serializer.AddMixinType (typeof (BT1Mixin1));
      Assert.That (_serializer.Values[0], Is.SameAs (typeof (BT1Mixin1)));
    }

    [Test]
    public void AddOverriders ()
    {
      _serializer.AddOverriders (new HashSet<MethodInfo> { _simpleMethod });

      Assert.That (_serializer.Values[1].GetType (), Is.EqualTo (typeof (object[])));
      Assert.That (((object[]) _serializer.Values[1]).Length, Is.EqualTo (1));
      Assert.That (((object[]) ((object[]) _serializer.Values[1])[0]).Length, Is.EqualTo (3));
      Assert.That (((object[]) ((object[]) _serializer.Values[1])[0])[0], Is.SameAs (typeof (BaseType1)));
      Assert.That (((object[]) ((object[]) _serializer.Values[1])[0])[1], Is.EqualTo ("VirtualMethod"));
      Assert.That (((object[]) ((object[]) _serializer.Values[1])[0])[2], Is.EqualTo ("System.String VirtualMethod()"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void AddOverriders_ClosedGeneric ()
    {
      _serializer.AddOverriders (new HashSet<MethodInfo> { _genericMethod.MakeGenericMethod (typeof (int)) });
    }

    [Test]
    public void AddOverridden ()
    {
      _serializer.AddOverridden (new HashSet<MethodInfo> { _simpleMethod });

      Assert.That (_serializer.Values[2].GetType (), Is.EqualTo (typeof (object[])));
      Assert.That (((object[]) _serializer.Values[2]).Length, Is.EqualTo (1));
      Assert.That (((object[]) ((object[]) _serializer.Values[2])[0]).Length, Is.EqualTo (3));
      Assert.That (((object[]) ((object[]) _serializer.Values[2])[0])[0], Is.SameAs (typeof (BaseType1)));
      Assert.That (((object[]) ((object[]) _serializer.Values[2])[0])[1], Is.EqualTo ("VirtualMethod"));
      Assert.That (((object[]) ((object[]) _serializer.Values[2])[0])[2], Is.EqualTo ("System.String VirtualMethod()"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void AddOverridden_ClosedGeneric ()
    {
      _serializer.AddOverridden (new HashSet<MethodInfo> { _genericMethod.MakeGenericMethod (typeof (int)) });
    }
  }
}
