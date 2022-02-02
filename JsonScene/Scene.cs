namespace JsonScene
{
    public class Scene
    {
        public bool Load(string path)
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
            throw new NotImplementedException();
        }
    }
}
