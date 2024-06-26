// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0

// ReSharper disable once CheckNamespace

using System;

#nullable enable
namespace Remotion.UnitTests.Utilities.MemberInfoEqualityComparerTestDomain
{
  public class ClassWithMethods
  {
    public void SimpleMethod1 ()
    {
    }

    public void SimpleMethod2 ()
    {
    }

    public virtual void OverriddenMethod ()
    {
    }

    public void GenericMethod<T> ()
    {
    }
  }
}
