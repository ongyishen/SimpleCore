﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

// ReSharper disable UnusedMember.Global
#nullable enable
#pragma warning disable HAA0601, HAA0502, HAA0505

namespace SimpleCore.CommandLine.Shell
{
	/// <summary>
	///     Utilities for working with the command prompt.
	/// </summary>
	/// <remarks>
	///     Used to implement <see cref="BaseCommand" />, <see cref="BatchFileCommand" />, <see cref="ConsoleCommand" />
	/// </remarks>
	public static class Command
	{
		/// <summary>
		///     Creates a <see cref="Process" /> to execute <paramref name="cmd" /> with the command prompt.
		/// </summary>
		/// <param name="cmd">Command to run</param>
		/// <returns>Created command prompt process</returns>
		public static Process Shell(string cmd)
		{
			var startInfo = new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = $"/C {cmd}",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			var process = new Process
			{
				StartInfo = startInfo,
				EnableRaisingEvents = true
			};


			return process;
		}

		public static string[] ReadAllLines(StreamReader stream)
		{
			var list = new List<string>();

			while (!stream.EndOfStream) {
				string? line = stream.ReadLine();

				if (line != null) {
					list.Add(line);
				}
			}

			return list.ToArray();

		}
	}
}