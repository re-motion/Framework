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
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// A <see cref="TestCaseFactoryBase"/> for testing <see cref="IControlSelector"/>s.
  /// </summary>
  public abstract class ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, TParameter> : GenericTestPageTestCaseFactoryBase<TParameter>
      where TControlSelector : IControlSelector
      where TControl : ControlObject
      where TParameter : IGenericTestPageParameter, new()
  {
    private IFluentControlSelector<TControlSelector, TControl> _selector;

    protected ControlSelectorTestCaseFactoryBase ()
    {
    }

    /// <summary>
    /// The <see cref="IFluentControlSelector{TControlSelector,TControlObject}"/> for the specified control selector type.
    /// </summary>
    [NotNull]
    protected IFluentControlSelector<TControlSelector, TControl> Selector
    {
      get { return _selector; }
    }

    // todo test until RM-6773 is fixed
    protected void SwitchToIFrame ()
    {
      var frame = Helper.MainBrowserSession.Window.FindFrame ("testFrame");
      frame.FindId ("target").Now();
    }

    /// <inheritdoc />
    protected override IEnumerable<TestCaseData> GetTests ()
    {
      return GetTests<GenericPageTestMethodAttribute> (CreateTestCaseData);
    }


    protected void PrepareTest (
        [NotNull] GenericPageTestMethodAttribute attribute,
        [NotNull] WebTestHelper helper,
        [NotNull] SelectorFactory<TControlSelector, TControl> factory,
        [NotNull] string control)
    {
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      ArgumentUtility.CheckNotNull ("helper", helper);
      ArgumentUtility.CheckNotNull ("factory", factory);
      ArgumentUtility.CheckNotNull ("control", control);

      base.PrepareTest (attribute, helper, control);

      _selector = factory (Home);
    }

    private TestCaseData CreateTestCaseData ([NotNull] GenericPageTestMethodAttribute attribute, [NotNull] MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      ArgumentUtility.CheckNotNull ("method", method);

      var testCaseData = new TestCaseData (
          (GenericSelectorTestSetupAction<TControlSelector, TControl>) ((helper, factory, control) =>
          {
            PrepareTest (attribute, helper, factory, control);
            RunTest (method);
          }));

      testCaseData.SetCategory ("ControlSelectorTest");

      return testCaseData;
    }
  }
}