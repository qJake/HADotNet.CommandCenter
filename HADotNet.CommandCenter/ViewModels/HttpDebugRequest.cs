namespace HADotNet.CommandCenter.ViewModels
{
    public class HttpDebugRequest
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string Headers { get; set; }
        public string Body { get; set; }
        public HttpDebugResponse Response { get; set; }
    }
}
