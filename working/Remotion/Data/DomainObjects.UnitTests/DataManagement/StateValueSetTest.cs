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
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class StateValueSetTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException), ExpectedMessage = 
        "Invalid StateType value.\r\nParameter name: stateValues\r\nActual value was -1.")]
    public void Initialization_InvalidEnum ()
    {
      new StateValueSet ((StateType) (-1));
    }

    [Test]
    public void Matches_Changed ()
    {
      CheckMatch (StateType.Changed, true, StateType.Unchanged, StateType.Changed);
      CheckMatch (StateType.Changed, false, StateType.Unchanged, StateType.New);
    }

    [Test]
    public void Matches_Unchanged ()
    {
      CheckMatch (StateType.Unchanged, true, StateType.Unchanged, StateType.Changed);
      CheckMatch (StateType.Unchanged, false, StateType.Changed, StateType.New);
    }

    [Test]
    public void Matches_New ()
    {
      CheckMatch (StateType.New, true, StateType.Unchanged, StateType.New);
      CheckMatch (StateType.New, false, StateType.Unchanged, StateType.Deleted);
    }

    [Test]
    public void Matches_Deleted ()
    {
      CheckMatch (StateType.Deleted, true, StateType.Unchanged, StateType.Deleted);
      CheckMatch (StateType.Deleted, false, StateType.Unchanged, StateType.Invalid);
    }

    [Test]
    public void Matches_Invalid ()
    {
      CheckMatch (StateType.Invalid, true, StateType.Unchanged, StateType.Invalid);
      CheckMatch (StateType.Invalid, false, StateType.Unchanged, StateType.Deleted);
    }

    [Test]
    public void Matches_NotLoadedYet ()
    {
      CheckMatch (StateType.NotLoadedYet, true, StateType.Unchanged, StateType.NotLoadedYet);
      CheckMatch (StateType.NotLoadedYet, false, StateType.Unchanged, StateType.Deleted);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException), ExpectedMessage =
        "Invalid StateType value.\r\nParameter name: stateValue\r\nActual value was -1.")]
    public void Matches_InvalidEnumValue ()
    {
      new StateValueSet ().Matches ((StateType) (-1));
    }

    private void CheckMatch (StateType stateToCheck, bool expectedMatch, params StateType[] stateSet)
    {
      var set = new StateValueSet (stateSet);
      Assert.That (set.Matches (stateToCheck), Is.EqualTo (expectedMatch));
    }
  }
}