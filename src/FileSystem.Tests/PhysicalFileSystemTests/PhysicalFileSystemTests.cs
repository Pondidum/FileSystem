﻿using System;
using System.IO;

namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public abstract class PhysicalFileSystemTests : IDisposable
	{
		protected string Root { get; }
		protected PhysicalFileSystem Fs { get; }

		public PhysicalFileSystemTests()
		{
			Root = Guid.NewGuid().ToString();
			Fs = new PhysicalFileSystem();

			Directory.CreateDirectory(Root);
		}

		public virtual void Dispose()
		{
			try
			{
				Directory.Delete(Root, true);
			}
			catch (Exception)
			{
				Console.WriteLine("Unable to Delete test directory");
			}
		}
	}
}
