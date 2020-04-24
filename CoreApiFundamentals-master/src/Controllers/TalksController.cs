using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/Camps/{moniker}/talks")]
    [ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        // note that the value of moniker will be taken from the Url as specified above in the "Route" attribute
        // http://localhost:6600/api/camps/ATL2018/talks
        public async Task<IActionResult> Get(string moniker)
        {
            try
            {
                var results = await _repository.GetTalksByMonikerAsync(moniker, true);

                TalkModel[] models = _mapper.Map<TalkModel[]>(results);
                return Ok(models);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }
        }

        [HttpGet("{id:int}")]
        // http://localhost:6600/api/camps/ATL2018/talks/1
        public async Task<IActionResult> Get(string moniker, int id)
        {
            try
            {
                var results = await _repository.GetTalkByMonikerAsync(moniker, id, true);

                TalkModel models = _mapper.Map<TalkModel>(results);
                return Ok(models);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }
        }

        [HttpPost]
        //http://localhost:6600/api/camps/ATL2018/talks
        public async Task<IActionResult> Post(string moniker, TalkModel talkModel)
        {
            try
            {
                // link(associate) sent talk with this camp
                var talk = _mapper.Map<Talk>(talkModel);

                // first check that this camp exist
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return BadRequest("Camp does not exist");
              
                talk.Camp = camp;

                // check that sent talk has speaker, to get the id
                if (talkModel.Speaker == null) return BadRequest("Speaker Id is required");

                var speaker = await _repository.GetSpeakerAsync(talkModel.Speaker.SpeakerId);
                if (speaker == null) return BadRequest("speaker could not be found");

                // link(associate) sent talk with this speaker
                talk.Speaker = speaker;

                // add updated talk
                _repository.Add(talk);
                if(await _repository.SaveChangesAsync())
                {
                    var url = _linkGenerator.GetPathByAction(HttpContext, "Get", values: new { moniker, id = talk.TalkId });
                    return Created(url, _mapper.Map<TalkModel>(talk));
                }

                return BadRequest("Failed To Save new Talk");
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int id, TalkModel talkModel)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id, true);
                if (talk == null) return NotFound("Talk not found");

                // copy the property from "talkModel" to "talk"
                // but the problem with this, when it copy from "talkModel" it will copy every thing including the speaker and the camp
                // so if the put request not includes the information of camp, and speaker, it will makes them null although they
                // may have values in the "talk" object that will be nulled if we made below map
                // so we need to ignore speaker, and camp in this mapping, and leave it only for talk
                // see the ignore in "campProfile"
                _mapper.Map(talkModel, talk);

                // in case the speaker is not null, and we need to update it
                if(talkModel.Speaker != null)
                {
                    var speaker = await _repository.GetSpeakerAsync(talkModel.Speaker.SpeakerId);
                    if(speaker != null)
                    {
                        talk.Speaker = speaker;
                    }
                }

                if(await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<TalkModel>(talk);
                }

                else 
                {
                    return BadRequest("Failed To Update Database");
                }
                
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }
        }
    }
}
