using System.ComponentModel.DataAnnotations;

namespace HoaHoeHoaSoi.Helpers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ImageExtenstionAttribute : ValidationAttribute
    {
        private readonly string[] VALID_EXTENSTIONS = { ".png", ".jpg", ".jpeg" };

        public override bool IsValid(object? value)
        {
            var file = value as IFormFile;
            if (file == null) return false;

            var extension = Path.GetExtension(file.FileName);
            return VALID_EXTENSTIONS.Contains(extension.ToLower());
        }
    }
}
