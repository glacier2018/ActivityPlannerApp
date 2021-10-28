using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Application.Core;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
namespace API.Controllers
{

    public class ActivitiesController : BaseApiController
    {
        [HttpGet]

        public async Task<IActionResult> GetActivities()
        {
            return HandleResult(await Mediator.Send(new GetAll.Query()));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivity(Guid id)
        {
            var result = await Mediator.Send(new GetOne.Query { Id = id });

            return HandleResult<ActivityDto>(result);

        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity acty)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Activity = acty }));

        }
        [HttpPut("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<IActionResult> EditActivity(Guid id, Activity acty)
        {
            acty.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { Activity = acty }));
        }
        [HttpDelete("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [HttpPost("{id}/attend")]
        public async Task<IActionResult> Attend(Guid id)
        {
            return HandleResult(await Mediator.Send(new UpdateAttendance.Command { Id = id }));
        }

    }
}