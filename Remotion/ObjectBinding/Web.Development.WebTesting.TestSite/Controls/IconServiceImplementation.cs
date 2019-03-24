using System;
using System.Web;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.Services;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public class IconServiceImplementation
  {
    public IconProxy GetIcon (HttpContextBase httpContext, string businessObjectClass, string businessObject, string arguments)
    {
      if (businessObjectClass == null)
        return null;

      var type = TypeUtility.GetType (businessObjectClass, true);
      var businessObjectProvider = BindableObjectProvider.GetProviderForBindableObjectType (type);
      var bindableObjectClass = businessObjectProvider.GetBindableObjectClass (type);
      IBusinessObjectWithIdentity businessObjectWithIdentity = null;
      if (!string.IsNullOrEmpty (businessObject))
      {
        var businessObjectClassWithIdentity = (IBusinessObjectClassWithIdentity) bindableObjectClass;
        businessObjectWithIdentity = businessObjectClassWithIdentity.GetObject (businessObject);
      }

      var iconInfo = BusinessObjectBoundWebControl.GetIcon (businessObjectWithIdentity, bindableObjectClass.BusinessObjectProvider);
      if (iconInfo == null)
        return null;

      return IconProxy.Create (httpContext, iconInfo);
    }
  }
}