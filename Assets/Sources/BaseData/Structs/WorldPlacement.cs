﻿using UnityEngine;

namespace CarSumo.Structs
{
    public readonly struct WorldPlacement
    {
        public Vector3 Position { get; }
        public Vector3 ForwardVector { get; }

        public WorldPlacement(Vector3 position, Vector3 forwardVector)
        {
            Position = position;
            ForwardVector = forwardVector;
        }
    }
}