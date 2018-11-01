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
using System.IO;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class QueriesTest : SerializationBaseTest
  {
    [Test]
    public void QueryParameter ()
    {
      QueryParameter queryParameter = new QueryParameter ("name", "value", QueryParameterType.Text);

      QueryParameter deserializedQueryParameter = (QueryParameter) SerializeAndDeserialize (queryParameter);

      AreEqual (queryParameter, deserializedQueryParameter);
    }

    [Test]
    public void QueryParameterCollection ()
    {
      QueryParameterCollection queryParameters = new QueryParameterCollection ();
      queryParameters.Add ("Text Parameter", "Value 1", QueryParameterType.Text);
      queryParameters.Add ("Value Parameter", "Value 2", QueryParameterType.Value);

      QueryParameterCollection deserializedQueryParameters = (QueryParameterCollection) SerializeAndDeserialize (queryParameters);

      AreEqual (queryParameters, deserializedQueryParameters);
    }

    [Test]
    public void QueryDefinition ()
    {
      QueryDefinition queryDefinition = new QueryDefinition ("queryID", TestDomainStorageProviderDefinition, "statement", QueryType.Collection, typeof (DomainObjectCollection));

      QueryDefinition deserializedQueryDefinition = (QueryDefinition) SerializeAndDeserialize (queryDefinition);

      Assert.That (ReferenceEquals (queryDefinition, deserializedQueryDefinition), Is.False);
      AreEqual (queryDefinition, deserializedQueryDefinition);
    }

    [Test]
    public void QueryDefinitionInQueryConfiguration ()
    {
      QueryDefinition queryDefinition = DomainObjectsConfiguration.Current.Query.QueryDefinitions["OrderQuery"];

      QueryDefinition deserializedQueryDefinition = (QueryDefinition) SerializeAndDeserialize (queryDefinition);

      Assert.That (deserializedQueryDefinition, Is.SameAs (queryDefinition));
    }

    [Test]
    [ExpectedException (typeof (QueryConfigurationException), ExpectedMessage = "QueryDefinition 'UnknownQuery' does not exist.")]
    public void UnknownQueryDefinitionInQueryConfiguration ()
    {
      QueryDefinition unknownQueryDefinition = new QueryDefinition ("UnknownQuery", TestDomainStorageProviderDefinition, "select 42", QueryType.Scalar);
      DomainObjectsConfiguration.Current.Query.QueryDefinitions.Add (unknownQueryDefinition);

      using (MemoryStream stream = new MemoryStream ())
      {
        Serialize (stream, unknownQueryDefinition);
        DomainObjectsConfiguration.SetCurrent (
            new FakeDomainObjectsConfiguration (
                DomainObjectsConfiguration.Current.MappingLoader, DomainObjectsConfiguration.Current.Storage, new QueryConfiguration()));

        Deserialize (stream);
      }
    }

    [Test]
    public void QueryDefinitionCollection ()
    {
      QueryDefinitionCollection queryDefinitions = new QueryDefinitionCollection ();
      queryDefinitions.Add (DomainObjectsConfiguration.Current.Query.QueryDefinitions[0]);
      queryDefinitions.Add (DomainObjectsConfiguration.Current.Query.QueryDefinitions[1]);

      QueryDefinitionCollection deserializedQueryDefinitions = (QueryDefinitionCollection) SerializeAndDeserialize (queryDefinitions);
      AreEqual (queryDefinitions, deserializedQueryDefinitions);
      Assert.That (DomainObjectsConfiguration.Current.Query.QueryDefinitions[0], Is.SameAs (deserializedQueryDefinitions[0]));
      Assert.That (DomainObjectsConfiguration.Current.Query.QueryDefinitions[1], Is.SameAs (deserializedQueryDefinitions[1]));
    }

    [Test]
    public void Query ()
    {
      var query = (Query) QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1);

      var deserializedQuery = (Query) SerializeAndDeserialize (query);
      AreEqual (query, deserializedQuery);
      Assert.That (deserializedQuery.Definition, Is.SameAs (DomainObjectsConfiguration.Current.Query.QueryDefinitions["OrderQuery"]));
    }

    private void AreEqual (Query expected, Query actual)
    {
      Assert.That (ReferenceEquals (expected, actual), Is.False);
      Assert.That (actual, Is.Not.Null);

      Assert.That (actual.ID, Is.EqualTo (expected.ID));
      Assert.That (actual.Definition, Is.SameAs (expected.Definition));
      AreEqual (expected.Parameters, actual.Parameters);
    }

    private void AreEqual (QueryDefinitionCollection expected, QueryDefinitionCollection actual)
    {
      Assert.That (ReferenceEquals (expected, actual), Is.False);
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Count, Is.EqualTo (expected.Count));

      for (int i = 0; i < expected.Count; i++)
        AreEqual (expected[i], actual[i]);
    }

    private void AreEqual (QueryParameter expected, QueryParameter actual)
    {
      Assert.That (ReferenceEquals (expected, actual), Is.False);
      Assert.That (actual.Name, Is.EqualTo (expected.Name));
      Assert.That (actual.ParameterType, Is.EqualTo (expected.ParameterType));
      Assert.That (actual.Value, Is.EqualTo (expected.Value));
    }

    private void AreEqual (QueryParameterCollection expected, QueryParameterCollection actual)
    {
      Assert.That (ReferenceEquals (expected, actual), Is.False);
      Assert.That (actual.Count, Is.EqualTo (expected.Count));
      Assert.That (actual.IsReadOnly, Is.EqualTo (expected.IsReadOnly));

      for (int i = 0; i < expected.Count; i++)
      {
        AreEqual (expected[i], actual[i]);

        // Check if Hashtable of CommonCollection is deserialized correctly
        QueryParameter actualQueryParameter = actual[i];
        Assert.That (actual[actualQueryParameter.Name], Is.SameAs (actualQueryParameter));
      }
    }

    private void AreEqual (QueryDefinition expected, QueryDefinition actual)
    {
      Assert.That (actual.ID, Is.EqualTo (expected.ID));
      Assert.That (actual.QueryType, Is.EqualTo (expected.QueryType));
      Assert.That (actual.Statement, Is.EqualTo (expected.Statement));
      Assert.That (actual.StorageProviderDefinition, Is.EqualTo (expected.StorageProviderDefinition));
      Assert.That (actual.CollectionType, Is.EqualTo (expected.CollectionType));
    }

  }
}
