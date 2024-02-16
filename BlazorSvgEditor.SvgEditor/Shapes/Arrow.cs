using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.Misc;
using BlazorSvgEditor.SvgEditor.ShapeEditors;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor.Shapes;

public class Arrow : Shape
{
	public Arrow(SvgEditor svgEditor) : base(svgEditor) { }

	internal override Type Presenter => typeof(ArrowEditor);
	public override ShapeType ShapeType => ShapeType.Arrow;

	public double X1 { get; set; }
	public double Y1 { get; set; }
	public double X2 { get; set; }
	public double Y2 { get; set; }

	private Coord<double> AddPosition = new(-1, -1);

	protected override BoundingBox Bounds => new(Math.Min(X1, X2), Math.Min(Y1, Y2), Math.Max(X1, X2) - Math.Min(X1, X2), Math.Max(Y1, Y2) - Math.Min(Y1, Y2));

	internal override void SnapToInteger()
	{
		X1 = X1.ToInt();
		Y1 = Y1.ToInt();
		X2 = X2.ToInt();
		Y2 = Y2.ToInt();
	}

	bool isMoved = false;
	internal override void HandlePointerMove(PointerEventArgs eventArgs)
	{
		var point = SvgEditor.DetransformPoint(eventArgs.OffsetX, eventArgs.OffsetY);
		Coord<double> resultCoord = BoundingBox.GetAvailableResultCoord(SvgEditor.ImageBoundingBox, point);

		switch (SvgEditor.EditMode)
		{
			case EditMode.Add:

				if (AddPosition.X.IsEqual(-1)) AddPosition = new Coord<double>(X1, Y1);

				X2 = resultCoord.X;
				Y2 = resultCoord.Y;

				// todo necessary?
				//if (Width < 1) Width = 1;
				//if (Height < 1) Height = 1;

				break;

			case EditMode.Move:
				var diff = (point - SvgEditor.MoveStartDPoint);
				var result = BoundingBox.GetAvailableMovingCoord(SvgEditor.ImageBoundingBox, Bounds, diff);

				X1 += result.X;
				X2 += result.X;
				Y1 += result.Y;
				Y2 += result.Y;

				isMoved = true;
				break;

			case EditMode.MoveAnchor:
				SvgEditor.SelectedAnchorIndex ??= 0;

				switch (SvgEditor.SelectedAnchorIndex)
				{
					case 0:
						X1 = resultCoord.X;
						Y1 = resultCoord.Y;
						break;
					case 1:
						X2 = resultCoord.X;
						Y2 = resultCoord.Y;
						break;
				}

				// todo necessary?
				//if (Width < 0)
				//{
				//	Width = -Width;
				//	X -= Width;
				//	if (SvgEditor.SelectedAnchorIndex == 0) SvgEditor.SelectedAnchorIndex = 1;
				//	else if (SvgEditor.SelectedAnchorIndex == 1) SvgEditor.SelectedAnchorIndex = 0;
				//	else if (SvgEditor.SelectedAnchorIndex == 2) SvgEditor.SelectedAnchorIndex = 3;
				//	else if (SvgEditor.SelectedAnchorIndex == 3) SvgEditor.SelectedAnchorIndex = 2;
				//}
				//if (Height < 0)
				//{
				//	Height = -Height;
				//	Y -= Height;
				//	if (SvgEditor.SelectedAnchorIndex == 0) SvgEditor.SelectedAnchorIndex = 3;
				//	else if (SvgEditor.SelectedAnchorIndex == 1) SvgEditor.SelectedAnchorIndex = 2;
				//	else if (SvgEditor.SelectedAnchorIndex == 2) SvgEditor.SelectedAnchorIndex = 1;
				//	else if (SvgEditor.SelectedAnchorIndex == 3) SvgEditor.SelectedAnchorIndex = 0;
				//}

				//if (Width < 1) Width = 1;
				//if (Height < 1) Height = 1;

				break;
		}
	}

	internal override async Task HandlePointerUp(PointerEventArgs eventArgs)
	{
		if (SvgEditor.EditMode == EditMode.Add)
		{
			// todo necessary?
			//if (Width < 1) Width = 1;
			//if (Height < 1) Height = 1;
			await Complete();
		}

		if (SvgEditor.EditMode == EditMode.Move && isMoved)
		{
			isMoved = false;
			await FireOnShapeChangedMove();
		}
		else if (SvgEditor.EditMode == EditMode.MoveAnchor) await FireOnShapeChangedEdit();

		SvgEditor.EditMode = EditMode.None;
	}

	internal override void HandlePointerOut(PointerEventArgs eventArgs)
	{
		throw new NotImplementedException();
	}
}