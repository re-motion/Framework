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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Remotion.Utilities;

namespace Remotion.Xml
{
  /// <summary>
  /// Use this class to easily serialize and deserialize objects to or from XML.
  /// </summary>
  public static class XmlSerializationUtility
  {
    public static object DeserializeUsingSchema (XmlReader reader, Type type, string defaultNamespace, XmlReaderSettings settings)
    {
      ArgumentUtility.CheckNotNull ("reader", reader);
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("settings", settings);

      XmlSchemaValidationHandler validationHandler = new XmlSchemaValidationHandler (true);
      settings.ValidationEventHandler += validationHandler.Handler;
      settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

      XmlReader innerReader = XmlReader.Create (reader, settings);
      XmlSerializer serializer = new XmlSerializer (type, defaultNamespace);

      try
      {
        return serializer.Deserialize (innerReader);
      }
      catch (InvalidOperationException e)
      {
        // unwrap an inner XmlSchemaValidationException 
        XmlSchemaValidationException schemaException = e.InnerException as XmlSchemaValidationException;
        if (schemaException != null)
          throw schemaException;

        // wrap any other InvalidOperationException in an XmlException with line info
        IXmlLineInfo lineInfo = (IXmlLineInfo) innerReader;
        if (lineInfo != null)
        {
          string errorMessage = string.Format (
              "Error reading {0} ({1},{2}): {3}",
              innerReader.BaseURI,
              lineInfo.LineNumber,
              lineInfo.LinePosition,
              e.Message);
          throw new XmlException (errorMessage, e, lineInfo.LineNumber, lineInfo.LinePosition);
        }
        else
        {
          string errorMessage = string.Format (
              "Error reading {0}: {1}",
              innerReader.BaseURI,
              e.Message); 
          throw new XmlException (errorMessage, e);
        }
      }
    }

    public static object DeserializeUsingSchema (XmlReader reader, Type type, string defaultNamespace, XmlSchemaSet schemas)
    {
      ArgumentUtility.CheckNotNull ("reader", reader);

      XmlReaderSettings settings = new XmlReaderSettings();
      settings.Schemas = schemas;
      settings.ValidationType = ValidationType.Schema;

      return DeserializeUsingSchema (reader, type, defaultNamespace, settings);
    }

    public static object DeserializeUsingSchema (XmlReader reader, Type type, XmlSchemaSet schemas)
    {
      return DeserializeUsingSchema (reader, type, GetNamespace (type), schemas);
    }

    public static object DeserializeUsingSchema (XmlReader reader, Type type, string schemaUri, XmlReader schemaReader)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("schemaUri", schemaUri);
      ArgumentUtility.CheckNotNull ("schemaReader", schemaReader);

      XmlSchemaSet schemas = new XmlSchemaSet();
      schemas.Add (schemaUri, schemaReader);
      return DeserializeUsingSchema (reader, type, GetNamespace (type), schemas);
    }

   
    /// <summary>
    /// Get the Namespace from a type's <see cref="XmlRootAttribute"/> (preferred) or <see cref="XmlTypeAttribute"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown if no namespace is specified through at least one of the possible attributes. </exception>
    public static string GetNamespace (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      XmlTypeAttribute xmlType = (XmlTypeAttribute) Attribute.GetCustomAttribute (type, typeof (XmlTypeAttribute), true);
      XmlRootAttribute xmlRoot = (XmlRootAttribute) Attribute.GetCustomAttribute (type, typeof (XmlRootAttribute), true);
      bool hasXmlType = xmlType != null;
      bool hasXmlRoot = xmlRoot != null;
      if (!hasXmlType && !hasXmlRoot)
      {
        throw new ArgumentException (
            string.Format (
                "Cannot determine the xml namespace of type '{0}' because no neither an XmlTypeAttribute nor an XmlRootAttribute has been provided.",
                type.FullName),
            "type");
      }

      bool hasXmlTypeNamespace = hasXmlType ? (! String.IsNullOrEmpty (xmlType.Namespace)) : false;
      bool hasXmlRootNamespace = hasXmlRoot ? (! String.IsNullOrEmpty (xmlRoot.Namespace)) : false;
      if (! hasXmlTypeNamespace && ! hasXmlRootNamespace)
      {
        throw new ArgumentException (
            string.Format (
                "Cannot determine the xml namespace of type '{0}' because neither an XmlTypeAttribute nor an XmlRootAttribute is used to define a namespace for the type.",
                type.FullName),
            "type");
      }

      if (hasXmlRootNamespace)
        return xmlRoot.Namespace;
      else
        return xmlType.Namespace;
    }
  }
}
