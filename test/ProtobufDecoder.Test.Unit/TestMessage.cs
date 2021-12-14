// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: TestMessage.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace ProtobufDecoder.Test.Unit {

  /// <summary>Holder for reflection information generated from TestMessage.proto</summary>
  public static partial class TestMessageReflection {

    #region Descriptor
    /// <summary>File descriptor for TestMessage.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static TestMessageReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChFUZXN0TWVzc2FnZS5wcm90bxIZUHJvdG9idWZEZWNvZGVyX1Rlc3RfVW5p",
            "dCI8CgtUZXN0TWVzc2FnZRIVCg1yZXBlYXRlZEludDMyGAEgAygFEhYKDnJl",
            "cGVhdGVkU3RyaW5nGAIgAygJQhyqAhlQcm90b2J1ZkRlY29kZXIuVGVzdC5V",
            "bml0YgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::ProtobufDecoder.Test.Unit.TestMessage), global::ProtobufDecoder.Test.Unit.TestMessage.Parser, new[]{ "RepeatedInt32", "RepeatedString" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class TestMessage : pb::IMessage<TestMessage> {
    private static readonly pb::MessageParser<TestMessage> _parser = new pb::MessageParser<TestMessage>(() => new TestMessage());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<TestMessage> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ProtobufDecoder.Test.Unit.TestMessageReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TestMessage() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TestMessage(TestMessage other) : this() {
      repeatedInt32_ = other.repeatedInt32_.Clone();
      repeatedString_ = other.repeatedString_.Clone();
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TestMessage Clone() {
      return new TestMessage(this);
    }

    /// <summary>Field number for the "repeatedInt32" field.</summary>
    public const int RepeatedInt32FieldNumber = 1;
    private static readonly pb::FieldCodec<int> _repeated_repeatedInt32_codec
        = pb::FieldCodec.ForInt32(10);
    private readonly pbc::RepeatedField<int> repeatedInt32_ = new pbc::RepeatedField<int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> RepeatedInt32 {
      get { return repeatedInt32_; }
    }

    /// <summary>Field number for the "repeatedString" field.</summary>
    public const int RepeatedStringFieldNumber = 2;
    private static readonly pb::FieldCodec<string> _repeated_repeatedString_codec
        = pb::FieldCodec.ForString(18);
    private readonly pbc::RepeatedField<string> repeatedString_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<string> RepeatedString {
      get { return repeatedString_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as TestMessage);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(TestMessage other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!repeatedInt32_.Equals(other.repeatedInt32_)) return false;
      if(!repeatedString_.Equals(other.repeatedString_)) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= repeatedInt32_.GetHashCode();
      hash ^= repeatedString_.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      repeatedInt32_.WriteTo(output, _repeated_repeatedInt32_codec);
      repeatedString_.WriteTo(output, _repeated_repeatedString_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += repeatedInt32_.CalculateSize(_repeated_repeatedInt32_codec);
      size += repeatedString_.CalculateSize(_repeated_repeatedString_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(TestMessage other) {
      if (other == null) {
        return;
      }
      repeatedInt32_.Add(other.repeatedInt32_);
      repeatedString_.Add(other.repeatedString_);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10:
          case 8: {
            repeatedInt32_.AddEntriesFrom(input, _repeated_repeatedInt32_codec);
            break;
          }
          case 18: {
            repeatedString_.AddEntriesFrom(input, _repeated_repeatedString_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
