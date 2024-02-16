using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.Misc;
using BlazorSvgEditor.SvgEditor.ShapeEditors;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor.Shapes;

public class NumberMarker : Shape
{
    internal override Type Presenter => typeof(NumberMarkerEditor);
    public override ShapeType ShapeType => ShapeType.NumberMarker;

    // Own Properties
    public double Cx { get; set; }
    public double Cy { get; set; }
    public double R { get; set; } = 20;
    public double ArrowX { get; set; }
    public double ArrowY { get; set; }
    public string TextColor { get; set; } = "white";
    public string TextNumber { get; set; }

    public NumberMarker(SvgEditor svgEditor) : base(svgEditor) { }

    protected override bool Filled => true;
    protected override BoundingBox Bounds => new BoundingBox(Cx - R, Cy - R, Cx + R, Cy + R);

    internal override void SnapToInteger()
    {
        Cx = Cx.ToInt();
        Cy = Cy.ToInt();
        ArrowX = ArrowX.ToInt();
        ArrowY = ArrowY.ToInt();
        R = R.ToInt();
    }

    bool isMoved = false;

    internal override void HandlePointerMove(PointerEventArgs eventArgs)
    {
        var point = SvgEditor.DetransformPoint(eventArgs.OffsetX, eventArgs.OffsetY);

        switch (SvgEditor.EditMode)
        {
            case EditMode.Add:
                {
                    Coord<double> arrowPoint = MathHelper.PointOnCircle(Cx, Cy, point, R);
                    ArrowX = arrowPoint.X.Round();
                    ArrowY = arrowPoint.Y.Round();

                    break;
                }
            case EditMode.Move:
                {
                    var diff = (point - SvgEditor.MoveStartDPoint);
                    var result = BoundingBox.GetAvailableMovingCoord(SvgEditor.ImageBoundingBox, Bounds, diff);

                    Cx = (Cx + result.X).Round();
                    Cy = (Cy + result.Y).Round();
                    ArrowX = (ArrowX + result.X).Round();
                    ArrowY = (ArrowY + result.Y).Round();

                    isMoved = true;
                    break;
                }
            case EditMode.MoveAnchor:
                {
                    SvgEditor.SelectedAnchorIndex ??= 0;

                    Coord<double> arrowPoint = MathHelper.PointOnCircle(Cx, Cy, point, R);
                    ArrowX = arrowPoint.X.Round();
                    ArrowY = arrowPoint.Y.Round();

                    break;
                }
        }
    }

    internal override async Task HandlePointerUp(PointerEventArgs eventArgs)
    {
        if (SvgEditor.EditMode == EditMode.Move && isMoved)
        {
            isMoved = false;
            await FireOnShapeChangedMove();
        }
        else if (SvgEditor.EditMode == EditMode.MoveAnchor)
            await FireOnShapeChangedEdit();

        SvgEditor.EditMode = EditMode.None;
    }

    internal override void HandlePointerOut(PointerEventArgs eventArgs)
    {
        throw new NotImplementedException();
    }
}