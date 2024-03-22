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
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.UI
{
  /// <summary>
  /// Declares extension methods for <see cref="HtmlHeadAppender"/>.
  /// </summary>
  public static class HtmlHeadAppenderExtensions
  {
    /// <summary>
    /// Registers the client scripts in non-themed HTML folder of Remotion.ObjectBinding.Web.dll.
    /// </summary>
    public static void RegisterObjectBindingWebClientScriptInclude (this HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull(nameof(htmlHeadAppender), htmlHeadAppender);

      htmlHeadAppender.RegisterWebClientScriptInclude();

      const string scriptKey = "Remotion.ObjectBinding.Web.ClientScript";
      if (htmlHeadAppender.IsRegistered(scriptKey))
        return;

      var objectBindingWebClientScriptUrl = ResourceUrlFactory.CreateResourceUrl(typeof(HtmlHeadAppenderExtensions), ResourceType.Html, "Remotion.ObjectBinding.Web.ClientScript.js");
      htmlHeadAppender.RegisterJavaScriptInclude(scriptKey, objectBindingWebClientScriptUrl);
    }

    private static IResourceUrlFactory ResourceUrlFactory => SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>();
  }
}
