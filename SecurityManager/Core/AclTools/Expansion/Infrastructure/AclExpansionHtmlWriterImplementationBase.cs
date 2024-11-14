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
using System.IO;
using System.Linq;
using System.Text;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  /// <summary>
  /// Base implementation class for <see cref="AclExpansionHtmlWriter"/> and <see cref="AclExpansionMultiFileHtmlWriter"/>.
  /// </summary>
  public class AclExpansionHtmlWriterImplementationBase
  {
    public static string ToValidFileName (string name)
    {
      var sb = new StringBuilder();
      List<char> invalidFileNameCharsSortedList = Path.GetInvalidFileNameChars().ToList();
      invalidFileNameCharsSortedList.Sort();
      foreach (char c in name)
      {
        if (invalidFileNameCharsSortedList.BinarySearch(c) >= 0)
        {
          sb.Append('_');
        }
        else
        {
          sb.Append(c);
        }
      }
      return sb.ToString();
    }

    private readonly HtmlTagWriter.HtmlTagWriter _htmlTagWriter;
    private bool _isInTableRow;


    public AclExpansionHtmlWriterImplementationBase (TextWriter textWriter, bool indentXml)
    {
      _htmlTagWriter = new HtmlTagWriter.HtmlTagWriter(textWriter, indentXml);
    }


    public void WriteTableEnd ()
    {
      _htmlTagWriter.Tags.tableEnd();
    }

    public virtual void WriteTableStart (string tableId)
    {
      _htmlTagWriter.Tags.table().Attribute("style", "width: 100%;").Attribute("class", "aclExpansionTable").Attribute("id", tableId);
    }


    public virtual HtmlTagWriter.HtmlTagWriter WritePageStart (string pageTitle)
    {
      _htmlTagWriter.WritePageHeader(pageTitle, "AclExpansion.css");
      _htmlTagWriter.Tag("body");
      return _htmlTagWriter;
    }


    public virtual void WritePageEnd ()
    {
      _htmlTagWriter.TagEnd("body");
      _htmlTagWriter.TagEnd("html");

      _htmlTagWriter.Close();
    }

    public virtual void WriteHeaderCell (string columnName)
    {
      _htmlTagWriter.Tags.th().Attribute("class", "header");
      _htmlTagWriter.Value(columnName);
      _htmlTagWriter.Tags.thEnd();
    }

    public virtual void WriteTableData (string? value)
    {
      WriteTableRowBeginIfNotInTableRow();
      _htmlTagWriter.Tags.td();
      _htmlTagWriter.Value(value);
      _htmlTagWriter.Tags.tdEnd();
    }



    public virtual void WriteTableRowBeginIfNotInTableRow ()
    {
      if (!_isInTableRow)
      {
        _htmlTagWriter.Tags.tr();
        _isInTableRow = true;
      }
    }

    public virtual void WriteTableRowEnd ()
    {
      _htmlTagWriter.Tags.trEnd();
      _isInTableRow = false;
    }

    public HtmlTagWriter.HtmlTagWriter HtmlTagWriter
    {
      get { return _htmlTagWriter; }
    }
  }
}
