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
using Remotion.Data.DomainObjects;
using Remotion.Security.Schemas;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  public class MetadataImporter
  {
    private delegate T CreateItemDelegate<T> (XmlNamespaceManager namespaceManager, XmlNode itemNode) where T: MetadataObject;

    private ClientTransaction _transaction;
    private Dictionary<Guid, SecurableClassDefinition> _classes;
    private Dictionary<Guid, StatePropertyDefinition> _stateProperties;
    private Dictionary<Guid, AbstractRoleDefinition> _abstractRoles;
    private Dictionary<Guid, AccessTypeDefinition> _accessTypes;

    private Dictionary<Guid, Guid> _baseClassReferences;
    private Dictionary<Guid, List<Guid>> _statePropertyReferences;
    private Dictionary<Guid, List<Guid>> _accessTypeReferences;

    private int _securableClassDefinitionCount = 0;
    private int _accessTypeDefinitionCount = 0;
    private int _abstractRoleDefinitionCount = 0;
    private int _statePropertyDefinitionCount = 0;

    public MetadataImporter (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull("transaction", transaction);

      _transaction = transaction;
      _classes = new Dictionary<Guid, SecurableClassDefinition>();
      _stateProperties = new Dictionary<Guid, StatePropertyDefinition>();
      _abstractRoles = new Dictionary<Guid, AbstractRoleDefinition>();
      _accessTypes = new Dictionary<Guid, AccessTypeDefinition>();

      _baseClassReferences = new Dictionary<Guid, Guid>();
      _statePropertyReferences = new Dictionary<Guid, List<Guid>>();
      _accessTypeReferences = new Dictionary<Guid, List<Guid>>();
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public Dictionary<Guid, SecurableClassDefinition> Classes
    {
      get { return _classes; }
    }

    public Dictionary<Guid, StatePropertyDefinition> StateProperties
    {
      get { return _stateProperties; }
    }

    public Dictionary<Guid, AbstractRoleDefinition> AbstractRoles
    {
      get { return _abstractRoles; }
    }

    public Dictionary<Guid, AccessTypeDefinition> AccessTypes
    {
      get { return _accessTypes; }
    }

    public void Import (string metadataFilePath)
    {
      XmlDocument metadataXmlDocument = new XmlDocument();
      metadataXmlDocument.Load(metadataFilePath);

      Import(metadataXmlDocument);
    }

    public void Import (XmlDocument metadataXmlDocument)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        SecurityMetadataSchema metadataSchema = new SecurityMetadataSchema();
        if (!metadataXmlDocument.Schemas.Contains(metadataSchema.SchemaUri))
          metadataXmlDocument.Schemas.Add(metadataSchema.LoadSchemaSet());

        metadataXmlDocument.Validate(null);

        XmlNamespaceManager namespaceManager = new XmlNamespaceManager(metadataXmlDocument.NameTable);
        namespaceManager.AddNamespace("md", metadataSchema.SchemaUri);

        AddItem(_classes, metadataXmlDocument, "/md:securityMetadata/md:classes/md:class", namespaceManager, CreateSecurableClassDefinition);
        AddItem(
            _stateProperties,
            metadataXmlDocument,
            "/md:securityMetadata/md:stateProperties/md:stateProperty",
            namespaceManager,
            CreateStatePropertyDefinition);
        AddItem(
            _abstractRoles,
            metadataXmlDocument,
            "/md:securityMetadata/md:abstractRoles/md:abstractRole",
            namespaceManager,
            CreateAbstractRoleDefinition);
        AddItem(_accessTypes, metadataXmlDocument, "/md:securityMetadata/md:accessTypes/md:accessType", namespaceManager, CreateAccessTypeDefinition);

        LinkDerivedClasses();
        LinkStatePropertiesToClasses();
        LinkAccessTypesToClasses();
      }
    }

    private void AddItem<T> (
        Dictionary<Guid, T> dictionary,
        XmlNode parentNode,
        string xpath,
        XmlNamespaceManager namespaceManager,
        CreateItemDelegate<T> createItemDelegate) where T: MetadataObject
    {
      XmlNodeList itemNodes = parentNode.SelectNodes(xpath, namespaceManager)!;
      foreach (XmlNode itemNode in itemNodes)
      {
        T item = createItemDelegate(namespaceManager, itemNode);
        dictionary.Add(item.MetadataItemID, item);
      }
    }

    private void LinkDerivedClasses ()
    {
      foreach (Guid classID in _baseClassReferences.Keys)
      {
        SecurableClassDefinition securableClass = _classes[classID];
        Guid baseClassID = _baseClassReferences[classID];

        if (!_classes.ContainsKey(baseClassID))
          throw new ImportException(string.Format("The base class '{0}' referenced by the class '{1}' could not be found.", baseClassID, classID));

        SecurableClassDefinition baseClass = _classes[_baseClassReferences[classID]];
        securableClass.BaseClass = baseClass;
      }
    }

    private void LinkStatePropertiesToClasses ()
    {
      foreach (Guid classID in _statePropertyReferences.Keys)
      {
        List<Guid> statePropertyReferences = _statePropertyReferences[classID];

        foreach (Guid statePropertyID in statePropertyReferences)
        {
          if (!_stateProperties.ContainsKey(statePropertyID))
            throw new ImportException(string.Format("The state property '{0}' referenced by the class '{1}' could not be found.", statePropertyID, classID));

          _classes[classID].AddStateProperty(_stateProperties[statePropertyID]);
        }
      }
    }

    private void LinkAccessTypesToClasses ()
    {
      foreach (Guid classID in _accessTypeReferences.Keys)
      {
        List<Guid> accessTypeReferences = _accessTypeReferences[classID];

        foreach (Guid accessTypeID in accessTypeReferences)
        {
          if (!_accessTypes.ContainsKey(accessTypeID))
            throw new ImportException(string.Format("The access type '{0}' referenced by the class '{1}' could not be found.", accessTypeID, classID));

          _classes[classID].AddAccessType(_accessTypes[accessTypeID]);
        }
      }
    }

    private SecurableClassDefinition CreateSecurableClassDefinition (XmlNamespaceManager namespaceManager, XmlNode securableClassDefinitionNode)
    {
      SecurableClassDefinition securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.Name = GetAttributeValue(securableClassDefinitionNode, "name");
      securableClassDefinition.MetadataItemID = new Guid(GetAttributeValue(securableClassDefinitionNode, "id"));
      securableClassDefinition.Index = _securableClassDefinitionCount;
      _securableClassDefinitionCount++;

      var baseClassAttribute = securableClassDefinitionNode.Attributes!["base"];
      if (baseClassAttribute != null)
      {
        Guid baseClassID = new Guid(baseClassAttribute.Value);
        _baseClassReferences.Add(securableClassDefinition.MetadataItemID, baseClassID);
      }

      CreateReferences(
          securableClassDefinition, securableClassDefinitionNode, namespaceManager, "md:stateProperties/md:statePropertyRef", _statePropertyReferences);
      CreateReferences(
          securableClassDefinition, securableClassDefinitionNode, namespaceManager, "md:accessTypes/md:accessTypeRef", _accessTypeReferences);

      return securableClassDefinition;
    }

    private void CreateReferences (
        SecurableClassDefinition securableClassDefinition,
        XmlNode securableClassDefinitionNode,
        XmlNamespaceManager namespaceManager,
        string xpath,
        Dictionary<Guid, List<Guid>> referenceRegistry)
    {
      List<Guid> references = new List<Guid>();
      XmlNodeList referenceNodes = securableClassDefinitionNode.SelectNodes(xpath, namespaceManager)!;

      foreach (XmlNode referenceNode in referenceNodes)
        references.Add(new Guid(referenceNode.InnerText));

      referenceRegistry.Add(securableClassDefinition.MetadataItemID, references);
    }

    private AbstractRoleDefinition CreateAbstractRoleDefinition (XmlNamespaceManager namespaceManager, XmlNode abstractRoleDefinitionNode)
    {
      AbstractRoleDefinition roleDefinition = AbstractRoleDefinition.NewObject();
      roleDefinition.Name = GetAttributeValue(abstractRoleDefinitionNode, "name");
      roleDefinition.MetadataItemID = new Guid(GetAttributeValue(abstractRoleDefinitionNode, "id"));
      roleDefinition.Index = _abstractRoleDefinitionCount;
      _abstractRoleDefinitionCount++;
      roleDefinition.Value = int.Parse(GetAttributeValue(abstractRoleDefinitionNode, "value"));

      return roleDefinition;
    }

    private AccessTypeDefinition CreateAccessTypeDefinition (XmlNamespaceManager namespaceManager, XmlNode accessTypeDefinitionNode)
    {
      AccessTypeDefinition accessTypeDefinition = AccessTypeDefinition.NewObject();
      accessTypeDefinition.Name = GetAttributeValue(accessTypeDefinitionNode, "name");
      accessTypeDefinition.MetadataItemID = new Guid(GetAttributeValue(accessTypeDefinitionNode, "id"));
      accessTypeDefinition.Value = int.Parse(GetAttributeValue(accessTypeDefinitionNode, "value"));
      accessTypeDefinition.Index = _accessTypeDefinitionCount;
      _accessTypeDefinitionCount++;

      return accessTypeDefinition;
    }

    private StatePropertyDefinition CreateStatePropertyDefinition (XmlNamespaceManager namespaceManager, XmlNode statePropertyDefinitionNode)
    {
      StatePropertyDefinition statePropertyDefinition = StatePropertyDefinition.NewObject();
      statePropertyDefinition.MetadataItemID = new Guid(GetAttributeValue(statePropertyDefinitionNode, "id"));
      statePropertyDefinition.Name = GetAttributeValue(statePropertyDefinitionNode, "name");
      statePropertyDefinition.Index = _statePropertyDefinitionCount;
      _statePropertyDefinitionCount++;

      XmlNodeList stateNodes = statePropertyDefinitionNode.SelectNodes("md:state", namespaceManager)!;
      foreach (XmlNode stateNode in stateNodes)
        statePropertyDefinition.AddState(CreateStateDefinition(namespaceManager, stateNode));

      return statePropertyDefinition;
    }

    private StateDefinition CreateStateDefinition (XmlNamespaceManager namespaceManager, XmlNode stateDefinitionNode)
    {
      StateDefinition stateDefinition = StateDefinition.NewObject();
      stateDefinition.Name = GetAttributeValue(stateDefinitionNode, "name");
      stateDefinition.Value = int.Parse(GetAttributeValue(stateDefinitionNode, "value"));
      stateDefinition.Index = stateDefinition.Value;

      return stateDefinition;
    }

    private string GetAttributeValue (XmlNode node, string attributeName)
    {
      var attribute = node.Attributes![attributeName];
      Assertion.IsNotNull(attribute, "{0}/@{1}", node.Name, attribute);

      return attribute.Value;
    }
  }
}
