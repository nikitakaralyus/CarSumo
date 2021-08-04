﻿using System.Collections.Generic;
using CarSumo.Teams;
using CarSumo.Teams.TeamChanging;
using Cinemachine;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace CarSumo.Cameras
{
    public class CameraTeamTransposer : SerializedMonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private IDictionary<Team, float> _teamCameraPositions;

        private bool _rememberPosition = false;
        
        private ITeamDefiner _previousTeamDefiner;
        private ITeamPresenter _teamPresenter;
        private CinemachineOrbitalTransposer _transposer;

        [Inject]
        private void Construct(ITeamDefiner previousTeamDefiner, ITeamPresenter teamPresenter)
        {
            _previousTeamDefiner = previousTeamDefiner;
            _teamPresenter = teamPresenter;
        }

        private void Start()
        {
            _transposer = _camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            _teamPresenter.ActiveTeam.Subscribe(team => ChangeCameraPosition(team, _rememberPosition));
            _rememberPosition = true;
        }
        
        private void ChangeCameraPosition(Team team, bool rememberPosition)
        {
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