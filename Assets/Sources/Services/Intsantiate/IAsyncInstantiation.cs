﻿using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.Instantiate
{
    public interface IAsyncInstantiation
    {
        Task<T> InstantiateAsync<T>(AssetReferenceGameObject asset, Transform parent = null) where T : Component;
    }
}