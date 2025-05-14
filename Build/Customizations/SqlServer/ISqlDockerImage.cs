// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;

namespace Customizations.SqlServer;

public interface ISqlDockerImage : IDisposable
{
  string GetConnectionString ();
}
