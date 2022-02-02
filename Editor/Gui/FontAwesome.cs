using System.Net;

namespace Editor
{
    public static class FontAwesome47
    {
        public const string URL = "https://github.com/FortAwesome/Font-Awesome/raw/v4.7.0/fonts/fontawesome-webfont.ttf";

        public static string CachePath => Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "fontawesome-webfont.ttf");

        public static byte[] GetOrDownload()
        {
            var path = CachePath;
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            // download
            var request = WebRequest.Create(URL);
            request.Method = "GET";
            using var webResponse = request.GetResponse();
            using var webStream = webResponse.GetResponseStream();
            var buffer = new List<byte>();
            var tmp = new byte[1024 * 1024];
            while (true)
            {
                var size = webStream.Read(tmp, 0, tmp.Length);
                if (size == 0)
                {
                    break;
                }
                buffer.AddRange(tmp.Take(size));
            }
            var data = buffer.ToArray();
            File.WriteAllBytes(path, data);
            return data;
        }
    }
}
