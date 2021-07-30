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
  /// Declares extension methods for HtmlHeadAppender.
  /// </summary>
  public static class HtmlHeadAppenderExtensions
  {
    /// <summary>
    /// Registers jquery and Utilities.js in non-themed HTML folder of Remotion.Web.dll.
    /// </summary>
    public static void RegisterUtilitiesJavaScriptInclude (this HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string jqueryKey = typeof (HtmlHeadContents).GetFullNameChecked() + "_JQuery";
      var jqueryFileUrl = ResourceUrlFactory.CreateResourceUrl (typeof (HtmlHeadContents), ResourceType.Html, "jquery-1.6.4.js");
      htmlHeadAppender.RegisterJavaScriptInclude (jqueryKey, jqueryFileUrl);

      string jQueryIframeShimScriptKey = typeof (HtmlHeadContents).GetFullNameChecked() + "_JQueryBgiFrames";
      var href = ResourceUrlFactory.CreateResourceUrl (typeof (HtmlHeadContents), ResourceType.Html, "jquery.IFrameShim.js");
      htmlHeadAppender.RegisterJavaScriptInclude (jQueryIframeShimScriptKey, href);

      string utilitiesKey = typeof (HtmlHeadContents).GetFullNameChecked() + "_Utilities";
      var utilitiesScripFileUrl = ResourceUrlFactory.CreateResourceUrl (typeof (HtmlHeadContents), ResourceType.Html, "Utilities.js");
      htmlHeadAppender.RegisterJavaScriptInclude (utilitiesKey, utilitiesScripFileUrl);
    }

    /// <summary>
    /// Registers jquery.IFrameShim.js in non-themed HTML folder of Remotion.Web.dll.
    /// </summary>
    [Obsolete ("JQuery iFrame shim was only needed for IE, which is no longer supported. (Version 3.0.0-alpha.12)", true)]
    public static void RegisterJQueryIFrameShimJavaScriptInclude (this HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      throw new NotSupportedException ("JQuery iFrame shim was only needed for IE, which is no longer supported. (Version 3.0.0-alpha.12)");
    }

    /// <summary>
    /// Registeres style.css in themed HTML folder of Remotion.Web.dll with priority level <see cref="HtmlHeadAppender.Priority.Page"/>.
    /// </summary>
    public static void RegisterPageStylesheetLink (this HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string key = typeof (HtmlHeadContents).GetFullNameChecked() + "_Style";
      var url = InfrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Html, "Style.css");
      htmlHeadAppender.RegisterStylesheetLink (key, url, HtmlHeadAppender.Priority.Page);
    }

    /// <summary>
    /// Registers Common.css in themed HTML folder of Remotion.Web.dll with priority level <see cref="HtmlHeadAppender.Priority.Library"/>.
    /// </summary>
    public static void RegisterCommonStyleSheet (this HtmlHeadAppender htmlHeadAppender)
    {
      var key = typeof (HtmlHeadContents).GetFullNameChecked() + "_CommonStyle";
      var url = ResourceUrlFactory.CreateThemedResourceUrl (typeof (HtmlHeadContents), ResourceType.Html, "Common.css");

      htmlHeadAppender.RegisterStylesheetLink (key, url, HtmlHeadAppender.Priority.Library);
    }

    private static IResourceUrlFactory ResourceUrlFactory
    {
      get { return SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>(); }
    }

    private static IInfrastructureResourceUrlFactory InfrastructureResourceUrlFactory
    {
      get { return SafeServiceLocator.Current.GetInstance<IInfrastructureResourceUrlFactory>(); }
    }
  }
}