using System.Reflection;

using HarmonyLib;
using Verse;

using UnityEngine;

namespace PawnPeeker
{
    [StaticConstructorOnStartup]
    public class Mod : Verse.Mod
    {
        public Mod(ModContentPack content) : base(content)
        {
            Debug.Log(Assembly.GetExecutingAssembly().ToString());

            GetSettings<Settings>();

#if DEBUG
            Harmony.DEBUG = true;
#endif

            Harmony harmony = new Harmony("com.talakhallen.PawnPeeker");
            harmony.PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            GetSettings<Settings>().DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Pawn Peeker";
        }
    }
}
