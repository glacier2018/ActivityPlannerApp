using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        //since this will be a Command and doesn't return anything,
        //IRequest Interface won't have a type value as the return type
        {
            public Activity Activity { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        //IRequestHandler also just have one type parameter since it won't return anything
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(appUser =>
                appUser.UserName == _userAccessor.GetUsername());

                var activity = request.Activity;

                activity.Attendees.Add(new ActivityAttendee
                {

                    AppUser = user,
                    Activity = activity,
                    IsHost = true
                });
                _context.Activities.Add(request.Activity);

                var result = await _context.SaveChangesAsync() > 0; //result now is a bool and will
                                                                    //and will be true if save succ
                                                                    //and will be true if save succeed and false if failed

                return Result<Unit>.Success(Unit.Value);  //Unit.Value should be nothing   

            }
        }
    }
}