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
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  /// <summary>
  /// Implements <see cref="IMixinDeclarationAnalyzer{TAnalyzedEntity}"/> by analyzing the given <typeparamref name="TAnalyzedEntity"/> objects for 
  /// custom attributes implementing <see cref="IMixinConfigurationAttribute{TTarget}"/> and applying them to the 
  /// <see cref="MixinConfigurationBuilder"/>.
  /// </summary>
  /// <typeparam name="TAnalyzedEntity">The type of entity searched for <see cref="IMixinConfigurationAttribute{TTarget}"/> is applied to.</typeparam>
  public class MixinConfigurationAttributeAnalyzer<TAnalyzedEntity> : IMixinDeclarationAnalyzer<TAnalyzedEntity>
  {
    private readonly Func<TAnalyzedEntity, IEnumerable<IMixinConfigurationAttribute<TAnalyzedEntity>>> _attributeProvider;

    public MixinConfigurationAttributeAnalyzer (Func<TAnalyzedEntity, IEnumerable<IMixinConfigurationAttribute<TAnalyzedEntity>>> attributeProvider)
    {
      ArgumentUtility.CheckNotNull ("attributeProvider", attributeProvider);

      _attributeProvider = attributeProvider;
    }

    public virtual void Analyze (TAnalyzedEntity entity, MixinConfigurationBuilder configurationBuilder)
    {
      ArgumentUtility.CheckNotNull ("entity", entity);
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);

      var attributes = _attributeProvider (entity);

      foreach (var attribute in attributes)
        attribute.Apply (configurationBuilder, entity);
    }
  }
}