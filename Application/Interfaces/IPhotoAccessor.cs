
using System.Threading.Tasks;
using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IPhotoAccessor
    {
        Task<PhotoUploadResult> AddPhoto(IFormFile file);
        //file represents a file send via http request

        Task<string> DeletePhoto(string publicId);
        //publicId will be the param we need to delete a photo from Cloudinary

    }
}