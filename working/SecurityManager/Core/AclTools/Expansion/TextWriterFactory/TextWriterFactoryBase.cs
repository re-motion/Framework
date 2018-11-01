// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.IO;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory
{
  public abstract class TextWriterFactoryBase : ITextWriterFactory
  {
    private readonly Dictionary<string, TextWriterData> _nameToTextWriterData = new Dictionary<string, TextWriterData> (); 

    public abstract TextWriter CreateTextWriter (string directory, string name, string extension);
    public abstract TextWriter CreateTextWriter (string name);
    public string Directory { get; set; }
    public string Extension { get; set; }

    public Dictionary<string, TextWriterData> NameToTextWriterData
    {
      get { return _nameToTextWriterData; }
    }


    public static string AppendExtension(string name, string extension)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      if (String.IsNullOrEmpty (extension))
      {
        return name;
      }
      else
      {
        return name + "." + extension;
      }
    }

    public virtual string GetRelativePath (string fromName, string toName)
    {
      ArgumentUtility.CheckNotNull ("fromName", fromName);
      ArgumentUtility.CheckNotNull ("toName", toName);

      if (!TextWriterExists(toName))
        throw new ArgumentException (string.Format ("No TextWriter with name \"{0}\" registered => no relative path exists.", toName));
      return Path.Combine(".", AppendExtension (toName, Extension)); 
    }

    public virtual bool TextWriterExists (string toName)
    {
      return _nameToTextWriterData.ContainsKey (toName);
    }

    public TextWriterData GetTextWriterData (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      return _nameToTextWriterData[name];
    }

    public int Count { get { return _nameToTextWriterData.Count; } }

  }
}
