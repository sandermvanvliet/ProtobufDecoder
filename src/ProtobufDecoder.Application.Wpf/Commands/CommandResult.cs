namespace ProtobufDecoder.Application.Wpf.Commands
{
    public class CommandResult
    {
        public Result Result { get; set; }
        public string Message { get; set; }
        
        public static CommandResult Success(string message = null)
        {
            return new() { Result = Result.Success, Message = message };
        }
        
        public static CommandResult SuccessWithWarning(string message)
        {
            return new() { Result = Result.SuccessWithWarnings, Message = message };
        }

        public static CommandResult Failure(string message)
        {
            return new CommandResult { Result = Result.Failure, Message = message };
        }

        public static CommandResult Aborted()
        {
            return new CommandResult { Result = Result.NotExecuted };
        }
    }
}