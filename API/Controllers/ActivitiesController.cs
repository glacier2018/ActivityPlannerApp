using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ActivitiesController : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            return await Mediator.Send(new GetAll.Query());
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(Guid id)
        {
            return await Mediator.Send(new GetOne.Query { Id = id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity acty)
        {
            return Ok(await Mediator.Send(new Create.Command { Activity = acty }));

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, Activity acty)
        {
            acty.Id = id;
            return Ok(await Mediator.Send(new Edit.Command { Activity = acty }));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            return Ok(await Mediator.Send(new Delete.Command { Id = id }));
        }

    }
}