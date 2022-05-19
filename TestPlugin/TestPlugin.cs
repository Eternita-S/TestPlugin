using Dalamud.Logging;
using Dalamud.Plugin;
using System;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Configuration;
using Dalamud.Hooking;
using System.Runtime.InteropServices;
using System.Text;

namespace TestPlugin
{
    unsafe public class TestPlugin : IDalamudPlugin
    {
        public string Name => "TestPlugin";
        Config config;
        private delegate byte* ProcessChatBoxDelegate(IntPtr uiModule, IntPtr message, IntPtr unused, byte a4);
        Hook<ProcessChatBoxDelegate> ProcessChatBoxHook;

        public TestPlugin(DalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Svc>();
            try
            {
                ProcessChatBoxHook = new(Svc.SigScanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9"), ProcessChatBoxDetour);
                ProcessChatBoxHook.Enable();
                PluginLog.Information("Enabled");
            }
            catch(Exception e)
            {
                PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
            }
        }

        byte* ProcessChatBoxDetour(IntPtr uiModule, IntPtr message, IntPtr unused, byte a4)
        {
            var payload = (ChatPayload*)message;
            Svc.Chat.PrintError($"Message intercepted: {Encoding.UTF8.GetString(payload->textPtr, (int)payload->textLen)}");
            var original = ProcessChatBoxHook.Original(uiModule, message, unused, a4);
            return original;
        }


        class Config : IPluginConfiguration
        {
            public int Version { get; set; } = 1;
            public SeString sestring = new();
        }

        public void Dispose()
        {
            ProcessChatBoxHook.Disable();
            ProcessChatBoxHook.Dispose();
        }

    }
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct ChatPayload
    {
        [FieldOffset(0)]
        public byte* textPtr;
        [FieldOffset(16)]
        public ulong textLen;
        [FieldOffset(8)]
        public ulong unk1;
        [FieldOffset(24)]
        public ulong unk2;
    }
}
