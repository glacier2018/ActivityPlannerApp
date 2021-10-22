using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class UpdateAttendance
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }


        public class Handler : IRequestHandler<Command, Result<Unit>>
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
                var activity = await _context.Activities.FindAsync(request.Id);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == _userAccessor.GetUsername());

                if (user == null || activity == null) return null;

                bool IsHost = _userAccessor.GetUsername() == activity.Attendees.FirstOrDefault(x => x.IsHost).AppUser.UserName;


                if (IsHost) activity.IsCancelled = !activity.IsCancelled;
                activity.Attendees.Remove(activity.Attendees.FirstOrDefault(x => x.AppUser.UserName == _userAccessor.GetUsername()));


                var result = await _context.SaveChangesAsync() > 0;

                return result
                    ? Result<Unit>.Success(Unit.Value)
                    : Result<Unit>.Failure("Got problem changing attendance");

            }
        }
    }
}