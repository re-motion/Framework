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
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Resolves a method from the given parameters.
  /// </summary>
  public static class MethodResolver
  {
    /// <summary>
    /// Resolves the method with the given name and signature. Cannot resolve closed generic methods.
    /// </summary>
    /// <param name="declaringType">Declaring type of the method to be resolved.</param>
    /// <param name="name">The simple name of the method to be resolved.</param>
    /// <param name="signature">The signature string of the method to be resolved. This has the same format as what is produced by the 
    /// <see cref="object.ToString"/> method implementation of <see cref="MethodInfo"/> objects.</param>
    /// <returns>The resolved method.</returns>
    /// <exception cref="MissingMethodException">No matching method could be found.</exception>
    /// <remarks>
    /// This mimics the behavior used by Reflection to serialize MethodInfos. It's not performant at all, but it should work reliably.
    /// </remarks>
    public static MethodInfo ResolveMethod (Type declaringType, string name, string signature)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNullOrEmpty ("signature", signature);

      const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
      var candidates = (MethodInfo[]) declaringType.GetMember (name, MemberTypes.Method, flags);
      if (candidates.Length == 1)
      {
        return candidates[0];
      }
      else
      {
        var method = (from c in candidates where c.ToString () == signature select c).SingleOrDefault();
        if (method == null)
        {
          var message = string.Format ("The method '{0}' could not be found on type '{1}'.", signature, declaringType);
          throw new MissingMethodException (message);
        }
        else
        {
          return method;
        }
      }
    }
  }
}
