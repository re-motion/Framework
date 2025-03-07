using System;

namespace Remotion.Web.Development.WebTesting.BrowserLog;

public static class ExpectedBrowserLogMessages
{
  public const string FavIconNotFound = @".*favicon\.ico - Failed to load resource: the server responded with a status of 404 \(Not Found\)";
  public const string FontNotUsed = @"The resource http://.+/Roboto-\w+\.ttf was preloaded using link preload but not used within a few seconds from the window's load event\..*";
  public const string BeforeUnloadAlertBlocked = @".*Blocked attempt to show a 'beforeunload' confirmation panel for a frame that never had a user gesture since its load\. .*";
  public const string LoadResourceCausedInternalServerErrorTemplate = @".*{0}( - | )Failed to load resource: the server responded with a status of 500 \(Internal Server Error\)";
}
