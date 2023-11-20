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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class RootTransactionCommitStateTransitionTest : ClientTransactionStateTransitionBaseTest
  {
    [Test]
    public void CommitRootChanged ()
    {
      Order obj = GetUnchanged();
      ++obj.OrderNumber;
      Assert.That(obj.State.IsChanged, Is.True);
      Assert.That(obj.State.IsNewInHierarchy, Is.False);

      ClientTransactionScope.CurrentTransaction.Commit();

      Assert.That(obj.State.IsChanged, Is.False);
      Assert.That(obj.State.IsNewInHierarchy, Is.False);
    }

    [Test]
    public void CommitRootDeleted ()
    {
      Order obj = GetUnchanged();
      FullyDeleteOrder(obj);
      Assert.That(obj.State.IsDeleted, Is.True);

      ClientTransactionScope.CurrentTransaction.Commit();

      Assert.That(obj.State.IsInvalid, Is.True);
    }

    [Test]
    public void CommitRootNew ()
    {
      ClassWithAllDataTypes obj = GetNewUnchanged();
      obj.PopulateMandatoryProperties();
      obj.DateProperty = DateTime.Today;
      obj.DateTimeProperty = DateTime.Now;
      Assert.That(obj.State.IsNew, Is.True);
      Assert.That(obj.State.IsNewInHierarchy, Is.True);

      ClientTransactionScope.CurrentTransaction.Commit();

      Assert.That(obj.State.IsUnchanged, Is.True);
      Assert.That(obj.State.IsNew, Is.False);
      Assert.That(obj.State.IsNewInHierarchy, Is.False);
    }
  }
}
