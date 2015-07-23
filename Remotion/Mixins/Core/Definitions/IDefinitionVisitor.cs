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

namespace Remotion.Mixins.Definitions
{
  public interface IDefinitionVisitor
  {
    void Visit (TargetClassDefinition targetClass);
    void Visit (MixinDefinition mixin);
    void Visit (InterfaceIntroductionDefinition interfaceIntroduction);
    void Visit (NonInterfaceIntroductionDefinition nonIntroductionDefinition);
    void Visit (MethodIntroductionDefinition methodIntroduction);
    void Visit (PropertyIntroductionDefinition propertyIntroduction);
    void Visit (EventIntroductionDefinition eventIntroduction);
    void Visit (MethodDefinition method);
    void Visit (PropertyDefinition property);
    void Visit (EventDefinition eventDefintion);
    void Visit (RequiredTargetCallTypeDefinition requiredTargetCallType);
    void Visit (RequiredNextCallTypeDefinition requiredNextCallType);
    void Visit (RequiredMixinTypeDefinition requiredMixinType);
    void Visit (RequiredMethodDefinition definition);
    void Visit (TargetCallDependencyDefinition targetCallDependency);
    void Visit (NextCallDependencyDefinition nextCallDependency);
    void Visit (MixinDependencyDefinition mixinDependency);
    void Visit (ComposedInterfaceDependencyDefinition dependency);
    void Visit (AttributeDefinition attribute);
    void Visit (AttributeIntroductionDefinition attributeIntroduction);
    void Visit (NonAttributeIntroductionDefinition nonAttributeIntroduction);
    void Visit (SuppressedAttributeIntroductionDefinition suppressedAttributeIntroduction);
  }
}
