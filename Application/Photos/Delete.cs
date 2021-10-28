using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }

        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _photoAccessor = photoAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .Include(au => au.Photos)
                    .FirstOrDefaultAsync(au => au.UserName == _userAccessor.GetUsername());
                if (user == null) return null;
                
                // get the photo to be delete from DbContext, with id provided by http request
                var photo = user.Photos.FirstOrDefault(p => p.Id == request.Id);
                if (photo == null) return null;

                //prevent user from deleting their main photos
                if (photo.IsMain) return Result<Unit>.Failure("You cannot delete your main photo");

                //the deleting process have two steps,
                //Step one is sending a delete request to Cloudinary 
                var result = await _photoAccessor.DeletePhoto(photo.Id);

                if (result == null)
                    return Result<Unit>.Failure("problem deleting your photo from Cloudinary");

                // Step two is to remove from memeory then save to dbContext, if Cloudinary delete succeeds
                user.Photos.Remove(photo);

                var success = await _context.SaveChangesAsync() > 0;

                return success
                    ? Result<Unit>.Success(Unit.Value)
                    : Result<Unit>.Failure("Problem deleting your photo from APIs");
            }
        }
    }
}