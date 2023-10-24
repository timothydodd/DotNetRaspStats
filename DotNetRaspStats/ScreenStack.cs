using SkiaSharp;

public class ScreenStack
{
    public int FontSize = 13;
    public int Padding = 2;


    List<string> Items = new List<string>();
    public void Add(string item)
    {
        Items.Add(item);
    }
    public void Reset()
    {
        Items.Clear();
    }
    public void Draw(SKCanvas canvas, SKPaint paint)
    {
        int y = Padding + FontSize;
        for (var i = 0; i < Items.Count; i++)
        {
            canvas.DrawText(Items[i], 0, y, paint);
            y += Padding + FontSize;
        }
    }
}