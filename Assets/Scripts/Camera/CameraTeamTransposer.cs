﻿using System.Collections.Generic;
using CarSumo.Infrastructure.Services.TeamChangeService;
using CarSumo.Teams;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace CarSumo.Cameras
{
    public class CameraTeamTransposer : SerializedMonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private IDictionary<Team, float> _teamCameraPositions;

        private ITeamDefiner _previousTeamDefiner;
        private ITeamChangeService _teamChangeService;
        private CinemachineOrbitalTransposer _transposer;

        [Inject]
        private void Construct(ITeamDefiner previousTeamDefiner, ITeamChangeService teamChangeService)
        {
            _previousTeamDefiner = previousTeamDefiner;
            _teamChangeService = teamChangeService;
        }

        private void Awake()
        {
            _transposer = _camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        }

        private void OnEnable()
        {
            _teamChangeService.TeamChanged += ChangeCameraPosition;

            ChangeCameraPosition(false);
        }

        private void OnDisable()
        {
            _teamChangeService.TeamChanged -= ChangeCameraPosition;
        }

        private void ChangeCameraPosition()
        {
            ChangeCameraPosition(true);
        }

        private void ChangeCameraPosition(bool rememberPosition)
        {
            Team team = _teamChangeService.ActiveTeam;

            if (rememberPosition)
                RememberCameraPosition(team);

            _transposer.m_XAxis.Value = _teamCameraPositions[team];
        }

        private void RememberCameraPosition(Team team)
        {
            var previousTeam = _previousTeamDefiner.DefinePrevious(team);
            _teamCameraPositions[previousTeam] = _transposer.m_XAxis.Value;
        }
    }
}