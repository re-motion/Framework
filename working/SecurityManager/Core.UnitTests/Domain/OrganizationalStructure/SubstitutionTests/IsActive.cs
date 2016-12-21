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
      Assert.That (_substitution.State, Is.Not.EqualTo (StateType.Unchanged));
      Assert.That (_substitution.IsActive, Is.False);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithoutTimeSpan ()
    {
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.True);
    }

    [Test]
    public void WithIsEnabledFalse_WithoutTimeSpan()
    {
      _substitution.IsEnabled = false;
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.False);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithBeginDateLessThanCurrentDate ()
    {
      _substitution.BeginDate = DateTime.Today.AddDays (-2);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithBeginDateSameAsCurrentDate ()
    {
      _substitution.BeginDate = DateTime.Today;
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithBeginDateSameAsCurrentDateButGreaterTime ()
    {
      _substitution.BeginDate = DateTime.Now.AddMinutes (1);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesFalse_WithIsEnabledTrue_WithBeginDateGreaterThanCurrentDate()
    {
      _substitution.BeginDate = DateTime.Today.AddDays (+2);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.False);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithEndDateSameAsCurrentDate ()
    {
      _substitution.EndDate = DateTime.Today;
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesTrue_WithIsEnabledTrue_WithEndDateSameAsCurrentDateButLessTime()
    {
      _substitution.EndDate = DateTime.Now.AddMinutes (-1);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.True);
    }

    [Test]
    public void EvaluatesFalse_WithIsEnabledTrue_WithEndDateLessThanCurrentDate()
    {
      _substitution.EndDate = DateTime.Today.AddDays (-2);
      TestHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      Assert.That (_substitution.IsActive, Is.False);
    }
  }
}
