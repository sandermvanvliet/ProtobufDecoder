using System;
using System.Collections.ObjectModel;
using FluentAssertions;
using Google.Protobuf;
using ProtobufDecoder.Output.Protobuf;
using ProtobufDecoder.Tags;
using ProtobufDecoder.Values;
using Xunit;

namespace ProtobufDecoder.Test.Unit
{
    public class WhenRenderingProtobufInterface
    {
        private readonly Renderer _renderer = new Renderer();

        [Fact]
        public void GivenMessageWithoutTags_OnlyMessageIsWritten()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage"
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
}
");
        }

        [Fact]
        public void GivenMessageWithSingleVarintTag()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagSingle
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.Varint
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    uint32 tag1 = 1;
}
");
        }

        [Fact]
        public void GivenMessageWithSingleString()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagString
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.LengthDelimited
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    string tag1 = 1;
}
");
        }

        [Fact]
        public void GivenMessageWithLengthDelimitedTag()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagSingle
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.LengthDelimited
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    bytes tag1 = 1;
}
");
        }

        [Fact]
        public void GivenMessageWithSingleFixed32()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagSingle
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.Fixed32
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    float tag1 = 1;
}
");
        }

        [Fact]
        public void GivenMessageWithSingleFixed64()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagSingle
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.Fixed64
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    double tag1 = 1;
}
");
        }

        [Fact]
        public void GivenMessageWithRepeatedFixed64()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagRepeated
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.Fixed64
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    repeated double tag1 = 1;
}
");
        }

        [Fact]
        public void GivenMessageWithPackedVarint()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagPackedVarint()
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.Varint
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    repeated uint32 tag1 = 1 [packed=true];
}
");
        }

        [Fact]
        public void GivenMessageWithPackedFloat()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagPackedFloat
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.Fixed32
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    repeated float tag1 = 1 [packed=true];
}
");
        }

        [Fact]
        public void GivenMessageWithPackedFloatEmbedded()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagRepeated
                    {
                        Index = 1,
                        WireType = WireFormat.WireType.LengthDelimited,
                        Items =
                        {
                            new ProtobufTagEmbeddedMessage(
                                new ProtobufTagSingle
                                {
                                    Index = 1,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[] 
                                {
                                    new ProtobufTagPackedFloat
                                    {
                                        Index = 2,
                                        WireType = WireFormat.WireType.Fixed32
                                    }
                                })
                            {
                                Name="Embedded1"
                            }
                        }
                    }
                }
                
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    message Embedded1
    {
        repeated float tag2 = 2 [packed=true];
    }

    repeated Embedded1 tag1 = 1;
}
");
        }

        [Fact]
        public void GivenMessageWithSingleEmbeddedMessage()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                        {
                            Index = 10,
                            WireType = WireFormat.WireType.LengthDelimited
                        },
                        new ProtobufTag[]
                        {
                            new ProtobufTagSingle
                            {
                                Index = 1,
                                WireType = WireFormat.WireType.Fixed64
                            }
                        })
                    {
                        Name = "EmbeddedMessage"
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    message EmbeddedMessage
    {
        double tag1 = 1;
    }

    EmbeddedMessage tag10 = 10;
}
");
        }

        [Fact]
        public void GivenMessageWithTwoInstancesOfEmbeddedMessage()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagRepeated
                    {
                        Index = 10,
                        WireType = WireFormat.WireType.LengthDelimited,
                        Items = new ObservableCollection<ProtobufTagSingle>
                        {
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagSingle
                                    {
                                        Index = 1,
                                        WireType = WireFormat.WireType.Fixed64
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            },
                            
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagSingle
                                    {
                                        Index = 1,
                                        WireType = WireFormat.WireType.Fixed64
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            }
                        }
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    message EmbeddedMessage
    {
        double tag1 = 1;
    }

    repeated EmbeddedMessage tag10 = 10;
}
");
        }

        [Fact]
        public void GivenMessageWithTwoInstancesOfEmbeddedMessageWhichContainsEmbeddedMessageTag()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagRepeated
                    {
                        Index = 10,
                        WireType = WireFormat.WireType.LengthDelimited,
                        Items = new ObservableCollection<ProtobufTagSingle>
                        {
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagEmbeddedMessage(new ProtobufTagSingle(), new []
                                    {
                                        new ProtobufTagSingle { Index = 1, WireType = WireFormat.WireType.Varint }
                                    })
                                    {
                                        Index = 1,
                                        Name = "SubEmbeddedMessage"
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            },
                            
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagEmbeddedMessage(new ProtobufTagSingle(), new []
                                    {
                                        new ProtobufTagSingle { Index = 1, WireType = WireFormat.WireType.Varint }
                                    })
                                    {
                                        Index = 1,
                                        Name = "SubEmbeddedMessage"
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            }
                        }
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    message EmbeddedMessage
    {
        message SubEmbeddedMessage
        {
            uint32 tag1 = 1;
        }

        SubEmbeddedMessage tag1 = 1;
    }

    repeated EmbeddedMessage tag10 = 10;
}
");
        }

        [Fact]
        public void GivenMessageWithTwoInstancesOfEmbeddedMessageWhichContainsRepeatedTag()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagRepeated
                    {
                        Index = 10,
                        WireType = WireFormat.WireType.LengthDelimited,
                        Items = new ObservableCollection<ProtobufTagSingle>
                        {
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagRepeated
                                    {
                                        Index = 1,
                                        WireType = WireFormat.WireType.Fixed64
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            },
                            
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagRepeated
                                    {
                                        Index = 1,
                                        WireType = WireFormat.WireType.Fixed64
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            }
                        }
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    message EmbeddedMessage
    {
        repeated double tag1 = 1;
    }

    repeated EmbeddedMessage tag10 = 10;
}
");
        }

        [Fact]
        public void GivenMessageWithTwoInstancesOfEmbeddedMessageWhichContainsPackedFloatTag()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagRepeated
                    {
                        Index = 10,
                        WireType = WireFormat.WireType.LengthDelimited,
                        Items = new ObservableCollection<ProtobufTagSingle>
                        {
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagPackedFloat
                                    {
                                        Index = 1
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            },
                            
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagPackedFloat
                                    {
                                        Index = 1
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            }
                        }
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    message EmbeddedMessage
    {
        repeated float tag1 = 1 [packed=true];
    }

    repeated EmbeddedMessage tag10 = 10;
}
");
        }

        [Fact]
        public void GivenMessageWithTwoInstancesOfEmbeddedMessageWhichContainsLengthDelimitedTag()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagRepeated
                    {
                        Index = 10,
                        WireType = WireFormat.WireType.LengthDelimited,
                        Items = new ObservableCollection<ProtobufTagSingle>
                        {
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagLengthDelimited
                                    {
                                        Index = 1
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            },
                            
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagLengthDelimited
                                    {
                                        Index = 1
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            }
                        }
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    message EmbeddedMessage
    {
        bytes tag1 = 1;
    }

    repeated EmbeddedMessage tag10 = 10;
}
");
        }

        [Fact]
        public void GivenMessageWithTwoInstancesOfEmbeddedMessageWhichContainsStringTag()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagRepeated
                    {
                        Index = 10,
                        WireType = WireFormat.WireType.LengthDelimited,
                        Items = new ObservableCollection<ProtobufTagSingle>
                        {
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagString
                                    {
                                        Index = 1
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            },
                            
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagString
                                    {
                                        Index = 1
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            }
                        }
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    message EmbeddedMessage
    {
        string tag1 = 1;
    }

    repeated EmbeddedMessage tag10 = 10;
}
");
        }

        [Fact]
        public void GivenMessageWithTwoInstancesOfEmbeddedMessageWithDifferentTagsInEachInstance()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                    new ProtobufTagRepeated
                    {
                        Index = 10,
                        WireType = WireFormat.WireType.LengthDelimited,
                        Items = new ObservableCollection<ProtobufTagSingle>
                        {
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagSingle
                                    {
                                        Index = 1,
                                        WireType = WireFormat.WireType.Fixed64
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            },
                            
                            new ProtobufTagEmbeddedMessage(new ProtobufTagSingle
                                {
                                    Index = 10,
                                    WireType = WireFormat.WireType.LengthDelimited
                                },
                                new ProtobufTag[]
                                {
                                    new ProtobufTagSingle
                                    {
                                        Index = 2,
                                        WireType = WireFormat.WireType.Varint
                                    }
                                })
                            {
                                Name = "EmbeddedMessage"
                            }
                        }
                    }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    message EmbeddedMessage
    {
        optional double tag1 = 1;
        optional uint32 tag2 = 2;
    }

    repeated EmbeddedMessage tag10 = 10;
}
");
        }

        [Fact]
        public void GivenMessageWithEmbeddedMessageWhichContainsEmbeddedMessage()
        {
            var message = new ProtobufMessage
            {
                Name = "TestMessage",
                Tags =
                {
                   new ProtobufTagEmbeddedMessage(
                       new ProtobufTagSingle
                       {
                           Index = 10,
                           WireType = WireFormat.WireType.LengthDelimited
                       },
                       new ProtobufTag[]
                       {
                           new ProtobufTagEmbeddedMessage(
                               new ProtobufTagSingle
                               {
                                   Index = 1,
                                   WireType = WireFormat.WireType.Fixed64
                               },
                               new ProtobufTag[]
                               {
                                   new ProtobufTagSingle
                                   {
                                       Index = 1,
                                       WireType = WireFormat.WireType.Fixed64
                                   }
                               })
                           {
                               Name = "EmbeddedMessage1"
                           }
                       })
                   {
                       Name = "EmbeddedMessage"
                   }
                }
            };

            var proto = _renderer.Render(message);

            proto
                .Should()
                .Be(@"message TestMessage
{
    message EmbeddedMessage
    {
        message EmbeddedMessage1
        {
            double tag1 = 1;
        }

        EmbeddedMessage1 tag1 = 1;
    }

    EmbeddedMessage tag10 = 10;
}
");
        }
    }
}
