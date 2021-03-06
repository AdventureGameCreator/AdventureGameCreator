﻿using System;
using System.IO;
using System.Xml.Serialization;

namespace AdventureGameCreator.Data.DataServices
{
    public class XMLDataService
    {
        private static XMLDataService instance;


        /// <summary>
        /// Default constructor
        /// </summary>
        private XMLDataService() { }


        /// <summary>
        /// Returns the instance of the XMLDataService
        /// </summary>
        public static XMLDataService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new XMLDataService();
                }
                return instance;
            }
        }


        /// <summary>
        /// Deserializes the XML data from the specified file path and returns an object of type T
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="filePath">The file path</param>
        /// <returns>Object of type T</returns>
        public T Load<T>(string filePath)
        {
            T deserializedObject;

            deserializedObject = Load<T>(filePath, null);

            return deserializedObject;
        }

        /// <summary>
        /// Deserializes the XML data from the specified file path and returns an object of type T
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="filePath">The file path</param>
        /// <param name="extraTypes">The Type array of additional object types to serialize</param>           
        /// <returns>Object of type T</returns>
        public T Load<T>(string filePath, Type[] extraTypes)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            T deserializedObject;

            if (fileInfo.Exists)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), extraTypes);
                FileStream fileStream = new FileStream(filePath, FileMode.Open);

                deserializedObject = (T)xmlSerializer.Deserialize(fileStream);

                fileStream.Close();
            }
            else
            {
                deserializedObject = (T)Activator.CreateInstance(typeof(T));
                Save<T>(filePath, deserializedObject);
            }

            return deserializedObject;
        }

        /// <summary>
        /// Serializes the serializableObject and saves the XML to the specified file path
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="filePath">The file path</param>
        /// <param name="serializeableObject">The object to serialize</param>
        public void Save<T>(string filePath, T serializeableObject)
        {
            Save<T>(filePath, serializeableObject, null);
        }

        /// <summary>
        /// Serializes the serializableObject and saves the XML to the specified file path
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="filePath">The file path</param>
        /// <param name="serializeableObject">The object to serialize</param>
        /// <param name="extraTypes">The Type array of additional object types to serialize</param>
        public void Save<T>(string filePath, T serializeableObject, Type[] extraTypes)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            Directory.CreateDirectory(fileInfo.Directory.ToString());

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), extraTypes);
            FileStream fileStream = new FileStream(filePath, FileMode.Create);

            xmlSerializer.Serialize(fileStream, serializeableObject);

            fileStream.Close();
        }
    }
}