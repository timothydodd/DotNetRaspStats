using SkiaSharp;

public class ScreenStack
{
    public int FontSize = 13;
    public int Padding = 2;
    private readonly List<string> Items = new();
    private int GraphIndex = 0;
    private Graph? graph;
    public void Add(string item)
    {
        Items.Add(item);
    }
    public void AddGraph(Graph graph)
    {
        this.graph = graph;
        GraphIndex = Items.Count - 1;
    }
    public void Reset()
    {
        Items.Clear();
        graph = null;
    }
    public void Draw(SKCanvas canvas, SKPaint paint)
    {
        int y = FontSize;
        for (var i = 0; i < Items.Count; i++)
        {
            canvas.DrawText(Items[i], 0, y, paint);
            y += Padding;
            if (i == 0)
            {
                y += Padding;
            }

            if (i == GraphIndex && graph != null)
            {
                graph.Draw(canvas, paint, y);
            }
            y += FontSize;
        }
    }
}
