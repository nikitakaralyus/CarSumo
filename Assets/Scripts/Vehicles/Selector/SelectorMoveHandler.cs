﻿using System.Collections;
using CarSumo.Coroutines;
using CarSumo.Input;
using CarSumo.Teams.TeamChanging;
using CarSumo.Vehicles.Speedometers;
using Services.Timer;
using UnityEngine;

namespace CarSumo.Vehicles.Selector
{
    public class SelectorMoveHandler
    {
        private readonly ITeamChange _teamChange;
        private readonly IVehicleSpeedometer _speedometer;
        private readonly VehicleSelectorData _data;
        private readonly CoroutineExecutor _executor;
        private readonly ITimerOperations _timer;

        private bool _isMovePerforming = false;

        public SelectorMoveHandler(ITeamChange teamChange,
                                   IVehicleSpeedometer speedometer,
                                   VehicleSelectorData data,
                                   CoroutineExecutor executor,
                                   ITimerOperations timer)
        {
            _teamChange = teamChange;
            _speedometer = speedometer;
            _data = data;
            _executor = executor;
            _timer = timer;
        }

        public void HandleVehiclePush(Vehicle vehicle, SwipeData swipeData)
        {
            if (_isMovePerforming)
                return;

            if (_speedometer.PowerPercentage <= _data.CancelDistancePercent)
            {
                vehicle.Engine.TurnOff();
                return;
            }

            var forceModifier = CalculateForceMultiplier(swipeData);
            vehicle.Engine.SpeedUp(forceModifier);
            _executor.StartCoroutine(PerformMove());
        }

        public bool CanPerformMove()
        {
            return _isMovePerforming == false;
        }

        private float CalculateForceMultiplier(SwipeData swipeData)
        {
            var clampedDistance = Mathf.Clamp(swipeData.Distance, _data.MinSelectDistance, _data.MaxSelectDistance);

            var part = (clampedDistance - _data.MinSelectDistance) / (_data.MaxSelectDistance - _data.MinSelectDistance);

            return _data.MaxAccelerationMultiplier * part;
        }

        private IEnumerator PerformMove()
        {
            _isMovePerforming = true;
            _timer.Stop();

            yield return new WaitForSeconds(_data.TimeForMove);

            _isMovePerforming = false;
            _teamChange.ChangeOnNextTeam();
        }
    }
}
