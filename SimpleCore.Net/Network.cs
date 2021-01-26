﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Json;
using System.Linq;
using System.Web;
using RestSharp;

// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

// ReSharper disable UnusedMember.Global
#nullable enable


namespace SimpleCore.Net
{
	public static class Network
	{
		#region Mime

		/// <summary>
		/// Identifies the MIME type of <paramref name="url"/>
		/// </summary>
		public static string IdentifyType(string url)
		{
			var req    = new RestRequest(url, Method.HEAD);
			var client = new RestClient();

			var res = client.Execute(req);

			return res.ContentType;
		}

		public static string GetMimeComponent(string type) => type.Split('/')[0];

		private static readonly string[] ImageMimeTypes =
			{"image", "bmp", "gif", "jpeg", "png", "svg+xml", "tiff", "webp"};

		/// <summary>
		/// Whether the MIME type <paramref name="type"/> is an image type.
		/// </summary>
		public static bool IsImage(string type)
		{
			return ImageMimeTypes.Any(i => i == GetMimeComponent(type));
		}

		#endregion

		public static string? GetFinalRedirect(string url)
		{
			// https://stackoverflow.com/questions/704956/getting-the-redirected-url-from-the-original-url

			if (String.IsNullOrWhiteSpace(url))
				return url;

			const int MAX_REDIR = 8;


			int maxRedirCount = MAX_REDIR; // prevent infinite loops

			string? newUrl = url;

			do {
				HttpWebResponse? resp = null;

				try {
					var req = (HttpWebRequest) WebRequest.Create(url);
					req.Method            = "HEAD";
					req.AllowAutoRedirect = false;
					resp                  = (HttpWebResponse) req.GetResponse();

					switch (resp.StatusCode) {
						case HttpStatusCode.OK:
							return newUrl;
						case HttpStatusCode.Redirect:
						case HttpStatusCode.MovedPermanently:
						case HttpStatusCode.RedirectKeepVerb:
						case HttpStatusCode.RedirectMethod:
							newUrl = resp.Headers["Location"];

							if (newUrl == null)
								return url;

							if (newUrl.IndexOf("://", System.StringComparison.Ordinal) == -1) {
								// Doesn't have a URL Schema, meaning it's a relative or absolute URL
								Uri u = new Uri(new Uri(url), newUrl);
								newUrl = u.ToString();
							}

							break;
						default:
							return newUrl;
					}

					url = newUrl;
				}
				catch (WebException) {
					// Return the last known good URL
					return newUrl;
				}
				catch (Exception) {
					return null;
				}
				finally {
					resp?.Close();
				}
			} while (maxRedirCount-- > 0);

			return newUrl;
		}

		public static string DownloadUrl(string url, string folder)
		{
			string fileName = Path.GetFileName(url);

			using WebClient client = new();
			client.Headers.Add("User-Agent: Other");

			string? dir = Path.Combine(folder, fileName);

			client.DownloadFile(url, dir);

			return dir;
		}

		public static string DownloadUrl(string url)
		{
			string? folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

			return DownloadUrl(url, folder);
		}

		public static void OpenUrl(string url)
		{
			// https://stackoverflow.com/questions/4580263/how-to-open-in-default-browser-in-c-sharp
			// url must start with a protocol i.e. http://

			try {
				Process.Start(url);
			}
			catch {
				// hack because of this: https://github.com/dotnet/corefx/issues/10361
				if (OperatingSystem.IsWindows()) {
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") {CreateNoWindow = true});
				}
				else {
					throw;
				}
			}
		}

		public static IRestResponse GetSimpleResponse(string url)
		{
			var restReq = new RestRequest(url);
			var client  = new RestClient();
			var restRes = client.Execute(restReq);

			return restRes;
		}

		public static string GetString(string url)
		{
			using var wc = new WebClient();
			return wc.DownloadString(url);
		}
	}
}