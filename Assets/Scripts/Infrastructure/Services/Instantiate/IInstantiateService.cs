﻿using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CarSumo.Infrastructure.Services.Instantiate
{
    public interface IInstantiateService
    {
        Task<T> InstantiateAsync<T>(AssetReference reference, Transform parent = null) where T : Component;
    }
}