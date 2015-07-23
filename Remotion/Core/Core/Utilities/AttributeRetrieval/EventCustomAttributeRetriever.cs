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

namespace Remotion.Utilities.AttributeRetrieval
{
  // Note: This class is currently only tested integratively, via AttributeUtility. When changing it, consider adding tests specifically for this 
  // class.
  /// <summary>
  /// Implements <see cref="InheritanceAwareCustomAttributeRetriever{TCustomAttributeProvider}"/> for <see cref="EventInfo"/> objects.
  /// </summary>
  public sealed class EventCustomAttributeRetriever : InheritanceAwareCustomAttributeRetriever<EventInfo>
  {
    protected override EventInfo GetBaseMember (EventInfo memberInfo)
    {
      var accessorMethod = memberInfo.GetAddMethod (true);
      Assertion.DebugAssert (accessorMethod != null, "A Event must have an add accessor.");

      var baseAccessor = GetBaseMethod (accessorMethod);
      if (baseAccessor == null)
        return null;

      // Note: We're ignoring the case where a base event has a different name than the overriding event - that can't be implemented in C#/VB anyway.
      Assertion.DebugAssert (baseAccessor.DeclaringType != null, "Global methods canot be overridden.");
      return baseAccessor.DeclaringType.GetEvent (memberInfo.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
    }
  }
}