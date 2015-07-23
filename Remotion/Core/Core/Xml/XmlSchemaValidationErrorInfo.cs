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
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Remotion.Xml
{
  public class XmlSchemaValidationErrorInfo: IXmlLineInfo
  {
    private string _errorMessage;
    private string _context;
    private int _lineNumber;
    private int _linePosition;
    private bool _hasLineInfo;
    private XmlSeverityType _severity;

    public XmlSchemaValidationErrorInfo (string errorMessage, string context, IXmlLineInfo lineInfo, XmlSeverityType severity)
    {
      _errorMessage = errorMessage;
      _context = context;
      if (lineInfo != null)
      {
        _hasLineInfo = lineInfo.HasLineInfo();
        _lineNumber = lineInfo.LineNumber;
        _linePosition = lineInfo.LinePosition;
      }
      else
      {
        _hasLineInfo = false;
        _lineNumber = 0;
        _linePosition = 0;
      }
      _severity = severity;
    }

    public string ErrorMessage
    {
      get { return _errorMessage; }
    }

    public int LineNumber
    {
      get { return _lineNumber; }
    }

    public int LinePosition
    {
      get { return _linePosition; }
    }

    public bool HasLineInfo()
    {
      return _hasLineInfo;
    }

    public XmlSeverityType Severity
    {
      get { return _severity; }
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder (200);
      sb.Append ("Schema validation ");
      sb.Append (_severity.ToString().ToLower());

      if (_context != null)
      {
        sb.Append (" in ");
        sb.Append (_context);
      }

      if (_hasLineInfo)
      {
        sb.Append (" (");
        sb.Append (_lineNumber);
        sb.Append (",");
        sb.Append (_linePosition);
        sb.Append (")");
      }

      sb.Append (": ");
      sb.Append (_errorMessage);

      return sb.ToString();
    }
  }
}
