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
using Remotion.Utilities;

namespace Remotion.Mixins.Context.FluentBuilders
{
  /// <summary>
  /// Builds <see cref="ClassContext"/> instances from <see cref="ClassContextBuilder"/> objects and applies inheritance from a parent configuration 
  /// and the types indicated by a <see cref="IMixinInheritancePolicy"/>.
  /// This is done as follows:
  /// <list type="bullet">
  /// <item>A <see cref="ClassContext"/> that exists in the parent configuration is kept as is, unless there is a 
  /// <see cref="ClassContextBuilder"/> for the same type.</item>
  /// <item>A <see cref="ClassContextBuilder"/> is transformed into a new <see cref="ClassContext"/>:</item>
  ///   <list type="bullet">
  ///   <item>First, the <see cref="ClassContext"/> objects for its base classes and interfaces are retrieved or created.</item> 
  ///   <item>Then, the corresponding <see cref="ClassContext"/> from the parent configuration is retrieved.</item>
  ///   <item>Then, a new <see cref="ClassContext"/> is created from the <see cref="ClassContextBuilder"/>; inheriting everything from the base
  ///   and parent contexts.</item>
  ///   </list>
  /// </list>
  /// </summary>
  public class InheritanceResolvingClassContextBuilder
  {
    private readonly Dictionary<Type, Tuple<ClassContextBuilder, ClassContext>> _buildersAndParentContexts;
    private readonly IEnumerable<ClassContext> _unmodifiedParentContexts;
    private readonly Dictionary<Type, ClassContext> _finishedContextCache;
    private readonly IMixinInheritancePolicy _inheritancePolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="InheritanceResolvingClassContextBuilder"/> class.
    /// </summary>
    /// <param name="classContextBuilders">All class context builders relevant for the inheritance hierarchy of the target types involved.</param>
    /// <param name="parentContexts">The <see cref="ClassContext"/> instances defined by the parent configuration.</param>
    /// <param name="inheritancePolicy">The inheritance policy to use when resolving the inheritance hierarchy of the target types.</param>
    public InheritanceResolvingClassContextBuilder (
        IEnumerable<ClassContextBuilder> classContextBuilders,
        ClassContextCollection parentContexts, 
        IMixinInheritancePolicy inheritancePolicy)
    {
      ArgumentUtility.CheckNotNull ("classContextBuilders", classContextBuilders);
      ArgumentUtility.CheckNotNull ("parentContexts", parentContexts);
      ArgumentUtility.CheckNotNull ("inheritancePolicy", inheritancePolicy);

      _buildersAndParentContexts = classContextBuilders.ToDictionary (
          classContextBuilder => classContextBuilder.TargetType,
          classContextBuilder => Tuple.Create (classContextBuilder, parentContexts.GetWithInheritance (classContextBuilder.TargetType)));

      _unmodifiedParentContexts = parentContexts.Where (parentContext => !_buildersAndParentContexts.ContainsKey (parentContext.Type));

      _finishedContextCache = _unmodifiedParentContexts.ToDictionary (c => c.Type);
      _inheritancePolicy = inheritancePolicy;
    }

    public IEnumerable<ClassContext> BuildAllAndCombineWithParentContexts ()
    {
      return BuildAll().Concat (_unmodifiedParentContexts);
    }

    public IEnumerable<ClassContext> BuildAll ()
    {
      return _buildersAndParentContexts.Keys.Select (type => Build (type));
    }

    public ClassContext Build (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      // First probe the cache...
      var cachedContext = GetFinishedContextFromCache (type);
      if (cachedContext != null)
        return cachedContext;

      // If we have nothing in the cache, get the contexts of the base classes we need to derive our mixins from, then create a new context.
      var contextsToInheritFrom = _inheritancePolicy.GetClassContextsToInheritFrom (type, Build); // recursion!
      ClassContext builtContext = CreateContext (type, contextsToInheritFrom);
      _finishedContextCache.Add (type, builtContext);

      return builtContext;
    }

    private ClassContext GetFinishedContextFromCache (Type type)
    {
      ClassContext finishedContext;
      _finishedContextCache.TryGetValue (type, out finishedContext);
      return finishedContext;
    }

    private ClassContext CreateContext (Type type, IEnumerable<ClassContext> baseContextsToInheritFrom)
    {
      var inheritedContextCombiner = new ClassContextCombiner ();
      inheritedContextCombiner.AddRangeAllowingNulls (baseContextsToInheritFrom);

      Tuple<ClassContextBuilder, ClassContext> builderWithParentContext;
      if (_buildersAndParentContexts.TryGetValue (type, out builderWithParentContext))
      {
        inheritedContextCombiner.AddIfNotNull (builderWithParentContext.Item2);
        return CreateContextWithBuilder (builderWithParentContext.Item1, inheritedContextCombiner.GetCombinedContexts (type));
      }
      else
      {
        return CreateContextWithoutBuilder (type, inheritedContextCombiner.GetCombinedContexts (type));
      }
    }

    private ClassContext CreateContextWithBuilder (ClassContextBuilder builder, ClassContext inheritedContext)
    {
      var inheritedContexts = inheritedContext != null ? new[] { inheritedContext } : new ClassContext[0];
      var builtContext = builder.BuildClassContext (inheritedContexts);
      return builtContext;
    }

    private ClassContext CreateContextWithoutBuilder (Type type, ClassContext inheritedContext)
    {
      var builtContext = inheritedContext ?? new ClassContext (type, Enumerable.Empty<MixinContext>(), Enumerable.Empty<Type>());
      Assertion.IsTrue (builtContext.Type == type, "Guaranteed by ClassContextCombiner");
      return builtContext;
    }
  }
}
