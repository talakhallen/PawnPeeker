using HarmonyLib;
using Verse;

namespace PawnPeeker
{
    [StaticConstructorOnStartup]
    public class Mod : Verse.Mod
    {
        public Mod(ModContentPack content) : base(content)
        {
#if DEBUG
            Harmony.DEBUG = true;
#endif

            Harmony harmony = new Harmony("com.talakhallen.pawnpeeker");
            harmony.PatchAll();
        }
    }
}
