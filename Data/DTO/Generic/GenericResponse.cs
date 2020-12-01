namespace Consent_Aries_VC.Data.DTO.Generic
{
    public class GenericResponse<T> where T : new()
    {
        public int StatusCode { get; set; }
        public bool IsError { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
    }
}
