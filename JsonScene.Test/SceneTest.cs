using System;
using System.IO;
using NUnit.Framework;

namespace JsonScene.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestAvogado()
    {
        var dir = System.Environment.GetEnvironmentVariable("GLTF_SAMPLE_MODELS");
        if (dir == null)
        {
            throw new System.Exception();
        }

        var path = Path.Combine(dir, "2.0/Avocado/glTF-Binary/Avocado.glb");
        Assert.True(File.Exists(path));

        Memory<byte> bytes = File.ReadAllBytes(path);

        bytes = Glb.Header(bytes);
        {
            GlbChunk chunk;
            (bytes, chunk) = Glb.Chunk(bytes);
            Assert.AreEqual(Glb.JSON_CHUNK_TYPE, chunk.ChunkType);
        }
        {
            GlbChunk chunk;
            (bytes, chunk) = Glb.Chunk(bytes);
            Assert.AreEqual(Glb.BIN_CHUNK_TYPE, chunk.ChunkType);
        }
        Assert.AreEqual(0, bytes.Length);
    }
}
