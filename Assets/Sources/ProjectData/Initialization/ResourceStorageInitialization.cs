﻿using System.Threading.Tasks;
using CarSumo.DataModel.GameResources;
using DataModel.DataPersistence;
using DataModel.GameData.GameSave;
using DataModel.GameData.Resources;
using DataModel.GameData.Resources.Binding;
using Zenject;

namespace Infrastructure.Initialization
{
    public class ResourceStorageInitialization : IAsyncInitializable
    {
        private readonly DiContainer _container;
        private readonly IAsyncFileService _fileService;
        private readonly IResourcesConfiguration _configuration;
        private readonly IResourceStorageBinding _storageBinding;
        private readonly IInitialResourceStorageProvider _initialResourceStorage;

        public ResourceStorageInitialization(DiContainer container,
                                              IAsyncFileService fileService,
                                              IResourcesConfiguration configuration,
                                              IResourceStorageBinding storageBinding,
                                              IInitialResourceStorageProvider initialResourceStorage)
        {
            _container = container;
            _fileService = fileService;
            _configuration = configuration;
            _storageBinding = storageBinding;
            _initialResourceStorage = initialResourceStorage;
        }
        
        public async Task InitializeAsync()
        {
            SerializableResources serializableResources = await LoadSerializableResourcesAsync();

            BindResourceStorageInterfaces(serializableResources is null
                ? EnsureCreated()
                : _storageBinding.BindFrom(serializableResources));
            
            BindResourcesSave();
        }

        private GameResourceStorage EnsureCreated() => _initialResourceStorage.GetInitialStorage();

        private void BindResourcesSave()
        {
            _container
                .Bind<ResourcesSave>()
                .FromNew()
                .AsSingle()
                .NonLazy();

            _container.Resolve<ResourcesSave>();
        }

        private void BindResourceStorageInterfaces(GameResourceStorage resourceStorage) =>
            _container
                .BindInterfacesAndSelfTo<GameResourceStorage>()
                .FromInstance(resourceStorage)
                .AsSingle()
                .NonLazy();

        private async Task<SerializableResources> LoadSerializableResourcesAsync()
        {
            string path = _configuration.ResourcesFilePath;
            return await _fileService.LoadAsync<SerializableResources>(path);
        }
    }
}