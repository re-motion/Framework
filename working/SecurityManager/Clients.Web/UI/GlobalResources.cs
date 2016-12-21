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