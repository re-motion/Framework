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
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Remotion.SecurityManager.Clients.Web.Test.SetupDB
{

public class DBUtility
{
  // types

  // static members and constants

  public static void ExecuteSqlFile (string sqlFile, SqlConnection connection)
  {
    ExecuteSqlFile(sqlFile, connection, null, null);
  }

  public static void ExecuteSqlFile (string sqlFile, SqlConnection connection, string databaseName)
  {
    ExecuteSqlFile(sqlFile, connection, databaseName, null);
  }

  public static void ExecuteSqlFile (string sqlFile, SqlConnection connection, string databaseName, string databaseFilesPath)
  {
    string sqlString = ReadFile(sqlFile);

    if (databaseName != null && databaseName != string.Empty)
      sqlString = sqlString.Replace("<Database>", databaseName);

    if (databaseFilesPath != null && databaseFilesPath != string.Empty)
      sqlString = sqlString.Replace("<DatabaseFilesPath>", databaseFilesPath);

    string[] sqlScriptParts = Regex.Split(
      sqlString, @"^[ \t]*GO[ \t]*(\r\n)?", RegexOptions.IgnoreCase | RegexOptions.Multiline);

    foreach (string sqlScriptPart in sqlScriptParts)
    {
      if (sqlScriptPart.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim() != string.Empty)
      {
        using (SqlCommand command = new SqlCommand(sqlScriptPart, connection))
        {
          command.ExecuteNonQuery();
        }
      }
    }
  }

  public static string ReadFile (string file)
  {
    using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
    {
      return reader.ReadToEnd();
    }
  }

  #if VERBOSE
    private static DateTime s_prevNow = DateTime.Now;
  #endif

  /// <remarks>
  /// key: string (table name)
  /// value DataTable (schema table)
  /// </remarks>
  private static Hashtable _schemaTables = new Hashtable();

  public static void LoadAllCsvFiles (string directory, bool resetTables, SqlConnection connection)
  {
    LoadAllCsvFiles(directory, resetTables, connection, null);
  }

  public static void LoadAllCsvFiles (string directory, bool resetTables, SqlConnection connection, SqlTransaction transaction)
  {
    string[] tables = {
        "File",
        "FileItem",
        "Tenant",
        "Group",
        "GroupType",
        "GroupTypePosition",
        "Position",
        "Role",
        "User",
        "SecurableClassDefinition",
        "StatePropertyDefinition",
        "EnumValueDefinition",
        "StatePropertyReference",
        "AccessTypeReference",
        "Culture",
        "LocalizedName",
        "AccessControlList",
        "StateCombination",
        "StateUsage",
        "AccessControlEntry",
        "Permission"
   };

    if (resetTables)
    {
      StringBuilder sb = new StringBuilder(1000);

      sb.Append(
          @"DECLARE @statement nvarchar (MAX)
          SET @statement = ''
          SELECT @statement = @statement + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
              FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
              WHERE fk.xtype = 'F'
              ORDER BY t.name, fk.name
          exec sp_executesql @statement;");

      foreach (string table in tables)
        sb.AppendFormat("DELETE FROM [{0}];\r\n", table);

      SqlCommand command = new SqlCommand(sb.ToString(), connection, transaction);
      command.ExecuteNonQuery();
    }

    for (int i = tables.Length - 1; i >= 0; --i)
    {
      StringBuilder sb = new StringBuilder(directory, directory.Length + 30);
      if (! directory.EndsWith("\\"))
        sb.Append("\\");
      sb.Append(tables[i]);
      sb.Append(".csv");
      string path = sb.ToString();
      if (new FileInfo(path).Exists)
        LoadCsvFile(path.ToString(), connection, transaction);
    }
  }

  public static void LoadCsvFile (string path, SqlConnection connection, SqlTransaction transaction)
  {
    TimeTrace("start of loadData");

    FileInfo fileInfo = new FileInfo(path);
    int lastBs = path.LastIndexOf("\\");
    string name = (lastBs != -1) ? path.Substring(lastBs + 1) : path;
    int lastDot = name.LastIndexOf(".");
    string tableName = (lastDot != -1) ? name.Substring(0, lastDot) : name;

    StreamReader csvReader = null;
    try
    {
      Trace("------------");
      TimeTrace("before read csv file");

      csvReader = new StreamReader(path, Encoding.UTF8);

      string columnNameLine = csvReader.ReadLine();
      int commaPos = columnNameLine.IndexOf(",");
      char delimiter = (commaPos != -1) ? ',' : ';';
      string[] columnNames = columnNameLine.Split(delimiter);

      #region tracing
        #if VERBOSE
          Trace("tableName: {0}", tableName);
          foreach (string columnName in columnNames)
            Trace("column: {0}", columnName);
          Trace("------------");
        #endif
      #endregion

      string[][] records = ParseCsv(csvReader.ReadToEnd(), delimiter);
      TimeTrace("after read csv file");
      LoadData(tableName, columnNames, records, connection, transaction);

      #region tracing
        #if VERBOSE
          foreach (string[] record in records)
          {
            for (int i = 0; i < record.Length; ++i)
              Trace(columnNames[i] + ": " + record[i]);
            Trace("----");
          }
        #endif
      #endregion
    }
    finally
    {
      if (csvReader != null)
        csvReader.Close();
    }
    TimeTrace("end of LoadData");
  }

  private static void LoadData (
      string tableName, string[] columnNames, string[][] records, SqlConnection connection, SqlTransaction transaction)
  {
    DataTable schemaTable = (DataTable)_schemaTables[tableName];
    if (schemaTable == null)
    {
      string selectStatement = "SELECT * FROM [" + tableName + "]";
      SqlDataAdapter schemaAdapter = new SqlDataAdapter(selectStatement, connection);
      TimeTrace("before reading schema");

      schemaAdapter.SelectCommand.Transaction = transaction;
      using (SqlDataReader schemaReader = schemaAdapter.SelectCommand.ExecuteReader(
              CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
      {
        schemaTable = schemaReader.GetSchemaTable();
        _schemaTables[tableName] = schemaTable;
        schemaReader.Close();
      }
      TimeTrace("after reading schema");
    }

    Type[] columnTypes = new Type[columnNames.Length];
    bool[] columnNullableFlags = new bool[columnNames.Length];

    ArrayList columnNamesComplete = new ArrayList(columnNames);
    bool hasCreatedBy = false;
    bool hasCreatedAt = false;
    bool hasChangedBy = false;
    bool hasChangedAt = false;
    bool hasClassID = false;

    foreach (DataRow schemaRow in schemaTable.Rows)
    {
      string columnName = (string)schemaRow["ColumnName"];
      TimeTrace("preparing schema for " + columnName);
      int index = Array.IndexOf(columnNames, columnName);

      if (index != -1)
      {
        columnTypes[index] = (Type)schemaRow["DataType"];
        columnNullableFlags[index] = (bool)schemaRow["AllowDBNull"];
      }
      else if (columnName == "CreatedBy")
      {
        hasCreatedBy = true;
        columnNamesComplete.Add("CreatedBy");
      }
      else if (columnName == "CreatedAt")
      {
        hasCreatedAt = true;
        columnNamesComplete.Add("CreatedAt");
      }
      else if (columnName == "ChangedBy")
      {
        hasChangedBy = true;
        columnNamesComplete.Add("ChangedBy");
      }
      else if (columnName == "ChangedAt")
      {
        hasChangedAt = true;
        columnNamesComplete.Add("ChangedAt");
      }
      else if (columnName == "ClassID")
      {
        hasClassID = true;
        columnNamesComplete.Add("ClassID");
      }
    }
    TimeTrace("finished preparing columns");

    // Not all tables have identity columns (e.g. Resources) => 
    // Check if table has an identity column before using IDENTITY_INSERT.
    string sqlStatementTemplate =
        "IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID ('[{0}]') and (colstat & 1 <> 0))\n" +
        "  SET IDENTITY_INSERT [{0}] {1}";

    SqlCommand identityOnCommand = new SqlCommand(string.Format(sqlStatementTemplate, tableName, "ON"), connection, transaction);
    SqlCommand identityOffCommand = new SqlCommand(string.Format(sqlStatementTemplate, tableName, "OFF"), connection, transaction);

    string commandString = GetCommandString(tableName, (string[])columnNamesComplete.ToArray(typeof(string)));

    TimeTrace("before set identity insert");
    identityOnCommand.ExecuteNonQuery();
    TimeTrace("after set identity insert");

    for (int idxRecord = 0; idxRecord < records.Length; ++idxRecord)
    {
      TimeTrace("prepare insert record " + idxRecord);
      SqlCommand command = new SqlCommand(commandString, connection, transaction);
      TimeTrace("after new sqlcommand" + idxRecord);
      string[] record = records[idxRecord];
      if (record.Length > columnNames.Length)
      {
        throw new ApplicationException(string.Format("error in row {0} (line {1}): field count = {2}, column count = {3}.",
            idxRecord, idxRecord + 2, record.Length, columnNames.Length));
      }

      TimeTrace("before parameters");
      bool isClassIDPresent = false;
      for (int idxColumn = 0; idxColumn < columnNames.Length; ++idxColumn)
      {
        //TimeTrace ("... parameter " + idxColumn);
        Type columnType = columnTypes[idxColumn];
        bool columnIsNullable = columnNullableFlags[idxColumn];
        string columnValue = (idxColumn < record.Length) ? record[idxColumn] : string.Empty;
        string parameterName = "@" + columnNames[idxColumn];
        bool isNull = columnValue.Trim() == string.Empty;
        SqlParameter parameter;

        if (columnNames[idxColumn] == "ClassID")
          isClassIDPresent = true;

        if (columnType == typeof(string))
        {
          string columnStringValue = columnValue.Replace("\n", "\r\n");
          parameter = command.Parameters.Add(parameterName, SqlDbType.VarChar, columnStringValue.Length);
          parameter.Value = (columnStringValue.Trim() == string.Empty && columnIsNullable)
              ? DBNull.Value : (object)columnStringValue;
        }
        else if (columnType == typeof(int))
        {
          parameter = command.Parameters.Add(parameterName, SqlDbType.Int);
          parameter.Value = isNull ? DBNull.Value : (object)int.Parse(columnValue);
        }
        else if (columnType == typeof(bool))
        {
          parameter = command.Parameters.Add(parameterName, SqlDbType.Bit);
          parameter.Value = isNull ? DBNull.Value : (object)(bool)(columnValue.Trim() == "1");
        }
        else if (columnType == typeof(DateTime))
        {
          parameter = command.Parameters.Add(parameterName, SqlDbType.DateTime2);
          parameter.Value = isNull ? DBNull.Value : (object)DateTime.Parse(columnValue.Trim(), new CultureInfo("en-US"));
        }
        else if (columnType == typeof(Decimal))
        {
          parameter = command.Parameters.Add(parameterName, SqlDbType.Decimal);
          parameter.Value = isNull ? DBNull.Value : (object)Decimal.Parse(columnValue, CultureInfo.InvariantCulture);
        }
        else if (columnType == typeof(Double))
        {
          parameter = command.Parameters.Add(parameterName, SqlDbType.Float);
          parameter.Value = isNull ? DBNull.Value : (object)Double.Parse(columnValue, CultureInfo.InvariantCulture);
        }
        else if (columnType == typeof(Guid))
        {
          parameter = command.Parameters.Add(parameterName, SqlDbType.UniqueIdentifier);
          parameter.Value = isNull ? DBNull.Value : (object)new Guid(columnValue);
        }
        else if (columnType == typeof(Byte[]))
        {
          parameter = command.Parameters.Add(parameterName, SqlDbType.Image);
          parameter.Value = isNull ? DBNull.Value : (object)Encoding.UTF8.GetBytes(columnValue);
        }
        else
        {
          throw new ApplicationException(string.Format("Unsupported type in column {0} (Views?). Cannot insert {1} values.",
              columnNames[idxColumn],
              (columnType != null) ? columnType.FullName : "(unknown type)"));
        }
      }

      if (!isClassIDPresent && hasClassID)
      {
        string classID = tableName;

        SqlParameter parameter = command.Parameters.Add("@ClassID", SqlDbType.VarChar, classID.Length);
        parameter.Value = classID;
      }

      TimeTrace("after parameters");

      string userName = "TestDataLoader";
      if (hasCreatedBy)
        command.Parameters.Add("@CreatedBy", SqlDbType.VarChar, userName.Length).Value = userName;
      if (hasCreatedAt)
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
      if (hasChangedBy)
        command.Parameters.Add("@ChangedBy", SqlDbType.VarChar, userName.Length).Value = userName;
      if (hasChangedAt)
        command.Parameters.AddWithValue("@ChangedAt", DateTime.Now);

      TimeTrace("before executeNonQuery()");
      int rows = command.ExecuteNonQuery();
    }

    identityOffCommand.ExecuteNonQuery();
  }

  private static string GetCommandString (string tableName, string[] columnNames)
  {
    StringBuilder columns = new StringBuilder();
    StringBuilder parameters = new StringBuilder();
    StringBuilder setStatements = new StringBuilder();

    bool hasID = false;
    bool firstSetParameter = true;

    for (int i = 0; i < columnNames.Length; ++i)
    {
      if (i > 0)
      {
        columns.Append(", ");
        parameters.Append(", ");
      }
      string columnName = columnNames[i];

      columns.Append("[" + columnName + "]");
      parameters.Append("@" + columnName);

      if (columnName.ToLower() == "id")
        hasID = true;
      else
      {
        if (firstSetParameter)
          firstSetParameter = false;
        else
          setStatements.Append(", ");
        setStatements.Append("[" + columnName + "] = @" + columnName);
      }
    }

    if (hasID)
    {
      return string.Format(
          "IF EXISTS (SELECT * FROM [{0}] WHERE ID = @id)"
            + "\n   UPDATE [{0}] SET {3}"
            + "\n   WHERE ID = @id"
            + "\n ELSE"
            + "\n   INSERT INTO [{0}] ( {1} )"
            + "\n   VALUES ( {2} )",
          tableName, columns, parameters, setStatements);
    }
    else
    {
      return string.Format("INSERT INTO {0} ( {1} ) VALUES ( {2} )",
          tableName, columns, parameters);
    }
  }

  private static string[][] ParseCsv (string csvContent, char delimiter)
  {
    ArrayList records = new ArrayList();
    ArrayList cells = new ArrayList();

    int state = 0;
    StringBuilder cell = new StringBuilder();
    for (int i = 0; i < csvContent.Length; ++i)
    {
      char c = csvContent[i];
      char nextC = (i < csvContent.Length - 1) ? csvContent[i+1] : '\0';
      switch (state)
      {
        case 0: // within cell, outside quotation marks
          if (c == delimiter)
          {
            cells.Add(cell.ToString());
            cell = new StringBuilder();
          }
          else if (c == '"')
          {
            state = 1;
          }
          else if (c == '\r' && nextC == '\n')
          {
            cells.Add(cell.ToString());
            records.Add(cells.ToArray(typeof(string)));
            cells = new ArrayList();
            cell = new StringBuilder();
            ++i;
          }
          else
          {
            cell.Append(c);
          }
          break;

        case 1: // within quotation marks
          if (c == '"' && nextC == '"')
          {
            cell.Append('"');
            ++i;
          }
          else if (c == '"')
          {
            state = 0;
          }
          else
          {
            cell.Append(c);
          }
          break;
      }
    }
    return (string[][])records.ToArray(typeof(string[]));
  }

  [Conditional("VERBOSE")]
  private static void TimeTrace (string msg)
  {
    #if VERBOSE
      DateTime now = DateTime.Now;
      TimeSpan diff = now - s_prevNow;
      string time = string.Format("{0}.{1:D3}\t{2,5}", now.ToLongTimeString(), now.Millisecond, diff.Milliseconds);
      Debug.WriteLine(time + "\t" + msg);
      s_prevNow = now;
    #endif 
  }

  [Conditional("VERBOSE")]
  private static void Trace (string msg)
  {
    Debug.WriteLine(msg);
  }

  [Conditional("VERBOSE")]
  private static void Trace (string format, params object[] args)
  {
    Debug.WriteLine(string.Format(format, args));
  }

  // member fields

  // construction and disposing

  private DBUtility ()
  {
  }


  // methods and properties
}

}
