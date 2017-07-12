using Newtonsoft.Json;
using NUnit.Framework;
using Pulsar4X.ECSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pulsar4X.Tests
{
    [TestFixture]
    [Description("DataBlob Tests")]
    internal class DataBlobTests
    {
        private static readonly List<Type> DataBlobTypes = new List<Type>(Assembly.GetAssembly(typeof(BaseDataBlob)).GetTypes().Where(type => type.IsSubclassOf(typeof(BaseDataBlob)) && !type.IsAbstract));

        [Test]
        public void TypeCount()
        {
            // This is mostly a test for the EntityManager, but is included here because we do DataBlobType reflection here.
            // EntityManager does the same reflection for some of its functions.
            Assert.AreEqual(DataBlobTypes.Count, EntityManager.BlankDataBlobMask().Length);
        }

        /// <summary>
        /// This test ensures our DataBlobs can be created by Json during deserialization.
        /// Any type that fails this test cannot be instantiated by Json, and thus will throw an exception if you try to deserialize it.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(DataBlobTypes))]
        public void JsonConstructor(Type dataBlobType)
        {
            ConstructorInfo[] constructors = dataBlobType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Attribute jsonConstructorAttribute = new JsonConstructorAttribute();

            if (constructors.Any(constructorInfo => constructorInfo.GetCustomAttributes().Contains(jsonConstructorAttribute)))
            {
                // Test for any constructor marked with a [JsonConstructor] attribute.
                Assert.Pass(dataBlobType + " will deserialize with the constructor marked with [JsonConstructor]");
            }

            if (constructors.Any(constructorInfo => constructorInfo.GetParameters().Length == 0 && constructorInfo.IsPublic))
            {
                // Test for a public constructor with no parameters.
                Assert.Pass(dataBlobType + " will deserialize with the default parameterless constructor.");
            }

            if (constructors.Length == 1)
            {
                if (constructors[0].GetParameters().Length != 0)
                {
                    // Test the datablob to see if it has only 1 constructor, and that constructor has parameters.
                    Assert.Pass(dataBlobType + " will deserialize with the only parametrized constructor available. Make sure parameters match the Json property names saved in the Json file.");
                }
            }

            if (constructors.Any(constructorInfo => constructorInfo.GetParameters().Length == 0 && constructorInfo.IsPrivate))
            {
                // Test if the datablob has a private constructor with no parameters (JSON can use a private constructor, though undesirable)
                Assert.Pass(dataBlobType + " will deserialize with the private default parameterless constructor.");
            }

            // No constructors exist for this datablob that JSON can use to instantiate this datablob type during deserialization.
            Assert.Fail(dataBlobType + " does not have a Json constructor");
        }

        [Test]
        [TestCaseSource(nameof(DataBlobTypes))]
        public void AccessibilityTest(Type dataBlobType)
        {
            if (dataBlobType.IsAbstract)
            {
                return;
            }
            if (!dataBlobType.IsPublic)
            {
                Assert.IsNotNull(dataBlobType.GetCustomAttribute<TestUseOnlyAttribute>(true), "DataBlob is not public");
            }

            foreach (PropertyInfo property in dataBlobType.GetProperties())
            {
                if (property.CanRead)
                {
                    if (property.SetMethod != null)
                    {
                        Assert.IsFalse(property.SetMethod.IsAssembly, $"Interal Set Property {property.Name}");
                    }
                }
            }
        }
    }
}