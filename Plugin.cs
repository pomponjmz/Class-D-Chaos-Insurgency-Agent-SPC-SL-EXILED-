using System;
using Exiled.API.Features;
using PlayerHandler = Exiled.Events.Handlers.Player;
using ServerHandler = Exiled.Events.Handlers.Server;

namespace UndercoverCIAgent
{
    /// <summary>
    /// Entry-point for the Undercover CI Agent EXILED plugin.
    /// </summary>
    public sealed class Plugin : Plugin<Config>
    {
        // ─── Singleton ────────────────────────────────────────────────────────

        /// <summary>Singleton instance accessible from command handlers.</summary>
        public static Plugin Instance { get; private set; } = null!;

        // ─── Plugin metadata ──────────────────────────────────────────────────

        public override string Name    => "UndercoverCIAgent";
        public override string Author  => "YourNameHere";
        public override string Prefix  => "undercover_ci";
        public override Version Version => new(1, 1, 0);
        public override Version RequiredExiledVersion => new(8, 0, 0);

        // ─── Event handler instance ────────────────────────────────────────────

        /// <summary>
        /// Public so Remote Admin commands can call ForceAssignAgent / ResetAgent.
        /// </summary>
        public EventHandlers? Handlers { get; private set; }

        // ─── Lifecycle ────────────────────────────────────────────────────────

        public override void OnEnabled()
        {
            Instance = this;
            Handlers = new EventHandlers(this);

            // Server events
            ServerHandler.RoundStarted += Handlers.OnRoundStarted;

            // Player events
            PlayerHandler.Escaping += Handlers.OnEscaping;
            PlayerHandler.Died     += Handlers.OnDied;
            PlayerHandler.Left     += Handlers.OnLeft;

            Log.Info($"[{Name}] v{Version} enabled. Spawn chance: {Config.SpawnChance}%");
            Log.Info($"[{Name}] Commands: ci_agent assign | ci_agent list | ci_agent reset");

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ServerHandler.RoundStarted -= Handlers!.OnRoundStarted;

            PlayerHandler.Escaping -= Handlers.OnEscaping;
            PlayerHandler.Died     -= Handlers.OnDied;
            PlayerHandler.Left     -= Handlers.OnLeft;

            Handlers.KillDelays();
            Handlers = null;

            Instance = null!;

            Log.Info($"[{Name}] disabled.");

            base.OnDisabled();
        }
    }
}
