using KMSGuildExtractor.Core;
using KMSGuildExtractor.Localization;

namespace KMSGuildExtractor
{
    public static class GuildExtension
    {
        public static string ToLocalizedString(this WorldID world) => world switch
        {
            WorldID.Unknown => null,
            WorldID.Scania => LocalizationString.world_scania,
            WorldID.Bera => LocalizationString.world_bera,
            WorldID.Luna => LocalizationString.world_luna,
            WorldID.Zenith => LocalizationString.world_zenith,
            WorldID.Croa => LocalizationString.world_croa,
            WorldID.Union => LocalizationString.world_union,
            WorldID.Elysium => LocalizationString.world_elysium,
            WorldID.Enosis => LocalizationString.world_enosis,
            WorldID.Red => LocalizationString.world_red,
            WorldID.Aurora => LocalizationString.world_aurora,
            WorldID.Reboot => LocalizationString.world_reboot,
            WorldID.Reboot2 => LocalizationString.world_reboot2,
            WorldID.Burning => LocalizationString.world_burning,
            WorldID.Burning2 => LocalizationString.world_burning2,
            WorldID.Arcane => LocalizationString.world_arcane,
            WorldID.Nova => LocalizationString.world_nova,
            _ => null,
        };

        public static string ToLocalizedString(this GuildPosition position) => position switch
        {
            GuildPosition.Owner => LocalizationString.guild_position_owner,
            GuildPosition.Staff => LocalizationString.guild_position_staff,
            GuildPosition.Member => LocalizationString.guild_position_member,
            _ => null,
        };
    }
}
