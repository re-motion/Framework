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
using Remotion.Mixins.Samples.UsesAndExtends.Core;
using Remotion.TypePipe;

namespace Remotion.Mixins.Samples.UsesAndExtends.UnitTests
{
  [TestFixture]
  public class EquatableMixinTest
  {
    [Uses(typeof (EquatableMixin<C>))]
    public class C
    {
      public int I;
      public string S;
      public bool B;
    }

    [Test]
    public void ImplementsEquatable()
    {
      C c = new C();
      Assert.That (c is IEquatable<C>, Is.False);

      C c2 = ObjectFactory.Create<C>(ParamList.Empty);
      Assert.That (c2 is IEquatable<C>, Is.True);
    }

    [Test]
    public void EqualsRespectsMembers ()
    {
      C c = ObjectFactory.Create<C> (ParamList.Empty);
      C c2 = ObjectFactory.Create<C> (ParamList.Empty);
      Assert.That (c2, Is.EqualTo (c));

      c2.S = "foo";
      Assert.That (c2, Is.Not.EqualTo (c));
      c2.I = 5;
      c2.B = true;
      Assert.That (c2, Is.Not.EqualTo (c));
      c.S = "foo";
      Assert.That (c2, Is.Not.EqualTo (c));
      c.I = 5;
      Assert.That (c2, Is.Not.EqualTo (c));
      c.B = true;
      Assert.That (c2, Is.EqualTo (c));
    }

    [Test]
    public void GetHashCodeRespectsMembers ()
    {
      C c = ObjectFactory.Create<C> (ParamList.Empty);
      C c2 = ObjectFactory.Create<C> (ParamList.Empty);
      Assert.That (c2.GetHashCode(), Is.EqualTo (c.GetHashCode()));

      c2.S = "foo";
      Assert.That (c2.GetHashCode(), Is.Not.EqualTo (c.GetHashCode()));
      c2.I = 5;
      c2.B = true;
      Assert.That (c2.GetHashCode (), Is.Not.EqualTo (c.GetHashCode ()));
      c.S = "foo";
      Assert.That (c2.GetHashCode (), Is.Not.EqualTo (c.GetHashCode ()));
      c.I = 5;
      Assert.That (c2.GetHashCode (), Is.Not.EqualTo (c.GetHashCode ()));
      c.B = true;
      Assert.That (c2.GetHashCode (), Is.EqualTo (c.GetHashCode ()));
    }
  }
}
