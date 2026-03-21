#set page(paper: "a4", margin: 2cm)
#set text(font: "New Computer Modern", size: 12pt)
#set heading(numbering: "1.1.")
#set par(justify: true)

// --- TITLE PAGE ---
#align(center + horizon)[
  #text(size: 24pt, weight: "bold")[Software Requirements Specification] \
  #v(1em)
  #text(size: 18pt)[Project Name: RangeFinder] \
  #v(2em)
]

#align(left + bottom)[
  #text(size: 14pt)[
    *Authors:* Jan Frančák, Jaroslav Vaněček, Jakub Šoma, Tomáš Elexa, Lola Rodríguez \
    *Version:* 1.3\
    *Date:* 2026-03-21\
  ]
]

#pagebreak()

#outline()

#outline(title: "Figures", target: figure.where(kind: image))

#pagebreak()

= Introduction
== Purpose
This document specifies the requirements for the Rangefinder, a Raspberry PI based range-finding tool with a mobile phone app user interface. Its users are able to see the real time measurements of distance together with a chart to easily see any outliers.

== Document conventions
- Requirement IDs: FREQ-x for functional requirements; NFREQ-x for nonfunctional requirements; SCN-x for scenarios.
- Time is stated in seconds; availability in %; data sizes in MB/GB
-	RF – shortcut for Rangefinder – mobile application
-	RPi – shortcut for Raspberry Pi
-	SeS - shortcut for SensorStation, RPi with connected sensors
-	BT - shortcut for Bluetooth


== Intended Audience and Reading Suggestions
- Product owner / client: read Sections 1–3 for scope, users, and scenarios; skim 4 and 6 for priorities.
- Developers / architects: read Sections 2, 4–8; start with 7 for architecture constraints
- Testers / QA: read Sections 3, 4, 6 for traceability and acceptance criteria.
- Operations / security: read Sections 2.4–2.7 and 6.2–6.4

== Product Scope
The Rangefinder provides a real time measurement of distance from the sensors mounted on the Raspberry Pi, that are transmitted by Bluetooth to a mobile phone app. Once the user pairs his app to the gadget, he will be able to see measurements of each sensor, the chart showing the measurements in a time period and a debug window of the Bluetooth connection. This version (1.3) focuses on the foundation of a more complex rangefinder, allowing easy further development.

== References
Raspberry Pi: #link("https://www.raspberrypi.com")

= Overall Description
== Product Perspective

#figure(
  image("context_diagram.png", width: 70%),
  caption: [
    Context diagram of the RangeFinder system
  ],
)

Rangefinder is a new product based on the Raspberry PI helping with commercial-use distance measurement. Together with the Raspberry PI chassis is an in-house built phone application that shows all measured data. The context diagram shows a simplified look at the workings of the system.

== Product Features/Functions
- Real-time measurements of each sensor
- Chart showing measurements over a set period of time
- Debug window for Bluetooth connection
- True/False obstacle checker
- Reliability evaluation of the sensor readings

== User Classes and Characteristics
Not applicable

== Operating Environment
- Raspberry Pi Zero 2W -- Raspbian Lite
- Android operating system

== Design and Implementation Constraints
- Inexperience with Bluetooth integration

== User Documentation
- PDF User Manual
- PDF Troubleshooting Guide
- Release Notes per Version

== Assumptions and Dependencies
- Stable Bluetooth connection

#pagebreak()

= System scenarios (Use cases)
== Use-case diagrams
#figure(
  image("use_case_diagram.png", width: 115%),
  caption: [
    Use-case diagram of the RangeFinder system
  ],
)

== Scenarios
=== SCN-1: User measures distance to an object

#table(
  columns: (auto, auto),
  [*FREQ reference*], [FREQ-7, FREQ-6, FREQ-5],
  [*NFREQ reference*], [NFREQ-3, NFREQ-1],
  [*Short Description:*],
  [User uses the SeS to measure a distance to an certain object],
  [*Actors:*], [User, SeS],
  [*Precondition:*], [SeS is turned on with Bluetooth enabled and SeS is paired with RF],
)

*Basic flow:* User measures distance to an object
#table(
  columns: (auto, auto, auto),
  [*Step*], [*User action*], [*System response*],
  [1], [Places an object in front of the ultrasonic / ToF sensor], [The object is detected by the sensors],
  [2], [-], [SeS mesures the distance and sends it thorugh Bluetooth interface],
  [3], [-], [RF app recieves the data from SeS],
)

*Post-condition:* SeS keeps sending new data as long as it is not turned off

*Alternative flows* \
*A1 -- Object is out of bounds*
-	Trigger: Object is either too close or too far from the sensor
-	System response: SeS sends either 0 or max value respectively
-	Result: The measurement it inaccurate
*A2 -- Bluetooth connection is lost*
-	Trigger: The Bluetooth connection is unexpectedly lost
-	System response: RF app alerts the user
-	Result: User has to reconnect his BT connection

\
=== SCN-2: User displays measured data

#table(
  columns: (auto, auto),
  [*FREQ reference*], [FREQ-2, FREQ-3, FREQ-4],
  [*NFREQ reference*], [NFREQ-3, NFREQ-4],
  [*Short Description:*],
  [User uses the Rangefinder app to read the data currently being sent by the sensors],
  [*Actors:*], [User],
  [*Precondition:*], [SeS connected and actively transmitting],
)

*Basic flow: * Users displays measured data in the app
#table(
  columns: (auto, auto, auto),
  [*Step*], [*User action*], [*System response*],
  [1], [SCN-1], [SCN-1],
  [2], [-], [RF app handles data processing and displays measurements in Simple or Advanced section of mobile app],
  [3], [Opens “Advanced” menu], [“Advanced” menu gets opened],
  [4], [Views data], [-]

)

*Post-condition:* SeS keeps sending new data as long as it is not turned off

*Alternative flows* \
*A1 -- Corrupted data*
-	Trigger: RF app receives a malformed payload
-	System response: App catches the parsing exception and ignores the bad packet
-	Result: App waits for the next packet without crashing

\
=== SCN-3: User pairs Rangefinder through Bluetooth interface 

#table(
  columns: (auto, auto),
  [*FREQ reference*], [FREQ-1],
  [*NFREQ reference*], [NFREQ-3],
  [*Short Description:*],
  [User uses the mobile app’s Bluetooth interface to pair the app to SeS],
  [*Actors:*], [User, SeS],
  [*Precondition:*], [SeS is on and advertising, phone has BT and location services enabled],
)

*Basic flow:* User pairs Rangefinder through Bluetooth interface
#table(
  columns: (auto, auto, auto),
  [*Step*], [*User action*], [*System response*],
  [1], [User selects the "Settings" section], [Section opens],
  [2], [Turns scanning on by clicking on “Scan devices”], [App starts scanning for pairable devices],
  [3], [-], [App displays a list of available devices],
  [4], [User selects the SeS from the list], [App attempts to connect to the selected device],
  [5], [-], [Connection is successful, app starts receiving data from SeS]

)

*Post-condition:* SeS and the app are successfully paired, and the app starts receiving data

*Alternative flows*\
*A1 -- Malfunctioning SeS*
-	Trigger: SeS not working properly
-	System response: SeS does not appear in the BT search
-	Result: User cannot pair the app with SeS
*A2 -- Connection timeout*
-	Trigger: RF app fails to connect
-	System response: App displays an error message
-	Result: User is prompted to try again

\
=== SCN-4: User disconnects from Sensor Station 

#table(
  columns: (auto, auto),
  [*FREQ reference*], [FREQ-8],
  [*NFREQ reference*], [-],
  [*Short Description:*],
  [User terminates the connection to the SeS to save battery and stop the data stream],
  [*Actors:*], [User, SeS],
  [*Precondition:*], [RF app is currently connected to the SeS],
)

*Basic flow:* User disconnects from Sensor Station
#table(
  columns: (auto, auto, auto),
  [*Step*], [*User action*], [*System response*],
  [1], [User selects the "Settings" section], [Section opens],
  [2], [User selects the "Disconnect" option], [App explicitly unsubscribes from the BLE characteristic and closes the BT connection],
  [3], [-], [App displays last known values in the data views, with a “Disconnected” tag],
  [4], [-], [SeS detects the disconnection and goes back to advertising for new connections],

)

*Post-condition:* SeS waits for another connection

*Alternative flows*\
*A1 -- Hard exit*
-	Trigger: User forcefully kills the app
-	System response: Mobile OS automatically severs the BLE connection
-	Result: SeS detects the severing and goes back to broadcasting safely

\
=== SCN-5: User configures units of measurement

#table(
  columns: (auto, auto),
  [*FREQ reference*], [FREQ-9, FREQ-2, FREQ-3],
  [*NFREQ reference*], [-],
  [*Short Description:*],
  [User changes the display units from meters to centimetres and inputs a new distance threshold],
  [*Actors:*], [User],
  [*Precondition:*], [RF app is currently connected to the SeS],
)

*Basic flow:* User configures units of measurement
#table(
  columns: (auto, auto, auto),
  [*Step*], [*User action*], [*System response*],
  [1], [User selects the "Settings" section], [Section opens],
  [2], [User selects the "Units" option], [App displays unit options],
  [3], [User selects the desired units for each sensor], [App updates the display units],
)

*Post-condition:* RF app is on operating on new units of measurement

\
=== SCN-6: User configures distance threshold

#table(
  columns: (auto, auto),
  [*FREQ reference*], [FREQ-9, FREQ-2, FREQ-3],
  [*NFREQ reference*], [-],
  [*Short Description:*],
  [User sets a custom distance threshold for UI warnings.],
  [*Actors:*], [User],
  [*Precondition:*], [RF app is currently connected to the SeS],
)

*Basic flow:* User configures distance threshold
#table(
  columns: (auto, auto, auto),
  [*Step*], [*User action*], [*System response*],
  [1], [User selects the "Settings" section], [Section opens],
  [2], [User selects the "Threshold" option], [App displays threshold options],
  [3], [User inputs a new distance threshold for each sensor], [App updates the threshold],
)

*Post-condition:* RF app is sending UI warnings based on the new distance threshold


#pagebreak()
*Alterative flows*
*A1 -- Invalid threshold input*
-	Trigger: User inputs a negative number, text or a value out of sensors physical bounds.
-	System response: App informs the user of the invalid value and reverts to last known valid setting
-	Result: The threshold is ready to be changed again

\

= Functional Requirements
== FREQ-1 -- Connecting to the SeS
*Description:* The phone app should allow the user to connect to the SeS using Bluetooth \
*Priority:* High \
*Acceptance criteria:* Given the Bluetooth is working correctly on both devices, the user is able to connect to the SeS and starts to receive data.

== FREQ-2 -- Displaying data
*Description:* The phone app should allow the user to see data from each of the sensors, including the True/False obstacle detector. \
*Priority:* High \
*Acceptance criteria:* Given the user is connected to the SeS, they can clearly see data from each sensor in the "Advanced" menu of the app and min/max value in the "Simple" menu of the app.

== FREQ-3 -- Displaying the chart
*Description:* The phone app should allow the user to see data from all the sensors on the chart. \
*Priority:* Medium \
*Acceptance criteria:* Given the user is connected to the SeS, they can see the chart in the "Advanced" menu of the app and they can distinguish each sensor by their assigned colour.

== FREQ-4 -- Displaying the Debug window
*Description:* The phone app should allow the user to see the Debug window. \
*Priority:* Medium \
*Acceptance criteria:* Given the user is connected to the SeS, they can see the debug window on the bottom of the "Advanced" menu, where the entire Bluetooth connection between the devices is continuously displayed.

== FREQ-5 -- Sending data
*Description:* The SeS should begin data transmission once the BT connection is properly established. \
*Priority:* High \
*Acceptance criteria:* The data is being properly transmitted through the Bluetooth connection.

== FREQ-6 -- Receiving data
*Description:* The RF app should correctly receive the data, that are being sent through the Bluetooth connection. \
*Priority:* High \
*Acceptance criteria:* The data from the SeS are received and usable.

== FREQ-7 -- Detecting and measuring distance from objects \
*Description: *The sensors should correctly measure distance from objects and send their data throughout the SeS to be transmitted through Bluetooth. \
*Priority:* High \
*Acceptance criteria:* The measured data from the sensors are being passed to the SeS. \

== FREQ-8 -- Disconnecting from SeS \
*Description:* The app should allow the user to disconnect from the SeS \
*Priority:* Medium \
*Acceptance criteria:* The app is successfully disconnected from the SeS \

== FREQ-9 -- Configuring units of measurement
*Description:* The app should allow the user to configure measurement units between mm, cm, dm and m. \
*Priority:* Low
*Acceptance criteria:* The data are correctly recalculated and displayed. \

== FREQ-10 -- Configuring distance threshold \
*Description:* The app shall allow the user to configure distance threshold in currently chosen units of measurement. \
*Priority:* Low \
*Acceptance criteria:* If the object is closer or equals the threshold, a warning is displayed in the app. \

\
\
= External Interface Requirements
== User Interfaces (UI design / User Interaction Flow)
- UI should include: Advanced menu, Simple menu, Settings menu, and appropriate navigation between them.
- Advanced menu should include:
  - A chart displaying data of each sensor in a set time period
  - Current measurement of each sensor in a separate box
  - Debug window of Bluetooth connection
  - Scrollbar to reach the debug window
- Simple menu should include:
  - Minimum distance currently measured by a sensor
  - Maximum distance currently measured by a sensor
  - Current data from the obstacle sensor

== Hardware Interfaces
-	I2C, GPIO pin readings
-	Raspberry Zero 2W
-	WaveShare Laser sensor
-	Ultrasound sensor HC-SR04
-	Time-of-flight sensor VL53LDK


== Software Interfaces (API Interfaces)
-	Go Bluetooth -- BLE
-	Bluetooth LE plugin for Xamarin & MAUI

== Communications Interfaces
- Bluetooth with AES encryption

= Other Nonfunctional Requirements
== Performance Requirements
Not applicable

== Safety Requirements
Not applicable

== Security Requirements
The data transfer between SeS and RF app should be encrypted. For this purpose AES encryption will be used. The data on SeS will be encrypted by Secret Key, transmitted safely and then decrypted by the RF app. \

== Software Quality Attributes
+ *NFREQ-1 (Correctness):* The system shall measure within a meter accuracy.
+ *NFREQ-2 (Maintainability):* Codebase shall include automated tests covering at least 60% of backend logic and 40% of frontend components.
+ *NFREQ-3 (Portability):* The application shall work with any modern android phone.
+ *NFREQ-4 (Usability):* The application shall display all measured distances within at maximum half a second.

= System Design & Architecture
== High-Level Architecture
#figure(
  image("arch_diagram.png", width: 100%),
  caption: [
    Component diagram of the RangeFinder system.
  ],
)


== Technology Stack
-	Phone App: .NET MAUI
-	Hardware:
 -	Main unit: Raspberry Pi Zero 2W
  -	ARM64 architecture, 512 MB RAM, integrated BT chip
 -	Operating system: Linux (Raspberry Pi OS, 64-bit)
 -	Programming language: Go (Golang)
 -	Architecture of runtime: Goroutines + Mutex – safe and effective asynchronous parallel processing
 -	Key libraries:
  -	go-rpio/v4 : low-level GPIO control
  -	go-i2c & go-vl5310x : communication with ToF sensor through HW registry
  -	go-logger : for log control (I2C console spamming)
 -	Communication and safety:
  -	Bluetooth Low Energy (BLE) – GATT server using system BlueZ
  -	E2E encryption: AES-256 in GCM mode
   -	PSK Pre-Shared-Key key management through environment variables
 -	Data structure: JSON

== Data Flow
Data is collected from a single sensor $->$ Data is transferred by Bluetooth to the mobile device application $->$ Data is parsed and displayed in the application.

= Other Requirements
None.
