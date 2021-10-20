
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class GetOne
    {
        public class Query : IRequest<Result<Activity>>
        {
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<Activity>>

         
        //Query is first type parameter that passes in to Handler and Activity will be the second type parameter 
        //And will be the type of return value
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                // return await _context.Activities.FindAsync(request.Id);
                var activity = await _context.Activities.FindAsync(request.Id);

                return Result<Activity>.Success(activity);

            }
        }
    }
}