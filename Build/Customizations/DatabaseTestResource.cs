// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using Customizations.SqlServer;
using Remotion.BuildScript.Test;

namespace Customizations;

public class DatabaseTestResource : ITestResource
{
  public string Name { get; }

  public Databases Database { get; }

  private readonly ISqlDockerImage _sqlDockerImage;

  public DatabaseTestResource (
      string name,
      Databases database,
      ISqlDockerImage sqlDockerImage)
  {
    Name = name;
    Database = database;
    _sqlDockerImage = sqlDockerImage;
  }

  public void Dispose ()
  {
    _sqlDockerImage.Dispose();
  }

  public string GetConnectionString ()
  {
    return _sqlDockerImage.GetConnectionString();
  }
}
