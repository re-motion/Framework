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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.TestSite.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite.Shared.GenericPages
{
  /// <summary>
  /// Custom <see cref="IGenericTestPage{TOptions}"/> for a <see cref="HtmlTable"/>.
  /// </summary>
  public class FormGridGenericTestPage : IGenericTestPage<GenericTestOptions>
  {
    public FormGridGenericTestPage ()
    {
    }

    /// <inheritdoc />
    public void AddParameters (Dictionary<string, GenericTestPageParameter> parameterCollection, GenericTestOptions options)
    {
    }

    /// <inheritdoc />
    public Control CreateControl (GenericTestOptions options)
    {
      var table = new HtmlTable { ID = options.ID };
      var row = new HtmlTableRow();
      row.Cells.Add(new HtmlTableCell { ColSpan = 2, InnerText = options.Title });
      table.Rows.Add(row);

      var row2 = new HtmlTableRow();
      row2.Cells.Add(new HtmlTableCell());
      row2.Cells.Add(new HtmlTableCell { InnerText = "test content" });
      table.Rows.Add(row2);

      var container = new Panel();
      container.Controls.Add(table);
      container.Controls.Add(new FormGridManager { ID = options.ID, FormGridSuffix = options.ID });

      return container;
    }
  }
}
