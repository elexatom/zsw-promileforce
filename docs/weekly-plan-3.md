# Weekly Plan 3

## Overview

This week's goal is to lay the groundwork for the PromileForce project by defining the team roles, gathering all necessary materials and components, establishing the requirements specification, creating initial design models, and agreeing on the overall development workflow.

## Division of Work (Roles)

| Role | Responsibility |
|------|---------------|
| Hardware Lead | Set up Raspberry Pi, connect and test ultrasonic and laser sensors |
| Mobile App Lead | Scaffold the mobile application, define Bluetooth communication interface |
| Requirements / Documentation | Write requirements specification, maintain project documentation |
| Integration / DevOps | Define project structure, set up version control conventions, coordinate between HW and SW |

## Getting All Materials

- Raspberry Pi board with Raspberry Pi OS Lite and power supply
- Ultrasonic sensor (×1) and laser distance sensors (×2) — three sensors in total, satisfying the minimum requirement of two operating simultaneously
- Connecting wires, breadboard, and any needed resistors
- Bluetooth adapter (if not built in to the RPi)
- Mobile device (Android/iOS) for app testing
- Development laptops with required IDEs and SDKs installed

## Requirements Specification

- The device must read distance data from **at least two sensors simultaneously**.
- Measured values must be transmitted to the mobile app **in real time via Bluetooth**.
- The mobile app must display:
  - The measured distance value
  - The identifier of the sensor providing the value
- The system must support the ultrasonic sensor and both laser sensors.
- Communication protocol between RPi and mobile app to be defined (e.g., BLE GATT or RFCOMM serial).

## Models and Design

- **System block diagram**: RPi ↔ Sensors, RPi ↔ Bluetooth ↔ Mobile App
- **Data flow model**: Sensor reading → RPi processing → Bluetooth packet → App display
- **Mobile app UI wireframe**: Single screen showing sensor ID and distance value with live updates

## Workflow

1. Each team member sets up their local development environment this week.
2. Hardware Lead tests individual sensors and confirms readings on the RPi console.
3. Mobile App Lead creates a skeleton app with a placeholder Bluetooth connection.
4. Requirements / Documentation member drafts the full requirements document and shares it for team review.
5. Team meets mid-week to review the requirements draft, agree on the communication protocol, and assign next-week tasks.
6. All code and documents are pushed to the repository by end of week.
