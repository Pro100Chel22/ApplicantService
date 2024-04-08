﻿using Microsoft.AspNetCore.Mvc;
using Common.Attributes;
using Common.Models;
using Common.DTOs;
using Common.Helpers;

namespace Common.Controllers
{
    [ApiController]
    [ValidateModelState]
    public abstract class BaseController : ControllerBase
    {
        protected BadRequestObjectResult BadRequest(ExecutionResult executionResult, string? otherMassage = null)
        {
            return BadRequest(new ErrorResponse()
            {
                Title = otherMassage ?? "One or more errors occurred.",
                Status = 400,
                Errors = executionResult.Errors,
            });
        }

        protected async Task<ActionResult<TResult>> ExecutionResultHandlerAsync<TResult>(OperationCallbackAsync<TResult> operation)
        {
            if (!HttpContext.TryGetUserId(out Guid userId))
            {
                return BadRequest(new ExecutionResult("UnknowError", "Unknow error"));
            }

            ExecutionResult<TResult> response = await operation(userId);

            if (!response.IsSuccess) return BadRequest(response);
            return Ok(response.Result!);
        }

        protected async Task<ActionResult> ExecutionResultHandlerAsync(OperationCallbackAsync operation)
        {
            if (!HttpContext.TryGetUserId(out Guid userId))
            {
                return BadRequest(new ExecutionResult("UnknowError", "Unknow error"));
            }

            ExecutionResult response = await operation(userId);

            if (!response.IsSuccess) return BadRequest(response);
            return NoContent();
        }


        protected delegate Task<ExecutionResult<TResult>> OperationCallbackAsync<TResult>(Guid userId);
        protected delegate Task<ExecutionResult> OperationCallbackAsync(Guid userId);
    }
}
