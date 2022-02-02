using System.Text.Json.Nodes;

namespace JsonScene
{
    public class Scene
    {
        public JsonObject Gltf { get; }
        Memory<byte> _bin;

        public Scene(JsonObject gltf, Memory<byte> bin)
        {
            Gltf = gltf;
            _bin = bin;
        }

        public static Scene? LoadPath(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            switch (Path.GetExtension(path).ToLower())
            {
                case ".glb":
                    return LoadGlb(File.ReadAllBytes(path));

                default:
                    throw new NotImplementedException();
            }
        }

        public static Scene? LoadGlb(Memory<byte> bytes)
        {
            bytes = Glb.Header(bytes);
            GlbChunk jsonChunk;
            (bytes, jsonChunk) = Glb.Chunk(bytes);
            GlbChunk binChunk;
            (bytes, binChunk) = Glb.Chunk(bytes);

            return Load(jsonChunk.Data, binChunk.Data);
        }

        public static Scene? Load(Memory<byte> json, Memory<byte> bin)
        {
            var gltf = JsonNode.Parse(json.Span);
            if (gltf == null)
            {
                return null;
            }
            return new Scene(gltf.AsObject(), bin);
        }
    }
}
