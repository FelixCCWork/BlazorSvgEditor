using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.Shapes;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    private readonly List<NumberMarker> _numberMarkers = new();

    private void OnMarkerAdded(NumberMarker marker)
    {
        marker.TextNumber = _numberMarkers.Count.ToString();
        _numberMarkers.Add(marker);
    }

    private void OnMarkerRemoved(NumberMarker marker)
    {
        _numberMarkers.Remove(marker);

        foreach((NumberMarker numberMarker, int index) in _numberMarkers.WithIndex())
            numberMarker.TextNumber = index.ToString();
    }

    private void OnClearMarkers()
    {
        _numberMarkers.Clear();
    }
}