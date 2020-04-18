﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreCodeCamp.Controllers
{
    // this route is going to be exposed to route table
    // [controller] -> means whatever comes before the word "controller", so there is no need to hard code the word "Camps".
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        // make sure it is get operation.
        // the "Route" [Route("api/[controller]")] + "Verb" [HttpGet] on the action -> is how you get some operation that someone can call
        [HttpGet]
        public async Task<IActionResult> GetCamps()
        {
            // if we want to say specific what this api returns for me, we can declare the function name as follows:
            // public async Task<ActionResult<CampModel[]>> GetCamps()
            try
            {
                var results = await _repository.GetAllCampsAsync();

                // mean please map from results to array of CampModel
                CampModel[] models = _mapper.Map<CampModel[]>(results);
                return Ok(models);
            }
            catch
            {
                // becuase we do not have something like "Ok" for internal server error
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }

        }
    }
}