// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Xml;
using Remotion.Data.DomainObjects;
using Remotion.Security.Metadata;
using Remotion.Security.Schemas;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  public class CultureImporter
  {
    private ClientTransaction _transaction;
    private List<LocalizedName> _localizedNames;
    private List<Culture> _cultures;

    public CultureImporter (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      _transaction = transaction;
      _localizedNames = new List<LocalizedName> ();
      _cultures = new List<Culture> ();
    }

    public List<LocalizedName> LocalizedNames
    {
      get { return _localizedNames; }
    }

    public List<Culture> Cultures
    {
      get { return _cultures; }
    }

    public void Import (string filePath)
    {
      XmlDocument xmlDocument = new XmlDocument ();
      xmlDocument.Load (filePath);

      Import (xmlDocument);
    }

    public void Import (XmlDocument document)
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        SecurityMetadataLocalizationSchema schema = new SecurityMetadataLocalizationSchema();
        if (!document.Schemas.Contains (schema.SchemaUri))
          document.Schemas.Add (schema.LoadSchemaSet());

        document.Validate (null);

        XmlNamespaceManager namespaceManager = new XmlNamespaceManager (document.NameTable);
        namespaceManager.AddNamespace ("mdl", schema.SchemaUri);

        Culture culture = ImportCulture (document.DocumentElement, namespaceManager);
        ImportLocalizedNames (culture, document, namespaceManager);
      }
    }

    private Culture ImportCulture (XmlElement rootElement, XmlNamespaceManager namespaceManager)
    {
      string cultureName = rootElement.Attributes["culture"].Value;
      // TODO: Convert to CultureInfo via GetCulture
      Culture culture = Culture.NewObject (cultureName);

      _cultures.Add (culture);

      return culture;
    }

    private void ImportLocalizedNames (Culture culture, XmlNode parentNode, XmlNamespaceManager namespaceManager)
    {
      XmlNodeList nameNodes = parentNode.SelectNodes ("/mdl:localizedNames/mdl:localizedName", namespaceManager);

      foreach (XmlNode nameNode in nameNodes)
      {
        LocalizedName localizedName = ImportLocalizedName (culture, namespaceManager, nameNode);
        _localizedNames.Add (localizedName);
      }
    }

    private LocalizedName ImportLocalizedName (Culture culture, XmlNamespaceManager namespaceManager, XmlNode nameNode)
    {
      string metadataID = nameNode.Attributes["ref"].Value;
      XmlAttribute commentAttribute = nameNode.Attributes["comment"];
      
      MetadataObject metadataObject = MetadataObject.Find (metadataID);
      if (metadataObject == null)
      {
        string objectDetails = commentAttribute == null ? string.Empty : "('" + commentAttribute.Value + "') ";
        throw new ImportException (string.Format ("The metadata object with the ID '{0}' {1}could not be found.", metadataID, objectDetails));
      }

      string text = nameNode.InnerText.Trim ();

      LocalizedName localizedName = metadataObject.GetLocalizedName (culture);
      if (localizedName != null)
      {
        localizedName.Text = text;
        return localizedName;
      }

      return LocalizedName.NewObject (text, culture, metadataObject);
    }
  }
}
