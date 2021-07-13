#include<Wire.h>
#include<Adafruit_MCP4725.h>
Adafruit_MCP4725 dac;
#define read1 A1
#define read2 A2
String DACbitSTR;
int loopCnt;
int loopCnt2;
float average1;
float average2;
void setup() {
  Serial.begin(115200);
  dac.begin(0x60);
  pinMode(read1, INPUT);
  pinMode(read2, INPUT);
  dac.setVoltage(0, false);
  loopCnt = 0;
  loopCnt2 = 0;
  average1 = 0;
  average2 = 0;
}
void loop() {
  float value1 = analogRead(read1);
  float value2 = analogRead(read2);
  average1 += value1;
  average2 += value2;
  if (loopCnt >= 425) {
    average1 = average1 / loopCnt;
    int average11 = average1;
    Serial.print(average11);
    Serial.print("/");
    average2 = average2 / loopCnt;
    int average22 = average2;
    Serial.println(average22);
    average1 = 0;
    average2 = 0;
    loopCnt = 0;
  }

  while (Serial.available() > 0) {
    char chr = Serial.read();
    if (chr == '\n') {
      int dacInt = DACbitSTR.toInt();
      DACbitSTR = "";
      dac.setVoltage(dacInt, false);
      loopCnt2 = 0;
    }
    else {
      DACbitSTR += chr;
    }
  }

  if (loopCnt2 >= 6001) {
    dac.setVoltage(0, false);
    loopCnt2 = 0;
  }

  loopCnt++;
  loopCnt2++;
}
