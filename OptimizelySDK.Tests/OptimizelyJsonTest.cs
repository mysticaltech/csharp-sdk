﻿/**
 *
 *    Copyright 2020, Optimizely and contributors
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */

using Moq;
using NUnit.Framework;
using OptimizelySDK.ErrorHandler;
using OptimizelySDK.Logger;
using System;
using System.Collections.Generic;

namespace OptimizelySDK.Tests
{
    [TestFixture]
    public class OptimizelyJsonTest
    {
        private string Payload;
        private Dictionary<string, object> Map;
        private Mock<ILogger> LoggerMock;
        private Mock<IErrorHandler> ErrorHandlerMock;

        [SetUp]
        public void Initialize()
        {
            ErrorHandlerMock = new Mock<IErrorHandler>();
            ErrorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            LoggerMock = new Mock<ILogger>();
            LoggerMock.Setup(i => i.Log(It.IsAny<LogLevel>(), It.IsAny<string>()));

            Payload = "{ \"field1\": 1, \"field2\": 2.5, \"field3\": \"three\", \"field4\": {\"inner_field1\":3,\"inner_field2\":[\"1\",\"2\", 3, 4.23, true]}, \"field5\": true, }";

            Map = new Dictionary<string, object>() {
                { "strField", "john doe" },
                { "intField", 12 },
                { "doubleField", 2.23 },
                { "boolField", true},
                { "objectField", new Dictionary<string, object> () {
                        { "inner_field_int", 3 },
                        { "inner_field_double", 13.21 },
                        { "inner_field_string", "john" },
                        { "inner_field_boolean", true }
                    }
                }
            };
        }

        [Test]
        public void TestOptimizelyJsonObjectIsValid()
        {
            OptimizelyJson OptimizelyJSONUsingMap = new OptimizelyJson(Map, ErrorHandlerMock.Object, LoggerMock.Object);
            OptimizelyJson OptimizelyJSONUsingString = new OptimizelyJson(Payload, ErrorHandlerMock.Object, LoggerMock.Object);

            Assert.IsNotNull(OptimizelyJSONUsingMap);
            Assert.IsNotNull(OptimizelyJSONUsingString);
        }
        [Test]
        public void TestToStringReturnValidString()
        {
            Dictionary<string, object> map = new Dictionary<string, object>() {
                { "strField", "john doe" },
                { "intField", 12 },
                { "objectField", new Dictionary<string, object> () {
                        { "inner_field_int", 3 }
                    }
                }
            };
            OptimizelyJson OptimizelyJSONUsingMap = new OptimizelyJson(map, ErrorHandlerMock.Object, LoggerMock.Object);
            string str = OptimizelyJSONUsingMap.ToString();
            string expectedStringObj = "{\"strField\":\"john doe\",\"intField\":12,\"objectField\":{\"inner_field_int\":3}}";
            Assert.AreEqual(expectedStringObj, str);
        }

        [Test]
        public void TestGettingErrorUponInvalidJsonString()
        {
            OptimizelyJson OptimizelyJSONUsingString = new OptimizelyJson("{\"invalid\":}", ErrorHandlerMock.Object, LoggerMock.Object);
            LoggerMock.Verify(log => log.Log(LogLevel.ERROR, "Provided string could not be converted to map."), Times.Once);
        }

        [Test]
        public void TestGettingErrorUponNotFindingValuePath()
        {
            OptimizelyJson OptimizelyJSONUsingString = new OptimizelyJson("{\"invalid\":}", ErrorHandlerMock.Object, LoggerMock.Object);
            LoggerMock.Verify(log => log.Log(LogLevel.ERROR, "Provided string could not be converted to map."), Times.Once);
        }

        [Test]
        public void TestOptimizelyJsonGetVariablesWhenSetUsingMap()
        {
            OptimizelyJson OptimizelyJSONUsingMap = new OptimizelyJson(Map, ErrorHandlerMock.Object, LoggerMock.Object);

            Assert.AreEqual(OptimizelyJSONUsingMap.GetValue<string>("strField"), "john doe");
            Assert.AreEqual(OptimizelyJSONUsingMap.GetValue<int>("intField"), 12);
            Assert.AreEqual(OptimizelyJSONUsingMap.GetValue<double>("doubleField"), 2.23);
            Assert.AreEqual(OptimizelyJSONUsingMap.GetValue<bool>("boolField"), true);
            Assert.AreEqual(OptimizelyJSONUsingMap.GetValue<int>("objectField.inner_field_int"), 3);
            Assert.AreEqual(OptimizelyJSONUsingMap.GetValue<double>("objectField.inner_field_double"), 13.21);
            Assert.AreEqual(OptimizelyJSONUsingMap.GetValue<string>("objectField.inner_field_string"), "john");
            Assert.AreEqual(OptimizelyJSONUsingMap.GetValue<bool>("objectField.inner_field_boolean"), true);
            Assert.IsTrue(OptimizelyJSONUsingMap.GetValue<Dictionary<string, object>>("objectField") is Dictionary<string, object>);
        }

        [Test]
        public void TestOptimizelyJsonGetVariablesWhenSetUsingString()
        {
            OptimizelyJson OptimizelyJSONUsingString = new OptimizelyJson(Payload, ErrorHandlerMock.Object, LoggerMock.Object);

            Assert.AreEqual(OptimizelyJSONUsingString.GetValue<long>("field1"), 1);
            Assert.AreEqual(OptimizelyJSONUsingString.GetValue<double>("field2"), 2.5);
            Assert.AreEqual(OptimizelyJSONUsingString.GetValue<string>("field3"), "three");
            Assert.AreEqual(OptimizelyJSONUsingString.GetValue<long>("field4.inner_field1"), 3);
            Assert.AreEqual(OptimizelyJSONUsingString.GetValue<List<object>>("field4.inner_field2"), new List<object>() { "1", "2", 3, 4.23, true });
        }
    }
}
