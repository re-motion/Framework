// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0

// ReSharper disable once CheckNamespace

using System;

#nullable enable
namespace Remotion.UnitTests.Utilities.MemberInfoEqualityComparerTestDomain
{
  public class DerivedClassWithMethods : ClassWithMethods
  {
    public override void OverriddenMethod ()
    {
      base.OverriddenMethod();
    }
  }
}
