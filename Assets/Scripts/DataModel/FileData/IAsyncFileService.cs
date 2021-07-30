﻿using System.Threading.Tasks;

namespace DataModel.FileData
{
    public interface IAsyncFileService
    {
        Task<TModel> LoadAsync<TModel>(string path);
        Task SaveAsync<TModel>(TModel model, string path);
    }
}