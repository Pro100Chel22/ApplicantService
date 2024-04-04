﻿using ApplicantService.Core.Application.DTOs;
using Common.Models;

namespace ApplicantService.Core.Application.Interfaces.Services
{
    public interface IDocumentService
    {
        Task<ExecutionResult<List<DocumentInfo>>> GetApplicantDocumentsAsync(Guid applicantId);
        Task<ExecutionResult> DeleteApplicantDocumentAsync(Guid documentId, Guid applicantId);

        Task<ExecutionResult<PassportInfo>> GetApplicantPassportAsync(Guid applicantId);
        Task<ExecutionResult> UpdateApplicantPassportAsync(EditAddPassportInfo documentInfo, Guid applicantId);
        Task<ExecutionResult> AddApplicantPassportAsync(EditAddPassportInfo documentInfo, Guid applicantId);

        Task<ExecutionResult<EducationDocumentInfo>> GetApplicantEducationDocumentAsync(Guid documentId, Guid applicantId);
        Task<ExecutionResult> UpdateApplicantEducationDocumentAsync(Guid documentId, Guid applicantId, EditAddEducationDocumentInfo documentInfo);
        Task<ExecutionResult> AddApplicantEducationDocumentAsync(Guid applicantId, EditAddEducationDocumentInfo documentInfo);
    }
}
