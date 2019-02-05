﻿using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace Xwt.Mac
{
	public sealed class AccessibilityHelper : Accessibility.BaseAccessibilityHelper
	{
		static AccessibilityHelper instance;
		public AccessibilityHelper ()
		{
			instance = this;
			// TODO: Test if this works with VSM swizzle
			SwizzleNSApplicationAccessibilitySetter ();
		}

		[DllImport ("/usr/lib/libobjc.dylib")]
		private static extern IntPtr class_getInstanceMethod (IntPtr classHandle, IntPtr Selector);

		[DllImport ("/usr/lib/libobjc.dylib")]
		private static extern IntPtr method_getImplementation (IntPtr method);

		[DllImport ("/usr/lib/libobjc.dylib")]
		private static extern IntPtr imp_implementationWithBlock (ref BlockLiteral block);

		[DllImport ("/usr/lib/libobjc.dylib")]
		private static extern void method_setImplementation (IntPtr method, IntPtr imp);

		[MonoNativeFunctionWrapper]
		delegate void AccessibilitySetValueForAttributeDelegate (IntPtr self, IntPtr selector, IntPtr valueHandle, IntPtr attributeHandle);
		delegate void SwizzledAccessibilitySetValueForAttributeDelegate (IntPtr block, IntPtr self, IntPtr valueHandle, IntPtr attributeHandle);

		static IntPtr originalAccessibilitySetValueForAttributeMethod;
		void SwizzleNSApplicationAccessibilitySetter ()
		{
			// Swizzle accessibilitySetValue:forAttribute: so that we can detect when VoiceOver gets enabled
			var nsApplicationClassHandle = Class.GetHandle ("NSApplication");
			var accessibilitySetValueForAttributeSelector = Selector.GetHandle ("accessibilitySetValue:forAttribute:");

			var accessibilitySetValueForAttributeMethod = class_getInstanceMethod (nsApplicationClassHandle, accessibilitySetValueForAttributeSelector);
			originalAccessibilitySetValueForAttributeMethod = method_getImplementation (accessibilitySetValueForAttributeMethod);

			var block = new BlockLiteral ();

			SwizzledAccessibilitySetValueForAttributeDelegate d = accessibilitySetValueForAttribute;
			block.SetupBlock (d, null);
			var imp = imp_implementationWithBlock (ref block);
			method_setImplementation (accessibilitySetValueForAttributeMethod, imp);
		}

		[MonoPInvokeCallback (typeof (SwizzledAccessibilitySetValueForAttributeDelegate))]
		static void accessibilitySetValueForAttribute (IntPtr block, IntPtr self, IntPtr valueHandle, IntPtr attributeHandle)
		{
			var d = Marshal.GetDelegateForFunctionPointer (originalAccessibilitySetValueForAttributeMethod, typeof (AccessibilitySetValueForAttributeDelegate));
			d.DynamicInvoke (self, Selector.GetHandle ("accessibilitySetValue:forAttribute:"), valueHandle, attributeHandle);

			var val = (NSNumber)ObjCRuntime.Runtime.GetNSObject (valueHandle);

			bool previousValue = accessibilityInUse;
			accessibilityInUse = val.BoolValue;
			Console.WriteLine ("accessibilityInUse: " + accessibilityInUse);
			if (accessibilityInUse != previousValue)
				instance.OnAccessibilityInUseChanged ();
		}

		static bool accessibilityInUse;
		public override bool AccessibilityInUse => accessibilityInUse;
	}
}
