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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UberProfIntegration.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UberProfIntegration.UnitTests
{
  [TestFixture]
  public class LinqToSqlIntegrationTest : LinqToSqlTestBase
  {
    private ServiceLocatorScope _serviceLocatorScope;
    private TracingAppender _tracingAppender;
    private ObjectID _objectID;

    public override void SetUp ()
    {
      base.SetUp();

      var clientTransaction = ClientTransaction.CreateRootTransaction();
      var sampleObject = LifetimeService.NewObject(clientTransaction, typeof(SampleObject), ParamList.Empty);
      _objectID = sampleObject.ID;
      clientTransaction.Commit();

      var storageSettings = SafeServiceLocator.Current.GetInstance<IStorageSettings>();

      var locator = DefaultServiceLocator.Create();
      var factory = new LinqToSqlExtensionFactory();
      locator.RegisterSingle<IClientTransactionExtensionFactory>(() => factory);
      locator.RegisterSingle<IPersistenceExtensionFactory>(() => factory);
      locator.RegisterSingle(() => storageSettings);
      _serviceLocatorScope = new ServiceLocatorScope(locator);

      _tracingAppender = new TracingAppender();
      SetAppender(_tracingAppender);
    }

    public override void TearDown ()
    {
      _serviceLocatorScope.Dispose();
      base.TearDown();
    }

    [Test]
    public void LoadSingleObject ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      LifetimeService.GetObject(clientTransaction, _objectID, false);
      clientTransaction.Discard();

      Assert.That(
          _tracingAppender.TraceLog,
          Does.Match(
              @"^ConnectionStarted \((?<connectionid>[^,]+)\)" + Environment.NewLine
              + @"StatementExecuted \(\k<connectionid>, (?<statementid>[^,]+), "
              + @"SELECT \[ID\], \[ClassID\], \[Timestamp\], \[SampleProperty\] "
              + @"FROM \[SampleObject\] WHERE \[ID\] = \@ID;" + Environment.NewLine
              + @"-- Ignore unbounded result sets: TOP \*" + Environment.NewLine
              + @"-- Parameters:" + Environment.NewLine
              + string.Format(@"-- \@ID = \[-\[{0}\]-\] \[-\[Type \(0\)\]-\]", _objectID.Value) + Environment.NewLine
              + @"\)" + Environment.NewLine
              + @"CommandDurationAndRowCount \(\k<connectionid>, \d+, \<null\>\)" + Environment.NewLine
              + @"StatementRowCount \(\k<connectionid>, \k<statementid>, 1\)" + Environment.NewLine
              + @"ConnectionDisposed \(\k<connectionid>\)" + Environment.NewLine
              + @"$"));
    }
  }
}
