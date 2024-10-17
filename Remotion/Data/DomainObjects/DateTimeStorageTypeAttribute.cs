// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Data;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects;

/// <summary>
/// Apply this attribute to override the default storage type of a <see cref="DateTime" /> property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class DateTimeStorageTypeAttribute (DateTimeStorageType storageType) : Attribute
{
  public DateTimeStorageType StorageType { get; } = storageType;
}
