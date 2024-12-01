namespace HoaHoeHoaSoi.Helpers
{
    public static class FuncHelpers
    {
        public static string ConvertImgToBase64(IFormFile img)
        {
            if (img == null)
                return string.Empty;

            string imgValue = string.Empty;
            using (var ms = new MemoryStream())
            {
                img.CopyTo(ms);
                imgValue = Convert.ToBase64String(ms.ToArray());
            }

            return imgValue;
        }
    }
}
