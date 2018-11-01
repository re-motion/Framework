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
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// Creates a new <see cref="IFluentControlSelector{TControlSelector,TControlObject}"/> with the specified types from the specified <paramref name="controlHost"/>.
  /// </summary>
  public delegate IFluentControlSelector<TControlSelector, TControl> SelectorFactory<out TControlSelector, TControl> (IControlHost controlHost)
      where TControlSelector : IControlSelector
      where TControl : ControlObject;

  /// <summary>
  /// Setup action for a test using the generic infrastructure.
  /// </summary>
  public delegate void TestSetupAction (WebTestHelper webTestHelper, string url);

  /// <summary>
  /// Setup action for a test using the generic test page.
  /// </summary>
  public delegate void GenericTestSetupAction (WebTestHelper webTestHelper, string controlName);

  /// <summary>
  /// Setup action for a selector test using the generic test page.
  /// </summary>
  public delegate void GenericSelectorTestAction<in TControlSelector, TControl> (
      WebTestHelper webTestHelper,
      SelectorFactory<TControlSelector, TControl> factory,
      string controlName)
      where TControlSelector : IControlSelector
      where TControl : ControlObject;
}