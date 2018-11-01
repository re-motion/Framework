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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class ConcurrencyViolationTest : ClientTransactionBaseTest
  {
    [Test]
    public void ConcurrencyViolationException_WhenSomebodyElseModifiesData ()
    {
      SetDatabaseModifyable();

      var computer = DomainObjectIDs.Computer1.GetObject<Computer> ();
      computer.SerialNumber = "100";

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var computerInOtherTransaction = DomainObjectIDs.Computer1.GetObject<Computer> ();
        computerInOtherTransaction.SerialNumber = "200";
        ClientTransaction.Current.Commit ();
      }

      try
      {
        TestableClientTransaction.Commit ();
        Assert.Fail ("Expected ConcurrencyViolationException");
      }
      catch (ConcurrencyViolationException)
      {
        // succeed
      }
    }

    [Test]
    public void ConcurrencyViolationException_WhenSomebodyElseRegistersForCommit ()
    {
      SetDatabaseModifyable ();

      var computer = DomainObjectIDs.Computer1.GetObject<Computer> ();
      computer.RegisterForCommit ();

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var computerInOtherTransaction = DomainObjectIDs.Computer1.GetObject<Computer> ();
        computerInOtherTransaction.RegisterForCommit ();
        ClientTransaction.Current.Commit ();
      }

      try
      {
        TestableClientTransaction.Commit ();
        Assert.Fail ("Expected ConcurrencyViolationException");
      }
      catch (ConcurrencyViolationException)
      {
        // succeed
      }
    }
  }
}
