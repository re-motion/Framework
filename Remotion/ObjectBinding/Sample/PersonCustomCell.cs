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
using System.Web.UI.HtmlControls;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Sample
{
  public class PersonCustomCell: BocCustomColumnDefinitionCell
  {
    protected override Control CreateControl(BocCustomCellArguments arguments)
    {
      HtmlInputFile inputFile = new HtmlInputFile();
      return inputFile;
    }

    protected override void OnLoad(BocCustomCellLoadArguments arguments)
    {
      if (arguments.List.Page.IsPostBack)
      {
        HtmlInputFile inputFile = (HtmlInputFile) arguments.Control;
        if (inputFile != null && inputFile.PostedFile != null)
        {
        }
      }
    }

    protected override void OnPreRender (BocCustomCellPreRenderArguments arguments)
    {
      base.OnPreRender (arguments);
      foreach (var row in arguments.GetRowsToRender())
        RegisterForSynchronousPostBack (row, "sync");
    }

    protected override void OnClick(BocCustomCellClickArguments arguments, string eventArgument)
    {
    }

    protected override void OnValidate(BocCustomCellValidationArguments arguments)
    {
    }

    protected override void Render (HtmlTextWriter writer, BocCustomCellRenderArguments arguments)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      string onClickEvent = GetPostBackClientEvent ("async");
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClickEvent);
      writer.RenderBeginTag (HtmlTextWriterTag.A);
      writer.Write ("async");
      writer.RenderEndTag();
      writer.Write (" ");

      writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      onClickEvent = GetPostBackClientEvent ("sync");
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClickEvent);
      writer.RenderBeginTag (HtmlTextWriterTag.A);
      writer.Write ("sync");
      writer.RenderEndTag();
      writer.Write ("<br />");
    }
  }
}
