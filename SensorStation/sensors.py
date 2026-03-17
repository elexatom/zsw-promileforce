import time
import board
import busio
import adafruit_vl53l0x
from gpiozero import DistanceSensor, DigitalInputDevice

# initialize i2c for vl53ldk
i2c = busio.I2C(board.SCL, board.SDA)
tof_sensor = adafruit_vl53l0x.VL53L0X(i2c)

# initialize HC-SR04
ultrasonic = DistanceSensor(echo=24, trigger=23)

# initialize Laser sensor (digital)
laser_sensor = DigitalInputDevice(22)

try:
	while True:
		us_dist_cm = ultrasonic.distance * 100
		tof_dist_mm = tof_sensor.range
		laser_state = "Obstacle" if laser_sensor.value == 0 else "Clear"

		print(f"Ultrasonic: {us_dist_cm:5.1f} cm | ToF: {tof_dist_mm:4d} mm | Laser: {laser_state}")
		time.sleep(0.1)

except KeyboardInterrupt:
	print("Exit")

except ValueError as e:
    print(f"\nI2C Error: {e}")
