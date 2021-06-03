using Financial.Api.Application.Models;
using Financial.Api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Financial.Api.Controllers
{
    [Route("v1")]
    [ApiController]
    public class FinancialController : ControllerBase
    {
        private readonly IFinancialManager _financialManager;

        public FinancialController(IFinancialManager financialManager)
        {
            _financialManager = financialManager;
        }

        [HttpGet("health-check")]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult HealthCheck()
        {
            Log.Information("Financial Service Health-Check");

            return Ok("Post Service is running");
        }

        [HttpGet("{query}")]
        [ProducesResponseType(typeof(List<CompanySearchResult>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> SearchCompaniesAsync(string query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest();

            try
            {
                var searchResult = await _financialManager.SearchCompaniesAsync(query);

                return Ok(searchResult);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Cannot search companies using query {query}");

                return ex switch
                {
                    _ => StatusCode(500)
                };
            }
        }
    }
}
