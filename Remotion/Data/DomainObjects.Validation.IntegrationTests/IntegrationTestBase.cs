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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.Validation;
using Remotion.Validation.Merging;
using LogManager = log4net.LogManager;

namespace Remotion.Data.DomainObjects.Validation.IntegrationTests
{
  [SetUICulture("")]
  [SetCulture("")]
  public abstract class IntegrationTestBase
  {
    protected IValidatorProvider ValidationProvider;
    protected MemoryAppender MemoryAppender;
    protected bool ShowLogOutput;
    private ServiceLocatorScope _serviceLocatorScope;

    [SetUp]
    public virtual void SetUp ()
    {
      var storageSettings = SafeServiceLocator.Current.GetInstance<IStorageSettings>();

      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterSingle<IClientTransactionExtensionFactory>(
          () => new ValidationClientTransactionExtensionFactory(serviceLocator.GetInstance<IValidatorProvider>()));
      serviceLocator.RegisterSingle(() => storageSettings);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);

      MemoryAppender = new MemoryAppender();
      BasicConfigurator.Configure(MemoryAppender);

      ValidationProvider = serviceLocator.GetInstance<IValidatorProvider>();
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
      _serviceLocatorScope.Dispose();

      Assert.That(LogManager.GetLogger(typeof(DiagnosticOutputValidationRuleMergeDecorator)).IsDebugEnabled, Is.False);
    }
  }
}
