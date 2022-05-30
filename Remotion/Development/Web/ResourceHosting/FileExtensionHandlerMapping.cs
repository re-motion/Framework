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

namespace Remotion.Development.Web.ResourceHosting
{
  /// <summary>
  /// used for mapping file name extensions to asp.net handlers
  /// </summary>
  public class FileExtensionHandlerMapping
  {
    public static FileExtensionHandlerMapping[] Default
    {
      get
      {
        return new[]
               {
                   new FileExtensionHandlerMapping("jpg", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping("jpeg", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping("png", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping("gif", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping("svg", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping("js", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping("map", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping("css", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping("ttf", ResourceVirtualPathProvider.StaticFileHandler),
               };
      }
    }

    private readonly string _extension;
    private readonly IHttpHandler _handler;

    public FileExtensionHandlerMapping (string extension, IHttpHandler handler)
    {
      ArgumentUtility.CheckNotNull("extension", extension);
      ArgumentUtility.CheckNotNull("handler", handler);

      _extension = extension;
      _handler = handler;
    }

    public string Extension
    {
      get { return _extension; }
    }

    public IHttpHandler Handler
    {
      get { return _handler; }
    }
  }
}
