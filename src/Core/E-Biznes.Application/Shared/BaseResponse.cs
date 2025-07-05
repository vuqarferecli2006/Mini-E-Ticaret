using System.Net;

namespace E_Biznes.Application.Shared;

public class BaseResponse<T>
{
    public bool Succes { get; set; }

    public string? Message { get; set; }

    public HttpStatusCode StatusCode { get; set; }

    public T? Data { get; set; }

    public BaseResponse(HttpStatusCode statusCode)
    {
        Succes = true;
        StatusCode = statusCode;
    }
    public BaseResponse(string message,HttpStatusCode statusCode)
    {
        Message = message;
        StatusCode = statusCode;
        Succes=false;
    }
    public BaseResponse(string message,bool isSucces,HttpStatusCode statusCode)
    {
        Message = message;
        Succes = isSucces;
        StatusCode=statusCode;
    }
    public BaseResponse(string message,T? data,HttpStatusCode statusCode)
    {
        Message = message;
        StatusCode = statusCode;
        Data = data;
        Succes = true;
    }

}
