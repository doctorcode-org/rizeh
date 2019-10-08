namespace Parsnet
{
    public class Message
    {
        public Commands Command { get; set; }
        public object Data { get; set; }
        public string SystemId { get; set; }
        public string Version { get; set; }
    }
}

