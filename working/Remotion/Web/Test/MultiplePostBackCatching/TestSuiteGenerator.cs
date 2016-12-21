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
using Remotion.Web.UI.Controls.PostBackTargets;
using Remotion.Web.Utilities;

namespace Remotion.Web.Test.MultiplePostBackCatching
{
  public class TestSuiteGenerator
  {
    public static void GenerateTestCases (Page testSuitePage, TableRowCollection rows, string testPage, string testPrefix)
    {
      TestSuiteGenerator testSuiteGenerator = new TestSuiteGenerator (testSuitePage, testPage);
      rows.AddRange (testSuiteGenerator.CreateTestCases (testPrefix));
    }
    
    private readonly Page _testSuitePage;
    private readonly string _testPage;

    public TestSuiteGenerator (Page testSuitePage, string testPage)
    {
      ArgumentUtility.CheckNotNull ("page", testSuitePage);
      ArgumentUtility.CheckNotNullOrEmpty ("testPage", testPage);

      _testSuitePage = testSuitePage;
      _testPage = testPage;
    }

    public TableRow[] CreateTestCases (string prefix)
    {
      TestControlGenerator testControlGenerator = new TestControlGenerator (_testSuitePage, new PostBackEventHandler());
      List<TableRow> rows = new List<TableRow>();

      foreach (Control initialControl in testControlGenerator.GetTestControls (null))
      {
        if (testControlGenerator.IsEnabled (initialControl))
        {
          rows.Add (
              CreateTest (
                  CreateID (prefix, initialControl.ID),
                  UrlUtility.AddParameter (_testSuitePage.ResolveUrl (_testPage), TestExpectationsGenerator.TestCaseParameter, initialControl.ID)));
        }
      }

      return rows.ToArray();
    }

    private TableRow CreateTest (string title, string url)
    {
      TableRow row = new TableRow();
      TableCell cell = new TableCell();

      HyperLink hyperLink = new HyperLink();
      hyperLink.NavigateUrl = url;
      hyperLink.Text = title;

      cell.Controls.Add (hyperLink);
      row.Cells.Add (cell);

      return row;
    }

    private string CreateID (string prefix, string id)
    {
      return (string.IsNullOrEmpty (prefix) ? string.Empty : prefix + "_") + id;
    }
  }
}
