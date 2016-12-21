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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Loading
{
  public class CanceledLoadOperationTest : ClientTransactionBaseTest
  {
    private IClientTransactionListener _listenerDynamicMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _listenerDynamicMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock (TestableClientTransaction);
    }

    [Test]
    public void ExceptionInOnLoading_StillAllowsSubsequentLoading ()
    {
      var exception = new Exception ("Test");

      // First, load an object and throw in ObjectsLoading.

      _listenerDynamicMock
          .Expect (mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything))
          .Throw (exception);
      
      var abortedDomainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObjectReference<ClassWithAllDataTypes> ();
      Assert.That (() => abortedDomainObject.EnsureDataAvailable(), Throws.Exception.SameAs (exception));

      // Now, try again. It now works.

      abortedDomainObject.EnsureDataAvailable();

      Assert.That (abortedDomainObject.State, Is.EqualTo (StateType.Unchanged));
    }

  }
}