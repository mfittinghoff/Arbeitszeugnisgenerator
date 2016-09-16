﻿using Brockhaus.PraktikumZeugnisGenerator.Model;
using DocumentFormat.OpenXml;
using Novacode;
using System.Text;
using System.Text.RegularExpressions;


namespace Brockhaus.PraktikumZeugnisGenerator.Services
{
    public class StringEditor
    {
        public static string ReplaceMuster(InternDetails internDetails, string docText)
        {
            Regex regExMuster = new Regex(@"(<<NACHNAME>>)",RegexOptions.IgnoreCase);
            docText = regExMuster.Replace(docText, internDetails.LastName != null ? internDetails.LastName : "");
            return docText;
        }

        public static void ReplaceMuster(InternDetails internDetails, DocX document,string path)
        {
            Regex regExMuster = new Regex(@"(<<NACHNAME>>)", RegexOptions.IgnoreCase);
            string text = document.Text.ToString();
            MatchCollection mc = regExMuster.Matches(text);
            var matches = new string[mc.Count];
            for (int i = 0; i < matches.Length; i++)
            {
                document.ReplaceText(mc[i].ToString(), internDetails.LastName != null ? internDetails.LastName : "");
            }
            document.SaveAs(path);
        }

        public static void ReplaceWordsBasedOnGender(DocX document, InternDetails internDetails, string path)
        {
            string text = document.Text.ToString();
            ReplaceWordsBasedOnGender(document, internDetails);
            document.SaveAs(path);
        }

        public static void ReplaceWordsBasedOnGender(DocX document, InternDetails internDetails)
        {
            Regex replaceTag = new Regex(@"(<<.*?\/.*?>>)");
            Regex reg_male = new Regex(@"(<<.*?\/)");
            Regex reg_female = new Regex(@"(\/.*?>>)");
            string word = "DUMMY/FEHLER";
            string text = document.Text.ToString();

            MatchCollection mc = replaceTag.Matches(text);
            var matches = new string[mc.Count];
            for (int i = 0; i < matches.Length; i++)
            {
                matches[i] = mc[i].ToString();

                if (internDetails.Sex == Sex.Male)
                {
                    string tempMale = reg_male.Match(matches[i].ToString()).ToString();
                    word = tempMale.Substring(2, tempMale.Length - 3);
                    document.ReplaceText(mc[i].ToString(), word);
                }
                else
                {
                    string tempFemale = reg_female.Match(matches[i].ToString()).ToString();
                    word = tempFemale.Substring(1, tempFemale.Length - 3);
                    document.ReplaceText(mc[i].ToString(), word);
                }
            }
        }

        public static string ReplaceWordsBasedOnGender(InternDetails internDetails, string doctText)
        {
            Regex replaceTag = new Regex(@"(<<.*?>>)");
            Regex reg_male = new Regex(@"(<<.*?\/)");
            Regex reg_female = new Regex(@"(\/.*?>>)");
            string word = "DUMMY/FEHLER";

            MatchCollection mc = replaceTag.Matches(doctText);
            var matches = new string[mc.Count];
            for (int i = 0; i < matches.Length; i++)
            {
                matches[i] = mc[i].ToString();

                if (Regex.IsMatch(matches[i], @"(<<.*?\/.*?>>)")) //MW Wenn Geschlechtsanhängiges Wort
                {
                    if (internDetails.Sex == Sex.Male)
                    {
                        string tempMale = reg_male.Match(matches[i]).ToString();
                        word = tempMale.Substring(2, tempMale.Length - 3);
                        doctText = doctText.Replace(mc[i].ToString(), word);
                    }
                    else
                    {
                        string tempFemale = reg_female.Match(matches[i]).ToString();
                        word = tempFemale.Substring(1, tempFemale.Length - 3);
                        doctText = doctText.Replace(mc[i].ToString(), word);
                    }
                }
                else
                {
                    doctText = doctText.Replace(mc[i].ToString(),ReplaceMuster(internDetails, matches[i]));
                }
            }
            return doctText;
        }

        public static string GetFirstOccuringGenderWord(string doctText)
        {
            Regex replaceTag = new Regex(@"(<<.*?\/.*?>>)");
            Match mc = Regex.Match(doctText, @"(<<.*?\/.*?>>)");

            return mc.ToString();
        }

        private static string GetPlainText(OpenXmlElement element)
        {
            System.Text.StringBuilder PlainTextInWord = new StringBuilder();
            foreach (OpenXmlElement section in element.Elements())
            {
                switch (section.LocalName)
                {
                    // Text 
                    case "t":
                        PlainTextInWord.Append(section.InnerText);
                        break;
                    case "cr":                          // Carriage return 
                    case "br":                          // Page break 
                        PlainTextInWord.Append(System.Environment.NewLine);
                        break;
                    // Tab 
                    case "tab":
                        PlainTextInWord.Append("\t");
                        break;
                    // Paragraph 
                    case "p":
                        PlainTextInWord.Append(GetPlainText(section));
                        PlainTextInWord.AppendLine(System.Environment.NewLine);
                        break;
                    default:
                        PlainTextInWord.Append(GetPlainText(section));
                        break;
                }
            }
            return PlainTextInWord.ToString();
        }
    }
}
