using App.Common.Filters;
using App.Common.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json;
using App.Common.Extensions;

namespace App.PL.Others
{
	public class SwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
	{
		private readonly IApiVersionDescriptionProvider _provider;

		public SwaggerOptions(IApiVersionDescriptionProvider provider)
		{
			_provider = provider;
		}

		/// <summary>
		/// Configure Swagger Options. Inherited from the Interface
		/// </summary>
		/// <param name="name"></param>
		/// <param name="options"></param>
		public void Configure(string? name, SwaggerGenOptions options)
		{
			Configure(options);
		}

		/// <summary>
		/// Configure each API discovered for Swagger Documentation
		/// </summary>
		/// <param name="options"></param>
		public void Configure(SwaggerGenOptions options)
		{
			// 使 not nullable 欄位在 swagger.json 上標記為 required
			options.SupportNonNullableReferenceTypes();
			options.SchemaFilter<RequiredNotNullableSchemaFilter>();
			options.OperationFilter<JsonIgnoreQueryOperationFilter>();

			options.CustomOperationIds(e =>
			{
				// 設定 operationId 為 action name，前端若透過 codegen 套件自動產生 api 檔案，可得到一樣的方法名
				var descriptor = e.ActionDescriptor as ControllerActionDescriptor;
				var actionName = descriptor!.ActionName;
				return actionName.ToCamel();
			});

			// 設定 JWT 驗證機制
			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Description = @"使用 JWT 進行驗證，需將登入後取得的 accessToken 攜帶至 request header Authorization 欄位中。
					若欲於 swagger 進行測試，請在以下 Value 欄位輸入：Bearer __ACCESS_TOKEN__",
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer"
			});

			options.AddSecurityRequirement(new OpenApiSecurityRequirement()
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
						Scheme = "oauth2",
						Name = "Bearer",
						In = ParameterLocation.Header,
					},
					new List<string>()
				}
			});

			// 避免 JsonDocument 在 Swagger 顯示為 string 類型
			options.MapType<JsonDocument>(() => new OpenApiSchema { Type = "object" });

			// 加入 xml 檔案到 swagger
			// App.PL, App.Common
			var xmlFiles = new string[] { $"{Assembly.GetExecutingAssembly().GetName().Name}.xml", $"{typeof(GetListBaseRequest).Assembly.GetName().Name}.xml" };
			foreach (var xmlFile in xmlFiles)
			{
				// includeControllerXmlComments - 讀取 Controller 的 Comment
				options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile), includeControllerXmlComments: true);
			}

			foreach (var description in _provider.ApiVersionDescriptions)
			{
				var info = new OpenApiInfo()
				{
					Title = "中原大學 - 軟體測試與漏洞分析",
					Version = description.ApiVersion.ToString(),
					Contact = new OpenApiContact { Name = "黃品睿", Email = "fee8ack21@gmail.com" },
				};

				if (description.IsDeprecated)
				{
					info.Description += " - This API version has been deprecated. Please use one of the new APIs available from the explorer.";
				}

				options.SwaggerDoc(description.GroupName, info);
			}
		}
	}
}
