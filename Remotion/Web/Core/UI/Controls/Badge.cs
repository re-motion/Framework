﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Represents a small amount of information that is displayed besides its corresponding parent element.
  /// </summary>
  [TypeConverter(typeof(BadgeConverter))]
  public class Badge
  {
    public static bool ShouldSerialize (Badge? badge)
    {
      if (badge == null)
        return false;

      if (badge.Value.IsEmpty && badge.Description.IsEmpty)
        return false;

      return true;
    }

    private WebString _value;
    private PlainTextString _description;

    public Badge ()
        : this(WebString.Empty, PlainTextString.Empty)
    {
    }

    public Badge (WebString value, PlainTextString description)
    {
      Value = value;
      Description = description;
    }

    /// <summary>
    /// Gets or sets the text that is displayed in the badge.
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [DefaultValue(typeof(WebString), "")]
    [NotifyParentProperty(true)]
    public WebString Value
    {
      get { return _value; }
      [MemberNotNull(nameof(_value))]
      set { _value = value; }
    }

    /// <summary>
    /// Gets or sets a screen reader friendly description of <see cref="Value"/>.
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [DefaultValue(typeof(PlainTextString), "")]
    [NotifyParentProperty(true)]
    public PlainTextString Description
    {
      get { return _description; }
      [MemberNotNull(nameof(_description))]
      set { _description = value; }
    }

    public override string ToString ()
    {
      return _value.ToString();
    }
  }
}
