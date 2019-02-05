using System;

namespace Xwt.Accessibility
{
	public class BaseAccessibilityHelper
	{
		public virtual bool AccessibilityInUse { get; } = false;
		public event EventHandler AccessibilityInUseChanged;
		protected virtual void OnAccessibilityInUseChanged ()
		{
			AccessibilityInUseChanged?.Invoke (this, EventArgs.Empty);
		}

		// TODO:
		public virtual void MakeAccessibilityAnnouncement () { }
	}
}
