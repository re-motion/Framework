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
using System.Web.UI;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI.Controls.ControlReplacing;

namespace Remotion.Web.UnitTests.Core.UI.Controls.ControlReplacing
{
  [TestFixture]
  public class StateClearingStrategyTest : TestBase
  {
    [Test]
    public void LoadControlState ()
    {
      var testPageHolder = new TestPageHolder(false, RequestMode.PostBack);
      IStateModificationStrategy stateModificationStrategy = new StateClearingStrategy();
      var replacer = new ControlReplacer(MemberCallerMock.Object);
      replacer.StateModificationStrategy = stateModificationStrategy;
      replacer.Controls.Add(testPageHolder.NamingContainer);

      MemberCallerMock.Setup(mock => mock.ClearChildControlState(replacer)).Verifiable();

      stateModificationStrategy.LoadControlState(replacer, MemberCallerMock.Object);

      MemberCallerMock.Verify();
    }

    [Test]
    public void LoadViewState ()
    {
      var testPageHolder = new TestPageHolder(false, RequestMode.PostBack);
      IStateModificationStrategy stateModificationStrategy = new StateClearingStrategy();
      var replacer = new ControlReplacer(MemberCallerMock.Object);
      replacer.StateModificationStrategy = stateModificationStrategy;
      testPageHolder.Page.Controls.Add(replacer);
      ControlInvoker replacerInvoker = new ControlInvoker(replacer);
      replacerInvoker.LoadViewStateRecursive(new Pair(null, new ArrayList { 0, new Pair("ChildState", null) }));

      stateModificationStrategy.LoadViewState(replacer, MemberCallerMock.Object);

      var newControl = new ControlMock();
      replacer.Controls.Add(newControl);
      Assert.That(newControl.ValueInViewState, Is.Null);
    }
  }
}
