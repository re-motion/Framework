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
using System.IO;
using System.Xml;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.ConfigurationLoader
{
  public static class LoaderUtility
  {
    public static Type GetType (string typeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("typeName", typeName);

      return TypeUtility.GetType(typeName.Trim(), throwOnError: true)!;
    }

    public static Type GetType (XmlNode node)
    {
      ArgumentUtility.CheckNotNull("node", node);

      return GetType(node.InnerText);
    }

    public static Type GetType (XmlNode node, string xPath, XmlNamespaceManager namespaceManager)
    {
      ArgumentUtility.CheckNotNull("node", node);
      ArgumentUtility.CheckNotNullOrEmpty("xPath", xPath);
      ArgumentUtility.CheckNotNull("namespaceManager", namespaceManager);

      var selectedNode = node.SelectSingleNode(xPath, namespaceManager);
      if (selectedNode == null)
        throw new ConfigurationException(string.Format("XPath '{0}' does not exist on node '{1}'.", xPath, node.Name));
      return GetType(selectedNode);
    }

    public static Type? GetOptionalType (XmlNode selectionNode, string xPath, XmlNamespaceManager namespaceManager)
    {
      ArgumentUtility.CheckNotNull("selectionNode", selectionNode);
      ArgumentUtility.CheckNotNullOrEmpty("xPath", xPath);
      ArgumentUtility.CheckNotNull("namespaceManager", namespaceManager);

      XmlNode? typeNode = selectionNode.SelectSingleNode(xPath, namespaceManager);

      if (typeNode != null)
        return GetType(typeNode);
      else
        return null;
    }

    public static string GetConfigurationFileName (string appSettingKey, string defaultFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("appSettingKey", appSettingKey);
      ArgumentUtility.CheckNotNullOrEmpty("defaultFileName", defaultFileName);

      string? fileName = ConfigurationWrapper.Current.GetAppSetting(appSettingKey, false);
      if (fileName != null)
        return fileName;

      return Path.Combine(ReflectionUtility.GetConfigFileDirectory(), defaultFileName);
    }
  }
}
