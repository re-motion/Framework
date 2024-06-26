// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
#if !FEATURE_SERIALIZATION
using NUnit.Framework;
#endif

// ReSharper disable once CheckNamespace
namespace Remotion.Development.NUnit.UnitTesting
{
  public static class Assert2
  {
    public static void IgnoreIfFeatureSerializationIsDisabled ()
    {
#if !FEATURE_SERIALIZATION
      Assert.Ignore("Binary serialization has been disabled.");
#endif
    }
  }
}
