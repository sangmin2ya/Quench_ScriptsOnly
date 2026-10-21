using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 삼각형 집합(메시 표면) 위의 균등 랜덤 샘플러
/// 삼각형을 면적에 비례하는 확률로 고르므로(누적 면적 + 이진 탐색), 작은 삼각형이 몰린 곳이 과밀해지지 않는다.
/// 에디터 툴과 분리된 순수 로직이라 EditMode 테스트로 검증한다.
/// </summary>
public class SurfaceSampler
{
    private readonly List<Vector3> _a = new List<Vector3>();
    private readonly List<Vector3> _b = new List<Vector3>();
    private readonly List<Vector3> _c = new List<Vector3>();
    // i번째까지의 삼각형 면적 합 (삼각형이 많아도 작은 면적이 묻히지 않도록 double)
    private readonly List<double> _cumulativeArea = new List<double>();

    public int TriangleCount => _a.Count;
    public double TotalArea => _cumulativeArea.Count == 0 ? 0.0 : _cumulativeArea[_cumulativeArea.Count - 1];

    /// <summary>
    /// 삼각형 추가. 면적이 0이거나 잘못된 값(NaN)인 삼각형은 무시
    /// </summary>
    public void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        float area = Vector3.Cross(b - a, c - a).magnitude * 0.5f;
        if (!(area > 0f))
        {
            return;
        }
        _a.Add(a);
        _b.Add(b);
        _c.Add(c);
        _cumulativeArea.Add(TotalArea + area);
    }

    /// <summary>
    /// 삼각형을 XZ 사각형 영역으로 잘라낸 부분만 추가 (청크 경계에 걸친 삼각형용)
    /// 잘린 다각형은 부채꼴로 다시 삼각형 분할하며, 높이(Y)는 원래 삼각형 평면 위에서 보간된다.
    /// </summary>
    public void AddTriangleClippedXZ(Vector3 a, Vector3 b, Vector3 c, float minX, float maxX, float minZ, float maxZ)
    {
        List<Vector3> polygon = new List<Vector3> { a, b, c };
        polygon = ClipAxis(polygon, minX, true, true);
        polygon = ClipAxis(polygon, maxX, true, false);
        polygon = ClipAxis(polygon, minZ, false, true);
        polygon = ClipAxis(polygon, maxZ, false, false);
        for (int i = 1; i + 1 < polygon.Count; i++)
        {
            AddTriangle(polygon[0], polygon[i], polygon[i + 1]);
        }
    }

    // Sutherland–Hodgman: 한 축의 경계 한쪽만 남김
    private static List<Vector3> ClipAxis(List<Vector3> polygon, float limit, bool isX, bool keepGreater)
    {
        List<Vector3> result = new List<Vector3>();
        for (int i = 0; i < polygon.Count; i++)
        {
            Vector3 current = polygon[i];
            Vector3 next = polygon[(i + 1) % polygon.Count];
            float currentValue = isX ? current.x : current.z;
            float nextValue = isX ? next.x : next.z;
            bool currentInside = keepGreater ? currentValue >= limit : currentValue <= limit;
            bool nextInside = keepGreater ? nextValue >= limit : nextValue <= limit;

            if (currentInside)
            {
                result.Add(current);
            }
            if (currentInside != nextInside)
            {
                float t = (limit - currentValue) / (nextValue - currentValue);
                Vector3 crossing = current + (next - current) * t;
                // 경계 값을 정확히 맞춰 다음 축 클리핑에서 부동소수 오차로 빠지지 않도록
                if (isX) crossing.x = limit; else crossing.z = limit;
                result.Add(crossing);
            }
        }
        return result;
    }

    /// <summary>
    /// 면적 가중으로 삼각형을 고르고 그 안의 균등한 점을 반환
    /// </summary>
    public Vector3 Sample(System.Random random)
    {
        if (TriangleCount == 0)
        {
            throw new InvalidOperationException("샘플링할 삼각형이 없습니다.");
        }
        int index = PickTriangle(random.NextDouble() * TotalArea);
        return RandomPointInTriangle(_a[index], _b[index], _c[index], (float)random.NextDouble(), (float)random.NextDouble());
    }

    // 누적 면적에서 target을 처음 넘는 삼각형을 이진 탐색
    private int PickTriangle(double target)
    {
        int low = 0;
        int high = _cumulativeArea.Count - 1;
        while (low < high)
        {
            int mid = (low + high) / 2;
            if (_cumulativeArea[mid] <= target)
            {
                low = mid + 1;
            }
            else
            {
                high = mid;
            }
        }
        return low;
    }

    /// <summary>
    /// 삼각형 내부의 균등한 점. r1 + r2 > 1이면 접어서 삼각형 안으로 보낸다
    /// </summary>
    public static Vector3 RandomPointInTriangle(Vector3 a, Vector3 b, Vector3 c, float r1, float r2)
    {
        if (r1 + r2 > 1)
        {
            r1 = 1 - r1;
            r2 = 1 - r2;
        }
        float r3 = 1 - r1 - r2;
        return r1 * a + r2 * b + r3 * c;
    }
}
