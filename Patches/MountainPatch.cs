using HarmonyLib;
using System.Reflection.Emit;

namespace CustomLocksUpdated.Patches;

static class MountainPatch {

    public static IEnumerable<CodeInstruction> MountainCheckAction_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
        var method = AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetAllowEarlyGuild));

        var matcher = new CodeMatcher(instructions, generator);

        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Ldc_I4, 1136),
            new CodeMatch(OpCodes.Bne_Un_S)
        ).ThrowIfNotMatch("Couldn't find match for guild tile index");

        var switchBreakLabel = (Label)matcher.Instruction.operand;

        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldsfld),
            new CodeMatch(OpCodes.Ldstr, "Strings\\Locations:Mountain_AdventurersGuildNote"),
            new CodeMatch(i => i.opcode == OpCodes.Callvirt),
            new CodeMatch(OpCodes.Ldc_I4_S)
        ).ThrowIfNotMatch("Couldn't find match for guild note");

        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, method),
            new CodeInstruction(OpCodes.Brtrue, switchBreakLabel)
        );

        return matcher.InstructionEnumeration();
    }

}