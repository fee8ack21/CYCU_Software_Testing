using App.Common.Models;
using Microsoft.OpenApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace App.Common.Exceptions
{
	/// <summary>
	/// 500 錯誤
	/// </summary>
	public class InternalServerException : Exception
	{
		public InternalServerException(string message) : base(message)
		{
		}
	}
}
