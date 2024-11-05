using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Common.Exceptions
{
	/// <summary>
	/// 403 錯誤
	/// </summary>
	public class ForbiddenException : Exception
	{
		public ForbiddenException(string message) : base(message)
		{
		}
	}
}
