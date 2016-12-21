// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation
{
  [TestFixture]
  public class AccessTypeStatisticsTest 
  {
    private AccessControlTestHelper _testHelper;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new AccessControlTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope ();
    }

    [TearDown]
    public virtual void TearDown ()
    {
      ClientTransactionScope.ResetActiveScope ();
    }


    [Test]
    public void AddAccessTypesSupplyingAceAndIsInAccessTypesSupplyingAcesTest ()
    {
      AccessTypeStatistics accessTypeStatistics = new AccessTypeStatistics();
      var ace = _testHelper.CreateAceWithAbstractRole();
      var ace2 = _testHelper.CreateAceWithoutGroupCondition();

      Assert.That (accessTypeStatistics.IsInAccessTypesContributingAces (ace), Is.False);
      Assert.That (accessTypeStatistics.IsInAccessTypesContributingAces (ace2), Is.False);

      accessTypeStatistics.AddAccessTypesContributingAce (ace);
      Assert.That (accessTypeStatistics.IsInAccessTypesContributingAces (ace), Is.True);
      Assert.That (accessTypeStatistics.IsInAccessTypesContributingAces (ace2), Is.False);

      accessTypeStatistics.AddAccessTypesContributingAce (ace2);
      Assert.That (accessTypeStatistics.IsInAccessTypesContributingAces (ace), Is.True);
      Assert.That (accessTypeStatistics.IsInAccessTypesContributingAces (ace2), Is.True);
    }


    [Test]
    public void AddMatchingAceAndIsInMatchingAcesTest ()
    {
      AccessTypeStatistics accessTypeStatistics = new AccessTypeStatistics ();
      var ace = _testHelper.CreateAceWithAbstractRole ();
      var ace2 = _testHelper.CreateAceWithoutGroupCondition ();

      Assert.That (accessTypeStatistics.IsInMatchingAces (ace), Is.False);
      Assert.That (accessTypeStatistics.IsInMatchingAces (ace2), Is.False);

      accessTypeStatistics.AddMatchingAce (ace);
      Assert.That (accessTypeStatistics.IsInMatchingAces (ace), Is.True);
      Assert.That (accessTypeStatistics.IsInMatchingAces (ace2), Is.False);

      accessTypeStatistics.AddMatchingAce (ace2);
      Assert.That (accessTypeStatistics.IsInMatchingAces (ace), Is.True);
      Assert.That (accessTypeStatistics.IsInMatchingAces (ace2), Is.True);
    }

  }
}
