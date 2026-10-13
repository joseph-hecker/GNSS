using NUnit.Framework.Constraints;

namespace GNSS.Workflows.Test;

public class GeometryTests
{
    private const double DELTA = 1e-9;

    private static RangeConstraint IsAlmost(double expected) => Is.InRange(expected - DELTA, expected + DELTA);

    [Test, TestOf(nameof(Geometry.CircleCircleIntersection))]
    [TestCaseSource(typeof(GeometrySource), nameof(GeometrySource.CircleCircleIntersectionSource))]
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
