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
using System.IO;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Web.UI.Controls.ControlReplacing;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.ControlReplacing
{
  [TestFixture]
  public class StateReplacingStrategyTest : TestBase
  {
    private ControlReplacer _replacer;
    private IStateModificationStrategy _stateModificationStrategy;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _replacer = new ControlReplacer (MemberCallerMock);

      Pair state = new Pair (new Hashtable(), new object());
      LosFormatter formatter = new LosFormatter ();
      StringWriter writer = new StringWriter ();
      formatter.Serialize (writer, state);

      _stateModificationStrategy = new StateReplacingStrategy (writer.ToString ());
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (((StateReplacingStrategy)_stateModificationStrategy).ViewState, Is.Not.Null);
      Assert.That (((StateReplacingStrategy) _stateModificationStrategy).ControlState, Is.Not.Null);
    }

    [Test]
    public void LoadControlState ()
    {
      var testPageHolder = new TestPageHolder (false, RequestMode.PostBack);
      _replacer.StateModificationStrategy = _stateModificationStrategy;
      _replacer.Controls.Add (testPageHolder.NamingContainer);

      MemberCallerMock.Expect (mock => mock.SetChildControlState (Arg<ControlReplacer>.Is.Same (_replacer), Arg<Hashtable>.Is.NotNull));
      MockRepository.ReplayAll ();

      _stateModificationStrategy.LoadControlState (_replacer, MemberCallerMock);

      MockRepository.VerifyAll ();
    }

    [Test]
    public void LoadViewState ()
    {
      var testPageHolder = new TestPageHolder (false, RequestMode.PostBack);
      _replacer.StateModificationStrategy = _stateModificationStrategy;
      _replacer.Controls.Add (testPageHolder.NamingContainer);

      MemberCallerMock.Expect (mock => mock.LoadViewStateRecursive (Arg<ControlReplacer>.Is.Same (_replacer), Arg<Hashtable>.Is.NotNull));

      MockRepository.ReplayAll ();

      _stateModificationStrategy.LoadViewState (_replacer, MemberCallerMock);

      MemberCallerMock.VerifyAllExpectations ();
    }
  }
}
