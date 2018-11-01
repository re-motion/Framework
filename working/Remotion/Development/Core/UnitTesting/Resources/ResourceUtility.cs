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
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Resources
{
  public static class ResourceUtility
  {
    public static Stream GetResourceStream (Assembly assembly, string resourceID)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNullOrEmpty ("resourceID", resourceID);

      var resourceStream = assembly.GetManifestResourceStream (resourceID);
      if (resourceStream == null)
        throw new ResourceNotFoundException (string.Format ("Resource '{0}' in assembly '{1}' could not be found.", resourceID, assembly.FullName));
      return resourceStream;
    }

    public static Stream GetResourceStream (Type namespaceProvider, string shortResourceName)
    {
      var resourceStream = namespaceProvider.Assembly.GetManifestResourceStream (namespaceProvider, shortResourceName);
      if (resourceStream == null)
      {
        var message = string.Format (
            "Resource '{0}.{1}' in assembly '{2}' could not be found.", namespaceProvider.Namespace, shortResourceName, namespaceProvider);
        throw new ResourceNotFoundException (message);
      }
      return resourceStream;
    }

    public static byte[] GetResource (Assembly assembly, string resourceID)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNullOrEmpty ("resourceID", resourceID);

      using (var resourceStream = GetResourceStream (assembly, resourceID))
      {
        return GetBytes (resourceStream);
      }
    }

    public static byte[] GetResource (Type namespaceProvider, string shortResourceName)
    {
      ArgumentUtility.CheckNotNull ("namespaceProvider", namespaceProvider);
      ArgumentUtility.CheckNotNullOrEmpty ("shortResourceName", shortResourceName);


      using (var resourceStream = GetResourceStream (namespaceProvider, shortResourceName))
      {
        return GetBytes (resourceStream);
      }
    }

    public static string GetResourceString (Assembly assembly, string resourceID)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNullOrEmpty ("resourceID", resourceID);

      using (var resourceStream = GetResourceStream (assembly, resourceID))
      {
        using (var streamReader = new StreamReader (resourceStream))
        {
          return streamReader.ReadToEnd();
        }
      }
    }

    public static string GetResourceString (Type namespaceProvider, string shortResourceName)
    {
      ArgumentUtility.CheckNotNull ("namespaceProvider", namespaceProvider);
      ArgumentUtility.CheckNotNullOrEmpty ("shortResourceName", shortResourceName);

      using (var resourceStream = GetResourceStream (namespaceProvider, shortResourceName))
      {
        return GetString (resourceStream);
      }
    }

    private static byte[] GetBytes (Stream resourceStream)
    {
      using (var binaryReader = new BinaryReader (resourceStream))
      {
        return binaryReader.ReadBytes ((int) resourceStream.Length);
      }
    }

    private static string GetString (Stream resourceStream)
    {
      using (var streamReader = new StreamReader (resourceStream))
      {
        return streamReader.ReadToEnd();
      }
    }
  }
}