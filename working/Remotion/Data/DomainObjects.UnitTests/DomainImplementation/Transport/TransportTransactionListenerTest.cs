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
using Remotion.Data.DomainObjects.DomainImplementation.Transport;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Transport
{
  [TestFixture]
  public class TransportTransactionListenerTest : ClientTransactionBaseTest
  {
    private DomainObjectTransporter _transporter;
    private TransportTransactionListener _listener;

    public override void SetUp ()
    {
      base.SetUp ();
      _transporter = new DomainObjectTransporter();
      _listener = new TransportTransactionListener (_transporter);
    }

    [Test]
    public void Serializable ()
    {
      Serializer.SerializeAndDeserialize (_listener);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot use the transported transaction for changing properties after " 
        + "it has been deserialized.")]
    public void Serialization_AndMethodCalled ()
    {
      TransportTransactionListener listener = Serializer.SerializeAndDeserialize (_listener);
      listener.PropertyValueChanging (TestableClientTransaction, null, null, null, null);
    }

    [Test]
    public void ModifyingProperty_Loaded ()
    {
      _transporter.Load (DomainObjectIDs.Computer1);

      var source = (Computer) _transporter.GetTransportedObject(DomainObjectIDs.Computer1);
      _listener.PropertyValueChanging (TestableClientTransaction, source, GetPropertyDefinition (typeof (Computer), "SerialNumber"), null, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Object 'Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid' " 
        + "cannot be modified for transportation because it hasn't been loaded yet. Load it before manipulating it.")]
    public void ModifyingProperty_NotLoaded ()
    {
      _transporter.Load (DomainObjectIDs.Computer2);

      var transportTransaction = _transporter.GetTransportedObject (DomainObjectIDs.Computer2).RootTransaction;
      var source = LifetimeService.GetObject (transportTransaction, DomainObjectIDs.Computer1, false);
      _listener.PropertyValueChanging (TestableClientTransaction, source, GetPropertyDefinition (typeof (Computer), "SerialNumber"), null, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The transport transaction cannot be committed.")]
    public void CommitingTransaction ()
    {
      _transporter.Load (DomainObjectIDs.Computer2).RootTransaction.Commit ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The transport transaction cannot be rolled back.")]
    public void RollingBackTransaction ()
    {
      _transporter.Load (DomainObjectIDs.Computer2).RootTransaction.Rollback ();
    }
  }
}
