using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Consent_Aries_VC.Data.DTO.Generic
{
    public class GenericResponse<T>
    {
        public GenericResponse(string message, T data, int statusCode) {
            this.Message = message;
            this.StatusCode = statusCode;
            this.Data = data;
            this.IsError = statusCode >= 400 ? true : false;
        }

        public GenericResponse(Exception ex) {
            this.Message = ex.Message;
            this.IsError = true;
        }

        public GenericResponse(string errMsg, int statusCode)
        {
            this.Message = errMsg;
            this.IsError = true;
            this.StatusCode = statusCode;
        }

        public IActionResult toHttpResponse() {
            switch (this.StatusCode) {
                case Status200OK:
                    return new OkObjectResult(this);
                case Status400BadRequest:
                    return new BadRequestObjectResult(this);
                case Status401Unauthorized:
                    return new UnauthorizedObjectResult(this);
                case Status404NotFound:
                    return new NotFoundObjectResult(this);
                case Status500InternalServerError:
                default:
                    return new StatusCodeResult(500);
            }
        }

        private int StatusCode { get; set; }
        private bool IsError { get; set; }
        private T Data { get; set; }
        private string Message { get; set; }
    }
}
