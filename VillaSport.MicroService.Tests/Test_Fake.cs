using System;
using Xunit;

namespace VillaSport.MicroService.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // arrange
            var number = 2;
            var expectedResult = 4;

            // act
            var add = Add(number, number);

            // assert
            Assert.Equal(add, expectedResult);
        }
        int Add(int x, int y)
        {
            return x + y;
        }
    }
}
