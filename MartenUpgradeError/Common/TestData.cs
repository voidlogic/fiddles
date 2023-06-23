using Newtonsoft.Json;

namespace Common
{
    public class TestData
    {
        public string Id { get; set; }

        [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
        public InnerTestData[] InnerTestDatas { get; set; }

        public static TestData GetTestData(int index)
        {
            var testData = new TestData
            {
                Id = $"testdata_{index}",
                InnerTestDatas = new[] {
                    new InnerTestData
                    {
                        InnerTestDataId = $"innertestdata_{index}",
                    },
                },
            };

            return testData;
        }
    }

    public class InnerTestData
    {
        public string InnerTestDataId { get; set; }
    }
}
