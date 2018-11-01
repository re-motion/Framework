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

namespace Remotion.Mixins
{
  /// <summary>
  /// When applied to a mixin, specifies that this mixin does not introduce a specific interface or attribute to the target class.
  /// </summary>
  /// <remarks>Use this attribute if a mixin should implement an interface or declare an attribute "just for itself" and the interface should not be
  /// forwarded to the target class.</remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public class NonIntroducedAttribute : Attribute
  {
    private readonly Type _nonIntroducedType;

    public NonIntroducedAttribute (Type nonIntroducedType)
    {
      _nonIntroducedType = ArgumentUtility.CheckNotNull ("nonIntroducedType", nonIntroducedType);
    }

    public Type NonIntroducedType
    {
      get { return _nonIntroducedType; }
    }
  }
}
