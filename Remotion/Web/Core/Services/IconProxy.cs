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
using System.Web;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.Web.Services
{
  /// <summary>
  /// Represents an icon sent over a web service interface.
  /// </summary>
  public sealed class IconProxy
  {
    private readonly string _url;
    private readonly string _alternateText;
    private readonly string _toolTip;
    private readonly string _height;
    private readonly string _width;

    /// <summary>
    /// Creates an <see cref="IconProxy"/> from an <see cref="IconInfo"/>.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> of the current request. Used for resolving the URL. Must not be <see langword="null" />.</param>
    /// <param name="iconInfo">The <see cref="IconInfo"/> to be converted into an <see cref="IconProxy"/>. Must not be <see langword="null" />.</param>
    /// <returns>An <see cref="IconProxy"/> representing the <paramref name="iconInfo"/> in a web service interface. </returns>
    /// <exception cref="ArgumentException">Thrown if the <see cref="IconInfo.Url"/> of the <paramref name="iconInfo"/> is not set.</exception>
    public static IconProxy Create (HttpContextBase httpContext, IconInfo iconInfo)
    {
      ArgumentUtility.CheckNotNull ("httpContext", httpContext);
      ArgumentUtility.CheckNotNull ("iconInfo", iconInfo);

      if (string.IsNullOrEmpty (iconInfo.Url))
        throw new ArgumentException ("IconProxy does not support IconInfo objects without an empty Url.", "iconInfo");
      var absoluteUrl = UrlUtility.GetAbsoluteUrl (httpContext, iconInfo.Url);

      return new IconProxy (
          absoluteUrl,
          StringUtility.EmptyToNull (iconInfo.AlternateText),
          StringUtility.EmptyToNull (iconInfo.ToolTip),
          StringUtility.EmptyToNull (iconInfo.Height.ToString()),
          StringUtility.EmptyToNull (iconInfo.Width.ToString()));
    }

    private IconProxy (string url, string alternateText, string toolTip, string height, string width)
    {
      _url = url;
      _alternateText = alternateText;
      _toolTip = toolTip;
      _height = height;
      _width = width;
    }

    [Obsolete ("Default ctor for ASMX WSDL page.", true)]
    private IconProxy ()
    {
    }

    public string Url
    {
      get { return _url; }
    }

    public string AlternateText
    {
      get { return _alternateText; }
    }

    public string ToolTip
    {
      get { return _toolTip; }
    }

    public string Height
    {
      get { return _height; }
    }

    public string Width
    {
      get { return _width; }
    }
  }
}