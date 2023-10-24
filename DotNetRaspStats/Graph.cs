using SkiaSharp;

public class Graph
{

    private byte[] items = new byte[57];
    private int index = 0;
    private int gMax;
    private int iMax;
    private int height;
    //SKPaint paint;
    public Graph(int height)
    {
        gMax = items.GetUpperBound(0);
        index = gMax;
        this.height = height;
        // paint = new()
        // {
        //     Color = new SKColor(255, 255, 255),
        //     StrokeWidth = 1,
        //     Style = SKPaintStyle.StrokeAndFill,
        // };
    }
    public void Update(byte percent)
    {

        if (percent > 100)
            percent = 100;

        items[index] = (byte)percent;
        index++;
        if (iMax < gMax)
            iMax++;
        if (index > gMax)
            index = 0;
    }
    public void Draw(SKCanvas canvas, SKPaint paint, int y)
    {
        int x = 70;
        int y2 = y - height;
        for (var i = 0; i < gMax; i++)
        {
            var v = (items[i] / (float)100) * height;
            var yo = height - v;
            canvas.DrawRect(x + i, y2 + yo, 1, v, paint);
        }
    }
}