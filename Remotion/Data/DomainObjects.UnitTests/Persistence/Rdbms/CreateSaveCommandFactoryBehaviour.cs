// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms;

public enum CreateSaveCommandFactoryBehaviour
{
  CreateBatchedSaveCommandFactory,
  CreateIndividualSaveCommandFactory,
  CreateExceptionThrowingSaveCommandFactory
}
