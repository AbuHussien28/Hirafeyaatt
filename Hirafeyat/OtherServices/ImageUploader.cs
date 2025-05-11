namespace Hirafeyat.OtherServices
{

        public static class ImageUploader
        {
            public static string UploadImage(IFormFile imageFile, string folderName = "Images")
            {
                if (imageFile == null || imageFile.Length == 0)
                    return null;

                try
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{folderName}");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    return $"/{folderName}/{uniqueFileName}";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error uploading image: {ex.Message}");
                    return null;
                }
            }
        }
    
}
