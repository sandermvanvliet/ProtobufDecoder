using System.Text;

namespace ProtobufDecoder.Output.Protobuf
{
    public class Renderer : IRenderer
    {
        public string Render(ProtobufMessage message)
        {
            var builder = new StringBuilder();
            var visitor = new ProtobufTagVisitor(builder, "    ");

            builder.AppendLine($"message {message.Name}");
            builder.AppendLine("{");

            foreach (var tag in message.Tags)
            {
                visitor.Visit(tag);
            }

            builder.AppendLine("}");

            return builder.ToString();
        }
    }
}
