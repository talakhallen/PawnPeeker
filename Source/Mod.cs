using System.Reflection;

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
