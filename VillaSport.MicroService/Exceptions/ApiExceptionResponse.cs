using System;
namespace VillaSport.MicroService.Exceptions
{
    public class ApiExceptionResponse
    {
        public ApiExceptionResponse()
        {
        }

        public string DisplayMessage { get; internal set; }
        public object ExceptionMessage { get; internal set; }
    }
}
