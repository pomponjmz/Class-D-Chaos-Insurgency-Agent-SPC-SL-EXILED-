using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace UndercoverCIAgent
{
    /// <summary>
    /// Contains all event-handler logic for the Undercover CI Agent plugin.
    /// Registered/unregistered by <see cref="Plugin"/> on enable/disable.
    /// </summary>
    public sealed class EventHandlers
    {
        // ─── State (internal so commands can read it) ─────────────────────────

        /// <summary>The player currently designated as the Undercover Agent this round.</summary>
        internal Player? CurrentAgent { get; private set; }

        /// <summary>Whether the agent has successfully escaped this round.</summary>
        internal bool AgentEscaped { get; private set; }

        /// <summary>Cancellation source for all in-flight delayed tasks.</summary>
        private CancellationTokenSource _cts = new();

        // ─── Reference back to the plugin ─────────────────────────────────────
        private readonly Plugin _plugin;

        internal EventHandlers(Plugin plugin)
        {
            _plugin = plugin;
        }

        // ─── Server event ─────────────────────────────────────────────────────

        /// <summary>Called when the round starts. Selects (or skips) an agent.</summary>
        internal void OnRoundStarted()
        {
            // Reset round-scoped state and cancel lingering tasks
            CurrentAgent  = null;
            AgentEscaped  = false;
            _cts.Cancel();
            _cts = new CancellationTokenSource();

            var cfg = _plugin.Config;

            if (UnityEngine.Random.value * 100f > cfg.SpawnChance)
            {
                Log.Debug("[UndercoverCI] Chance roll failed – no agent this round.");
                return;
            }

            _ = DelayedSelectAgent(1500, _cts.Token);
        }

        // ─── Player events ────────────────────────────────────────────────────

        /// <summary>Intercepts the agent escaping, then hands out the reward loadout.</summary>
        internal void OnEscaping(EscapingEventArgs ev)
        {
            if (CurrentAgent == null || AgentEscaped)
                return;

            if (ev.Player != CurrentAgent)
                return;

            if (ev.Player.Role.Type != RoleTypeId.ClassD)
                return;

            AgentEscaped = true;

            Log.Debug($"[UndercoverCI] Agent {ev.Player.Nickname} is escaping – queuing reward.");

            _ = DelayedGrantReward(ev.Player, 400, _cts.Token);
        }

        /// <summary>Cleans up if the agent is killed.</summary>
        internal void OnDied(DiedEventArgs ev)
        {
            if (ev.Player == CurrentAgent && !AgentEscaped)
            {
                Log.Debug($"[UndercoverCI] Agent {ev.Player?.Nickname} died. Clearing slot.");
                CurrentAgent = null;
            }
        }

        /// <summary>Cleans up if the agent disconnects.</summary>
        internal void OnLeft(LeftEventArgs ev)
        {
            if (ev.Player == CurrentAgent)
            {
                Log.Debug($"[UndercoverCI] Agent {ev.Player.Nickname} left. Clearing slot.");
                CurrentAgent = null;
            }
        }

        // ─── Command API (called by CIAgentCommand sub-commands) ──────────────

        /// <summary>
        /// Manually assigns <paramref name="player"/> as the Undercover Agent.
        /// Strips any existing agent first, then applies identity + starting gear.
        /// </summary>
        internal void ForceAssignAgent(Player player)
        {
            // Strip old agent's cosmetics if one exists
            if (CurrentAgent != null && CurrentAgent != player)
                ClearAgentIdentity(CurrentAgent);

            CurrentAgent = player;
            AgentEscaped = false;

            ApplyAgentIdentity(player);
            GiveStartingGear(player);
            BroadcastSpawnMessage(player);

            Log.Info($"[UndercoverCI] Agent manually assigned: {player.Nickname}");
        }

        /// <summary>
        /// Clears the current agent slot and strips their cosmetics.
        /// </summary>
        internal void ResetAgent()
        {
            if (CurrentAgent == null) return;

            Log.Info($"[UndercoverCI] Agent slot reset (was {CurrentAgent.Nickname}).");
            ClearAgentIdentity(CurrentAgent);
            CurrentAgent = null;
            AgentEscaped = false;
        }

        // ─── Internal helpers ─────────────────────────────────────────────────

        private async Task DelayedSelectAgent(int delayMs, CancellationToken ct)
        {
            try
            {
                await Task.Delay(delayMs, ct).ConfigureAwait(false);
                SelectAgent();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { Log.Error($"[UndercoverCI] DelayedSelectAgent: {ex}"); }
        }

        private async Task DelayedGrantReward(Player agent, int delayMs, CancellationToken ct)
        {
            try
            {
                await Task.Delay(delayMs, ct).ConfigureAwait(false);
                GrantEscapeReward(agent);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { Log.Error($"[UndercoverCI] DelayedGrantReward: {ex}"); }
        }

        private async Task DelayedHint(Player agent, string message, float duration, int delayMs, CancellationToken ct)
        {
            try
            {
                await Task.Delay(delayMs, ct).ConfigureAwait(false);
                if (agent.IsAlive)
                    agent.ShowHint(message, duration);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { Log.Error($"[UndercoverCI] DelayedHint: {ex}"); }
        }

        // ─────────────────────────────────────────────────────────────────────

        private void SelectAgent()
        {
            var cfg = _plugin.Config;

            var candidates = Player.List
                .Where(p => p.Role.Type == RoleTypeId.ClassD && p.IsAlive)
                .ToList();

            if (candidates.Count < cfg.MinDClassToSelect)
            {
                Log.Debug("[UndercoverCI] Not enough D-Class players – skipping.");
                return;
            }

            CurrentAgent = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            Log.Info($"[UndercoverCI] Sleeper agent selected: {CurrentAgent.Nickname}");

            ApplyAgentIdentity(CurrentAgent);
            GiveStartingGear(CurrentAgent);
            BroadcastSpawnMessage(CurrentAgent);
        }

        private void ApplyAgentIdentity(Player agent)
        {
            var cfg = _plugin.Config;
            agent.CustomInfo  = cfg.AgentCustomInfo;
            agent.BadgeHidden = false;
            agent.RankName    = cfg.AgentBadgeText;
            agent.RankColor   = cfg.AgentBadgeColor;
        }

        private void GiveStartingGear(Player agent)
        {
            var cfg = _plugin.Config;

            agent.ClearItems();
            agent.AddItem(ItemType.GunFSP9);
            agent.AddItem(ItemType.KeycardChaosInsurgency);
            agent.AddItem(ItemType.Painkillers);
            agent.AddAmmo(AmmoType.Nato9, cfg.StartingAmmo);

            Log.Debug($"[UndercoverCI] Starting gear given to {agent.Nickname}.");
        }

        private void BroadcastSpawnMessage(Player agent)
        {
            var cfg = _plugin.Config;

            agent.Broadcast(
                cfg.SpawnBroadcastDuration,
                cfg.SpawnBroadcastMessage,
                global::Broadcast.BroadcastFlags.Normal,
                shouldClearPrevious: true);

            int hintDelay = Math.Max(0, (cfg.SpawnBroadcastDuration - 1) * 1000);
            _ = DelayedHint(agent, cfg.SpawnHintMessage, cfg.SpawnHintDuration, hintDelay, _cts.Token);
        }

        private void GrantEscapeReward(Player agent)
        {
            if (!agent.IsAlive)
            {
                Log.Debug("[UndercoverCI] Agent not alive after escape – skipping reward.");
                return;
            }

            var cfg = _plugin.Config;

            agent.ClearItems();
            agent.AddItem(ItemType.GunFRMG0);
            agent.AddItem(ItemType.KeycardO5);
            agent.AddItem(ItemType.Coin);
            agent.AddItem(ItemType.Coin);
            agent.AddItem(ItemType.Adrenaline);
            agent.AddItem(ItemType.GrenadeHE);
            agent.AddItem(ItemType.GrenadeHE);
            agent.AddAmmo(AmmoType.Nato556, cfg.EscapeAmmo);
            agent.AddAmmo(AmmoType.Nato762, (ushort)(cfg.EscapeAmmo / 2));

            Log.Info($"[UndercoverCI] Escape reward granted to {agent.Nickname}.");

            agent.Broadcast(
                cfg.EscapeBroadcastDuration,
                cfg.EscapeBroadcastMessage,
                global::Broadcast.BroadcastFlags.Normal,
                shouldClearPrevious: true);

            if (!string.IsNullOrWhiteSpace(cfg.ServerEscapeAnnouncement))
                Map.Broadcast(cfg.ServerEscapeAnnouncementDuration, cfg.ServerEscapeAnnouncement);

            ClearAgentIdentity(agent);
        }

        private static void ClearAgentIdentity(Player agent)
        {
            agent.CustomInfo = string.Empty;
            agent.RankName   = string.Empty;
            agent.RankColor  = string.Empty;
        }

        // ─── Cleanup ──────────────────────────────────────────────────────────

        /// <summary>Cancels all in-flight delayed tasks. Called by <see cref="Plugin.OnDisabled"/>.</summary>
        internal void KillDelays()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }
    }
}
