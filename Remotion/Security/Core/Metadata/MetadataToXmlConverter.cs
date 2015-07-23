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
using Remotion.Security.Schemas;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class MetadataToXmlConverter : IMetadataConverter
  {
    private delegate XmlNode CreateCollectionItemNodeDelegate<T> (XmlDocument document, T itemInfo);

    private SecurityMetadataSchema _metadataSchema;

    public MetadataToXmlConverter ()
    {
      _metadataSchema = new SecurityMetadataSchema ();
    }

    public void ConvertAndSave (MetadataCache cache, string filename)
    {
      ArgumentUtility.CheckNotNull ("cache", cache);
      ArgumentUtility.CheckNotNullOrEmpty ("filename", filename);

      XmlDocument xmlDocument = Convert (cache);
      xmlDocument.Save (filename);
    }

    public XmlDocument Convert (MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("cache", cache);

      XmlDocument document = new XmlDocument ();
      XmlDeclaration declaration = document.CreateXmlDeclaration ("1.0", string.Empty, string.Empty);
      document.AppendChild (declaration);

      XmlElement rootElement = document.CreateElement ("securityMetadata", _metadataSchema.SchemaUri);

      AppendCollection (document, rootElement, "classes", cache.GetSecurableClassInfos (), CreateClassNode);
      AppendCollection (document, rootElement, "stateProperties", cache.GetStatePropertyInfos (), CreateStatePropertyNode);
      AppendCollection (document, rootElement, "accessTypes", cache.GetAccessTypes (), CreateAccessTypeNode);
      AppendCollection (document, rootElement, "abstractRoles", cache.GetAbstractRoles (), CreateAbstractRoleNode);

      document.AppendChild (rootElement);
      return document;
    }

    private void AppendCollection<T> (
        XmlDocument document, 
        XmlElement parentElement, 
        string collectionElementName, 
        List<T> infos,
        CreateCollectionItemNodeDelegate<T> createCollectionItemNodeDelegate)
    {
      if (infos.Count > 0)
      {
        XmlElement collectionElement = document.CreateElement (collectionElementName, _metadataSchema.SchemaUri);

        foreach (T info in infos)
          collectionElement.AppendChild (createCollectionItemNodeDelegate (document, info));

        parentElement.AppendChild (collectionElement);
      }
    }

    private XmlElement CreateAccessTypeNode (XmlDocument document, EnumValueInfo accessTypeInfo)
    {
      XmlElement accessTypeElement = document.CreateElement ("accessType", _metadataSchema.SchemaUri);
      AppendEnumValueInfoAttributes (document, accessTypeInfo, accessTypeElement);
      return accessTypeElement;
    }

    private XmlElement CreateAbstractRoleNode (XmlDocument document, EnumValueInfo abstractRoleInfo)
    {
      XmlElement abstractRoleElement = document.CreateElement ("abstractRole", _metadataSchema.SchemaUri);
      AppendEnumValueInfoAttributes (document, abstractRoleInfo, abstractRoleElement);
      return abstractRoleElement;
    }

    private void AppendEnumValueInfoAttributes (XmlDocument document, EnumValueInfo enumValueInfo, XmlElement enumValueElement)
    {
      XmlAttribute enumValueIDAttribute = document.CreateAttribute ("id");
      enumValueIDAttribute.Value = enumValueInfo.ID;

      EnumWrapper enumWrapper = EnumWrapper.Get(enumValueInfo.Name, enumValueInfo.TypeName);
      XmlAttribute enumValueNameAttribute = document.CreateAttribute ("name");
      enumValueNameAttribute.Value = enumWrapper.Name;

      XmlAttribute enumValueValueAttribute = document.CreateAttribute ("value");
      enumValueValueAttribute.Value = enumValueInfo.Value.ToString ();

      enumValueElement.Attributes.Append (enumValueIDAttribute);
      enumValueElement.Attributes.Append (enumValueNameAttribute);
      enumValueElement.Attributes.Append (enumValueValueAttribute);
    }

    private XmlElement CreateStatePropertyNode (XmlDocument document, StatePropertyInfo propertyInfo)
    {
      XmlElement propertyElement = document.CreateElement ("stateProperty", _metadataSchema.SchemaUri);

      XmlAttribute propertyIdAttribute = document.CreateAttribute ("id");
      propertyIdAttribute.Value = propertyInfo.ID;
      
      XmlAttribute propertyNameAttribute = document.CreateAttribute ("name");
      propertyNameAttribute.Value = propertyInfo.Name;
      
      propertyElement.Attributes.Append (propertyIdAttribute);
      propertyElement.Attributes.Append (propertyNameAttribute);
      
      foreach (EnumValueInfo enumValueInfo in propertyInfo.Values)
        propertyElement.AppendChild (CreateStatePropertyValueNode (document, enumValueInfo));
      
      return propertyElement;
    }

    private XmlElement CreateStatePropertyValueNode (XmlDocument document, EnumValueInfo enumValueInfo)
    {
      XmlElement propertyValueElement = document.CreateElement ("state", _metadataSchema.SchemaUri);

      XmlAttribute propertyValueNameAttribute = document.CreateAttribute ("name");
      propertyValueNameAttribute.Value = EnumWrapper.Get(enumValueInfo.Name, enumValueInfo.TypeName).Name;
      
      XmlAttribute propertyValueValueAttribute = document.CreateAttribute ("value");
      propertyValueValueAttribute.Value = enumValueInfo.Value.ToString ();

      propertyValueElement.Attributes.Append (propertyValueNameAttribute);
      propertyValueElement.Attributes.Append (propertyValueValueAttribute);
      
      return propertyValueElement;
    }

    private XmlElement CreateClassNode (XmlDocument document, SecurableClassInfo classInfo)
    {
      XmlElement classElement = document.CreateElement ("class", _metadataSchema.SchemaUri);
      
      XmlAttribute classIdAttribute = document.CreateAttribute ("id");
      classIdAttribute.Value = classInfo.ID;
      
      XmlAttribute classNameAttribute = document.CreateAttribute ("name");
      classNameAttribute.Value = classInfo.Name;
      
      classElement.Attributes.Append (classIdAttribute);
      classElement.Attributes.Append (classNameAttribute);

      if (classInfo.BaseClass != null)
      {
        XmlAttribute baseClassAttribute = document.CreateAttribute ("base");
        baseClassAttribute.Value = classInfo.BaseClass.ID;

        classElement.Attributes.Append (baseClassAttribute);
      }

      AppendCollection (document, classElement, "stateProperties", classInfo.Properties, CreateStatePropertyRefElement);
      AppendCollection (document, classElement, "accessTypes", classInfo.AccessTypes, CreateAccessTypeRefElement);

      return classElement;
    }

    private XmlElement CreateStatePropertyRefElement (XmlDocument document, StatePropertyInfo propertyInfo)
    {
      return CreateRefElement (document, "statePropertyRef", propertyInfo.ID);
    }

    private XmlElement CreateAccessTypeRefElement (XmlDocument document, EnumValueInfo accessTypeInfo)
    {
      return CreateRefElement (document, "accessTypeRef", accessTypeInfo.ID);
    }

    private XmlElement CreateRefElement (XmlDocument document, string elementName, string idText)
    {
      XmlElement refElement = document.CreateElement (elementName, _metadataSchema.SchemaUri);

      XmlText idTextNode = document.CreateTextNode (idText);
      refElement.AppendChild (idTextNode);

      return refElement;
    }
  }
}
