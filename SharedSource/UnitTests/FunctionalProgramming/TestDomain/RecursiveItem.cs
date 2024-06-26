// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.ObjectModel;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.FunctionalProgramming.TestDomain
{
  public class RecursiveItem
  {
    private readonly RecursiveItem[] _children;

    public RecursiveItem (params RecursiveItem[] children)
    {
      _children = children;
    }

    public ReadOnlyCollection<RecursiveItem> Children
    {
      get { return Array.AsReadOnly(_children); }
    }
  }
}
