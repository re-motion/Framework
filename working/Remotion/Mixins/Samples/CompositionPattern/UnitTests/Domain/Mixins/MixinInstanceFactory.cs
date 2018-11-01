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
using Remotion.Mixins.Samples.CompositionPattern.Core.Framework;
using Rhino.Mocks;

namespace Remotion.Mixins.Samples.CompositionPattern.UnitTests.Domain.Mixins
{
  public static class MixinInstanceFactory
  {
    public static TMixin CreateDomainObjectMixinWithTargetStub<TMixin, TTarget> (out TTarget targetStub)
        where TTarget : class, IDomainObject
        where TMixin : Mixin<TTarget>, IDomainObjectMixin
    {
      return CreateDomainObjectMixinWithTargetStub<TMixin, TTarget, TTarget> (out targetStub);
    }

    public static TMixin CreateDomainObjectMixinWithTargetStub<TMixin, TTargetInterface, TTargetImplementation> (out TTargetImplementation targetStub)
      where TTargetImplementation : class, IDomainObject, TTargetInterface
      where TMixin : Mixin<TTargetInterface>, IDomainObjectMixin where TTargetInterface: class
    {
      targetStub = MockRepository.GenerateStub<TTargetImplementation> ();
      targetStub.Stub (stub => stub.ID).Return (Guid.Empty);

      var fakeEvents = new DomainObjectEventSource (targetStub);
      targetStub.Stub (stub => stub.Events).Return (fakeEvents);

      var mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<TMixin, TTargetInterface> (targetStub);
      mixin.OnTargetReferenceInitializing ();
      return mixin;
    }
  }
}