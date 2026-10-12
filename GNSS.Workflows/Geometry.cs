namespace GNSS.Workflows;

public static class Geometry
{
    public static (double x, double y)[] CircleCircleIntersection(double x0, double y0, double r0, double x1, double y1, double r1)
    {
        // Determain the distance between the circle centers.
        double dx = x1 - x0;
        double dy = y1 - y0;
        double d = Math.Sqrt(dx * dx + dy * dy);

        // The circles do not intersect or one circle is completely contained within the other.
        if (d > r0 + r1 || d <= Math.Abs(r0 - r1))
        {
            return [];
        }

        // The radial center is the point where the line going through the 2
        // intersection points intersects the line between the circle centers.

        // Determine the distance from point 0 to the radial center.
        double a = ((r0 * r0) - (r1 * r1) + (d * d)) / (2 * d);

        // Determine the coordinates of the radial center.
        double x2 = x0 + (dx * a / d);
        double y2 = y0 + (dy * a / d);

        // Determine the distance from the radial center to the intersection points.
        double h = Math.Sqrt((r0 * r0) - (a * a));

        // Determine the offsets of the intersection points from the radial center.
        double rx = -dy * (h / d);
        double ry = dx * (h / d);

        (double, double) p1 = (x2 + rx, y2 + ry);
        (double, double) p2 = (x2 - rx, y2 - ry);
        return h == 0 ? [p1] : [p1, p2];
    }
}
