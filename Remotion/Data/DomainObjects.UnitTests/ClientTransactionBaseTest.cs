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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public class ClientTransactionBaseTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private TestableClientTransaction _testableClientTransaction;
    private TestDataContainerObjectMother _testDataContainerObjectMother;

    // construction and disposing

    protected ClientTransactionBaseTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp();

      ReInitializeTransaction();
    }

    public override void TearDown ()
    {
      base.TearDown();
      _testDataContainerObjectMother = null;
      DisposeTransaction();
    }

    protected TestableClientTransaction TestableClientTransaction
    {
      get { return _testableClientTransaction; }
    }

    protected TestDataContainerObjectMother TestDataContainerObjectMother
    {
      get { return _testDataContainerObjectMother; }
    }

    private void DisposeTransaction ()
    {
      ClientTransactionScope.ResetActiveScope();
    }

    protected void ReInitializeTransaction ()
    {
      DisposeTransaction();

      _testableClientTransaction = new TestableClientTransaction();
      _testableClientTransaction.EnterDiscardingScope();
      _testDataContainerObjectMother = new TestDataContainerObjectMother();
    }

    protected void CheckIfObjectIsDeleted (ObjectID id)
    {
      try
      {
        DomainObject domainObject = id.GetObject<TestDomainBase>(includeDeleted: true);
        Assert.That(domainObject, Is.Null, string.Format("Object '{0}' was not deleted.", id));
      }
      catch (ObjectsNotFoundException)
      {
      }
    }
  }
}
