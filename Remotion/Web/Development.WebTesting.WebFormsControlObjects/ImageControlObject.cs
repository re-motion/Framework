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

namespace Remotion.Web.Development.WebTesting.WebFormsControlObjects
{
  /// <summary>
  /// Control object representing simple HTML &lt;img&gt; tags.
  /// </summary>
  public class ImageControlObject : WebFormsControlObject
  {
    public ImageControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Returns the src URL of the image.
    /// </summary>
    /// <returns>Returns the src URL of the image or <see langword="null" /> if not set.</returns>
    public string GetSourceUrl ()
    {
      return Scope["src"];
    }

    /// <summary>
    /// Returns the alternative text of the image.
    /// </summary>
    /// <returns>
    /// Returns the alterantive text of the image or an empty string if not set (yep, unfortunately we cannot distinguish between "not set" and "set
    /// to empty string").</returns>
    public string GetAltText ()
    {
      return Scope["alt"];
    }
  }
}
