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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.GenericTestPageParameters;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories
{
  /// <summary>
  /// Contains tests for readonly state of <see cref="BocControlObject"/>s.
  /// </summary>
  public class ReadOnlyTestCaseFactory<TControlSelector, TControl>
      : ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, ReadOnlyTestPageParameters>
      where TControlSelector : IHtmlIDControlSelector<TControl>
      where TControl : BocControlObject
  {
    public ReadOnlyTestCaseFactory ()
    {
    }

    /// <inheritdoc />
    protected override string TestPrefix
    {
      get { return "ReadOnly"; }
    }

    [GenericPageTestMethod (PageType = GenericTestPageType.EnabledReadOnly)]
    public void IsEditable ()
    {
      var control = Selector.GetByID(Parameter.EnabledHtmlID);
      Assert.That(control.IsReadOnly(), Is.False);
    }

    [GenericPageTestMethod (PageType = GenericTestPageType.EnabledReadOnly)]
    public void IsReadOnly ()
    {
      var control = Selector.GetByID(Parameter.ReadOnlyHtmlID);
      Assert.That(control.IsReadOnly(), Is.True);
    }
  }
}