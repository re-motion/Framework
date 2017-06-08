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
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  public static class GlobalResourcesHelper
  {
    public static string GetString (GlobalResources value)
    {
      var globalizationSerivce = SafeServiceLocator.Current.GetInstance<IGlobalizationService>();
      return GetString (globalizationSerivce, value);
    }

    public static string GetString(IGlobalizationService service, GlobalResources value)
    {
      var resourceManager = service.GetResourceManager(typeof(GlobalResources));
      return resourceManager.GetString(value);
    }
  }

  [ResourceIdentifiers]
  [MultiLingualResources ("Remotion.SecurityManager.Clients.Web.Globalization.UI.GlobalResources")]
  public enum GlobalResources
  {
    User,
    Save,
    Cancel,
    Apply,
    Edit,
    ErrorMessage,
    GroupType,
    Group,
    Position,
    New,
    Delete,
    Add,
    Remove,
    AccessControl,
    OrganizationalStructure,
    SecurableClassDefinition,
    Tenant,
  }
}