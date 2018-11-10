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
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestPageParameters;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories
{
  /// <summary>
  /// Contains tests for accessing the label of a <see cref="BocControlObject"/> in a form grid.
  /// </summary>
  public class LabelTestCaseFactory<TControlSelector, TControl>
      : ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, LabelTestPageParameters>
      where TControlSelector : IHtmlIDControlSelector<TControl>
      where TControl : BocControlObject
  {
    protected override string TestPrefix
    {
      get { return "Label"; }
    }

    [GenericPageTestMethod (PageType = GenericTestPageType.EnabledFormGrid)]
    public void GetLabelForEditableControl ()
    {
      var control = Selector.GetByID (Parameter.FormGridControlHtmlId);

      Assert.That (control.GetLabels().Count, Is.EqualTo (1));
      Assert.That (control.GetLabels().First().GetText(), Is.EqualTo (Parameter.FormGridLabel));
    }

    [GenericPageTestMethod (PageType = GenericTestPageType.EnabledFormGrid)]
    public void GetLabelForReadOnlyControl ()
    {
      var control = Selector.GetByID (Parameter.ReadonlyFormGridControlHtmlId);

      Assert.That (control.GetLabels().Count, Is.EqualTo (1));
      Assert.That (control.GetLabels().First().GetText(), Is.EqualTo (Parameter.FormGridLabel));
    }

    [GenericPageTestMethod (PageType = GenericTestPageType.EnabledFormGrid)]
    public void GetLabelForControlOverMultipleFormGridRows ()
    {
      var control = Selector.GetByID (Parameter.OneControlOverMultipleRowsFormGridControlHtmlId);

      Assert.That (control.GetLabels ().Count, Is.EqualTo (1));
      Assert.That (control.GetLabels ().First ().GetText (), Is.EqualTo (Parameter.FormGridLabel));
    }

    [GenericPageTestMethod (PageType = GenericTestPageType.EnabledFormGrid)]
    public void GetLabelForControlWithShiftedFormGridColumns ()
    {
      var control = Selector.GetByID (Parameter.ColumnsShiftedFormGridControlHtmlId);

      Assert.That (control.GetLabels ().Count, Is.EqualTo (1));
      Assert.That (control.GetLabels ().First ().GetText (), Is.EqualTo (Parameter.FormGridLabel));
    }

    [GenericPageTestMethod (PageType = GenericTestPageType.EnabledFormGrid)]
    public void GetLabelForControlWithMultipleControlsInOneFormGrid ()
    {
      var control1 = Selector.GetByID (Parameter.FormGridMultiControl1HtmlId);
      var control2 = Selector.GetByID (Parameter.FormGridMultiControl2HtmlId);

      Assert.That (control1.GetLabels ().Count, Is.EqualTo (1));
      Assert.That (control1.GetLabels ().First ().GetText (), Is.EqualTo (Parameter.FormGridLabel));
      Assert.That (control2.GetLabels ().Count, Is.EqualTo (1));
      Assert.That (control2.GetLabels ().First ().GetText (), Is.EqualTo (Parameter.FormGridLabel));
    }

    [GenericPageTestMethod (PageType = GenericTestPageType.EnabledFormGrid)]
    public void GetLabelForControlNotInFormGrid ()
    {
      if (typeof (TControl) == typeof (BocDateTimeValueControlObject))
        Assert.Ignore ("BocDateTime Value always has a label.");

      var control = Selector.GetByID (Parameter.ControlNotInFormGridHtmlId);
      Assert.That (() => control.GetLabels ().Count, Is.EqualTo (0));
    }
  }
}