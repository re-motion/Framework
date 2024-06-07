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
using System.IO;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory
{
  public class StreamWriterFactory : TextWriterFactoryBase
  {
    public override TextWriter CreateTextWriter (string directory, string name, string? extension)
    {
      ArgumentUtility.CheckNotNull("directory", directory);
      ArgumentUtility.CheckNotNull("name", name);

      if (!System.IO.Directory.Exists(directory))
      {
        System.IO.Directory.CreateDirectory(directory);
      }

      if (NameToTextWriterData.ContainsKey(name))
        throw new ArgumentException(string.Format("TextWriter with name \"{0}\" already exists.", name));

      string nameWithExtension = AppendExtension(name, extension);

      var textWriterData = new TextWriterData(new StreamWriter(Path.Combine(directory, nameWithExtension)), directory, extension);
      NameToTextWriterData[name] = textWriterData;
      return textWriterData.TextWriter;
    }

    public override TextWriter CreateTextWriter (string name)
    {
      ArgumentUtility.CheckNotNull("name", name);
      if (Directory == null)
        throw new InvalidOperationException("Directory must not be null. Set using \"Directory\"-property before calling \"CreateTextWriter\"");

      return CreateTextWriter(Directory, name, Extension);
    }
  }
}
