using System.Text.Json.Nodes;

namespace JsonScene
{
    public class Scene
    {
        public JsonNode? _gltf;
        public Memory<byte> _bin;

        public bool LoadPath(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            switch (Path.GetExtension(path).ToLower())
            {
                case ".glb":
                    return LoadGlb(File.ReadAllBytes(path));

                default:
                    throw new NotImplementedException();
            }
        }

        public bool LoadGlb(Memory<byte> bytes)
        {
            bytes = Glb.Header(bytes);
            GlbChunk jsonChunk;
            (bytes, jsonChunk) = Glb.Chunk(bytes);
            GlbChunk binChunk;
            (bytes, binChunk) = Glb.Chunk(bytes);

            return Load(jsonChunk.Data, binChunk.Data);
        }

        public bool Load(Memory<byte> json, Memory<byte> bin)
        {
            _gltf = JsonNode.Parse(json.Span);
            _bin = bin;
            return _gltf != null;
        }
    }
}
