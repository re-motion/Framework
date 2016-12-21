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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class LoadedObjectDataPendingRegistrationCollectorTest : StandardMappingTest
  {
    private FreshlyLoadedObjectData _data1;
    private FreshlyLoadedObjectData _data2;
    private LoadedObjectDataPendingRegistrationCollector _collector;

    public override void SetUp ()
    {
      base.SetUp ();

      _data1 = LoadedObjectDataObjectMother.CreateFreshlyLoadedObjectData (DomainObjectIDs.Order1);
      _data2 = LoadedObjectDataObjectMother.CreateFreshlyLoadedObjectData (DomainObjectIDs.Order3);
      _collector = new LoadedObjectDataPendingRegistrationCollector();
    }

    [Test]
    public void Add_AddsObjectsToList_AndReturnsAddedObject ()
    {
      var result1 = _collector.Add (_data1);
      Assert.That (_collector.DataPendingRegistration, Is.EquivalentTo (new[] { _data1 }));
      Assert.That (result1, Is.SameAs (_data1));
      
      var result2 = _collector.Add (_data2);
      Assert.That (_collector.DataPendingRegistration, Is.EquivalentTo (new[] { _data1, _data2 }));
      Assert.That (result2, Is.SameAs (_data2));
    }

    [Test]
    public void Add_SameObject_DoesNotAddAgain ()
    {
      _collector.Add (_data1);
      _collector.Add (_data1);

      Assert.That (_collector.DataPendingRegistration, Is.EquivalentTo (new[] { _data1 }));
    }

    [Test]
    public void AddDataContainers_MultipleTimes_DifferentObjectWithSameObjectID_FirstObjectWins_AndIsReturnedBySecondCall ()
    {
      var alternativeData = LoadedObjectDataObjectMother.CreateFreshlyLoadedObjectData (_data1.ObjectID);

      _collector.Add (_data1);
      var resultOfSecondCall = _collector.Add (alternativeData);

      Assert.That (_collector.DataPendingRegistration, Is.EquivalentTo (new[] { _data1 }));
      Assert.That (resultOfSecondCall, Is.SameAs (_data1));
    }
  }
}