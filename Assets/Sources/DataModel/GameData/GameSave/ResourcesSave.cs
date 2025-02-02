﻿using System;
using System.Collections.Generic;
using CarSumo.DataModel.GameResources;
using DataModel.DataPersistence;
using UniRx;

namespace DataModel.GameData.GameSave
{
    public class ResourcesSave : IDisposable
    {
        private readonly IResourceStorage _storage;
        private readonly IResourcesConfiguration _configuration;
        private readonly IAsyncFileService _fileService;

        public ResourcesSave(IResourceStorage storage, 
	        				IResourcesConfiguration configuration,
	        				IResourceStorageMessages storageMessages,
	        				IAsyncFileService fileService)
        {
            _storage = storage;
            _configuration = configuration;
            _fileService = fileService;

            storageMessages
                .ObserveResourceChanged()
                .Subscribe(_ => Save());
        }
        
        public void Dispose() => Save();

        private void Save()
        {
            SerializableResources serializableResources = ToSerializableResources(_storage);
            string path = _configuration.ResourcesFilePath;
            _fileService.SaveAsync(serializableResources, path);
        }

        private SerializableResources ToSerializableResources(IResourceStorage storage)
        {
            int registeredResources = Enum.GetNames(typeof(ResourceId)).Length;
            
            // Dictionary<ResourceId, int> amounts = new Dictionary<ResourceId, int>(registeredResources);
            // Dictionary<ResourceId, int?> limits = new Dictionary<ResourceId, int?>(registeredResources);

            var amounts = new List<ResourceAmount>(registeredResources);
            var limits = new List<ResourceLimit>(registeredResources);

            for (int i = 0; i < registeredResources; i++)
            {
                ResourceId resource = (ResourceId)i;
                
                int amount = storage.GetResourceAmount(resource).Value;
                int? limit = storage.GetResourceLimit(resource).Value;
                
                amounts.Add(new ResourceAmount(resource, amount));
                limits.Add(new ResourceLimit(resource, limit));
            }

            return new SerializableResources
            {
                Amounts = amounts,
                Limits = limits
            };
        }
    }
}