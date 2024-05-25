// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: gxovnt.messaging.container.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Gxovnt.Messaging {

  /// <summary>Holder for reflection information generated from gxovnt.messaging.container.proto</summary>
  public static partial class GxovntMessagingContainerReflection {

    #region Descriptor
    /// <summary>File descriptor for gxovnt.messaging.container.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static GxovntMessagingContainerReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CiBneG92bnQubWVzc2FnaW5nLmNvbnRhaW5lci5wcm90bxIQZ3hvdm50Lm1l",
            "c3NhZ2luZxoeZ3hvdm50Lm1lc3NhZ2luZy5jb21tYW5kLnByb3RvGh1neG92",
            "bnQubWVzc2FnaW5nLmNvbmZpZy5wcm90byLUAgoJQ29udGFpbmVyEkEKDW1l",
            "c3NhZ2VUeXBlSWQYASABKA4yKi5neG92bnQubWVzc2FnaW5nLkNvbnRhaW5l",
            "ci5NZXNzYWdlVHlwZV9JZBITCglUZXh0VmFsdWUYAiABKAlIABISCghJbnRW",
            "YWx1ZRgDIAEoBUgAItMBCg5NZXNzYWdlVHlwZV9JZBIPCgtDbGVhckNvbmZp",
            "ZxAAEg4KClNhdmVDb25maWcQARIRCg1TZXRTeXN0ZW1OYW1lEAISEQoNR2V0",
            "U3lzdGVtTmFtZRADEhEKDVNldFN5c3RlbVR5cGUQBBIRCg1HZXRTeXN0ZW1U",
            "eXBlEAUSDwoLU2V0V2lGaVNTSUQQBhIPCgtHZXRXaUZpU1NJRBAHEhMKD1Nl",
            "dFdpRmlQYXNzd29yZBAIEhMKD0dldFdpRmlQYXNzd29yZBAJEggKBEVjaG8Q",
            "CkIFCgNtc2diBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Gxovnt.Messaging.GxovntMessagingCommandReflection.Descriptor, global::Gxovnt.Messaging.GxovntMessagingConfigReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Gxovnt.Messaging.Container), global::Gxovnt.Messaging.Container.Parser, new[]{ "MessageTypeId", "TextValue", "IntValue" }, new[]{ "Msg" }, new[]{ typeof(global::Gxovnt.Messaging.Container.Types.MessageType_Id) }, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class Container : pb::IMessage<Container>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<Container> _parser = new pb::MessageParser<Container>(() => new Container());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<Container> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Gxovnt.Messaging.GxovntMessagingContainerReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Container() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Container(Container other) : this() {
      messageTypeId_ = other.messageTypeId_;
      switch (other.MsgCase) {
        case MsgOneofCase.TextValue:
          TextValue = other.TextValue;
          break;
        case MsgOneofCase.IntValue:
          IntValue = other.IntValue;
          break;
      }

      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Container Clone() {
      return new Container(this);
    }

    /// <summary>Field number for the "messageTypeId" field.</summary>
    public const int MessageTypeIdFieldNumber = 1;
    private global::Gxovnt.Messaging.Container.Types.MessageType_Id messageTypeId_ = global::Gxovnt.Messaging.Container.Types.MessageType_Id.ClearConfig;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Gxovnt.Messaging.Container.Types.MessageType_Id MessageTypeId {
      get { return messageTypeId_; }
      set {
        messageTypeId_ = value;
      }
    }

    /// <summary>Field number for the "TextValue" field.</summary>
    public const int TextValueFieldNumber = 2;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string TextValue {
      get { return HasTextValue ? (string) msg_ : ""; }
      set {
        msg_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        msgCase_ = MsgOneofCase.TextValue;
      }
    }
    /// <summary>Gets whether the "TextValue" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTextValue {
      get { return msgCase_ == MsgOneofCase.TextValue; }
    }
    /// <summary> Clears the value of the oneof if it's currently set to "TextValue" </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTextValue() {
      if (HasTextValue) {
        ClearMsg();
      }
    }

    /// <summary>Field number for the "IntValue" field.</summary>
    public const int IntValueFieldNumber = 3;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int IntValue {
      get { return HasIntValue ? (int) msg_ : 0; }
      set {
        msg_ = value;
        msgCase_ = MsgOneofCase.IntValue;
      }
    }
    /// <summary>Gets whether the "IntValue" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasIntValue {
      get { return msgCase_ == MsgOneofCase.IntValue; }
    }
    /// <summary> Clears the value of the oneof if it's currently set to "IntValue" </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearIntValue() {
      if (HasIntValue) {
        ClearMsg();
      }
    }

    private object msg_;
    /// <summary>Enum of possible cases for the "msg" oneof.</summary>
    public enum MsgOneofCase {
      None = 0,
      TextValue = 2,
      IntValue = 3,
    }
    private MsgOneofCase msgCase_ = MsgOneofCase.None;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public MsgOneofCase MsgCase {
      get { return msgCase_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearMsg() {
      msgCase_ = MsgOneofCase.None;
      msg_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as Container);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(Container other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (MessageTypeId != other.MessageTypeId) return false;
      if (TextValue != other.TextValue) return false;
      if (IntValue != other.IntValue) return false;
      if (MsgCase != other.MsgCase) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (MessageTypeId != global::Gxovnt.Messaging.Container.Types.MessageType_Id.ClearConfig) hash ^= MessageTypeId.GetHashCode();
      if (HasTextValue) hash ^= TextValue.GetHashCode();
      if (HasIntValue) hash ^= IntValue.GetHashCode();
      hash ^= (int) msgCase_;
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (MessageTypeId != global::Gxovnt.Messaging.Container.Types.MessageType_Id.ClearConfig) {
        output.WriteRawTag(8);
        output.WriteEnum((int) MessageTypeId);
      }
      if (HasTextValue) {
        output.WriteRawTag(18);
        output.WriteString(TextValue);
      }
      if (HasIntValue) {
        output.WriteRawTag(24);
        output.WriteInt32(IntValue);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (MessageTypeId != global::Gxovnt.Messaging.Container.Types.MessageType_Id.ClearConfig) {
        output.WriteRawTag(8);
        output.WriteEnum((int) MessageTypeId);
      }
      if (HasTextValue) {
        output.WriteRawTag(18);
        output.WriteString(TextValue);
      }
      if (HasIntValue) {
        output.WriteRawTag(24);
        output.WriteInt32(IntValue);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (MessageTypeId != global::Gxovnt.Messaging.Container.Types.MessageType_Id.ClearConfig) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) MessageTypeId);
      }
      if (HasTextValue) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(TextValue);
      }
      if (HasIntValue) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(IntValue);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(Container other) {
      if (other == null) {
        return;
      }
      if (other.MessageTypeId != global::Gxovnt.Messaging.Container.Types.MessageType_Id.ClearConfig) {
        MessageTypeId = other.MessageTypeId;
      }
      switch (other.MsgCase) {
        case MsgOneofCase.TextValue:
          TextValue = other.TextValue;
          break;
        case MsgOneofCase.IntValue:
          IntValue = other.IntValue;
          break;
      }

      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            MessageTypeId = (global::Gxovnt.Messaging.Container.Types.MessageType_Id) input.ReadEnum();
            break;
          }
          case 18: {
            TextValue = input.ReadString();
            break;
          }
          case 24: {
            IntValue = input.ReadInt32();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            MessageTypeId = (global::Gxovnt.Messaging.Container.Types.MessageType_Id) input.ReadEnum();
            break;
          }
          case 18: {
            TextValue = input.ReadString();
            break;
          }
          case 24: {
            IntValue = input.ReadInt32();
            break;
          }
        }
      }
    }
    #endif

    #region Nested types
    /// <summary>Container for nested types declared in the Container message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static partial class Types {
      public enum MessageType_Id {
        [pbr::OriginalName("ClearConfig")] ClearConfig = 0,
        [pbr::OriginalName("SaveConfig")] SaveConfig = 1,
        [pbr::OriginalName("SetSystemName")] SetSystemName = 2,
        [pbr::OriginalName("GetSystemName")] GetSystemName = 3,
        [pbr::OriginalName("SetSystemType")] SetSystemType = 4,
        [pbr::OriginalName("GetSystemType")] GetSystemType = 5,
        [pbr::OriginalName("SetWiFiSSID")] SetWiFiSsid = 6,
        [pbr::OriginalName("GetWiFiSSID")] GetWiFiSsid = 7,
        [pbr::OriginalName("SetWiFiPassword")] SetWiFiPassword = 8,
        [pbr::OriginalName("GetWiFiPassword")] GetWiFiPassword = 9,
        [pbr::OriginalName("Echo")] Echo = 10,
      }

    }
    #endregion

  }

  #endregion

}

#endregion Designer generated code