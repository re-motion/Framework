// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.FunctionalProgramming.TestDomain
{
  internal class Element
  {
    private readonly int _value;
    private Element? _parent;

    public Element (int value, Element? parent)
    {
      _value = value;
      _parent = parent;
    }

    public Element? Parent
    {
      get { return _parent; }
    }

    public void SetParent (Element parent)
    {
      _parent = parent;
    }

    public override string ToString ()
    {
      return _value.ToString();
    }
  }
}
