﻿using DODQuiz.Application.Abstract.Services;
using DODQuiz.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace DODQuiz.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "admin")]
    [ApiController]
    public class AdminController : ControllerBase

    {

        private readonly IProfileService profileService;
        private readonly IGameService gameService;
        public AdminController(IProfileService profileService, IGameService gameService)
        {
            this.profileService = profileService;
            this.gameService = gameService;
        }
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(UserRegisterRequest registerRequest, CancellationToken cancellationToken)
        {
            var result = await profileService.Register(registerRequest, cancellationToken);
            return Ok(result);
        }
        [HttpGet("GetUsers")]
        public async Task<ActionResult> GetUsers(CancellationToken cancellationToken)
        {
            return Ok();
        }

        [HttpGet("GetQuestionByUser")]
        public async Task<ActionResult> GetQuestionByUser(Guid userId, CancellationToken cancellationToken)
        {
            return Ok();
        }
        [HttpGet("GetAllQuestions")]
        public async Task<ActionResult> GetAllQuestions(CancellationToken cancellationToken)
        {
            var result = await gameService.GetAllQuestions(cancellationToken);
            if (result.IsError)
            {
                return BadRequest(result);
            }
            return Ok(result.Value);
        }

        [HttpPost("AddQuestion")]
        public async Task<ActionResult> AddQuestion(QuestionRequest questionRequest, CancellationToken cancellationToken)
        {
            var result = await gameService.AddQuestion(questionRequest, cancellationToken);
            if ( result.IsError)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("EditQuestion")]
        public async Task<ActionResult> EditQuestion(Guid id, QuestionRequest questionRequest, CancellationToken cancellationToken)
        {
            return Ok();
        }
        [HttpDelete("DeleteQuestion")]
        public async Task<ActionResult> DeleteQuestion(Guid questionId, CancellationToken cancellationToken)
        {
            
            
            return Ok();
        }

    }
}
