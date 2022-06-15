using DN.WebApi.Infrastructure.Common.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Data;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace DN.WebApi.Infrastructure.Common
{
	public static class Utility
	{
		public static string EscapeString(this string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return "''";
			}

			return $"'{input}'";
		}

		public static string EscapeStringWithBracket(this string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return "[]";
			}

			return $"[{input}]";
		}

		public static string EscapeStringWithBrace(this string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return "()";
			}

			return $"({input})";
		}

		public static string EscapeString(this string input, string value)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return $"{value} {value}";
			}

			return $"{value}{input}{value}";
		}

		public static string HandleDbNull(this object o)
		{
			try
			{
				if (o == DBNull.Value)
				{
					return null;
				}

				return Convert.ToString(o);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static object HandleDbNull(this object value, object defaultValue)
		{
			return value == DBNull.Value ? defaultValue : value;
		}

		public static string GetGuidFromObject(this object obj)
		{
			return GetMd5String(obj.SerializeToJson());
		}

		public static string GetGuidFromObject(this object obj, string prefix)
		{
			return $"{prefix}_{ GetMd5String(obj.SerializeToJson())} ";
		}



		public static string GetMd5String(string input)
		{
			var sb = new StringBuilder();

			using (var md5 = MD5.Create())
			{
				var inputBytes = Encoding.ASCII.GetBytes(input);
				var hashBytes = md5.ComputeHash(inputBytes);

				// Convert the byte array to hexadecimal string
				foreach (var t in hashBytes)
				{
					sb.Append(t.ToString("X2"));
				}
				return sb.ToString();
			}
		}

		public static DateTime ConvertFromDateTimeOffset(this DateTimeOffset dateTime)
		{
			if (dateTime.Offset.Equals(TimeSpan.Zero))
				return dateTime.UtcDateTime;
			else if (dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime)))
				return DateTime.SpecifyKind(dateTime.DateTime, DateTimeKind.Local);
			else
				return dateTime.DateTime;
		}


		public static string GetAppRoot()
		{
			var exePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

			if (exePath == null)
				return "";

			var appPathMatcher = new System.Text.RegularExpressions.Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
			return appPathMatcher.Match(exePath).Value;
		}

		public static string SerializeToXml<T>(this T value)
		{
			if (EqualityComparer<T>.Default.Equals(value, default))
			{
				return string.Empty;
			}

			try
			{
				var xmlserializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
				var stringWriter = new StringWriter();
				using (var writer = System.Xml.XmlWriter.Create(stringWriter))
				{
					xmlserializer.Serialize(writer, value);
					return stringWriter.ToString();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Xml serilization failed", ex);
			}
		}



		public static T DeSerializeXml<T>(this string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return default;
			}

			try
			{
				var xmlserializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
				using (TextReader reader = new StringReader(value))
				{
					return (T)xmlserializer.Deserialize(reader);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Xml de-serilization failed", ex);
			}
		}

		public static string RemoveWhiteSpace(this string input)
		{
			return Regex.Replace(input, @"\s+", "");

		}

		public static string ReplaceSingleQuoteToDouble(this string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return $"{value}";
			}
			else
			{
				return value.Replace("'", "''");
			}
		}

		public static string ReplaceCharacters(this string input, string replacement)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return input;
			}

			return Regex.Replace(input, "[^0-9a-zA-Z]+", replacement);
		}

		public static string GetNumber(this string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return input;
			}

			return Regex.Match(input, @"-?\d+").Value;
		}


		public static string RemoveInvalidXmlChars(this string text)
		{

			if (string.IsNullOrWhiteSpace(text))
			{
				return text;
			}

			// From xml spec valid chars:
			// #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]    
			// any Unicode character, excluding the surrogate blocks, FFFE, and FFFF.
			string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
			return Regex.Replace(text, re, "");
		}


		public static string FormatDate(this DateTime dt)
		{
			return dt.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static string FormatDate(this string dt)
		{
			return dt.FormatDate("yyyy-MM-dd HH:mm:ss");
		}

		public static string ToAS400Format(this DateTime dt)
		{
			return dt.ToString("yyMMdd");
		}

		public static string FormatDate(this string dt, string format)
		{
			if (string.IsNullOrWhiteSpace(dt))
			{
				return default;
			}

			DateTime.TryParse(dt, out DateTime dateValue);

			return dateValue.ToString(format);
		}


		public static DateTime GetDateByDayOfWeek(this DateTime dt, DayOfWeek startOfWeek)
		{
			int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
			return dt.AddDays(-1 * diff).Date;
		}

		public static DateTime GetDateByWeeks(this DateTime dt, int Weeks)
		{
			return dt.AddDays(Weeks * 7);
		}

		public static DateTime GetQuaterStartDate(this DateTime dt)
		{
			int offset = 2, monthsInQtr = 3;

			var quarter = (dt.Month + offset) / monthsInQtr;
			var totalMonths = quarter * monthsInQtr;

			return new DateTime(dt.Year, totalMonths - offset, 1);
		}

		public static DateTime GetYearStartDate(this DateTime dt)
		{
			return new DateTime(dt.Year, 1, 1);
		}

		public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
		{
			int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
			return dt.AddDays(-1 * diff).Date;
		}


		public static string ToTitleCase(this string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return input;
			}

			System.Globalization.TextInfo ti = new System.Globalization.CultureInfo("en-US", false).TextInfo;
			return ti.ToTitleCase(input.ToLower());
		}


		public static T GetDefaultOrValue<T>(this object input)
		{
			if (input == null)
			{
				return default;
			}

			return (T)Convert.ChangeType(input, typeof(T));
		}

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> seenKeys = new HashSet<TKey>();
			foreach (TSource element in source)
			{
				if (seenKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}
		}



		public static string PathCombine(this string pathOne, string pathTwo)
		{
			return Path.Combine(pathOne, pathTwo);
		}


		/// <summary>
		/// Get week count .
		/// </summary>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		/// <returns></returns>

		public static int NumberOfWeeks(DateTime fromDate, DateTime toDate)
		{
			TimeSpan span = toDate.Subtract(fromDate);

			if (span.Days <= 7)
			{
				if (fromDate.DayOfWeek > toDate.DayOfWeek)
				{
					return 2;
				}

				return 1;
			}
			int days = span.Days - 7 + (int)fromDate.DayOfWeek;
			int weekCount = 1;
			int dayCount = 0;
			for (weekCount = 1; dayCount < days; weekCount++)
			{
				dayCount += 7;
			}
			return weekCount;
		}
		public static string NullToString(this object? Value) => Value?.ToString() ?? string.Empty;


		public static bool Between(this DateTime dt, DateTime start, DateTime end)
		{
			if (start < end)
			{
				return dt >= start && dt <= end;
			}

			return dt >= end && dt <= start;
		}

		/// <summary>
		/// Returns comma seperated string
		/// </summary>
		/// <param name="lstr"></param>
		/// <param name="c"></param>
		/// <param name="escapeValue"> true to escape the value (result: 'Test Value')</param>
		/// <returns></returns>
		public static string ToCharacterSeparated(this IEnumerable<string> lstr, string c = ",", bool escapeValue = false)
		{
			if (!escapeValue)
				return lstr.Any() ? String.Join(c, lstr) : string.Empty;
			else
				return lstr.Any() ? String.Join(c, lstr.Select(t => t.EscapeString(Symbols.Collan))) : string.Empty;
		}

		public static T GetValue<T>(this IDictionary<string, object> input, string key)
		{
			if (input == null || input[key] == null)
			{
				return default;
			}

			return (T)Convert.ChangeType(input[key], typeof(T));
		}


		/// <summary>
		/// Returns string as (ColumnName = 'Value').Handles Comma seperated values with IN operator
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetCondition(string columnName, string value, string Operator = Symbols.Equal)
		{
			if (!string.IsNullOrWhiteSpace(value) && value.Contains(","))
			{
				return GetCondition(columnName, value.Split(','));

			}

			string retValue = Operator switch
			{
				Symbols.Equal => $" ({columnName} = {value.EscapeString(Symbols.Collan)})",
				Symbols.NotEqual => $" ({columnName} != {value.EscapeString(Symbols.Collan)})",
				Symbols.Like => $" ({columnName} LIKE {value.EscapeString(Symbols.Percentile).EscapeString(Symbols.Collan)})",
				Symbols.In => $" ({columnName} IN {value.EscapeString(Symbols.Collan)})",
				_ => $" ({columnName} = ({value.EscapeString(Symbols.Collan)}))",
			};

			return retValue;
		}

		public static string GetCondition(string columnName, IEnumerable<string> value)
		{
			if (value.Count() < 1)
				return default;


			if (value.Count() == 1)
			{
				return $" ({columnName} = {value.First().EscapeString(Symbols.Collan)})";
			}

			return $" ({columnName} IN ({value.ToCharacterSeparated(Symbols.Comma, true)}))";
		}


		/// <summary>
		/// Appends Where to the existing string when incrementor 0 else AND 
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="incrementor"></param>
		public static void IncludeOperator(this StringBuilder sb, ref int incrementor)
		{
			switch (incrementor)
			{
				case 0:
					sb.Append(" WHERE ");
					break;
				case > 0:
					sb.Append(" AND ");
					break;
				default:
					sb.Append(" ");
					break;
			}

			incrementor++;
		}

		public static bool IsFileLocked(FileInfo file)
		{
			try
			{
				using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
				{
					stream.Close();
				}
			}
			catch (IOException)
			{
				//the file is unavailable because it is: still being written toor being processed by another thread or does not exist
				return true;
			}

			//file is not locked
			return false;
		}

		public static T DeserializeJson<T>(this string text)
		{
			return JsonConvert.DeserializeObject<T>(text);
		}

		public static string SerializeToJson<T>(this T obj)
		{
			return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				NullValueHandling = NullValueHandling.Ignore,

			});
		}

		public static string Serialize<T>(this T obj, Type type)
		{
			return JsonConvert.SerializeObject(obj, type, new());
		}


		/// <summary>
		/// Returns sub string of Last characters
		/// </summary>
		/// <param name="input"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string Right(this string input, int length)
		{
			if (string.IsNullOrWhiteSpace(input))
				return "";

			input = input.Trim();

			return input.Substring(input.Length - length, length);
		}

		public static string Range(this string input, int length)
		{
			if (string.IsNullOrWhiteSpace(input))
				return "";

			return input[..length];
		}

		public static string Left(this string input, int length, int index = 0)
		{
			if (string.IsNullOrWhiteSpace(input))
				return "";

			return input.Substring(index, length);
		}



	}
}

