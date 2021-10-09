using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class Command : IRequest
        {
            public Activity Activity { get; set; }
        }
        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;

            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Activity.Id);

                //this is a sample for updating the title property of an Activity.
                // activity.Title = request.Activity.Title ?? activity.Title;

                //using the Mapping method, first parameter is the data source to map with
                //second parameter is the target to be mapped 
                _mapper.Map(request.Activity, activity);
                

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}