//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Injectable.Tests
//{
//    public class EmbeddedResourceBuilderTests
//    {
//        [Fact]
//        public void Should_LoadString_When_ResourceFound()
//        {
//            var text = EmbeddedResourceReader.ReadAsString(this.GetType().Assembly, "Injectable.Tests.Resources.TestResource.txt");
//            Assert.Equal("Embedded Test Data", text);
//        }

//        [Fact]
//        public void HasEmbeddedResource()
//        {
//            var resourceNames = this.GetType().Assembly.GetManifestResourceNames();
//            Assert.NotEmpty(resourceNames);
//        }
//    }
//}
