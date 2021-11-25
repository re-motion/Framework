using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.DropDownMenuImplementation
{
  public static class DropDownMenuExtensions
  {
    public static void SetLoadMenuItemStatus (
        this IDropDownMenu dropDownMenu,
        string servicePath,
        string serviceMethodName,
        Dictionary<string, string?> stringValueParametersDictionary)
    {
      var resolvedServicePath = dropDownMenu.ResolveClientUrl(servicePath);

      var visibleMenuItemIDs = dropDownMenu.MenuItems.Cast<WebMenuItem>()
          .Where(m => m.IsVisible)
          .Where(m => !string.IsNullOrEmpty(m.ItemID))
          .Select(m => m.ItemID);
      var stringArrayParametersDictionary = new Dictionary<string, IReadOnlyCollection<string>?>();
      stringArrayParametersDictionary.Add("itemIDs", visibleMenuItemIDs.ToArray());


      var script = new StringBuilder();
      script.Append("function (onSuccess, onError)").AppendLine();
      script.Append("{").AppendLine();
      script.Append("  const serviceUrl = '").Append(resolvedServicePath).Append("';").AppendLine();
      script.Append("  const serviceMethod = '").Append(serviceMethodName).Append("';").AppendLine();

      script.Append("  const params = ");
      script.WriteDictionaryAsJson(stringValueParametersDictionary, stringArrayParametersDictionary);
      script.Append(";").AppendLine();

      script.Append("  WebServiceUtility.Execute (serviceUrl, serviceMethod, params, onSuccess, onError)").AppendLine();
      script.Append("}");

      var scriptComplete = script.ToString();
      dropDownMenu.SetLoadMenuItemStatus(scriptComplete);
    }
  }
}