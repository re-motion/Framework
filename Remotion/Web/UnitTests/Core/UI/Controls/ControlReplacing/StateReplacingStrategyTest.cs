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
using Moq;
using NUnit.Framework;
using Remotion.Web.UI.Controls.ControlReplacing;

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

      _replacer = new ControlReplacer(MemberCallerMock.Object);

      Pair state = new Pair(new Hashtable(), new object());
#pragma warning disable CFW0001
      LosFormatter formatter = new LosFormatter();
#pragma warning restore CFW0001
      StringWriter writer = new StringWriter();
      formatter.Serialize(writer, state);

      _stateModificationStrategy = new StateReplacingStrategy(writer.ToString());
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(((StateReplacingStrategy)_stateModificationStrategy).ViewState, Is.Not.Null);
      Assert.That(((StateReplacingStrategy)_stateModificationStrategy).ControlState, Is.Not.Null);
    }

    [Test]
    public void LoadControlState ()
    {
      var testPageHolder = new TestPageHolder(false, RequestMode.PostBack);
      _replacer.StateModificationStrategy = _stateModificationStrategy;
      _replacer.Controls.Add(testPageHolder.NamingContainer);

      MemberCallerMock.Setup(mock => mock.SetChildControlState(_replacer, It.IsNotNull<Hashtable>())).Verifiable();

      _stateModificationStrategy.LoadControlState(_replacer, MemberCallerMock.Object);

      MemberCallerMock.Verify();
    }

    [Test]
    public void LoadViewState ()
    {
      var testPageHolder = new TestPageHolder(false, RequestMode.PostBack);
      _replacer.StateModificationStrategy = _stateModificationStrategy;
      _replacer.Controls.Add(testPageHolder.NamingContainer);

      MemberCallerMock.Setup(mock => mock.LoadViewStateRecursive(_replacer, It.IsNotNull<object>())).Verifiable();

      _stateModificationStrategy.LoadViewState(_replacer, MemberCallerMock.Object);

      MemberCallerMock.Verify();
    }
  }
}
