using System.Runtime.InteropServices;
using System.Windows.Automation.Peers;

namespace Xwt.WPF.Xwt.WPFBackend
{
	public sealed class AccessibilityHelper : Accessibility.BaseAccessibilityHelper
	{
		[DllImport("user32.dll")]
		static extern bool SystemParametersInfo (int iAction, int iParam, out bool bActive, int iUpdate);

		static bool Is3rdPartyScreenReaderActive ()
		{
			int iAction = 70; // SPI_GETSCREENREADER constant;
			int iParam = 0;
			int iUpdate = 0;
			bool bActive = false;
			bool bReturn = SystemParametersInfo (iAction, iParam, out bActive, iUpdate);
			return bReturn && bActive;
		}

		static bool IsSystemNarratorActive {
			get {
				return AutomationPeer.ListenerExists (AutomationEvents.AutomationFocusChanged);
			}
		}

		public override bool AccessibilityInUse {
			get {
				return IsSystemNarratorActive || Is3rdPartyScreenReaderActive ();
			}
		}

		// AccessibilityInUseChanged event would require listening for a top-level window' events in WndProc
	}
}
