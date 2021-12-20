namespace ProtobufDecoder
{
    public sealed class MessageParseResult
    {
        public bool Successful { get; set; }
        public ProtobufMessage Message { get; set; }
        public string FailureReason { get; set; }

        public static MessageParseResult Failure(string reason)
        {
            return new MessageParseResult
            {
                Successful = false,
                FailureReason = reason
            };
        }

        public static MessageParseResult Success(ProtobufMessage message)
        {
            return new MessageParseResult
            {
                Successful = true,
                Message = message
            };
        }
    }
}