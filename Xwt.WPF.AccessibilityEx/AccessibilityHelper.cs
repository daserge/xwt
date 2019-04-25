using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Threading;
using Xwt.Backends;

namespace Xwt.WPF.AccessibilityEx
{
	public sealed class AccessibilityHelper : Xwt.WPFBackend.AccessibilityHelper
	{
		[DllImport("user32.dll")]
		static extern bool SystemParametersInfo (int iAction, int iParam, out bool bActive, int iUpdate);

		static AccessibilityHelper instance;
		static AccessibilityHelper ()
		{
			if (instance == null)
				instance = new AccessibilityHelper ();
			Xwt.Application.AccessibilityHelper = instance;
		}

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
				// AutomationEvents.AutomationFocusChanged returns false sometimes when the application loses
				// accessibility focus. So LiveRegionChanged plays fallback role here (it comes as true more reliable).
				return AutomationPeer.ListenerExists (AutomationEvents.AutomationFocusChanged)
					|| AutomationPeer.ListenerExists (AutomationEvents.LiveRegionChanged);
			}
		}

		public static bool Test {
			get {
				return IsSystemNarratorActive;
			}
		}

		public override bool AccessibilityInUse {
			get {
				return IsSystemNarratorActive || Is3rdPartyScreenReaderActive ();
			}
		}

		public override void MakeAnnouncement (UIElement element, IAccessibleBackend accessible, string message)
		{
			string previousAccessibleLabel = accessible.Label;
			element.Dispatcher.BeginInvoke((Action)(() =>
			{
				AutomationProperties.SetLiveSetting(element, AutomationLiveSetting.Assertive);

				accessible.Label = message;
				var peer = FrameworkElementAutomationPeer.FromElement(element);
				if (peer != null)
				{
					peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);

					// HACK: Giving some time to announce the message
					Task.Run(async () =>
					{
						await Task.Delay(5000);
						element.Dispatcher.BeginInvoke((Action)(() =>
						{
							AutomationProperties.SetLiveSetting(element, AutomationLiveSetting.Off);
							accessible.Label = previousAccessibleLabel;
						}), DispatcherPriority.Render);
					});
				}
			}), DispatcherPriority.Render);
		}

		// TODO: AccessibilityInUseChanged event would require listening for a top-level window' events in WndProc
	}
}
