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

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.EditableRowSupport
{
  [TestFixture]
  public class EditModeControllerWithoutEditModeTest : EditModeControllerTestBase
  {
    [Test]
    public void Initialize ()
    {
      Assert.That(Controller.IsRowEditModeActive, Is.False);
      Assert.That(Controller.IsListEditModeActive, Is.False);
    }

    [Test]
    public void InitRecursive ()
    {
      Invoker.InitRecursive();

      Assert.That(Controller.Controls.Count, Is.EqualTo(0));
    }

    [Test]
    public void Validate ()
    {
      Invoker.InitRecursive();
      Invoker.LoadRecursive();

      Assert.That(Controller.Validate(), Is.True);
    }

    [Test]
    public void IsRequired ()
    {
      Invoker.InitRecursive();
      Assert.That(Controller.IsRequired(0), Is.False);
      Assert.That(Controller.IsRequired(1), Is.False);
    }

    [Test]
    public void IsDirty ()
    {
      Invoker.InitRecursive();
      Assert.That(Controller.IsDirty(), Is.False);
    }

    [Test]
    public void GetTrackedIDs ()
    {
      Invoker.InitRecursive();

      Assert.That(Controller.GetTrackedClientIDs(), Is.EqualTo(new string[0]));
    }

    [Test]
    public void SaveAndLoadControlState ()
    {
      Invoker.InitRecursive();

      object viewState = ControllerInvoker.SaveControlState();
      Assert.That(viewState, Is.Not.Null);
      ControllerInvoker.LoadControlState(viewState);
    }

    [Test]
    public void LoadControlStateWithNull ()
    {
      Invoker.InitRecursive();

      ControllerInvoker.LoadControlState(null);

      Assert.That(Controller.IsRowEditModeActive, Is.False);
      Assert.That(Controller.IsListEditModeActive, Is.False);
    }

    [Test]
    public void EnsureEditModeRestored ()
    {
      Assert.That(Controller.IsRowEditModeActive, Is.False);

      Controller.EnsureEditModeRestored(Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.False);
    }

    [Test]
    public void EnsureEditModeRestoredWithValueNull ()
    {
      EditModeHost.Value = null;

      Assert.That(Controller.IsRowEditModeActive, Is.False);

      Controller.EnsureEditModeRestored(Columns);

      Assert.That(Controller.IsRowEditModeActive, Is.False);
    }

    [Test]
    public void GetEditedRow ()
    {
      Assert.Throws<InvalidOperationException>(
          () => Controller.GetEditedRow(),
          "Cannot retrieve edited row: The BocList '{0}' is not in row edit mode.",
          EditModeHost.ID);
    }
  }
}
