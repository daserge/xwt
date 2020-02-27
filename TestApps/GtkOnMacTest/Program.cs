// 
// Main.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using Samples;
using Xwt;

namespace GtkTest
{
	class MainClass
	{
		static string LIB_ATKCOCOA_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../../libs/libatkcocoa.so");
		static string LIB_ATKCOCOA_PATH_MONODEVELOP = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../../monodevelop/main/build/lib/gtk-2.0/libatkcocoa.so");
		public static void Main (string[] args)
		{
			AddAtkCocoa();
			App.Run (ToolkitType.Gtk);
		}

		static void AddAtkCocoa()
		{
			// Taking the AtkCocoa from monodevelop project if it exists and built
			if (File.Exists(LIB_ATKCOCOA_PATH_MONODEVELOP))
				Environment.SetEnvironmentVariable("GTK_MODULES", LIB_ATKCOCOA_PATH_MONODEVELOP);
			else if (File.Exists(LIB_ATKCOCOA_PATH))
			{
				Environment.SetEnvironmentVariable("GTK_MODULES", LIB_ATKCOCOA_PATH);
			}
			else
			{
				Console.WriteLine("AtkCocoa library was not found. Accessibility may not work properly.");
			}
		}

	}
}
