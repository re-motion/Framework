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
using System.Collections;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class RemoveFromOneToManyRelationTest : ClientTransactionBaseTest
  {
    private Employee _supervisor;
    private Employee _subordinate;

    private DomainObjectEventReceiver _supervisorEventReceiver;
    private DomainObjectEventReceiver _subordinateEventReceiver;
    private DomainObjectCollectionEventReceiver _subordinateCollectionEventReceiver;

    public override void SetUp ()
    {
      base.SetUp ();

      _supervisor = DomainObjectIDs.Employee1.GetObject<Employee> ();
      _subordinate = DomainObjectIDs.Employee4.GetObject<Employee> ();

      _supervisorEventReceiver = new DomainObjectEventReceiver (_supervisor);
      _subordinateEventReceiver = new DomainObjectEventReceiver (_subordinate);
      _subordinateCollectionEventReceiver = new DomainObjectCollectionEventReceiver (_supervisor.Subordinates);
    }

    [Test]
    public void ChangeEvents ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      _supervisor.Subordinates.Remove (_subordinate.ID);

      Assert.That (_subordinateEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
      Assert.That (_subordinateEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (true));
      Assert.That (_subordinateEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
      Assert.That (_subordinateEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
      Assert.That (_subordinateEventReceiver.ChangingOldRelatedObject, Is.SameAs (_supervisor));
      Assert.That (_subordinateEventReceiver.ChangingNewRelatedObject, Is.Null);
      Assert.That (_subordinateEventReceiver.ChangedOldRelatedObject, Is.SameAs (_supervisor));
      Assert.That (_subordinateEventReceiver.ChangedNewRelatedObject, Is.Null);

      Assert.That (_subordinateCollectionEventReceiver.HasRemovingEventBeenCalled, Is.True);
      Assert.That (_subordinateCollectionEventReceiver.HasRemovedEventBeenCalled, Is.True);
      Assert.That (_subordinateCollectionEventReceiver.RemovingDomainObjects[0], Is.SameAs (_subordinate));
      Assert.That (_subordinateCollectionEventReceiver.RemovedDomainObjects[0], Is.SameAs (_subordinate));

      Assert.That (_supervisorEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
      Assert.That (_supervisorEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (true));
      Assert.That (_supervisorEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates"));
      Assert.That (_supervisorEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates"));
      Assert.That (_supervisorEventReceiver.ChangingOldRelatedObject, Is.SameAs (_subordinate));
      Assert.That (_supervisorEventReceiver.ChangingNewRelatedObject, Is.Null);
      Assert.That (_supervisorEventReceiver.ChangedOldRelatedObject, Is.SameAs (_subordinate));
      Assert.That (_supervisorEventReceiver.ChangedNewRelatedObject, Is.Null);

      Assert.That (_subordinate.State, Is.EqualTo (StateType.Changed));
      Assert.That (_supervisor.State, Is.EqualTo (StateType.Changed));
      Assert.That (_supervisor.Subordinates[_subordinate.ID], Is.Null);
      Assert.That (_subordinate.Supervisor, Is.Null);
    }

    [Test]
    public void SubordinateCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = true;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      try
      {
        _supervisor.Subordinates.Remove (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_subordinateEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
        Assert.That (_subordinateEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
        Assert.That (_subordinateEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
        Assert.That (_subordinateEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_subordinateEventReceiver.ChangingOldRelatedObject, Is.SameAs (_supervisor));
        Assert.That (_subordinateEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_subordinateCollectionEventReceiver.HasRemovingEventBeenCalled, Is.False);
        Assert.That (_subordinateCollectionEventReceiver.HasRemovedEventBeenCalled, Is.False);
        Assert.That (_subordinateCollectionEventReceiver.RemovingDomainObjects.Count, Is.EqualTo (0));
        Assert.That (_subordinateCollectionEventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));

        Assert.That (_supervisorEventReceiver.HasRelationChangingEventBeenCalled, Is.False);
        Assert.That (_supervisorEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
        Assert.That (_supervisorEventReceiver.ChangingRelationPropertyName, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_subordinate.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_supervisor.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_supervisor.Subordinates[_subordinate.ID], Is.Not.Null);
        Assert.That (_subordinate.Supervisor, Is.SameAs (_supervisor));
      }
    }


    [Test]
    public void SubordinateCollectionCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = true;
      _supervisorEventReceiver.Cancel = false;

      try
      {
        _supervisor.Subordinates.Remove (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_subordinateEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
        Assert.That (_subordinateEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
        Assert.That (_subordinateEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
        Assert.That (_subordinateEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_subordinateEventReceiver.ChangingOldRelatedObject, Is.SameAs (_supervisor));
        Assert.That (_subordinateEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_subordinateCollectionEventReceiver.HasRemovingEventBeenCalled, Is.True);
        Assert.That (_subordinateCollectionEventReceiver.HasRemovedEventBeenCalled, Is.False);
        Assert.That (_subordinateCollectionEventReceiver.RemovingDomainObjects[0], Is.SameAs (_subordinate));
        Assert.That (_subordinateCollectionEventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));

        Assert.That (_supervisorEventReceiver.HasRelationChangingEventBeenCalled, Is.False);
        Assert.That (_supervisorEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
        Assert.That (_supervisorEventReceiver.ChangingRelationPropertyName, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_subordinate.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_supervisor.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_supervisor.Subordinates[_subordinate.ID], Is.Not.Null);
        Assert.That (_subordinate.Supervisor, Is.SameAs (_supervisor));
      }
    }

    [Test]
    public void SupervisorCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = true;

      try
      {
        _supervisor.Subordinates.Remove (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_subordinateEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
        Assert.That (_subordinateEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
        Assert.That (_subordinateEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
        Assert.That (_subordinateEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_subordinateEventReceiver.ChangingOldRelatedObject, Is.SameAs (_supervisor));
        Assert.That (_subordinateEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_subordinateCollectionEventReceiver.HasRemovingEventBeenCalled, Is.True);
        Assert.That (_subordinateCollectionEventReceiver.HasRemovedEventBeenCalled, Is.False);
        Assert.That (_subordinateCollectionEventReceiver.RemovingDomainObjects[0], Is.SameAs (_subordinate));
        Assert.That (_subordinateCollectionEventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));

        Assert.That (_supervisorEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (_supervisorEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
        Assert.That (_supervisorEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates"));
        Assert.That (_supervisorEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangingOldRelatedObject, Is.SameAs (_subordinate));
        Assert.That (_supervisorEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_subordinate.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_supervisor.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_supervisor.Subordinates[_subordinate.ID], Is.Not.Null);
        Assert.That (_subordinate.Supervisor, Is.SameAs (_supervisor));
      }
    }

    [Test]
    public void StateTracking ()
    {
      Assert.That (_supervisor.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_subordinate.State, Is.EqualTo (StateType.Unchanged));

      _supervisor.Subordinates.Remove (_subordinate);

      Assert.That (_supervisor.State, Is.EqualTo (StateType.Changed));
      Assert.That (_subordinate.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void RelationEndPointMap ()
    {
      _supervisor.Subordinates.Remove (_subordinate.ID);

      Assert.That (_subordinate.Supervisor, Is.Null);
    }

    [Test]
    public void SetPropertyValue ()
    {
      _supervisor.Subordinates.Remove (_subordinate);

      Assert.That (_subordinate.Properties[typeof (Employee), "Supervisor"].GetRelatedObjectID (), Is.Null);
    }

    [Test]
    public void SetSupervisorNull ()
    {
      _subordinate.Supervisor = null;

      Assert.That (_subordinate.Supervisor, Is.Null);
      Assert.That (_supervisor.Subordinates.Count, Is.EqualTo (1));
      Assert.That (_supervisor.Subordinates[_subordinate.ID], Is.Null);
    }

    [Test]
    public void SetNumericIndexerOfSupervisorNull ()
    {
      Employee employeeToRemove = (Employee) _supervisor.Subordinates[0];
      Employee unaffectedEmployee = (Employee) _supervisor.Subordinates[1];

      _supervisor.Subordinates[0] = null;

      Assert.That (_supervisor.Subordinates.Count, Is.EqualTo (1));
      Assert.That (_supervisor.Subordinates.ContainsObject (unaffectedEmployee), Is.True);
      Assert.That (_supervisor.Subordinates.ContainsObject (employeeToRemove), Is.False);

      Assert.That (unaffectedEmployee.Supervisor, Is.SameAs (_supervisor));
      Assert.That (employeeToRemove.Supervisor, Is.Null);
    }

    [Test]
    public void SetNumericIListIndexerOfSupervisorNull ()
    {
      Employee employeeToRemove = (Employee) _supervisor.Subordinates[0];
      Employee unaffectedEmployee = (Employee) _supervisor.Subordinates[1];

      IList list = _supervisor.Subordinates;
      list[0] = null;

      Assert.That (_supervisor.Subordinates.Count, Is.EqualTo (1));
      Assert.That (_supervisor.Subordinates.ContainsObject (unaffectedEmployee), Is.True);
      Assert.That (_supervisor.Subordinates.ContainsObject (employeeToRemove), Is.False);

      Assert.That (unaffectedEmployee.Supervisor, Is.SameAs (_supervisor));
      Assert.That (employeeToRemove.Supervisor, Is.Null);
    }
  }
}
