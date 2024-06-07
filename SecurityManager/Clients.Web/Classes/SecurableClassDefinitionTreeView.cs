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
using Remotion.Globalization;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public class SecurableClassDefinitionTreeView : BocTreeView
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.Classes.SecurableClassDefinitionTreeViewResources")]
    public enum ResourceIdentifier
    {
      NoAclsText,
      SingleAclText,
      MultipleAclsText,
    }

    public SecurableClassDefinitionTreeView ()
    {
    }

    protected virtual IResourceManager GetResourceManager ()
    {
      return GetResourceManager(typeof(ResourceIdentifier));
    }

    protected override Badge? GetBadge (IBusinessObjectWithIdentity businessObject)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);

      var classDefinition = businessObject as SecurableClassDefinition;
      if (classDefinition == null)
        return null;

      var aclCount = 0;
      if (classDefinition.StatelessAccessControlList != null)
        aclCount++;
      aclCount += classDefinition.StatefulAccessControlLists.Count;

      var resourceManager = GetResourceManager(typeof(ResourceIdentifier));

      if (aclCount == 0)
        return CreateBadge(resourceManager.GetString(ResourceIdentifier.NoAclsText));
      else if (aclCount == 1)
        return CreateBadge(resourceManager.GetString(ResourceIdentifier.SingleAclText));
      else
        return CreateBadge(string.Format(resourceManager.GetString(ResourceIdentifier.MultipleAclsText), aclCount));

      static Badge CreateBadge (string text) => new Badge(
          PlainTextString.CreateFromText($"({text})"),
          PlainTextString.CreateFromText(text));
    }
  }
}
