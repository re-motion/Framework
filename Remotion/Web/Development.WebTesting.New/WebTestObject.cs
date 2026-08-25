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
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Remotion.Web.Development.WebTesting
{
  public abstract class WebTestObject<TWebTestObjectContext>
      where TWebTestObjectContext : WebTestObjectContext
  {
    private readonly ILogger _logger;
    private readonly TWebTestObjectContext _context;

    protected WebTestObject ([NotNull] TWebTestObjectContext context)
    {
      ArgumentNullException.ThrowIfNull(context);

      _logger = context.LoggerFactory.CreateLogger(GetType());
      _context = context;
    }

    /// <summary>
    /// The <see cref="ILogger"/> associated with this <see cref="WebTestObject{TWebTestObjectContext}"/>'s type.
    /// </summary>
    public ILogger Logger
    {
      get { return _logger; }
    }

    /// <summary>
    /// The web test object's <see cref="WebTestObjectContext"/>.
    /// </summary>
    public TWebTestObjectContext Context
    {
      get { return _context; }
    }

    /// <summary>
    /// Shortcut for <see cref="Context"/>.<see cref="WebTestObjectContext.Scope"/>.
    /// </summary>
    public ElementScope Scope
    {
      get { return Context.Scope; }
    }
  }
}
