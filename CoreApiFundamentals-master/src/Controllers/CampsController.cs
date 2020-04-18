﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreCodeCamp.Data;
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

        public CampsController(ICampRepository repository)
        {
            _repository = repository;
        }
        // make sure it is get operation.
        // the "Route" [Route("api/[controller]")] + "Verb" [HttpGet] on the action -> is how you get some operation that someone can call
        [HttpGet]
        public IActionResult GetCamps()
        {
            var results = _repository.GetAllCampsAsync();
            return Ok(new { Moniker = "ATL2020", Name = "Atlanta Code Camp" });
        }
    }
}