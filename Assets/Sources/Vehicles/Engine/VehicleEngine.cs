﻿using System;
using System.Collections;
using AdvancedAudioSystem;
using CarSumo.Coroutines;
using CarSumo.Vehicles.Speedometers;
using CarSumo.Vehicles.Stats;
using UniRx;
using UnityEngine;

namespace CarSumo.Vehicles.Engine
{
    public class VehicleEngine : MonoBehaviour, IVehicleEngine
    {
        [Header("Components")]
        [SerializeField] private VehicleEngineParticles _particles;
        [SerializeField] private VehicleEngineSound _engineSound;
        [SerializeField] private MonoAudioCuePlayer _hornSound;

        [Header("Preferences")] 
        [SerializeField] private bool _invertedForwardVector = true;
        
        private Rigidbody _rigidbody;
        private CoroutineExecutor _executor;
        private MagnitudeSpeedometer _speedometer;

        private IVehicleStatsProvider _statsProvider;
        private IDisposable _speedUpRoutine;

        public void Initialize(IVehicleStatsProvider statsProvider, Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
            _statsProvider = statsProvider;
            
            _executor = new CoroutineExecutor(this);
            _speedometer = new MagnitudeSpeedometer(rigidbody, _executor);
        }

        private VehicleStats Stats => _statsProvider.GetStats();

        private void OnDisable()
        {
	        _speedUpRoutine?.Dispose();
        }

        public void TurnOn(IVehicleSpeedometer speedometer)
        {
            _engineSound.PlayUntil(() => false, speedometer);
            _particles.EmitEngineTurnedOnParticles(speedometer);
        }

        public void TurnOff()
        {
            _engineSound.Stop();
            _particles.StopAllParticles();
        }

        public void SpeedUp(float timeModifier)
        {
	        _speedUpRoutine?.Dispose();

	        _speedUpRoutine = Observable
		        .FromMicroCoroutine(() => ConfigureVelocity(timeModifier, Stats))
		        .Subscribe();

	        _speedometer.StartCalculatingPowerPercentage();
	        
	        _engineSound.Stop();
	        _engineSound.PlayUntil(IsVehicleStopped, _speedometer);

            _particles.StopAllParticles();
            _particles.EmitExhaustParticlesUntil(IsVehicleStopped);

            _hornSound.Play();
        }

        public bool Stopped => IsVehicleStopped();

        private IEnumerator ConfigureVelocity(float timeModifer, VehicleStats stats)
        {
	        timeModifer = Mathf.Clamp01(timeModifer);
	        
	        float enterTime = Time.time;
	        float timePassed = 0.0f;
	        Vector3 direction = transform.forward;

	        float drivingTime = stats.NormalizedDrivingTime.Evaluate(timeModifer);
	        
	        while (Time.time <= enterTime + drivingTime)
	        {
		        Vector3 velocity = direction * GetDrivingSpeedValue(stats, timePassed, drivingTime);

		        _rigidbody.velocity = ProcessVelocityDirection(velocity);
		        
		        timePassed += Time.deltaTime;
		        
		        yield return null;
	        }
        }

        private bool IsVehicleStopped()
        {
            return _rigidbody.velocity.magnitude == 0.0f;
        }

        private Vector3 ProcessVelocityDirection(Vector3 originalVelocity)
        {
	        return _invertedForwardVector ? -originalVelocity : originalVelocity;
        }

        private float GetDrivingSpeedValue(VehicleStats stats, float timePassed, float drivingTime)
        {
	        return stats.NormalizedDrivingSpeed.Evaluate(timePassed / drivingTime);
        }
    }
}
