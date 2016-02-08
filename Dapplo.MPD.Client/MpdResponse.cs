/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.MPD

	Dapplo.MPD is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.MPD is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Dapplo.MPD. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dapplo.MPD.Client
{
	public class MpdResponse
	{
		private const string Ok = "OK";
		private readonly Regex _ackRegex = new Regex(@"^ACK \[(?<error>[^@]+)@(?<command_listNum>[^\]]+)] {(?<command>[^}]*)} (?<message>.*)", RegexOptions.Compiled);

		/// <summary>
		/// Add a line to the response, it will be processed if it is an OK or ACK
		/// As long as false is returned the response is not complete
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

		public IList<string> ResponseLines
		{
			get;
		} = new List<string>();

		public bool IsOk
		{
			get;
			private set;
		}

		public AckCodes ErrorCode
		{
			get;
			private set;
		} = AckCodes.Unknown;

		public string ErrorMessage
		{
			get;
			private set;
		}
	}
}
