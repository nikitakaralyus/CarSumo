﻿using CarSumo.DataModel.GameData.Accounts;
using Infrastructure.Installers.Factories;
using Zenject;

namespace Infrastructure.Installers.SubContainers
{
    public class AccountsInstaller : Installer<AccountsInstaller>
    {
        public override void InstallBindings()
        {
            BindAccountBinding();
            BindAccountSerialization();
        }

        private void BindAccountSerialization()
        {
            Container
                .Bind<IAccountSerialization>()
                .To<AccountSerialization>()
                .AsSingle();
        }

        private void BindAccountBinding()
        {
            Container
                .Bind<IAsyncAccountBinding>()
                .To<AddressableAccountBinding>()
                .FromFactory<AddressableAccountBindingFactory>()
                .AsSingle();
        }
    }
}