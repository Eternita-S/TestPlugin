global using static ECommons.GenericHelpers;
global using ECommons.DalamudServices;
using Dalamud.Logging;
using Dalamud.Plugin;
using System;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Configuration;
using Dalamud.Hooking;
using System.Runtime.InteropServices;
using System.Text;
using Dalamud.Game;
using Dalamud.Game.ClientState.Keys;
using ECommons.Reflection;
using Dalamud.Game.Network;
using System.Linq;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using ImGuiNET;
using System.Numerics;
using System.Diagnostics;
using ECommons;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Net;

namespace TestPlugin
{
    unsafe public class TestPlugin : IDalamudPlugin
    {
        public string Name => "TestPlugin";
        Config config;

        public TestPlugin(DalamudPluginInterface pluginInterface)
        {
            ECommons.ECommons.Init(pluginInterface, ECommons.Module.All);
            //Svc.Framework.Update += OnUpdate;
            Svc.PluginInterface.UiBuilder.Draw += Draw;
            Svc.GameNetwork.NetworkMessage += Message;
            Svc.Chat.PrintChat(new()
            {
                Type = Dalamud.Game.Text.XivChatType.Ls8,
                Message = $"Initialized"
            });
            //Module.Initialize();
        }

        private void Message(IntPtr dataPtr, ushort opCode, uint sourceActorId, uint targetActorId, NetworkMessageDirection direction)
        {
            if (opCode == 0x2E0 && direction == NetworkMessageDirection.ZoneDown)
            {
                var fx = (FFXIVIpcMapEffect*)dataPtr;
                Svc.Chat.PrintChat(new()
                {
                    Type = Dalamud.Game.Text.XivChatType.Ls8,
                    Message = $"{fx->parm1:X8}, {fx->parm2:X8}, {fx->parm3:X4}, {fx->parm4:X4}"
                });
            }
        }

        string s = "";
        void Draw()
        {
            return;
            /*if (ImGui.Begin("Test"))
            {
                Safe(delegate
                {
                    ImGui.InputText("Name", ref s, 100);
                    if (ImGui.Button("Select tradecraft"))
                    {
                        if (TryGetAddonByName<AddonGuildLeve>("GuildLeve", out var addon))
                        {
                            ClickGuildLeve.Using((IntPtr)addon).Tradecraft();
                        }
                    }
                    if (ImGui.Button("Select alchemist"))
                    {
                        if (TryGetAddonByName<AddonGuildLeve>("GuildLeve", out var addon))
                        {
                            ClickGuildLeve.Using((IntPtr)addon).Alchemist();
                        }
                    }
                    if (ImGui.Button("Unwrap all"))
                    {
                        if (TryGetAddonByName<AtkUnitBase>("GuildLeve", out var addon))
                        {
                            var list = addon->UldManager.NodeList[11]->GetAsAtkComponentNode();
                            for (var i = 1; i < list->Component->UldManager.NodeListCount; i++)
                            {
                                var node = list->Component->UldManager.NodeList[i]->GetAsAtkComponentNode();
                                if (node->Component->UldManager.NodeListCount > 3)
                                {
                                    var item = node->Component->UldManager.NodeList[3];
                                    if (item->Type == NodeType.Image)
                                    {
                                        var image = item->GetAsAtkImageNode();
                                        if (image->PartId == 0)
                                        {
                                            var item2 = node->Component->UldManager.NodeList[2];
                                            if (item2->Type == NodeType.Text)
                                            {
                                                var text = item2->GetAsAtkTextNode();
                                                var s = text->NodeText.ToString();
                                                if (s.StartsWith("Level "))
                                                {
                                                    PluginLog.Information(s);
                                                    var atkList = (AtkComponentList*)list;
                                                    //var t = atkList->
                                                    //PluginLog.Information(t);
                                                    //Safe(() => Module.ClickList(addon, list, i));
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
            ImGui.End();*/
        }

        private void OnUpdate(Framework framework)
        {
            /*if (Svc.KeyState.GetRawValue(VirtualKey.KEY_1) != 0)
            {
                Svc.KeyState.SetRawValue(VirtualKey.KEY_1, 0);
                DalamudReflector.SetKeyState(VirtualKey.KEY_3, 3);
            }*/
        }

        class Config : IPluginConfiguration
        {
            public int Version { get; set; } = 1;
            /*public SeString sestring = new();
            internal static bool AutoHideAfterOpener;*/
        }

        public void Dispose()
        {
            Safe(delegate
            {
                ECommons.ECommons.Dispose();
                Svc.Framework.Update -= OnUpdate;
                Svc.PluginInterface.UiBuilder.Draw -= Draw;
                Svc.GameNetwork.NetworkMessage -= Message;
                //Module.Dispose();
            });
        }

    }
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct FFXIVIpcMapEffect
    {
        [FieldOffset(0)]
        public uint parm1;

        [FieldOffset(4)]
        public uint parm2;

        [FieldOffset(8)]
        public ushort parm3;

        [FieldOffset(12)]
        public ushort parm4;
    }
}