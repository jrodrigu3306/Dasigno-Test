namespace back_dasigno_test.Model
{
    public class ErrorLog
    {
        public int ErrorLogId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }

        public ErrorLog CreateErrorLogFromException(Exception ex)
        {
            var errorLog = new ErrorLog
            {
                Timestamp = DateTime.Now,
                Type = ex.GetType().FullName,
                Message = ex.Message,
                Detail = ex.StackTrace
            };

            errorLog.Detail = (ex.StackTrace == null)? ex.Message: ex.StackTrace;

            return errorLog;
        }

    }
}
