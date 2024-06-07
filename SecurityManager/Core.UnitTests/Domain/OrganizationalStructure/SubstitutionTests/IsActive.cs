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
using System.Threading;
using NUnit.Framework;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.SubstitutionTests
{
  [TestFixture]
  public class IsActive : SubstitutionTestBase
  {
    private Substitution _substitution;

    public override void SetUp ()
    {
      base.SetUp();

      _substitution = Substitution.NewObject();
    }

    [Test]
    public void EvaluatesFalse_BeforeCommit ()
    {
      Assert.That(_substitution.State.IsUnchanged, Is.False);
      Assert.That(_substitution.IsActive, Is.False);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithoutTimeSpan ()
    {
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That(_substitution.IsActive, Is.True);
    }

    [Test]
    public void WithIsEnabledFalse_WithoutTimeSpan ()
    {
      _substitution.IsEnabled = false;
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That(_substitution.IsActive, Is.False);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithBeginDateLessThanCurrentDate ()
    {
      _substitution.BeginDate = DateTime.Today.AddDays(-2);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That(_substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithBeginDateSameAsCurrentDate ()
    {
      // Guard midnight
      if (DateTime.Now.AddSeconds(10).Date != DateTime.Now.Date)
        Thread.Sleep(TimeSpan.FromSeconds(10));

      _substitution.BeginDate = DateTime.Today;
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That(_substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithBeginDateSameAsCurrentDateButGreaterTime ()
    {
      // Guard midnight
      if (DateTime.Now.AddMinutes(1).Date != DateTime.Now.Date)
        Thread.Sleep(TimeSpan.FromMinutes(2));

      _substitution.BeginDate = DateTime.Now.AddMinutes(1);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That(_substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesFalse_WithIsEnabledTrue_WithBeginDateGreaterThanCurrentDate ()
    {
      _substitution.BeginDate = DateTime.Today.AddDays(+2);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That(_substitution.IsActive, Is.False);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithEndDateSameAsCurrentDate ()
    {
      // Guard midnight
      if (DateTime.Now.AddSeconds(10).Date != DateTime.Now.Date)
        Thread.Sleep(TimeSpan.FromSeconds(10));

      _substitution.EndDate = DateTime.Today;
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That(_substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithEndDateSameAsCurrentDateButLessTime ()
    {
      // Guard midnight
      if (DateTime.Now.AddMinutes(-1).Date != DateTime.Now.Date)
        Thread.Sleep(TimeSpan.FromMinutes(2));

      _substitution.EndDate = DateTime.Now.AddMinutes(-1);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That(_substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesFalse_WithIsEnabledTrue_WithEndDateLessThanCurrentDate ()
    {
      _substitution.EndDate = DateTime.Today.AddDays(-2);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That(_substitution.IsActive, Is.False);
    }
  }
}
