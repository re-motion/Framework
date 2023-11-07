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
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI
{
  /// <summary>
  /// Declares extension methods for <see cref="HtmlHeadAppender"/>.
  /// </summary>
  public static class HtmlHeadAppenderExtensions
  {
    /// <summary>
    /// Registers the client scripts in non-themed HTML folder of Remotion.Web.dll.
    /// </summary>
    public static void RegisterWebClientScriptInclude (this HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      const string scriptKey = "Remotion.Web.ClientScript";
      if (htmlHeadAppender.IsRegistered(scriptKey))
        return;

      var webClientScriptUrl = ResourceUrlFactory.CreateResourceUrl(typeof(HtmlHeadAppenderExtensions), ResourceType.Html, "Remotion.Web.ClientScript.js");
      htmlHeadAppender.RegisterJavaScriptInclude(scriptKey, webClientScriptUrl);
    }

    /// <summary>
    /// Registers Utilities.js in non-themed HTML folder of Remotion.Web.dll.
    /// </summary>
    [Obsolete("Please use RegisterWebClientScript instead. (Version 6.0.0)")]
    public static void RegisterUtilitiesJavaScriptInclude (this HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      RegisterWebClientScriptInclude(htmlHeadAppender);
    }

    /// <summary>
    /// Registers jquery.IFrameShim.js in non-themed HTML folder of Remotion.Web.dll.
    /// </summary>
    [Obsolete("JQuery iFrame shim was only needed for IE, which is no longer supported. (Version 3.0.0-alpha.12)", true)]
    public static void RegisterJQueryIFrameShimJavaScriptInclude (this HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      throw new NotSupportedException("JQuery iFrame shim was only needed for IE, which is no longer supported. (Version 3.0.0-alpha.12)");
    }

    /// <summary>
    /// Registeres style.css in themed HTML folder of Remotion.Web.dll with priority level <see cref="HtmlHeadAppender.Priority.Page"/>.
    /// </summary>
    public static void RegisterPageStylesheetLink (this HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      string key = typeof(HtmlHeadContents).GetFullNameChecked() + "_Style";
      var url = InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Html, "Style.css");
      htmlHeadAppender.RegisterStylesheetLink(key, url, HtmlHeadAppender.Priority.Page);
    }

    /// <summary>
    /// Registers Common.css in themed HTML folder of Remotion.Web.dll with priority level <see cref="HtmlHeadAppender.Priority.Library"/>.
    /// </summary>
    public static void RegisterCommonStyleSheet (this HtmlHeadAppender htmlHeadAppender)
    {
      var key = typeof(HtmlHeadContents).GetFullNameChecked() + "_CommonStyle";
      var url = ResourceUrlFactory.CreateThemedResourceUrl(typeof(HtmlHeadContents), ResourceType.Html, "Common.css");

      htmlHeadAppender.RegisterStylesheetLink(key, url, HtmlHeadAppender.Priority.Library);

      var robotoRegularKey = typeof(HtmlHeadContents).GetFullNameChecked() + "_RobotoRegular";
      var robotoRegularUrl = ResourceUrlFactory.CreateThemedResourceUrl(typeof(HtmlHeadContents), ResourceType.Html, "Roboto-Regular.ttf");
      htmlHeadAppender.RegisterHeadElement(robotoRegularKey, new FontPreloadLink(robotoRegularUrl, "font/ttf"), HtmlHeadAppender.Priority.Script);

      var robotoMediumKey = typeof(HtmlHeadContents).GetFullNameChecked() + "_RobotoMedium";
      var robotoMediumUrl = ResourceUrlFactory.CreateThemedResourceUrl(typeof(HtmlHeadContents), ResourceType.Html, "Roboto-Medium.ttf");
      htmlHeadAppender.RegisterHeadElement(robotoMediumKey, new FontPreloadLink(robotoMediumUrl, "font/ttf"), HtmlHeadAppender.Priority.Script);

      var robotoBoldKey = typeof(HtmlHeadContents).GetFullNameChecked() + "_RobotoBold";
      var robotoBoldUrl = ResourceUrlFactory.CreateThemedResourceUrl(typeof(HtmlHeadContents), ResourceType.Html, "Roboto-Bold.ttf");
      htmlHeadAppender.RegisterHeadElement(robotoBoldKey, new FontPreloadLink(robotoBoldUrl, "font/ttf"), HtmlHeadAppender.Priority.Script);
    }

    private static IResourceUrlFactory ResourceUrlFactory
    {
      get { return SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>(); }
    }

    private static IInfrastructureResourceUrlFactory InfrastructureResourceUrlFactory
    {
      get { return SafeServiceLocator.Current.GetInstance<IInfrastructureResourceUrlFactory>(); }
    }

    private static ResourceTheme ResourceTheme
    {
      get { return SafeServiceLocator.Current.GetInstance<ResourceTheme>(); }
    }
  }
}
