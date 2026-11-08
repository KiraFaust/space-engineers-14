using Robust.Shared.Configuration;

namespace Content.Shared._NF.CCVar;

[CVarDefs]
public sealed class NFCCVars
{
    /// <summary>
    /// If true, the admin overlay will display the players starting position.
    /// </summary>
    public static readonly CVarDef<bool> AdminOverlayBalance =
        CVarDef.Create("nf14.ui.admin_overlay_balance", true, CVar.CLIENTONLY | CVar.ARCHIVE);
}
