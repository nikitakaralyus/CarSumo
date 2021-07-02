﻿using CarSumo.Teams;
using CarSumo.Vehicles.Stats;

namespace CarSumo.Vehicles
{
    public interface IVehicle : IVehicleStatsProvider
    {
        void Upgrade();
        void Destroy();

        public class FakeVehicle : IVehicle
        {
            private readonly Team _team;

            public FakeVehicle(Team team)
            {
                _team = team;
            }

            public void Destroy()
            {
            }

            public VehicleStats GetStats()
            {
                return new VehicleStats(_team, 0.0f, 0.0f);
            }

            public void Upgrade()
            {
            }
        }
    }
}
