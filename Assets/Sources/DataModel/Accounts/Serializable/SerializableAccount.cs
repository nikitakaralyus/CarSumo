﻿using System.Collections.Generic;
using DataModel.Vehicles;

namespace CarSumo.DataModel.Accounts
{
    public class SerializableAccount
    {
        public string Name { get; set; }
        
        public string Icon { get; set; }

        public IEnumerable<Vehicle> VehicleLayout { get; set; }
    }
    
    public class UnregisteredSerializableAccount : SerializableAccount
    {
        public UnregisteredSerializableAccount()
        {
            Name = "Unregistered";
            Icon = null;
            VehicleLayout = new[] {Vehicle.Jeep, Vehicle.Jeep, Vehicle.Jeep};
        }
    }
}