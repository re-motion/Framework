﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it
// and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 2.1 of the
// License, or (at your option) any later version.
//
// re-motion is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace Remotion.Logging.Log4Net.UnitTests;

public class Log4NetLoggerProviderTest
{
  [Test]
  public void GetLogger_WithNameAsString ()
  {
    var loggerProvider = new Log4NetLoggerProvider();
    var categoryName = "TestCategory";

    var logger = loggerProvider.CreateLogger(categoryName);

    Assert.That(logger, Is.InstanceOf<Log4NetLogger>());
    var log4NetLogger = (Log4NetLogger)logger;
    Assert.That(log4NetLogger.Logger.Name, Is.EqualTo("TestCategory"));
  }

  [Test]
  public void CreateLogger_WithValidCategoryName_ReturnsLoggerInstance ()
  {
    var loggerProvider = new Log4NetLoggerProvider();
    var categoryName = "TestCategory";

    var logger = loggerProvider.CreateLogger(categoryName);

    Assert.That(logger, Is.InstanceOf<Log4NetLogger>());
    var secondLogger = loggerProvider.CreateLogger(categoryName);
    Assert.That(logger.As<Log4NetLogger>().Logger, Is.SameAs(secondLogger.As<Log4NetLogger>().Logger));
  }
}
