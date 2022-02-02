using System.Runtime.InteropServices;

namespace JsonScene
{
    public record struct GlbChunk
    (
        uint ChunkType,
        Memory<byte> Data
    );

    /// <summary>
    /// https://www.khronos.org/registry/glTF/specs/2.0/glTF-2.0.html#glb-file-format-specification
    /// </summary>
    public static class Glb
    {
        public const uint MAGIC = 0x46546C67;
        public const uint VERSION = 2;
        public const uint JSON_CHUNK_TYPE = 0x4E4F534A;
        public const uint BIN_CHUNK_TYPE = 0x004E4942;

        /// <summary>
        /// parse glb header and return body length bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Memory<byte> Header(Memory<byte> bytes)
        {
            var span = MemoryMarshal.Cast<byte, uint>(bytes.Span);
            if (span[0] != MAGIC)
            {
                throw new GlbExcpetion($"invalid magic: {span[0]} != {MAGIC}");
            }
            if (span[1] != VERSION)
            {
                throw new GlbExcpetion($"invalid version: {span[1]} != {VERSION}");
            }
            return bytes.Slice(0, (int)span[2]).Slice(12);
        }

        public static (Memory<byte> Bytes, GlbChunk Chunk) Chunk(Memory<byte> bytes)
        {
            var span = MemoryMarshal.Cast<byte, uint>(bytes.Span);
            var length = span[0];
            var chunk = new GlbChunk(span[1], bytes.Slice(8, (int)length));
            return (bytes.Slice(8 + (int)length), chunk);
        }
    }
}
