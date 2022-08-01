using System.ComponentModel.DataAnnotations;

namespace Babelon.Infrastucture.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value ,ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extention =Path.GetExtension(file.FileName);
                string[] extentions = {"jpg", "png" };
                bool result = extention.Any(x => extention.EndsWith(x));
                if (!result)
                {
                    return new ValidationResult("Allowed extentions {jpg , png}");
                }
            }
            return ValidationResult.Success;

        }
    }
}
