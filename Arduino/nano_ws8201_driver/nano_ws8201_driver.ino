#include "FastLED.h"

#define LED_PIN     5
#define NUM_LEDS    71
#define LED_TYPE    WS2812B
#define COLOR_ORDER GRB

uint8_t ledDataSize = NUM_LEDS * 3;

CRGB leds[NUM_LEDS];

void handleSerialInput();
byte voidOne(byte possibleOne);

void setup() 
{  
  delay( 1000 ); // power-up safety delay
  
  Serial.begin(115200);
  Serial.setTimeout(20);
  
  FastLED.addLeds<LED_TYPE, LED_PIN, COLOR_ORDER>(leds, NUM_LEDS).setCorrection( TypicalLEDStrip );
}

void loop() 
{  
  handleSerialInput();
}

void handleSerialInput() {  
  if (Serial.available() > 0)
  {      
    char input[ledDataSize];
    Serial.readBytes(input, ledDataSize + 1);

    String prompt = input;    
    if (prompt.startsWith("init")) { // answer discovery prompt
      Serial.print("ambilight");
    } else { // display LEDS       
      for(uint8_t i = 0; i < NUM_LEDS; i++)
      {
        uint8_t cur = i * 3;
        byte r = voidOne( input[cur + 0] );
        byte g = voidOne( input[cur + 1] );
        byte b = voidOne( input[cur + 2] );
        leds[i] = CRGB( b, g, r );
      }

      FastLED.show();
    }
  }
}

byte voidOne(byte possibleOne) {
  return possibleOne == 1 ? 0 : possibleOne;
}
