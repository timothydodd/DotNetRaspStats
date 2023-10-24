using Sang.IoT.SSD1306;

using (SSD1306_128_64 oled = new(1))
{
    oled.Begin();
    oled.Image("assets/test.png");
    oled.Display();
}