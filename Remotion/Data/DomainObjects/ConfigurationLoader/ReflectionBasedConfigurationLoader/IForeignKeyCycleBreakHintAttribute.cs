// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

using System;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

/// <summary>
/// When applied to properties of type <see cref="DomainObject" />, a <see cref="ForeignKeyCycleBreakHint"/> is applied
/// to the generated foreign keys.
/// </summary>
public interface IForeignKeyCycleBreakHintAttribute : IMappingAttribute
{
  ForeignKeyCycleBreakHint CycleBreakHint { get; }
}
