// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

using System;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects;

/// <summary>
/// When applied to properties, this cycle break will respect the <see cref="CycleBreakHint"/>.
/// When no attribute is applied, <see cref="ForeignKeyCycleBreakHint.Automatic"/> rules are applied.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ForeignKeyCycleBreakHintAttribute : Attribute, IForeignKeyCycleBreakHintAttribute
{
  public ForeignKeyCycleBreakHintAttribute (ForeignKeyCycleBreakHint cycleBreakHint)
  {
    CycleBreakHint = cycleBreakHint;
  }

  public ForeignKeyCycleBreakHint CycleBreakHint { get; }
}
