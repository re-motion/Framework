// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting
{
  partial class PEVerifyException : Exception
  {
    public PEVerifyException (string message) : base(message)
    {
    }

    public PEVerifyException (string message, Exception inner) : base(message, inner)
    {
    }

    public PEVerifyException (int resultCode, string output) : base(ConstructMessage(resultCode, output))
    {
    }

    private static string ConstructMessage (int code, string output)
    {
      return string.Format("PEVerify returned {0}.\n{1}", code, output);
    }
  }
}
