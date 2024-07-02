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
using Remotion.BuildScript.Test;

namespace Customizations;

public class SqlServers : TestDimension
{
  public static readonly SqlServers NoDB = new(nameof(NoDB));
  public static readonly SqlServers SqlServerDefault = new(nameof(SqlServerDefault));
  public static readonly SqlServers SqlServer2016 = new(nameof(SqlServer2016), 2016);
  public static readonly SqlServers SqlServer2017 = new(nameof(SqlServer2017), 2017);
  public static readonly SqlServers SqlServer2019 = new(nameof(SqlServer2019), 2019);
  public static readonly SqlServers SqlServer2022 = new(nameof(SqlServer2022), 2022);

  public bool HasSpecificVersion { get; }

  public int Version { get; }

  public SqlServers (string value)
      : base(value)
  {
    HasSpecificVersion = false;
  }

  public SqlServers (string value, int version)
      : base(value)
  {
    HasSpecificVersion = true;
    Version = version;
  }
}
