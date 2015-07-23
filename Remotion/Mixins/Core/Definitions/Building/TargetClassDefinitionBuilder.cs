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
using System.Linq;
using Remotion.FunctionalProgramming;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using ReflectionUtility = Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.Definitions.Building
{
  /// <summary>
  /// Builds <see cref="TargetClassDefinition"/> objects containing all metadata required for code generation from a <see cref="ClassContext"/>.
  /// </summary>
  [ImplementationFor (typeof (ITargetClassDefinitionBuilder), Lifetime = LifetimeKind.Singleton)]
  public class TargetClassDefinitionBuilder : ITargetClassDefinitionBuilder
  {
    private readonly IMixinDefinitionSorter _mixinSorter;

    public TargetClassDefinitionBuilder (IMixinDefinitionSorter mixinSorter)
    {
      ArgumentUtility.CheckNotNull ("mixinSorter", mixinSorter);
      _mixinSorter = mixinSorter;
    }

    public IMixinDefinitionSorter MixinSorter
    {
      get { return _mixinSorter; }
    }

    public TargetClassDefinition Build (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      if (classContext.Type.ContainsGenericParameters)
      {
        string message = string.Format ("The base class {0} contains generic parameters. This is not supported.", classContext.Type.FullName);
        throw new ConfigurationException (message);
      }

      var classDefinition = new TargetClassDefinition (classContext);

      var membersBuilder = new MemberDefinitionBuilder (classDefinition, ReflectionUtility.IsPublicOrProtectedOrExplicit);
      membersBuilder.Apply (classDefinition.Type);

      var attributesBuilder = new AttributeDefinitionBuilder (classDefinition);
      attributesBuilder.Apply (classDefinition.Type);

      ApplyComposedInterfaces (classDefinition, classContext);
      ApplyMixins (classDefinition, classContext);
      ApplyMethodRequirements (classDefinition);

      AnalyzeOverrides (classDefinition);
      AnalyzeAttributeIntroductions (classDefinition);
      AnalyzeMemberAttributeIntroductions (classDefinition);
      return classDefinition;
    }

    private void ApplyComposedInterfaces (TargetClassDefinition classDefinition, ClassContext classContext)
    {
      foreach (Type composedInterface in classContext.ComposedInterfaces)
      {
        var composedInterfaceDependencyDefinitionBuilder = new ComposedInterfaceDependencyDefinitionBuilder (classDefinition, composedInterface);
        // Apply recursively creates aggregated dependencies for interfaces implemented by composedInterface
        composedInterfaceDependencyDefinitionBuilder.Apply (new[] { composedInterface });
      }
    }

    private void ApplyMixins (TargetClassDefinition classDefinition, ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      // The IMixinDefinitionSorter requires that the mixins have already been added to the class (and that the dependencoes have been set up
      // correctly). Therefore, we add all the mixins, then sort them, then re-add them in the correct order.

      var mixinDefinitionBuilder = new MixinDefinitionBuilder (classDefinition);
      foreach (var mixinContext in classContext.Mixins)
          mixinDefinitionBuilder.Apply (mixinContext, -1);

      var sortedMixins = SortMixins(classDefinition);

      classDefinition.Mixins.Clear();
      foreach (var mixinDefinition in sortedMixins)
      {
        mixinDefinition.MixinIndex = classDefinition.Mixins.Count;
        classDefinition.Mixins.Add (mixinDefinition);
      }
    }

    private ICollection<MixinDefinition> SortMixins (TargetClassDefinition targetClassDefinition)
    {
      try
      {
        return _mixinSorter.SortMixins (targetClassDefinition.Mixins).ConvertToCollection ();
      }
      catch (InvalidOperationException ex)
      {
        string message = string.Format (
            "The mixins applied to target class '{0}' cannot be ordered. {1}",
            targetClassDefinition.FullName,
            ex.Message);
        throw new ConfigurationException (message, ex);
      }
    }

    // This can only be done once all the mixins are available, therefore, the TargetClassDefinitionBuilder has to do it.
    private void ApplyMethodRequirements (TargetClassDefinition classDefinition)
    {
      var methodRequirementBuilder = new RequiredMethodDefinitionBuilder (classDefinition);
      foreach (RequiredTargetCallTypeDefinition requirement in classDefinition.RequiredTargetCallTypes)
        methodRequirementBuilder.Apply (requirement);

      foreach (RequiredNextCallTypeDefinition requirement in classDefinition.RequiredNextCallTypes)
        methodRequirementBuilder.Apply (requirement);
    }

    private void AnalyzeOverrides (TargetClassDefinition definition)
    {
      var mixinMethods = definition.Mixins.SelectMany (m => m.Methods);
      var methodAnalyzer = new OverridesAnalyzer<MethodDefinition> (typeof (OverrideMixinAttribute), mixinMethods);
      foreach (var methodOverride in methodAnalyzer.Analyze (definition.Methods))
        InitializeOverride (methodOverride.Overrider, methodOverride.BaseMember);

      var mixinProperties = definition.Mixins.SelectMany (m => m.Properties);
      var propertyAnalyzer = new OverridesAnalyzer<PropertyDefinition> (typeof (OverrideMixinAttribute), mixinProperties);
      foreach (var propertyOverride in propertyAnalyzer.Analyze (definition.Properties))
        InitializeOverride (propertyOverride.Overrider, propertyOverride.BaseMember);

      var mixinEvents = definition.Mixins.SelectMany (m => m.Events);
      var eventAnalyzer = new OverridesAnalyzer<EventDefinition> (typeof (OverrideMixinAttribute), mixinEvents);
      foreach (var eventOverride in eventAnalyzer.Analyze (definition.Events))
        InitializeOverride (eventOverride.Overrider, eventOverride.BaseMember);
    }

    private void InitializeOverride (MemberDefinitionBase overrider, MemberDefinitionBase baseMember)
    {
      overrider.BaseAsMember = baseMember;
      baseMember.AddOverride (overrider);
    }

    private void AnalyzeAttributeIntroductions (TargetClassDefinition classDefinition)
    {
      var builder = new AttributeIntroductionDefinitionBuilder (classDefinition);

      var attributesOnMixins = from m in classDefinition.Mixins
                               from a in m.CustomAttributes
                               select a;
      var potentialSuppressors = classDefinition.CustomAttributes.Concat (attributesOnMixins);
      builder.AddPotentialSuppressors (potentialSuppressors);
      
      foreach (MixinDefinition mixin in classDefinition.Mixins)
        builder.Apply (mixin);
    }

    private void AnalyzeMemberAttributeIntroductions (TargetClassDefinition classDefinition)
    {
      // Check that SuppressAttributesAttribute cannot be applied to methods, properties, and fields.
      // As long as this holds, we don't need to deal with potential suppressors here.
      const AttributeTargets memberTargets = AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field;
      Assertion.IsTrue ((AttributeUtility.GetAttributeUsage (typeof (SuppressAttributesAttribute)).ValidOn & memberTargets) == 0, 
          "TargetClassDefinitionBuilder must be updated with AddPotentialSuppressors once SuppressAttributesAttribute supports members");

      foreach (MemberDefinitionBase member in classDefinition.GetAllMembers ())
      {
        if (member.Overrides.Count != 0)
        {
          var introductionBuilder = new AttributeIntroductionDefinitionBuilder (member);
          foreach (MemberDefinitionBase overrider in member.Overrides)
            introductionBuilder.Apply (overrider);
        }
      }
    }
  }
}

