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
using System.Xml.Schema;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public class SchemaValidationObject
  {
    private readonly FileInfo _file;
    private readonly int _firstLineNumber;
    private readonly List<int> _indents;
    private bool _hasFailed;

    public SchemaValidationObject (FileInfo file, int firstLineNumber, List<int> indents)
    {
      ArgumentUtility.CheckNotNull ("file", file);
      ArgumentUtility.CheckNotNull ("indents", indents);

      _file = file;
      _firstLineNumber = firstLineNumber;
      _indents = indents;
    }

    public bool HasFailed
    {
      get { return _hasFailed; }
    }

    public ValidationEventHandler CreateValidationHandler ()
    {
      return delegate (object sender, ValidationEventArgs e)
      {
        XmlSchemaException schemaError = e.Exception;

        int lineNumber = schemaError.LineNumber + _firstLineNumber - 1;
        int linePosition = schemaError.LinePosition + _indents[schemaError.LineNumber - 1];
        string errorMessage = string.Format (
            "{0}({1},{2}): {3} WG{4:0000}: {5}",
            _file.FullName,
            lineNumber,
            linePosition,
            e.Severity.ToString ().ToLower (),
            (int) InputError.InvalidSchema,
            schemaError.Message);
        Console.Error.WriteLine (errorMessage);

        if (e.Severity == XmlSeverityType.Error)
          _hasFailed = true;
      };
    }
  }
}
