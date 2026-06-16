# 🔴 UndercoverCIAgent — EXILED Plugin

> *"Blend in. Survive. Escape. Then wipe them out."*

An EXILED plugin for **SCP: Secret Laboratory** that secretly converts one lucky D-Class player per round into a **Chaos Insurgency Sleeper Agent** — complete with smuggled starting gear, a secret identity badge, and a devastating endgame reward when they successfully escape.

---

## ✨ Features

| Feature | Details |
|---|---|
| **Sleeper Agent Spawn** | One random D-Class is secretly selected at round start |
| **Configurable Chance** | Default 60 % — server admins can set 0–100 % |
| **Unmissable Broadcast** | Giant red centre-screen message announcing the secret role |
| **Secret Identity Badge** | `[CI-AGENT]` nametag + crimson rank badge |
| **Smuggled Starting Gear** | FSP-9, Chaos Keycard, Painkillers & 40 rounds of 9mm |
| **Ultimate Escape Reward** | FR-MG-0, O5 Keycard, 2× Coin, Adrenaline, 2× HE Grenade |
| **Server Announcement** | Optional server-wide broadcast when the agent escapes |
| **Full Config** | Every message, duration, chance and item count is editable |

---

## 📦 Installation

1. **Download ** the plugin DLL,
2. Drop `UndercoverCIAgent.dll` into your server's **EXILED Plugins** folder:
   - **Path**: `\EXILED\Plugins\`
3. **Restart** your server or run `exiled reload` in the console.
4. The config file is auto-generated at `EXILED/Configs/<port>-config.yml`.

---



## ⚙️ Configuration

The auto-generated YAML section looks like this (edit in your server's config file):

```yaml
undercover_ci:
  is_enabled: true
  debug: false

  # Chance (0–100 %) that an agent spawns this round
  spawn_chance: 60.0

  # Need at least this many D-Class alive before selecting an agent
  min_d_class_to_select: 1

  # ── Broadcast / UI ────────────────────────────────────────────────
  spawn_broadcast_duration: 12
  spawn_broadcast_message: |
    <size=40><color=#c00000><b>⚠ CHAOS INSURGENCY ⚠</b></color></size>
    <size=28><color=#ff4444>You are an <b>Undercover CI Agent</b>.</color></size>
    <size=22><color=#ffaa00>Blend in. Survive. <b>Escape</b> for your reward.</color></size>

  spawn_hint_message: "🔴  <b>MISSION:</b> Reach the surface alive and escape as a D-Class.\nYour true rewards await beyond the gates."
  spawn_hint_duration: 8.0

  agent_custom_info: "[CI-AGENT]"
  agent_badge_text: "CI Sleeper Agent"
  agent_badge_color: "crimson"

  # ── Gear ──────────────────────────────────────────────────────────
  starting_ammo: 40
  escape_ammo: 120

  # ── Escape reward ─────────────────────────────────────────────────
  escape_broadcast_duration: 10
  escape_broadcast_message: |
    <size=38><color=#c00000><b>🏆 MISSION COMPLETE</b></color></size>
    <size=24><color=#ffaa00>Your Chaos Insurgency war-kit has been delivered.
    <b>Lead the charge. Wipe them out.</b></color></size>

  # Set to empty string "" to disable the server-wide announcement
  server_escape_announcement: "<size=30><color=#ff2222><b>⚠  A Chaos Insurgency Agent has infiltrated and ESCAPED!  ⚠</b></color></size>"
  server_escape_announcement_duration: 8
```

---

## 🎒 Gear Tables

### Smuggled Starting Gear (Spawn)

| Item | Purpose |
|---|---|
| `GunFSP9` | Security Guard SMG — blend in, fight early |
| `KeycardChaosInsurgency` | Bypass standard CI checkpoints |
| `Painkillers` | Early-game HP recovery |
| 40× 9mm Ammo | Loaded for the FSP-9 |

### Ultimate Escape Reward

| Item | Purpose |
|---|---|
| `GunFRMG0` | MTF Captain's LMG — the deadliest primary in the game |
| `KeycardO5` | Tier-7 access — opens everything |
| `Coin × 2` | SCP-914 / vending machine utility |
| `Adrenaline` | Burst heal + speed boost |
| `GrenadeHE × 2` | Area denial / room clearing |
| 120× 5.56mm Ammo | Full magazine for the FR-MG-0 |
| 60× 7.62mm Ammo | Reserve supply |

---

## 🗂️ Project Structure

```
UndercoverCIAgent/
├── UndercoverCIAgent.csproj   ← .NET project file
├── Plugin.cs                  ← Entry-point, lifecycle, event registration
├── Config.cs                  ← IConfig with all configurable values
├── EventHandlers.cs           ← All game logic (spawn, badge, gear, escape)
└── README.md                  ← This file
```

---

## 📝 Compatibility

| | |
|---|---|
| **EXILED version** | 8.x (requires `RequiredExiledVersion = 8.0.0`) |
| **SCP:SL version** | 14.x (tested against Parabellum era) |
| **Target framework** | .NET Framework 4.8 |

---

## 🤝 Contributing

Pull requests and issue reports are welcome! If you add new features (e.g., custom voice lines, SCPUtils integration, custom items via EXILED's CustomItems API), please keep the code clean and well-commented.

---

## 📜 License

Released under the **MIT License**. See `LICENSE` for details.

created by POMPON[JMZ]
