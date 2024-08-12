#include <SPI.h>
#include <TFT_eSPI.h>
TFT_eSPI tft = TFT_eSPI();  //st7735S ekarn ile kullanacağım ancak siz istediğinizi kullanabilirsiniz.| Im gonna use st7735s in that project but you can use whatever you want
int cpu;
int gpu;
String ramstr;
float ram;
int gputmp;
String port = "";
void setup(void) {
  tft.init();
  tft.setRotation(3);
  tft.fillScreen(TFT_BLACK);
  tft.setCursor(0, 4, 4);
  tft.setTextColor(TFT_SILVER);
  tft.fillRect(1, 1, 158, 76, TFT_BLACK);

  Serial.begin(9600);
}

void loop() {
  if (Serial.available() > 0) {
    port = Serial.readStringUntil('-');                                      //PC'den gelen veriyi yakalama | capturing data from PC
    cpu = port.substring(port.indexOf('!') + 1, port.indexOf('+')).toInt();  //c# uygulamasıdan gelen veriyi decode etme. | decoding data that coems from c# app
    gpu = port.substring(port.indexOf('+') + 1, port.indexOf('*')).toInt();  //c# uygulamasıdan gelen veriyi decode etme. | decoding data that coems from c# app
    ramstr = port.substring(port.indexOf('*') + 1, port.indexOf('?'));       //c# uygulamasıdan gelen veriyi decode etme. | decoding data that coems from c# app
    ramstr.replace(',', '.');                                                //C# floatını arduino formatına çevirme | converting C# float to  arduino format
    ram = ramstr.toFloat();
    gputmp = port.substring(port.indexOf('?') + 1, port.indexOf('-')).toInt();

    //ekrana yazdırma | printing to screen --------------------------------------------------değiştirilebilir | modifiable
    tft.setCursor(12, 17, 2);
    tft.setTextColor(TFT_CYAN, TFT_BLACK);
    tft.println("%" + String(cpu) + "  ");
    tft.drawSmoothArc(25, 25, 22, 19, 0, 360, TFT_DARKGREY, TFT_DARKGREY, true);
    tft.drawSmoothArc(25, 25, 22, 19, 0, map(cpu, 0, 100, 1, 360), TFT_CYAN, TFT_DARKGREY, true);

    tft.setCursor(62, 22, 1);
    tft.setTextColor(TFT_GOLD, TFT_BLACK);
    tft.println(String(ram) + " ");
    tft.drawSmoothArc(75, 25, 22, 19, 0, 360, TFT_DARKGREY, TFT_DARKGREY, true);
    tft.drawSmoothArc(75, 25, 22, 19, 0, map(ram, 0, 32, 1, 360), TFT_ORANGE, TFT_DARKGREY, true);

    tft.setCursor(113, 17, 2);
    tft.setTextColor(TFT_GREEN, TFT_BLACK);
    tft.println("%" + String(gpu) + "  ");
    tft.drawSmoothArc(125, 25, 22, 19, 0, 360, TFT_DARKGREY, TFT_DARKGREY, true);
    tft.drawSmoothArc(125, 25, 22, 19, 0, map(gpu, 0, 100, 1, 360), TFT_GREEN, TFT_DARKGREY, true);


    tft.setCursor(20, 60, 4);
    tft.setTextColor(TFT_GREEN, TFT_BLACK);
    tft.println("GPU:" + String(gputmp) + "c  ");
  }
}
