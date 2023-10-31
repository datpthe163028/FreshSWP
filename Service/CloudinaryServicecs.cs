using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace WebApplication6.Service
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }

    public interface ICloudinaryService
    {
        string UploadImage(IFormFile file, string folderName);
    }
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinarySettings _cloudinarySettings;
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings)
        {
            _cloudinarySettings = cloudinarySettings.Value;

            Account account = new Account(
                _cloudinarySettings.CloudName,
                _cloudinarySettings.ApiKey,
                _cloudinarySettings.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }

        public string UploadImage(IFormFile file, string folderName)
        {
            using (var stream = file.OpenReadStream())
            {
               
                var uploadParams = new ImageUploadParams
                {   
                    Folder = folderName,
                    File = new FileDescription(file.FileName, stream)
                };

                var uploadResult = _cloudinary.Upload(uploadParams);
           
                if (uploadResult.Error != null && uploadResult.Error.Message.Contains("already exists"))
                {
                    return "0";
                }
                var str = uploadResult.SecureUrl.ToString();

                if(str.Substring(str.Length-4).ToUpper().Equals("AVIF"))
                {
                    return str.Substring(0, str.Length - 4) + "png";
                }

                return str;

            }
        }
    }
}
