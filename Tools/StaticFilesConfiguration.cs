using Microsoft.AspNetCore.Builder;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace KizspyWebApp.ErrorHandling
{
    public static class StaticFilesConfiguration {
		public static void AddStaticFilesConfigs(this IApplicationBuilder app)
		{
			app.UseStaticFiles();
			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/Role")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/Role/Edit")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/Role/Delete")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/Member")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/ManageUser")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/ManageUser/SetPassword")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/ManageUser/AddRole")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/ManageUser/AddClaim")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/Account")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/Categories/Details")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/Categories/Edit")
			});

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
                RequestPath = new PathString("/Categories/Delete")
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
                RequestPath = new PathString("/Categories")
            });

            app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/Products/Edit")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/Products/Details")
			});

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
                RequestPath = new PathString("/Products/Delete")
            });

            app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/Products")
			});

            app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
				RequestPath = new PathString("/database-manage")
			});
		}
	}
}