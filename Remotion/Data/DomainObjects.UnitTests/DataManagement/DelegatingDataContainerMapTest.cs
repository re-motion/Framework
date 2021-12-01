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
using System.Collections.Generic;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Development.RhinoMocks.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DelegatingDataContainerMapTest : StandardMappingTest
  {
    [Test]
    public void DelegatingMembers ()
    {
      var objectID = DomainObjectIDs.Order1;
      var dataContainer = DataContainer.CreateNew(objectID);
      var enumeratorGeneric = new Mock<IEnumerator<DataContainer>>();
      var enumeratorObject = new Mock<IEnumerator>();

      CheckDelegation(dm => dm.Count, 5);
      CheckDelegation(dm => dm[objectID], dataContainer);
      CheckDelegation(dm => dm.GetEnumerator(), enumeratorGeneric.Object);
      CheckDelegation(dm => ((IEnumerable)dm).GetEnumerator(), enumeratorObject.Object);
    }

    private void CheckDelegation<TR> (Func<IDataContainerMapReadOnlyView, TR> func, TR fakeResult)
    {
      var innerMock = new Mock<IDataContainerMapReadOnlyView> (MockBehavior.Strict);
      var delegatingDataContainerMap = new DelegatingDataContainerMap();
      delegatingDataContainerMap.InnerDataContainerMap = innerMock.Object;

      var helper = new DecoratorTestHelper<IDataContainerMapReadOnlyView>(delegatingDataContainerMap, innerMock.Object);
      helper.CheckDelegation(func, fakeResult);

      delegatingDataContainerMap.InnerDataContainerMap = null;
      Assert.That(
          () => func(delegatingDataContainerMap),
          Throws.InvalidOperationException.With.Message.EqualTo("InnerDataContainerMap property must be set before it can be used."));
    }
  }
}
