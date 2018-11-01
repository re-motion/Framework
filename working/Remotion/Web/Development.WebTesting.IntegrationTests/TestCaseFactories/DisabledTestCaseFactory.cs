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
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.GenericTestPageParameters;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.TestCaseFactories
{
  /// <summary>
  /// Contains tests for disabled state of <see cref="ControlObject"/>s.
  /// </summary>
  public class DisabledTestCaseFactory<TControlSelector, TControl>
      : ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, DisabledTestPageParameters>
      where TControlSelector : IHtmlIDControlSelector<TControl>
      where TControl : ControlObject, ISupportsDisabledState
  {
    public DisabledTestCaseFactory ()
    {
    }

    /// <inheritdoc />
    protected override string TestPrefix
    {
      get { return "Disabled"; }
    }

    [GenericPageTestMethod (PageType = GenericTestPageType.EnabledDisabled)]
    public void IsEnabled ()
    {
      var control = Selector.GetByID (Parameter.EnabledHtmlID);
      Assert.That (control.IsDisabled(), Is.False);
    }

    [GenericPageTestMethod (PageType = GenericTestPageType.EnabledDisabled)]
    public void IsDisabled ()
    {
      var control = Selector.GetByID (Parameter.DisabledHtmlID);
      Assert.That (control.IsDisabled(), Is.True);
    }
  }
}