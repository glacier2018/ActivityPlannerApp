using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {

    }
    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly DataContext _dbContext;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public IsHostRequirementHandler(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            // since we are going to query against database to find Attendee with combination of both UserId and ActivityId
            //1. get userId from AuthorizationHandlerContext
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //if userId doens't exist, they don't meet auth requirement and this task is completed.
            if (userId == null) return Task.CompletedTask;


            //find activityId value from HttpContextAccessor,and parse type to Guid
            var activityId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues
            .SingleOrDefault(x => x.Key == "id").Value?.ToString());

            // find the attendee from join table with userId and activityId
            //DbContext.FindAsync()method can takes multiple keyValues as parameter.
            // var attendee = _dbContext.ActivityAttendees.FindAsync(activityId, userId).Result; 
            //FindAsync returns a ValueTask, and Result of it is Attendee

            //However the code above is instroducing a bug, we dont' want to track attendee in memory 
            //Thus we need to us AsNoTracking() method which doens't track recordin memory
            var attendee = _dbContext.ActivityAttendees
                            .AsNoTracking()
                            .SingleOrDefaultAsync(x => x.ActivityId == activityId && x.AppUserId == userId)
                            .Result;

            if (attendee == null) return Task.CompletedTask;

            //context.Succeed means Auhtorization succeeded;
            // this is the only condition that we grant authorization by setting the context to Success flag
            if (attendee.IsHost) context.Succeed(requirement);

            return Task.CompletedTask;



        }
    }
}