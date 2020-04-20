using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

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
        private readonly LinkGenerator _linkGenerator;

        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }
        // make sure it is get operation.
        // the "Route" [Route("api/[controller]")] + "Verb" [HttpGet] on the action -> is how you get some operation that someone can call
        [HttpGet]
        // the default value in the parameter is important, because it gives us the options to add query string to the URI, or leave it without query string
        public async Task<IActionResult> GetCamps(bool includeTalks = false)
        {
            // if we want to say specific what this api returns for me, we can declare the function name as follows:
            // public async Task<ActionResult<CampModel[]>> GetCamps()
            try
            {
                var results = await _repository.GetAllCampsAsync(includeTalks);

                // mean please map from results to array of CampModel
                CampModel[] models = _mapper.Map<CampModel[]>(results);
                return Ok(models);
            }
            catch(Exception e)
            {
                // becuase we do not have something like "Ok" for internal server error
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }

        }

        // this attribute to have the following URL to be routed here, and injected to the string parameter in this method
        // http://localhost:6600/api/camps/monikerName
        [HttpGet("{moniker}")]
        public async Task<IActionResult> Get(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker);

                if (result == null) return NotFound();

                CampModel models = _mapper.Map<CampModel>(result);
                return Ok(models);
            }
            catch
            {
                // becuase we do not have something like "Ok" for internal server error
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }

        }

        [HttpGet("search")]
        // http://localhost:6600/api/camps/search?theDate=2018-10-18
        // the default value in the parameter is important, because it gives us the options to add query string to the URI, or leave it without query string
        public async Task<IActionResult> SearchByDate(DateTime theDate,bool includeTalks = false)
        {
            // if we want to say specific what this api returns for me, we can declare the function name as follows:
            // public async Task<ActionResult<CampModel[]>> GetCamps()
            try
            {
                var results = await _repository.GetAllCampsByEventDate(theDate, includeTalks);

                if (!results.Any()) return NotFound();
                // mean please map from results to array of CampModel
                CampModel[] models = _mapper.Map<CampModel[]>(results);
                return Ok(models);
            }
            catch (Exception e)
            {
                // becuase we do not have something like "Ok" for internal server error
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }

        }

        [HttpPost]
        // http://localhost:6600/api/camps
        // you can use either [FromBody] or [APIController]
        public async Task<IActionResult> PostCamp(CampModel model)
        {
            try
            {
                var existing = await _repository.GetCampAsync(model.Moniker);
                if(existing != null)
                {
                    return BadRequest("Moniker In Use");
                }
                var location = _linkGenerator.GetPathByAction("Get", "Camps", new { moniker = model.Moniker });
                if(string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use current moniker");
                }
                // take the "campModel" and map it back to our "camp"
                var camp = _mapper.Map<Camp>(model);

                _repository.Add(camp);
                if(await _repository.SaveChangesAsync())
                {
                    // in POST method we did not return Ok(); we have to return different value
                    // we have to return Created
                    // created takes 2 parameters, the first is the "URI" that you should use in the "Get" request to get this object
                    // second parameter is the object that should be returned if use this URI.
                    // in order to make the URI generic, we have to use one of ASP.Net Core Features
                    // you can use the following $"/api/camps/{camp.Moniker}", but this is not good because it is fragile because it is hardcoding
                    return Created("", _mapper.Map<CampModel>(camp));
                }
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }

            // if "SaveChanges" fail
            return BadRequest();
        }
    }
}