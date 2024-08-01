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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.ExtensibleEnums;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation.Transport
{
  public struct XmlTransportItem : IXmlSerializable
  {
    public static XmlTransportItem[] Wrap (TransportItem[] items)
    {
      ArgumentUtility.CheckNotNull("items", items);
      return Array.ConvertAll(items, item => new XmlTransportItem(item));
    }

    public static TransportItem[] Unwrap (XmlTransportItem[] xmlItems)
    {
      ArgumentUtility.CheckNotNull("xmlItems", xmlItems);
      return Array.ConvertAll(xmlItems, item => item.TransportItem);
    }

    private TransportItem _transportItem;

    public XmlTransportItem (TransportItem itemToBeSerialized)
    {
      _transportItem = itemToBeSerialized;
    }

    public TransportItem TransportItem
    {
      get { return _transportItem; }
    }

    public XmlSchema? GetSchema ()
    {
      return null;
    }

    public void WriteXml (XmlWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      writer.WriteAttributeString("ID", _transportItem.ID.ToString());
      SerializeProperties(writer);
    }

    public void ReadXml (XmlReader reader)
    {
      ArgumentUtility.CheckNotNull("reader", reader);

      string? idString = reader.GetAttribute("ID");
      Assertion.IsNotNull(idString, "No value was found for required attribute 'ID' on the current node.");
      ObjectID id = ObjectID.Parse(idString);

      reader.Read();
      List<KeyValuePair<string, object?>> properties = DeserializeProperties(reader, id.ClassDefinition);
      reader.ReadEndElement();

      _transportItem = CreateTransportItem(id, properties);
    }

    private TransportItem CreateTransportItem (ObjectID id, List<KeyValuePair<string, object?>> properties)
    {
      var propertyDictionary = new Dictionary<string, object?>();
      for (int i = 0; i < properties.Count; ++i)
      {
        propertyDictionary.Add(properties[i].Key, properties[i].Value);
      }
      return new TransportItem(id, propertyDictionary);
    }

    private void SerializeProperties (XmlWriter writer)
    {
      writer.WriteStartElement("Properties");
      foreach (KeyValuePair<string, object?> property in _transportItem.Properties)
        SerializeProperty(writer, _transportItem.ID.ClassDefinition, property);
      writer.WriteEndElement();
    }

    private List<KeyValuePair<string, object?>> DeserializeProperties (XmlReader reader, ClassDefinition classDefinition)
    {
      reader.ReadStartElement("Properties");
      var properties = new List<KeyValuePair<string, object?>>();
      while (reader.IsStartElement("Property"))
        properties.Add(DeserializeProperty(reader, classDefinition));
      reader.ReadEndElement();
      return properties;
    }

    private void SerializeProperty (XmlWriter writer, ClassDefinition classDefinition, KeyValuePair<string, object?> property)
    {
      PropertyDefinition? propertyDefinition = classDefinition.GetPropertyDefinition(property.Key);
      writer.WriteStartElement("Property");
      writer.WriteAttributeString("Name", property.Key);
      SerializePropertyValue(writer, propertyDefinition, property.Value);
      writer.WriteEndElement();
    }

    private KeyValuePair<string, object?> DeserializeProperty (XmlReader reader, ClassDefinition classDefinition)
    {
      string? name = reader.GetAttribute("Name");
      Assertion.IsNotNull(name, "No value was found for required attribute 'Name' on the current node.");
      PropertyDefinition? propertyDefinition = classDefinition.GetPropertyDefinition(name);
      object? value = DeserializePropertyValue(reader, propertyDefinition);
      return new KeyValuePair<string, object?>(name, value);
    }

    private void SerializePropertyValue (XmlWriter writer, PropertyDefinition? propertyDefinition, object? value)
    {
      Type? valueType = propertyDefinition == null ? SerializeCustomValueType(writer, value) : propertyDefinition.PropertyType;

      if (value == null)
      {
        writer.WriteElementString("null", "");
      }
      else
      {
        Assertion.DebugIsNotNull(valueType, "valueType != null when value != null");
        if (IsObjectID(valueType))
        {
          writer.WriteString(value.ToString());
        }
        else if (ExtensibleEnumUtility.IsExtensibleEnumType(Assertion.IsNotNull(valueType)))
        {
          writer.WriteString(((IExtensibleEnum)value).ID);
        }
        else if (IsDateOnlyType(Assertion.IsNotNull(valueType)))
        {
          var dateOnly = value as DateOnly?;
          writer.WriteString(dateOnly.HasValue ? dateOnly.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "");
        }
        else
        {
          var valueSerializer = new XmlSerializer(valueType);
          valueSerializer.Serialize(writer, value);
        }
      }
    }

    private object? DeserializePropertyValue (XmlReader reader, PropertyDefinition? propertyDefinition)
    {
      Type? valueType = propertyDefinition == null ? DeserializeCustomValueType(reader) : propertyDefinition.PropertyType;

      reader.ReadStartElement("Property");

      object? value;
      if (reader.IsStartElement("null"))
      {
        reader.ReadStartElement("null"); // no end element for null
        value = null;
      }
      else if (IsDateOnlyType(Assertion.IsNotNull(valueType)))
      {
        var valueString = reader.ReadContentAsString();
        value = DateOnly.ParseExact(valueString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
      }
      else if (ExtensibleEnumUtility.IsExtensibleEnumType(Assertion.IsNotNull(valueType)))
      {
        string idString = reader.ReadContentAsString();
        value = ExtensibleEnumUtility.GetDefinition(valueType).GetValueInfoByID(idString).Value;
      }
      else if (IsObjectID(valueType))
      {
        string objectIDString = reader.ReadContentAsString();
        value = ObjectID.Parse(objectIDString);
      }
      else
      {
        var valueDeserializer = new XmlSerializer(valueType);
        value = valueDeserializer.Deserialize(reader);
      }
      reader.ReadEndElement();
      return value;
    }

    [return: NotNullIfNotNull("value")]
    private Type? SerializeCustomValueType (XmlWriter writer, object? value)
    {
      Type? valueType = value != null ? value.GetType() : null;

      if (valueType == null)
        writer.WriteAttributeString("Type", "null");
      else if (IsObjectID(valueType))
        writer.WriteAttributeString("Type", "ObjectID");
      else
        writer.WriteAttributeString("Type", valueType.GetAssemblyQualifiedNameChecked());

      return valueType;
    }

    private Type? DeserializeCustomValueType (XmlReader reader)
    {
      string? valueTypeAttribute = reader.GetAttribute("Type");
      Assertion.IsNotNull(valueTypeAttribute, "No value was found for required attribute 'Type' on the current node.");
      switch (valueTypeAttribute)
      {
        case "null":
          return null;
        case "ObjectID":
          return typeof(ObjectID);
        default:
          return TypeUtility.GetType(valueTypeAttribute, true);
      }
    }

    private bool IsDateOnlyType (Type type)
    {
      return type == typeof(DateOnly) || type == typeof(DateOnly?);
    }

    private static bool IsObjectID (Type valueType)
    {
      return typeof(ObjectID).IsAssignableFrom(valueType);
    }
  }
}
