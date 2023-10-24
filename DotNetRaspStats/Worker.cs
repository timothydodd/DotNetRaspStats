using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sang.IoT.SSD1306;
using SkiaSharp;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    // public override async Task StopAsync(CancellationToken stoppingToken)
    // {
    //     _logger.LogInformation(
    //         "Consume Scoped Service Hosted Service is stopping.");

    //     await base.StopAsync(stoppingToken);
    // }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Up");

        Graph g = new Graph(17);
        using (var oled = new SSD1306_128_64(1))
        {
            var currentFolder = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            Console.WriteLine(currentFolder);
            SKPaint paint = new()
            {
                Color = new SKColor(255, 255, 255),
                StrokeWidth = 1,
                Typeface = SKTypeface.FromFile(Path.Combine(currentFolder, "assets/RobotoRegular-3m4L.ttf")),
                TextSize = 13,
                Style = SKPaintStyle.Fill,
            };
            ScreenStack stack = new();

            var ip = Metrics.GetLocalIPAddress();

            while (stoppingToken.IsCancellationRequested == false)
            {
                MemoryMetrics metrics = Metrics.GetUnixMetrics();
                UptimeMetrics cpuUsage = Metrics.GetCpuMetrics();
                var totalMemory = Metrics.SizeSuffix((long)metrics.Total, 1);
                var usedMemory = Metrics.SizeSuffix((long)metrics.Used, 1);
                g.Update((byte)cpuUsage.CPU);
                stack.Add($"IP:{ip}");
                stack.Add($"CPU: {cpuUsage.CPU}%");
                stack.AddGraph(g);
                stack.Add($"Mem: {usedMemory} / {totalMemory}");
                stack.Add($"Up: {cpuUsage.UpTime}");


                oled.Begin();
                oled.Clear();

                using SKBitmap bitmap = new(128, 64, true);

                using SKCanvas canvas = new(bitmap);

                stack.Draw(canvas, paint);

                oled.Image(bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray());


                oled.Display();
                stack.Reset();
                if (stoppingToken.IsCancellationRequested)
                    break;
                await Task.Delay(2000, stoppingToken);
                // stoppingToken.WaitHandle.WaitOne(5000);
            }

            // Clear Screen on exit
            Console.WriteLine("Exiting");
            oled.Begin();
            oled.Clear();
            oled.Display();
        }
    }
}