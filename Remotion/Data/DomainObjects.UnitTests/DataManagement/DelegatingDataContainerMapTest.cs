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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Development.RhinoMocks.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DelegatingDataContainerMapTest : StandardMappingTest
  {
    [Test]
    public void DelegatingMembers ()
    {
      var objectID = DomainObjectIDs.Order1;
      var dataContainer = DataContainer.CreateNew (objectID);
      var enumeratorGeneric = MockRepository.GenerateStub<IEnumerator<DataContainer>>();
      var enumeratorObject = MockRepository.GenerateStub<IEnumerator>();

      CheckDelegation (dm => dm.Count, 5);
      CheckDelegation (dm => dm[objectID], dataContainer);
      CheckDelegation (dm => dm.GetEnumerator(), enumeratorGeneric);
      CheckDelegation (dm => ((IEnumerable) dm).GetEnumerator(), enumeratorObject);
    }

    private void CheckDelegation<TR> (Func<IDataContainerMapReadOnlyView, TR> func, TR fakeResult)
    {
      var innerMock = MockRepository.GenerateStrictMock<IDataContainerMapReadOnlyView>();
      var delegatingDataContainerMap = new DelegatingDataContainerMap();
      delegatingDataContainerMap.InnerDataContainerMap = innerMock;

      var helper = new DecoratorTestHelper<IDataContainerMapReadOnlyView> (delegatingDataContainerMap, innerMock);
      helper.CheckDelegation (func, fakeResult);

      delegatingDataContainerMap.InnerDataContainerMap = null;
      Assert.That (
          () => func (delegatingDataContainerMap),
          Throws.InvalidOperationException.With.Message.EqualTo ("InnerDataContainerMap property must be set before it can be used."));
    }
  }
}