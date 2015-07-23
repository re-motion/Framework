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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Provides functionality to resolve type name templates to actual types. Type name templates are assembly-qualified type names that contain
  /// "&lt;version&gt;" and "&lt;publicKeyToken&gt;" as placeholders for version and public key token. Those placeholders will be replaced with
  /// the version and public key token of a given reference <see cref="Assembly"/>, then <see cref="TypeUtility.GetType(string, bool)"/> is used to resolve the type.
  /// </summary>
  [Obsolete ("Resolving the type via the assembly qualified typename is no longer required. (Version 1.15.10.0")]
  public static class TypeNameTemplateResolver
  {
    public static Type ResolveToType (string typeNameTemplate, Assembly referenceAssembly)
    {
      return TypeUtility.GetType (ResolveToTypeName (typeNameTemplate, referenceAssembly), true);
    }

    public static string ResolveToTypeName (string typeNameTemplate, Assembly referenceAssembly)
    {
      string versioned = typeNameTemplate.Replace ("<version>", referenceAssembly.GetName().Version.ToString());
      return versioned.Replace ("<publicKeyToken>", GetPublicKeyTokenString (referenceAssembly));
    }
    
    private static string GetPublicKeyTokenString (Assembly referenceAssembly)
    {
      byte[] bytes = referenceAssembly.GetName ().GetPublicKeyToken ();
      if (bytes.Length == 0)
        return "null";

      return string.Format ("{0:x2}{1:x2}{2:x2}{3:x2}{4:x2}{5:x2}{6:x2}{7:x2}",
          bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]);
    }

  }
}