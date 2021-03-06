﻿#nullable enable
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMember.Global

// ReSharper disable InconsistentNaming
#pragma warning disable CA1416

namespace SimpleCore.Cli
{
	/// <summary>
	/// Describes a console interface for use with <see cref="NConsole"/>
	/// </summary>
	public class NConsoleInterface
	{
		public static string DefaultName { get; set; } = System.Console.Title;

		public NConsoleInterface(IEnumerable<NConsoleOption> options,
			string? name, string? prompt, bool selectMultiple, string? status)
		{
			Options        = options;
			SelectMultiple = selectMultiple;
			Name           = name ?? DefaultName;
			Prompt         = prompt;
			Status         = status;
		}

		public NConsoleInterface(IEnumerable<NConsoleOption> options) : this(options, null, null, false, null) { }


		public NConsoleInterface() : this(Enumerable.Empty<NConsoleOption>()) { }

		public IEnumerable<NConsoleOption> Options { get; set; }

		public bool SelectMultiple { get; set; }

		public string? Name { get; set; }

		public string? Prompt { get; set; }

		public string? Status { get; set; }

		public NConsoleOption this[int i]
		{
			get
			{
				var o = Options.ElementAt(i);


				return o!;
			}
		}

		public int Length => Options.Count();


		public HashSet<object> Run() => NConsole.ReadOptions(this);

		
	}
}