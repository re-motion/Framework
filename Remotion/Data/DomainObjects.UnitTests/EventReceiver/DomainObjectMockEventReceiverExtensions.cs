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
using Moq;
using Moq.Language.Flow;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public static class DomainObjectMockEventReceiverExtensions
  {
    public static ISetup<IDomainObjectMockEventReceiver> SetupRelationChanging (
        this Mock<IDomainObjectMockEventReceiver> fluent,
        DomainObject sender,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      return fluent
          .Setup(
              mock => mock.RelationChanging(
                  sender,
                  It.Is<RelationChangingEventArgs>(
                      args =>
                          args.RelationEndPointDefinition == relationEndPointDefinition
                          && args.OldRelatedObject == oldRelatedObject
                          && args.NewRelatedObject == newRelatedObject)));
    }

    public static ISetup<IDomainObjectMockEventReceiver> SetupRelationChanging (
        this MockWrapper<IDomainObjectMockEventReceiver> fluent,
        DomainObject sender,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      return fluent
          .Setup(
              mock => mock.RelationChanging(
                  sender,
                  It.Is<RelationChangingEventArgs>(
                      args =>
                          args.RelationEndPointDefinition == relationEndPointDefinition
                          && args.OldRelatedObject == oldRelatedObject
                          && args.NewRelatedObject == newRelatedObject)));
    }

    public static ISetup<IDomainObjectMockEventReceiver> SetupRelationChanged (
        this Mock<IDomainObjectMockEventReceiver> fluent,
        DomainObject sender,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      return fluent
          .Setup(
              mock => mock.RelationChanged(
                  sender,
                  It.Is<RelationChangedEventArgs>(
                      args =>
                          args.RelationEndPointDefinition == relationEndPointDefinition
                          && args.OldRelatedObject == oldRelatedObject
                          && args.NewRelatedObject == newRelatedObject)));
    }

    public static ISetup<IDomainObjectMockEventReceiver> SetupRelationChanged (
        this MockWrapper<IDomainObjectMockEventReceiver> fluent,
        DomainObject sender,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      return fluent
          .Setup(
              mock => mock.RelationChanged(
                  sender,
                  It.Is<RelationChangedEventArgs>(
                      args =>
                          args.RelationEndPointDefinition == relationEndPointDefinition
                          && args.OldRelatedObject == oldRelatedObject
                          && args.NewRelatedObject == newRelatedObject)));
    }

    public static ISetup<IDomainObjectMockEventReceiver> SetupPropertyChanging (
        this MockWrapper<IDomainObjectMockEventReceiver> fluent,
        DomainObject sender,
        PropertyDefinition propertyDefinition,
        object oldValue,
        object newValue)
    {
      return fluent
          .Setup(
              mock => mock.PropertyChanging(
                  sender,
                  It.Is<PropertyChangeEventArgs>(
                      args => args.PropertyDefinition == propertyDefinition
                              && args.OldValue == oldValue
                              && args.NewValue == newValue)));
    }

    public static ISetup<IDomainObjectMockEventReceiver> SetupPropertyChanged (
        this MockWrapper<IDomainObjectMockEventReceiver> fluent,
        DomainObject sender,
        PropertyDefinition propertyDefinition,
        object oldValue,
        object newValue)
    {
      return fluent
          .Setup(
              mock => mock.PropertyChanged(
                  sender,
                  It.Is<PropertyChangeEventArgs>(
                      args => args.PropertyDefinition == propertyDefinition
                              && args.OldValue == oldValue
                              && args.NewValue == newValue)));
    }
  }
}
