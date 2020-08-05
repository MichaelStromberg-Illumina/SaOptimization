using NirvanaCommon;
using Xunit;

namespace UnitTests.NirvanaCommon
{
    public class LongHashTableTests
    {
        [Fact]
        public void Nominal()
        {
            var hashTable = new LongHashTable();
            hashTable.Add(10);
            hashTable.Add(20);
            hashTable.Add(30);
            hashTable.Add(40);
            hashTable.Add(20);
            hashTable.Add(30);
            hashTable.Add(30);
            hashTable.Add(11);
            
            Assert.Equal(5, hashTable.Count);
            
            Assert.True(hashTable.Contains(10));
            Assert.True(hashTable.Contains(11));
            Assert.True(hashTable.Contains(20));
            Assert.True(hashTable.Contains(30));
            Assert.True(hashTable.Contains(40));
            
            Assert.False(hashTable.Contains(0));
            Assert.False(hashTable.Contains(12));
            Assert.False(hashTable.Contains(50));
        }
    }
}