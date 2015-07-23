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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectPropertyReadAccessStrategyTest : ObjectBindingTestBase
  {
    [Test]
    public void SupportedStateTypes ()
    {
      Assert.That (
          Enum.GetValues (typeof (StateType)).Length,
          Is.EqualTo (7),
          "StateType enum has changed. BindableDomainObjectPropertyReadAccessStrategy implementation must be updated accordingly.");
    }
    [Test]
    public void CanRead_NewObject_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty (instance);
      var strategy = (IBindablePropertyReadAccessStrategy) new BindableDomainObjectPropertyReadAccessStrategy();

      Assert.That (instance.State, Is.EqualTo (StateType.New));

      var result = strategy.CanRead (instance, property);

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanRead_UnchangedObject_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty (instance);
      var strategy = (IBindablePropertyReadAccessStrategy) new BindableDomainObjectPropertyReadAccessStrategy();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        instance.EnsureDataAvailable();
        Assert.That (instance.State, Is.EqualTo (StateType.Unchanged));
        var result = strategy.CanRead (instance, property);

        Assert.That (result, Is.True);
      }
    }

    [Test]
    public void CanRead_ChangedObject_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty (instance);
      var strategy = (IBindablePropertyReadAccessStrategy) new BindableDomainObjectPropertyReadAccessStrategy();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        instance.Name = "new value";
        Assert.That (instance.State, Is.EqualTo (StateType.Changed));
        var result = strategy.CanRead (instance, property);

        Assert.That (result, Is.True);
      }
    }

    [Test]
    public void CanRead_InvalidObject_ReturnsFalse ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty (instance);
      var strategy = (IBindablePropertyReadAccessStrategy) new BindableDomainObjectPropertyReadAccessStrategy();

      LifetimeService.DeleteObject (instance.DefaultTransactionContext.ClientTransaction, instance);
      Assert.That (instance.State, Is.EqualTo (StateType.Invalid));

      var result = strategy.CanRead (instance, property);

      Assert.That (result, Is.False);
    }

    [Test]
    public void CanRead_DeletedObject_ReturnsFalse ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty (instance);
      var strategy = (IBindablePropertyReadAccessStrategy) new BindableDomainObjectPropertyReadAccessStrategy();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        LifetimeService.DeleteObject (instance.DefaultTransactionContext.ClientTransaction, instance);
        Assert.That (instance.State, Is.EqualTo (StateType.Deleted));

        var result = strategy.CanRead (instance, property);

        Assert.That (result, Is.False);
      }
    }

    [Test]
    public void CanRead_NotLoadedObject_LoadsExistingObject_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty (instance);
      var strategy = (IBindablePropertyReadAccessStrategy) new BindableDomainObjectPropertyReadAccessStrategy();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (instance.State, Is.EqualTo (StateType.NotLoadedYet));

        var result = strategy.CanRead (instance, property);

        Assert.That (result, Is.True);
        Assert.That (instance.State, Is.EqualTo (StateType.Unchanged));
      }
    }

    [Test]
    public void CanRead_NotLoadedObject_LoadsNonExistentObject_ReturnsFalse ()
    {
      var instance = (SampleBindableDomainObject) LifetimeService.GetObjectReference (
          ClientTransaction.Current,
          new ObjectID (typeof (SampleBindableDomainObject), Guid.NewGuid()));

      var property = GetProperty (instance);
      var strategy = (IBindablePropertyReadAccessStrategy) new BindableDomainObjectPropertyReadAccessStrategy();

      Assert.That (instance.State, Is.EqualTo (StateType.NotLoadedYet));

      var result = strategy.CanRead (instance, property);

      Assert.That (result, Is.False);
      Assert.That (instance.State, Is.EqualTo (StateType.Invalid));
    }

    [Test]
    public void CanRead_NotADomainObject_ReturnsTrue ()
    {
      var instance = MockRepository.GenerateStub<IBusinessObject>();
      var property = GetProperty (SampleBindableDomainObject.NewObject());

      var strategy = (IBindablePropertyReadAccessStrategy) new BindableDomainObjectPropertyReadAccessStrategy();
      var result = strategy.CanRead (instance, property);

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanRead_BusinessObjectIsNull_ReturnsTrue ()
    {
      var property = GetProperty (SampleBindableDomainObject.NewObject());

      var strategy = (IBindablePropertyReadAccessStrategy) new BindableDomainObjectPropertyReadAccessStrategy();
      var result = strategy.CanRead (null, property);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsPropertyAccessible_WithObjectInvalidException_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty (instance);
      var strategy = (IBindablePropertyReadAccessStrategy) new BindableDomainObjectPropertyReadAccessStrategy();
      var originalException = new ObjectInvalidException ("The Message");

      BusinessObjectPropertyAccessException actualException;
      var result = strategy.IsPropertyAccessException (instance, property, originalException, out actualException);
      Assert.That (result, Is.True);

      Assert.That (actualException, Is.Not.Null);
      Assert.That (
          actualException.Message,
          Is.EqualTo (
              "An ObjectInvalidException occured while getting the value of property 'Name' for business object with ID '" + instance.ID + "'."));
      Assert.That (actualException.InnerException, Is.SameAs (originalException));
    }

    [Test]
    public void IsPropertyAccessible_WithObjectDeletedException_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty (instance);
      var strategy = (IBindablePropertyReadAccessStrategy) new BindableDomainObjectPropertyReadAccessStrategy();
      var originalException = new ObjectDeletedException ("The Message");

      BusinessObjectPropertyAccessException actualException;
      var result = strategy.IsPropertyAccessException (instance, property, originalException, out actualException);
      Assert.That (result, Is.True);

      Assert.That (actualException, Is.Not.Null);
      Assert.That (
          actualException.Message,
          Is.EqualTo (
              "An ObjectDeletedException occured while getting the value of property 'Name' for business object with ID '" + instance.ID + "'."));
      Assert.That (actualException.InnerException, Is.SameAs (originalException));
    }

    [Test]
    public void IsPropertyAccessible_WithObjectsNotFoundException_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty (instance);
      var strategy = (IBindablePropertyReadAccessStrategy) new BindableDomainObjectPropertyReadAccessStrategy();
      var originalException = new ObjectsNotFoundException ("The Message", new ObjectID[0], null);

      BusinessObjectPropertyAccessException actualException;
      var result = strategy.IsPropertyAccessException (instance, property, originalException, out actualException);
      Assert.That (result, Is.True);

      Assert.That (actualException, Is.Not.Null);
      Assert.That (
          actualException.Message,
          Is.EqualTo (
              "An ObjectsNotFoundException occured while getting the value of property 'Name' for business object with ID '" + instance.ID + "'."));
      Assert.That (actualException.InnerException, Is.SameAs (originalException));
    }

    private PropertyBase GetProperty (IBusinessObject instance)
    {
      return (PropertyBase) instance.BusinessObjectClass.GetPropertyDefinition ("Name");
    }
  }
}