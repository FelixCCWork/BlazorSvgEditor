using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.Misc;
using BlazorSvgEditor.SvgEditor.Shapes;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    // Methods for component communication
    public async Task AddExistingShape(Shape shape)
    {
        Shapes.Add(shape);
        StateHasChanged();
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeAdded(shape));
    }
    
    public void AddNewShape(ShapeType shapeType, string? color = null)
    {
        EditMode = EditMode.AddTool;
        ShapeType = shapeType;
        
        _newShapeColor = color;
        
        SelectedShape?.UnSelectShape();
        SelectedShape = null;
    }
    
    public async Task RemoveSelectedShape()
    {
        if (SelectedShape != null)
        {
            int deletedShapeId = SelectedShape.CustomId;
            Shapes.Remove(SelectedShape);

            if (SelectedShape is NumberMarker marker)
                OnMarkerRemoved(marker);

            SelectedShape = null;
            SelectedAnchorIndex = null;

            await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeDeleted(deletedShapeId));
        }
        else
        {
            if (ShowDiagnosticInformation)
                Console.WriteLine("No shape selected - so nothing to delete");
        }
    }
    
    public async Task RemoveShape(int shapeId)
    {
        Shape? shape = Shapes.FirstOrDefault(s => s.CustomId == shapeId);
        if (shape != null)
        {
            Shapes.Remove(shape);

            if (shape is NumberMarker marker)
                OnMarkerRemoved(marker);

            await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeDeleted(shapeId));
        }
        else
        {
            if(ShowDiagnosticInformation) 
                Console.WriteLine("Shape with id " + shapeId + " not found - so nothing to delete");
        }
    }
    
    public async Task ClearShapes()
    {
        Shapes.Clear();
        OnClearMarkers();
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapesCleared());
    }
    
    public async Task ResetTransform()
    {
        await SetContainerBoundingBox();
        await ResetTransformation();
    }
    
    //Use this method to set the translation to a specific value -> e.g. to syncronize the translation of two SvgEditors
    public void SetTranslateAndScale(Coord<double>? newTranslate = null, double? newScale = null)
    {
        if(newTranslate != null) Translate = newTranslate.Value;
        if (newScale != null) Scale = newScale.Value;
        StateHasChanged();
    }
    public (Coord<double> translation, double scale) GetTranslateAndScale() => (Translate, Scale);

    public async Task ReloadImage()
    {
        _imageSourceLoading = true;
        StateHasChanged();

        (string imageSource, int width, int height) result;
        if (ImageSourceLoadingFunc != null)
        {
            result = await ImageSourceLoadingFunc();
            ImageSize = (result.width, result.height);
            ImageSource = result.imageSource;
        }
        
        _imageSourceLoading = false;
        StateHasChanged();
    }
    
    public void Refresh() => StateHasChanged();
}