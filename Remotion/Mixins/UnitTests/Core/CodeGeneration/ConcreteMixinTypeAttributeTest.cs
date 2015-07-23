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
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [TestFixture]
  public class ConcreteMixinTypeAttributeTest
  {
    private ConcreteMixinTypeIdentifier _simpleIdentifier;

    [SetUp]
    public void SetUp ()
    {
      _simpleIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
    }

    [Test]
    public void FromAttributeApplication ()
    {
      var attribute = ((ConcreteMixinTypeAttribute[]) 
          typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers)
              .GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false))
              .Single();

      var identifier = attribute.GetIdentifier ();
      Assert.That (identifier.MixinType, Is.SameAs (typeof (MixinWithAbstractMembers)));

      const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
      Assert.That (identifier.Overridden.ToArray(), Is.EquivalentTo(new[] {
          typeof (MixinWithAbstractMembers).GetMethod ("AbstractMethod", flags),
          typeof (MixinWithAbstractMembers).GetMethod ("RaiseEvent", flags),
          typeof (MixinWithAbstractMembers).GetMethod ("get_AbstractProperty", flags),
          typeof (MixinWithAbstractMembers).GetMethod ("add_AbstractEvent", flags),
          typeof (MixinWithAbstractMembers).GetMethod ("remove_AbstractEvent", flags)

      }));
    }

    [Test]
    public void Create_Identifier ()
    {
      var attribute = ConcreteMixinTypeAttribute.Create (_simpleIdentifier);

      var deserializer = new AttributeConcreteMixinTypeIdentifierDeserializer (attribute.ConcreteMixinTypeIdentifierData);
      Assert.That (ConcreteMixinTypeIdentifier.Deserialize (deserializer), Is.EqualTo (_simpleIdentifier));
    }

    [Test]
    public void GetIdentifier ()
    {
      var attribute = ConcreteMixinTypeAttribute.Create (_simpleIdentifier);
      Assert.That (attribute.GetIdentifier (), Is.EqualTo (_simpleIdentifier));
    }
  }
}
