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
using System.Reflection;
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;
using ReflectionUtility = Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.Definitions.Building
{
  public class MixinDefinitionBuilder
  {
    private readonly TargetClassDefinition _targetClass;
    private readonly RequirementsAnalyzer _targetRequirementsAnalyzer; 
    private readonly RequirementsAnalyzer _nextRequirementsAnalyzer;

    public MixinDefinitionBuilder (TargetClassDefinition targetClass)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);
      _targetClass = targetClass;
      _targetRequirementsAnalyzer = new RequirementsAnalyzer (MixinGenericArgumentFinder.TargetArgumentFinder);
      _nextRequirementsAnalyzer = new RequirementsAnalyzer (MixinGenericArgumentFinder.NextArgumentFinder);
    }

    public TargetClassDefinition TargetClass
    {
      get { return _targetClass; }
    }

    public void Apply (MixinContext mixinContext, int index)
    {
      ArgumentUtility.CheckNotNull ("mixinContext", mixinContext);
      ArgumentUtility.CheckNotNull ("index", index);

      MixinDefinition mixin = CreateMixinDefinition(mixinContext);

      AnalyzeMembers(mixin);
      AnalyzeAttributes(mixin);
      AnalyzeInterfaceIntroductions (mixin, mixinContext.IntroducedMemberVisibility);
      AnalyzeOverrides (mixin);
      AnalyzeDependencies(mixin, mixinContext.ExplicitDependencies);
    }

    private MixinDefinition CreateMixinDefinition (MixinContext mixinContext)
    {
      Type mixinType = TargetClass.MixinTypeCloser.GetClosedMixinType (mixinContext.MixinType);
      bool acceptsAlphabeticOrdering = AcceptsAlphabeticOrdering (mixinType);
      var mixin = new MixinDefinition (mixinContext.MixinKind, mixinType, TargetClass, acceptsAlphabeticOrdering);
      TargetClass.Mixins.Add (mixin);
      return mixin;
    }

    private bool AcceptsAlphabeticOrdering (Type mixinType)
    {
      return mixinType.IsDefined (typeof (AcceptsAlphabeticOrderingAttribute), false);
    }

    private void AnalyzeMembers (MixinDefinition mixin)
    {
      var membersBuilder = new MemberDefinitionBuilder (mixin, IsVisibleToInheritorsOrExplicitInterfaceImpl);
      membersBuilder.Apply (mixin.Type);
    }

    private bool IsVisibleToInheritorsOrExplicitInterfaceImpl (MethodInfo method)
    {
      return ReflectionUtility.IsPublicOrProtectedOrExplicit (method);
    }

    private void AnalyzeAttributes (MixinDefinition mixin)
    {
      var attributesBuilder = new AttributeDefinitionBuilder (mixin);
      attributesBuilder.Apply (mixin.Type);
    }

    private void AnalyzeInterfaceIntroductions (MixinDefinition mixin, MemberVisibility defaultVisibility)
    {
      var introductionBuilder = new InterfaceIntroductionDefinitionBuilder (mixin, defaultVisibility);
      introductionBuilder.Apply ();
    }

    private void AnalyzeOverrides (MixinDefinition mixin)
    {
      var methodAnalyzer = new OverridesAnalyzer<MethodDefinition> (typeof (OverrideTargetAttribute), _targetClass.Methods);
      foreach (var methodOverride in methodAnalyzer.Analyze (mixin.Methods))
        InitializeOverride (methodOverride.Overrider, methodOverride.BaseMember);

      var propertyAnalyzer = new OverridesAnalyzer<PropertyDefinition> (typeof (OverrideTargetAttribute), _targetClass.Properties);
      foreach (var propertyOverride in propertyAnalyzer.Analyze (mixin.Properties))
        InitializeOverride (propertyOverride.Overrider, propertyOverride.BaseMember);

      var eventAnalyzer = new OverridesAnalyzer<EventDefinition> (typeof (OverrideTargetAttribute), _targetClass.Events);
      foreach (var eventOverride in eventAnalyzer.Analyze (mixin.Events))
        InitializeOverride (eventOverride.Overrider, eventOverride.BaseMember);
    }

    private void InitializeOverride (MemberDefinitionBase overrider, MemberDefinitionBase baseMember)
    {
      overrider.BaseAsMember = baseMember;
      if (baseMember.Overrides.ContainsKey (overrider.DeclaringClass.Type))
      {
        string message = string.Format ("Mixin {0} overrides method {1} twice: {2} and {3} both target the same method.",
            overrider.DeclaringClass.FullName, baseMember.FullName, overrider.FullName, baseMember.Overrides[overrider.DeclaringClass.Type].FullName);
        throw new ConfigurationException (message);
      }
      baseMember.AddOverride (overrider);
    }
    
    private void AnalyzeDependencies (MixinDefinition mixin, IEnumerable<Type> additionalDependencies)
    {
      var targetCallDependencyBuilder = new TargetCallDependencyDefinitionBuilder (mixin);
      targetCallDependencyBuilder.Apply (_targetRequirementsAnalyzer.GetRequirements (mixin.Type));

      var nextCallDependencyBuilder = new NextCallDependencyDefinitionBuilder (mixin);
      nextCallDependencyBuilder.Apply (_nextRequirementsAnalyzer.GetRequirements (mixin.Type));
      
      var mixinDependencyBuilder = new MixinDependencyDefinitionBuilder (mixin);
      mixinDependencyBuilder.Apply (additionalDependencies);
    }
  }
}
