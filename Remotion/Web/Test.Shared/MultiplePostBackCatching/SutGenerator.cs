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
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.PostBackTargets;

namespace Remotion.Web.Test.Shared.MultiplePostBackCatching
{
  public class SutGenerator
  {
    public const string LastClickFieldID = "LastClickField";
    public const string ServerDelayParameter = "ServerDelay";

    public static int GetServerDelayUrlParameter (Page page)
    {
      ArgumentUtility.CheckNotNull("page", page);

      string serverDelayString = page.Request.QueryString[SutGenerator.ServerDelayParameter];
      if (serverDelayString != null)
        return int.Parse(serverDelayString);
      return 2000;
    }

    public static void GenerateSut (Page sutPage, ControlCollection controls)
    {
      SutGenerator sutGenerator = new SutGenerator(sutPage);
      foreach (Control control in sutGenerator.CreateSut())
        controls.Add(control);
    }

    private readonly Page _sutPage;
    private TestControlGenerator _testControlGenerator;
    private readonly int _serverDelay;
    private Label _label;
    private PostBackEventHandler _postBackEventHandler;

    public SutGenerator (Page sutPage, int serverDelay)
    {
      ArgumentUtility.CheckNotNull("sutPage", sutPage);
      _sutPage = sutPage;
      _serverDelay = serverDelay;
    }

    public SutGenerator (Page sutPage)
      : this(sutPage, GetServerDelayUrlParameter(sutPage))
    {
    }

    public Control[] CreateSut ()
    {
      List<Control> controls = new List<Control>();

      _label = new Label();
      _label.Text = "Test Result: ###";
      controls.Add(_label);

      _postBackEventHandler = new PostBackEventHandler();
      _postBackEventHandler.ID = "PostBackEventHandler";
      _postBackEventHandler.PostBack += HandlePostBack;
      controls.Add(_postBackEventHandler);

      Table sutTable = new Table();
      sutTable.ID = "SutTable";
      sutTable.EnableViewState = false;
      _testControlGenerator = new TestControlGenerator(_sutPage, _postBackEventHandler);
      _testControlGenerator.Click += TestControlGeneratorOnClick;
      foreach (Control initialControl in _testControlGenerator.GetTestControls(null))
          sutTable.Rows.Add(CreateRow(initialControl));
      controls.Add(sutTable);

      return controls.ToArray();
    }

    private void TestControlGeneratorOnClick (object sender, IDEventArgs idEventArgs)
    {
      HandlePostBack(idEventArgs.ID);
    }

    private void HandlePostBack (object sender, PostBackEventHandlerEventArgs e)
    {
      HandlePostBack(e.EventArgument);
    }

    private void HandlePostBack (string result)
    {
      _label.Text = "Test Result: " + result;
      ScriptManager.RegisterHiddenField(_sutPage, LastClickFieldID, result);
      System.Threading.Thread.Sleep(_serverDelay);
    }

    private TableRow CreateRow (Control initialControl)
    {
      TableRow row = new TableRow();
      row.Cells.Add(CreateCell(initialControl));
      foreach (Control followUpControl in _testControlGenerator.GetTestControls(initialControl.ID))
        row.Cells.Add(CreateCell(followUpControl));
      return row;
    }

    private TableCell CreateCell (Control control)
    {
      TableCell cell = new TableCell();
      cell.Controls.Add(control);
      return cell;
    }
 }
}
