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

namespace Remotion.Mixins.Definitions
{
  /// <summary>
  /// Represents the dependency a mixin has on a type to be used for <see cref="Mixin{TTarget}.Target"/> calls.
  /// </summary>
  public class TargetCallDependencyDefinition : DependencyDefinitionBase
  {
    private readonly MixinDefinition _dependingMixin;

    public TargetCallDependencyDefinition (RequiredTargetCallTypeDefinition requiredType, MixinDefinition dependingMixin, TargetCallDependencyDefinition aggregator)
      : base (requiredType, aggregator)
    {
      ArgumentUtility.CheckNotNull ("dependingMixin", dependingMixin);
      _dependingMixin = dependingMixin;
    }

    public new RequiredTargetCallTypeDefinition RequiredType
    {
      get { return (RequiredTargetCallTypeDefinition) base.RequiredType; }
    }

    public override IVisitableDefinition Depender
    {
      get { return _dependingMixin; }
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public override string GetDependencyDescription ()
    {
      return string.Format ("mixin '{0}'", _dependingMixin.FullName);
    }

  }
}
