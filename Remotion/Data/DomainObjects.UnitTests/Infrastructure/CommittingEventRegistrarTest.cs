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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class CommittingEventRegistrarTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;

    private DomainObject _newDomainObject;
    private DomainObject _changedObject;
    private DomainObject _deletedObject;
    private DomainObject _invalidObject;
    private DomainObject _unchangedObject;
    private DomainObject _notLoadedYetObject;

    private CommittingEventRegistrar _registrar;

    public override void SetUp ()
    {
      base.SetUp();

      _clientTransaction = ClientTransaction.CreateRootTransaction();

      _newDomainObject = _clientTransaction.ExecuteInScope(() => Order.NewObject());
      _changedObject = _clientTransaction.ExecuteInScope(() =>
      {
        var instance = DomainObjectIDs.Order1.GetObject<Order>();
        instance.RegisterForCommit();
        return instance;
      });
      _deletedObject = _clientTransaction.ExecuteInScope(() =>
      {
        var instance = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
        instance.Delete();
        return instance;
      });
      _invalidObject = _clientTransaction.ExecuteInScope(() =>
      {
        var instance = Order.NewObject();
        instance.Delete();
        return instance;
      });
      _unchangedObject = _clientTransaction.ExecuteInScope(() => DomainObjectIDs.Order4.GetObject<Order>());
      _notLoadedYetObject = (DomainObject)LifetimeService.GetObjectReference(_clientTransaction, DomainObjectIDs.Order5);

      _registrar = new CommittingEventRegistrar(_clientTransaction);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_registrar.RegisteredObjects, Is.Empty);
    }

    [Test]
    public void RegisterForAdditionalCommittingEvents ()
    {
      Assert.That(_registrar.RegisteredObjects, Is.Empty);

      _registrar.RegisterForAdditionalCommittingEvents(_newDomainObject, _changedObject, _newDomainObject);

      Assert.That(_registrar.RegisteredObjects, Is.EquivalentTo(new[] { _newDomainObject, _changedObject }));

      _registrar.RegisterForAdditionalCommittingEvents(_changedObject, _deletedObject);

      Assert.That(_registrar.RegisteredObjects, Is.EquivalentTo(new[] { _newDomainObject, _changedObject, _deletedObject }));
    }

    [Test]
    public void RegisterForAdditionalCommittingEvents_NonRegisterableObjects ()
    {
      Assert.That(
          () => _registrar.RegisterForAdditionalCommittingEvents(_invalidObject),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              string.Format(
                  "The given DomainObject '{0}' cannot be registered due to its DomainObjectState (Invalid). Only objects that are part of the commit "
                  + "set can be registered. Use RegisterForCommit to add an unchanged object to the commit set.",
                  _invalidObject.ID),
              "domainObjects"));

      Assert.That(
          () => _registrar.RegisterForAdditionalCommittingEvents(_unchangedObject),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
            string.Format(
                  "The given DomainObject '{0}' cannot be registered due to its DomainObjectState (Unchanged). Only objects that are part of the commit "
                  + "set can be registered. Use RegisterForCommit to add an unchanged object to the commit set.",
                  _unchangedObject.ID),
            "domainObjects"));

      Assert.That(
          () => _registrar.RegisterForAdditionalCommittingEvents(_notLoadedYetObject),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
            string.Format(
                  "The given DomainObject '{0}' cannot be registered due to its DomainObjectState (NotLoadedYet). Only objects that are part of the commit "
                  + "set can be registered. Use RegisterForCommit to add an unchanged object to the commit set.",
                  _notLoadedYetObject.ID),
                  "domainObjects"));
    }
  }
}
