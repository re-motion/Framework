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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders.Specifications
{
  /// <summary>
  /// The <see cref="SqlXmlSetComparedColumnSpecification"/> builds a specification command that allows retrieving a set of records.
  /// </summary>
  public class SqlXmlSetComparedColumnSpecification : IComparedColumnsSpecification
  {
    private readonly ColumnDefinition _columnDefinition;
    private readonly object[] _objectValues;

    public SqlXmlSetComparedColumnSpecification (ColumnDefinition columnDefinition, IEnumerable<object> objectValues)
    {
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);
      ArgumentUtility.CheckNotNull ("objectValues", objectValues);

      _columnDefinition = columnDefinition;
      _objectValues = objectValues.ToArray();
    }

    public ColumnDefinition ColumnDefinition
    {
      get { return _columnDefinition; }
    }

    public object[] ObjectValues
    {
      get { return _objectValues; }
    }

    public void AddParameters (IDbCommand command, ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      var stringWriter = new StringWriter ();
      var xmlWriter = new XmlTextWriter (stringWriter);
      xmlWriter.WriteStartElement ("L");
      foreach (var value in ObjectValues)
      {
        xmlWriter.WriteStartElement ("I");
        if (value == null)
          throw new NotSupportedException ("SQL Server cannot represent NULL values in an XML data type.");
        xmlWriter.WriteString (value.ToString ());
        xmlWriter.WriteEndElement ();
      }
      xmlWriter.WriteEndElement ();

      var parameter = command.CreateParameter ();
      parameter.ParameterName = GetParameterName (sqlDialect);
      parameter.DbType = DbType.Xml;
      parameter.Value = stringWriter.ToString ();
      command.Parameters.Add (parameter);
    }

    public void AppendComparisons (
        StringBuilder statement, IDbCommand command, ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      statement.Append (sqlDialect.DelimitIdentifier (_columnDefinition.Name));
      statement.Append (" IN (");
      statement.Append ("SELECT T.c.value('.', '").Append (_columnDefinition.StorageTypeInfo.StorageTypeName).Append ("')");
      statement.Append (" FROM ");
      statement.Append (GetParameterName (sqlDialect));
      statement.Append (".nodes('/L/I') T(c))");

      command.CommandText = statement.ToString();
    }

    private string GetParameterName (ISqlDialect sqlDialect)
    {
      return sqlDialect.GetParameterName (_columnDefinition.Name);
    }
  }
}