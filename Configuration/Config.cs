﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;

namespace Configuration
{
    public class Config
    {
        public static string Browser => ConfigurationManager.AppSettings["Browser"];
        public static string Url => ConfigurationManager.AppSettings["SiteUrl"];
        public static string UserName => ConfigurationManager.AppSettings["Email"];
        public static string Password => ConfigurationManager.AppSettings["Password"];

        public static string Person => ConfigurationManager.AppSettings["Person"];
        public static string AnimalType => ConfigurationManager.AppSettings["AnimalType"];
    }

    public class Parameter
    {
        private static Dictionary<string, object> _parametersDictionary = new Dictionary<string, object>();

        public static void Add<T>(string key, T value, bool shouldLog = false) where T : class
        {
            if (_parametersDictionary.ContainsKey(key))
                _parametersDictionary[key] = value;
            else
                _parametersDictionary.Add(key, value);
        }

        public static T Get<T>(string key, bool shouldLog = false) where T : class
        {
            object value = null;
            _parametersDictionary.TryGetValue(key, out value);
            if (value != null)
                return value as T;        
            Console.WriteLine("[Null Value] - Parameter collection does not contain key: [{0}]", key);
            return null;
        }

        public static void Collect(string parametersFileName, List<string> section)
        {
            string filePath = string.Empty;
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            filePath = directory + parametersFileName;
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            foreach (var sectionXpath in section)
            {
                var xmlNode = xmlDoc.SelectSingleNode(sectionXpath);
                if (xmlNode != null && xmlNode.HasChildNodes)
                {
                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        if ((node.NodeType != (XmlNodeType.Comment)) && (node.NodeType != XmlNodeType.Whitespace))
                            Parameter.Add<string>(node.Name, node.InnerText.ToString());
                    }
                }
                else
                    Console.WriteLine($"[Skipping collection] - Parameters file: [{parametersFileName}]. Section xpath: [{sectionXpath}], there are no parameters or xpath is wrong.");
            }
        }

        public static void ClearParameters()
        {
            _parametersDictionary.Clear();
        }
    }

    public class FakeData
    {
        static string Prefix = "Auto_";
        public static string PhoneNumber
        {
            get
            {
                return new Bogus.DataSets.PhoneNumbers().PhoneNumber();
            }
        }
        public static string ClinicName
        {
            get
            {
                return new Bogus.DataSets.Company().CompanyName();
            }
        }
        public static string FirstName
        {
            get
            {
                return new Bogus.DataSets.Name().FirstName().ToString();
            }
        }
        public static string LastName
        {
            get
            {
                return new Bogus.DataSets.Name().LastName().ToString();
            }
        }
        public static string MiddleName
        {
            get
            {
                return new Bogus.DataSets.Name().ToString();
            }
        }
        public static string Word
        {
            get
            {
                return $"{Prefix}{new Bogus.DataSets.Name().Random.Words(10)}";
            }
        }
        public static string FullAddress
        {
            get { return new Bogus.DataSets.Address().FullAddress(); }
        }
        public static string Zip
        {
            get { return new Bogus.DataSets.Address().ZipCode(); }
        }
        public static string StreetAddress
        {
            get { return new Bogus.DataSets.Address().StreetAddress(); }
        }
        public static string Number(int min = 1, int max = 1000)
        {
            return $"{new Bogus.Randomizer().Number(min, max)}";
        }
    }
}
