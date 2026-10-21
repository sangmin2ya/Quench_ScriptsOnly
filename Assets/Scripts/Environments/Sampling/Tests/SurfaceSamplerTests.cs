using NUnit.Framework;
using UnityEngine;

public class SurfaceSamplerTests
{
    // XZ 평면 위 직각삼각형 (넓이 = size * size / 2)
    private static void AddFlatTriangle(SurfaceSampler sampler, float originX, float size)
    {
        sampler.AddTriangle(new Vector3(originX, 0, 0), new Vector3(originX + size, 0, 0), new Vector3(originX, 0, size));
    }

    [Test]
    public void TriangleIsPickedInProportionToArea()
    {
        var sampler = new SurfaceSampler();
        AddFlatTriangle(sampler, 0, 1);       // 넓이 0.5
        AddFlatTriangle(sampler, 10, 3);      // 넓이 4.5 (9배)
        var random = new System.Random(1);

        int inLarge = 0;
        const int samples = 20000;
        for (int i = 0; i < samples; i++)
        {
            if (sampler.Sample(random).x >= 10) inLarge++;
        }

        // 기대값 0.9, 인덱스 균등 선택이었다면 0.5
        Assert.AreEqual(0.9, (double)inLarge / samples, 0.01);
    }

    [Test]
    public void SamplesStayInsideTriangle()
    {
        var sampler = new SurfaceSampler();
        AddFlatTriangle(sampler, 0, 2);
        var random = new System.Random(7);

        for (int i = 0; i < 5000; i++)
        {
            Vector3 p = sampler.Sample(random);
            Assert.IsTrue(p.x >= 0 && p.z >= 0 && p.x + p.z <= 2.0001f, $"삼각형 밖의 점: {p}");
        }
    }

    [Test]
    public void SameSeedGivesSameResult()
    {
        var sampler = new SurfaceSampler();
        AddFlatTriangle(sampler, 0, 1);
        AddFlatTriangle(sampler, 5, 2);

        var first = new System.Random(42);
        var second = new System.Random(42);
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual(sampler.Sample(first), sampler.Sample(second));
        }
    }

    [Test]
    public void DegenerateTriangleIsIgnored()
    {
        var sampler = new SurfaceSampler();
        sampler.AddTriangle(Vector3.zero, Vector3.one, Vector3.one * 2); // 일직선
        AddFlatTriangle(sampler, 0, 1);

        Assert.AreEqual(1, sampler.TriangleCount);
        Assert.AreEqual(0.5, sampler.TotalArea, 0.0001);
    }

    [Test]
    public void ClippedTriangleKeepsOnlyAreaInsideRect()
    {
        // 넓이 2인 삼각형 (0,0)-(2,0)-(0,2) 중 x <= 1 부분은 넓이 1.5
        var sampler = new SurfaceSampler();
        sampler.AddTriangleClippedXZ(new Vector3(0, 0, 0), new Vector3(2, 0, 0), new Vector3(0, 0, 2), -10, 1, -10, 10);
        Assert.AreEqual(1.5, sampler.TotalArea, 0.0001);

        var random = new System.Random(3);
        for (int i = 0; i < 2000; i++)
        {
            Assert.LessOrEqual(sampler.Sample(random).x, 1.0001f);
        }
    }

    [Test]
    public void LargeTriangleIsSplitAcrossChunksWithoutLoss()
    {
        // 큰 삼각형 하나를 4개 청크로 잘라도 전체 넓이가 보존되고, 경사면 높이도 유지됨
        Vector3 a = new Vector3(0, 0, 0), b = new Vector3(40, 10, 0), c = new Vector3(0, 0, 40);
        double total = 0;
        for (int x = 0; x < 2; x++)
        {
            for (int z = 0; z < 2; z++)
            {
                var chunk = new SurfaceSampler();
                chunk.AddTriangleClippedXZ(a, b, c, x * 20, x * 20 + 20, z * 20, z * 20 + 20);
                total += chunk.TotalArea;
            }
        }
        var whole = new SurfaceSampler();
        whole.AddTriangle(a, b, c);
        Assert.AreEqual(whole.TotalArea, total, 0.01);
    }

    [Test]
    public void TriangleOutsideRectAddsNothing()
    {
        var sampler = new SurfaceSampler();
        sampler.AddTriangleClippedXZ(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1), 5, 6, 5, 6);
        Assert.AreEqual(0, sampler.TriangleCount);
    }

    [Test]
    public void EmptySamplerThrows()
    {
        var sampler = new SurfaceSampler();
        Assert.Throws<System.InvalidOperationException>(() => sampler.Sample(new System.Random(0)));
    }
}
