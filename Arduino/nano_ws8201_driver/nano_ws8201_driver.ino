//#define BLUE 0xFF0000
//#define GREEN 0x00FF00
//#define RED 0x0000FF
//#define YELLOW 0x00FFFF
//#define OFF 0x000000

#define LED_COUNT 50

long matrix_colors[LED_COUNT];
char input[LED_COUNT*3];

int clk = 7;
int dat = 8;

void post_frame();
long btol(byte bytes[3]);

void setup() 
{
  pinMode(clk, OUTPUT);
  pinMode(dat, OUTPUT);
  Serial.begin(115200);
  Serial.setTimeout(20);
}

void loop() 
{  
  int i;
  if (Serial.available() > 0)
  {
    byte size = Serial.readBytes(input, LED_COUNT*3);
    
    for(i = 0; i < LED_COUNT; i++)
    {
      byte colorbuff[3];
      int cur = i*3;
      colorbuff[0] = input[cur+0];
      colorbuff[1] = input[cur+1];
      colorbuff[2] = input[cur+2];
      matrix_colors[i] = btol(colorbuff)-0x010101; //redundant?
    }

    post_frame();
  }
}

long btol(byte bytes[3])
{
  long l_value;
  l_value += (long)bytes[0] << 16;
  l_value += (long)bytes[1] << 8;
  l_value += (long)bytes[2];
  return l_value;
}

void post_frame()
{
  int LED_number;
  for (LED_number = 0 ; LED_number < LED_COUNT; LED_number++)
  {
    long this_led_color = matrix_colors[LED_number];
    unsigned char color_bit;

    for (color_bit = 23 ; color_bit != 255 ; color_bit--)
    {
      digitalWrite(clk, LOW);

      long mask = 1L << color_bit;

      if (this_led_color & mask)
        digitalWrite(dat, HIGH);
      else
        digitalWrite(dat, LOW);
      digitalWrite(clk, HIGH);
    }
  }
  digitalWrite(clk, LOW);
  delayMicroseconds(500);
}
