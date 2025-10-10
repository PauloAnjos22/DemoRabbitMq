namespace UserService.Application.DTOs
{
    public class ResultResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public ResultResponse(bool success, string? errorMessage = null)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public static ResultResponse Ok() => new ResultResponse(true);
        public static ResultResponse Fail(string errorMessage) => new ResultResponse(false, errorMessage);

    }
}
