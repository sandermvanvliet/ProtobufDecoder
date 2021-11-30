using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using FluentAssertions;
using ProtobufDecoder.Application.Wpf.Converters;
using Xunit;

namespace ProtobufDecoder.Application.Wpf.Test.Unit
{
    public class WhenConvertingTagToProtoFile
    {
        [Fact]
        public void GivenProtobufMessage_StringIsReturned()
        {
            Convert(new ProtobufMessage())
                .Should()
                .BeOfType<string>();
        }

        [Fact]
        public void GivenInputIsNotAProtobufMessage_DependencyPropertyUnsetValueIsReturned()
        {
            Convert(new object())
                .Should()
                .Be(DependencyProperty.UnsetValue);
        }

        [Fact]
        public void GivenProtobufMessageWithoutName_TestMessageIsUsedAsNameInOutput()
        {
            Convert(new ProtobufMessage())
                .As<string>()
                .Should()
                .Contain("message TestMessage");
        }

        private object Convert(object input)
        {
            return new ProtobufMessageConverter()
                .Convert(
                    input,
                    typeof(List<ProtobufTagPropertyDescriptor>),
                    null,
                    CultureInfo.InvariantCulture);
        }
    }
}