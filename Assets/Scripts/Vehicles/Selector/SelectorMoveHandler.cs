﻿using System.Collections;
using CarSumo.Coroutines;
using CarSumo.Infrastructure.Services.TeamChangeService;
using CarSumo.Infrastructure.Services.TimerService;
using CarSumo.Input;
using CarSumo.Vehicles.Speedometers;
using UnityEngine;

namespace CarSumo.Vehicles.Selector
{
    public class SelectorMoveHandler
    {
        private readonly ITeamChangeService _changeService;
        private readonly IVehicleSpeedometer _speedometer;
        private readonly VehicleSelectorData _data;
        private readonly CoroutineExecutor _executor;
        private readonly ITimerService _timerService;

        private bool _isMovePerforming = false;

        public SelectorMoveHandler(
            ITeamChangeService changeService,
            IVehicleSpeedometer speedometer,
            VehicleSelectorData data,
            CoroutineExecutor executor,
            ITimerService timerService)
        {
            _changeService = changeService;
            _speedometer = speedometer;
            _data = data;
            _executor = executor;
            _timerService = timerService;
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
            _timerService.Stop();

            yield return new WaitForSeconds(_data.TimeForMove);

            _isMovePerforming = false;
            _changeService.ChangeOnNext();
        }
    }
}
