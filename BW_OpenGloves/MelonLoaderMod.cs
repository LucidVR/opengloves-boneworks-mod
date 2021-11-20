using System;
using System.IO;
using System.IO.Pipes;
using System.Collections;
using System.Collections.Generic;

using MelonLoader;
using UnityEngine;

using Boneworks;
using ModThatIsNotMod;
using StressLevelZero.VRMK;
using StressLevelZero.Interaction;
using Valve.VR;

namespace Mod
{
    public static class BuildInfo
    {
        public const string Name = "BW_OpenGloves"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "L4rs"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class Mod : MelonMod
    {
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            ForceFeedbackLink linkL = new ForceFeedbackLink(ForceFeedbackLink.Handness.Left);
            ForceFeedbackLink linkR = new ForceFeedbackLink(ForceFeedbackLink.Handness.Right);

            Hooking.OnGripAttached += (Grip grip, Hand hand) =>
            {
                if (hand.handedness == StressLevelZero.Handedness.LEFT)
                    linkL.SetFromCurl(hand.fingerCurl);
                else
                    linkR.SetFromCurl(hand.fingerCurl);
            };

            Hooking.OnGripDetached += (Grip grip, Hand hand) =>
            {
                if (hand.handedness == StressLevelZero.Handedness.LEFT)
                    linkL.Relax();
                else
                    linkR.Relax();
            };
        }
    }
}
