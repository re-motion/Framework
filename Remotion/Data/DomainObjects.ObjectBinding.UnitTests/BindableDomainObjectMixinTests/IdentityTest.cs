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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class IdentityTest : ObjectBindingTestBase
  {
    [Test]
    public void BindableDomainObjectsHaveIdentity ()
    {
      SampleBindableMixinDomainObject domainObject = SampleBindableMixinDomainObject.NewObject ();
      Assert.That (domainObject is IBusinessObjectWithIdentity, Is.True);
    }

    [Test]
    public void BindableDomainObjectClassesHaveIdentity ()
    {
      SampleBindableMixinDomainObject domainObject = SampleBindableMixinDomainObject.NewObject ();
      Assert.That (((IBusinessObjectWithIdentity)domainObject).BusinessObjectClass is IBusinessObjectClassWithIdentity, Is.True);
    }
    
    [Test]
    public void UniqueIdentifier ()
    {
      SampleBindableMixinDomainObject domainObject = SampleBindableMixinDomainObject.NewObject ();
      Assert.That (((IBusinessObjectWithIdentity) domainObject).UniqueIdentifier, Is.EqualTo (domainObject.ID.ToString ()));
    }

    [Test]
    public void GetFromUniqueIdentifier ()
    {
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (typeof (IGetObjectService), new BindableDomainObjectGetObjectService());
      SampleBindableMixinDomainObject original = SampleBindableMixinDomainObject.NewObject ();
      
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (SampleBindableMixinDomainObject));
      var boClass = (BindableObjectClassWithIdentity) provider.GetBindableObjectClass (typeof (SampleBindableMixinDomainObject));
      Assert.That (boClass.GetObject (original.ID.ToString ()), Is.SameAs (original));
    }
  }
}
