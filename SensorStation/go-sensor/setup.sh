#!/usr/bin/env bash
set -euo pipefail

# Usage:
#   sudo bash setup.sh /path/to/sensorstation-binary
#   ENCRYPTION_SECRET="<32-char-secret>" sudo --preserve-env=ENCRYPTION_SECRET bash setup.sh /path/to/sensorstation-binary

BIN_SOURCE="${1:-/home/pi/sensorstation-arm64-rpi}"
INSTALL_DIR="/opt/sensorstation"
BIN_TARGET="${INSTALL_DIR}/sensorstation"
START_SCRIPT="/usr/local/bin/sensorstation-start.sh"
SERVICE_FILE="/etc/systemd/system/sensorstation.service"
ENV_DIR="/etc/sensorstation"
ENV_FILE="${ENV_DIR}/sensorstation.env"

if [[ "${EUID}" -ne 0 ]]; then
  echo "[ERROR] Run as root (example: sudo bash setup.sh /home/pi/sensorstation-arm64-rpi)"
  exit 1
fi

if [[ ! -f "${BIN_SOURCE}" ]]; then
  echo "[ERROR] Binary not found: ${BIN_SOURCE}"
  exit 1
fi

if [[ -z "${ENCRYPTION_SECRET:-}" ]] && [[ ! -f "${ENV_FILE}" ]]; then
  echo "[ERROR] ENCRYPTION_SECRET is not set and ${ENV_FILE} does not exist."
  echo "        Provide ENCRYPTION_SECRET (must be exactly 32 chars) when running setup."
  exit 1
fi

if [[ -n "${ENCRYPTION_SECRET:-}" ]] && [[ "${#ENCRYPTION_SECRET}" -ne 32 ]]; then
  echo "[ERROR] ENCRYPTION_SECRET must be exactly 32 characters (current: ${#ENCRYPTION_SECRET})."
  exit 1
fi

echo "[INFO] Installing binary..."
install -d "${INSTALL_DIR}"
install -m 0755 "${BIN_SOURCE}" "${BIN_TARGET}"

echo "[INFO] Ensuring environment file..."
install -d -m 0755 "${ENV_DIR}"
if [[ -n "${ENCRYPTION_SECRET:-}" ]]; then
  cat > "${ENV_FILE}" <<EOF
ENCRYPTION_SECRET=${ENCRYPTION_SECRET}
EOF
fi
if [[ -f "${ENV_FILE}" ]]; then
  chown root:root "${ENV_FILE}"
  chmod 0600 "${ENV_FILE}"
fi

echo "[INFO] Writing startup script..."
cat > "${START_SCRIPT}" <<'EOF'
#!/usr/bin/env bash
set -euo pipefail

cd /opt/sensorstation
exec /opt/sensorstation/sensorstation
EOF
chmod 0755 "${START_SCRIPT}"

echo "[INFO] Writing systemd service..."
cat > "${SERVICE_FILE}" <<'EOF'
[Unit]
Description=Sensor Station Service
After=network-online.target
Wants=network-online.target

[Service]
Type=simple
User=root
WorkingDirectory=/opt/sensorstation
EnvironmentFile=/etc/sensorstation/sensorstation.env
ExecStart=/usr/local/bin/sensorstation-start.sh
Restart=always
RestartSec=2
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
EOF

echo "[INFO] Enabling and starting service..."
systemctl daemon-reload
systemctl enable sensorstation.service
systemctl restart sensorstation.service

echo "[INFO] Done. Service status:"
systemctl --no-pager --full status sensorstation.service || true

echo "[INFO] Follow logs with: journalctl -u sensorstation.service -f"
echo "[INFO] ENCRYPTION_SECRET is loaded from /etc/sensorstation/sensorstation.env"
