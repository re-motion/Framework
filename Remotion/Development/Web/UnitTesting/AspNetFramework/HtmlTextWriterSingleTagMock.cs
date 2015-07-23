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
using System.Collections;
using System.IO;
using System.Web.UI;

namespace Remotion.Development.Web.UnitTesting.AspNetFramework
{
  public class HtmlTextWriterSingleTagMock: HtmlTextWriter
  {
    private Hashtable _attributes = new Hashtable();
    private HtmlTextWriterTag _tag;

    public HtmlTextWriterSingleTagMock()
        : base (new StringWriter ())
    {
    }

    public Hashtable Attributes
    {
      get { return _attributes; }
    }

    public HtmlTextWriterTag Tag
    {
      get { return _tag; }
    }

    public override void AddAttribute (HtmlTextWriterAttribute key, string value, bool fEncode)
    {
      base.AddAttribute (key, value, fEncode);
      _attributes[key] = value;
    }

    public override void AddAttribute (HtmlTextWriterAttribute key, string value)
    {
      base.AddAttribute (key, value);
      _attributes[key] = value;
    }

    protected override void AddAttribute (string name, string value, HtmlTextWriterAttribute key)
    {
      base.AddAttribute (name, value, key);
      _attributes[key] = value;
    }

    public override void RenderBeginTag (HtmlTextWriterTag tagKey)
    {
      base.RenderBeginTag (tagKey);
      _tag = tagKey;
    }
  }
}
