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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class AddToOneToManyRelationTest : ClientTransactionBaseTest
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
      _subordinate = DomainObjectIDs.Employee2.GetObject<Employee> ();

      _supervisorEventReceiver = new DomainObjectEventReceiver (_supervisor);
      _subordinateEventReceiver = new DomainObjectEventReceiver (_subordinate);
      _subordinateCollectionEventReceiver = new DomainObjectCollectionEventReceiver (_supervisor.Subordinates);
    }

    [Test]
    public void AddEvents ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      _supervisor.Subordinates.Add (_subordinate);

      Assert.That (_subordinateEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
      Assert.That (_subordinateEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (true));
      Assert.That (_subordinateEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
      Assert.That (_subordinateEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
      Assert.That (_subordinateEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That (_subordinateEventReceiver.ChangingNewRelatedObject, Is.SameAs (_supervisor));
      Assert.That (_subordinateEventReceiver.ChangedOldRelatedObject, Is.Null);
      Assert.That (_subordinateEventReceiver.ChangedNewRelatedObject, Is.SameAs (_supervisor));

      Assert.That (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled, Is.True);
      Assert.That (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled, Is.True);
      Assert.That (_subordinateCollectionEventReceiver.AddingDomainObject, Is.SameAs (_subordinate));
      Assert.That (_subordinateCollectionEventReceiver.AddedDomainObject, Is.SameAs (_subordinate));

      Assert.That (_supervisorEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
      Assert.That (_supervisorEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (true));
      Assert.That (_supervisorEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates"));
      Assert.That (_supervisorEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates"));
      Assert.That (_supervisorEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That (_supervisorEventReceiver.ChangingNewRelatedObject, Is.SameAs (_subordinate));
      Assert.That (_supervisorEventReceiver.ChangedOldRelatedObject, Is.Null);
      Assert.That (_supervisorEventReceiver.ChangedNewRelatedObject, Is.SameAs (_subordinate));

      Assert.That (_subordinate.State, Is.EqualTo (StateType.Changed));
      Assert.That (_supervisor.State, Is.EqualTo (StateType.Changed));
      Assert.That (_supervisor.Subordinates[_subordinate.ID], Is.Not.Null);
      Assert.That (_supervisor.Subordinates.IndexOf (_subordinate), Is.EqualTo (_supervisor.Subordinates.Count - 1));
      Assert.That (_subordinate.Supervisor, Is.SameAs (_supervisor));
    }


    [Test]
    public void SubordinateCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = true;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      try
      {
        _supervisor.Subordinates.Add (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_subordinateEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
        Assert.That (_subordinateEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
        Assert.That (_subordinateEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
        Assert.That (_subordinateEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_subordinateEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_subordinateEventReceiver.ChangingNewRelatedObject, Is.SameAs (_supervisor));

        Assert.That (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled, Is.False);
        Assert.That (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled, Is.False);
        Assert.That (_subordinateCollectionEventReceiver.AddingDomainObject, Is.Null);
        Assert.That (_subordinateCollectionEventReceiver.AddedDomainObject, Is.Null);

        Assert.That (_supervisorEventReceiver.HasRelationChangingEventBeenCalled, Is.False);
        Assert.That (_supervisorEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
        Assert.That (_supervisorEventReceiver.ChangingRelationPropertyName, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_subordinate.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_supervisor.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_supervisor.Subordinates.Count, Is.EqualTo (2));
        Assert.That (_subordinate.Supervisor, Is.Null);
      }
    }


    [Test]
    public void SubOrdinateCollectionCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = true;
      _supervisorEventReceiver.Cancel = false;

      try
      {
        _supervisor.Subordinates.Add (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_subordinateEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
        Assert.That (_subordinateEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
        Assert.That (_subordinateEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
        Assert.That (_subordinateEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_subordinateEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_subordinateEventReceiver.ChangingNewRelatedObject, Is.SameAs (_supervisor));

        Assert.That (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled, Is.True);
        Assert.That (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled, Is.False);
        Assert.That (_subordinateCollectionEventReceiver.AddingDomainObject, Is.SameAs (_subordinate));
        Assert.That (_subordinateCollectionEventReceiver.AddedDomainObject, Is.Null);

        Assert.That (_supervisorEventReceiver.HasRelationChangingEventBeenCalled, Is.False);
        Assert.That (_supervisorEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
        Assert.That (_supervisorEventReceiver.ChangingRelationPropertyName, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangingNewRelatedObject, Is.Null);

        Assert.That (_subordinate.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_supervisor.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_supervisor.Subordinates.Count, Is.EqualTo (2));
        Assert.That (_subordinate.Supervisor, Is.Null);
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
        _supervisor.Subordinates.Add (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_subordinateEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
        Assert.That (_subordinateEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
        Assert.That (_subordinateEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
        Assert.That (_subordinateEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_subordinateEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_subordinateEventReceiver.ChangingNewRelatedObject, Is.SameAs (_supervisor));

        Assert.That (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled, Is.True);
        Assert.That (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled, Is.False);
        Assert.That (_subordinateCollectionEventReceiver.AddingDomainObject, Is.SameAs (_subordinate));
        Assert.That (_subordinateCollectionEventReceiver.AddedDomainObject, Is.Null);

        Assert.That (_supervisorEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (_supervisorEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
        Assert.That (_supervisorEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates"));
        Assert.That (_supervisorEventReceiver.ChangedRelationPropertyName, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangingOldRelatedObject, Is.Null);
        Assert.That (_supervisorEventReceiver.ChangingNewRelatedObject, Is.SameAs (_subordinate));

        Assert.That (_subordinate.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_supervisor.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_supervisor.Subordinates.Count, Is.EqualTo (2));
        Assert.That (_subordinate.Supervisor, Is.Null);
      }
    }

    [Test]
    public void StateTracking ()
    {
      Assert.That (_supervisor.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_subordinate.State, Is.EqualTo (StateType.Unchanged));

      _supervisor.Subordinates.Add (_subordinate);

      Assert.That (_supervisor.State, Is.EqualTo (StateType.Changed));
      Assert.That (_subordinate.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void RelationEndPointMap ()
    {
      _supervisor.Subordinates.Add (_subordinate);

      Assert.That (_subordinate.Supervisor, Is.SameAs (_supervisor));
    }

    [Test]
    public void SetPropertyValue ()
    {
      _supervisor.Subordinates.Add (_subordinate);

      Assert.That (_subordinate.Properties[typeof (Employee), "Supervisor"].GetRelatedObjectID(), Is.EqualTo (_supervisor.ID));
    }

    [Test]
    public void SetSupervisor ()
    {
      _subordinate.Supervisor = _supervisor;

      Assert.That (_subordinate.Supervisor, Is.SameAs (_supervisor));
      Assert.That (_supervisor.Subordinates.Count, Is.EqualTo (3));
      Assert.That (_supervisor.Subordinates[_subordinate.ID], Is.Not.Null);
    }

    [Test]
    public void SetSameSupervisor ()
    {
      Employee employeeWithSupervisor = _supervisor.Subordinates[DomainObjectIDs.Employee4];
      employeeWithSupervisor.Supervisor = _supervisor;

      Assert.That (_supervisor.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (employeeWithSupervisor.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void SetSupervisorNull ()
    {
      _subordinate.Supervisor = null;

      // expectation: no exception
    }

    [Test]
    public void InsertEvents ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      _supervisor.Subordinates.Insert (0, _subordinate);

      Assert.That (_supervisor.Subordinates.IndexOf (_subordinate), Is.EqualTo (0));
      Assert.That (_subordinateEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
      Assert.That (_subordinateEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (true));
      Assert.That (_subordinateEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
      Assert.That (_subordinateEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
      Assert.That (_subordinateEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That (_subordinateEventReceiver.ChangingNewRelatedObject, Is.SameAs (_supervisor));
      Assert.That (_subordinateEventReceiver.ChangedOldRelatedObject, Is.Null);
      Assert.That (_subordinateEventReceiver.ChangedNewRelatedObject, Is.SameAs (_supervisor));

      Assert.That (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled, Is.True);
      Assert.That (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled, Is.True);
      Assert.That (_subordinateCollectionEventReceiver.AddingDomainObject, Is.SameAs (_subordinate));
      Assert.That (_subordinateCollectionEventReceiver.AddedDomainObject, Is.SameAs (_subordinate));

      Assert.That (_supervisorEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo (true));
      Assert.That (_supervisorEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo (true));
      Assert.That (_supervisorEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates"));
      Assert.That (_supervisorEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates"));
      Assert.That (_supervisorEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That (_supervisorEventReceiver.ChangingNewRelatedObject, Is.SameAs (_subordinate));
      Assert.That (_supervisorEventReceiver.ChangedOldRelatedObject, Is.Null);
      Assert.That (_supervisorEventReceiver.ChangedNewRelatedObject, Is.SameAs (_subordinate));

      Assert.That (_subordinate.State, Is.EqualTo (StateType.Changed));
      Assert.That (_supervisor.State, Is.EqualTo (StateType.Changed));
      Assert.That (_supervisor.Subordinates[_subordinate.ID], Is.Not.Null);
      Assert.That (_subordinate.Supervisor, Is.SameAs (_supervisor));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The collection already contains an object with ID 'Employee|c3b2bbc3-e083-4974-bac7-9cee1fb85a5e|System.Guid'.\r\n"
        + "Parameter name: domainObject")]
    public void AddObjectAlreadyInCollection ()
    {
      _supervisor.Subordinates.Add (_subordinate);
      _supervisor.Subordinates.Add (_subordinate);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The collection already contains an object with ID 'Employee|c3b2bbc3-e083-4974-bac7-9cee1fb85a5e|System.Guid'.\r\n"
        + "Parameter name: domainObject")]
    public void InsertObjectAlreadyInCollection ()
    {
      _supervisor.Subordinates.Insert (0, _subordinate);
      _supervisor.Subordinates.Insert (0, _subordinate);
    }

    [Test]
    public void AddSubordinateWithOldSupervisor ()
    {
      Employee subordinate = DomainObjectIDs.Employee3.GetObject<Employee> ();
      Employee oldSupervisorOfSubordinate = DomainObjectIDs.Employee2.GetObject<Employee> ();

      var subordinateEventReceiver = new DomainObjectEventReceiver (subordinate);
      subordinateEventReceiver.Cancel = false;

      var oldSupervisorEventReceiver = new DomainObjectEventReceiver (oldSupervisorOfSubordinate);
      oldSupervisorEventReceiver.Cancel = false;

      var oldSupervisorSubordinateCollectionEventReceiver = new DomainObjectCollectionEventReceiver (oldSupervisorOfSubordinate.Subordinates);
      oldSupervisorSubordinateCollectionEventReceiver.Cancel = false;

      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      _supervisor.Subordinates.Add (subordinate);

      Assert.That (oldSupervisorEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That (oldSupervisorEventReceiver.HasRelationChangedEventBeenCalled, Is.True);
      Assert.That (oldSupervisorEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates"));
      Assert.That (oldSupervisorEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates"));

      Assert.That (oldSupervisorSubordinateCollectionEventReceiver.HasRemovingEventBeenCalled, Is.True);
      Assert.That (oldSupervisorSubordinateCollectionEventReceiver.HasRemovedEventBeenCalled, Is.True);
      Assert.That (oldSupervisorSubordinateCollectionEventReceiver.RemovingDomainObjects.Count, Is.EqualTo (1));
      Assert.That (oldSupervisorSubordinateCollectionEventReceiver.RemovingDomainObjects[0], Is.SameAs (subordinate));
      Assert.That (oldSupervisorSubordinateCollectionEventReceiver.RemovingDomainObjects.Count, Is.EqualTo (1));
      Assert.That (oldSupervisorSubordinateCollectionEventReceiver.RemovingDomainObjects[0], Is.SameAs (subordinate));


      Assert.That (subordinateEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That (subordinateEventReceiver.HasRelationChangedEventBeenCalled, Is.True);
      Assert.That (subordinateEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
      Assert.That (subordinateEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
      Assert.That (subordinateEventReceiver.ChangingOldRelatedObject, Is.SameAs (oldSupervisorOfSubordinate));
      Assert.That (subordinateEventReceiver.ChangingNewRelatedObject, Is.SameAs (_supervisor));
      Assert.That (subordinateEventReceiver.ChangedOldRelatedObject, Is.SameAs (oldSupervisorOfSubordinate));
      Assert.That (subordinateEventReceiver.ChangedNewRelatedObject, Is.SameAs (_supervisor));

      Assert.That (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled, Is.True);
      Assert.That (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled, Is.True);
      Assert.That (_subordinateCollectionEventReceiver.AddingDomainObject, Is.SameAs (subordinate));
      Assert.That (_subordinateCollectionEventReceiver.AddedDomainObject, Is.SameAs (subordinate));

      Assert.That (_supervisorEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That (_supervisorEventReceiver.HasRelationChangedEventBeenCalled, Is.True);
      Assert.That (_supervisorEventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates"));
      Assert.That (_supervisorEventReceiver.ChangedRelationPropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates"));
      Assert.That (_supervisorEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That (_supervisorEventReceiver.ChangingNewRelatedObject, Is.SameAs (subordinate));
      Assert.That (_supervisorEventReceiver.ChangedOldRelatedObject, Is.Null);
      Assert.That (_supervisorEventReceiver.ChangedNewRelatedObject, Is.SameAs (subordinate));

      Assert.That (subordinate.State, Is.EqualTo (StateType.Changed));
      Assert.That (_supervisor.State, Is.EqualTo (StateType.Changed));
      Assert.That (oldSupervisorOfSubordinate.State, Is.EqualTo (StateType.Changed));

      Assert.That (_supervisor.Subordinates[subordinate.ID], Is.Not.Null);
      Assert.That (_supervisor.Subordinates.IndexOf (subordinate), Is.EqualTo (_supervisor.Subordinates.Count - 1));
      Assert.That (oldSupervisorOfSubordinate.Subordinates.ContainsObject (subordinate), Is.False);
      Assert.That (subordinate.Supervisor, Is.SameAs (_supervisor));
    }
  }
}
