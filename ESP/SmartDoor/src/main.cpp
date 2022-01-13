#include <ESP8266WiFi.h>
#include <PubSubClient.h>

// Update these with values suitable for your network.

const char* ssid = "SSID";
const char* password = "password";
const char* mqtt_server = "Ip";
const char* mqtt_identifier = "vrata";
const char* mqtt_password = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJtcXR0IjoidnJhdGEiLCJleHAiOjE2NTc0ODAxOTh9.teAZ-fBAmbXS2idTWcIajALoIcArhZ185AUYYidJtY9RwjRBYRxXykZVha801kbWAEuxK8I91FpC_rGKDFG5fw";

const int PIN_RELAY = D1;

WiFiClient espClient;
PubSubClient client(espClient);
unsigned long lastMsg = 0;
#define MSG_BUFFER_SIZE	(50)
char msg[MSG_BUFFER_SIZE];
int value = 0;
unsigned long turnoff = -1;

void setup_wifi() {

  delay(10);
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);

  bool led = true;
  while (WiFi.status() != WL_CONNECTED) {
    delay(150);
    digitalWrite(BUILTIN_LED, led ? HIGH : LOW);
    led = !led;
  }

  randomSeed(micros());

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
}

void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();

  if ((char)payload[0] == '1')
  {
    digitalWrite(PIN_RELAY, HIGH);
      Serial.println("A");
    turnoff = millis() + 2000;
  }
  if ((char)payload[0] == '0')
  {
    digitalWrite(PIN_RELAY, LOW);
      Serial.println("B"); 
  }

//   // Switch on the LED if an 1 was received as first character
//   if ((char)payload[0] == '1') {
//     digitalWrite(BUILTIN_LED, LOW);   // Turn the LED on (Note that LOW is the voltage level
//     // but actually the LED is on; this is because
//     // it is active low on the ESP-01)
//   } else {
//     digitalWrite(BUILTIN_LED, HIGH);  // Turn the LED off by making the voltage HIGH
//   }

}

void reconnect() {
  // Loop until we're reconnected
  digitalWrite(BUILTIN_LED, LOW);
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Attempt to connect
    if (client.connect(mqtt_identifier, mqtt_identifier, mqtt_password)) {
      Serial.println("connected");
      // Once connected, publish an announcement...
      client.publish("state", "hello world");
      // ... and resubscribe
      client.subscribe("state");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}

void setup() {
  

  pinMode(BUILTIN_LED, OUTPUT);     // Initialize the BUILTIN_LED pin as an output
  pinMode(PIN_RELAY, OUTPUT);
  Serial.begin(9600);
  setup_wifi();
  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);

  
}

void loop() {

  if (!client.connected()) {
    reconnect();
    client.publish("state", "0");
  }
  client.loop();

  unsigned long now = millis();
//   if (now - lastMsg > 2000) {
//     lastMsg = now;
//     ++value;
//     snprintf (msg, MSG_BUFFER_SIZE, "hello world #%ld", value);
//     Serial.print("Publish message: ");
//     Serial.println(msg);
//     client.publish("state", msg);
//   }
  if (now > turnoff && turnoff != -1)
  {
    digitalWrite(PIN_RELAY, LOW);
    turnoff = -1;
    client.publish("state", "0");
  }

  if (now % 5000 < 25)
    digitalWrite(BUILTIN_LED, LOW);
  else
    digitalWrite(BUILTIN_LED, HIGH);
}