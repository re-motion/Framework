// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;

namespace Remotion.SecurityManager.UnitTests
{
  public class DatabaseConfiguration
  {
    public static string DataSource
    {
      get { return ConfigurationManager.AppSettings["DataSource"]; }
    }

    public static string DatabaseDirectory
    {
      get { return ConfigurationManager.AppSettings["DatabaseDirectory"]; }
    }

    public static ReadOnlyDictionary<string, string> GetReplacementDictionary ()
    {
      return new ReadOnlyDictionary<string, string> (
          new Dictionary<string, string>
          {
              {
                  "C:\\Databases", DatabaseDirectory
              }
          });
    }
  }
}