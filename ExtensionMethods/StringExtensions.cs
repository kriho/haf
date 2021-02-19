using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HAF {
  public static class StringExtensions {
    public static bool IsNullOrEmpty(this string s) {
      return String.IsNullOrEmpty(s);
    }

    public static bool IsNullOrWhiteSpace(this string s) {
      return String.IsNullOrWhiteSpace(s);
    }

    public static string Fill(this string s, params object[] values) {
      return string.Format(s, values);
    }

    public static int GetWordCount(this string s) {
      try {
        var re = new Regex(@"[^\s]+");
        return re.Matches(s).Count;
      } catch {
        return 0;
      }
    }

    public static int IndexAfter(this string s, string match) {
      if (!s.Contains(match)) {
        throw new Exception("no instance of \"{0}\" was found.".Fill(match));
      }
      return s.IndexOf(match) + match.Length;
    }

    public static bool ContainsAny(this string s, IEnumerable<string> values) {
      foreach (var value in values) {
        if (s.Contains(value)) {
          return true;
        }
      }
      return false;
    }
  }
}
