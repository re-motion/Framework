// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Remotion.Logging.Log4Net;
using Remotion.ServiceLocation;

namespace Remotion.Globalization.ExtensibleEnums.UnitTests;

[SetUpFixture]
public class SetUpFixture
{
  [OneTimeSetUp]
  public void SetUp ()
  {
    BootstrapServiceConfiguration.SetLoggerFactory(new LoggerFactory([new Log4NetLoggerProvider()]));
  }
}
