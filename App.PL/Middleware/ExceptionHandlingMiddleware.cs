using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net.Mime;
using System.Text.Json;
using App.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace App.PL.Middleware
{
	/// <summary>
	/// 全域錯誤處理
	/// </summary>
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;
		private readonly ProblemDetailsFactory _problemDetailsFactory;

		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, ProblemDetailsFactory problemDetailsFactory)
		{
			_next = next;
			_logger = logger;
			_problemDetailsFactory = problemDetailsFactory;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		/// <summary>
		/// 依 Exception 類型定義不同 ProblemDetails 內容
		/// </summary>
		private Task HandleExceptionAsync(HttpContext context, Exception ex)
		{
			int statusCode;
			string title;

			switch (ex)
			{
				case BadRequestException:
					statusCode = StatusCodes.Status400BadRequest;
					title = ex.Message;
					break;
				case ForbiddenException:
					statusCode = StatusCodes.Status403Forbidden;
					title = ex.Message;
					break;
				case NotFoundException:
					statusCode = StatusCodes.Status404NotFound;
					title = ex.Message;
					break;
				case ConflictException:
				case DbUpdateConcurrencyException:
					statusCode = StatusCodes.Status409Conflict;
					title = ex.Message;
					break;
				case InternalServerException:
					statusCode = StatusCodes.Status500InternalServerError;
					title = ex.Message;
					break;
				default:
					statusCode = StatusCodes.Status500InternalServerError;
					title = "不知名的錯誤發生。";
					break;
			}

			var pb = _problemDetailsFactory.CreateProblemDetails(context, statusCode: statusCode, title: title);
			context.Response.ContentType = MediaTypeNames.Application.Json;
			context.Response.StatusCode = (int)pb.Status!;
			_logger.LogError(ex, "HandleExceptionAsync");
			return context.Response.WriteAsync(JsonSerializer.Serialize(pb));
		}
	}
}
