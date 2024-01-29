using System.Dynamic;
using System.Net;

namespace HubIdentificacao.src.App.Dtos
{
    public class ResponseGeneral<T> where T :  class
    {
        public HttpStatusCode CodeHttp  {get; set;}

        public T? DataRetorn {get; set;}

        public ExpandoObject? ErrorReturn {get; set;}

        public string? Message {get; set;}

    }

}