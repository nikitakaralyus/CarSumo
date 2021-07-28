﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarSumo.Players.Models;
using DataManagement.Players.Models;
using DataManagement.Players.Services;

namespace CarSumo.Menu.Models
{
    public class PlayerProfilesBinder : IPlayerProfilesProvider
    {
        private readonly IPlayerProfileBuilder _builder;
        private readonly PlayersDataService _dataService;

        public PlayerProfilesBinder(IPlayerProfileBuilder builder, PlayersDataService dataService, IPlayerProfilesUpdate update)
        {
            _builder = builder;
            _dataService = dataService;

            update.Updated += Bind;
        }

        public PlayerProfile SelectedPlayer { get; private set; }
        
        public IEnumerable<PlayerProfile> OtherPlayers { get; private set; }

        public async void Bind()
        {
            SelectedPlayer = await BuildSelectedPlayer(_dataService.StoredData);
            OtherPlayers = BuildPlayerProfilesExceptSelected(_dataService.StoredData);
        }

        private async Task<PlayerProfile> BuildSelectedPlayer(IPlayersRepository repository)
        {
            return await _builder.BuildFrom(repository.SelectedPlayer);
        }

        private IEnumerable<PlayerProfile> BuildPlayerProfilesExceptSelected(IPlayersRepository repository)
        {
            return repository.Players
                .Where(player => player != repository.SelectedPlayer)
                .Select(async player => await _builder.BuildFrom(player))
                .Select(task => task.Result);
        }
    }
}