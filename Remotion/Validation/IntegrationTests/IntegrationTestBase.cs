// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using log4net.Appender;
using log4net.Config;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.Validation.Merging;
using LogManager = log4net.LogManager;

namespace Remotion.Validation.IntegrationTests
{
  [SetUICulture("")]
  [SetCulture("")]
  public abstract class IntegrationTestBase
  {
    protected IValidatorBuilder ValidationBuilder;
    protected MemoryAppender MemoryAppender;
    protected bool ShowLogOutput;
    private ServiceLocatorScope _serviceLocatorScope;

    [SetUp]
    public virtual void SetUp ()
    {
      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);

      MemoryAppender = new MemoryAppender();
      BasicConfigurator.Configure(MemoryAppender);

      ValidationBuilder = serviceLocator.GetInstance<IValidatorBuilder>();
    }

    [TearDown]
    public void TearDown ()
    {
      if (ShowLogOutput)
      {
        var logEvents = MemoryAppender.GetEvents().Reverse().ToArray();
        Console.WriteLine(logEvents.Skip(1).First().RenderedMessage);
        Console.WriteLine(logEvents.First().RenderedMessage);
      }

      MemoryAppender.Clear();
      LogManager.ResetConfiguration();

      Assert.That(LogManager.GetLogger(typeof(DiagnosticOutputValidationRuleMergeDecorator)).IsDebugEnabled, Is.False);
      _serviceLocatorScope.Dispose();
    }
  }
}
