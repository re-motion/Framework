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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.CollectionData
{
  [TestFixture]
  public class DomainObjectCollectionDataTest : ClientTransactionBaseTest
  {
    private DomainObjectCollectionData _data;

    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;

    public override void SetUp ()
    {
      base.SetUp();
      _data = new DomainObjectCollectionData();
      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _order2 = DomainObjectIDs.Order2.GetObject<Order>();
      _order3 = DomainObjectIDs.Order3.GetObject<Order>();
      _order4 = DomainObjectIDs.Order4.GetObject<Order>();
    }

    [Test]
    public void Initialization ()
    {
      var data = new DomainObjectCollectionData();
      Assert.That(data.ToArray(), Is.Empty);
      Assert.That(data.Count, Is.EqualTo(0));
    }

    [Test]
    public void Initialization_WithValues ()
    {
      var data = new DomainObjectCollectionData(new[] { _order1, _order2, _order3 });
      Assert.That(data.ToArray(), Is.EqualTo(new[] { _order1, _order2, _order3 }));
      Assert.That(data.Count, Is.EqualTo(3));
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That(_data.IsReadOnly, Is.False);
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That(((IDomainObjectCollectionData)_data).IsDataComplete, Is.True);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      Assert.That(((IDomainObjectCollectionData)_data).IsDataComplete, Is.True);
      ((IDomainObjectCollectionData)_data).EnsureDataComplete();
      Assert.That(((IDomainObjectCollectionData)_data).IsDataComplete, Is.True);
    }

    [Test]
    public void AssociatedEndPoint ()
    {
      Assert.That(((IDomainObjectCollectionData)_data).AssociatedEndPointID, Is.Null);
    }

    [Test]
    public void RequiredItemType ()
    {
      Assert.That(((IDomainObjectCollectionData)_data).RequiredItemType, Is.Null);
    }

    [Test]
    public void Insert ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      _data.Insert(2, _order4);

      Assert.That(_data.ToArray(), Is.EqualTo(new[] { _order1, _order2, _order4, _order3 }));
    }

    [Test]
    public void Insert_SameItemTwice ()
    {
      Add(_order1);
      Assert.That(
          () => _data.Insert(1, _order1),
          Throws.ArgumentException);
    }

    [Test]
    public void Insert_SameItemTwice_DoesNotCorruptState ()
    {
      Add(_order1);

      try
      {
        _data.Insert(1, _order1);
      }
      catch (ArgumentException)
      {
      }

      Assert.That(PrivateInvoke.GetNonPublicField(_data, "_orderedObjectIDs"), Is.EqualTo(new[] { _order1.ID }));
      Assert.That(
          PrivateInvoke.GetNonPublicField(_data, "_objectsByID"),
          Is.EqualTo(new[] { new KeyValuePair<ObjectID, DomainObject>(_order1.ID, _order1) }));
    }

    [Test]
    public void Insert_InvalidIndex ()
    {
      Add(_order1);
      Assert.That(
          () => _data.Insert(17, _order2),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Insert_InvalidIndex_DoesNotCorruptState ()
    {
      Add(_order1);

      try
      {
        _data.Insert(17, _order2);
      }
      catch (ArgumentOutOfRangeException)
      {
      }

      CheckInternalState(_order1);
    }

    [Test]
    public void Insert_ChangesVersion ()
    {
      Add(_order1);

      CheckVersionChanged(() => _data.Insert(1, _order4));
    }

    [Test]
    public void ContainsObjectID_False ()
    {
      Assert.That(_data.ContainsObjectID(DomainObjectIDs.Order1), Is.False);
    }

    [Test]
    public void ContainsObjectID_True ()
    {
      Assert.That(_data.ContainsObjectID(DomainObjectIDs.Order1), Is.False);
      Add(_order1);
      Assert.That(_data.ContainsObjectID(DomainObjectIDs.Order1), Is.True);
    }

    [Test]
    public void GetObject_ByID ()
    {
      Add(_order1);
      Add(_order2);

      Assert.That(_data.GetObject(_order1.ID), Is.SameAs(_order1));
    }

    [Test]
    public void GetObject_InvalidID ()
    {
      Assert.That(_data.GetObject(_order1.ID), Is.Null);
    }

    [Test]
    public void GetObject_ByIndex ()
    {
      Add(_order1);
      Add(_order2);

      Assert.That(_data.GetObject(0), Is.SameAs(_order1));
      Assert.That(_data.GetObject(1), Is.SameAs(_order2));
    }

    [Test]
    public void GetObject_InvalidIndex ()
    {
      Add(_order1);
      Add(_order2);
      Assert.That(
          () => _data.GetObject(3),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void IndexOf ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      Assert.That(_data.IndexOf(_order1.ID), Is.EqualTo(0));
      Assert.That(_data.IndexOf(_order2.ID), Is.EqualTo(1));
      Assert.That(_data.IndexOf(_order3.ID), Is.EqualTo(2));
      Assert.That(_data.IndexOf(_order4.ID), Is.EqualTo(-1));
    }

    [Test]
    public void Clear ()
    {
      Add(_order1);
      Add(_order2);

      _data.Clear();

      Assert.That(_data.Count, Is.EqualTo(0));
      Assert.That(_data.ToArray(), Is.Empty);
    }

    [Test]
    public void Clear_AlsoClearsObjectsByID ()
    {
      Add(_order1);
      Add(_order2);

      _data.Clear();

      Assert.That(_data.ContainsObjectID(_order1.ID), Is.False);
    }

    [Test]
    public void Clear_ChangesVersion ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      CheckVersionChanged(() => _data.Clear());
    }

    [Test]
    public void Remove ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      bool result = _data.Remove(_order2);
      Assert.That(_data.ToArray(), Is.EqualTo(new[] { _order1, _order3 }));

      Assert.That(result, Is.True);
    }

    [Test]
    public void Remove_ChangesVersion ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      CheckVersionChanged(() => _data.Remove(_order3));
    }

    [Test]
    public void Remove_NonExistingElement ()
    {
      bool result = _data.Remove(_order2);

      Assert.That(_data.ToArray(), Is.Empty);
      Assert.That(result, Is.False);
    }

    [Test]
    public void Remove_NonExistingElement_NoVersionChange ()
    {
      long oldVersion = _data.Version;
      _data.Remove(_order2);
      Assert.That(_data.Version, Is.EqualTo(oldVersion));
    }

    [Test]
    public void Remove_ID ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      var result = _data.Remove(_order2.ID);
      Assert.That(_data.ToArray(), Is.EqualTo(new[] { _order1, _order3 }));
      Assert.That(result, Is.True);
    }

    [Test]
    public void Remove_ID_ChangesVersion ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      CheckVersionChanged(() => _data.Remove(_order3.ID));
    }

    [Test]
    public void Remove_ID_NonExistingElement ()
    {
      bool result = _data.Remove(_order2.ID);

      Assert.That(_data.ToArray(), Is.Empty);
      Assert.That(result, Is.False);
    }

    [Test]
    public void Remove_ID_NonExistingElement_NoVersionChange ()
    {
      long oldVersion = _data.Version;
      _data.Remove(_order2.ID);
      Assert.That(_data.Version, Is.EqualTo(oldVersion));
    }

    [Test]
    public void Replace ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      _data.Replace(1, _order4);

      Assert.That(_data.ToArray(), Is.EqualTo(new[] { _order1, _order4, _order3 }));
    }

    [Test]
    public void Replace_InvalidIndex ()
    {
      Assert.That(
          () => _data.Replace(1, _order4),
          Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Replace_ChangesVersion ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      CheckVersionChanged(() => _data.Replace(1, _order4));
    }

    [Test]
    public void Replace_NoVersionChange_OnSelfReplace ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      var oldVersion = _data.Version;
      _data.Replace(1, _order2);
      Assert.That(_data.Version, Is.EqualTo(oldVersion));
    }

    [Test]
    public void Replace_WithDuplicate ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);
      Assert.That(
          () => _data.Replace(1, _order1),
          Throws.ArgumentException);
    }

    [Test]
    public void Replace_WithDuplicate_DoesNotCorruptState ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      try
      {
        _data.Replace(1, _order1);
      }
      catch (ArgumentException)
      {
      }

      CheckInternalState(_order1, _order2, _order3);
    }

    [Test]
    public void Replace_WithSelf ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      _data.Replace(1, _order2);

      Assert.That(_data.ToArray(), Is.EqualTo(new[] { _order1, _order2, _order3 }));
    }

    [Test]
    public void Sort ()
    {
      _data.Add(_order1);
      _data.Add(_order2);
      _data.Add(_order3);
      _data.Add(_order4);

      var weights = new Dictionary<DomainObject, int> { { _order1, 3 }, { _order2, 2 }, { _order3, 0 }, { _order4, 1 } };

      _data.Sort((obj1, obj2) => weights[obj1].CompareTo(weights[obj2]));

      Assert.That(_data, Is.EqualTo(new[] { _order3, _order4, _order2, _order1 }));
    }

    [Test]
    public void Sort_ChangesVersion ()
    {
      CheckVersionChanged(() => _data.Sort((one, two) => 0));
    }

    [Test]
    public void Enumeration_ChokesOnVersionChanges ()
    {
      Add(_order1);
      Add(_order2);
      Add(_order3);

      var enumerator = _data.GetEnumerator();
      enumerator.MoveNext();

      Add(_order4);
      Assert.That(
          () => enumerator.MoveNext(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Collection was modified during enumeration."));
    }

    [Test]
    public void Enumeration_ChokesOnVersionChanges_Remove ()
    {
      Add(_order1);
      Add(_order2);

      Assert.That(
          () =>
          {
            foreach (var x in _data)
              _data.Remove(x);
          },
          Throws.InvalidOperationException
              .With.Message.EqualTo("Collection was modified during enumeration."));
    }

    private void Add (Order order)
    {
      _data.Insert(_data.Count, order);
    }

    private void CheckVersionChanged (Action action)
    {
      long version = _data.Version;
      action();
      Assert.That(_data.Version, Is.GreaterThan(version));
    }

    private void CheckInternalState (params DomainObject[] expectedData)
    {
      Assert.That(PrivateInvoke.GetNonPublicField(_data, "_orderedObjectIDs"), Is.EqualTo(expectedData.Select(obj => obj.ID).ToArray()));

      var keyValuePairs = expectedData.Select(obj => new KeyValuePair<ObjectID, DomainObject>(obj.ID, obj)).ToArray();
      Assert.That(PrivateInvoke.GetNonPublicField(_data, "_objectsByID"), Is.EqualTo(keyValuePairs));
    }
  }
}
