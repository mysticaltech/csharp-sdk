﻿/* 
 * Copyright 2020, Optimizely
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using OptimizelySDK.ErrorHandler;
using OptimizelySDK.Logger;
using Moq;
using Xunit;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OptimizelySDK.Exceptions;
using OptimizelySDK.Entity;
using Newtonsoft.Json;
using OptimizelySDK.Utils;
using OptimizelySDK.Config;

namespace OptimizelySDK.XUnitTests
{
    public class ProjectConfigTest
    {
        private Mock<ILogger> LoggerMock;
        private Mock<IErrorHandler> ErrorHandlerMock;
        private ProjectConfig Config;

        public ProjectConfigTest()
        {
            LoggerMock = new Mock<ILogger>();
            ErrorHandlerMock = new Mock<IErrorHandler>();
            ErrorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            Config = DatafileProjectConfig.Create(TestData.Datafile, LoggerMock.Object, ErrorHandlerMock.Object);
        }

        public static Dictionary<string, object> CreateDictionary(string name, object entityObject)
        {
            return new Dictionary<string, object>() { { name, entityObject } };
        }

        [Fact]
        public void TestInit()
        {
            // Check Version
            Assert.Equal("4", Config.Version);

            // Check Account ID
            Assert.Equal("1592310167", Config.AccountId);
            // Check Project ID
            Assert.Equal("7720880029", Config.ProjectId);
            // Check Revision 
            Assert.Equal("15", Config.Revision);

            // Check Group ID Map
            var expectedGroupId = CreateDictionary("7722400015", Config.GetGroup("7722400015"));

            var actual = Config.GroupIdMap;
            Assert.True(TestData.CompareObjects(expectedGroupId, actual));

            // Check Experiment Key Map
            var experimentKeyMap = new Dictionary<string, object>()
            {
                {"test_experiment",Config.GetExperimentFromKey("test_experiment") },
                { "paused_experiment",Config.GetExperimentFromKey("paused_experiment") },
                { "test_experiment_multivariate",Config.GetExperimentFromKey("test_experiment_multivariate") },
                { "test_experiment_with_feature_rollout",Config.GetExperimentFromKey("test_experiment_with_feature_rollout") },
                { "test_experiment_double_feature",Config.GetExperimentFromKey("test_experiment_double_feature") },
                { "test_experiment_integer_feature",Config.GetExperimentFromKey("test_experiment_integer_feature") },
                { "group_experiment_1",Config.GetExperimentFromKey("group_experiment_1") },
                {"group_experiment_2",Config.GetExperimentFromKey("group_experiment_2") },
                {"etag1",Config.GetExperimentFromKey("etag1") },
                {"etag2",Config.GetExperimentFromKey("etag2") },
                {"etag3",Config.GetExperimentFromKey("etag3") },
                {"etag4",Config.GetExperimentFromKey("etag4") }
            };

            Assert.True(TestData.CompareObjects(experimentKeyMap, Config.ExperimentKeyMap));

            // Check Experiment ID Map

            var experimentIdMap = new Dictionary<string, object>()
            {
                {"7716830082",Config.GetExperimentFromId("7716830082") },
                {"7716830585",Config.GetExperimentFromId("7716830585") },
                {"122230",Config.GetExperimentFromId("122230") },
                {"122235",Config.GetExperimentFromId("122235") },
                {"122238",Config.GetExperimentFromId("122238") },
                {"122241",Config.GetExperimentFromId("122241") },
                { "7723330021",Config.GetExperimentFromId("7723330021") },
                { "7718750065",Config.GetExperimentFromId("7718750065") },
                { "223",Config.GetExperimentFromId("223") },
                { "118",Config.GetExperimentFromId("118") },
                { "224",Config.GetExperimentFromId("224") },
                { "119",Config.GetExperimentFromId("119") }
            };

            Assert.True(TestData.CompareObjects(experimentIdMap, Config.ExperimentIdMap));

            // Check Event key Map
            var eventKeyMap = new Dictionary<string, object> { { "purchase", Config.GetEvent("purchase") } };
            Assert.True(TestData.CompareObjects(eventKeyMap, Config.EventKeyMap));

            // Check Attribute Key Map
            var attributeKeyMap = new Dictionary<string, object>
            {
                { "device_type", Config.GetAttribute("device_type") },
                { "location", Config.GetAttribute("location")},
                { "browser_type", Config.GetAttribute("browser_type")},
                { "boolean_key", Config.GetAttribute("boolean_key")},
                { "integer_key", Config.GetAttribute("integer_key")},
                { "double_key", Config.GetAttribute("double_key")}
            };
            Assert.True(TestData.CompareObjects(attributeKeyMap, Config.AttributeKeyMap));

            // Check Audience ID Map
            var audienceIdMap = new Dictionary<string, object>
            {
                { "7718080042", Config.GetAudience("7718080042") },
                { "11154", Config.GetAudience("11154") },
                { "100", Config.GetAudience("100") }
            };
            Assert.True(TestData.CompareObjects(audienceIdMap, Config.AudienceIdMap));

            // Check Variation Key Map
            var expectedVariationKeyMap = new Dictionary<string, object>
            {
                { "test_experiment", new Dictionary<string, object>
                 {
                    { "control", Config.GetVariationFromKey("test_experiment", "control") },
                    { "variation", Config.GetVariationFromKey("test_experiment", "variation")}
                 }
                },
                { "paused_experiment", new Dictionary<string, object>
                 {
                     { "control", Config.GetVariationFromKey("paused_experiment", "control") },
                     { "variation", Config.GetVariationFromKey("paused_experiment", "variation") }
                 }
                },
                { "group_experiment_1", new Dictionary<string, object>
                 {
                    {"group_exp_1_var_1", Config.GetVariationFromKey("group_experiment_1", "group_exp_1_var_1") },
                    { "group_exp_1_var_2", Config.GetVariationFromKey("group_experiment_1", "group_exp_1_var_2") }
                 }
                },
                { "group_experiment_2", new Dictionary<string, object>
                 {
                     {"group_exp_2_var_1", Config.GetVariationFromKey("group_experiment_2", "group_exp_2_var_1") },
                     { "group_exp_2_var_2", Config.GetVariationFromKey("group_experiment_2", "group_exp_2_var_2") }
                 }
                },
                { "test_experiment_multivariate", new Dictionary<string, object>
                 {
                     {"Fred", Config.GetVariationFromKey("test_experiment_multivariate", "Fred") },
                     { "Feorge", Config.GetVariationFromKey("test_experiment_multivariate", "Feorge") },
                     { "Gred", Config.GetVariationFromKey("test_experiment_multivariate", "Gred") },
                     { "George", Config.GetVariationFromKey("test_experiment_multivariate", "George") }
                 }
                },
                { "test_experiment_with_feature_rollout", new Dictionary<string, object>
                 {
                     {"control", Config.GetVariationFromKey("test_experiment_with_feature_rollout", "control") },
                     { "variation", Config.GetVariationFromKey("test_experiment_with_feature_rollout", "variation") }
                 }
                },
                { "test_experiment_double_feature", new Dictionary<string, object>
                 {
                     {"control", Config.GetVariationFromKey("test_experiment_double_feature", "control") },
                     { "variation", Config.GetVariationFromKey("test_experiment_double_feature", "variation") }
                 }
                },
                { "test_experiment_integer_feature", new Dictionary<string, object>
                 {
                     {"control", Config.GetVariationFromKey("test_experiment_integer_feature", "control") },
                     { "variation", Config.GetVariationFromKey("test_experiment_integer_feature", "variation") }
                 }
                },
                { "177770", new Dictionary<string, object>
                 {
                     {"177771", Config.GetVariationFromKey("177770", "177771") }
                 }
                },
                { "177772", new Dictionary<string, object>
                 {
                     {"177773", Config.GetVariationFromKey("177772", "177773") }
                 }
                },
                { "177776", new Dictionary<string, object>
                 {
                     {"177778", Config.GetVariationFromKey("177776", "177778") }
                 }
                },
                { "177774", new Dictionary<string, object>
                 {
                     {"177775", Config.GetVariationFromKey("177774", "177775") }
                 }
                },
                { "177779", new Dictionary<string, object>
                 {
                     {"177780", Config.GetVariationFromKey("177779", "177780") }
                 }
                },
                { "177781", new Dictionary<string, object>
                 {
                     {"177782", Config.GetVariationFromKey("177781", "177782") }
                 }
                },
                { "177783", new Dictionary<string, object>
                 {
                     {"177784", Config.GetVariationFromKey("177783", "177784") }
                 }
                },
                { "188880", new Dictionary<string, object>
                 {
                     {"188881", Config.GetVariationFromKey("188880", "188881") }
                 }
                },
                { "etag1", new Dictionary<string, object>
                 {
                     {"vtag1", Config.GetVariationFromKey("etag1", "vtag1") },
                     {"vtag2", Config.GetVariationFromKey("etag1", "vtag2") }
                 }
                },
                { "etag2", new Dictionary<string, object>
                 {
                     {"vtag3", Config.GetVariationFromKey("etag2", "vtag3") },
                     {"vtag4", Config.GetVariationFromKey("etag2", "vtag4") }
                 }
                },
                { "etag3", new Dictionary<string, object>
                 {
                     {"vtag5", Config.GetVariationFromKey("etag3", "vtag5") },
                     {"vtag6", Config.GetVariationFromKey("etag3", "vtag6") }
                 }
                },
                { "etag4", new Dictionary<string, object>
                 {
                     {"vtag7", Config.GetVariationFromKey("etag4", "vtag7") },
                     {"vtag8", Config.GetVariationFromKey("etag4", "vtag8") }
                 }
                }
            };

            Assert.True(TestData.CompareObjects(expectedVariationKeyMap, Config.VariationKeyMap));

            // Check Variation ID Map
            var expectedVariationIdMap = new Dictionary<string, object>
            {
                { "test_experiment", new Dictionary<string, object>
                 {
                     {"7722370027", Config.GetVariationFromId("test_experiment", "7722370027") },
                     { "7721010009", Config.GetVariationFromId("test_experiment", "7721010009") }
                 }
                },
                { "paused_experiment", new Dictionary<string, object>
                 {
                     {"7722370427", Config.GetVariationFromId("paused_experiment", "7722370427") },
                     { "7721010509", Config.GetVariationFromId("paused_experiment", "7721010509") }
                 }
                },
                { "test_experiment_multivariate", new Dictionary<string, object>
                 {
                     { "122231", Config.GetVariationFromId("test_experiment_multivariate", "122231") },
                     { "122232", Config.GetVariationFromId("test_experiment_multivariate", "122232") },
                     { "122233", Config.GetVariationFromId("test_experiment_multivariate", "122233") },
                     { "122234", Config.GetVariationFromId("test_experiment_multivariate", "122234") }
                 }
                },
                { "test_experiment_with_feature_rollout", new Dictionary<string, object>
                 {
                     { "122236", Config.GetVariationFromId("test_experiment_with_feature_rollout", "122236") },
                     { "122237", Config.GetVariationFromId("test_experiment_with_feature_rollout", "122237") }
                 }
                },
                { "test_experiment_double_feature", new Dictionary<string, object>
                 {
                     { "122239", Config.GetVariationFromId("test_experiment_double_feature", "122239") },
                     { "122240", Config.GetVariationFromId("test_experiment_double_feature", "122240") }
                 }
                },
                { "test_experiment_integer_feature", new Dictionary<string, object>
                 {
                     { "122242", Config.GetVariationFromId("test_experiment_integer_feature", "122242") },
                     { "122243", Config.GetVariationFromId("test_experiment_integer_feature", "122243") }
                 }
                },
                { "group_experiment_1", new Dictionary<string, object>
                 {
                     {"7722260071", Config.GetVariationFromId("group_experiment_1", "7722260071") },
                     { "7722360022", Config.GetVariationFromId("group_experiment_1", "7722360022")}
                 }
                },
                { "group_experiment_2", new Dictionary<string, object>
                 {
                     {"7713030086", Config.GetVariationFromId("group_experiment_2", "7713030086") },
                     { "7725250007", Config.GetVariationFromId("group_experiment_2", "7725250007")}
                 }
                },
                { "177770", new Dictionary<string, object>
                 {
                     {"177771", Config.GetVariationFromId("177770", "177771") }
                 }
                },
                { "177772", new Dictionary<string, object>
                 {
                     {"177773", Config.GetVariationFromId("177772", "177773") }
                 }
                },
                { "177776", new Dictionary<string, object>
                 {
                     {"177778", Config.GetVariationFromId("177776", "177778") }
                 }
                },
                { "177774", new Dictionary<string, object>
                 {
                     {"177775", Config.GetVariationFromId("177774", "177775") }
                 }
                },
                { "177779", new Dictionary<string, object>
                 {
                     {"177780", Config.GetVariationFromId("177779", "177780") }
                 }
                },
                { "177781", new Dictionary<string, object>
                 {
                     {"177782", Config.GetVariationFromId("177781", "177782") }
                 }
                },
                { "177783", new Dictionary<string, object>
                 {
                     {"177784", Config.GetVariationFromId("177783", "177784") }
                 }
                },
                { "188880", new Dictionary<string, object>
                 {
                     {"188881", Config.GetVariationFromId("188880", "188881") }
                 }
                },
                { "etag1", new Dictionary<string, object>
                 {
                     {"276", Config.GetVariationFromId("etag1", "276") },
                     {"277", Config.GetVariationFromId("etag1", "277") }
                 }
                },
                { "etag2", new Dictionary<string, object>
                 {
                     {"278", Config.GetVariationFromId("etag2", "278") },
                     {"279", Config.GetVariationFromId("etag2", "279") }
                 }
                },
                { "etag3", new Dictionary<string, object>
                 {
                     {"280", Config.GetVariationFromId("etag3", "280") },
                     {"281", Config.GetVariationFromId("etag3", "281") }
                 }
                },
                { "etag4", new Dictionary<string, object>
                 {
                     {"282", Config.GetVariationFromId("etag4", "282") },
                     {"283", Config.GetVariationFromId("etag4", "283") }
                 }
                }
            };

            Assert.True(TestData.CompareObjects(expectedVariationIdMap, Config.VariationIdMap));

            // Check Variation returns correct variable usage
            var featureVariableUsageInstance = new List<FeatureVariableUsage>
            {
                new FeatureVariableUsage{Id="155560", Value="F"},
                new FeatureVariableUsage{Id="155561", Value="red"},
            };

            var expectedVariationUsage = new Variation { Id = "122231", Key = "Fred", FeatureVariableUsageInstances = featureVariableUsageInstance, FeatureEnabled = true };
            var actualVariationUsage = Config.GetVariationFromKey("test_experiment_multivariate", "Fred");

            Assert.True(TestData.CompareObjects(expectedVariationUsage, actualVariationUsage));
            
            // Check Feature Key map.
            var expectedFeatureKeyMap = new Dictionary<string, FeatureFlag>
            {
                { "boolean_feature", Config.GetFeatureFlagFromKey("boolean_feature") },
                { "double_single_variable_feature", Config.GetFeatureFlagFromKey("double_single_variable_feature") },
                { "integer_single_variable_feature", Config.GetFeatureFlagFromKey("integer_single_variable_feature") },
                { "boolean_single_variable_feature", Config.GetFeatureFlagFromKey("boolean_single_variable_feature") },
                { "string_single_variable_feature", Config.GetFeatureFlagFromKey("string_single_variable_feature") },
                { "multi_variate_feature", Config.GetFeatureFlagFromKey("multi_variate_feature") },
                { "mutex_group_feature", Config.GetFeatureFlagFromKey("mutex_group_feature") },
                { "empty_feature", Config.GetFeatureFlagFromKey("empty_feature") },
                { "no_rollout_experiment_feature", Config.GetFeatureFlagFromKey("no_rollout_experiment_feature") },
                { "unsupported_variabletype", Config.GetFeatureFlagFromKey("unsupported_variabletype") }
            };

            Assert.True(TestData.CompareObjects(expectedFeatureKeyMap, Config.FeatureKeyMap));

            // Check Feature Key map.
            var expectedRolloutIdMap = new Dictionary<string, Rollout>
            {
                { "166660", Config.GetRolloutFromId("166660") },
                { "166661", Config.GetRolloutFromId("166661") }
            };

            Assert.True(TestData.CompareObjects(expectedRolloutIdMap, Config.RolloutIdMap));
        }

        [Fact]
        public void TestGetAccountId()
        {
            Assert.Equal("1592310167", Config.AccountId);
        }

        [Fact]
        public void TestGetProjectId()
        {
            Assert.Equal("7720880029", Config.ProjectId);
        }

        [Fact]
        public void TestGetGroupValidId()
        {
            var group = Config.GetGroup("7722400015");
            Assert.Equal("7722400015", group.Id);
            Assert.Equal("random", group.Policy);            
        }

        [Fact]
        public void TestGetGroupInvalidId()
        {
            var group = Config.GetGroup("invalid_id");

            LoggerMock.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()), Times.Exactly(1));
            LoggerMock.Verify(l => l.Log(LogLevel.ERROR, @"Group ID ""invalid_id"" is not in datafile."));

            ErrorHandlerMock.Verify(e => e.HandleError(
                It.Is<InvalidGroupException>(ex => ex.Message == "Provided group is not in datafile.")), 
                Times.Once, "Failed");

            Assert.True(TestData.CompareObjects(group, new Entity.Group()));
        }

        [Fact]
        public void TestGetExperimentValidKey()
        {
            var experiment = Config.GetExperimentFromKey("test_experiment");
            Assert.Equal("test_experiment", experiment.Key);
            Assert.Equal("7716830082", experiment.Id);
        }

        [Fact]
        public void TestGetExperimentInvalidKey()
        {
            var experiment = Config.GetExperimentFromKey("invalid_key");

            LoggerMock.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()), Times.Once);
            LoggerMock.Verify(l => l.Log(LogLevel.ERROR, @"Experiment key ""invalid_key"" is not in datafile."));

            ErrorHandlerMock.Verify(e => e.HandleError(It.Is<InvalidExperimentException>(ex => ex.Message == "Provided experiment is not in datafile.")));

            Assert.True(TestData.CompareObjects(new Entity.Experiment(), experiment));
        }

        [Fact]
        public void TestGetExperimentValidId()
        {
            var experiment = Config.GetExperimentFromId("7716830082");
            Assert.Equal("7716830082", experiment.Id);
            Assert.Equal("test_experiment", experiment.Key);
        }

        [Fact]
        public void TestGetExperimentInvalidId()
        {
            var experiment = Config.GetExperimentFromId("42");

            LoggerMock.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()), Times.Once);
            LoggerMock.Verify(l => l.Log(LogLevel.ERROR, @"Experiment ID ""42"" is not in datafile."));

            ErrorHandlerMock.Verify(e => e.HandleError(It.Is<InvalidExperimentException>(ex => ex.Message == "Provided experiment is not in datafile.")));

            Assert.True(TestData.CompareObjects(new Entity.Experiment(), experiment));
        }

        [Fact]
        public void TestGetEventValidKey()
        {
            var ev = Config.GetEvent("purchase");
            Assert.Equal("purchase", ev.Key);
            Assert.Equal("7718020063", ev.Id);

            Assert.True(TestData.CompareObjects(new object[] { "7716830082", "7723330021", "7718750065", "7716830585" }, ev.ExperimentIds));

        }

        [Fact]
        public void TestGetEventInvalidKey()
        {
            var ev = Config.GetEvent("invalid_key");

            LoggerMock.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()), Times.Once);
            LoggerMock.Verify(l => l.Log(LogLevel.ERROR, @"Event key ""invalid_key"" is not in datafile."));

            ErrorHandlerMock.Verify(e => e.HandleError(It.Is<InvalidEventException>(ex => ex.Message == "Provided event is not in datafile.")));

            Assert.True(TestData.CompareObjects(new Entity.Event(), ev));
        }

        [Fact]
        public void TestGetAudienceValidId()
        {
            var audience = Config.GetAudience("7718080042");

            Assert.Equal("7718080042", audience.Id);
            Assert.Equal("iPhone users in San Francisco", audience.Name);
        }

        [Fact]
        public void TestGetAudienceInvalidKey()
        {
            var audience = Config.GetAudience("invalid_id");

            LoggerMock.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()), Times.Once);
            LoggerMock.Verify(l => l.Log(LogLevel.ERROR, @"Audience ID ""invalid_id"" is not in datafile."));

            ErrorHandlerMock.Verify(e => e.HandleError(It.Is<InvalidAudienceException>(ex => ex.Message == "Provided audience is not in datafile.")));
            Assert.True(TestData.CompareObjects(new Entity.Audience(), audience));
        }

        [Fact]
        public void TestGetAttributeValidKey()
        {
            var attribute = Config.GetAttribute("device_type");

            Assert.Equal("device_type", attribute.Key);
            Assert.Equal("7723280020", attribute.Id);
        }

        [Fact]
        public void TestGetAttributeInvalidKey()
        {

            var attribute = Config.GetAttribute("invalid_key");

            LoggerMock.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()), Times.Once);
            LoggerMock.Verify(l => l.Log(LogLevel.ERROR, @"Attribute key ""invalid_key"" is not in datafile."));

            ErrorHandlerMock.Verify(e => e.HandleError(It.Is<InvalidAttributeException>(ex => ex.Message == "Provided attribute is not in datafile.")));
            Assert.Equal(new Entity.Attribute(), attribute);
        }

        /// <summary>
        /// EK = Experiment Key 
        /// VK = Variation Key
        /// </summary>
        [Fact]
        public void TestGetVariationFromKeyValidEKValidVK()
        {
            var variation = Config.GetVariationFromKey("test_experiment", "control");

            Assert.Equal("7722370027", variation.Id);
            Assert.Equal("control", variation.Key);
        }

        /// <summary>
        /// EK = Experiment Key 
        /// VK = Variation Key
        /// </summary>
        [Fact]
        public void TestGetVariationFromKeyValidEKInvalidVK()
        {
            var variation = Config.GetVariationFromKey("test_experiment", "invalid_key");

            LoggerMock.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()), Times.Once);
            LoggerMock.Verify(l => l.Log(LogLevel.ERROR, @"No variation key ""invalid_key"" defined in datafile for experiment ""test_experiment""."));

            ErrorHandlerMock.Verify(e => e.HandleError(It.Is<InvalidVariationException>(ex => ex.Message == "Provided variation is not in datafile.")));

            Assert.Equal(new Entity.Variation(), variation);


        }

        [Fact]
        public void TestGetVariationFromKeyInvalidEK()
        {
            var variation = Config.GetVariationFromKey("invalid_experiment", "control");

            LoggerMock.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()), Times.Once);
            LoggerMock.Verify(l => l.Log(LogLevel.ERROR, @"No variation key ""control"" defined in datafile for experiment ""invalid_experiment""."));

            ErrorHandlerMock.Verify(e => e.HandleError(It.Is<InvalidVariationException>(ex => ex.Message == "Provided variation is not in datafile.")));
            Assert.Equal(new Entity.Variation(), variation);
        }

        [Fact]
        public void TestGetVariationFromIdValidEKValidVId()
        {

            var variation = Config.GetVariationFromId("test_experiment", "7722370027");
            Assert.Equal("control", variation.Key);
            Assert.Equal("7722370027", variation.Id);
        }

        [Fact]
        public void TestGetVariationFromIdValidEKInvalidVId()
        {

            var variation = Config.GetVariationFromId("test_experiment", "invalid_id");

            LoggerMock.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()), Times.Once);
            LoggerMock.Verify(l => l.Log(LogLevel.ERROR, @"No variation ID ""invalid_id"" defined in datafile for experiment ""test_experiment""."));

            ErrorHandlerMock.Verify(e => e.HandleError(It.Is<InvalidVariationException>(ex => ex.Message == "Provided variation is not in datafile.")));
            Assert.Equal(new Entity.Variation(), variation);
        }

        [Fact]
        public void TestGetVariationFromIdInvalidEK()
        {
            var variation = Config.GetVariationFromId("invalid_experiment", "7722370027");

            LoggerMock.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()), Times.Once);
            LoggerMock.Verify(l => l.Log(LogLevel.ERROR, @"No variation ID ""7722370027"" defined in datafile for experiment ""invalid_experiment""."));

            ErrorHandlerMock.Verify(e => e.HandleError(It.Is<InvalidVariationException>(ex => ex.Message == "Provided variation is not in datafile.")));
            Assert.Equal(new Entity.Variation(), variation);
        }

        [Fact]
        public void TempProjectConfigTest()
        {
            ProjectConfig config = DatafileProjectConfig.Create(TestData.Datafile, new Mock<ILogger>().Object, new DefaultErrorHandler());
            Assert.NotNull(config);
            Assert.Equal("1592310167", config.AccountId);
        }

        // Test that getDatafile returns the expected datafile.
        [Fact]
        public void TestProjectConfigDatafileIsSame()
        {
            ProjectConfig config = DatafileProjectConfig.Create(TestData.Datafile, new Mock<ILogger>().Object, new DefaultErrorHandler());
            Assert.Equal(config.ToDatafile(), TestData.Datafile);
        }


        // test set/get forced variation for the following cases:
        //      - valid and invalid user ID
        //      - valid and invalid experiment key
        //      - valid and invalid variation key, null variation key

        [Fact]
        public void TestVariationFeatureEnabledProperty()
        {
            // Verify that featureEnabled property of variation is false if not defined.
            var variation = Config.GetVariationFromKey("test_experiment", "control");
            Assert.False(variation.IsFeatureEnabled);
        }

        [Fact]
        public void TestBotFilteringValues()
        {
            // Verify that bot filtering value is true as defined in Config data.
            Assert.True(Config.BotFiltering.GetValueOrDefault());

            // Remove botFilering node and verify returned value in null.
            JObject projConfig = JObject.Parse(TestData.Datafile);
            if (projConfig.TryGetValue("botFiltering", out JToken token))
            {
                projConfig.Property("botFiltering").Remove();
                var configWithoutBotFilter = DatafileProjectConfig.Create(JsonConvert.SerializeObject(projConfig),
                    LoggerMock.Object, ErrorHandlerMock.Object);

                // Verify that bot filtering is null when not defined in datafile.
                Assert.Null(configWithoutBotFilter.BotFiltering);
            }
        }

        [Fact]
        public void TestGetAttributeIdWithReservedPrefix()
        {
            // Verify that attribute key is returned for reserved attribute key.
            Assert.Equal(Config.GetAttributeId(ControlAttributes.USER_AGENT_ATTRIBUTE), ControlAttributes.USER_AGENT_ATTRIBUTE);

            // Verify that attribute Id is returned for attribute key with reserved prefix that does not exist in datafile.
            Assert.Equal("$opt_reserved_prefix_attribute", Config.GetAttributeId("$opt_reserved_prefix_attribute"));

            // Create config file copy with additional resered prefix attribute.
            string reservedPrefixAttrKey = "$opt_user_defined_attribute";
            JObject projConfig = JObject.Parse(TestData.Datafile);
            var attributes = (JArray)projConfig["attributes"];

            var reservedAttr = new Entity.Attribute { Id = "7723348204", Key = reservedPrefixAttrKey };
            attributes.Add((JObject)JToken.FromObject(reservedAttr));

            // Verify that attribute Id is returned and warning is logged for attribute key with reserved prefix that exists in datafile.
            var reservedAttrConfig = DatafileProjectConfig.Create(JsonConvert.SerializeObject(projConfig), LoggerMock.Object, ErrorHandlerMock.Object);
            Assert.Equal(reservedAttrConfig.GetAttributeId(reservedPrefixAttrKey), reservedAttrConfig.GetAttribute(reservedPrefixAttrKey).Id);
            LoggerMock.Verify(l => l.Log(LogLevel.WARN, $@"Attribute {reservedPrefixAttrKey} unexpectedly has reserved prefix {DatafileProjectConfig.RESERVED_ATTRIBUTE_PREFIX}; using attribute ID instead of reserved attribute name."));
        }

        [Fact]
        public void TestGetAttributeIdWithInvalidAttributeKey()
        {
            // Verify that null is returned when provided attribute key is invalid.
            Assert.Null(Config.GetAttributeId("invalid_attribute"));
            LoggerMock.Verify(l => l.Log(LogLevel.ERROR, @"Attribute key ""invalid_attribute"" is not in datafile."));
        }

        [Fact]
        public void TestCreateThrowsWithNullDatafile()
        {
            var exception = Assert.Throws<ConfigParseException>(() => DatafileProjectConfig.Create(null, null, null));
            Assert.Equal("Unable to parse null datafile.", exception.Message);
        }

        [Fact]
        public void TestCreateThrowsWithEmptyDatafile()
        {
            var exception = Assert.Throws<ConfigParseException>(() => DatafileProjectConfig.Create("", null, null));
            Assert.Equal("Unable to parse empty datafile.", exception.Message);
        }

        [Fact]
        public void TestCreateThrowsWithUnsupportedDatafileVersion()
        {
            var exception = Assert.Throws<ConfigParseException>(() => DatafileProjectConfig.Create(TestData.UnsupportedVersionDatafile, null, null));
            Assert.Equal($"This version of the C# SDK does not support the given datafile version: 5", exception.Message);
        }

        [Fact]
        public void TestCreateDoesNotThrowWithValidDatafile()
        {
            Record.Exception(() => DatafileProjectConfig.Create(TestData.Datafile, null, null));
        }

        [Fact]
        public void TestExperimentAudiencesRetrivedFromTypedAudiencesFirstThenFromAudiences()
        {
            var typedConfig = DatafileProjectConfig.Create(TestData.TypedAudienceDatafile, null, null);
            var experiment = typedConfig.GetExperimentFromKey("feat_with_var_test");

            var expectedAudienceIds = new string[] { "3468206642", "3988293898", "3988293899", "3468206646", "3468206647", "3468206644", "3468206643" };
            Assert.Equal(expectedAudienceIds, experiment.AudienceIds);
        }

        [Fact]
        public void TestIsFeatureExperimentReturnsFalseForExperimentThatDoesNotBelongToAnyFeature()
        {
            var typedConfig = DatafileProjectConfig.Create(TestData.TypedAudienceDatafile, null, null);
            var experiment = typedConfig.GetExperimentFromKey("typed_audience_experiment");

            Assert.False(typedConfig.IsFeatureExperiment(experiment.Id));
        }

        [Fact]
        public void TestIsFeatureExperimentReturnsTrueForExperimentThatBelongsToAFeature()
        {
            var typedConfig = DatafileProjectConfig.Create(TestData.TypedAudienceDatafile, null, null);
            var experiment = typedConfig.GetExperimentFromKey("feat2_with_var_test");

            Assert.True(typedConfig.IsFeatureExperiment(experiment.Id));
        }
    }
}