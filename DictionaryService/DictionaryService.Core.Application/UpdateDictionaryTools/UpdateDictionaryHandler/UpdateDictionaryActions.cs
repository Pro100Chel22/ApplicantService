﻿using Common.Models;

namespace DictionaryService.Core.Application.UpdateDictionaryTools.UpdateDictionaryHandler
{
    public class UpdateDictionaryActions<TEntity, TExternalEntity>
    {
        public required Func<Task> BeforeActionsAsync { get; init; }
        public required Func<TEntity, TExternalEntity, bool> CompareKey { get; init; }
        public required Func<Task<List<TEntity>>> GetEntityAsync { get; init; }
        public required Func<Task<ExecutionResult<List<TExternalEntity>>>> GetExternalEntityAsync { get; init; }
        public required Func<TEntity, TExternalEntity, List<string>, Task<bool>> CheckBeforeUpdateEntityAsync { get; init; }
        public required Action<TEntity, TExternalEntity> UpdateEntity { get; init; }
        public required Func<TExternalEntity, List<string>, Task<bool>> CheckBeforeAddEntityAsync { get; init; }
        public required Func<TExternalEntity, TEntity> AddEntity { get; init; }
        public required Func<bool, TEntity, List<string>, Task<bool>> DeleteEntityAsync { get; init; }
    }
}
