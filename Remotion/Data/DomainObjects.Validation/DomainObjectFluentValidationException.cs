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
using System.Linq;
using System.Runtime.Serialization;
using FluentValidation.Results;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  [Serializable]
  public class DomainObjectFluentValidationException : DomainObjectValidationException
  {
    private readonly DomainObject[] _affectedObjects;
    private readonly IReadOnlyCollection<ValidationFailure> _validationFailures;

    public DomainObjectFluentValidationException ([NotNull] IEnumerable<DomainObject> affectedObjects, [NotNull] IEnumerable<ValidationFailure> validationFailures, string errorMessage)
      : this (affectedObjects, validationFailures, errorMessage, null)
    {
    }

    public DomainObjectFluentValidationException ([NotNull] IEnumerable<DomainObject> affectedObjects, [NotNull] IEnumerable<ValidationFailure> validationFailures, string errorMessage, Exception inner)
        : base (errorMessage, inner)
    {
      ArgumentUtility.CheckNotNull ("affectedObjects", affectedObjects);
      ArgumentUtility.CheckNotNull ("validationFailures", validationFailures);
      
      _affectedObjects = affectedObjects.ToArray();
      _validationFailures = validationFailures.ToArray();
    }

    protected DomainObjectFluentValidationException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      _affectedObjects = (DomainObject[]) info.GetValue ("_affectedObjects", typeof (DomainObject[]));
      _validationFailures = (IReadOnlyCollection<ValidationFailure>) info.GetValue ("_validationFailures", typeof (IReadOnlyCollection<ValidationFailure>));
    }

    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData (info, context);
      info.AddValue ("_affectedObjects", _affectedObjects);
      info.AddValue ("_validationFailures", _validationFailures);
    }

    public override DomainObject[] AffectedObjects
    {
      get { return _affectedObjects; }
    }

    public IReadOnlyCollection<ValidationFailure> ValidationFailures
    {
      get { return _validationFailures; }
    }
  }
}