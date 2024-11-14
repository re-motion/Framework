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
using System.Collections.Specialized;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Use the <see cref="WxePermaUrlOptions"/> to specify that the user should be provided with a perma-URL to the <see cref="WxeFunction"/> in the 
  /// browser's location bar. Use the <see cref="Null"/> value of the <see cref="WxePermaUrlOptions"/> if you do not wish to create a perma-URL for
  /// the <see cref="WxeFunction"/>.
  /// </summary>
  public sealed class WxePermaUrlOptions : INullObject
  {
    public static readonly WxePermaUrlOptions Null = new WxePermaUrlOptions(false, false, null);

    private readonly bool _usePermaUrl;
    private readonly bool _useParentPermaUrl;
    private readonly NameValueCollection? _urlParameters;

    public WxePermaUrlOptions ()
        : this(true, false, null)
    {
    }

    public WxePermaUrlOptions (bool useParentPermaUrl)
        : this(true, useParentPermaUrl, null)
    {
    }

    public WxePermaUrlOptions (bool useParentPermaUrl, NameValueCollection? urlParameters)
        : this(true, useParentPermaUrl, urlParameters)
    {
    }

    private WxePermaUrlOptions (bool usePermaUrl, bool useParentPermaUrl, NameValueCollection? urlParameters)
    {
      _usePermaUrl = usePermaUrl;
      _useParentPermaUrl = useParentPermaUrl;
      _urlParameters = urlParameters;
    }

    public bool UsePermaUrl
    {
      get { return _usePermaUrl; }
    }

    public bool UseParentPermaUrl
    {
      get { return _useParentPermaUrl; }
    }

    public NameValueCollection? UrlParameters
    {
      get { return _urlParameters; }
    }

    bool INullObject.IsNull
    {
      get { return !_usePermaUrl; }
    }
  }
}
