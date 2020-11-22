// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dapplo.MPD.Client
{
	public class MpdResponse
	{
		private const string Ok = "OK";
		private readonly Regex _ackRegex = new Regex(@"^ACK \[(?<error>[^@]+)@(?<command_listNum>[^\]]+)] {(?<command>[^}]*)} (?<message>.*)", RegexOptions.Compiled);

		public AckCodes ErrorCode { get; private set; } = AckCodes.Unknown;

		public string ErrorMessage { get; private set; }

		public bool IsOk { get; private set; }

		public IList<string> ResponseLines { get; } = new List<string>();

		/// <summary>
		///     Add a line to the response, it will be processed if it is an OK or ACK
		///     As long as false is returned the response is not complete
		/// </summary>
		/// <param name="line">line to add</param>
		/// <returns>true if the response is completed</returns>
		public bool AddLine(string line)
		{
			if (line.StartsWith(Ok))
			{
				IsOk = true;
				var responseLine = line.Replace(Ok, "");
				if (!string.IsNullOrEmpty(responseLine))
				{
					ResponseLines.Add(responseLine);
				}
				return true;
			}

			var match = _ackRegex.Match(line);
			if (match.Success)
			{
				ErrorMessage = match.Groups["message"].Value;
				AckCodes ackCode;
				if (Enum.TryParse(match.Groups["error"].Value, true, out ackCode))
				{
					ErrorCode = ackCode;
				}
				return true;
			}
			ResponseLines.Add(line);
			return false;
		}
	}
}