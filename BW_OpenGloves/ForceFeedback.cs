using System;
using System.IO;
using System.IO.Pipes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using StressLevelZero.Player;
using MelonLoader;

namespace Mod
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VRFFBInput
    {
        //Curl goes between 0-1000
        public VRFFBInput(short thumbCurl, short indexCurl, short middleCurl, short ringCurl, short pinkyCurl)
        {
            this.thumbCurl = thumbCurl;
            this.indexCurl = indexCurl;
            this.middleCurl = middleCurl;
            this.ringCurl = ringCurl;
            this.pinkyCurl = pinkyCurl;
        }
        public short thumbCurl;
        public short indexCurl;
        public short middleCurl;
        public short ringCurl;
        public short pinkyCurl;
    };

    public class ForceFeedbackLink
    {
        private NamedPipeClientStream pipe;

        public enum Handness
        {
            Left,
            Right
        }

        public ForceFeedbackLink(Handness handness)
        {
            string hand = handness == Handness.Right ? "right" : "left";
            using (pipe = new NamedPipeClientStream($"vrapplication\\ffb\\curl\\{hand}"))
            {
                MelonLogger.Msg($"Connecting to {hand} hand pipe...");
                try
                {
                    pipe.Connect(5000);
                }
                catch(Exception e) { MelonLogger.Error($"Failed to connect ({e.Message.TrimEnd('\r', '\n')})"); return; }
                if (pipe.IsConnected) { MelonLogger.Msg("Connected!"); } else { MelonLogger.Error("Failed to connect"); return; }
            }
        }

        public void Relax()
        {
            Write(new VRFFBInput(0, 0, 0, 0, 0));
        }

        public void SetFromCurl(FingerCurl curl)
        {
            VRFFBInput input = new VRFFBInput(0, 0, 0, 0, 0);
            input.thumbCurl = (short)(curl.thumb * 1000);
            input.indexCurl = (short)(curl.index * 1000);
            input.middleCurl = (short)(curl.middle * 1000);
            input.ringCurl = (short)(curl.ring * 1000);
            input.pinkyCurl = (short)(curl.pinky * 1000);
            Write(input);
        }

        public void Write(VRFFBInput input)
        {
            MelonLogger.Msg(ConsoleColor.Cyan, $"[OpenGloves] {input.thumbCurl}:{input.indexCurl}:{input.middleCurl}:{input.ringCurl}:{input.pinkyCurl}");

            if (!pipe.IsConnected) return;

            int size = Marshal.SizeOf(input);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(input, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            pipe.Write(arr, 0, size);
        }
    }
}
