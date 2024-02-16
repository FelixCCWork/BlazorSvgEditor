namespace BlazorSvgEditor.SvgEditor.Helper;

internal static class MathHelper
{
    public static double RadiusFromPointAndCenter(double centerX, double centerY, double pointOnCircleX, double pointOnCircleY)
    {
        var dx = pointOnCircleX - centerX;
        var dy = pointOnCircleY - centerY;

        return Math.Sqrt(dx * dx + dy * dy);
    }

    public static Coord<double> PointOnCircle(double centerX, double centerY, double radius, double angleInDegrees)
    {
        double angleInRadians = (Math.PI / 180) * angleInDegrees;

        double x = centerX + radius * Math.Cos(angleInRadians);
        double y = centerY + radius * Math.Sin(angleInRadians);

        return new Coord<double>(x, y);
    }

    public static Coord<double> PointOnCircle(double centerX, double centerY, Coord<double> outsidePoint, double radius)
    {
        double dx = outsidePoint.X - centerX;
        double dy = outsidePoint.Y - centerY;

        double magnitude = Math.Sqrt(dx * dx + dy * dy);

        double directionX = dx / magnitude;
        double directionY = dy / magnitude;

        double intersectionX = centerX + directionX * radius;
        double intersectionY = centerY + directionY * radius;

        return new Coord<double>(intersectionX, intersectionY);
    }

    public static Coord<double> PointOnCircle(double centerX, double centerY, double outsidePointX, double outsidePointY, double radius)
    {
        double dx = outsidePointX - centerX;
        double dy = outsidePointY - centerY;

        double magnitude = Math.Sqrt(dx * dx + dy * dy);

        double directionX = dx / magnitude;
        double directionY = dy / magnitude;

        double intersectionX = centerX + directionX * radius;
        double intersectionY = centerY + directionY * radius;

        return new Coord<double>(intersectionX, intersectionY);
    }
}
