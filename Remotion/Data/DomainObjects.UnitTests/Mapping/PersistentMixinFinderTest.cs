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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.MixinTestDomain;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class PersistentMixinFinderTest
  {
    [Test]
    public void Initialization_NullBaseType ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<object> ().AddMixin<int> ().EnterScope ())
      {
        var finder = new PersistentMixinFinder (typeof (object), false);
        Assert.That (finder.MixinConfiguration, Is.Not.Null);
        Assert.That (finder.ParentClassContext, Is.Null);
      }
    }

    [Test]
    public void IsInParentContext_WithEmptyParentContext ()
    {
      var finder = new PersistentMixinFinder (typeof (TargetClassBase), false);
      Assert.That (finder.ParentClassContext.IsEmpty(), Is.True);
      Assert.That (finder.IsInParentContext (typeof (MixinBase)), Is.False);
    }

    [Test]
    public void IsInParentContext_WithParentContext ()
    {
      var finder = new PersistentMixinFinder (typeof (TargetClassA), false);
      Assert.That (finder.ParentClassContext, Is.Not.Null);
      Assert.That (finder.IsInParentContext (typeof (MixinBase)), Is.True);
      Assert.That (finder.IsInParentContext (typeof (MixinA)), Is.False);
    }

    [Test]
    public void IsPersistenceRelevant ()
    {
      Assert.That (PersistentMixinFinder.IsPersistenceRelevant (typeof (MixinBase)), Is.True);
      Assert.That (PersistentMixinFinder.IsPersistenceRelevant (typeof (MixinA)), Is.True);
      Assert.That (PersistentMixinFinder.IsPersistenceRelevant (typeof (NonDomainObjectMixin)), Is.False);
    }

    [Test]
    public void ForTargetClassBase ()
    {
      Type targetType = typeof (TargetClassBase);
      CheckPersistentMixins (targetType, false, typeof (MixinBase));
    }

    [Test]
    public void ForTargetClassA ()
    {
      Type targetType = typeof (TargetClassA);
      CheckPersistentMixins (targetType, false, typeof (MixinA), typeof (MixinC), typeof (MixinD));
    }

    [Test]
    public void ForTargetClassB ()
    {
      Type targetType = typeof (TargetClassB);
      CheckPersistentMixins (targetType, false, typeof (MixinB), typeof (MixinE));
    }

    [Test]
    public void ForTargetClassB_WithIncludeInheritedMixins ()
    {
      Type targetType = typeof (TargetClassB);
      CheckPersistentMixins (targetType, true, typeof (MixinB), typeof (MixinE), typeof (MixinC), typeof (MixinD));
    }

    public class PersistedGenericMixin<T> : DomainObjectMixin<T>
        where T : DomainObject
    {
    }

    public class NonPersistedGenericMixin<T> : Mixin<T>
        where T : DomainObject
    {
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "The persistence-relevant mixin "
        + "Remotion.Data.DomainObjects.UnitTests.Mapping.PersistentMixinFinderTest+PersistedGenericMixin`1 applied to class "
        + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order has open generic type parameters. All type parameters of the mixin must be "
        + "specified when it is applied to a DomainObject.")]
    public void PersistenceRelevant_OpenGenericMixin ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Order)).Clear ().AddMixins (typeof (PersistedGenericMixin<>)).EnterScope ())
      {
        new PersistentMixinFinder (typeof (Order), false).GetPersistentMixins ();
      }
    }

    [Test]
    public void PersistenceIrrelevant_OpenGenericMixin ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (Order)).Clear ().AddMixins (typeof (NonPersistedGenericMixin<>)).EnterScope ())
      {
        Type[] persistentMixins = new PersistentMixinFinder (typeof (Order), false).GetPersistentMixins ();
        Assert.That (persistentMixins, Is.Empty);
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Remotion.Data.DomainObjects.UnitTests.Mapping."
      + "MixinTestDomain.TargetClassA' suppresses mixin 'MixinA' inherited from its base class 'TargetClassBase'. This is not allowed because "
      + "the mixin adds persistence information to the base class which must also be present in the derived class.")]
    public void PersistenceRelevant_MixinSuppressingInherited ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass (typeof (TargetClassBase))
          .AddMixins (typeof (MixinA))
          .ForClass (typeof (TargetClassA))
          .AddMixin (typeof (MixinC))
          .SuppressMixin (typeof (MixinA))
          .EnterScope ())
      {
        new PersistentMixinFinder (typeof (TargetClassA), false).GetPersistentMixins ();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Remotion.Data.DomainObjects.UnitTests.Mapping."
      + "MixinTestDomain.TargetClassA' suppresses mixin 'MixinA' inherited from its base class 'TargetClassBase'. This is not allowed because "
      + "the mixin adds persistence information to the base class which must also be present in the derived class.")]
    public void PersistenceRelevant_MixinClearingContext ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassBase))
          .AddMixins (typeof (MixinA))
          .ForClass (typeof (TargetClassA))
          .Clear()
          .EnterScope ())
      {
        new PersistentMixinFinder (typeof (TargetClassA), false).GetPersistentMixins ();
      }
    }

    [Test]
    public void PersistenceIrrelevant_MixinSuppressingInherited ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassBase))
          .AddMixins (typeof (NonDomainObjectMixin))
          .ForClass (typeof (TargetClassA))
          .AddMixin (typeof (NonPersistedGenericMixin<>)).SuppressMixin (typeof (NonDomainObjectMixin))
          .EnterScope ())
      {
        Type[] persistentMixins = new PersistentMixinFinder (typeof (TargetClassA), false).GetPersistentMixins ();
        Assert.That (persistentMixins, Is.Empty);
      }
    }

    [Test]
    public void FindOriginalMixinTarget_NoMixinConfiguration_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassA), false).FindOriginalMixinTarget (typeof (MixinB));
        Assert.That (originalTarget, Is.Null);
      }
    }

    [Test]
    public void FindOriginalMixinTarget_NoMixinConfiguration_InheritedTrue ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassA), true).FindOriginalMixinTarget (typeof (MixinB));
        Assert.That (originalTarget, Is.Null);
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinNotDefined ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassA))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassA), false).FindOriginalMixinTarget (typeof (MixinB));
        Assert.That (originalTarget, Is.Null);
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinOnClass_InheritedTrue ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassA))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassA), true).FindOriginalMixinTarget(typeof (MixinA));
        Assert.That (originalTarget, Is.SameAs (typeof (TargetClassA)));
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinOnClass_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassA))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassA), false).FindOriginalMixinTarget (typeof (MixinA));
        Assert.That (originalTarget, Is.SameAs (typeof (TargetClassA)));
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinOnBase_InheritedTrue ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassBase))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassA), true).FindOriginalMixinTarget (typeof (MixinA));
        Assert.That (originalTarget, Is.SameAs (typeof (TargetClassBase)));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The given mixin is inherited from the base class, but "
        + "includeInherited is not set to true.")]
    public void FindOriginalMixinTarget_MixinOnBase_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassBase))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        new PersistentMixinFinder (typeof (TargetClassA), false).FindOriginalMixinTarget (typeof (MixinA));
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinOnBaseBase_InheritedTrue ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassBase))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassB), true).FindOriginalMixinTarget (typeof (MixinA));
        Assert.That (originalTarget, Is.SameAs (typeof (TargetClassBase)));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The given mixin is inherited from the base class, but " 
        + "includeInherited is not set to true.")]
    public void FindOriginalMixinTarget_MixinOnBaseBase_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetClassBase))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassB), false).FindOriginalMixinTarget (typeof (MixinA));
        Assert.That (originalTarget, Is.SameAs (typeof (TargetClassBase)));
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinOnBaseBaseBase ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (object))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        Type originalTarget = new PersistentMixinFinder (typeof (TargetClassB), true).FindOriginalMixinTarget (typeof (MixinA));
        Assert.That (originalTarget, Is.SameAs (typeof (object)));
      }
    }

    [Test]
    public void FindOriginalMixinTarget_MixinOnBaseBaseBase_WithNonCurrentConfiguration ()
    {
      PersistentMixinFinder persistentMixinFinder;
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (object))
          .AddMixin (typeof (MixinA))
          .EnterScope ())
      {
        persistentMixinFinder = new PersistentMixinFinder (typeof (TargetClassB), true);
      }
      Type originalTarget = persistentMixinFinder.FindOriginalMixinTarget (typeof (MixinA));
      Assert.That (originalTarget, Is.SameAs (typeof (object)));
    }

    private void CheckPersistentMixins (Type targetType, bool includeInherited, params Type[] expectedTypes)
    {
      Type[] mixinTypes = new PersistentMixinFinder (targetType, includeInherited).GetPersistentMixins ();
      Assert.That (mixinTypes, Is.EquivalentTo (expectedTypes));
    }
  }
}
