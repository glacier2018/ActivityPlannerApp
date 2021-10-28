using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<Result<Photo>>// we need to return the photo to the client
        {
            public IFormFile File { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Photo>>
        {
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
                _photoAccessor = photoAccessor;
            }

            public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                //first we need get user from Database , that the destination to upload to 
                //we also need to use eager loading to get related photos 
                var user = await _context.Users
                    .Include(au => au.Photos)
                    .FirstOrDefaultAsync(au => au.UserName == _userAccessor.GetUsername());
                if (user == null) return null;

                var photoUploadResult = await _photoAccessor.AddPhoto(request.File);
                //if above fails, PhotoAccessor if going to throw an exception
                var photo = new Photo
                {
                    Url = photoUploadResult.Url,
                    Id = photoUploadResult.PublicId
                };

                //if there is no photos setting to main, this one will be main
                if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;
                user.Photos.Add(photo);

                var result = await _context.SaveChangesAsync() > 0;

                return result ? Result<Photo>.Success(photo)
                              : Result<Photo>.Failure("having trouble adding photos");

            }
        }
    }
}