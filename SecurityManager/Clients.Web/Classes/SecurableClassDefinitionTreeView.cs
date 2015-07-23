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
using Remotion.Globalization;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public class SecurableClassDefinitionTreeView : BocTreeView
  {
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.SecurityManager.Clients.Web.Globalization.Classes.SecurableClassDefinitionTreeViewResources")]
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
      return GetResourceManager (typeof (ResourceIdentifier));
    }

    protected override string GetText (IBusinessObjectWithIdentity businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      string text = base.GetText (businessObject);

      SecurableClassDefinition classDefinition = businessObject as SecurableClassDefinition;
      if (classDefinition == null)
        return text;

      int aclCount = 0;
      if (classDefinition.StatelessAccessControlList != null)
        aclCount++;
      aclCount += classDefinition.StatefulAccessControlLists.Count;

      var resourceManager = GetResourceManager(typeof(ResourceIdentifier));
      if (aclCount == 0)
        return string.Format (resourceManager.GetString (ResourceIdentifier.NoAclsText), text);
      if (aclCount == 1)
        return string.Format (resourceManager.GetString (ResourceIdentifier.SingleAclText), text);
      else
        return string.Format (resourceManager.GetString (ResourceIdentifier.MultipleAclsText), text, aclCount);
    }
  }
}
