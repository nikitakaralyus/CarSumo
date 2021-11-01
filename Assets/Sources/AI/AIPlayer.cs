﻿using System.Threading;
using AI.StateMachine.Common;
using AI.StateMachine.States;
using AI.Structures;
using CarSumo.Teams;
using CarSumo.Teams.TeamChanging;
using CarSumo.Units.Tracking;
using Sources.BaseData.Operations;
using UniRx;
using UnityEngine;
using Zenject;

namespace Sources.AI
{
	public class AIPlayer : MonoBehaviour
	{
		[SerializeField] private int _thinkMillisecondsDelay;
		[SerializeField] private float _prepareDuration;
		
		private const Team BotTeam = Team.Blue;
		private const Team EnemyTeam = Team.Red;

		private readonly CancellationTokenSource _source = new CancellationTokenSource();
		
		[Inject]
		private void Construct(ITeamChange teamChange, IVehicleTracker tracker, ITeamPresenter teamPresenter)
		{
			var transfer = new PairTransfer();

			var stateMachine = new AIStateMachine(new IAsyncState[]
			{
				new AIThinkDelayState(_thinkMillisecondsDelay),
				new AISelectTargetState(tracker, transfer, BotTeam, EnemyTeam),
				new AIPrepareState(transfer, new UnityAsyncTimeOperationPerformer(), _prepareDuration),
				new AIThinkDelayState(_thinkMillisecondsDelay),
				new AIDriveOnTargetState(transfer),
				new AICompleteMoveState(teamChange, transfer)
			});

			teamPresenter.ActiveTeam.Subscribe(team =>
			{
				if (team == BotTeam)
					stateMachine.RunAsync(_source.Token);
			});
		}

		private void OnDisable()
		{
			_source.Cancel();
		}
	}
}