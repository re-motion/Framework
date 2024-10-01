// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Remotion.ServiceLocation;

namespace Remotion.Xml.UnitTests;

[SetUpFixture]
public class SetUpFixture
{
  [OneTimeSetUp]
  public void SetUp ()
  {
    BootstrapServiceConfiguration.SetLoggerFactory(NullLoggerFactory.Instance);
  }
}
