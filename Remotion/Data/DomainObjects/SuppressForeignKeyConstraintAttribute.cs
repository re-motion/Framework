// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects;

/// <summary>
///   When applied to properties of type <see cref="DomainObject" />, no foreign key constraint is generated for this property in the database.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SuppressForeignKeyConstraintAttribute : Attribute, ISuppressForeignKeyConstraintAttribute
{
  bool ISuppressForeignKeyConstraintAttribute.IsForeignKeyConstraintSuppressed => true;
}
