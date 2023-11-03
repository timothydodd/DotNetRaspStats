using System.Diagnostics;
using Sang.IoT.SSD1306;
using SkiaSharp;
public class Worker : BackgroundService, IDisposable
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly SSD1306_128_64 display;
    private readonly SKPaint paint;
    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        display = new SSD1306_128_64(1);
        var currentFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);
        if (currentFolder == null)
        {
            throw new Exception("Could not find current folder");
        }

        paint = new()
        {
            Color = new SKColor(255, 255, 255),
            StrokeWidth = 1,
            Typeface = SKTypeface.FromFile(Path.Combine(currentFolder, "assets/RobotoRegular-3m4L.ttf")),
            TextSize = 13,
            Style = SKPaintStyle.Fill,
        };
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Up");

        Graph g = new(17);

        ScreenStack stack = new();
        using SKBitmap bitmap = new(128, 64, true);
        using SKCanvas canvas = new(bitmap);
        display.Begin();
        display.Clear();
        var ip = Metrics.GetLocalIPAddress();
        Stopwatch stopWatch = new();
        stopWatch.Start();
        var showName = false;
        string hostName = System.Net.Dns.GetHostName();
        while (stoppingToken.IsCancellationRequested == false)
        {
            MemoryMetrics metrics = Metrics.GetUnixMetrics();
            UptimeMetrics cpuUsage = Metrics.GetCpuMetrics();
            var totalMemory = Metrics.SizeSuffix((long)metrics.Total, 1);
            var usedMemory = Metrics.SizeSuffix((long)metrics.Used, 1);
            g.Update((byte)cpuUsage.CPU);
            if (stopWatch.Elapsed.Seconds > 5)
            {
                showName = !showName;
                stopWatch.Restart();
            }
            if (showName)
            {
                stack.Add(hostName);
            }
            else
            {
                stack.Add($"IP:{ip}");
            }
            stack.Add($"CPU: {cpuUsage.CPU}%");
            stack.AddGraph(g);
            stack.Add($"Mem: {usedMemory} / {totalMemory}");
            stack.Add($"Up: {cpuUsage.UpTime}");
            canvas.Clear(SKColors.Black);
            stack.Draw(canvas, paint);
            display.Image(bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray());


            display.Display();
            stack.Reset();
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            await Task.Delay(3000, stoppingToken);
        }

    }
    public override void Dispose()
    {
        Console.WriteLine("Clearing Screen");
        display.Begin();
        display.Clear();
        display.Display();
        display.Dispose();
        base.Dispose();
    }
}
