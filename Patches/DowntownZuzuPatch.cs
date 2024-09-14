namespace CustomLocksUpdated.Patches;

static class DowntownZuzuPatch {

    public static bool Prefix_Prefix(ref bool __result) {
        __result = true;
        return false;
    }

}

