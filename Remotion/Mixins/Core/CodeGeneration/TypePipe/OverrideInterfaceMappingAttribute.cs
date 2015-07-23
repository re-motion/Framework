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
using Remotion.Reflection.CodeGeneration;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// Defines a mapping between an overridden mixin member and the member in the mixin's override interface. The attribute is applied to the members
  /// of the interface so that the mixin member can be determined when needed.
  /// </summary>
  [AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public class OverrideInterfaceMappingAttribute : MethodReferencingAttribute
  {
    public OverrideInterfaceMappingAttribute (Type declaringType, string methodName, string methodSignature)
        : base (declaringType, methodName, methodSignature)
    {
    }
  }
}
