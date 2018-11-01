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
  /// Represents the dependency on an interface added to a target class via a composed interface specification.
  /// </summary>
  public class ComposedInterfaceDependencyDefinition : DependencyDefinitionBase
  {
    private readonly Type _composedInterface;

    public ComposedInterfaceDependencyDefinition (RequiredTargetCallTypeDefinition requirement, Type composedInterface, DependencyDefinitionBase aggregator)
        : base (requirement, aggregator)
    {
      ArgumentUtility.CheckNotNull ("composedInterface", composedInterface);
      _composedInterface = composedInterface;
    }

    public Type ComposedInterface
    {
      get { return _composedInterface; }
    }

    public new RequiredTargetCallTypeDefinition RequiredType
    {
      get { return (RequiredTargetCallTypeDefinition) base.RequiredType; }
    }

    public override IVisitableDefinition Depender
    {
      get { return TargetClass; }
    }

    public override string GetDependencyDescription ()
    {
      return string.Format ("composed interface '{0}'", _composedInterface);
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}