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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.UnionInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Queries
{
  /// <summary>
  /// Tests different types of UNION queries against a small inheritance hierarchy with <see cref="Car"/> at the top.
  /// Tests that different constellations of UNION queries are reconciled and execute correctly.
  /// Reconciliation is necessary if the query projections are not compatible.
  /// </summary>
  [TestFixture]
  public class UnionQueryTest : IntegrationTestBase
  {
    [Test]
    public void Union_Vehicle_Vehicle ()
    {
      var query = QueryFactory.CreateLinqQuery<Vehicle>().Select(e => e)
          .Union(QueryFactory.CreateLinqQuery<Vehicle>().Select(e => e));

      CheckQueryResult(
          query,
          DomainObjectIDs.Car1, DomainObjectIDs.SportsCar1, DomainObjectIDs.Truck1, DomainObjectIDs.Truck2,
          DomainObjectIDs.Motorcycle1, DomainObjectIDs.Chopper1, DomainObjectIDs.SportBike1);
    }

    [Test]
    public void Union_Car_Car ()
    {
      var query = QueryFactory.CreateLinqQuery<Car>().Select(e => e)
          .Union(QueryFactory.CreateLinqQuery<Car>().Select(e => e));

      CheckQueryResult(query, DomainObjectIDs.Car1, DomainObjectIDs.SportsCar1);
    }

    [Test]
    public void Union_Car_Vehicle ([Values(false, true)] bool reversed)
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>();
      var vehicleQuery = QueryFactory.CreateLinqQuery<Vehicle>();

      var query = reversed
          ? vehicleQuery.Union(carQuery.Select(e => e))
          : carQuery.Union(vehicleQuery.Select(e => e));

      CheckQueryResult(
          query,
          DomainObjectIDs.Car1, DomainObjectIDs.SportsCar1, DomainObjectIDs.Truck1, DomainObjectIDs.Truck2,
          DomainObjectIDs.Motorcycle1, DomainObjectIDs.Chopper1, DomainObjectIDs.SportBike1);
    }

    [Test]
    public void Union_SportsCar_Vehicle ([Values(false, true)] bool reversed)
    {
      var sportsCarQuery = QueryFactory.CreateLinqQuery<SportsCar>().Select(e => e);
      var vehicleQuery = QueryFactory.CreateLinqQuery<Vehicle>().Select(e => e);

      var query = reversed ? vehicleQuery.Union(sportsCarQuery) : sportsCarQuery.Union(vehicleQuery);

      CheckQueryResult(
          query,
          DomainObjectIDs.Car1, DomainObjectIDs.SportsCar1, DomainObjectIDs.Truck1, DomainObjectIDs.Truck2,
          DomainObjectIDs.Motorcycle1, DomainObjectIDs.Chopper1, DomainObjectIDs.SportBike1);
    }

    [Test]
    public void Union_Truck_Vehicle ([Values(false, true)] bool reversed)
    {
      var truckQuery = QueryFactory.CreateLinqQuery<Truck>().Select(e => e);
      var vehicleQuery = QueryFactory.CreateLinqQuery<Vehicle>().Select(e => e);

      var query = reversed ? vehicleQuery.Union(truckQuery) : truckQuery.Union(vehicleQuery);

      CheckQueryResult(
          query,
          DomainObjectIDs.Car1, DomainObjectIDs.SportsCar1, DomainObjectIDs.Truck1, DomainObjectIDs.Truck2,
          DomainObjectIDs.Motorcycle1, DomainObjectIDs.Chopper1, DomainObjectIDs.SportBike1);
    }

    [Test]
    public void Union_SportsCar_Car ([Values(false, true)] bool reversed)
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>().Select(e => e);
      var sportsCarQuery = QueryFactory.CreateLinqQuery<SportsCar>().Select(e => e);

      var query = reversed ? sportsCarQuery.Union(carQuery) : carQuery.Union(sportsCarQuery);

      CheckQueryResult(query, DomainObjectIDs.Car1, DomainObjectIDs.SportsCar1);
    }

    [Test]
    public void Union_Chopper_Motorcycle ([Values(false, true)] bool reversed)
    {
      var motorcycleQuery = QueryFactory.CreateLinqQuery<Motorcycle>().Select(e => e);
      var chopperQuery = QueryFactory.CreateLinqQuery<Chopper>().Select(e => e);

      var query = reversed ? chopperQuery.Union(motorcycleQuery) : motorcycleQuery.Union(chopperQuery);

      CheckQueryResult(query, DomainObjectIDs.Motorcycle1, DomainObjectIDs.Chopper1, DomainObjectIDs.SportBike1);
    }

    [Test]
    public void Union_Chopper_Motorcycle2 ([Values(false, true)] bool reversed)
    {
      var motorcycleQuery = QueryFactory.CreateLinqQuery<Motorcycle>().Select(e => e);
      var chopperQuery = QueryFactory.CreateLinqQuery<Chopper>().Select(e => e);

      var query = reversed ? chopperQuery.Union(motorcycleQuery) : motorcycleQuery.Union(chopperQuery);

      CheckQueryResult(query.Where(e => e.LicensePlate.StartsWith("")), DomainObjectIDs.Motorcycle1, DomainObjectIDs.Chopper1, DomainObjectIDs.SportBike1);
    }

    [Test]
    public void Union_Car_Truck ([Values(false, true)] bool reversed)
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>().Select(e => e);
      var truckQuery = QueryFactory.CreateLinqQuery<Truck>().Select(e => e);

      var query = reversed ? truckQuery.Union<Vehicle>(carQuery) : carQuery.Union<Vehicle>(truckQuery);

      CheckQueryResult(query, DomainObjectIDs.Car1, DomainObjectIDs.SportsCar1, DomainObjectIDs.Truck1, DomainObjectIDs.Truck2);
    }

    [Test]
    public void Union_Chopper_SportBike ([Values(false, true)] bool reversed)
    {
      var chopperQuery = QueryFactory.CreateLinqQuery<Chopper>().Select(e => e);
      var sportBikeQuery = QueryFactory.CreateLinqQuery<SportBike>().Select(e => e);

      var query = reversed ? sportBikeQuery.Union<Motorcycle>(chopperQuery) : chopperQuery.Union<Motorcycle>(sportBikeQuery);

      CheckQueryResult(query, DomainObjectIDs.Chopper1, DomainObjectIDs.SportBike1);
    }

    [Test]
    public void Union_SportsCar_Truck ([Values(false, true)] bool reversed)
    {
      var sportsCarQuery = QueryFactory.CreateLinqQuery<SportsCar>().Select(e => e);
      var truckQuery = QueryFactory.CreateLinqQuery<Truck>().Select(e => e);

      var query = reversed ? truckQuery.Union<Vehicle>(sportsCarQuery) : sportsCarQuery.Union<Vehicle>(truckQuery);

      CheckQueryResult(query, DomainObjectIDs.SportsCar1, DomainObjectIDs.Truck1, DomainObjectIDs.Truck2);
    }

    [Test]
    public void Union_SportsCar_Chopper ([Values(false, true)] bool reversed)
    {
      var sportsCarQuery = QueryFactory.CreateLinqQuery<SportsCar>().Select(e => e);
      var chopperQuery = QueryFactory.CreateLinqQuery<Chopper>().Select(e => e);

      var query = reversed ? chopperQuery.Union<Vehicle>(sportsCarQuery) : sportsCarQuery.Union<Vehicle>(chopperQuery);

      CheckQueryResult(query, DomainObjectIDs.SportsCar1, DomainObjectIDs.Chopper1);
    }

    [Test]
    public void Union_Car_VehicleOfTypeCar ([Values(false, true)] bool reversed)
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>().Select(e => e);
      var vehicleOfTypeCarQuery = QueryFactory.CreateLinqQuery<Vehicle>().OfType<Car>().Select(e => e);

      var query = reversed ? vehicleOfTypeCarQuery.Union(carQuery) : carQuery.Union(vehicleOfTypeCarQuery);

      CheckQueryResult(query, DomainObjectIDs.Car1, DomainObjectIDs.SportsCar1);
    }

    [Test]
    public void Union_Truck_VehicleOfTypeTruck ([Values(false, true)] bool reversed)
    {
      var truckQuery = QueryFactory.CreateLinqQuery<Truck>().Select(e => e);
      var vehicleOfTypeTruckQuery = QueryFactory.CreateLinqQuery<Vehicle>().OfType<Truck>().Select(e => e);

      var query = reversed ? vehicleOfTypeTruckQuery.Union(truckQuery) : truckQuery.Union(vehicleOfTypeTruckQuery);

      CheckQueryResult(query, DomainObjectIDs.Truck1, DomainObjectIDs.Truck2);
    }

    [Test]
    public void Union_SportsCar_VehicleOfTypeSportsCar ([Values(false, true)] bool reversed)
    {
      var sportsCarQuery = QueryFactory.CreateLinqQuery<SportsCar>().Select(e => e);
      var vehicleOfTypeSportsCarQuery = QueryFactory.CreateLinqQuery<Vehicle>().OfType<SportsCar>().Select(e => e);

      var query = reversed
          ? vehicleOfTypeSportsCarQuery.Union(sportsCarQuery)
          : sportsCarQuery.Union(vehicleOfTypeSportsCarQuery);

      CheckQueryResult(query, DomainObjectIDs.SportsCar1);
    }

    [Test]
    public void Union_Chopper_VehicleOfTypeChopper ([Values(false, true)] bool reversed)
    {
      var chopperQuery = QueryFactory.CreateLinqQuery<Chopper>().Select(e => e);
      var vehicleOfTypeChopperQuery = QueryFactory.CreateLinqQuery<Vehicle>().OfType<Chopper>().Select(e => e);

      var query = reversed
          ? vehicleOfTypeChopperQuery.Union(chopperQuery)
          : chopperQuery.Union(vehicleOfTypeChopperQuery);

      CheckQueryResult(query, DomainObjectIDs.Chopper1);
    }

    [Test]
    public void Union_Chopper_MotorcycleOfTypeChopper ([Values(false, true)] bool reversed)
    {
      var chopperQuery = QueryFactory.CreateLinqQuery<Chopper>().Select(e => e);
      var motorcycleOfTypeChopperQuery = QueryFactory.CreateLinqQuery<Motorcycle>().OfType<Chopper>().Select(e => e);

      var query = reversed
          ? motorcycleOfTypeChopperQuery.Union(chopperQuery)
          : chopperQuery.Union(motorcycleOfTypeChopperQuery);

      CheckQueryResult(query, DomainObjectIDs.Chopper1);
    }

    [Test]
    public void Union_SportBike_MotorcycleOfTypeSportBike ([Values(false, true)] bool reversed)
    {
      var sportBikeQuery = QueryFactory.CreateLinqQuery<SportBike>().Select(e => e);
      var motorcycleOfTypeSportBikeQuery = QueryFactory.CreateLinqQuery<Motorcycle>().OfType<SportBike>().Select(e => e);

      var query = reversed
          ? motorcycleOfTypeSportBikeQuery.Union(sportBikeQuery)
          : sportBikeQuery.Union(motorcycleOfTypeSportBikeQuery);

      CheckQueryResult(query, DomainObjectIDs.SportBike1);
    }

    [Test]
    public void Union_Car_VehicleOfTypeCar_FilterByTowedBy ()
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>().Select(e => e);
      var vehicleOfTypeCarQuery = QueryFactory.CreateLinqQuery<Vehicle>().OfType<Car>().Select(e => e);

      var query = carQuery.Union(vehicleOfTypeCarQuery).Where(v => v.TowedBy != null);

      CheckQueryResult(query, DomainObjectIDs.Car1);
    }

    [Test]
    public void Union_Car_VehicleOfTypeCar_SelectTowedBy ()
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>().Select(e => e);
      var vehicleOfTypeCarQuery = QueryFactory.CreateLinqQuery<Vehicle>().OfType<Car>().Select(e => e);

      var query = carQuery.Union(vehicleOfTypeCarQuery)
          .Where(v => v.ID == DomainObjectIDs.Car1)
          .Select(v => v.TowedBy);

      CheckQueryResult(query, DomainObjectIDs.Truck1);
    }

    [Test]
    public void Union_Car_Truck_Motorcycle_ThreeWayFlat ()
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>().Select(e => e);
      var truckQuery = QueryFactory.CreateLinqQuery<Truck>().Select(e => e);
      var motorcycleQuery = QueryFactory.CreateLinqQuery<Motorcycle>().Select(e => e);

      var query = carQuery.Union<Vehicle>(truckQuery).Union(motorcycleQuery);

      CheckQueryResult(
          query,
          DomainObjectIDs.Car1, DomainObjectIDs.SportsCar1, DomainObjectIDs.Truck1, DomainObjectIDs.Truck2,
          DomainObjectIDs.Motorcycle1, DomainObjectIDs.Chopper1, DomainObjectIDs.SportBike1);
    }

    [Test]
    public void Union_Car_VehicleOfTypeCar_Truck_ThreeWayFlat_MixedNarrowingAndWidening ()
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>().Select(e => e);
      var vehicleOfTypeCarQuery = QueryFactory.CreateLinqQuery<Vehicle>().OfType<Car>().Select(e => e);
      var truckQuery = QueryFactory.CreateLinqQuery<Truck>().Select(e => e);

      var query = carQuery.Union(vehicleOfTypeCarQuery).Union<Vehicle>(truckQuery);

      CheckQueryResult(query, DomainObjectIDs.Car1, DomainObjectIDs.SportsCar1, DomainObjectIDs.Truck1, DomainObjectIDs.Truck2);
    }

    [Test]
    [Ignore("Nested UNION queries are not unified as it is currently out of scope. They should be written as a flat list.")]
    public void Union_Car_Truck_Motorcycle_ThreeWayNested_KnownLimitation ()
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>().Select(e => e);
      var truckQuery = QueryFactory.CreateLinqQuery<Truck>().Select(e => e);
      var motorcycleQuery = QueryFactory.CreateLinqQuery<Motorcycle>().Select(e => e);

      var query = carQuery.Union(truckQuery.Union<Vehicle>(motorcycleQuery));

      CheckQueryResult(
          query,
          DomainObjectIDs.Car1, DomainObjectIDs.SportsCar1, DomainObjectIDs.Truck1, DomainObjectIDs.Truck2,
          DomainObjectIDs.Motorcycle1, DomainObjectIDs.Chopper1, DomainObjectIDs.SportBike1);
    }

    [Test]
    public void Union_Car_Truck_OrderByLicensePlate_Take ()
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>().Select(e => e);
      var truckQuery = QueryFactory.CreateLinqQuery<Truck>().Select(e => e);

      var query = carQuery.Union<Vehicle>(truckQuery).OrderBy(v => v.LicensePlate).Take(2);

      CheckOrderedQueryResult(query, DomainObjectIDs.Car1, DomainObjectIDs.Truck1);
    }

    [Test]
    public void Union_Car_VehicleOfTypeCar_WhereManufacturer_SelectLicensePlate ()
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>().Select(e => e);
      var vehicleOfTypeCarQuery = QueryFactory.CreateLinqQuery<Vehicle>().OfType<Car>().Select(e => e);

      var query = carQuery.Union(vehicleOfTypeCarQuery)
          .Where(v => v.Manufacturer == "Ferrari")
          .Select(v => v.LicensePlate);

      Assert.That(query.ToArray(), Is.EqualTo(new[] { "W-99887F" }));
    }

    [Test]
    public void Union_Car_Truck_AnonymousProjection ()
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>().Select(v => new { v.Manufacturer, v.LicensePlate });
      var truckQuery = QueryFactory.CreateLinqQuery<Truck>().Select(v => new { v.Manufacturer, v.LicensePlate });

      var query = carQuery.Union(truckQuery);

      Assert.That(
          query.ToArray(),
          Is.EquivalentTo(
              new[]
              {
                  new { Manufacturer = "Toyota", LicensePlate = "W-12345A" },
                  new { Manufacturer = "Ferrari", LicensePlate = "W-99887F" },
                  new { Manufacturer = "Volvo", LicensePlate = "W-55501T" },
                  new { Manufacturer = "Scania", LicensePlate = "W-55502T" }
              }));
    }

    [Test]
    public void Union_Car_Truck_Motorcycle_AnonymousProjection_ThreeWay ()
    {
      var carQuery = QueryFactory.CreateLinqQuery<Car>().Select(v => new { v.Manufacturer });
      var truckQuery = QueryFactory.CreateLinqQuery<Truck>().Select(v => new { v.Manufacturer });
      var motorcycleQuery = QueryFactory.CreateLinqQuery<Motorcycle>().Select(v => new { v.Manufacturer });

      var query = carQuery.Union(truckQuery).Union(motorcycleQuery);

      Assert.That(
          query.ToArray().Select(x => x.Manufacturer),
          Is.EquivalentTo(new[] { "Toyota", "Ferrari", "Volvo", "Scania", "Honda", "Harley-Davidson", "Kawasaki" }));
    }
  }
}
