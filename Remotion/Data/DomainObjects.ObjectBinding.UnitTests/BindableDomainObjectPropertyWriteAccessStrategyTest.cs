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
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectPropertyWriteAccessStrategyTest : ObjectBindingTestBase
  {
    [Test]
    public void SupportedStateTypes ()
    {
      Assert.That(
          typeof(DomainObjectState).GetProperties(BindingFlags.Instance | BindingFlags.Public).Length,
          Is.EqualTo(11),
          "DomainObjectState enum has changed. BindableDomainObjectPropertyWriteAccessStrategy implementation must be updated accordingly.");
    }

    [Test]
    public void CanWrite_NewObject_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty(instance);
      var strategy = (IBindablePropertyWriteAccessStrategy)new BindableDomainObjectPropertyWriteAccessStrategy();

      Assert.That(instance.State.IsNew, Is.True);

      var result = strategy.CanWrite(instance, property);

      Assert.That(result, Is.True);
    }

    [Test]
    public void CanWrite_UnchangedObject_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty(instance);
      var strategy = (IBindablePropertyWriteAccessStrategy)new BindableDomainObjectPropertyWriteAccessStrategy();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        instance.EnsureDataAvailable();
        Assert.That(instance.State.IsUnchanged, Is.True);
        var result = strategy.CanWrite(instance, property);

        Assert.That(result, Is.True);
      }
    }

    [Test]
    public void CanWrite_ChangedObject_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty(instance);
      var strategy = (IBindablePropertyWriteAccessStrategy)new BindableDomainObjectPropertyWriteAccessStrategy();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        instance.Name = "new value";
        Assert.That(instance.State.IsChanged, Is.True);
        var result = strategy.CanWrite(instance, property);

        Assert.That(result, Is.True);
      }
    }

    [Test]
    public void CanWrite_InvalidObject_ReturnsFalse ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty(instance);
      var strategy = (IBindablePropertyWriteAccessStrategy)new BindableDomainObjectPropertyWriteAccessStrategy();

      LifetimeService.DeleteObject(instance.DefaultTransactionContext.ClientTransaction, instance);
      Assert.That(instance.State.IsInvalid, Is.True);

      var result = strategy.CanWrite(instance, property);

      Assert.That(result, Is.False);
    }

    [Test]
    public void CanWrite_DeletedObject_ReturnsFalse ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty(instance);
      var strategy = (IBindablePropertyWriteAccessStrategy)new BindableDomainObjectPropertyWriteAccessStrategy();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        LifetimeService.DeleteObject(instance.DefaultTransactionContext.ClientTransaction, instance);
        Assert.That(instance.State.IsDeleted, Is.True);

        var result = strategy.CanWrite(instance, property);

        Assert.That(result, Is.False);
      }
    }

    [Test]
    public void CanWrite_NotLoadedObject_LoadsExistingObject_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty(instance);
      var strategy = (IBindablePropertyWriteAccessStrategy)new BindableDomainObjectPropertyWriteAccessStrategy();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(instance.State.IsNotLoadedYet, Is.True);

        var result = strategy.CanWrite(instance, property);

        Assert.That(result, Is.True);
        Assert.That(instance.State.IsUnchanged, Is.True);
      }
    }

    [Test]
    public void CanWrite_NotLoadedObject_LoadsNonExistentObject_ReturnsFalse ()
    {
      var instance = (SampleBindableDomainObject)LifetimeService.GetObjectReference(
          ClientTransaction.Current,
          new ObjectID(typeof(SampleBindableDomainObject), Guid.NewGuid()));

      var property = GetProperty(instance);
      var strategy = (IBindablePropertyWriteAccessStrategy)new BindableDomainObjectPropertyWriteAccessStrategy();

      Assert.That(instance.State.IsNotLoadedYet, Is.True);

      var result = strategy.CanWrite(instance, property);

      Assert.That(result, Is.False);
      Assert.That(instance.State.IsInvalid, Is.True);
    }

    [Test]
    public void CanWrite_NotADomainObject_ReturnsTrue ()
    {
      var instance = new Mock<IBusinessObject>();
      var property = GetProperty(SampleBindableDomainObject.NewObject());

      var strategy = (IBindablePropertyWriteAccessStrategy)new BindableDomainObjectPropertyWriteAccessStrategy();
      var result = strategy.CanWrite(instance.Object, property);

      Assert.That(result, Is.True);
    }

    [Test]
    public void CanWrite_BusinessObjectIsNull_ReturnsTrue ()
    {
      var property = GetProperty(SampleBindableDomainObject.NewObject());

      var strategy = (IBindablePropertyWriteAccessStrategy)new BindableDomainObjectPropertyWriteAccessStrategy();
      var result = strategy.CanWrite(null, property);

      Assert.That(result, Is.True);
    }

    [Test]
    public void IsPropertyAccessible_WithObjectInvalidException_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty(instance);
      var strategy = (IBindablePropertyWriteAccessStrategy)new BindableDomainObjectPropertyWriteAccessStrategy();
      var originalException = new ObjectInvalidException("The Message");

      BusinessObjectPropertyAccessException actualException;
      var result = strategy.IsPropertyAccessException(instance, property, originalException, out actualException);
      Assert.That(result, Is.True);

      Assert.That(actualException, Is.Not.Null);
      Assert.That(
          actualException.Message,
          Is.EqualTo(
              "An ObjectInvalidException occured while setting the value of property 'Name' for business object with ID '" + instance.ID + "'."));
      Assert.That(actualException.InnerException, Is.SameAs(originalException));
    }

    [Test]
    public void IsPropertyAccessible_WithObjectDeletedException_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty(instance);
      var strategy = (IBindablePropertyWriteAccessStrategy)new BindableDomainObjectPropertyWriteAccessStrategy();
      var originalException = new ObjectDeletedException("The Message");

      BusinessObjectPropertyAccessException actualException;
      var result = strategy.IsPropertyAccessException(instance, property, originalException, out actualException);
      Assert.That(result, Is.True);

      Assert.That(actualException, Is.Not.Null);
      Assert.That(
          actualException.Message,
          Is.EqualTo(
              "An ObjectDeletedException occured while setting the value of property 'Name' for business object with ID '" + instance.ID + "'."));
      Assert.That(actualException.InnerException, Is.SameAs(originalException));
    }

    [Test]
    public void IsPropertyAccessible_WithObjectsNotFoundException_ReturnsTrue ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var property = GetProperty(instance);
      var strategy = (IBindablePropertyWriteAccessStrategy)new BindableDomainObjectPropertyWriteAccessStrategy();
      var originalException = new ObjectsNotFoundException("The Message", new ObjectID[0], null);

      BusinessObjectPropertyAccessException actualException;
      var result = strategy.IsPropertyAccessException(instance, property, originalException, out actualException);
      Assert.That(result, Is.True);

      Assert.That(actualException, Is.Not.Null);
      Assert.That(
          actualException.Message,
          Is.EqualTo(
              "An ObjectsNotFoundException occured while setting the value of property 'Name' for business object with ID '" + instance.ID + "'."));
      Assert.That(actualException.InnerException, Is.SameAs(originalException));
    }

    private PropertyBase GetProperty (IBusinessObject instance)
    {
      return (PropertyBase)instance.BusinessObjectClass.GetPropertyDefinition("Name");
    }
  }
}
