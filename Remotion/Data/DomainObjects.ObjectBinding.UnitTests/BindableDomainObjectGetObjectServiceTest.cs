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

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectGetObjectServiceTest : ObjectBindingTestBase
  {
    private SampleBindableDomainObject _instance;
    private BindableDomainObjectGetObjectService _service;
    private BindableObjectClassWithIdentity _businessObjectClass;
    private string _id;

    public override void SetUp ()
    {
      base.SetUp();

      _service = new BindableDomainObjectGetObjectService();

      _instance = SampleBindableDomainObject.NewObject();
      _businessObjectClass = (BindableObjectClassWithIdentity) ((IBusinessObject) _instance).BusinessObjectClass;
      _id =  ((IBusinessObjectWithIdentity) _instance).UniqueIdentifier;
    }

    [Test]
    public void GetObject_ObjectExists()
    {
      Assert.That (_service.GetObject (_businessObjectClass, _id), Is.SameAs (_instance));
    }

    [Test]
    public void GetObject_ObjectNotFound ()
    {
      Assert.That (_service.GetObject (_businessObjectClass, new ObjectID(typeof (SampleBindableDomainObject), Guid.NewGuid()).ToString()), Is.Null);
    }

    [Test]
    public void GetObject_ObjectInvalid()
    {
      _instance.Delete();
      Assert.That (_instance.State, Is.EqualTo (StateType.Invalid));

      Assert.That (_service.GetObject (_businessObjectClass, _id), Is.Null);
    }

    [Test]
    public void GetObject_ObjectDeleted()
    {
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        _instance.Delete();
        Assert.That (_instance.State, Is.EqualTo (StateType.Deleted));

        Assert.That (_service.GetObject (_businessObjectClass, _id), Is.Null);
      }
    }
  }
}