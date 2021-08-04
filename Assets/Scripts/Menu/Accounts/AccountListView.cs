﻿using System;
using System.Threading.Tasks;
using CarSumo.DataModel.Accounts;
using CarSumo.DataModel.GameResources;
using Services.Instantiate;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Menu.Accounts
{
    public class AccountListView : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject _accountViewPrefab;
        [SerializeField] private AssetReferenceGameObject _blankAccountViewPrefab;
        [SerializeField] private Transform _itemsRoot;

        private IAsyncInstantiation _instantiation;
        private IAccountStorage _accountStorage;
        private IResourceStorage _resourceStorage;

        [Inject]
        private void Construct(IAsyncInstantiation instantiation, IAccountStorage accountStorage,
            IResourceStorage resourceStorage)
        {
            _instantiation = instantiation;
            _accountStorage = accountStorage;
            _resourceStorage = resourceStorage;
        }

        private async void Start()
        {
            await CreateAccountViews(_itemsRoot);
            await CreateBlankAccountViews(_itemsRoot);
        }

        private async Task CreateAccountViews(Transform root)
        {
            if (AreAccountsFitIntoLimit(_resourceStorage, _accountStorage) == false)
            {
                throw new InvalidOperationException("The allowed number of accounts has been exceeded");
            }

            foreach (Account account in _accountStorage.AllAccounts)
            {
                AccountListItem listItem = await _instantiation.InstantiateAsync<AccountListItem>(_accountViewPrefab, root);
                listItem.Initialize(account);
            }
        }

        private async Task CreateBlankAccountViews(Transform root)
        {
            int blanksCount = CountBlankAccounts(_resourceStorage, _accountStorage);
            for (int i = 0; i < blanksCount; i++)
            {
                await _instantiation.InstantiateAsync<BlankAccountListView>(_blankAccountViewPrefab, root);
            }
        }

        private int CountBlankAccounts(IResourceStorage resourceStorage, IAccountStorage accountStorage)
        {
            IReadOnlyReactiveProperty<int?> slotsLimit = resourceStorage.GetResourceLimit(ResourceId.AccountSlots);

            if (slotsLimit.HasValue == false)
            {
                throw new InvalidOperationException("Slots limits should be limited");
            }
            
            return slotsLimit.Value.Value - accountStorage.AllAccounts.Count;
        }

        private bool AreAccountsFitIntoLimit(IResourceStorage resourceStorage, IAccountStorage accountStorage)
        {
            IReadOnlyReactiveProperty<int?> slotsLimit = resourceStorage.GetResourceLimit(ResourceId.AccountSlots);

            if (slotsLimit.HasValue == false)
            {
                throw new InvalidOperationException("Slots limits should be limited");
            }
            
            return slotsLimit.Value >= accountStorage.AllAccounts.Count;
        }
    }
}