using BossMod;
using ImGuiNET;

namespace UIDev;

class PlotTest : TestWindow
{
    private readonly UIPlot _plot = new();

    public PlotTest() : base("Plot test", new(400, 400), ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        _plot.DataMin = new(-180, 0);
        _plot.DataMax = new(180, 60);
        _plot.TickAdvance = new(45, 5);
    }

    public override void Draw()
    {
        _plot.Begin();
        _plot.Point(new(-45, 1), Colors.TextColor1, () => "first");
        _plot.Point(new(45, 10), Colors.TextColor4, () => "second");
        _plot.End();
    }
}
