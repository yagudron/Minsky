using Discord.Interactions;
using Microsoft.VisualBasic.Devices;
using Minsky.Entities;
using Minsky.Entities.Integration.Sneaker;
using Minsky.Helpers;
using Minsky.Integaration;
using Minsky.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Minsky.Modules
{
    public sealed class HelpModule : SlashCommandModuleBase
    {
        private readonly StatusService _statusService;
        private readonly ConfigurationService _configService;
        private readonly SneakerApiClient _sneakerApiClient;

        private static readonly Dictionary<string, string> _typeDictionary = new()
        {
            { "A_10A", "A-10A" },
            { "A_10C", "A-10C" },
            { "A_10C_2", "A-10C" },
            { "A_20G", "A-20G" },
            { "A_50", "A-50" },
            { "AH_1W", "AH-1W" },
            { "AH_64A", "AH-64A" },
            { "AH_64D", "AH-64D" },
            { "64D_BLK_II", "AH-64D" },
            { "AH-64D_BLK_II", "AH-64D" },
            { "AJS37", "AJS37" },
            { "An_26B", "An-26B" },
            { "An_30M", "An-30M" },
            { "AV8BNA", "AV-8B N/A" },
            { "B_17G", "B-17G" },
            { "B_1B", "B-1B" },
            { "B_52H", "B-52H" },
            { "Bf_109K_4", "Bf-109K-4" },
            { "C_101CC", "C-101CC" },
            { "C_101EB", "C-101EB" },
            { "C_130", "C-130" },
            { "C_17A", "C-17A" },
            { "CH_47D", "CH-47D" },
            { "CH_53E", "CH-53E" },
            { "Christen_Eagle_II", "Christen Eagle II" },
            { "E_2C", "E-2C" },
            { "E_3A", "E-3A" },
            { "F_117A", "F-117A" },
            { "F_14A", "F-14A" },
            { "F_14A_135_GR", "F-14A-135-GR" },
            { "F_14B", "F-14B" },
            { "F_15C", "F-15C" },
            { "F_15E", "F-15E" },
            { "F_16A", "F-16A" },
            { "F_16A_MLU", "F-16A MLU" },
            { "F_16C_bl_52d", "F-16C bl.52d" },
            { "F_4E", "F-4E" },
            { "F_5E", "F-5E" },
            { "F_5E_3", "F-5E-3" },
            { "F_86F_Sabre", "F-86F Sabre" },
            { "F_A_18A", "F/A-18A" },
            { "F-16C_50", "F-16C" },
            { "FA-18C_hornet", "F/A-18C" },
            { "FW_190A8", "FW-190A8" },
            { "FW_190D9", "FW-190D9" },
            { "Hawk", "Hawk" },
            { "I_16", "I-16" },
            { "IL_76MD", "IL-76MD" },
            { "IL_78M", "IL-78M" },
            { "J_11A", "J-11A" },
            { "JF_17", "JF-17" },
            { "Ju_88A4", "Ju-88A4" },
            { "Ka_27", "Ka-27" },
            { "Ka_50", "Ka-50" },
            { "KC_135", "KC-135" },
            { "KC130", "KC130" },
            { "KC135MPRS", "KC135MPRS" },
            { "KJ_2000", "KJ-2000" },
            { "L_39C", "L-39C" },
            { "L_39ZA", "L-39ZA" },
            { "M_2000C", "M-2000C" },
            { "Mi_24P", "Mi-24P" },
            { "Mi_24V", "Mi-24V" },
            { "Mi_26", "Mi-26" },
            { "Mi_28N", "Mi-28N" },
            { "Mi_8MT", "Mi-8MT" },
            { "MiG_15bis", "MiG-15bis" },
            { "MiG_19P", "MiG-19P" },
            { "MiG_21Bis", "MiG-21Bis" },
            { "MiG_23MLD", "MiG-23MLD" },
            { "MiG_25PD", "MiG-25PD" },
            { "MiG_25RBT", "MiG-25RBT" },
            { "MiG_27K", "MiG-27K" },
            { "MiG_29A", "MiG-29A" },
            { "MiG_29G", "MiG-29G" },
            { "MiG_29S", "MiG-29S" },
            { "MiG_31", "MiG-31" },
            { "Mirage_2000_5", "Mirage 2000-5" },
            { "MQ_9_Reaper", "MQ-9 Reaper" },
            { "OH_58D", "OH-58D" },
            { "P_47D_30", "P-47D-30" },
            { "P_47D_30bl1", "P-47D-30bl1" },
            { "P_47D_40", "P-47D-40" },
            { "P_51D", "P-51D" },
            { "P_51D_30_NA", "P-51D-30-NA" },
            { "RQ_1A_Predator", "RQ-1A Predator" },
            { "S_3B", "S-3B" },
            { "S_3B_Tanker", "S-3B Tanker" },
            { "SA342L", "SA342L" },
            { "SA342M", "SA342M" },
            { "SA342Minigun", "SA342Minigun" },
            { "SA342Mistral", "SA342Mistral" },
            { "SH_3W", "SH-3W" },
            { "SH_60B", "SH-60B" },
            { "SpitfireLFMkIX", "SpitfireLFMkIX" },
            { "SpitfireLFMkIXCW", "SpitfireLFMkIXCW" },
            { "Su_17M4", "Su-17M4" },
            { "Su_24M", "Su-24M" },
            { "Su_24MR", "Su-24MR" },
            { "Su_25", "Su-25" },
            { "Su_25T", "Su-25T" },
            { "Su_25TM", "Su-25TM" },
            { "Su_27", "Su-27" },
            { "Su_30", "Su-30" },
            { "Su_33", "Su-33" },
            { "Su_34", "Su-34" },
            { "TF_51D", "TF-51D" },
            { "Tornado_GR4", "Tornado GR4" },
            { "Tornado_IDS", "Tornado IDS" },
            { "Tu_142", "Tu-142" },
            { "Tu_160", "Tu-160" },
            { "Tu_22M3", "Tu-22M3" },
            { "Tu_95MS", "Tu-95MS" },
            { "UH_1H", "UH-1H" },
            { "UH_60A", "UH-60A" },
            { "WingLoong_I", "WingLoong-I" },
            { "Yak_40", "Yak-40" },
            { "Yak_52", "Yak-52" },
            { "F-15ESE", "F-15E" }
        };

        public HelpModule(
            StatusService statusService,
            ConfigurationService configurationService,
            SneakerApiClient sneakerApiClient) : base(configurationService)
        {
            _statusService = statusService;
            _configService = configurationService;
            _sneakerApiClient = sneakerApiClient;
        }

        [SlashCommand("servers", "Get servers info.")]
        public async Task GetServerAsync2()
        {
            var sneakerInfos = await SneakerApiClient.GetServerInfosAsync(_configService.SneakerApiAdress);
            var serverConfigs = _configService.Servers.OrderByDescending(c => c.Name).ToList();
            var embedBuilder = GetDefaultEmbedBuilder().WithTitle("Servers info");

            foreach (var serverConfig in serverConfigs)
            {
                var sneakerInfo = GetServerSneakerInfo(serverConfig, sneakerInfos);
                var portStatus = await StatusService.GetServerStatusAsync(serverConfig);

                var serverName = $"**{serverConfig.DisplayName}**";
                var ip = $"{portStatus.DcsOnline.StatusToEmoji()} **DCS:** {serverConfig.DcsPort.Ip}:{serverConfig.DcsPort.Port}";
                var srs = $"{portStatus.SrsOnline.StatusToEmoji()} **SRS:** {serverConfig.SrsPort.Ip}:{serverConfig.SrsPort.Port}";
                var password = !string.IsNullOrEmpty(serverConfig.Password) ? $"**PASS:** {serverConfig.Password}{Environment.NewLine}" : string.Empty;

                embedBuilder.AddField(serverName, $"{ip}{Environment.NewLine}{password}{srs}", inline: true);

                var playerCount = sneakerInfo.players.Count;
                var playerListTitle = playerCount > 0 ? $"Players ({playerCount})" : "Players";
                embedBuilder.AddField(playerListTitle, ComposePlayerList(sneakerInfo));
            }

            var hasGci = !string.IsNullOrEmpty(_configService.GciLinkUri) && !string.IsNullOrEmpty(_configService.GciLinkTitle);
            if (hasGci)
                embedBuilder.AddField("GCI", $"[{_configService.GciLinkTitle}]({_configService.GciLinkUri})", inline: true);

            var hasAbout = !string.IsNullOrEmpty(_configService.AboutLinkTitle) && !string.IsNullOrEmpty(_configService.AboutLinkUri);
            if (hasAbout)
                embedBuilder.AddField("About", $"[{_configService.AboutLinkTitle}]({_configService.AboutLinkUri})", inline: true);

            await RespondAsync(embedBuilder);
        }

        //[SlashCommand("performance", "Get performance info.")]
        public async Task GetServerperformanceAsync()
        {

            var cpuCounter = new PerformanceCounter
            {
                CategoryName = "Processor Information",
                CounterName = "% Processor Utility",
                InstanceName = "_Total"
            };
            var prevCpuSample = cpuCounter.NextSample();

            var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            var prevRamSample = ramCounter.NextSample();

            Thread.Sleep(1000);

            var currentCpuSample = cpuCounter.NextSample();
            var currentRamSample = ramCounter.NextSample();
            var cpuValue = CounterSample.Calculate(prevCpuSample, currentCpuSample);
            var ramValue = CounterSample.Calculate(prevRamSample, currentRamSample);

            var totalRam = new ComputerInfo().TotalPhysicalMemory / (float)(1024 * 1024 * 1024);
            var usedRam = totalRam - (ramValue / 1024);
            var usedPercentage = (usedRam / totalRam) * 100;
            var description = "```" +
                $"CPU: {Math.Round(cpuValue, 1)}%\r\n" +
                $"RAM: {Math.Round(usedRam, 1)}/{Math.Round((double)totalRam, 1)} GB ({Math.Round(usedPercentage, 0)}%)" +
                "```";
            var embedBuilder = GetDefaultEmbedBuilder()
                .WithTitle("Server performance")
                .WithDescription(description);

            await RespondAsync(embedBuilder);
        }

        private ServerInfoContract GetServerSneakerInfo(ServerConfiguration serverConfig, IEnumerable<ServerInfoContract> infos)
        {
            return infos.SingleOrDefault(i => i.name.ToLowerInvariant() == serverConfig.Name.ToLowerInvariant());
        }

        private static string ComposePlayerList(ServerInfoContract sneakerInfo)
        {
            var result = string.Empty;
            if (sneakerInfo?.players == null)
                return result;

            if (sneakerInfo.players.Count == 0)
                return $"{result}{Environment.NewLine}No active players.";
            else
                sneakerInfo.players.ForEach(p => result += $"{Environment.NewLine}{p.name} ({GetAirCraftType(p.type)})");

            return result;
        }

        private static string GetAirCraftType(string typeAsString)
        {
            return _typeDictionary.TryGetValue(typeAsString, out var description) ? description : typeAsString;
        }
    }
}
