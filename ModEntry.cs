using StardewModdingAPI;
using StardewValley.Locations;
using StardewValley;
using HarmonyLib;
using xTile.Dimensions;
using Microsoft.Xna.Framework;
using Rectangle = xTile.Dimensions.Rectangle;
using System.Reflection.Emit;
using CustomLocksUpdated.Patches;

namespace CustomLocksUpdated {
    public class ModEntry : Mod {

        public static ModConfig Config = new();
        public static IMonitor? monitor;

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();
            monitor = Monitor;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Mountain), nameof(Mountain.checkAction), [typeof(Location), typeof(Rectangle), typeof(Farmer)]),
                transpiler: new HarmonyMethod(typeof(MountainPatch), nameof(MountainPatch.MountainCheckAction_Transpiler))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performTouchAction), [typeof(string[]), typeof(Vector2)]),
               transpiler: new HarmonyMethod(typeof(GameLocationPatch), nameof(GameLocationPatch.GameLocationPerformTouchAction_Transpiler))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), [typeof(string[]), typeof(Farmer), typeof(Location)]),
               transpiler: new HarmonyMethod(typeof(GameLocationPatch), nameof(GameLocationPatch.GameLocationPerformAction_Transpiler))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.lockedDoorWarp)),
               transpiler: new HarmonyMethod(typeof(GameLocationPatch), nameof(GameLocationPatch.GameLocationLockedDoorWarp_Transpiler))
            );
            if (helper.ModRegistry.IsLoaded("DTZ.DowntownZuzuDLL")) {
                harmony.Patch(
                    original: AccessTools.Method(AccessTools.TypeByName("Downtown_Zuzu.HarmonyPatches.FestivalPatch"), "Prefix"),
                    prefix: new HarmonyMethod(typeof(DowntownZuzuPatch), nameof(DowntownZuzuPatch.Prefix_Prefix))
                );
            }
        }

        public static bool GetAllowRoomEntry() {
            bool allow = Config.Enabled ? Config.AllowStrangerRoomEntry : false;
            return allow;
        }

        public static bool GetAllowHomeEntry() {
            bool allow = Config.Enabled ? Config.AllowStrangerHomeEntry : false;
            return allow;
        }

        public static bool GetAllowEarlyGuild() {
            bool allow = Config.Enabled ? Config.AllowAdventureGuildEntry : false;
            return allow;
        }

        public static bool GetAllowOutsideTime() {
            bool allow = Config.Enabled ? Config.AllowOutsideTime : false;
            return allow;
        }

        public static bool GetAllowSeedShopWed() {
            bool allow = Config.Enabled ? Config.AllowSeedShopWed : false;
            return allow;
        }

        public static bool GetIgnoreEvents() {
            bool allow = Config.Enabled ? Config.IgnoreEvents : false;
            return allow;
        }

        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("overall-enabled.label"),
                tooltip: () => Helper.Translation.Get("overall-enabled.tooltip"),
                getValue: () => Config.Enabled,
                setValue: value => Config.Enabled = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("general-store-wed.label"),
                tooltip: () => Helper.Translation.Get("general-store-wed.tooltip"),
                getValue: () => Config.AllowSeedShopWed,
                setValue: value => Config.AllowSeedShopWed = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("outside-hours.label"),
                tooltip: () => Helper.Translation.Get("outside-hours.tooltip"),
                getValue: () => Config.AllowOutsideTime,
                setValue: value => Config.AllowOutsideTime = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("stranger-home-entry.label"),
                tooltip: () => Helper.Translation.Get("stranger-home-entry.tooltip"),
                getValue: () => Config.AllowStrangerHomeEntry,
                setValue: value => Config.AllowStrangerHomeEntry = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("stranger-room-entry.label"),
                tooltip: () => Helper.Translation.Get("stranger-room-entry.tooltip"),
                getValue: () => Config.AllowStrangerRoomEntry,
                setValue: value => Config.AllowStrangerRoomEntry = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("adventure-guild.label"),
                tooltip: () => Helper.Translation.Get("adventure-guild.tooltip"),
                getValue: () => Config.AllowAdventureGuildEntry,
                setValue: value => Config.AllowAdventureGuildEntry = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("ignore-events.label"),
                tooltip: () => Helper.Translation.Get("ignore-events.tooltip"),
                getValue: () => Config.IgnoreEvents,
                setValue: value => Config.IgnoreEvents = value
            );
        }

    }
}
