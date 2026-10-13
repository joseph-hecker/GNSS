namespace GNSS.Workflows.Test;

public static class GeometrySource
{
    public static IEnumerable<TestCaseData> CircleCircleIntersectionSource()
    {
        // No intersections
        yield return new TestCaseData(0, 0, 1, 0, 0, 1, Array.Empty<(double, double)>());
        yield return new TestCaseData(0, 0, 1, 0, 0, 2, Array.Empty<(double, double)>());
        yield return new TestCaseData(0, 1, 1, 0, 0, 4, Array.Empty<(double, double)>());
        yield return new TestCaseData(0, 0, 1, 0, 3, 1, Array.Empty<(double, double)>());
        yield return new TestCaseData(3, 0, 1, 0, 0, 1, Array.Empty<(double, double)>());
        yield return new TestCaseData(0, 0, 1, 0, -3, 1, Array.Empty<(double, double)>());
        yield return new TestCaseData(-3, 0, 1, 0, 0, 1, Array.Empty<(double, double)>());

        // 1 intersection
        yield return new TestCaseData(0, 0, 1, 0, 2, 1, new (double, double)[] { (0, 1) });
        yield return new TestCaseData(0, 0, 1, 2, 0, 1, new (double, double)[] { (1, 0) });
        yield return new TestCaseData(0, 0, 1, 0, -2, 1, new (double, double)[] { (0, -1) });
        yield return new TestCaseData(0, 0, 1, -2, 0, 1, new (double, double)[] { (-1, 0) });

        // 2 intersections
        double sqrt3Over2 = Math.Sqrt(3) / 2;
        yield return new TestCaseData(0, 0, 1, 0, 1, 1, new (double, double)[] { (-sqrt3Over2, 0.5), (sqrt3Over2, 0.5) });
        yield return new TestCaseData(0, 0, 1, 1, 0, 1, new (double, double)[] { (0.5, sqrt3Over2), (0.5, -sqrt3Over2) });
        yield return new TestCaseData(0, 0, 1, 0, -1, 1, new (double, double)[] { (sqrt3Over2, -0.5), (-sqrt3Over2, -0.5) });
        yield return new TestCaseData(0, 0, 1, -1, 0, 1, new (double, double)[] { (-0.5, -sqrt3Over2), (-0.5, sqrt3Over2) });
    }
}
