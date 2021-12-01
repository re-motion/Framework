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
      return GetString (globalizationSerivce, value);
    }

    public static string GetString(IGlobalizationService service, GlobalResources value)
    {
      var resourceManager = service.GetResourceManager(typeof(GlobalResources));
      return resourceManager.GetString (value);
    }

    public static WebString GetText (GlobalResources value)
    {
      var globalizationService = SafeServiceLocator.Current.GetInstance<IGlobalizationService>();
      return GetText (globalizationService, value);
    }

    public static WebString GetText (IGlobalizationService service, GlobalResources value)
    {
      var resourceManager = service.GetResourceManager (typeof (GlobalResources));
      return resourceManager.GetText (value);
    }

    public static WebString GetHtml (GlobalResources value)
    {
      var globalizationService = SafeServiceLocator.Current.GetInstance<IGlobalizationService>();
      return GetHtml (globalizationService, value);
    }

    public static WebString GetHtml (IGlobalizationService service, GlobalResources value)
    {
      var resourceManager = service.GetResourceManager (typeof (GlobalResources));
      return resourceManager.GetHtml (value);
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