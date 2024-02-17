using BlazorSvgEditor.SvgEditor.Misc;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor : IAsyncDisposable
{
    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = null!; //DI

    private Lazy<Task<IJSObjectReference>> moduleTask = null!; //wird gesetzt in OnInitialized


    private JsBoundingBox _containerBoundingBox = new();
    private ElementReference _containerElementReference;
    private ElementReference _baseImageReference;
    private ElementReference _targetImageReference;

    private async Task<JsBoundingBox> GetBoundingBox(ElementReference elementReference)
    {
        var module = await moduleTask.Value;

        return await module.InvokeAsync<JsBoundingBox>("getBoundingBox", elementReference);
    }

    private async Task SetContainerBoundingBox()
    {
        if (_containerElementReference.Id == null)
            throw new Exception("ContainerElementReference or SvgElementReference is null");

        _containerBoundingBox = await GetBoundingBox(_containerElementReference);
    }

    // todo
    public string? ImageResult { get; set; }

    public async Task<byte[]> SaveImage()
    {
        var module = await moduleTask.Value;
        // todo unselect if shape is selected // todo not working
        SelectedShapeId = 0;

        string base64Data = await module.InvokeAsync<string>("svgToBase64", _baseImageReference, _targetImageReference, ImageSize.Width, ImageSize.Height);
        byte[] data = Convert.FromBase64String(base64Data[(base64Data.LastIndexOf(',') + 1)..]);
        ImageResult = base64Data;
        Refresh();
        return data;
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}