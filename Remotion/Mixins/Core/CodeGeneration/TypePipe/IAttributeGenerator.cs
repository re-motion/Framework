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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// Adds custom <see cref="Attribute"/>s to <see cref="IMutableMember"/>s.
  /// </summary>
  public interface IAttributeGenerator
  {
    void AddDebuggerBrowsableAttribute (IMutableMember member, DebuggerBrowsableState debuggerBrowsableState);

    void AddDebuggerDisplayAttribute (IMutableMember member, string debuggerDisplayString, string debuggerDisplayNameStringOrNull);

    void AddIntroducedMemberAttribute (IMutableMember member, MemberInfo interfaceMember, MemberDefinitionBase implementingMember);

    void AddConcreteMixedTypeAttribute (IMutableMember member, ClassContext classContext, IEnumerable<Type> orderedMixinTypes);

    void AddConcreteMixinTypeAttribute (IMutableMember member, ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier);

    void AddOverrideInterfaceMappingAttribute (IMutableMember member, MethodInfo overriddenMethod);

    void AddGeneratedMethodWrapperAttribute (IMutableMember member, MethodInfo methodToBeWrapped);

    void AddAttribute (IMutableMember member, ICustomAttributeData attributeData);

    void ReplicateAttributes (IAttributableDefinition source, IMutableMember destination);

    bool ShouldBeReplicated (
        AttributeDefinition attribute, IAttributeIntroductionTarget targetConfiguration, TargetClassDefinition targetClassDefinition);
  }
}