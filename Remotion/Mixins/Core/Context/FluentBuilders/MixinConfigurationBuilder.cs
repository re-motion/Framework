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
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.FluentBuilders
{
  /// <summary>
  /// Provides a fluent interface for building <see cref="MixinConfiguration"/> objects.
  /// </summary>
  public class MixinConfigurationBuilder
  {
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<MixinConfigurationBuilder>();

    private readonly MixinConfiguration? _parentConfiguration;
    private readonly Dictionary<Type, ClassContextBuilder> _classContextBuilders = new Dictionary<Type, ClassContextBuilder>();

    public MixinConfigurationBuilder (MixinConfiguration? parentConfiguration)
    {
      _parentConfiguration = parentConfiguration;
    }

    /// <summary>
    /// Gets the parent configuration used as a base for the newly built mixin configuration.
    /// </summary>
    /// <value>The parent configuration.</value>
    public virtual MixinConfiguration? ParentConfiguration
    {
      get { return _parentConfiguration; }
    }

    /// <summary>
    /// Gets the class context builders collected so far via the fluent interfaces.
    /// </summary>
    /// <value>The class context builders collected so far.</value>
    public IEnumerable<ClassContextBuilder> ClassContextBuilders
    {
      get { return _classContextBuilders.Values; }
    }

    /// <summary>
    /// Begins configuration of a target class.
    /// </summary>
    /// <param name="targetType">The class to be configured.</param>
    /// <returns>A fluent interface object for configuring the given <paramref name="targetType"/>.</returns>
    public virtual ClassContextBuilder ForClass (Type targetType)
    {
      ArgumentUtility.CheckNotNull("targetType", targetType);
      if (!_classContextBuilders.ContainsKey(targetType))
      {
        var builder = new ClassContextBuilder(this, targetType);
        _classContextBuilders.Add(targetType, builder);
      }
      return _classContextBuilders[targetType];
    }

    /// <summary>
    /// Begins configuration of a target class.
    /// </summary>
    /// <typeparam name="TTargetType">The class to be configured.</typeparam>
    /// <returns>A fluent interface object for configuring the given <typeparamref name="TTargetType"/>.</returns>
    public virtual ClassContextBuilder ForClass<TTargetType> ()
    {
      return ForClass(typeof(TTargetType));
    }

    /// <summary>
    /// Adds the given mixin to the given target type with a number of explicit dependencies and suppressed mixins. This is a shortcut
    /// method for calling <see cref="ForClass"/>, <see cref="ClassContextBuilder.AddMixin(System.Type,Remotion.Mixins.Context.MixinContextOrigin)"/>,
    /// <see cref="MixinContextBuilder.WithDependencies"/>, and <see cref="MixinContextBuilder.ReplaceMixins"/> in a row.
    /// </summary>
    /// <param name="mixinKind">The kind of relationship the mixin has with its target class.</param>
    /// <param name="targetType">The target type to add a mixin for.</param>
    /// <param name="mixinType">The mixin type to add.</param>
    /// <param name="introducedMemberVisibility">The default visibility to be used for introduced members.</param>
    /// <param name="explicitDependencies">The explicit dependencies of the mixin in the context of the target type.</param>
    /// <param name="suppressedMixins">The mixins suppressed by this mixin in the context of the target type.</param>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    public virtual MixinConfigurationBuilder AddMixinToClass (
        MixinKind mixinKind,
        Type targetType,
        Type mixinType,
        MemberVisibility introducedMemberVisibility,
        IEnumerable<Type> explicitDependencies,
        IEnumerable<Type> suppressedMixins,
        MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull("targetType", targetType);
      ArgumentUtility.CheckNotNull("mixinType", mixinType);
      ArgumentUtility.CheckNotNull("explicitDependencies", explicitDependencies);
      ArgumentUtility.CheckNotNull("suppressedMixins", suppressedMixins);
      ArgumentUtility.CheckNotNull("origin", origin);

      MixinContextBuilder mixinContextBuilder = AddMixinToClass(targetType, mixinType, origin);

      mixinContextBuilder
          .OfKind(mixinKind)
          .WithDependencies(explicitDependencies.ToArray())
          .WithIntroducedMemberVisibility(introducedMemberVisibility)
          .ReplaceMixins(suppressedMixins.ToArray());

      return this;
    }

    /// <summary>
    /// Adds the given mixin to the given target type with a number of explicit dependencies and suppressed mixins,
    /// using the calling method as <see cref="MixinContextBuilder.Origin"/>. This is a shortcut
    /// method for calling <see cref="ForClass"/>, <see cref="ClassContextBuilder.AddMixin(System.Type,Remotion.Mixins.Context.MixinContextOrigin)"/>,
    /// <see cref="MixinContextBuilder.WithDependencies"/>, and <see cref="MixinContextBuilder.ReplaceMixins"/> in a row.
    /// </summary>
    /// <param name="mixinKind">The kind of relationship the mixin has with its target class.</param>
    /// <param name="targetType">The target type to add a mixin for.</param>
    /// <param name="mixinType">The mixin type to add.</param>
    /// <param name="introducedMemberVisibility">The default visibility to be used for introduced members.</param>
    /// <param name="explicitDependencies">The explicit dependencies of the mixin in the context of the target type.</param>
    /// <param name="suppressedMixins">The mixins suppressed by this mixin in the context of the target type.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public MixinConfigurationBuilder AddMixinToClass (
        MixinKind mixinKind,
        Type targetType,
        Type mixinType,
        MemberVisibility introducedMemberVisibility,
        IEnumerable<Type> explicitDependencies,
        IEnumerable<Type> suppressedMixins)
    {
      ArgumentUtility.CheckNotNull("targetType", targetType);
      ArgumentUtility.CheckNotNull("mixinType", mixinType);
      ArgumentUtility.CheckNotNull("explicitDependencies", explicitDependencies);
      ArgumentUtility.CheckNotNull("suppressedMixins", suppressedMixins);

      var origin = MixinContextOrigin.CreateForStackFrame(new StackFrame(1));
      return AddMixinToClass(mixinKind, targetType, mixinType, introducedMemberVisibility, explicitDependencies, suppressedMixins, origin);
    }

    private MixinContextBuilder AddMixinToClass (Type targetType, Type mixinType, MixinContextOrigin origin)
    {
      MixinContextBuilder? mixinContextBuilder;
      try
      {
        mixinContextBuilder = ForClass(targetType).AddMixin(mixinType, origin);
      }
      catch (ArgumentException ex)
      {
        Type typeForMessage = mixinType;
        if (typeForMessage.IsGenericType)
          typeForMessage = typeForMessage.GetGenericTypeDefinition();
        Assertion.IsNotNull(typeForMessage);
        string message = string.Format(
            "Two instances of mixin {0} are configured for target type {1}.",
            typeForMessage.GetFullNameSafe(),
            targetType.GetFullNameSafe());
        throw new ConfigurationException(message, ex);
      }
      return mixinContextBuilder;
    }

    /// <summary>
    /// Builds a configuration object with the data gathered so far.
    /// </summary>
    /// <returns>A new <see cref="MixinConfiguration"/> instance incorporating all the data acquired so far.</returns>
    public virtual MixinConfiguration BuildConfiguration ()
    {
      using (StopwatchScope.CreateScope(s_logger, LogLevel.Information, "Time needed to build mixin configuration from fluent builders: {elapsed}."))
      {
        var parentContexts = ParentConfiguration != null ? ParentConfiguration.ClassContexts : new ClassContextCollection();
        s_logger.LogDebug("Building a mixin configuration with {0} parent class contexts from fluent builders...", parentContexts.Count);

        var builder = new InheritanceResolvingClassContextBuilder(ClassContextBuilders, parentContexts, DefaultMixinInheritancePolicy.Instance);

        var allContexts = builder.BuildAllAndCombineWithParentContexts();
        var classContextCollection = new ClassContextCollection(allContexts);
        return new MixinConfiguration(classContextCollection)
            .LogAndReturnValue(
                s_logger,
                LogLevel.Information,
                conf => string.Format("Built mixin configuration from fluent builders with {0} class contexts.", conf.ClassContexts.Count));
      }
    }

    /// <summary>
    /// Builds a configuration object and calls the <see cref="EnterScope"/> method on it, thus activating the configuration for the current
    /// thread. The previous configuration is restored when the returned object's <see cref="IDisposable.Dispose"/> method is called (e.g. by a
    /// using statement).
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    public virtual IDisposable EnterScope ()
    {
      MixinConfiguration configuration = BuildConfiguration();
      return configuration.EnterScope();
    }
  }
}
