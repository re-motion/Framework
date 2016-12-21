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
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class BehavioralMixinTest : ClientTransactionBaseTest
  {
    [Test]
    public void DomainObjectsCanBeMixed ()
    {
      var domainObject = TargetClassForBehavioralMixin.NewObject ();
      Assert.That (Mixin.Get<NullMixin> (domainObject), Is.Not.Null);
    }

    [Test]
    public void MixinCanAddInterface ()
    {
      var domainObject = TargetClassForBehavioralMixin.NewObject ();
      Assert.That (domainObject is IInterfaceAddedByMixin, Is.True);
      Assert.That (((IInterfaceAddedByMixin) domainObject).GetGreetings (), Is.EqualTo ("Hello, my ID is " + domainObject.ID));
    }

    [Test]
    public void MixinCanOverrideVirtualPropertiesAndMethods ()
    {
      var instance = TargetClassForBehavioralMixin.NewObject();
      instance.Property = "Text";
      Assert.That (instance.Property, Is.EqualTo ("Text-MixinSetter-MixinGetter"));
      Assert.That (instance.GetSomething (), Is.EqualTo ("Something-MixinMethod"));
    }

    [DBTable]
    [TestDomain]
    [Uses (typeof (NullMixin))]
    public class NestedDomainObject : DomainObject
    {
      public static NestedDomainObject NewObject ()
      {
        return NewObject<NestedDomainObject> ();
      }
    }

    [Test]
    public void NestedDomainObjects_CanBeMixed ()
    {
      DomainObject domainObject = NestedDomainObject.NewObject ();
      Assert.That (Mixin.Get<NullMixin> (domainObject), Is.Not.Null);
    }
  }
}
