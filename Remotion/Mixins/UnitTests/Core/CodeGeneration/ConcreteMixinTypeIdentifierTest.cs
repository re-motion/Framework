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
using Moq;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [TestFixture]
  public class ConcreteMixinTypeIdentifierTest
  {
    private MethodInfo _overrider1;
    private MethodInfo _overrider2;
    private MethodInfo _overridden1;
    private MethodInfo _overridden2;

    [SetUp]
    public void SetUp ()
    {
      const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

      _overrider1 = typeof(MixinWithProtectedOverrider).GetMethod("VirtualMethod", flags);
      _overrider2 = typeof(MixinWithProtectedOverrider).GetMethod("get_VirtualProperty", flags);
      _overridden1 = typeof(MixinWithAbstractMembers).GetMethod("AbstractMethod", flags);
      _overridden2 = typeof(MixinWithAbstractMembers).GetMethod("get_AbstractProperty", flags);
    }

    [Test]
    public void Equals_True ()
    {
      var one = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2  },
          new HashSet<MethodInfo> { _overridden1, _overridden2  });
      var two = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2  },
          new HashSet<MethodInfo> { _overridden1, _overridden2  });

      Assert.That(one, Is.EqualTo(two));
    }

    [Test]
    public void Equals_True_OrderOfExternalOverridersIrrelevant ()
    {
      var one = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2  },
          new HashSet<MethodInfo> { _overridden1, _overridden2  });
      var two = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider2, _overrider1  },
          new HashSet<MethodInfo> { _overridden1, _overridden2  });

      Assert.That(one, Is.EqualTo(two));
    }

    [Test]
    public void Equals_True_OrderOfWrappedProtectedMembersIsIrrelevant ()
    {
      var one = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2  },
          new HashSet<MethodInfo> { _overridden1, _overridden2  });
      var two = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2  },
          new HashSet<MethodInfo> { _overridden2, _overridden1  });

      Assert.That(one, Is.EqualTo(two));
    }

    [Test]
    public void Equals_False_DifferentTypes ()
    {
      var one = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2  },
          new HashSet<MethodInfo> { _overridden1, _overridden2  });
      var two = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin2),
          new HashSet<MethodInfo> { _overrider1, _overrider2  },
          new HashSet<MethodInfo> { _overridden1, _overridden2  });

      Assert.That(one, Is.Not.EqualTo(two));
    }

    [Test]
    public void Equals_False_DifferentExternalOverriders ()
    {
      var one = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1  },
          new HashSet<MethodInfo> { _overridden1, _overridden2  });
      var two = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2  },
          new HashSet<MethodInfo> { _overridden1, _overridden2  });

      Assert.That(one, Is.Not.EqualTo(two));
    }

    [Test]
    public void Equals_False_DifferentWrappedProtectedMembers ()
    {
      var one = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2  },
          new HashSet<MethodInfo> { _overridden1  });
      var two = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2  },
          new HashSet<MethodInfo> { _overridden1, _overridden2  });

      Assert.That(one, Is.Not.EqualTo(two));
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var one = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2 },
          new HashSet<MethodInfo> { _overridden1, _overridden2 });
      var two = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2 },
          new HashSet<MethodInfo> { _overridden1, _overridden2 });

      Assert.That(one.GetHashCode(), Is.EqualTo(two.GetHashCode()));
    }

    [Test]
    public void GetHashCode_EqualObjects_OrderOfExternalOverridersIrrelevant ()
    {
      var one = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2 },
          new HashSet<MethodInfo> { _overridden1, _overridden2 });
      var two = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider2, _overrider1 },
          new HashSet<MethodInfo> { _overridden1, _overridden2 });

      Assert.That(one.GetHashCode(), Is.EqualTo(two.GetHashCode()));
    }

    [Test]
    public void GetHashCode_EqualObjects_OrderOfWrappedProtectedMembersIsIrrelevant ()
    {
      var one = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2 },
          new HashSet<MethodInfo> { _overridden1, _overridden2 });
      var two = new ConcreteMixinTypeIdentifier(
          typeof(BT1Mixin1),
          new HashSet<MethodInfo> { _overrider1, _overrider2 },
          new HashSet<MethodInfo> { _overridden2, _overridden1 });

      Assert.That(one.GetHashCode(), Is.EqualTo(two.GetHashCode()));
    }

    [Test]
    public void Serialize ()
    {
      var overriders = new HashSet<MethodInfo> { _overrider1, _overrider2 };
      var overridden = new HashSet<MethodInfo> { _overridden1, _overridden2 };

      var identifier = new ConcreteMixinTypeIdentifier(typeof(BT1Mixin1), overriders, overridden);
      var serializerMock = new Mock<IConcreteMixinTypeIdentifierSerializer>();

      identifier.Serialize(serializerMock.Object);

      serializerMock.Verify(mock => mock.AddMixinType(typeof(BT1Mixin1)), Times.AtLeastOnce());
      serializerMock.Verify(mock => mock.AddOverriders(overriders), Times.AtLeastOnce());
      serializerMock.Verify(mock => mock.AddOverridden(overridden), Times.AtLeastOnce());
    }

    [Test]
    public void Deserialize ()
    {
      var overriders = new HashSet<MethodInfo> { _overrider1, _overrider2 };
      var overridden = new HashSet<MethodInfo> { _overridden1, _overridden2 };
      var deserializerMock = new Mock<IConcreteMixinTypeIdentifierDeserializer>();

      deserializerMock.Setup(mock => mock.GetMixinType()).Returns(typeof(BT1Mixin1)).Verifiable();
      deserializerMock.Setup(mock => mock.GetOverriders()).Returns(overriders).Verifiable();
      deserializerMock.Setup(mock => mock.GetOverridden()).Returns(overridden).Verifiable();

      var identifier = ConcreteMixinTypeIdentifier.Deserialize(deserializerMock.Object);

      deserializerMock.Verify();
      Assert.That(identifier.MixinType, Is.SameAs(typeof(BT1Mixin1)));
      Assert.That(identifier.Overriders, Is.SameAs(overriders));
      Assert.That(identifier.Overridden, Is.SameAs(overridden));
    }
  }
}
