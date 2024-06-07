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
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  public static class GlobalResourcesHelper
  {
    public static string GetString (GlobalResources value)
    {
      var globalizationSerivce = SafeServiceLocator.Current.GetInstance<IGlobalizationService>();
      return GetString(globalizationSerivce, value);
    }

    public static string GetString (IGlobalizationService service, GlobalResources value)
    {
      var resourceManager = service.GetResourceManager(typeof(GlobalResources));
      return resourceManager.GetString(value);
    }

    public static WebString GetText (GlobalResources value)
    {
      var globalizationService = SafeServiceLocator.Current.GetInstance<IGlobalizationService>();
      return GetText(globalizationService, value);
    }

    public static WebString GetText (IGlobalizationService service, GlobalResources value)
    {
      var resourceManager = service.GetResourceManager(typeof(GlobalResources));
      return resourceManager.GetText(value);
    }

    public static WebString GetHtml (GlobalResources value)
    {
      var globalizationService = SafeServiceLocator.Current.GetInstance<IGlobalizationService>();
      return GetHtml(globalizationService, value);
    }

    public static WebString GetHtml (IGlobalizationService service, GlobalResources value)
    {
      var resourceManager = service.GetResourceManager(typeof(GlobalResources));
      return resourceManager.GetHtml(value);
    }
  }

  [ResourceIdentifiers]
  [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.GlobalResources")]
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
