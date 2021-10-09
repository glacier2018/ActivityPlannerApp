using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        public class Command : IRequest
        //since this will be a Command and doesn't return anything,
        //IRequest Interface won't have a type value as the return type
        {
            public Activity Activity { get; set; }
        }
        public class Handler : IRequestHandler<Command>
        //IRequestHandler also just have one type parameter since it won't return anything
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                _context.Activities.Add(request.Activity);
                //this operation doens't have to AddAsync since that one is for SQL server
                // it's just in memory at current stage.
                await _context.SaveChangesAsync();
                return Unit.Value;
            }
        }
    }
}