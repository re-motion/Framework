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
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.TestSite.Infrastructure;

namespace Remotion.Web.Development.WebTesting.TestSite.Shared
{
  /// <summary>
  /// Represents options that are passed to <see cref="IGenericTestPage{TOptions}"/>.
  /// </summary>
  public class GenericTestOptions
  {
    private readonly string _id;
    private readonly string _textContent;
    private readonly string _title;
    private readonly bool _enabled;

    public GenericTestOptions ([NotNull] string id, [NotNull] string textContent, [NotNull] string title, bool enabled)
    {
      ArgumentUtility.CheckNotNull("id", id);
      ArgumentUtility.CheckNotNull("textContent", textContent);
      ArgumentUtility.CheckNotNull("title", title);

      _id = id;
      _textContent = textContent;
      _title = title;
      _enabled = enabled;
    }

    public string ID
    {
      get { return _id; }
    }

    public string TextContent
    {
      get { return _textContent; }
    }

    public string Title
    {
      get { return _title; }
    }

    public bool Enabled
    {
      get { return _enabled; }
    }
  }
}
