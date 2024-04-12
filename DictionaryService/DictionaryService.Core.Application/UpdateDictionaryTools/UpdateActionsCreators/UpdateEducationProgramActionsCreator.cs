﻿using DictionaryService.Core.Application.DTOs;
using DictionaryService.Core.Application.UpdateDictionaryTools.UpdateActionsCreators.Base;
using DictionaryService.Core.Domain;
using DictionaryService.Core.Application.Interfaces.Repositories;
using DictionaryService.Core.Application.Interfaces.Services;
using Common.Models;

namespace DictionaryService.Core.Application.UpdateDictionaryTools.UpdateActionsCreators
{
    public class UpdateEducationProgramActionsCreator : UpdateActionsCreator<EducationProgram, EducationProgramExternalDTO>
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IEducationLevelRepository _educationLevelRepository;
        private readonly IEducationProgramRepository _educationProgramRepository;
        private readonly IExternalDictionaryService _externalDictionaryService;

        private List<EducationLevel>? educationLevelsCache = null;
        private List<Faculty>? facultiesCache = null;

        public UpdateEducationProgramActionsCreator(
            IFacultyRepository facultyRepository, IEducationLevelRepository educationLevelRepository, 
            IEducationProgramRepository educationProgramRepository, IExternalDictionaryService externalDictionaryService) 
        {
            _facultyRepository = facultyRepository;
            _educationLevelRepository = educationLevelRepository;
            _educationProgramRepository = educationProgramRepository;
            _externalDictionaryService = externalDictionaryService;
        }

        protected override async Task BeforeActionsAsync()
        {
            educationLevelsCache = await _educationLevelRepository.GetAllAsync();
            facultiesCache = await _facultyRepository.GetAllAsync();
        }

        protected override bool CompareKey(EducationProgram program, EducationProgramExternalDTO externalProgram)
            => program.Id == externalProgram.Id;

        protected override async Task<List<EducationProgram>> GetEntityAsync()
            => await _educationProgramRepository.GetAllAsync();

        protected override async Task<ExecutionResult<List<EducationProgramExternalDTO>>> GetExternalEntityAsync()
            => await _externalDictionaryService.GetEducationProgramAsync();

        protected override void UpdateEntity(EducationProgram program, EducationProgramExternalDTO externalProgram)
        {
            EducationLevel educationLevel = educationLevelsCache!.First(educationLevel => educationLevel.ExternalId == externalProgram.EducationLevel.Id);

            program.CreatedTime = externalProgram.CreateTime.ToUniversalTime();
            program.Name = externalProgram.Name;
            program.Code = externalProgram.Code;
            program.Language = externalProgram.Language;
            program.EducationForm = externalProgram.EducationForm;
            program.EducationLevelId = educationLevel.Id;
            program.FacultyId = externalProgram.Faculty.Id;
            program.Deprecated = false;
        }

        protected override EducationProgram AddEntity(EducationProgramExternalDTO externalProgram)
        {
            EducationLevel educationLevel = educationLevelsCache!.First(educationLevel => educationLevel.ExternalId == externalProgram.EducationLevel.Id);

            EducationProgram newEducationLevel = new()
            {
                Id = externalProgram.Id,
                CreatedTime = externalProgram.CreateTime.ToUniversalTime(),
                Name = externalProgram.Name,
                Code = externalProgram.Code,
                Language = externalProgram.Language,
                EducationForm = externalProgram.EducationForm,
                EducationLevelId = educationLevel.Id,
                FacultyId = externalProgram.Faculty.Id,
                Deprecated = false,
            };

            return newEducationLevel;
        }

        protected override async Task<bool> CheckBeforeUpdateEntityAsync(EducationProgram program, EducationProgramExternalDTO externalProgram, List<string> comments)
        {
            if (program.FacultyId == externalProgram.Faculty.Id) return true;
            return await CheckBeforeAddEntityAsync(externalProgram, comments);
        }

        protected override Task<bool> CheckBeforeAddEntityAsync(EducationProgramExternalDTO externalProgram, List<string> comments)
        {
            bool facultyExist = facultiesCache!.Any(faculty => faculty.Id == externalProgram.Faculty.Id);
            if (!facultyExist)
            {
                comments.Add($"The education program '{externalProgram.Name}' refers to a non-existent faculty '{externalProgram.Faculty.Name}'.");
            }

            bool educationLevelExist = educationLevelsCache!.Any(educationLevel => educationLevel.ExternalId == externalProgram.EducationLevel.Id);
            if (!educationLevelExist)
            {
                comments.Add($"The education program '{externalProgram.Name}' refers to a non-existent education level '{externalProgram.EducationLevel.Name}'.");
            }

            return Task.FromResult(facultyExist && educationLevelExist);
        }
    }
}
/*
 
            +++ List<EducationLevel> educationLevelsCache = await _educationLevelRepository.GetAllAsync();
            +++ List<Faculty> facultiesCache = await _facultyRepository.GetAllAsync();

            +++ CompareKey<EducationProgram, EducationProgramExternalDTO> compareKey = (program, externalProgram) => program.Id == externalProgram.Id;
            +++ GetEntityAsync<EducationProgram> getEntityAsync = _educationProgramRepository.GetAllAsync;
            +++ GetExternalEntityAsync<EducationProgramExternalDTO> getExternalEntityAsync = _externalDictionaryService.GetEducationProgramAsync;
            +++ OnUpdateEntity<EducationProgram, EducationProgramExternalDTO> onUpdateEntity = (program, externalProgram) =>
            {
                EducationLevel educationLevel = educationLevelsCache.First(educationLevel => educationLevel.ExternalId == externalProgram.EducationLevel.Id);

                program.CreatedTime = externalProgram.CreateTime.ToUniversalTime();
                program.Name = externalProgram.Name;
                program.Code = externalProgram.Code;
                program.Language = externalProgram.Language;
                program.EducationForm = externalProgram.EducationForm;
                program.EducationLevelId = educationLevel.Id;
                program.FacultyId = externalProgram.Faculty.Id;
                program.Deprecated = false;
            };
            +++ OnAddEntity<EducationProgram, EducationProgramExternalDTO> onAddEntity = (externalProgram) =>
            {
                EducationLevel educationLevel = educationLevelsCache.First(educationLevel => educationLevel.ExternalId == externalProgram.EducationLevel.Id);

                EducationProgram newEducationLevel = new()
                {
                    Id = externalProgram.Id,
                    CreatedTime = externalProgram.CreateTime.ToUniversalTime(),
                    Name = externalProgram.Name,
                    Code = externalProgram.Code,
                    Language = externalProgram.Language,    
                    EducationForm = externalProgram.EducationForm,
                    EducationLevelId = educationLevel.Id,
                    FacultyId = externalProgram.Faculty.Id,
                    Deprecated = false,
                };

                return newEducationLevel;
            };

            CheckBeforeAddEntityAsync<EducationProgramExternalDTO> checkBeforeAdd = (externalProgram, comments) =>
            {
                bool facultyExist = facultiesCache.Any(faculty => faculty.Id == externalProgram.Faculty.Id);
                if (!facultyExist)
                {
                    comments.Add($"The education program '{externalProgram.Name}' refers to a non-existent faculty '{externalProgram.Faculty.Name}'.");
                }

                bool educationLevelExist = educationLevelsCache.Any(educationLevel => educationLevel.ExternalId == externalProgram.EducationLevel.Id);
                if (!educationLevelExist)
                {
                    comments.Add($"The education program '{externalProgram.Name}' refers to a non-existent education level '{externalProgram.EducationLevel.Name}'.");
                }

                return Task.FromResult(facultyExist && educationLevelExist);
            };
            CheckBeforeUpdateEntityAsync<EducationProgram, EducationProgramExternalDTO > checkBeforeUpdate = async (program, externalProgram, comments) =>
            {
                if (program.FacultyId == externalProgram.Faculty.Id) return true;
                return await checkBeforeAdd(externalProgram, comments);
            };
 */