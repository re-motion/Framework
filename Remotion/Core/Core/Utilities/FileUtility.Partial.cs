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
using System.Reflection;
using Remotion.Reflection;

namespace Remotion.Utilities
{
  public static partial class FileUtility
  {
    /// <summary>
    /// Writes a string resource embedded in an assemby into a file with the passed filename.
    /// </summary>
    /// <param name="typeWhoseNamespaceTheStringResourceResidesIn"><see cref="Type"/> in whose assembly and namespace the string resource is located.</param>
    /// <param name="stringResourceName">Name of the string resource, relative to namespace of the passed <see cref="Type"/>.</param>
    /// <param name="filePath">The path of the file the string resource will be written into.</param>
    public static void WriteEmbeddedStringResourceToFile (
        Type typeWhoseNamespaceTheStringResourceResidesIn,
        string stringResourceName,
        string filePath)
    {
      ArgumentUtility.CheckNotNull("typeWhoseNamespaceTheStringResourceResidesIn", typeWhoseNamespaceTheStringResourceResidesIn);
      ArgumentUtility.CheckNotNull("stringResourceName", stringResourceName);
      ArgumentUtility.CheckNotNull("filePath", filePath);
      Assembly assembly = typeWhoseNamespaceTheStringResourceResidesIn.Assembly;
      using (
          Stream from = assembly.GetManifestResourceStream(typeWhoseNamespaceTheStringResourceResidesIn, stringResourceName)!,
              to = new FileStream(filePath, FileMode.Create))
      {
        Assertion.IsNotNull(from, "Resource '{0}' does not exist in assembly '{1}'.", stringResourceName, assembly.GetFullNameSafe());
        from.CopyTo(to);
      }
    }
  }
}
