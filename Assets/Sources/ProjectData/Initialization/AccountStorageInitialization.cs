﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarSumo.DataModel.Accounts;
using CarSumo.DataModel.GameData.Accounts;
using DataModel.DataPersistence;
using DataModel.GameData.Accounts;
using DataModel.GameData.GameSave;
using Zenject;

namespace Infrastructure.Initialization
{
    public class AccountStorageInitialization : IAsyncInitializable
    {
        private readonly DiContainer _container;
        private readonly IAccountStorageConfiguration _configuration;
        private readonly IAsyncAccountBinding _accountBinding;
        private readonly IAsyncFileService _fileService;

        public AccountStorageInitialization(DiContainer container,
            IAccountStorageConfiguration configuration,
            IAsyncAccountBinding accountBinding,
            IAsyncFileService fileService)
        {
            _container = container;
            _configuration = configuration;
            _accountBinding = accountBinding;
            _fileService = fileService;
        }

        public async Task InitializeAsync()
        {
            SerializableAccountStorage serializableStorage = await LoadSerializableAccountStorageAsync() ?? EnsureCreated();
            GameAccountStorage storage = await InitializeGameAccountStorageAsync(serializableStorage);

            BindStorageInterfaces(storage);
            BindAccountStorageSave();
        }

        private SerializableAccountStorage EnsureCreated()
        {
            SerializableAccount activeAccount = new UnregisteredSerializableAccount();

            return new SerializableAccountStorage()
            {
                ActiveAccount = activeAccount,
                AllAccounts = new[] {activeAccount}
            };
        }

        private void BindAccountStorageSave()
        {
            _container
                .Bind<AccountStorageSave>()
                .FromNew()
                .AsSingle()
                .NonLazy();
            
            _container.Resolve<AccountStorageSave>();
        }

        private void BindStorageInterfaces(GameAccountStorage storage)
        {
            _container
                .BindInterfacesAndSelfTo<GameAccountStorage>()
                .FromInstance(storage)
                .AsSingle()
                .NonLazy();
        }

        private async Task<GameAccountStorage> InitializeGameAccountStorageAsync(SerializableAccountStorage storage)
        {
            Account activeAccount = await _accountBinding.ToAccountAsync(storage.ActiveAccount);
            IEnumerable<Account> allAccounts = await GetBoundAccountsAsync(storage);
            return new GameAccountStorage(allAccounts.First(account => account.Equals(activeAccount)), allAccounts);
        }

        private async Task<IEnumerable<Account>> GetBoundAccountsAsync(SerializableAccountStorage storage)
        {
            var accounts = new List<Account>();
            foreach (SerializableAccount serializableAccount in storage.AllAccounts)
            {
                Account account = await _accountBinding.ToAccountAsync(serializableAccount);
                accounts.Add(account);
            }

            return accounts;
        }

        private async Task<SerializableAccountStorage> LoadSerializableAccountStorageAsync()
        {
            string filePath = _configuration.AccountStorageFilePath;
            return await _fileService.LoadAsync<SerializableAccountStorage>(filePath);
        }
    }
}