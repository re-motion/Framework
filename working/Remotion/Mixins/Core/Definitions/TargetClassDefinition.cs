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
using System.Diagnostics;
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{Type}")]
  public class TargetClassDefinition : ClassDefinitionBase, IAttributeIntroductionTarget
  {
    private readonly UniqueDefinitionCollection<Type, MixinDefinition> _mixins =
        new UniqueDefinitionCollection<Type, MixinDefinition> (m => m.Type);
    private readonly UniqueDefinitionCollection<Type, RequiredTargetCallTypeDefinition> _requiredTargetCallTypes =
        new UniqueDefinitionCollection<Type, RequiredTargetCallTypeDefinition> (t => t.Type);
    private readonly UniqueDefinitionCollection<Type, RequiredNextCallTypeDefinition> _requiredNextCallTypes =
        new UniqueDefinitionCollection<Type, RequiredNextCallTypeDefinition> (t => t.Type);
    private readonly UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> _requiredMixinTypes =
        new UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> (t => t.Type);
    
    private readonly UniqueDefinitionCollection<Type, ComposedInterfaceDependencyDefinition> _composedInterfaceDependencies =
        new UniqueDefinitionCollection<Type, ComposedInterfaceDependencyDefinition> (d => d.RequiredType.Type);
    
    private readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> _receivedInterfaces =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (i => i.InterfaceType);
    private readonly MultiDefinitionCollection<Type, AttributeIntroductionDefinition> _receivedAttributes;

    private readonly ClassContext _configurationContext;
    private readonly MixinTypeCloser _mixinTypeCloser;

    public TargetClassDefinition (ClassContext configurationContext)
        : base (configurationContext.Type)
    {
      ArgumentUtility.CheckNotNull ("configurationContext", configurationContext);

      _receivedAttributes = new MultiDefinitionCollection<Type, AttributeIntroductionDefinition> (a => a.AttributeType);

      _configurationContext = configurationContext;
      _mixinTypeCloser = new MixinTypeCloser (configurationContext.Type);
    }

    public MultiDefinitionCollection<Type, AttributeIntroductionDefinition> ReceivedAttributes
    {
      get { return _receivedAttributes; }
    }

    public ClassContext ConfigurationContext
    {
      get { return _configurationContext; }
    }

    public MixinTypeCloser MixinTypeCloser
    {
      get { return _mixinTypeCloser; }
    }

    public bool IsInterface
    {
      get { return Type.IsInterface; }
    }

    public bool IsAbstract
    {
      get { return Type.IsAbstract; }
    }

    public override IVisitableDefinition Parent
    {
      get { return null; }
    }

    public UniqueDefinitionCollection<Type, MixinDefinition> Mixins
    {
      get { return _mixins; }
    }

    public UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> ReceivedInterfaces
    {
      get { return _receivedInterfaces; }
    }

    public UniqueDefinitionCollection<Type, RequiredMixinTypeDefinition> RequiredMixinTypes
    {
      get { return _requiredMixinTypes; }
    }

    public UniqueDefinitionCollection<Type, RequiredNextCallTypeDefinition> RequiredNextCallTypes
    {
      get { return _requiredNextCallTypes; }
    }

    public UniqueDefinitionCollection<Type, RequiredTargetCallTypeDefinition> RequiredTargetCallTypes
    {
      get { return _requiredTargetCallTypes; }
    }

    public UniqueDefinitionCollection<Type, ComposedInterfaceDependencyDefinition> ComposedInterfaceDependencies
    {
      get { return _composedInterfaceDependencies; }
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);
      
      _mixins.Accept (visitor);
      _requiredTargetCallTypes.Accept (visitor);
      _requiredNextCallTypes.Accept (visitor);
      _requiredMixinTypes.Accept (visitor);
      _composedInterfaceDependencies.Accept (visitor);
    }

    public bool HasMixinWithConfiguredType(Type configuredType)
    {
      Type realType = MixinTypeCloser.GetClosedMixinType (configuredType);
      return _mixins.ContainsKey (realType);
    }

    public MixinDefinition GetMixinByConfiguredType(Type configuredType)
    {
      Type realType = MixinTypeCloser.GetClosedMixinType (configuredType);
      return _mixins[realType];
    }
  }
}
