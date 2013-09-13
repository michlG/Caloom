﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using AaltoGlobalImpact.OIP;
using JsonFx.Json;

namespace AzureSupport
{
    public static class JSONSupport
    {
        public static ExpandoObject GetJsonFromStream(TextReader input)
        {
            var reader = new JsonReader();
            dynamic jsonObject = reader.Read(input);
            return jsonObject;
        }

        public static ExpandoObject GetJsonFromStream(string input)
        {
            var reader = new JsonReader();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var strongType = reader.Read<NodeSummaryContainer>(input);
            watch.Stop();
            var elapsed1 = watch.ElapsedMilliseconds;
            watch.Restart();
            dynamic jsonObject = reader.Read(input);
            watch.Stop();
            var elapsed2 = watch.ElapsedMilliseconds;
            return jsonObject;
        }

        public static T GetObjectFromStream<T>(Stream stream)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            T result = (T) serializer.ReadObject(stream);
            watch.Stop();
            var elapsed = watch.ElapsedMilliseconds;
            return result;
        }
    }
}
