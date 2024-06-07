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
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace Remotion.SecurityManager.Clients.Web.Test.SetupDB
{
public class SetupDB
{
  [STAThread]
  public static void Main (string[] args)
  {
    SetupDB setup = new SetupDB();
    setup.PerformSetup(args);
  }

  private bool _setupDatabase = false;
  private string _databaseName = string.Empty;
  private enum LoadDataAction { None, Test }
  private LoadDataAction _loadDataAction = LoadDataAction.None;

  private void PerformSetup (string[] args)
  {
    try
    {
      try
      {
        ParseArguments(args);
      }
      catch (ApplicationException e)
      {
        Console.Error.WriteLine("Syntax error: {0}", e.Message);
        Console.Error.WriteLine(
          "Usage: SetupDB [/dbn:<databaseName>] [/s] [/l:t]"
          + "\nArguments:"
          + "\n /dbn   ... the database name to use (if not specified, the settings in app.config are used)"
          + "\n /s    ... create if not exists and set up database"
          + "\n /l     ... load data"
          + "\n   t    ...  load test data"
          + "\nExample: SetupDB /dbn:SecurityManagerTest /s /l:t"
          + "\n- creates and sets up database SecurityManagerTest"
          + "\n- loads test data");
        return;
      }

      // Ensure directory for database files exists

      string databaseFilesPath = ConfigurationManager.AppSettings["DatabaseFilesPath"];

      if (!Directory.Exists(databaseFilesPath))
        Directory.CreateDirectory(databaseFilesPath);


      string databaseSetupFilesPath = ConfigurationManager.AppSettings["DatabaseSetupFilesPath"];

      if (_databaseName == string.Empty)
        _databaseName = ConfigurationManager.AppSettings["DatabaseName"];

      string connectionString = ConfigurationManager.AppSettings["ConnectionString"].Replace("{Database}", _databaseName);

      if (_setupDatabase)
      {
        try
        {
          using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["MasterDBConnectionString"]))
          {
            connection.Open();

            Console.WriteLine("Ensure DB exists...");
            DBUtility.ExecuteSqlFile(Path.Combine(databaseSetupFilesPath, "CreateDB.sql"), connection, _databaseName, databaseFilesPath);
          }
        }
        catch (SqlException)
        {
        }
      }

      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();

        if (_setupDatabase)
        {
          Console.WriteLine("SetupDB...");
          DBUtility.ExecuteSqlFile(Path.Combine(databaseSetupFilesPath, "TearDownDB.sql"), connection, _databaseName, databaseFilesPath);
          DBUtility.ExecuteSqlFile(Path.Combine(databaseSetupFilesPath, "SecurityManagerSetupDB.sql"), connection, _databaseName, databaseFilesPath);
          DBUtility.ExecuteSqlFile(Path.Combine(databaseSetupFilesPath, "SecurityManagerSetupConstraints.sql"), connection, _databaseName, databaseFilesPath);
          DBUtility.ExecuteSqlFile(Path.Combine(databaseSetupFilesPath, "SecurityManagerSetupDBSpecialTables.sql"), connection, _databaseName, databaseFilesPath);
          DBUtility.ExecuteSqlFile(Path.Combine(databaseSetupFilesPath, "SetupDB.sql"), connection, _databaseName, databaseFilesPath);
        }

        if (_loadDataAction == LoadDataAction.Test)
        {
          Console.WriteLine("Load Data...");
          DBUtility.LoadAllCsvFiles(ConfigurationManager.AppSettings["TestDataFilesPath"], true, connection);
        }

        if (_setupDatabase)
        {
          string setupDBPostTestDataScriptFilePath = Path.Combine(databaseSetupFilesPath, "SetupDB_PostTestData.sql");

          if (File.Exists(setupDBPostTestDataScriptFilePath))
          {
            Console.WriteLine("SetupDB Post TestData...");
            DBUtility.ExecuteSqlFile(setupDBPostTestDataScriptFilePath, connection, _databaseName);
          }
        }
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);

      Console.WriteLine("Press enter to continue.");
      Console.ReadLine();
    }
  }

  private void ParseArguments (string[] args)
  {
    Hashtable arguments = new Hashtable();
    foreach (string arg in args)
    {
      if (arg.IndexOfAny(new char [] {'/', '-'}) != 0)
        throw new ApplicationException("Arguments must start with / or -");

      string argname, argvalue;
      int pos = arg.IndexOf(':');
      if (pos >= 0)
      {
        argname = arg.Substring(1, pos-1).ToLower();
        argvalue = arg.Substring(pos+1);
      }
      else
      {
        argname = arg.Substring(1).ToLower();
        argvalue = string.Empty;
      }

      if (argname != "s" && argname != "l" && argname != "dbn")
        throw new ApplicationException("Unknown argument name " + argname);

      arguments.Add(argname.ToLower(), argvalue);
    }

    if (arguments["s"] == null && arguments["l"] == null)
      throw new ApplicationException("At least one of the following arguments must be specified: /s, /l.");

    if (arguments["s"] != null)
    {
      _setupDatabase = true;
    }

    if (arguments["l"] != null)
    {
      switch (arguments["l"].ToString().ToLower())
      {
        case "t":
          _loadDataAction = LoadDataAction.Test;
          break;
        default:
          throw new ApplicationException("invalid argument parameter \"" + arguments["l"].ToString() + "\" for parameter /l");
      }
    }

    if (arguments["dbn"] != null)
      _databaseName = arguments["dbn"].ToString();
  }

	private SetupDB ()
	{
	}
}
}
