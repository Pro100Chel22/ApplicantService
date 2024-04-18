﻿using DictionaryService.Core.Domain;
using Common.Repositories;

namespace DictionaryService.Core.Application.UpdateDictionaryTools.UpdateDictionaryHandler
{
    public class UpdateAndAddDictionaryActions<TEntity, TExternalEntity> where TEntity : BaseDictionaryEntity
    {
        public required IBaseRepository<TEntity> Repository { get; set; }

        public required Func<TEntity, TExternalEntity, bool> CompareKey { get; init; }
        public required Func<TEntity, TExternalEntity, List<string>, Task<bool>> CheckBeforeUpdateEntityAsync { get; init; }
        public required Func<TEntity, TExternalEntity, bool> UpdateEntity { get; init; }
        public required Func<TExternalEntity, List<string>, Task<bool>> CheckBeforeAddEntityAsync { get; init; }
        public required Func<TExternalEntity, TEntity> AddEntity { get; init; }
    }
}
