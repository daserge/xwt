using System.Windows;
using Xwt.Accessibility;
using Xwt.Backends;

namespace Xwt.WPFBackend
{
	public class AccessibilityHelper : BaseAccessibilityHelper
	{
		public virtual void MakeAnnouncement(UIElement element, IAccessibleBackend accessible, string message) { }
	}
}
