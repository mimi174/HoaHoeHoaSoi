namespace HoaHoeHoaSoi.API.ViewModels
{
    public class ResponseVMs
    {
    }

    public class ResponseModel<T> where T : class
    {
        public string ErrorMsg { get; set; }
        public T Result { get; set; }
    }

    public class ResponseModelList<T> : ResponseModel<T> where T : class
    {
        public int TotalItem { get; set; }
        public int PageNumber { get; set; }
        public int ItemCount { get; set; }
    }
}
