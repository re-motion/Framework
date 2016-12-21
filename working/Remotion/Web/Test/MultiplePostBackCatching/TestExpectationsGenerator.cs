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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.PostBackTargets;
using Remotion.Web.Utilities;

namespace Remotion.Web.Test.MultiplePostBackCatching
{
  public class TestExpectationsGenerator
  {
    public const string AllTests = "All";
    public const string TestCaseParameter = "TestCase";

    public static string GetTestCaseUrlParameter (Page testPage)
    {
      return testPage.Request.QueryString[TestExpectationsGenerator.TestCaseParameter];
    }

    public static void GenerateExpectations (Page testPage, TableRowCollection rows, string sutPage)
    {
      TestExpectationsGenerator testExpectationsGenerator = new TestExpectationsGenerator (testPage, sutPage);
      rows.AddRange (testExpectationsGenerator.CreateExpectations (TestExpectationsGenerator.GetTestCaseUrlParameter (testPage)));
    }

    private readonly Page _testPage;
    private TestControlGenerator _testControlGenerator;
    private readonly string _sutPage;

    public TestExpectationsGenerator (Page testPage, string sutPage)
    {
      ArgumentUtility.CheckNotNull ("testPage", testPage);
      ArgumentUtility.CheckNotNullOrEmpty ("sutPage", sutPage);

      _testPage = testPage;
      _sutPage = sutPage;
    }

    public TableRow[] CreateExpectations (string testCase)
    {
      testCase = testCase ?? AllTests;
      _testControlGenerator = new TestControlGenerator (_testPage, new PostBackEventHandler());

      List<TableRow> rows = new List<TableRow>();
      rows.Add (Expect ("open", UrlUtility.AddParameter (_testPage.ResolveClientUrl (_sutPage), SutGenerator.ServerDelayParameter, "500"), null));

      foreach (Control control in _testControlGenerator.GetTestControls (null))
        rows.AddRange (ExpectControlAttributes (control));

      foreach (Control initialControl in _testControlGenerator.GetTestControls (null))
      {
        if (testCase == AllTests || testCase == initialControl.ID)
        {
          foreach (Control followUpControl in _testControlGenerator.GetTestControls (initialControl.ID))
            rows.AddRange (ExpectPostbackForControl (initialControl, followUpControl));
        }
      }

      return rows.ToArray();
    }

    private TableRow[] ExpectPostbackForControl (Control initialControl, Control followUpControl)
    {
      List<TableRow> rows = new List<TableRow>();

      if (_testControlGenerator.IsEnabled (initialControl) && _testControlGenerator.IsEnabled (followUpControl))
      {
        rows.Add (ExpectControlClick (initialControl));
        if (_testControlGenerator.IsAlertHyperLink (initialControl))
          rows.Add (Expect ("waitForAlert", "*", null));
        rows.Add (ExpectControlClick (followUpControl));
        if (_testControlGenerator.IsAlertHyperLink (followUpControl))
          rows.Add (Expect ("waitForAlert", "*", null));
        if (_testControlGenerator.IsAlertHyperLink (initialControl) || _testControlGenerator.IsAlertHyperLink (followUpControl))
          rows.Add (Expect ("assertElementNotPresent", "SmartPageStatusIsSubmittingMessage", null));
        if (!_testControlGenerator.IsAlertHyperLink (initialControl) && !_testControlGenerator.IsAlertHyperLink (followUpControl))
          rows.Add (Expect ("waitForVisible", "SmartPageStatusIsSubmittingMessage", null));
        if (!_testControlGenerator.IsAlertHyperLink (initialControl) || !_testControlGenerator.IsAlertHyperLink (followUpControl))
          rows.Add (Expect ("waitForPageToLoad", "1000", null));
        if (!_testControlGenerator.IsAlertHyperLink (initialControl)  && _testControlGenerator.IsAlertHyperLink (followUpControl))
          rows.Add (Expect ("assertValue", SutGenerator.LastClickFieldID, initialControl.ID));
        if (_testControlGenerator.IsAlertHyperLink (initialControl) && !_testControlGenerator.IsAlertHyperLink (followUpControl))
          rows.Add (Expect ("assertValue", SutGenerator.LastClickFieldID, followUpControl.ID));
      }
      return rows.ToArray();
    }

    private TableRow[] ExpectControlAttributes (Control control)
    {
      if (_testControlGenerator.IsEnabled (control))
      {
        if (control.GetType() == typeof (Button))
          return ExpectButtonAttributes ((Button) control);
        if (control.GetType() == typeof (WebButton))
          return ExpectWebButtonAttributes ((WebButton) control);
        if (control.GetType() == typeof (LinkButton))
          return ExpectLinkButtonAttributes ((LinkButton) control);
        if (control.GetType() == typeof (HyperLink))
          return ExpectHyperLinkAttributes ((HyperLink) control);
      }
      return new TableRow[0];
    }

    private TableRow[] ExpectButtonAttributes (Button button)
    {
      List<TableRow> rows = new List<TableRow>();

      rows.Add (ExpectElementTag (button, "INPUT"));
      rows.Add (ExpectAttribute (button, "type", button.UseSubmitBehavior ? "submit" : "button"));

      return rows.ToArray();
    }

    private TableRow[] ExpectWebButtonAttributes (WebButton button)
    {
      List<TableRow> rows = new List<TableRow>();

      rows.Add (ExpectElementTag (button, "BUTTON"));
      rows.Add (ExpectAttribute (button, "type", button.UseSubmitBehavior ? "submit" : "button"));

      return rows.ToArray();
    }

    private TableRow[] ExpectLinkButtonAttributes (LinkButton linkButton)
    {
      List<TableRow> rows = new List<TableRow>();

      rows.Add (ExpectAttribute (linkButton, "href", "javascript:__doPostBack*"));

      return rows.ToArray();
    }

    private TableRow[] ExpectHyperLinkAttributes (HyperLink hyperLink)
    {
      List<TableRow> rows = new List<TableRow>();

      if (!_testControlGenerator.IsAlertHyperLink (hyperLink))
      {
        rows.Add (ExpectAttribute (hyperLink, "href", "*#"));
        rows.Add (ExpectAttribute (hyperLink, "onclick", "*__doPostBack*"));
      }

      return rows.ToArray();
    }

    private TableRow ExpectControlClick (Control control)
    {
      Control targetControl = (control.Controls.Count == 0) ? control : control.Controls[0];
      return Expect ("click", targetControl.ID, null);
    }

    private TableRow ExpectElementTag (Control control, string tagName)
    {
      return Expect ("assertElementPresent", string.Format ("xpath=//{1}[contains(@id,'{0}')]", control.ID, tagName), null);
    }

    private TableRow ExpectAttribute (Control control, string name, string value)
    {
      return Expect ("assertAttribute", control.ID + "@" + name, value);
    }

    private TableRow Expect (string command, string target, string value)
    {
      TableRow row = new TableRow();

      TableCell commandCell = new TableCell();
      commandCell.Text = command;
      row.Cells.Add (commandCell);

      TableCell targetCell = new TableCell();
      targetCell.Text = target;
      row.Cells.Add (targetCell);

      TableCell valueCell = new TableCell();
      valueCell.Text = value;
      row.Cells.Add (valueCell);

      return row;
    }
  }
}
