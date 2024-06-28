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
using System.Linq;
using System.Reflection;
using System.Text;
using Remotion.Mixins.Context;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.Utilities;
using ReflectionUtility = Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.CrossReferencer
{
  public class InvolvedTypeFinder : IInvolvedTypeFinder
  {
    private readonly MixinConfiguration _mixinConfiguration;
    private readonly Assembly[] _assemblies;
    private readonly ErrorAggregator<ConfigurationException> _configurationErrors;
    private readonly ErrorAggregator<ValidationException> _validationErrors;

    public InvolvedTypeFinder (
        MixinConfiguration mixinConfiguration,
        Assembly[] assemblies,
        ErrorAggregator<ConfigurationException> configurationErrors,
        ErrorAggregator<ValidationException> validationErrors
    )
    {
      ArgumentUtility.CheckNotNull("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull("assemblies", assemblies);
      ArgumentUtility.CheckNotNull("configurationErrors", configurationErrors);
      ArgumentUtility.CheckNotNull("validationErrors", validationErrors);

      _mixinConfiguration = mixinConfiguration;
      _assemblies = assemblies;
      _configurationErrors = configurationErrors;
      _validationErrors = validationErrors;
    }

    public InvolvedType[] FindInvolvedTypes ()
    {
      var involvedTypes = new InvolvedTypeStore();
      var classContexts = _mixinConfiguration.ClassContexts;

      foreach (var assembly in _assemblies)
      {
        try
        {
          foreach (var type in assembly.GetTypes())
          {
            var classContext = classContexts.GetWithInheritance(type);
            if (classContext != null)
            {
              var involvedType = involvedTypes.GetOrCreateValue(type);
              var targetClassDefinition = GetTargetClassDefinition(type, classContext);
              involvedType.ClassContext = classContext;
              involvedType.TargetClassDefinition = targetClassDefinition;

              foreach (var mixinContext in classContext.Mixins)
              {
                var mixinType = mixinContext.MixinType;
                var mixin = involvedTypes.GetOrCreateValue(mixinType);
                mixin.TargetTypes.Add(involvedType, GetMixinDefiniton(mixinType, targetClassDefinition));
              }
            }

            // also add classes which inherit from Mixin<> or Mixin<,>, but are actually not used as Mixins (not in ClassContexts)
            if (ReflectionUtility.IsMixinType(type) && !CrossReferencerReflectionUtility.IsInfrastructureType(type))
              involvedTypes.GetOrCreateValue(type);
          }
        }
        catch (ReflectionTypeLoadException ex)
        {
          var loaderExceptionLog = new StringBuilder();
          foreach (var loaderException in ex.LoaderExceptions)
            loaderExceptionLog.AppendFormat("   {1}{0}", Environment.NewLine, loaderException.Message);

          // XRef.Log.SendWarning(
          //     "Unable to analyze '{1}' in '{2}' because some referenced assemblies could not be loaded: {0}{3}",
          //     Environment.NewLine,
          //     assembly.FullName,
          //     assembly.Location,
          //     loaderExceptionLog);
        }
      }

      var additionalTypesCollector = new AdditionalTypesCollector();

      foreach (IVisitableInvolved involvedType in involvedTypes)
        involvedType.Accept(additionalTypesCollector);

      foreach (var additionalType in additionalTypesCollector.AdditionalTypes)
        involvedTypes.GetOrCreateValue(additionalType);

      AddGenericDefinitionsRecursively(involvedTypes);

      return involvedTypes.ToArray();
    }

    private void AddGenericDefinitionsRecursively (InvolvedTypeStore involvedTypes)
    {
      foreach (var type in involvedTypes.Where(t => t.Type.IsGenericType))
      {
        var genericType = type.Type;
        while (genericType.IsGenericType && genericType.GetGenericTypeDefinition() != genericType)
        {
          var genericTypeDefinition = genericType.GetGenericTypeDefinition();
          genericType = involvedTypes.GetOrCreateValue(genericTypeDefinition).Type;
        }
      }
    }

    private MixinDefinition GetMixinDefiniton (Type mixinType, TargetClassDefinition targetClassDefinition)
    {
      return targetClassDefinition == null ? null : targetClassDefinition.GetMixinByConfiguredType(mixinType);
    }

    public TargetClassDefinition GetTargetClassDefinition (Type type, ClassContext classContext)
    {
      if (type.IsGenericTypeDefinition || type.IsInterface)
        return null;

      try
      {
        // may throw ConfigurationException or ValidationException
        return TargetClassDefinitionFactory.CreateAndValidate(classContext);
      }
      catch (ConfigurationException e)
      {
        _configurationErrors.AddException(e);
        return null; // MixinConfiguration is not valid
      }
      catch (ValidationException e)
      {
        _validationErrors.AddException(e);
        return null; // MixinConfiguration is not valid
      }
    }
  }
}
