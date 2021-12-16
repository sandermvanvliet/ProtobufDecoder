namespace ProtobufDecoder
{
    public sealed class MessageParseResult
    {
        public bool Success { get; set; }
        public ProtobufMessage Message { get; set; }
        public string FailureReason { get; set; }

        public static MessageParseResult Failed(string reason)
        {
            return new MessageParseResult
            {
                Success = false,
                FailureReason = reason
            };
        }

        public static MessageParseResult Succeeded(ProtobufMessage message)
        {
            return new MessageParseResult
            {
                Success = true,
                Message = message
            };
        }
    }
}