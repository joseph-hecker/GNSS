using NUnit.Framework.Constraints;

namespace GNSS.Workflows.Test;

public class GeometryTests
{
    private const double DELTA = 1e-9;

    private static RangeConstraint IsAlmost(double expected) => Is.InRange(expected - DELTA, expected + DELTA);

    private static IEnumerable<TestCaseData> CircleCircleIntersectionSource()
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

        double sqrt3Over2 = Math.Sqrt(3) / 2;

        // 2 intersections
        yield return new TestCaseData(0, 0, 1, 0, 1, 1, new (double, double)[] { (-sqrt3Over2, 0.5), (sqrt3Over2, 0.5) });
        yield return new TestCaseData(0, 0, 1, 1, 0, 1, new (double, double)[] { (0.5, sqrt3Over2), (0.5, -sqrt3Over2) });
        yield return new TestCaseData(0, 0, 1, 0, -1, 1, new (double, double)[] { (sqrt3Over2, -0.5), (-sqrt3Over2, -0.5) });
        yield return new TestCaseData(0, 0, 1, -1, 0, 1, new (double, double)[] { (-0.5, -sqrt3Over2), (-0.5, sqrt3Over2) });
    }

    [Test, TestOf(nameof(Geometry.CircleCircleIntersection))]
    [TestCaseSource(nameof(CircleCircleIntersectionSource))]
    public void TestCircleCircleIntersection(double x0, double y0, double r0, double x1, double y1, double r1, (double x, double y)[] expected)
    {
        var actual = Geometry.CircleCircleIntersection(x0, y0, r0, x1, y1, r1);

        Assert.That(actual, Has.Length.EqualTo(expected.Length));
        foreach (var (e, a) in expected.Zip(actual))
        {
            Assert.Multiple(() =>
            {
                Assert.That(a.x, IsAlmost(e.x));
                Assert.That(a.y, IsAlmost(e.y));
            });
        }
    }
}
