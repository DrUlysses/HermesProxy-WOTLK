using Bgs.Protocol;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace Framework.Util
{
    public static class ProtobufExtensions
    {
        private static Variant AddInternalGetRef(this RepeatedField<Attribute> attributes, string name)
        {
            var attribute = new Attribute();
            attribute.Name = name;
            attribute.Value = new Variant();
            attributes.Add(attribute);

            return attribute.Value;
        }

        public static void AddBlob(this RepeatedField<Attribute> attributes, string name, ByteString blob)
        {
            attributes.AddInternalGetRef(name).BlobValue = blob;
        }

        public static void AddString(this RepeatedField<Attribute> attributes, string name, string value)
        {
            attributes.AddInternalGetRef(name).StringValue = value;
        }

        public static void AddInt(this RepeatedField<Attribute> attributes, string name, long value)
        {
            attributes.AddInternalGetRef(name).IntValue = value;
        }
    }
}
