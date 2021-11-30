﻿using FluentAssertions;
using ProtobufDecoder.Application.Wpf.ViewModels;
using Xunit;

namespace ProtobufDecoder.Application.Wpf.Test.Unit
{
    public class WhenCheckingMainWindowCommandsCanExecute
    {
        [Fact]
        public void GivenViewModel_OpenFileCommandCanBeExecuted()
        {
            new MainWindowViewModel()
                .OpenFileCommand
                .CanExecute(null)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void GivenViewModel_LoadFileCommandCannotBeExecuted()
        {
            new MainWindowViewModel()
                .LoadFileCommand
                .CanExecute(null)
                .Should()
                .BeFalse("the InputFilePath is not yet set");
        }

        [Fact]
        public void GivenInputFilePathIsSet_LoadFileCommandCanBeExecuted()
        {
            var viewModel = new MainWindowViewModel
            {
                Model =
                {
                    InputFilePath = "/some/path.bin"
                }
            };

            viewModel
                .LoadFileCommand
                .CanExecute(null)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void GivenViewModel_SaveGeneratedProtoFileCommandCannotBeExecuted()
        {
            new MainWindowViewModel()
                .SaveGeneratedProtoCommand
                .CanExecute(null)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void GivenViewModelWithRenderedProtoContent_SaveGeneratedProtoFileCommandCanBeExecuted()
        {
            var viewModel = new MainWindowViewModel
            {
                Model =
                {
                    RenderedProtoFile = "some content"
                }
            };

            viewModel
                .SaveGeneratedProtoCommand
                .CanExecute(null)
                .Should()
                .BeTrue();
        }
    }
}