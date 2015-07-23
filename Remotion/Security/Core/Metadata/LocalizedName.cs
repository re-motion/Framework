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
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class LocalizedName
  {
    private string _referencedObjectID;
    private string _comment;
    private string _text;

    public LocalizedName (string referencedObjectID, string comment, string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("referencedObjectID", referencedObjectID);
      ArgumentUtility.CheckNotNullOrEmpty ("comment", comment);
      ArgumentUtility.CheckNotNull ("text", text);

      _referencedObjectID = referencedObjectID;
      _comment = comment;
      _text = text;
    }

    public string Text
    {
      get { return _text; }
    }

    public string Comment
    {
      get { return _comment; }
    }

    public string ReferencedObjectID
    {
      get { return _referencedObjectID; }
    }

    public override bool Equals (object obj)
    {
      LocalizedName otherName = obj as LocalizedName;
      if (otherName == null)
        return false;

      return otherName._comment.Equals (_comment) && otherName._referencedObjectID.Equals (_referencedObjectID) && otherName._text.Equals (_text);
    }

    public override int GetHashCode ()
    {
      return _comment.GetHashCode () ^ _referencedObjectID.GetHashCode () ^ _text.GetHashCode ();
    }
  }
}
