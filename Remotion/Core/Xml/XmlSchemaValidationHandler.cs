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
using System.Xml;
using System.Xml.Schema;
using Microsoft.Extensions.Logging;
using Remotion.Logging;

namespace Remotion.Xml
{
  public class XmlSchemaValidationHandler
  {
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<XmlSchemaValidationHandler>();

    private bool _failOnError;

    private List<XmlSchemaValidationErrorInfo> _messages = new List<XmlSchemaValidationErrorInfo>();

    private int _warnings = 0;
    private int _errors = 0;

    private XmlSchemaValidationErrorInfo? _firstError = null;
    private XmlSchemaException? _firstException = null;

    public XmlSchemaValidationHandler (bool failOnError)
    {
      _failOnError = failOnError;
    }

    public int Warnings
    {
      get { return _warnings; }
    }

    public int Errors
    {
      get { return _errors; }
    }

    public ValidationEventHandler Handler
    {
      get { return new ValidationEventHandler(HandleValidation); }
    }

    private void HandleValidation (object? sender, ValidationEventArgs args)
    {
      // TODO RM-7772: sender should be checked for null
      XmlReader reader = (XmlReader)sender!;

      // WORKAROUND: known bug in .NET framework 1.x
      // TODO: verify for 2.0
      if (args.Message.IndexOf("http://www.w3.org/XML/1998/namespace:base") >= 0)
      {
        s_logger.LogDebug(
            "Ignoring the following schema validation error in {0}, because it is considered a known .NET framework bug: {1}",
            reader.BaseURI,
            args.Message);
        return;
      }

      IXmlLineInfo? lineInfo = sender as IXmlLineInfo;
      XmlSchemaValidationErrorInfo errorInfo = new XmlSchemaValidationErrorInfo(args.Message, reader.BaseURI, lineInfo, args.Severity);
      _messages.Add(errorInfo);

      if (args.Severity == XmlSeverityType.Error)
      {
        s_logger.LogError(errorInfo.ToString());
        ++ _errors;
        if (_failOnError)
          throw args.Exception;

        if (_errors == 0)
        {
          _firstError = errorInfo;
          _firstException = args.Exception;
        }
      }
      else
      {
        s_logger.LogWarning(errorInfo.ToString());
        ++ _warnings;
      }
    }

    public XmlSchemaException? FirstException
    {
      get { return _firstException; }
    }

    public void EnsureNoErrors ()
    {
      if (_errors > 0)
      {
        throw _firstException!;

        //string lineInfoMessage = string.Empty;
        //if (_firstError.HasLineInfo())
        //  lineInfoMessage = string.Format (" Line {0}, position {1}.", _firstError.LineNumber, _firstError.LinePosition);

        //throw new RemotionXmlSchemaValidationException (
        //    string.Format (
        //        "Schema verification failed with {0} errors and {1} warnings in '{2}'. First error: {3}{4}",
        //        _errors,
        //        _warnings,
        //        _context,
        //        _firstError.ErrorMessage,
        //        lineInfoMessage),
        //    _context,
        //    _firstError.LineNumber,
        //    _firstError.LinePosition,
        //    null);
      }
    }
  }
}
