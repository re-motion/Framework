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
using System.Xml;
using System.Xml.Schema;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Configuration
{
  /// <summary>
  /// Utility class for validating XML fragment strings against a XSD schema.
  /// </summary>
  public static class XmlSchemaValidation
  {
    public static void Validate (string xmlFragment, string xsdContent)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);
      ArgumentUtility.CheckNotNullOrEmpty ("xsdContent", xsdContent);

      var validationErrors = GetValidationErrors (xmlFragment, xsdContent);
      if (validationErrors.Count > 0)
      {
        var errors = string.Join ("\r\n", validationErrors.Select (e => e.Message));
        var message = string.Format ("Validation of the xml fragment did not succeed for schema '{0}'.\r\n{1}", xsdContent, errors);
        throw new XmlSchemaValidationException (message);
      }
    }

    public static bool IsValid (string xmlFragment, string xsdPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);
      ArgumentUtility.CheckNotNullOrEmpty ("xsdPath", xsdPath);

      return GetValidationErrors (xmlFragment, xsdPath).Count == 0;
    }

    private static List<ValidationEventArgs> GetValidationErrors (string xmlFragment, string xsdContent)
    {
      var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
      settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema |
                                  XmlSchemaValidationFlags.ProcessSchemaLocation |
                                  XmlSchemaValidationFlags.ReportValidationWarnings;

      var validationErrors = new List<ValidationEventArgs>();
      settings.ValidationEventHandler += (sender, args) => validationErrors.Add (args);

      XmlSchema xmlSchema;
      using (var reader = new StringReader (xsdContent))
      {
        xmlSchema = XmlSchema.Read (
            reader,
            (sender, args) =>
            {
              var message = string.Format ("Schema is invalid: {0}", args.Message);
              throw new XmlSchemaException (message, args.Exception);
            });
      }
      settings.Schemas.Add (xmlSchema);

      var xmlReader = XmlReader.Create (new StringReader (xmlFragment), settings);
      ReadToEnd (xmlReader);

      return validationErrors;
    }

    private static void ReadToEnd (XmlReader xmlReader)
    {
      while (xmlReader.Read())
      {
      }
    }
  }
}