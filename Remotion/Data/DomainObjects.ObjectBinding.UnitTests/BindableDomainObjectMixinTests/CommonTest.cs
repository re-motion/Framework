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
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class CommonTest : ObjectBindingTestBase
  {
    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That (SampleBindableMixinDomainObject.NewObject(), Is.InstanceOf (typeof (IBusinessObject)));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      SampleBindableMixinDomainObject value = SampleBindableMixinDomainObject.NewObject ();
      Assert.That (value.Name, Is.Not.EqualTo ("Earl"));
      value.Name = "Earl";
      var deserialized = Serializer.SerializeAndDeserialize (Tuple.Create (TestableClientTransaction, value));

      using (deserialized.Item1.EnterDiscardingScope ())
      {
        Assert.That (deserialized.Item2.Name, Is.EqualTo ("Earl"));
        Assert.That (((IBusinessObject) deserialized.Item2).BusinessObjectClass, Is.SameAs (((IBusinessObject) value).BusinessObjectClass));
      }
    }

    [Test]
    public void SerializeAndDeserialize_WithNewBindableObjectProvider ()
    {
      SampleBindableMixinDomainObject value = SampleBindableMixinDomainObject.NewObject ();
      byte[] serialized = Serializer.Serialize (Tuple.Create (TestableClientTransaction, value));
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      var deserialized = (Tuple<ClientTransaction, SampleBindableMixinDomainObject>) Serializer.Deserialize (serialized);

      using (deserialized.Item1.EnterDiscardingScope ())
      {
        Assert.That (((IBusinessObject) deserialized.Item2).BusinessObjectClass, Is.Not.SameAs (((IBusinessObject) value).BusinessObjectClass));
      }
    }
  }
}
