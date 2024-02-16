using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.Misc;
using BlazorSvgEditor.SvgEditor.Shapes;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    private ShapeType ShapeType { get; set; } = ShapeType.None; 

    public void SelectShape(Shape shape, PointerEventArgs eventArgs)
    {
        SelectedShape?.UnSelectShape();
        
        SelectedShape = shape;      //Das neue Shape wird ausgewählt
        SelectedShape.SelectShape(); //Das neue Shape wird ausgewählt

        EditMode = EditMode.Move;
        MoveStartDPoint = DetransformOffset(eventArgs);
        StateHasChanged();
    }
    
    private void AddElement(ShapeType shapeType)
    {
        if(_imageSourceLoading) return;
        
        EditMode = EditMode.AddTool;
        ShapeType = shapeType;
        
        SelectedShape?.UnSelectShape();
        SelectedShape = null;
    }

    private string? _newShapeColor = null;
    private async Task AddToolPointerDown(PointerEventArgs e)
    {
        Shape? newShape = null;
        switch (ShapeType)
        {
            case ShapeType.None:
                return;
            
            case ShapeType.Polygon:
                newShape = new Polygon(this)
                {
                    Points = new List<Coord<double>>()
                    {
                        new(DetransformOffset(e))
                    }
                };
                break;
            
            case ShapeType.Rectangle:
                newShape = new Rectangle(this)
                {
                    X = DetransformOffset(e).X,
                    Y = DetransformOffset(e).Y
                };
                break;
            
            case ShapeType.Circle:
                newShape = new Circle(this)
                {
                    Cx = DetransformOffset(e).X,
                    Cy = DetransformOffset(e).Y
                };
                break;

            case ShapeType.Arrow:
                newShape = new Arrow(this)
                {
                    X1 = DetransformOffset(e).X,
                    Y1 = DetransformOffset(e).Y,
                    X2 = DetransformOffset(e).X + 0.1,
                    Y2 = DetransformOffset(e).Y + 0.1
                };
                break;

            case ShapeType.NumberMarker:
                newShape = new NumberMarker(this)
                {
                    Cx = DetransformOffset(e).X,
                    Cy = DetransformOffset(e).Y
                };
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(ShapeType));
        }

        var newShapeId = -1;
        if (Shapes.Count > 0) newShapeId = Math.Min(Enumerable.Min<Shape>(Shapes, x => x.CustomId) - 1, newShapeId);

        newShape.CustomId = newShapeId;

        if (_newShapeColor != null) newShape.Color = _newShapeColor;
        
        Shapes.Add(newShape);

        SelectedShape = newShape;
        SelectedShape.SelectShape();
                
        EditMode = EditMode.Add;

        await Task.Yield();
    }
    
    public async Task ShapeAddedCompleted(Shape shape)
    {
        SelectedShape = shape;
        SelectedShape.SelectShape();
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeAdded(shape));
    }
}