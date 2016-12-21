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

namespace Remotion.Validation.Mixins.Attributes
{
  /// <summary>
  /// The <see cref="ApplyWithMixinAttribute"/> can be applied to a <see cref="IComponentValidationCollector"/> 
  /// to specify the mixin <see cref="Type"/> within the class hierarchy to associate with the collector. 
  /// This can be used if the <see cref="IComponentValidationCollector.ValidatedType"/> is an interface 
  /// but the collector should only applied with one or more specific mixin types.
  /// </summary>
  /// //TODO RM-5906: sample for derived type inhieriting interface
  [AttributeUsage (AttributeTargets.Class)]
  public class ApplyWithMixinAttribute : Attribute
  {
    private readonly Type _mixinType;

    public ApplyWithMixinAttribute (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      _mixinType = mixinType;
    }

    public Type MixinType
    {
      get { return _mixinType; }
    }
  }
}