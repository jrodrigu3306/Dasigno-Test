namespace back_dasigno_test.Model
{
    public class ResutModel<T>
    {
        public List<T> Data { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
        public int State { get; set; }
        public int? Page { get; set; }
        public int? ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int? TotalPages { get; set; }
        public ResutModel() { }

        public ResutModel<T> getResultModel(bool err, List<T> data = null)
        {
            if (err)
            {
                return new ResutModel<T>()
                {
                    Data = null,
                    Error = true,
                    Message = "Fail",
                    State = 500,
                    TotalItems = 0
                };
            }
            else
            {
                return new ResutModel<T>()
                {
                    Data = data,
                    Error = false,
                    Message = "Success",
                    State = 200,
                    Page = 1,
                    ItemsPerPage = 1,
                    TotalItems = data.Count,
                    TotalPages = 1
                };
            }
        }

    }
}
