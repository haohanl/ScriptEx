using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ScriptExDee
{
    // Code adapted from Magnus Hjorth, Builder Companion
    /// <summary>
    /// Sets the power mode of the system.
    /// </summary>
    static class PowerControl
    {
        [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
        static extern UInt32 PowerSetActiveScheme(IntPtr RootPowerKey,
        [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid);

        [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
        static extern UInt32 PowerGetActiveScheme(IntPtr UserPowerKey, out IntPtr ActivePolicyGuid);

        //static readonly Guid GUID_SYSTEM_BUTTON_SUBGROUP = new Guid("4f971e89-eebd-4455-a8de-9e59040e7347");
        //static readonly Guid GUID_POWERBUTTON = new Guid("7648efa3-dd9c-4e3e-b566-50f929386280");
        //static readonly Guid GUID_SLEEPBUTTON = new Guid("96996bc0-ad50-47ec-923b-6f41874dd9eb ");
        static readonly Guid GUID_PERFORMANCEMODE = new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
        //static readonly Guid GUID_BATTERYSAVERMODE = new Guid("a1841308-3541-4fab-bc81-f71556f20b4a");

        /// <summary>
        /// Sets the Windows powermode to performance.
        /// </summary>
        public static void SetToPerformance()
        {
            PowerSetActiveScheme(IntPtr.Zero, GUID_PERFORMANCEMODE);
        }
    }
}
