﻿using UnityEngine;

namespace CarSumo.Structs
{
    [CreateAssetMenu(fileName = "Input Settings", menuName = "CarSumo/Settings/Input", order = 0)]
    public class InputSettings : ScriptableObject
    {
        [SerializeField] private float _swipeDeltaDivider;

        public float SwipeDeltaDivider => _swipeDeltaDivider;
    }
}