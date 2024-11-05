using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Common.Exceptions
{
	/// <summary>
	/// 404 錯誤
	/// </summary>
	public class NotFoundException : Exception
	{
		public NotFoundException(string message) : base(message)
		{
		}
	}
}
