using System;
using System.IO;
using FluentAssertions;
using Google.Protobuf;
using ProtobufDecoder.Application.Wpf.Commands;
using ProtobufDecoder.Application.Wpf.ViewModels;
using Xunit;

namespace ProtobufDecoder.Application.Wpf.Test.Unit
{
    public class WhenDecodingNestedMessageTags
    {
        [Fact]
        public void GivenNestedMessagePayload_NestedMessageTagOnFirstLevelCanBeDecoded()
        {
            _messageViewModel
                .Tags[3]
                .CanDecode
                .Should()
                .BeTrue();
        }

        [Fact]
        public void GivenNestedMessagePayload_NestedMessageTagOnFirstLevelIsDecodedSuccessfully()
        {
            var commandResult = _messageViewModel.Tags[3].DecodeTag();

            commandResult
                .Result
                .Should()
                .Be(Result.Success);
        }

        [Fact]
        public void GivenNestedMessagePayload_NestedMessageTagOnSecondLevelCanBeDecoded()
        {
            _messageViewModel.Tags[3].DecodeTag();
            var viewModel = _messageViewModel.Tags[3].Children[3];

            viewModel
                .CanDecode
                .Should()
                .BeTrue();
        }

        [Fact]
        public void GivenNestedMessagePayload_NestedMessageTagOnSecondLevelIsDecodedSuccessfully()
        {
            _messageViewModel.Tags[3].DecodeTag();
            var viewModel = _messageViewModel.Tags[3].Children[3];

            viewModel
                .DecodeTag()
                .Result
                .Should()
                .Be(Result.Success);
        }

        private readonly MessageViewModel _messageViewModel;

        public WhenDecodingNestedMessageTags()
        {
            var root = new Message1
            {
                IntValue = 1,
                StringValue = "one",
                ByteValue = ByteString.CopyFrom(0x1, 0x1),
                ObjValue = new Message2
                {
                    IntValue = 2,
                    StringValue = "two",
                    ByteValue = ByteString.CopyFrom(0x2, 0x2),
                    ObjValue = new Message3
                    {
                        IntValue = 3,
                        StringValue = "three",
                        ByteValue = ByteString.CopyFrom(0x3, 0x3),
                    }
                }
            };

            _messageViewModel = new MessageViewModel();
            _messageViewModel.LoadFromBytes(root.ToByteArray());
        }
    }
}