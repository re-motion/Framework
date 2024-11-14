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
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// Provides methods that support isolated testing of mixins by initializing them with mock versions of their TTarget and TNext generic parameters.
  /// </summary>
  public static class MixinTargetMockUtility
  {
    /// <summary>
    /// Mocks the target of the given mixin instance by setting or replacing its <see cref="Mixin{TTarget}.Target"/> and
    /// <see cref="Mixin{TTarget,TNext}.Next"/> properties to/with the given mocks.
    /// </summary>
    /// <typeparam name="TTarget">The type of the mixin's TTarget parameter.</typeparam>
    /// <typeparam name="TNext">The type of the mixin's TNext parameter.</typeparam>
    /// <param name="mixin">The mixin whose target is to be mocked.</param>
    /// <param name="targetMock">The mock object to use for the mixin's <see cref="Mixin{TTarget}.Target"/> property.</param>
    /// <param name="nextMock">The mock object to use for the mixin's <see cref="Mixin{TTarget,TNext}.Next"/> property.</param>
    /// <remarks>
    /// <para>
    /// This method indirectly invokes the <see cref="Mixin{TTarget}.OnInitialized"/> method.
    /// </para>
    /// <para>
    /// Use this method if you already have a mixin instance. To create a new mixin with the given target mock, use the 
    /// <see cref="CreateMixinWithMockedTarget{TMixin,TTarget,TNext}"/> method.
    /// </para>
    /// </remarks>
    public static void MockMixinTarget<TTarget, TNext> (Mixin<TTarget, TNext> mixin, TTarget targetMock, TNext nextMock)
        where TTarget : class
        where TNext: class
    {
      ArgumentUtility.CheckNotNull("mixin", mixin);
      ArgumentUtility.CheckNotNull("targetMock", targetMock);
      ArgumentUtility.CheckNotNull("nextMock", nextMock);

      ((IInitializableMixin)mixin).Initialize(targetMock, nextMock);
    }

    /// <summary>
    /// Mocks the target of the given mixin instance by setting or replacing its <see cref="Mixin{TTarget}.Target"/> property to/with the given mocks.
    /// </summary>
    /// <typeparam name="TTarget">The type of the mixin's TTarget parameter.</typeparam>
    /// <param name="mixin">The mixin whose target is to be mocked.</param>
    /// <param name="targetMock">The mock object to use for the mixin's <see cref="Mixin{TTarget}.Target"/> property.</param>
    /// <remarks>
    /// <para>
    /// This method indirectly invokes the <see cref="Mixin{TTarget}.OnInitialized"/> method.
    /// </para>
    /// <para>
    /// Use this method if you already have a mixin instance. To create a new mixin with the given target mock, use the 
    /// <see cref="CreateMixinWithMockedTarget{TMixin,TTarget}"/> method.
    /// </para>
    /// </remarks>
    public static void MockMixinTarget<TTarget> (Mixin<TTarget> mixin, TTarget targetMock)
        where TTarget : class
    {
      ArgumentUtility.CheckNotNull("mixin", mixin);
      ArgumentUtility.CheckNotNull("targetMock", targetMock);

      ((IInitializableMixin)mixin).Initialize(targetMock, null);
    }

    /// <summary>
    /// Creates a mixin with a mocked target object.
    /// </summary>
    /// <typeparam name="TMixin">The type of mixin to create.</typeparam>
    /// <typeparam name="TTarget">The TTarget parameter of the mixin.</typeparam>
    /// <typeparam name="TNext">The TNext parameter of the mixin.</typeparam>
    /// <param name="targetMock">The mock object to use for the mixin's <see cref="Mixin{TTarget}.Target"/> property.</param>
    /// <param name="nextMock">The mock object to use for the mixin's <see cref="Mixin{TTarget,TNext}.Next"/> property.</param>
    /// <param name="args">The constructor arguments to be used when instantiating the mixin.</param>
    /// <returns>A mixin instance with the given mock objects as its <see cref="Mixin{TTarget}.Target"/> and <see cref="Mixin{TTarget,TNext}.Next"/>
    /// parameters.</returns>
    /// <remarks>
    /// <para>
    /// This method indirectly invokes the <see cref="Mixin{TTarget}.OnInitialized"/> method.
    /// </para>
    /// <para>
    /// This method cannot mock mixins with abstract methods, but it can mock subclasses of those mixins if they implement the abstract methods. If 
    /// you already have a mixin instance to be mocked, use the <see cref="MockMixinTarget{TTarget,TNext}"/> method instead.
    /// </para>
    /// </remarks>
    public static TMixin CreateMixinWithMockedTarget<TMixin, TTarget, TNext> (TTarget targetMock, TNext nextMock, params object[] args)
        where TTarget : class
        where TNext : class
        where TMixin : Mixin<TTarget, TNext>
    {
      ArgumentUtility.CheckNotNull("targetMock", targetMock);
      ArgumentUtility.CheckNotNull("nextMock", nextMock);
      ArgumentUtility.CheckNotNull("args", args);

      var mixin = ObjectFactory.Create<TMixin>(true, ParamList.CreateDynamic(args));
      MockMixinTarget(mixin, targetMock, nextMock);
      return mixin;
    }

    /// <summary>
    /// Creates a mixin with a mocked target object.
    /// </summary>
    /// <typeparam name="TMixin">The type of mixin to create.</typeparam>
    /// <typeparam name="TTarget">The TTarget parameter of the mixin.</typeparam>
    /// <param name="targetMock">The mock object to use for the mixin's <see cref="Mixin{TTarget}.Target"/> property.</param>
    /// <param name="args">The constructor arguments to be used when instantiating the mixin.</param>
    /// <returns>A mixin instance with the given mock objects as its <see cref="Mixin{TTarget}.Target"/> and <see cref="Mixin{TTarget,TNext}.Next"/>
    /// parameters.</returns>
    /// <remarks>
    /// <para>
    /// This method indirectly invokes the <see cref="Mixin{TTarget}.OnInitialized"/> method.
    /// </para>
    /// <para>
    /// This method cannot mock mixins with abstract methods, but it can mock subclasses of those mixins if they implement the abstract methods. If 
    /// you already have a mixin instance to be mocked, use the <see cref="MockMixinTarget{TTarget,TNext}"/> method instead.
    /// </para>
    /// </remarks>
    public static TMixin CreateMixinWithMockedTarget<TMixin, TTarget> (TTarget targetMock, params object[] args)
      where TTarget : class
      where TMixin : Mixin<TTarget>
    {
      ArgumentUtility.CheckNotNull("targetMock", targetMock);
      ArgumentUtility.CheckNotNull("args", args);

      var mixin = ObjectFactory.Create<TMixin>(true, ParamList.CreateDynamic(args));
      MockMixinTarget(mixin, targetMock);
      return mixin;
    }
  }
}
