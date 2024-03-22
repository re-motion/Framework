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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  public class SqlProviderGeneratedSqlTestHelper : IDisposable
  {
    private IStorageSettings _storageSettings;
    private readonly RdbmsProviderDefinition _rdbmsProviderDefinition;
    private readonly Mock<ObservableRdbmsProvider.ICommandExecutionListener> _executionListenerStrictMock;
    private readonly RdbmsProvider _provider;

    public SqlProviderGeneratedSqlTestHelper (IStorageSettings storageSettings, RdbmsProviderDefinition rdbmsProviderDefinition)
    {
      _storageSettings = storageSettings;
      _rdbmsProviderDefinition = rdbmsProviderDefinition;
      _executionListenerStrictMock = new Mock<ObservableRdbmsProvider.ICommandExecutionListener>(MockBehavior.Strict);
      _provider = RdbmsProviderObjectMother.CreateForIntegrationTest(
          storageSettings,
          rdbmsProviderDefinition,
          (providerDefinition, persistenceListener, commandFactory) =>
              new ObservableRdbmsProvider(
                  providerDefinition,
                  NullPersistenceExtension.Instance,
                  commandFactory,
                  () => new SqlConnection(),
                  _executionListenerStrictMock.Object));
    }

    public void Dispose ()
    {
      _provider.Dispose();
    }

    public RdbmsProvider Provider
    {
      get { return _provider; }
    }

    public void VerifyAllExpectations ()
    {
      _executionListenerStrictMock.Verify();
    }

    internal void ExpectExecuteReader (
        VerifiableSequence sequence,
        CommandBehavior expectedCommandBehavior,
        string expectedSql,
        params Tuple<string, DbType, object>[] expectedParametersData)
    {
      _executionListenerStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.OnExecuteReader(It.Is<IDbCommand>(cmd => cmd.CommandText == expectedSql), expectedCommandBehavior))
          .Callback((IDbCommand command, CommandBehavior _) => CheckCommand(command, expectedSql, expectedParametersData))
          .Verifiable();
    }

    internal void ExpectExecuteScalar (
        VerifiableSequence sequence,
        string expectedSql,
        params Tuple<string, DbType, object>[] expectedParametersData)
    {
      _executionListenerStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.OnExecuteScalar(It.IsAny<IDbCommand>()))
          .Callback((IDbCommand command) => CheckCommand(command, expectedSql, expectedParametersData))
          .Verifiable();
    }

    internal void ExpectExecuteNonQuery (
        VerifiableSequence sequence,
        string expectedSql,
        params Tuple<string, DbType, object>[] expectedParametersData)
    {
      _executionListenerStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.OnExecuteNonQuery(It.Is<IDbCommand>(cmd => cmd.CommandText == expectedSql)))
          .Callback((IDbCommand command) => CheckCommand(command, expectedSql, expectedParametersData))
          .Verifiable();
    }

    public void CheckCommand (IDbCommand sqlCommand, string expectedSql, params Tuple<string, DbType, object>[] expectedParametersData)
    {
      try
      {
        Assert.That(
            sqlCommand.CommandText,
            Is.EqualTo(expectedSql),
            "Command text doesn't match.\r\nActual statement: {0}\r\nExpected statement: {1})",
            sqlCommand.CommandText,
            expectedSql);
        Assert.That(
            sqlCommand.CommandType,
            Is.EqualTo(CommandType.Text),
            "Command type doesn't match.\r\nExpected statement: {0})");
        Assert.That(
            sqlCommand.Parameters.Count,
            Is.EqualTo(expectedParametersData.Length),
            "Number of parameters doesn't match.\r\nStatement: {0})",
            expectedSql);
        for (int i = 0; i < expectedParametersData.Length; ++i)
        {
          var actualParameter = (IDataParameter)sqlCommand.Parameters[i];
          var expectedParameterData = expectedParametersData[i];

          Assert.That(
              actualParameter.ParameterName,
              Is.EqualTo(expectedParameterData.Item1),
              "Name of parameter " + i + " doesn't match.\r\nStatement: {0})",
              expectedSql);
          Assert.That(
              actualParameter.DbType,
              Is.EqualTo(expectedParameterData.Item2),
              "DbType of parameter " + i + " doesn't match.\r\nSstatement: {0})",
              expectedSql);
          Assert.That(
              actualParameter.Value,
              Is.EqualTo(expectedParameterData.Item3),
              "Value of parameter " + i + " doesn't match.\r\nStatement: {0})",
              expectedSql);
        }
      }
      catch (AssertionException)
      {
        Console.WriteLine(sqlCommand.CommandText);
        Console.WriteLine(sqlCommand.CommandType);
        Console.WriteLine(string.Join("," + Environment.NewLine, sqlCommand.Parameters.Cast<IDataParameter>().Select(parameter =>
        {
          string valueString;
          if (parameter.Value == DBNull.Value)
            valueString = "DBNull.Value";
          else if (parameter.Value is string)
            valueString = "\"" + parameter.Value + "\"";
          else if (parameter.Value == null)
            valueString = "null";
          else
            valueString = parameter.Value.ToString();

          return string.Format("Tuple.Create (\"{0}\", DbType.{1}, (object) {2})", parameter.ParameterName, parameter.DbType, valueString);
        })));

        throw;
      }
    }

    public DataContainer LoadDataContainerInSeparateProvider (ObjectID objectID)
    {
      using (var provider = RdbmsProviderObjectMother.CreateForIntegrationTest(_storageSettings, _rdbmsProviderDefinition))
      {
        return provider.LoadDataContainer(objectID).LocatedObject;
      }
    }
  }
}
