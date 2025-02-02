﻿using CarSumo.Coroutines;
using CarSumo.Input;
using CarSumo.Teams.TeamChanging;
using CarSumo.Vehicles.Speedometers;
using Services.Timer.InGameTimer;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace CarSumo.Vehicles.Selector
{
    public class VehicleSelector : SerializedMonoBehaviour
    {
        [SerializeField] private VehicleSelectorData _data;
        
        private VehiclePicker _vehiclePicker;
        private VehicleBoostConfiguration _boost;
        private SelectorMoveHandler _moveHandler;

        private SelectorSpeedometer _speedometer;

        private Vehicle _selectedVehicle;

        private Camera _camera;
        private ISwipeScreen _screen;
        private ITeamPresenter _teamPresenter;
        private ITeamChange _teamChange;
        private ITimerOperations _timer;

        [Inject]
        private void Construct(Camera camera,
                               ISwipeScreen swipeScreen,
                               ITeamPresenter teamPresenter,
                               ITeamChange teamChange,
                               ITimerOperations timerOperations)
        {
            _camera = camera;
            _screen = swipeScreen;
            _teamPresenter = teamPresenter;
            _teamChange = teamChange;
            _timer = timerOperations;
        }

        public VehicleCollection LastValidVehicles { get; private set; }

        public void Initialize(VehiclePicker.IRules rules)
        {
            var executor = new CoroutineExecutor(this);

            LastValidVehicles = new VehicleCollection();
            _speedometer = new SelectorSpeedometer(_data);

            _vehiclePicker = new VehiclePicker(_camera, LastValidVehicles, _speedometer, rules);
            _boost = new VehicleBoostConfiguration(_camera, _speedometer);
            _moveHandler = new SelectorMoveHandler(_teamChange, _speedometer, _data, executor, _timer);

            ReceiveInput();
        }

        private void ReceiveInput()
        {
            _screen.Begun += OnScreenSwipeBegun;
            _screen.Swiping += OnScreenSwiping;
            _screen.Released += OnScreenSwipeReleased;

            _teamPresenter.ActiveTeam.Subscribe(_ => _boost.TurnOffActiveVehicle());
        }

        private void OnDisable()
        {
            _screen.Begun -= OnScreenSwipeBegun;
            _screen.Swiping -= OnScreenSwiping;
            _screen.Released -= OnScreenSwipeReleased;
        }

        private void OnScreenSwipeBegun(Swipe swipe)
        {
            if (_moveHandler.CanPerformMove() == false)
                return;

            _selectedVehicle = _vehiclePicker.GetVehicleBySwipe(swipe);
        }

        private void OnScreenSwiping(Swipe swipe)
        {
            if (_moveHandler.CanPerformMove() == false)
                return;

            if (_vehiclePicker.IsValid(_selectedVehicle) == false)
            {
                _selectedVehicle = _vehiclePicker.GetVehicleBySwipe(swipe);
                return;
            }

            if (swipe.Distance <= _data.MinSelectDistance)
                return;

            _boost.ConfigureBoost(_selectedVehicle, swipe);
        }

        private void OnScreenSwipeReleased(Swipe swipe)
        {
            if (_moveHandler.CanPerformMove() == false)
                return;

            if (_vehiclePicker.IsValid(_selectedVehicle) == false)
                return;

            _moveHandler.HandleVehiclePush(_selectedVehicle, swipe);
            _selectedVehicle = null;
        }
    }
}
