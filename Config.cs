using System.ComponentModel;
using Exiled.API.Interfaces;

namespace UndercoverCIAgent
{
    /// <summary>
    /// Plugin configuration file.
    /// Serialised automatically to EXILED/Configs/UndercoverCIAgent.yml.
    /// </summary>
    public sealed class Config : IConfig
    {
        // ─── Required IConfig properties ──────────────────────────────────────
        /// <inheritdoc />
        [Description("Set to false to completely disable the plugin without unloading it.")]
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        [Description("Enables verbose debug logging in the server console.")]
        public bool Debug { get; set; } = false;

        // ─── Spawn settings ───────────────────────────────────────────────────
        [Description(
            "Percentage chance (0–100) that exactly one D-Class per round becomes an Undercover CI Agent. " +
            "Set to 100 to guarantee it every round.")]
        public float SpawnChance { get; set; } = 60f;

        [Description(
            "Minimum number of alive D-Class players required before the agent can be selected. " +
            "Prevents the agent from being the only D-Class on tiny servers.")]
        public int MinDClassToSelect { get; set; } = 1;

        // ─── Broadcast / UI ───────────────────────────────────────────────────
        [Description("Duration (seconds) of the centre-screen spawn broadcast shown to the agent.")]
        public ushort SpawnBroadcastDuration { get; set; } = 12;

        [Description("Message sent to the agent via centre-screen broadcast when they spawn.")]
        public string SpawnBroadcastMessage { get; set; } =
            "<size=40><color=#c00000><b>⚠ CHAOS INSURGENCY ⚠</b></color></size>\n" +
            "<size=28><color=#ff4444>You are an <b>Undercover CI Agent</b>.</color></size>\n" +
            "<size=22><color=#ffaa00>Blend in. Survive. <b>Escape</b> for your reward.</color></size>";

        [Description("Hint shown to the agent a few seconds after spawn as a reminder.")]
        public string SpawnHintMessage { get; set; } =
            "🔴  <b>MISSION:</b> Reach the surface alive and escape as a D-Class.\n" +
            "Your true rewards await beyond the gates.";

        [Description("Duration (seconds) of the reminder hint shown after spawn.")]
        public float SpawnHintDuration { get; set; } = 8f;

        [Description("Custom nametag text visible in the scoreboard / player list.")]
        public string AgentCustomInfo { get; set; } = "[CI-AGENT]";

        [Description("Badge text shown on the agent's in-game nametag.")]
        public string AgentBadgeText { get; set; } = "CI Sleeper Agent";

        [Description("Badge colour for the agent's nametag. Must be a valid EXILED badge colour name.")]
        public string AgentBadgeColor { get; set; } = "crimson";

        // ─── Starting gear ────────────────────────────────────────────────────
        [Description("Ammo amount given together with the starting FSP-9.")]
        public ushort StartingAmmo { get; set; } = 40;

        // ─── Escape reward broadcast ──────────────────────────────────────────
        [Description("Duration (seconds) of the escape reward broadcast shown to the agent.")]
        public ushort EscapeBroadcastDuration { get; set; } = 10;

        [Description("Message shown to the agent when they successfully escape.")]
        public string EscapeBroadcastMessage { get; set; } =
            "<size=38><color=#c00000><b>🏆 MISSION COMPLETE</b></color></size>\n" +
            "<size=24><color=#ffaa00>Your Chaos Insurgency war-kit has been delivered.\n" +
            "<b>Lead the charge. Wipe them out.</b></color></size>";

        [Description("Ammo amount for the primary weapon added on escape.")]
        public ushort EscapeAmmo { get; set; } = 120;

        // ─── Server-wide announcements ────────────────────────────────────────
        [Description(
            "Broadcast the entire server receives when the agent successfully escapes. " +
            "Leave empty to disable.")]
        public string ServerEscapeAnnouncement { get; set; } =
            "<size=30><color=#ff2222><b>⚠  A Chaos Insurgency Agent has infiltrated and ESCAPED!  ⚠</b></color></size>";

        [Description("Duration (seconds) of the server-wide escape announcement.")]
        public ushort ServerEscapeAnnouncementDuration { get; set; } = 8;
    }
}
