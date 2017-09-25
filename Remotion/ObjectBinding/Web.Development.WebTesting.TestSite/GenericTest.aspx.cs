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
using System.Web.UI;
using Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.GenericPages;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.TestSite.Infrastructure;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  public partial class GenericTest : GenericTestPageBase<GenericTestOptions>
  {
    private readonly GenericTestPageParameterCollection _parameters = new GenericTestPageParameterCollection();

    private GenericTestOptions _ambiguousControlOptions;
    private GenericTestOptions _hiddenControlOptions;
    private GenericTestOptions _visibleControlOptions;

    public GenericTest ()
    {
      Register ("autoCompleteReferenceValue", new BocAutoCompleteReferenceValueGenericTestPage());
      Register ("booleanValue", new BocBooleanValueGenericTestPage());
      Register ("checkBox", new BocCheckBoxValueGenericTestPage());
      Register ("dateTimeValue", new BocDateTimeValueGenericTestPage());
      Register ("dropDownList", new BocEnumValueGenericTestPage(ListControlType.DropDownList));
      Register ("list", new BocListGenericTestPage());
      Register ("listAsGrid", new BocListAsGridGenericTestPage());
      Register ("listBox", new BocEnumValueGenericTestPage(ListControlType.ListBox));
      Register ("multilineText", new BocMultilineTextValueGenericTestPage());
      Register ("radioButtonList", new BocEnumValueGenericTestPage(ListControlType.RadioButtonList));
      Register ("referenceValue", new BocReferenceValueGenericTestPage());
      Register ("textValue", new BocTextValueGenericTestPage());
      Register ("treeView", new BocTreeViewGenericTestPage());
    }

    /// <inheritdoc />
    protected override GenericTestOptions AmbiguousControlOptions
    {
      get { return _ambiguousControlOptions; }
    }

    /// <inheritdoc />
    protected override Control AmbiguousControlPanel
    {
      get { return PanelAmbiguousControl; }
    }

    /// <inheritdoc />
    protected override GenericTestOptions HiddenControlOptions
    {
      get { return _hiddenControlOptions; }
    }

    /// <inheritdoc />
    protected override Control HiddenControlPanel
    {
      get { return PanelHiddenControl; }
    }

    /// <inheritdoc />
    protected override GenericTestPageParameterCollection Parameters
    {
      get { return _parameters; }
    }

    /// <inheritdoc />
    protected override GenericTestOptions VisibleControlOptions
    {
      get { return _visibleControlOptions; }
    }

    /// <inheritdoc />
    protected override Control VisibleControlPanel
    {
      get { return PanelVisibleControl; }
    }

    /// <inheritdoc />
    protected override void OnInit (EventArgs e)
    {
      // Constants for all the controls on this generic page
      const string ambiguousID = "AmbiguousControl";
      const string hiddenID = "HiddenControl";
      const string visibleID = "VisibleControl";
      const string visibleIndex = "1", hiddenIndex = "133";
      const string correctDomainProperty = "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample";
      const string incorrectDomainProperty = "Remotion.ObjectBinding.Sample.Job, Remotion.ObjectBinding.Sample";

      // "Real" HTML ids of the controls
      var ambiguousHtmlID = string.Concat ("body_", hiddenID);
      var hiddenHtmlID = string.Concat ("body_", hiddenID);
      var visibleHtmlID = string.Concat ("body_", visibleID);

      // Options for creating the controls
      _ambiguousControlOptions = new GenericTestOptions (ambiguousID, ambiguousHtmlID, DataSource.ID, correctDomainProperty, incorrectDomainProperty);
      _hiddenControlOptions = new GenericTestOptions (hiddenID, hiddenHtmlID, DataSource.ID, correctDomainProperty, incorrectDomainProperty);
      _visibleControlOptions = new GenericTestOptions (visibleID, visibleHtmlID, DataSource.ID, correctDomainProperty, incorrectDomainProperty);

      // Parameters which will be passed to the client
      _parameters.Add (TestConstants.HtmlIDSelectorID, visibleHtmlID, hiddenHtmlID);
      _parameters.Add (TestConstants.IndexSelectorID, visibleIndex, hiddenIndex, visibleHtmlID);
      _parameters.Add (TestConstants.LocalIDSelectorID, visibleID, hiddenID, visibleHtmlID);
      _parameters.Add (TestConstants.FirstSelectorID, visibleHtmlID);
      _parameters.Add (TestConstants.SingleSelectorID, visibleHtmlID);

      base.OnInit (e);
    }

    /// <inheritdoc />
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      DataSource.BusinessObject = (IBusinessObject) ((GenericTestFunction) CurrentFunction).Person;
      DataSource.LoadValues (IsReturningPostBack);
    }

    /// <inheritdoc />
    protected override void SetTestInformation (string information)
    {
      var master = Master as Layout;
      if (master == null)
        throw new InvalidOperationException ("The master page of the generic test page is not set.");
      master.SetTestInformation (information);
    }
  }
}