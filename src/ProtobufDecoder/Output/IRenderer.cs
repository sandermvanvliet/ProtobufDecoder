namespace ProtobufDecoder.Output
{
    public interface IRenderer
    {
        string Render(ProtobufMessage message);
    }
}