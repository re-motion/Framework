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
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectWithIdentityBaseImplementationTest
  {
    [Test]
    public void Create ()
    {
      var wrapper = new ClassDerivedFromBindableObjectWithIdentityBase ();
      var mixin = BindableObjectWithIdentityBaseImplementation.Create (wrapper);
      Assert.That (mixin.BusinessObjectClass, Is.Not.Null);
      Assert.That (PrivateInvoke.GetNonPublicProperty (mixin, "Target"), Is.SameAs (wrapper));
    }

    [Test]
    public void Deserialization ()
    {
      var wrapper = new ClassDerivedFromBindableObjectWithIdentityBase ();
      var mixin = BindableObjectWithIdentityBaseImplementation.Create (wrapper);
      var deserializedData = Serializer.SerializeAndDeserialize (Tuple.Create (mixin, wrapper));
      Assert.That (deserializedData.Item1.BusinessObjectClass, Is.Not.Null);
      Assert.That (PrivateInvoke.GetNonPublicProperty (deserializedData.Item1, "Target"), Is.SameAs (deserializedData.Item2));
    }

    [Test]
    public void UniqueIdentifierViaImplementation ()
    {
      var instance = new ClassDerivedFromBindableObjectWithIdentityBase ();
      instance.SetUniqueIdentifier ("uniqueID");
      var mixin = (BindableObjectWithIdentityMixin) PrivateInvoke.GetNonPublicField (instance, "_implementation");
      Assert.That (mixin.UniqueIdentifier, Is.EqualTo ("uniqueID"));
    }

    [Test]
    public void BaseDisplayName ()
    {
      var wrapper = new ClassDerivedFromBindableObjectWithIdentityBase ();
      var implementation = BindableObjectWithIdentityBaseImplementation.Create (wrapper);
      Assert.That (implementation.BaseDisplayName, Is.EqualTo (wrapper.BusinessObjectClass.Identifier));
    }

    [Test]
    public void DisplayName_ViaImplementation_Default ()
    {
      var wrapper = new ClassDerivedFromBindableObjectWithIdentityBase ();
      var implementation = BindableObjectWithIdentityBaseImplementation.Create (wrapper);
      Assert.That (implementation.DisplayName, Is.EqualTo (wrapper.BusinessObjectClass.Identifier));
    }

    [Test]
    public void DisplayName_ViaImplementation_Overridden ()
    {
      var wrapper = new ClassDerivedFromBindableObjectWithIdentityBaseOverridingDisplayName ();
      var implementation = BindableObjectWithIdentityBaseImplementation.Create (wrapper);
      Assert.That (implementation.DisplayName, Is.EqualTo ("Overrotten!"));
    }
  }
}
