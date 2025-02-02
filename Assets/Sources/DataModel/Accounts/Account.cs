﻿using System;
using DataModel.Vehicles;
using UniRx;

namespace CarSumo.DataModel.Accounts
{
    public class Account : IEquatable<Account>
    {
        public IReactiveProperty<string> Name { get; }
        public IReactiveProperty<Icon> Icon { get; }
        public IVehicleDeck VehicleDeck { get; }

        public Account(string name, Icon icon, IVehicleDeck vehicleDeck)
        {
            Name = new ReactiveProperty<string>(name);
            Icon = new ReactiveProperty<Icon>(icon);
            VehicleDeck = vehicleDeck;
        }

        public bool Equals(Account other)
        {
            if (other is null)
            {
                return false;
            }

            return string.Equals(Name.Value, other.Name.Value, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}