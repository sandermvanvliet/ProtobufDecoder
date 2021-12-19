using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using ProtobufDecoder.Application.Wpf.Converters;
using ProtobufDecoder.Application.Wpf.ViewModels;
using ProtobufDecoder.Tags;
using Xunit;

namespace ProtobufDecoder.Application.Wpf.Test.Unit
{
    public class WhenConvertingProtobufTag
    {
        [Fact]
        public void GivenSingleTag_ListOfPropertyDescriptorsIsReturned()
        {
            var input = new ProtobufTagSingle();

            Convert(input)
                .Should()
                .BeOfType<List<ProtobufTagPropertyDescriptor>>();
        }

        [Fact]
        public void GivenInputIsNotATag_EmptyListIsReturned()
        {
            // Use a proper object with properties to
            // ensure we're not accidentially seeing
            // properties of the input
            var input = new 
            {
                Foo = "Bar"
            };

            Convert(input)
                .As<List<ProtobufTagPropertyDescriptor>>()
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GivenSingleTag_StartOffsetHasCategorySetToOffsets()
        {
            // Use a proper object with properties to
            // ensure we're not accidentially seeing
            // properties of the input
            var input = new ProtobufTagViewModel(new ProtobufTagSingle());

            Convert(input)
                .As<List<ProtobufTagPropertyDescriptor>>()
                .Single(p => p.Name == "Start offset") // Note: use the value from the DisplayName attribute
                .Category
                .Should()
                .Be("Offsets");
        }

        [Fact]
        public void GivenSingleTag_ParentPropertyIsExcludedBecauseItsNotBrowsable()
        {
            // Use a proper object with properties to
            // ensure we're not accidentially seeing
            // properties of the input
            var input = new ProtobufTagViewModel(new ProtobufTagSingle());

            Convert(input)
                .As<List<ProtobufTagPropertyDescriptor>>()
                .Should()
                .NotContain(p => p.Name == nameof(ProtobufTagSingle.Parent));
        }

        private object Convert(object input)
        {
            return new ProtobufTagConverter()
                .Convert(
                    input,
                    typeof(List<ProtobufTagPropertyDescriptor>),
                    null,
                    CultureInfo.InvariantCulture);
        }
    }
}